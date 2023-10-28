using System.Collections.Generic;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

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
        /// The transformations for this frame.
        /// </summary>
        public float[][] animTransforms = new float[GlobalConstants.MaxAnimatedElements][];

        /// <summary>
        /// The transformation matricies for this frame
        /// </summary>
        public float[] transformationMatrices = new float[16 * GlobalConstants.MaxAnimatedElements];

        /// <summary>
        /// The transformations for the root element of the frame.
        /// </summary>
        public List<ElementPose> RootElementTransforms = new List<ElementPose>();
        

        public AnimationFrame()
        {
            for (int i = 0; i < animTransforms.Length; i++)
            {
                animTransforms[i] = Mat4f.Create();
            }
            
        }

        /// <summary>
        /// Sets the transform of a particular joint ID.
        /// </summary>
        /// <param name="jointId"></param>
        /// <param name="modelTransform"></param>
        public void SetTransform(int jointId, float[] modelTransform)
        {
            animTransforms[jointId] = modelTransform;
        }



        /// <summary>
        /// Finalizes the matricies with joints assigned by their ID.
        /// </summary>
        /// <param name="jointsById"></param>
        public void FinalizeMatrices(Dictionary<int, AnimationJoint> jointsById)
        {
            int k = 0;
            for (int jointid = 0; jointid < GlobalConstants.MaxAnimatedElements; jointid++)
            {
                float[] animTransform = Mat4f.CloneIt(animTransforms[jointid]);

                if (jointsById.ContainsKey(jointid))
                {
                    Mat4f.Mul(animTransform, animTransform, jointsById[jointid].Element.inverseModelTransform);
                }

                for (int j = 0; j < 16; j++)
                {
                    transformationMatrices[k++] = animTransform[j];
                }
            }

            animTransforms = null;
        }

    }
}