using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// The position of an element.
    /// </summary>
    public class ElementPose
    {
        /// <summary>
        /// The element this positioning is for.
        /// </summary>
        public ShapeElement ForElement;

        /// <summary>
        /// The model matrix of this element.
        /// </summary>
        public float[] AnimModelMatrix;

        public List<ElementPose> ChildElementPoses = new List<ElementPose>();

        public float degX, degY, degZ;
        public float scaleX = 1, scaleY = 1, scaleZ = 1;
        public float translateX, translateY, translateZ;

        public void Clear()
        {
            degX = 0;
            degY = 0;
            degZ = 0;
            scaleX = 1;
            scaleY = 1;
            scaleZ = 1;
            translateX = 0;
            translateY = 0;
            translateZ = 0;
        }

        public void Add(ElementPose tf, ElementPose tfNext, float l, float weight)
        {
            degX += (tf.degX * (1-l) + tfNext.degX * l) * weight;
            degY += (tf.degY * (1 - l) + tfNext.degY * l) * weight;
            degZ += (tf.degZ * (1 - l) + tfNext.degZ * l) * weight;
            scaleX += ((tf.scaleX - 1) * (1-l) + (tfNext.scaleX - 1) * l) * weight;
            scaleY += ((tf.scaleY - 1) * (1 - l) + (tfNext.scaleY - 1) * l) * weight;
            scaleZ += ((tf.scaleZ - 1) * (1 - l) + (tfNext.scaleZ - 1) * l) * weight;
            translateX += (tf.translateX * (1 - l) + tfNext.translateX * l) * weight;
            translateY += (tf.translateY * (1 - l) + tfNext.translateY * l) * weight;
            translateZ += (tf.translateZ * (1 - l) + tfNext.translateZ * l) * weight;
        }

        internal void SetMat(float[] modelMatrix)
        {
            for (int i = 0; i < 16; i++)
            {
                AnimModelMatrix[i] = modelMatrix[i];
            }
        }
    }
}
