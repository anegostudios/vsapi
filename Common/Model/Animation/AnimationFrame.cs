using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    // https://youtu.be/cieheqt7eqc?t=11m51s
    public class AnimationFrame
    {
        public int FrameNumber;

        public float[][] animTransforms = new float[GlobalConstants.MaxAnimatedElements][];

        public float[] transformationMatrices = new float[16 * GlobalConstants.MaxAnimatedElements];

        public List<ElementPose> RootElementTransforms = new List<ElementPose>();
        

        public AnimationFrame()
        {
            for (int i = 0; i < animTransforms.Length; i++)
            {
                animTransforms[i] = Mat4f.Create();
            }
            
        }


        public void SetTransform(int jointId, float[] modelTransform)
        {
            animTransforms[jointId] = modelTransform;
        }




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