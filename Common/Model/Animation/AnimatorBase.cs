using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common;

#nullable enable

public delegate double WalkSpeedSupplierDelegate();

public class AnimFrameCallback
{
    public float Frame;
    public string? Animation;
    public Action? Callback;
}

/// <summary>
/// Syncs every frame with entity.ActiveAnimationsByAnimCode, starts, progresses and stops animations when necessary 
/// </summary>
public abstract class AnimatorBase : IAnimator
{
    public const int DEFAULT_MAX_ANIM = 1;
    protected RunningAnimation[] curAnims = new RunningAnimation[DEFAULT_MAX_ANIM];

    public static readonly float[] identMat = Mat4f.Create();
    public static readonly HashSet<string> logAntiSpam = [];

    private readonly WalkSpeedSupplierDelegate? walkSpeedSupplier;
    private readonly Action<string>? onAnimationStoppedListener = null;

    public ShapeElement[]? RootElements;
    public List<ElementPose>? RootPoses;

    private float accum = 0.25f;
    private double walkSpeed;

    /// <summary>
    /// We skip the last row - https://stackoverflow.com/questions/32565827/whats-the-purpose-of-magic-4-of-last-row-in-matrix-4x4-for-3d-graphics 
    /// </summary>
    protected float[]? transformationMatrices;

    /// <summary>
    /// The entity's default pose. Meaning for most elements this is the identity matrix, with exception of individually controlled elements such as the head.
    /// </summary>
    protected float[]? transformationMatricesDefaultPose;

    // Always assume these were filled out at some point.
    public float[] Matrices => ActiveAnimationCount > 0 ? transformationMatrices! : transformationMatricesDefaultPose!;

    /// <summary>
    /// All possible animations mapped to their code.
    /// </summary>
    private readonly Dictionary<string, RunningAnimation> animsByCode = [];

    /// <summary>
    /// All attachment points mapped to their code.
    /// </summary>
    protected Dictionary<string, AttachmentPointAndPose> attachmentPointByCode = []; // StringComparer.OrdinalIgnoreCase); - breaks fp hands for some reason.

    private readonly List<RunningAnimation> activeOrRunning = new(2); // For performance, a short list of the anims which are currently or were recently Active or Running. We add to this List in the one place where .Active state is set to true.

    public bool CalculateMatrices { get; set; } = true;

    public int ActiveAnimationCount { get; protected set; }

    /// <summary>
    /// A RunningAnimation object for each of the possible Animations for this object.
    /// </summary>
    protected RunningAnimation[] allEntityAnimations = [];

    [Obsolete("Use Animations instead")]
    public RunningAnimation[] RunningAnimations => Animations;
    public RunningAnimation[] Animations => allEntityAnimations;

    public abstract int MaxJointId { get; }
    protected Entity? entityForLogging;

    public RunningAnimation? GetAnimationState(string? code)
    {
        if (code == null) return null;
        animsByCode.TryGetValue(code.ToLowerInvariant(), out RunningAnimation? anim);
        return anim;
    }

    public AnimatorBase(WalkSpeedSupplierDelegate walkSpeedSupplier, Animation[]? allEntityAnimations, Action<string>? onAnimationStoppedListener = null)
    {
        this.walkSpeedSupplier = walkSpeedSupplier;
        this.onAnimationStoppedListener = onAnimationStoppedListener;

        this.allEntityAnimations = allEntityAnimations == null ? [] : new RunningAnimation[allEntityAnimations.Length];
        Dictionary<string, RunningAnimation> animsByCodeLocal = animsByCode;

        if (allEntityAnimations == null) return;

        for (int i = 0; i < this.allEntityAnimations.Length; i++)
        {
            Animation anim = allEntityAnimations[i];

            // Lol checking all the animation codes for lowercase EVERY TIME an entity is made.
            anim.Code = anim.Code.ToLowerInvariant();

            RunningAnimation newAnim = new()
            {
                Animation = anim
            };

            this.allEntityAnimations[i] = newAnim;
            animsByCodeLocal[anim.Code] = newAnim;
        }
    }

    protected abstract void IncreaseAnimationCapacity();

