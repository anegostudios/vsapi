using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public class AnimationJoint
    {
        public int JointId;
        public ShapeElement Element;

        public float[] ApplyInverseTransform(float[] frameModelTransform)
        {
            List<ShapeElement> elems = Element.GetParentPath();

            float[] modelTransform = Mat4f.Create();
            
            for (int i = 0; i < elems.Count; i++)
            {
                ShapeElement elem = elems[i];
                float[] localTransform = elem.GetLocalTransformMatrix();
                Mat4f.Mul(modelTransform, modelTransform, localTransform);
            }

            Mat4f.Mul(modelTransform, modelTransform, Element.GetLocalTransformMatrix());

            float[] inverseTransformMatrix = Mat4f.Invert(Mat4f.Create(), modelTransform);

            return Mat4f.Mul(frameModelTransform, frameModelTransform, inverseTransformMatrix);
        }
    }
}
