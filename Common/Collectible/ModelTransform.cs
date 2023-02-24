using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ModelTransformNoDefaults
    {
        /// <summary>
        /// Offsetting
        /// </summary>
        [JsonProperty]
        public Vec3f Translation;
        /// <summary>
        /// Rotation in degrees
        /// </summary>
        [JsonProperty]
        public Vec3f Rotation;
        /// <summary>
        /// To set uniform Scaling on all Axes
        /// </summary>
        [JsonProperty]
        public float Scale { set { ScaleXYZ.Set(value, value, value); } }
        /// <summary>
        /// Rotation/Scaling Origin
        /// </summary>
        [JsonProperty]
        public Vec3f Origin = new Vec3f(0.5f, 0.5f, 0.5f);
        /// <summary>
        /// For Gui Transform: Whether to slowly spin in gui item preview 
        /// For Ground Transform: Whether to apply a random rotation to the dropped item
        /// No effect on other transforms
        /// </summary>
        [JsonProperty]
        public bool Rotate = true;

        /// <summary>
        /// Scaling per axis
        /// </summary>
        [JsonProperty]
        public Vec3f ScaleXYZ = new Vec3f(1, 1, 1);


        /// <summary>
        /// Converts the transform into a matrix.
        /// </summary>
        public float[] AsMatrix
        {
            get
            {
                float[] mat = Mat4f.Create();
                Mat4f.Translate(mat, mat, Translation.X, Translation.Y, Translation.Z);
                Mat4f.Translate(mat, mat, Origin.X, Origin.Y, Origin.Z);

                Mat4f.RotateX(mat, mat, Rotation.X * GameMath.DEG2RAD);
                Mat4f.RotateY(mat, mat, Rotation.Y * GameMath.DEG2RAD);
                Mat4f.RotateZ(mat, mat, Rotation.Z * GameMath.DEG2RAD);

                Mat4f.Scale(mat, mat, ScaleXYZ.X, ScaleXYZ.Y, ScaleXYZ.Z);

                Mat4f.Translate(mat, mat, -Origin.X, -Origin.Y, -Origin.Z);

                return mat;
            }
        }


        /// <summary>
        /// Clears the transformation values.
        /// </summary>
        public void Clear()
        {
            Rotation.Set(0, 0, 0);
            Translation.Set(0, 0, 0);
            Origin.Set(0, 0, 0);
            Scale = 1f;
        }

    }

    /// <summary>
    /// Used for transformations applied to a block or item model
    /// </summary>


    public class ModelTransform : ModelTransformNoDefaults
    {
        public ModelTransform(ModelTransformNoDefaults baseTf, ModelTransform defaults)
        {
            Rotate = baseTf.Rotate;

            if (baseTf.Translation == null) Translation = defaults.Translation.Clone();
            else Translation = baseTf.Translation.Clone();

            if (baseTf.Rotation == null) Rotation = defaults.Rotation.Clone();
            else Rotation = baseTf.Rotation.Clone();

            if (baseTf.Origin == null) Origin = defaults.Origin.Clone();
            else Origin = baseTf.Origin.Clone();

            if (baseTf.ScaleXYZ == null) ScaleXYZ = defaults.ScaleXYZ.Clone();
            else ScaleXYZ = baseTf.ScaleXYZ.Clone();
        }

        public ModelTransform()
        {
        }

        /// <summary>
        /// Gets a new model with all values set to default.
        /// </summary>
        public static ModelTransform NoTransform
        {
            get
            {
                return new ModelTransform().EnsureDefaultValues();
            }
        }

        /// <summary>
        /// Scale = 1, No Translation, Rotation by -45 deg in Y-Axis
        /// </summary>
        /// <returns></returns>
        public static ModelTransform BlockDefaultGui()
        {
            return new ModelTransform()
            {
                Translation = new Vec3f(),
                Rotation = new Vec3f(-22.6f, -45 - 0.3f, 0),
                Scale = 1f
            };
        }

        /// <summary>
        /// Scale = 1, No Translation, Rotation by -45 deg in Y-Axis
        /// </summary>
        /// <returns></returns>
        public static ModelTransform BlockDefaultFp()
        {
            return new ModelTransform()
            {
                Translation = new Vec3f(0, -0.15f, 0.5f),
                Rotation = new Vec3f(0, -20, 0),
                Scale = 1.3f
            };
        }

        /// <summary>
        /// Scale = 1, No Translation, Rotation by -45 deg in Y-Axis
        /// </summary>
        /// <returns></returns>
        public static ModelTransform BlockDefaultTp()
        {
            return new ModelTransform()
            {
                Translation = new Vec3f(-2.1f, -1.8f, -1.5f),
                Rotation = new Vec3f(0, -45, -25),
                Scale = 0.3f
            };
        }


        /// <summary>
        /// Scale = 1, No Translation, Rotation by -45 deg in Y-Axis, 1.5x scale
        /// </summary>
        /// <returns></returns>
        public static ModelTransform BlockDefaultGround()
        {
            return new ModelTransform()
            {
                Translation = new Vec3f(),
                Rotation = new Vec3f(0, -45, 0),
                Origin = new Vec3f(0.5f, 0, 0.5f),
                Scale = 1.5f
            };
        }

        /// <summary>
        /// Scale = 1, No Translation, No Rotation
        /// </summary>
        /// <returns></returns>
        public static ModelTransform ItemDefaultGui()
        {
            return new ModelTransform()
            {
                Translation = new Vec3f(3, 1, 0),
                Rotation = new Vec3f(0, 0, 0),
                Origin = new Vec3f(0.6f, 0.6f, 0),
                Scale = 1f,
                Rotate = false
            };
        }

        /// <summary>
        /// Scale = 1, No Translation, Rotation by 180 deg in X-Axis
        /// </summary>
        /// <returns></returns>
        public static ModelTransform ItemDefaultFp()
        {
            return new ModelTransform()
            {
                Translation = new Vec3f(0.05f, 0, 0),
                Rotation = new Vec3f(180, 90, -30),
                Scale = 1f
            };
        }

        /// <summary>
        /// Scale = 1, No Translation, Rotation by 180 deg in X-Axis
        /// </summary>
        /// <returns></returns>
        public static ModelTransform ItemDefaultTp()
        {
            return new ModelTransform()
            {
                Translation = new Vec3f(-1.5f, -1.6f, -1.4f),
                Rotation = new Vec3f(0, -62, 18),
                Scale = 0.33f
            };
        }

        /// <summary>
        /// Creates a default transform for a model that is now on the ground
        /// </summary>
        /// <returns></returns>
        public static ModelTransform ItemDefaultGround()
        {
            return new ModelTransform()
            {
                Translation = new Vec3f(0, 0, 0),
                Rotation = new Vec3f(90, 0, 0),
                Origin = new Vec3f(0.5f, 0.5f, 0.53f),
                Scale = 1.5f
            };
        }




        /// <summary>
        /// Makes sure that Translation and Rotation is not null
        /// </summary>
        public ModelTransform EnsureDefaultValues()
        {
            if (Translation == null) Translation = new Vec3f();
            if (Rotation == null) Rotation = new Vec3f();
            return this;
        }

        public ModelTransform WithRotation(Vec3f rot)
        {
            Rotation = rot;
            return this;
        }

        /// <summary>
        /// Clones this specific transform.
        /// </summary>
        /// <returns></returns>
        public ModelTransform Clone()
        {
            return new ModelTransform()
            {
                Rotate = Rotate,
                Rotation = Rotation?.Clone(),
                Translation = Translation?.Clone(),
                ScaleXYZ = ScaleXYZ.Clone(),
                Origin = Origin?.Clone()
            };
        }


        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            EnsureDefaultValues();
        }
    }
}