    public virtual void OnFrame(Dictionary<string, AnimationMetaData> activeAnimationsByAnimCode, float dt)
    {
        ActiveAnimationCount = 0;

        if ((accum += dt) > 0.25f)
        {
            walkSpeed = walkSpeedSupplier == null ? 1f : walkSpeedSupplier();
            accum = 0;
        }

        string? missingAnimCode = null;
        foreach (string code in activeAnimationsByAnimCode.Keys)
        {
            if (!animsByCode.TryGetValue(code.ToLowerInvariant(), out RunningAnimation? anim)) { missingAnimCode = code; continue; }

            // Animation got started.
            if (!anim.Active)
            {
                AnimNowActive(anim, activeAnimationsByAnimCode[code]);
            }
        }
        if (missingAnimCode != null)
        {
            activeAnimationsByAnimCode.Remove(missingAnimCode);
            if (entityForLogging != null)
            {
                string entityCode = entityForLogging.Code.ToShortString();
                string logRecord = entityCode + "|" + missingAnimCode;
                if (logAntiSpam.Add(logRecord))
                {
                    entityForLogging.World.Logger.Debug(entityCode + " attempted to play an animation code which its shape does not have: \"" + missingAnimCode + "\"");
                }
            }
        }

        List<RunningAnimation> activeAnims = activeOrRunning;
        while (activeAnims.Count > curAnims.Length)
        {
            IncreaseAnimationCapacity();
        }

        for (int i = activeAnims.Count - 1; i >= 0; i--)
        {
            RunningAnimation anim = activeAnims[i];
            if (anim.Active && !activeAnimationsByAnimCode.ContainsKey(anim.Animation.Code)) // wasActive and now should not be active because it is no longer in activeAnimationsByCode.
            {
                // Animation got stopped.
                anim.Active = false;

                EnumEntityActivityStoppedHandling onActivityStop = anim.Animation.OnActivityStopped;
                if (onActivityStop == EnumEntityActivityStoppedHandling.Rewind)
                {
                    anim.ShouldRewind = true;
                }

                if (onActivityStop == EnumEntityActivityStoppedHandling.Stop)
                {
                    anim.Stop();
                    onAnimationStoppedListener?.Invoke(anim.Animation.Code);
                }

                if (onActivityStop == EnumEntityActivityStoppedHandling.PlayTillEnd)
                {
                    anim.ShouldPlayTillEnd = true;
                }
            }

            if (anim.Running) // Note: an animation may still need to be running even after it is no longer active, if it has Rewind / PlayTillEnd etc.
            {
                if (!ProgressRunningAnimation(anim, dt))
                {
                    string Code = anim.Animation.Code;
                    activeAnimationsByAnimCode.Remove(Code);
                    onAnimationStoppedListener?.Invoke(Code);
                }
            }

            if (!anim.Active && !anim.Running)
            {
                activeAnims.RemoveAt(i); // No CME or other problems, as we are counting through the list backwards starting at the end, remove only affects this and the ones later than it.
            }
        }

        CalculateOutputMatrices(dt);
    }

    /// <summary>
    /// The return value of false indicates the animation stopped, and requires removal from activeAnimationsByAnimCode this tick.
    /// (it could also stop and be removed NEXT tick...)
    /// </summary>
    /// <returns>False if the animation should immediately stop; true otherwise.</returns>
    private bool ProgressRunningAnimation(RunningAnimation anim, float dt)
    {
        EnumEntityAnimationEndHandling onAnimationEnd = anim.Animation.OnAnimationEnd;
        EnumEntityActivityStoppedHandling onActivityStopped = anim.Animation.OnActivityStopped;

        bool shouldStop = false;

        if (anim.Iterations > 0)
        {
            shouldStop =
                (onAnimationEnd == EnumEntityAnimationEndHandling.Stop) ||
                (!anim.Active && (onActivityStopped == EnumEntityActivityStoppedHandling.PlayTillEnd || onActivityStopped == EnumEntityActivityStoppedHandling.EaseOut) && anim.EasingFactor < 0.002f) ||
                (onAnimationEnd == EnumEntityAnimationEndHandling.EaseOut && anim.EasingFactor < 0.002f)
            ;
        }
        else if (anim.Iterations < 0)
        {
            shouldStop = !anim.Active && onActivityStopped == EnumEntityActivityStoppedHandling.Rewind && anim.EasingFactor < 0.002f;
        }

        if (shouldStop)
        {
            anim.Stop();
            return onAnimationEnd is not EnumEntityAnimationEndHandling.Stop and not EnumEntityAnimationEndHandling.EaseOut;
        }

        curAnims[ActiveAnimationCount++] = anim;

        if (anim.Iterations != 0 && ((!anim.Active && onAnimationEnd == EnumEntityAnimationEndHandling.Hold) || (onAnimationEnd == EnumEntityAnimationEndHandling.EaseOut)))
        {
            anim.EaseOut(dt);
        }

        anim.Progress(dt, (float)walkSpeed);
        return true;
    }

    public virtual string DumpCurrentState()
    {
        StringBuilder sb = new();

        for (int i = 0; i < allEntityAnimations.Length; i++)
        {
            RunningAnimation anim = allEntityAnimations[i];

            if (anim.Active && anim.Running) sb.Append("Active&Running: " + anim.Animation.Code);
            else if (anim.Active) sb.Append("Active: " + anim.Animation.Code);
            else if (anim.Running) sb.Append("Running: " + anim.Animation.Code);
            else continue;

            sb.Append(", easing: " + anim.EasingFactor);
            sb.Append(", currentframe: " + anim.CurrentFrame);
            sb.Append(", iterations: " + anim.Iterations);
            sb.Append(", blendedweight: " + anim.BlendedWeight);
            sb.Append(", animmetacode: " + anim.meta.Code);
            sb.AppendLine();
        }

        return sb.ToString();
    }

    protected virtual void AnimNowActive(RunningAnimation anim, AnimationMetaData animData)
    {
        anim.Running = true;
        anim.Active = true;
        if (!activeOrRunning.Contains(anim)) activeOrRunning.Add(anim);
        anim.meta = animData;
        anim.ShouldRewind = false;
        anim.ShouldPlayTillEnd = false;
        anim.CurrentFrame = animData.StartFrameOnce;
        animData.StartFrameOnce = 0;
    }

    protected abstract void CalculateOutputMatrices(float dt);

    public AttachmentPointAndPose? GetAttachmentPointPose(string code)
    {
        attachmentPointByCode.TryGetValue(code, out AttachmentPointAndPose? apap);
        return apap;
    }

    public virtual ElementPose? GetPosebyName(string name, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
    {
        throw new NotImplementedException();
    }

    public virtual void ReloadAttachmentPoints()
    {
        throw new NotImplementedException();
    }
}
