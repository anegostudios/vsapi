using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public class AttachmentPointAndPose
    {
        public float[] AnimModelMatrix;

        public ElementPose CachedPose;
        public AttachmentPoint AttachPoint;

        public AttachmentPointAndPose()
        {
            AnimModelMatrix = Mat4f.Create();
        }
    }


    public interface IAnimator
    {
        /// <summary>
        /// The 30 pose transformation matrices that go to the shader
        /// </summary>
        float[] Matrices { get; }

        /// <summary>
        /// Amount of currently active animations
        /// </summary>
        int ActiveAnimationCount { get; }


        AttachmentPointAndPose GetAttachmentPointPose(string code);

        void OnFrame(Dictionary<string, AnimationMetaData> activeAnimationsByAnimCode, float dt);
    }

    /// <summary>
    /// Everything needed for allowing animations the <see cref="Entity"/> class holds a reference to an IAnimator. 
    /// Currently implemented by <see cref="ServerAnimator"/>
    /// </summary>
    public interface IAnimationManager
    {
        IAnimator Animator { get; set; }
        EntityHeadController HeadController { get; set; }

        void Init(ICoreAPI api, Entity entity, Shape shape);

        
        bool AnimationsDirty { get; set; }
        
        bool StartAnimation(AnimationMetaData animdata);
        bool StartAnimation(string configCode);
        void StopAnimation(string code);

        void FromAttributes(ITreeAttribute tree);
        void ToAttributes(ITreeAttribute tree);


        Dictionary<string, AnimationMetaData> ActiveAnimationsByAnimCode { get; }

        void OnReceivedServerAnimations(int[] activeAnimations, int activeAnimationsCount, float[] activeAnimationSpeeds);

        void Dispose();
    }
}
