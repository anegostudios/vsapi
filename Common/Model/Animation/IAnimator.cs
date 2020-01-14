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
        /// <summary>
        /// The current model matrix for this attachment point for this entity for the current animation frame.
        /// </summary>
        public float[] AnimModelMatrix;

        /// <summary>
        /// The pose shared across all entities using the same shape. Don't use. It's used internally for calculating the animation state. Once calculated, the value is copied over to AnimModelMatrix
        /// </summary>
        public ElementPose CachedPose;

        /// <summary>
        /// The attachment point
        /// </summary>
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

        /// <summary>
        /// Gets the attachment point pose.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        AttachmentPointAndPose GetAttachmentPointPose(string code);

        /// <summary>
        /// The event fired on each frame.
        /// </summary>
        /// <param name="activeAnimationsByAnimCode"></param>
        /// <param name="dt"></param>
        void OnFrame(Dictionary<string, AnimationMetaData> activeAnimationsByAnimCode, float dt);
    }

    /// <summary>
    /// Everything needed for allowing animations the <see cref="Entity"/> class holds a reference to an IAnimator. 
    /// Currently implemented by <see cref="ServerAnimator"/>
    /// </summary>
    public interface IAnimationManager
    {
        /// <summary>
        /// The animator for this animation manager
        /// </summary>
        IAnimator Animator { get; set; }

        /// <summary>
        /// The head controller for this manager.
        /// </summary>
        EntityHeadController HeadController { get; set; }

        /// <summary>
        /// Initialization call for the animation manager.
        /// </summary>
        /// <param name="api">The core API</param>
        /// <param name="entity">The entity being animated.</param>
        void Init(ICoreAPI api, Entity entity);

        /// <summary>
        /// Whether or not the animation is dirty.
        /// </summary>
        bool AnimationsDirty { get; set; }

        bool IsAnimationActive(params string[] anims);

        /// <summary>
        /// Starts an animation based on the AnimationMetaData
        /// </summary>
        /// <param name="animdata"></param>
        /// <returns></returns>
        bool StartAnimation(AnimationMetaData animdata);

        /// <summary>
        /// Starts an animation based on JSON code.
        /// </summary>
        /// <param name="configCode">The json code.</param>
        /// <returns></returns>
        bool StartAnimation(string configCode);

        /// <summary>
        /// Stops the animation.
        /// </summary>
        /// <param name="code">The code to stop the animation on</param>
        void StopAnimation(string code);

        /// <summary>
        /// Additional attributes applied to the animation
        /// </summary>
        /// <param name="tree"></param>
        void FromAttributes(ITreeAttribute tree);

        /// <summary>
        /// Additional attributes applied from the animation
        /// </summary>
        /// <param name="tree"></param>
        void ToAttributes(ITreeAttribute tree, bool forClient);

        /// <summary>
        /// Gets the AnimationMetaData for the target action.
        /// </summary>
        Dictionary<string, AnimationMetaData> ActiveAnimationsByAnimCode { get; }

        /// <summary>
        /// The event fired when the client recieves the server animations
        /// </summary>
        /// <param name="activeAnimations">all of active animations</param>
        /// <param name="activeAnimationsCount">the number of the animations</param>
        /// <param name="activeAnimationSpeeds">The speed of those animations.</param>
        void OnReceivedServerAnimations(int[] activeAnimations, int activeAnimationsCount, float[] activeAnimationSpeeds);

        /// <summary>
        /// Safe dispose method for classes containing IAnimator
        /// </summary>
        void Dispose();

        /// <summary>
        /// The event fired when the animation is stopped.
        /// </summary>
        /// <param name="code">The code that the animation stopped with.</param>
        void OnAnimationStopped(string code);

        void OnServerTick(float dt);
    }
}
