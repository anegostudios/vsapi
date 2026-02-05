using System;
using System.Collections.Generic;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    public class AnimationJoint
    {
        /// <summary>
        /// The ID of the joint.
        /// </summary>
        public int JointId;

        /// <summary>
        /// The attached ShapeElement.
        /// </summary>
        public ShapeElement Element;

        /// <summary>
        /// Takes the transform and inverses it.
        /// </summary>
        /// <param name="frameModelTransform"></param>
        /// <returns></returns>
        public float[] ApplyInverseTransform(float[] frameModelTransform)
        {
            List<ShapeElement> elems = Element.GetParentPath();

            Span<float> modelTransform = stackalloc float [16];
            Mat4f.NewIdentity(modelTransform);
            float[] tmp = new float[16];   // Performance note: If we did not create it here, it would be created inside GetLocalTransformMatrix anyhow...
            
            for (int i = 0; i < elems.Count; i++)
            {
                ShapeElement elem = elems[i];
                Mat4f.Identity(tmp);
                Mat4f.Mul(modelTransform, elem.GetLocalTransformMatrix(0, tmp));
            }

            Mat4f.Identity(tmp);
            Mat4f.Mul(modelTransform, Element.GetLocalTransformMatrix(0, tmp));

            Mat4f.Invert(modelTransform);

            Mat4f.Mul(frameModelTransform, modelTransform);
            return frameModelTransform;
        }
    }
}
