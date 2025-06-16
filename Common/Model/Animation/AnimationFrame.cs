using System;
using System.Collections.Generic;

#nullable disable

namespace Vintagestory.API.Common
{
    // https://youtu.be/cieheqt7eqc?t=11m51s
    public class AnimationFrame
    {
        /// <summary>
        /// The frame number.
        /// </summary>
        public int FrameNumber;

        /// <summary>
        /// The transformations for the root element of the frame.
        /// </summary>
        public List<ElementPose> RootElementTransforms = new List<ElementPose>();
        

        /// <summary>
        /// Sets the transform of a particular joint ID.
        /// </summary>
        /// <param name="jointId"></param>
        /// <param name="modelTransform"></param>
        [Obsolete("Does nothing in 1.20.11 - actually it had no useful effect even before 1.20")]
        public void SetTransform(int jointId, float[] modelTransform)
        {
        }



        /// <summary>
        /// Finalizes the matricies with joints assigned by their ID.
        /// </summary>
        /// <param name="jointsById"></param>
        [Obsolete("Does nothing in 1.20.11 - actually it had no useful effect even before 1.20")]
        public void FinalizeMatrices(Dictionary<int, AnimationJoint> jointsById)
        {
        }
    }
}
