using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// A shape element built from JSON data within the model.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ShapeElement
    {
        /// <summary>
        /// A static reference to the logger (null on a server) - we don't want to hold a reference to the platform or api in every ShapeElement
        /// </summary>
        public static ILogger Logger;
        public static object locationForLogging;

        /// <summary>
        /// The name of the ShapeElement
        /// </summary>
        [JsonProperty]
        public string Name;


        [JsonProperty]
        public double[] From;
        [JsonProperty]
        public double[] To;

        /// <summary>
        /// Whether or not the shape element is shaded.
        /// </summary>
        [JsonProperty]
        public bool Shade = true;

        [JsonProperty]
        public bool GradientShade = false;

        /// <summary>
        /// The faces of the shape element by name (will normally be null except during object deserialization: use FacesResolved instead!)
        /// </summary>
        [JsonProperty]
        [Obsolete("Use FacesResolved instead")]
        public Dictionary<string, ShapeElementFace> Faces;
        /// <summary>
        /// An array holding the faces of this shape element in BlockFacing order: North, East, South, West, Up, Down.  May be null if not present or not enabled.
        /// <br/>Note: from game version 1.20.4, this is <b>null on server-side</b> (except during asset loading start-up stage)
        /// </summary>
        public ShapeElementFace[] FacesResolved = new ShapeElementFace[6];

        /// <summary>
        /// The origin point for rotation.
        /// </summary>
        [JsonProperty]
        public double[] RotationOrigin;

        /// <summary>
        /// The forward vertical rotation of the shape element.
        /// </summary>
        [JsonProperty]
        public double RotationX;

        /// <summary>
        /// The forward vertical rotation of the shape element.
        /// </summary>
        [JsonProperty]
        public double RotationY;

        /// <summary>
        /// The left/right tilt of the shape element
        /// </summary>
        [JsonProperty]
        public double RotationZ;

        /// <summary>
        /// How far away are the left/right sides of the shape from the center
        /// </summary>
        [JsonProperty]
        public double ScaleX = 1;

        /// <summary>
        /// How far away are the top/bottom sides of the shape from the center
        /// </summary>
        [JsonProperty]
        public double ScaleY = 1;

        /// <summary>
        /// How far away are the front/back sides of the shape from the center.
        /// </summary>
        [JsonProperty]
        public double ScaleZ = 1;

        [JsonProperty]
        public string ClimateColorMap = null;
        [JsonProperty]
        public string SeasonColorMap = null;
        [JsonProperty]
        public short RenderPass = -1;
        [JsonProperty]
        public short ZOffset = 0;
        /// <summary>
        /// Set this to true to disable randomDrawOffset and randomRotations on this specific element (e.g. used for the ice element of Coopers Reeds in Ice)
        /// </summary>
        [JsonProperty]
        public bool DisableRandomDrawOffset;

        /// <summary>
        /// The child shapes of this shape element
        /// </summary>
        [JsonProperty]
        public ShapeElement[] Children;

        /// <summary>
        /// The attachment points for this shape.
        /// </summary>
        [JsonProperty]
        public AttachmentPoint[] AttachmentPoints;

        /// <summary>
        /// The "remote" parent for this element
        /// </summary>
        [JsonProperty]
        public string StepParentName;

        /// <summary>
        /// The parent element reference for this shape.
        /// </summary>
        public ShapeElement ParentElement;

        /// <summary>
        /// The id of the joint attached to the parent element.
        /// </summary>
        public int JointId;

        /// <summary>
        /// For entity animations
        /// </summary>
        public int Color = ColorUtil.WhiteArgb;

        public float DamageEffect;

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

        public int CountParents()
        {
            int count = 0;
            ShapeElement parentElem = this.ParentElement;
            while (parentElem != null)
            {
                count++;
                parentElem = parentElem.ParentElement;
            }
            return count;
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

            Span<float> modelTransform = stackalloc float[16];
            Mat4f.NewIdentity(modelTransform);
            float[] tmp = new float[16];   // Performance note: If we did not create it here, it would be created inside GetLocalTransformMatrix anyhow...

            for (int i = 0; i < elems.Count; i++)
            {
                ShapeElement elem = elems[i];
                Mat4f.Identity(tmp);
                Mat4f.Mul(modelTransform, elem.GetLocalTransformMatrix(0, tmp));
            }

            Mat4f.Identity(tmp);
            Mat4f.Mul(modelTransform, GetLocalTransformMatrix(0, tmp));

            Mat4f.Invert(modelTransform);

            return Mat4f.Copy(tmp, modelTransform);
        }


        internal void SetJointId(int jointId)
        {
            this.JointId = jointId;

            var Children = this.Children;
            if (Children == null) return;
            for (int i = 0; i < Children.Length; i++)
            {
                Children[i].SetJointId(jointId);
            }
        }

        internal void ResolveReferences()
        {
            var Children = this.Children;
            if (Children != null)
            {
                for (int i = 0; i < Children.Length; i++)
                {
                    ShapeElement child = Children[i];
                    child.ParentElement = this;
                    child.ResolveReferences();
                }
            }

            var AttachmentPoints = this.AttachmentPoints;
            if (AttachmentPoints != null)
            {
                for (int i = 0; i < AttachmentPoints.Length; i++)
                {
                    AttachmentPoints[i].ParentElement = this;
                }
            }
        }

        internal void TrimTextureNamesAndResolveFaces()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            if (Faces != null)
            {
                foreach (var val in Faces)
                {
                    ShapeElementFace f = val.Value;
                    if (!f.Enabled) continue;
                    BlockFacing facing = BlockFacing.FromFirstLetter(val.Key);
                    if (facing == null)
                    {
                        Logger?.Warning("Shape element in " + locationForLogging + ": Unknown facing '" + facing.Code + "'. Ignoring face.");
                        continue;
                    }
                    FacesResolved[facing.Index] = f;
                    f.Texture = f.Texture.Substring(1).DeDuplicate();
                }
            }
            Faces = null;
#pragma warning restore CS0618 // Type or member is obsolete

            if (Children != null)
            {
                foreach (ShapeElement child in Children) child.TrimTextureNamesAndResolveFaces();
            }

            Name = Name.DeDuplicate();
            StepParentName = StepParentName.DeDuplicate();
            var AttachmentPoints = this.AttachmentPoints;
            if (AttachmentPoints != null)
            {
                for (int i = 0; i < AttachmentPoints.Length; i++) AttachmentPoints[i].DeDuplicate();
            }
        }


        static ElementPose noTransform = new ElementPose();

        public float[] GetLocalTransformMatrix(int animVersion, float[] output = null, ElementPose tf = null)
        {
            if (tf == null) tf = noTransform;

            ShapeElement elem = this;
            output ??= Mat4f.Create();
            Span<float> matrix = (Span<float>)output;

            float origin0 = 0f;
            float origin1 = 0f;
            float origin2 = 0f;

            if (elem.RotationOrigin != null)
            {
                origin0 = (float)elem.RotationOrigin[0] / 16;
                origin1 = (float)elem.RotationOrigin[1] / 16;
                origin2 = (float)elem.RotationOrigin[2] / 16;
            }

            // R = rotate, S = scale, T = translate
            // Version 0: R * S * T
            // Version 1: T * S * R

            if (animVersion == 1)
            {
                // ==================================================
                // BASE ELEMENT TRANSLATE SCALE ROTATE
                // ==================================================
                Mat4f.Translate(matrix, origin0, origin1, origin2);

                Mat4f.Scale(matrix, (float)elem.ScaleX, (float)elem.ScaleY, (float)elem.ScaleZ);

                Mat4f.RotateByXYZ(matrix,
                    (float)(elem.RotationX * GameMath.DEG2RAD),
                    (float)(elem.RotationY * GameMath.DEG2RAD),
                    (float)(elem.RotationZ * GameMath.DEG2RAD));

                // note: lumped together with next translation
                // Mat4f.Translate(output, output, -originX, -originY, -originZ);

                // ==================================================
                // KEYFRAME TRANSLATE SCALE ROTATE
                // ==================================================

                Mat4f.Translate(matrix,
                    -origin0 + (float)elem.From[0] / 16 + tf.translateX,
                    -origin1 + (float)elem.From[1] / 16 + tf.translateY,
                    -origin2 + (float)elem.From[2] / 16 + tf.translateZ
                );

                Mat4f.Scale(matrix, tf.scaleX, tf.scaleY, tf.scaleZ);

                Mat4f.RotateByXYZ(matrix,
                    (float)(tf.degX + tf.degOffX) * GameMath.DEG2RAD,
                    (float)(tf.degY + tf.degOffY) * GameMath.DEG2RAD,
                    (float)(tf.degZ + tf.degOffZ) * GameMath.DEG2RAD);
            }
            else
            {
                Mat4f.Translate(matrix, origin0, origin1, origin2);

                Mat4f.RotateByXYZ(matrix,
                    (float)(elem.RotationX + tf.degX + tf.degOffX) * GameMath.DEG2RAD,
                    (float)(elem.RotationY + tf.degY + tf.degOffY) * GameMath.DEG2RAD,
                    (float)(elem.RotationZ + tf.degZ + tf.degOffZ) * GameMath.DEG2RAD);

                Mat4f.Scale(matrix, (float)elem.ScaleX * tf.scaleX, (float)elem.ScaleY * tf.scaleY, (float)elem.ScaleZ * tf.scaleZ);

                Mat4f.Translate(matrix,
                    (float)elem.From[0] / 16 + tf.translateX - origin0,
                    (float)elem.From[1] / 16 + tf.translateY - origin1,
                    (float)elem.From[2] / 16 + tf.translateZ - origin2
                );
            }

            return output;
        }

        public ShapeElement Clone()
        {
            ShapeElement elem = new ShapeElement()
            {
                AttachmentPoints = (AttachmentPoint[])AttachmentPoints?.Clone(),
                FacesResolved = (ShapeElementFace[])FacesResolved?.Clone(),
                From = (double[])From?.Clone(),
                To = (double[])To?.Clone(),
                inverseModelTransform = (float[])inverseModelTransform?.Clone(),
                JointId = JointId,
                RenderPass = RenderPass,
                RotationX = RotationX,
                RotationY = RotationY,
                RotationZ = RotationZ,
                RotationOrigin = (double[])RotationOrigin?.Clone(),
                SeasonColorMap = SeasonColorMap,
                ClimateColorMap = ClimateColorMap,
                StepParentName = StepParentName,
                Shade = Shade,
                DisableRandomDrawOffset = DisableRandomDrawOffset,
                ZOffset = ZOffset,
                GradientShade = GradientShade,
                ScaleX = ScaleX,
                ScaleY = ScaleY,
                ScaleZ = ScaleZ,
                Name = Name
            };

            var Children = this.Children;
            if (Children != null)
            {
                elem.Children = new ShapeElement[Children.Length];
                for (int i = 0; i < Children.Length; i++)
                {
                    var child = Children[i].Clone();
                    child.ParentElement = elem;
                    elem.Children[i] = child;
                }
            }

            return elem;

        }

        public void SetJointIdRecursive(int jointId)
        {
            this.JointId = jointId;
            var Children = this.Children;
            if (Children != null)
            {
                for (int i = 0; i < Children.Length; i++)
                {
                    Children[i].SetJointIdRecursive(jointId);
                }
            }
        }

        public void CacheInverseTransformMatrixRecursive()
        {
            CacheInverseTransformMatrix();

            var Children = this.Children;
            if (Children != null)
            {
                for (int i = 0; i < Children.Length; i++)
                {
                    Children[i].CacheInverseTransformMatrixRecursive();
                }
            }
        }

        public void WalkRecursive(Action<ShapeElement> onElem)
        {
            onElem(this);
            var Children = this.Children;
            if (Children != null)
            {
                for (int i = 0; i < Children.Length; i++)
                {
                    Children[i].WalkRecursive(onElem);
                }
            }
        }

        internal bool HasFaces()
        {
            for (int i = 0; i < 6; i++) if (FacesResolved[i] != null) return true;
            return false;
        }

        public virtual void FreeRAMServer()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            Faces = null;
#pragma warning restore CS0618 // Type or member is obsolete
            FacesResolved = null;   // We don't need the Faces information on the server-side
            var Children = this.Children;
            if (Children != null)
            {
                for (int i = 0; i < Children.Length; i++)
                {
                    Children[i].FreeRAMServer();
                }
            }

            // For now, turns out the following all need to be kept, just in case we need to animate this ShapeElement fully on the server (e.g. when a creature dies)
            //From = null;
            //To = null;
            //RotationOrigin = null;
            //inverseModelTransform = null;
        }
    }
}
