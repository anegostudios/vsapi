using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    public delegate double WalkSpeedSupplierDelegate();

    public class AnimFrameCallback
    {
        public float Frame;
        public string Animation;
        public Action Callback;
    }

    /// <summary>
    /// Syncs every frame with entity.ActiveAnimationsByAnimCode, starts, progresses and stops animations when necessary 
    /// </summary>
    public abstract class AnimatorBase : IAnimator
    {
        public static readonly float[] identMat = Mat4f.Create();
        public static readonly HashSet<string> logAntiSpam = new HashSet<string>();

        WalkSpeedSupplierDelegate WalkSpeedSupplier;
        Action<string> onAnimationStoppedListener = null;
        protected int activeAnimCount = 0;

        public ShapeElement[] RootElements;
        public List<ElementPose> RootPoses;
        /// <summary>
        /// A RunningAnimation object for each of the possible Animations for this object
        /// </summary>
        public RunningAnimation[] anims;
        private readonly Dictionary<string, RunningAnimation> animsByCode;   // For performance, allows quick lookups instead of enumerating the whole anims array
        /// <summary>
        /// We skip the last row - https://stackoverflow.com/questions/32565827/whats-the-purpose-of-magic-4-of-last-row-in-matrix-4x4-for-3d-graphics 
        /// </summary>
        public float[] TransformationMatrices;
        /// <summary>
        /// The entity's default pose. Meaning for most elements this is the identity matrix, with exception of individually controlled elements such as the head.
        /// </summary>
        public float[] TransformationMatricesDefaultPose;
        public Dictionary<string, AttachmentPointAndPose> AttachmentPointByCode = new Dictionary<string, AttachmentPointAndPose>();// StringComparer.OrdinalIgnoreCase); - breaks fp hands for some reason
        public RunningAnimation[] CurAnims = new RunningAnimation[20];
        private readonly List<RunningAnimation> activeOrRunning = new(2);           // For performance, a short list of the anims which are currently or were recently Active or Running. We add to this List in the one place where .Active state is set to true
        public Entity entityForLogging;

        public bool CalculateMatrices { get; set; } = true;

        public float[] Matrices
        {
            get {
                return activeAnimCount > 0 ? TransformationMatrices : TransformationMatricesDefaultPose;
            }
        }

        public int ActiveAnimationCount
        {
            get { return activeAnimCount; }
        }

        [Obsolete("Use Animations instead")]
        public RunningAnimation[] RunningAnimations => Animations;
        public RunningAnimation[] Animations => anims;

        public abstract int MaxJointId { get; }

        public RunningAnimation GetAnimationState(string code)
        {
            if (code == null) return null;
            animsByCode.TryGetValue(code.ToLowerInvariant(), out var anim);
                    return anim;
                }

        public AnimatorBase(WalkSpeedSupplierDelegate WalkSpeedSupplier, Animation[] Animations, Action<string> onAnimationStoppedListener = null)
        {
            this.WalkSpeedSupplier = WalkSpeedSupplier;
            this.onAnimationStoppedListener = onAnimationStoppedListener;

            var anims = this.anims = Animations == null ? Array.Empty<RunningAnimation>() : new RunningAnimation[Animations.Length];
            animsByCode = new(anims.Length);
            var animsByCodeLocal = animsByCode;

            for (int i = 0; i < anims.Length; i++)
            {
                var Anim = Animations[i];
                Anim.Code = Anim.Code.ToLowerInvariant();

                RunningAnimation newAnim = new RunningAnimation()
                {
                    //Active = false,            // No need to specify the default values
                    //Running = false,
                    //CurrentFrame = 0,
                    Animation = Anim
                };
                anims[i] = newAnim;
                animsByCodeLocal[Anim.Code] = newAnim;
            }
        }

        float accum = 0.25f;
        double walkSpeed;

        public virtual void OnFrame(Dictionary<string, AnimationMetaData> activeAnimationsByAnimCode, float dt)
        {
            activeAnimCount = 0;

            if ((accum += dt) > 0.25f)
            {
                walkSpeed = WalkSpeedSupplier == null ? 1f : WalkSpeedSupplier();
                accum = 0;
            }
            
            string missingAnimCode = null;
            foreach (var code in activeAnimationsByAnimCode.Keys)
            {
                if (!animsByCode.TryGetValue(code.ToLowerInvariant(), out RunningAnimation anim)) { missingAnimCode = code; continue; }
                // Animation got started
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

            var activeAnims = this.activeOrRunning;
            for (int i = activeAnims.Count - 1; i >= 0; i--)
            {
                RunningAnimation anim = activeAnims[i];
                if (anim.Active && !activeAnimationsByAnimCode.ContainsKey(anim.Animation.Code))   // wasActive and now should not be active because it is no longer in activeAnimationsByCode
                {
                    // Animation got stopped
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

                if (anim.Running)               // Note: an animation may still need to be running even after it is no longer active, if it has Rewind / PlayTillEnd etc
                {
                    if (!ProgressRunningAnimation(anim, dt))
                    {
                        var Code = anim.Animation.Code;
                        activeAnimationsByAnimCode.Remove(Code);
                        onAnimationStoppedListener?.Invoke(Code);
                    }
                }

                if (!anim.Active && !anim.Running)
                {
                    activeAnims.RemoveAt(i);   // No CME or other problems, as we are counting through the list backwards starting at the end, remove only affects this and the ones later than it
                }
            }

            calculateMatrices(dt);
        }

        /// <summary>
        /// The return value of false indicates the animation stopped, and requires removal from activeAnimationsByAnimCode this tick
        /// (it could also stop and be removed NEXT tick...)
        /// </summary>
        /// <param name="anim"></param>
        /// <param name="dt"></param>
        /// <returns>False if the animation should immediately stop; true otherwise</returns>
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
                if (onAnimationEnd == EnumEntityAnimationEndHandling.Stop || onAnimationEnd == EnumEntityAnimationEndHandling.EaseOut)
                    {
                    return false;
                    }
                return true;
                }

            CurAnims[activeAnimCount++] = anim;

            if (anim.Iterations != 0 && ((!anim.Active && onAnimationEnd == EnumEntityAnimationEndHandling.Hold) || (onAnimationEnd == EnumEntityAnimationEndHandling.EaseOut)))
                {
                    anim.EaseOut(dt);
                }

                anim.Progress(dt, (float)walkSpeed);
            return true;
        }


        public virtual string DumpCurrentState()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < anims.Length; i++)
            {
                RunningAnimation anim = anims[i];

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

        protected abstract void calculateMatrices(float dt);
        

        public AttachmentPointAndPose GetAttachmentPointPose(string code)
        {
            AttachmentPointByCode.TryGetValue(code, out AttachmentPointAndPose apap);
            return apap;
        }

        public virtual ElementPose GetPosebyName(string name, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            throw new NotImplementedException();
        }

        public virtual void ReloadAttachmentPoints()
        {
            throw new NotImplementedException();
        }
    }
}
