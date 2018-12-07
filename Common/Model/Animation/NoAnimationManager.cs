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
    public class NoAnimator : IAnimator
    {
        public float[] Matrices => null;

        public int ActiveAnimationCount => 0;

        public AttachmentPointAndPose GetAttachmentPointPose(string code)
        {
            return null;
        }

        public void OnFrame(Dictionary<string, AnimationMetaData> activeAnimationsByAnimCode, float dt)
        {
            
        }
    }

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

        public void FromAttributes(ITreeAttribute tree)
        {
            
        }

        public void Init(ICoreAPI api, Entity entity, Shape shape)
        {
            
        }

        public void OnReceivedServerAnimations(int[] activeAnimations, int activeAnimationsCount, float[] activeAnimationSpeeds)
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

        public void ToAttributes(ITreeAttribute tree)
        {
            
        }
    }
}
