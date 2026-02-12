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


        /// <summary>
        /// Cached base matrices for different animation versions
        /// </summary>
        private float[] _cachedBaseMatrixV0;
        private float[] _cachedBaseMatrixV1;


        /// <summary>
        /// Flag indicating that the cache is stale
        /// </summary>
        private bool _isCacheDirty = true;

        /// <summary>
        /// Static field for identity transformation
        /// </summary>
        static ElementPose noTransform = new ElementPose();

        /// <summary>
        /// Gets the local transformation matrix with caching
        /// </summary>
        public float[] GetLocalTransformMatrix(int animVersion, float[] output = null, ElementPose tf = null)
        {
            if (tf == null) tf = noTransform;
            output ??= Mat4f.Create();

            // Check if cache needs to be updated
            if (_isCacheDirty)
            {
                RebuildCache();
            }

            // Check if tf is empty transformation
            bool isTfEmpty = IsTfEmpty(tf);

            if (animVersion == 1)
            {
                // For version 1 always use cached matrix
                if (_cachedBaseMatrixV1 == null)
                {
                    RebuildCache();
                }

                // Copy cached matrix
                Array.Copy(_cachedBaseMatrixV1, 0, output, 0, 16);

                // If tf is not empty, apply transformations
                if (!isTfEmpty)
                {
                    ApplyTfToMatrixV1(output, tf);
                }
            }
            else
            {
                // For version 0
                if (isTfEmpty)
                {
                    // If tf is empty, use cached matrix
                    if (_cachedBaseMatrixV0 == null)
                    {
                        RebuildCache();
                    }
                    Array.Copy(_cachedBaseMatrixV0, 0, output, 0, 16);
                }
                else
                {
                    // If tf is not empty, recalculate completely
                    ComputeMatrixV0WithTf(output, tf);
                }
            }

            return output;
        }

        /// <summary>
        /// Checks if transformation is empty
        /// </summary>
        private static bool IsTfEmpty(ElementPose tf)
        {
            return tf.translateX == 0 && tf.translateY == 0 && tf.translateZ == 0 &&
                   tf.scaleX == 1 && tf.scaleY == 1 && tf.scaleZ == 1 &&
                   tf.degX == 0 && tf.degY == 0 && tf.degZ == 0 &&
                   tf.degOffX == 0 && tf.degOffY == 0 && tf.degOffZ == 0;
        }


        /// <summary>
        /// Rebuilds cached matrices
        /// </summary>
        private void RebuildCache()
        {
            float origin0 = 0f, origin1 = 0f, origin2 = 0f;
            if (RotationOrigin != null)
            {
                origin0 = (float)RotationOrigin[0] / 16f;
                origin1 = (float)RotationOrigin[1] / 16f;
                origin2 = (float)RotationOrigin[2] / 16f;
            }

            // Create matrices for caching
            _cachedBaseMatrixV0 = Mat4f.Create();
            _cachedBaseMatrixV1 = Mat4f.Create();

            // Calculate base matrix for version 0
            ComputeBaseMatrixV0(_cachedBaseMatrixV0, origin0, origin1, origin2);

            // Calculate base matrix for version 1
            ComputeBaseMatrixV1(_cachedBaseMatrixV1, origin0, origin1, origin2);

            _isCacheDirty = false;
        }

        /// <summary>
        /// Calculates base matrix for version 0
        /// </summary>
        private void ComputeBaseMatrixV0(float[] matrix, float origin0, float origin1, float origin2)
        {
            Mat4f.Identity(matrix);

            if (origin0 != 0f || origin1 != 0f || origin2 != 0f)
                Mat4f.Translate(matrix, matrix, origin0, origin1, origin2);

            float rotX = (float)RotationX * GameMath.DEG2RAD;
            if (rotX != 0)
                Mat4f.RotateX(matrix, matrix, rotX);

            float rotY = (float)RotationY * GameMath.DEG2RAD;
            if (rotY != 0)
                Mat4f.RotateY(matrix, matrix, rotY);

            float rotZ = (float)RotationZ * GameMath.DEG2RAD;
            if (rotZ != 0)
                Mat4f.RotateZ(matrix, matrix, rotZ);

            float scaleX = (float)ScaleX;
            float scaleY = (float)ScaleY;
            float scaleZ = (float)ScaleZ;

            if (scaleX != 1f || scaleY != 1f || scaleZ != 1f)
                Mat4f.Scale(matrix, matrix, scaleX, scaleY, scaleZ);

            float tx = (float)From[0] / 16f - origin0;
            float ty = (float)From[1] / 16f - origin1;
            float tz = (float)From[2] / 16f - origin2;

            if (tx != 0f || ty != 0f || tz != 0f)
                Mat4f.Translate(matrix, matrix, tx, ty, tz);
        }

        /// <summary>
        /// Calculates base matrix for version 1
        /// </summary>
        private void ComputeBaseMatrixV1(float[] matrix, float origin0, float origin1, float origin2)
        {
            Mat4f.Identity(matrix);

            if (origin0 != 0f || origin1 != 0f || origin2 != 0f)
                Mat4f.Translate(matrix, matrix, origin0, origin1, origin2);

            float scaleX = (float)ScaleX;
            float scaleY = (float)ScaleY;
            float scaleZ = (float)ScaleZ;

            if (scaleX != 1f || scaleY != 1f || scaleZ != 1f)
                Mat4f.Scale(matrix, matrix, scaleX, scaleY, scaleZ);

            float rotX = (float)RotationX * GameMath.DEG2RAD;
            if (rotX != 0)
                Mat4f.RotateX(matrix, matrix, rotX);

            float rotY = (float)RotationY * GameMath.DEG2RAD;
            if (rotY != 0)
                Mat4f.RotateY(matrix, matrix, rotY);

            float rotZ = (float)RotationZ * GameMath.DEG2RAD;
            if (rotZ != 0)
                Mat4f.RotateZ(matrix, matrix, rotZ);

            float tx = -origin0 + (float)From[0] / 16f;
            float ty = -origin1 + (float)From[1] / 16f;
            float tz = -origin2 + (float)From[2] / 16f;

            if (tx != 0f || ty != 0f || tz != 0f)
                Mat4f.Translate(matrix, matrix, tx, ty, tz);
        }

        /// <summary>
        /// Applies transformation tf to version 1 matrix
        /// </summary>
        private void ApplyTfToMatrixV1(float[] matrix, ElementPose tf)
        {
            // Apply translation
            if (tf.translateX != 0f || tf.translateY != 0f || tf.translateZ != 0f)
                Mat4f.Translate(matrix, matrix, tf.translateX, tf.translateY, tf.translateZ);

            // Apply scaling
            if (tf.scaleX != 1f || tf.scaleY != 1f || tf.scaleZ != 1f)
                Mat4f.Scale(matrix, matrix, tf.scaleX, tf.scaleY, tf.scaleZ);

            // Apply rotation
            float rotX = (float)(tf.degX + tf.degOffX) * GameMath.DEG2RAD;
            if (rotX != 0)
                Mat4f.RotateX(matrix, matrix, rotX);

            float rotY = (float)(tf.degY + tf.degOffY) * GameMath.DEG2RAD;
            if (rotY != 0)
                Mat4f.RotateY(matrix, matrix, rotY);

            float rotZ = (float)(tf.degZ + tf.degOffZ) * GameMath.DEG2RAD;
            if (rotZ != 0)
                Mat4f.RotateZ(matrix, matrix, rotZ);
        }

        /// <summary>
        /// Calculates version 0 matrix with tf transformations
        /// </summary>
        private void ComputeMatrixV0WithTf(float[] output, ElementPose tf)
        {
            float origin0 = 0f, origin1 = 0f, origin2 = 0f;
            if (RotationOrigin != null)
            {
                origin0 = (float)RotationOrigin[0] / 16f;
                origin1 = (float)RotationOrigin[1] / 16f;
                origin2 = (float)RotationOrigin[2] / 16f;
            }

            Mat4f.Identity(output);

            if (origin0 != 0f || origin1 != 0f || origin2 != 0f)
                Mat4f.Translate(output, output, origin0, origin1, origin2);

            float rotX = (float)(RotationX + tf.degX + tf.degOffX) * GameMath.DEG2RAD;
            if (rotX != 0)
                Mat4f.RotateX(output, output, rotX);

            float rotY = (float)(RotationY + tf.degY + tf.degOffY) * GameMath.DEG2RAD;
            if (rotY != 0)
                Mat4f.RotateY(output, output, rotY);

            float rotZ = (float)(RotationZ + tf.degZ + tf.degOffZ) * GameMath.DEG2RAD;
            if (rotZ != 0)
                Mat4f.RotateZ(output, output, rotZ);

            float scaleX = (float)ScaleX * tf.scaleX;
            float scaleY = (float)ScaleY * tf.scaleY;
            float scaleZ = (float)ScaleZ * tf.scaleZ;

            if (scaleX != 1f || scaleY != 1f || scaleZ != 1f)
                Mat4f.Scale(output, output, scaleX, scaleY, scaleZ);

            float tx = (float)From[0] / 16f + tf.translateX - origin0;
            float ty = (float)From[1] / 16f + tf.translateY - origin1;
            float tz = (float)From[2] / 16f + tf.translateZ - origin2;

            if (tx != 0f || ty != 0f || tz != 0f)
                Mat4f.Translate(output, output, tx, ty, tz);
        }


        /// <summary>
        /// Clones the element while preserving cache
        /// </summary>
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
                Name = Name,

                // Copy cache
                _cachedBaseMatrixV0 = (float[])_cachedBaseMatrixV0?.Clone(),
                _cachedBaseMatrixV1 = (float[])_cachedBaseMatrixV1?.Clone(),

                _isCacheDirty = _isCacheDirty
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
