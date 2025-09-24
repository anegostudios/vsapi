using System;
using System.Collections.Generic;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// A NoAnimator built off of <see cref="IAnimator"/>
    /// </summary>
    public class NoAnimator : IAnimator
    {
        /// <summary>
        /// The matrices for this No-Animator
        /// </summary>
        public float[] Matrices => null;

        /// <summary>
        /// The active animation count for this no animator.
        /// </summary>
        public int ActiveAnimationCount => 0;

        public bool CalculateMatrices { get; set; }

        public RunningAnimation[] Animations => Array.Empty<RunningAnimation>();

        public int MaxJointId => throw new NotImplementedException();

        public string DumpCurrentState()
        {
            throw new NotImplementedException();
        }

        public RunningAnimation GetAnimationState(string code)
        {
            return null;
        }

        /// <summary>
        /// Gets the attachment point for this pose.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public AttachmentPointAndPose GetAttachmentPointPose(string code)
        {
            return null;
        }

        public ElementPose GetPosebyName(string name, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            return null;
        }

        /// <summary>
        /// The event fired when a specified frame has been hit.
        /// </summary>
        /// <param name="activeAnimationsByAnimCode"></param>
        /// <param name="dt"></param>
        public void OnFrame(Dictionary<string, AnimationMetaData> activeAnimationsByAnimCode, float dt)
        {
        }

        public void ReloadAttachmentPoints()
        {
            
        }
    }

    /// <summary>
    /// A No-Animation Manager built off of <see cref="IAnimationManager"/>.
    /// </summary>
    public class NoAnimationManager : IAnimationManager
    {

        public NoAnimationManager()
        {
            Animator = new NoAnimator();
        }

        public IAnimator Animator { get; set; }

        public bool AnimationsDirty { get; set; }


        public Dictionary<string, AnimationMetaData> ActiveAnimationsByAnimCode => new Dictionary<string, AnimationMetaData>();

        public EntityHeadController HeadController { get; set; }

        public bool AdjustCollisionBoxToAnimation => throw new NotImplementedException();

        public event StartAnimationDelegate OnStartAnimation;
        public event Action<string> OnAnimationStopped;
        public event StartAnimationDelegate OnAnimationReceived;

        public void CopyOverAnimStates(RunningAnimation[] copyOverAnims, IAnimator animator)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            
        }

        public void FromAttributes(ITreeAttribute tree, string version)
        {
            
        }

        public RunningAnimation GetAnimationState(string anim)
        {
            throw new NotImplementedException();
        }

        public void Init(ICoreAPI api, Entity entity)
        {
            
        }

        public bool IsAnimationActive(params string[] anims)
        {
            return false;
        }

        public IAnimator LoadAnimator(ICoreAPI api, Entity entity, Shape entityShape, RunningAnimation[] copyOverAnims, bool requirePosesOnServer, params string[] requireJointsForElements)
        {
            throw new NotImplementedException();
        }

        public void TriggerAnimationStopped(string code)
        {
            
        }

        public void OnClientFrame(float dt)
        {
            
        }

        public void OnReceivedServerAnimations(int[] activeAnimations, int activeAnimationsCount, float[] activeAnimationSpeeds)
        {
            
        }

        public void OnServerTick(float dt)
        {
            
        }

        public void RegisterFrameCallback(AnimFrameCallback trigger)
        {
            throw new NotImplementedException();
        }

        public void ResetAnimation(string beginholdAnim)
        {
        }

        public bool TryStartAnimation(AnimationMetaData animdata)
        {
            return false;
        }

        public bool StartAnimation(AnimationMetaData animdata)
        {
            return false;
        }

        public bool StartAnimation(string configCode)
        {
            return false;
        }

        public void StopAnimation(string code)
        {
            
        }

        public void StopAllAnimations()
        {

        }

        public void ToAttributes(ITreeAttribute tree, bool forClient)
        {
            
        }

        public void ShouldPlaySound(AnimationSound sound)
        {
            
        }
    }
}
