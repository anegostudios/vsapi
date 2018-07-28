using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ShapeElement
    {
        [JsonProperty]
        public string Name;
        [JsonProperty]
        public double[] From;
        [JsonProperty]
        public double[] To;
        [JsonProperty]
        public bool Shade = true;
        [JsonProperty]
        public Dictionary<string, ShapeElementFace> Faces;
        [JsonProperty]
        public double[] RotationOrigin;
        [JsonProperty]
        public double RotationX;
        [JsonProperty]
        public double RotationY;
        [JsonProperty]
        public double RotationZ;
        [JsonProperty]
        public double ScaleX = 1;
        [JsonProperty]
        public double ScaleY = 1;
        [JsonProperty]
        public double ScaleZ = 1;
        [JsonProperty]
        public int TintIndex = -1;
        [JsonProperty]
        public int RenderPass = -1;
        [JsonProperty]
        public ShapeElement[] Children;
        [JsonProperty]
        public AttachmentPoint[] AttachmentPoints;

        public ShapeElement ParentElement;
        public int JointId;
        public float[] inverseModelTransform;

        /// <summary>
        /// Walks the element tree and collects all parents, starting with the root element
        /// </summary>
        /// <returns></returns>
        public List<ShapeElement> GetParentPath()
        {
            List<ShapeElement> path = new List<ShapeElement>();
            ShapeElement parentElem = this.ParentElement;
            while (parentElem != null)
            {
                path.Add(parentElem);
                parentElem = parentElem.ParentElement;
            }
            path.Reverse();
            return path;
        }

        public void CacheInverseTransformMatrix()
        {
            if (inverseModelTransform == null)
            {
                inverseModelTransform = GetInverseModelMatrix();
            }
        }

        /// <summary>
        /// Returns the full inverse model matrix (includes all parent transforms)
        /// </summary>
        /// <returns></returns>
        public float[] GetInverseModelMatrix()
        {
            List<ShapeElement> elems = GetParentPath();

            float[] modelTransform = Mat4f.Create();

            for (int i = 0; i < elems.Count; i++)
            {
                ShapeElement elem = elems[i];
                float[] localTransform = elem.GetLocalTransformMatrix();
                Mat4f.Mul(modelTransform, modelTransform, localTransform);
            }

            Mat4f.Mul(modelTransform, modelTransform, GetLocalTransformMatrix());

            float[] inverseTransformMatrix = Mat4f.Invert(Mat4f.Create(), modelTransform);

            return inverseTransformMatrix;
        }


        internal void SetJointId(int jointId)
        {
            this.JointId = jointId;

            if (Children == null) return;
            for (int i = 0; i < Children.Length; i++)
            {
                Children[i].SetJointId(jointId);
            }
        }

        internal void ResolveRefernces()
        {
            if (Children != null)
            {
                for (int i = 0; i < Children.Length; i++)
                {
                    Children[i].ParentElement = this;
                    Children[i].ResolveRefernces();
                }
            }

            for (int i = 0; AttachmentPoints != null && i < AttachmentPoints.Length; i++)
            {
                AttachmentPoints[i].ParentElement = this;
            }
        }

        static ElementPose noTransform = new ElementPose();

        public float[] GetLocalTransformMatrix(float[] output = null, ElementPose tf = null)
        {
            if (tf == null) tf = noTransform;

            ShapeElement elem = this;
            if (output == null) output = Mat4f.Create();

            float[] origin = new float[] { 0f, 0f, 0f };

            if (elem.RotationOrigin != null)
            {
                origin[0] = (float)elem.RotationOrigin[0] / 16;
                origin[1] = (float)elem.RotationOrigin[1] / 16;
                origin[2] = (float)elem.RotationOrigin[2] / 16;
            }

            Mat4f.Translate(output, output, origin);

            if (elem.RotationX + tf.degX != 0)
            {
                Mat4f.RotateX(output, output, (float)(elem.RotationX + tf.degX) * GameMath.DEG2RAD);
            }
            if (elem.RotationY + tf.degY != 0)
            {
                Mat4f.RotateY(output, output, (float)(elem.RotationY + tf.degY) * GameMath.DEG2RAD);
            }
            if (elem.RotationZ + tf.degZ != 0)
            {
                Mat4f.RotateZ(output, output, (float)(elem.RotationZ + tf.degZ) * GameMath.DEG2RAD);
            }

            Mat4f.Scale(output, output, new float[] { (float)elem.ScaleX * tf.scaleX, (float)elem.ScaleY * tf.scaleY, (float)elem.ScaleZ * tf.scaleZ });

            Mat4f.Translate(output, output, new float[] { -origin[0], -origin[1], -origin[2] });

            Mat4f.Translate(output, output, new float[] {
                (float)elem.From[0] / 16 + tf.translateX,
                (float)elem.From[1] / 16 + tf.translateY,
                (float)elem.From[2] / 16 + tf.translateZ
            });



            return output;
        }
        
    }
}