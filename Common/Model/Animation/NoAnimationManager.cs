using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

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

        public RunningAnimation[] RunningAnimations => new RunningAnimation[0];

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

        /// <summary>
        /// The event fired when a specified frame has been hit.
        /// </summary>
        /// <param name="activeAnimationsByAnimCode"></param>
        /// <param name="dt"></param>
        public void OnFrame(Dictionary<string, AnimationMetaData> activeAnimationsByAnimCode, float dt)
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

        public void Dispose()
        {
            
        }

        public void FromAttributes(ITreeAttribute tree, string version)
        {
            
        }

        public void Init(ICoreAPI api, Entity entity)
        {
            
        }

        public bool IsAnimationActive(params string[] anims)
        {
            return false;
        }

        public void OnAnimationStopped(string code)
        {
            
        }

        public void OnReceivedServerAnimations(int[] activeAnimations, int activeAnimationsCount, float[] activeAnimationSpeeds)
        {
            
        }

        public void OnServerTick(float dt)
        {
            
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

        public void ToAttributes(ITreeAttribute tree, bool forClient)
        {
            
        }
    }
}
