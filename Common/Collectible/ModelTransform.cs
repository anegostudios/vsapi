using Newtonsoft.Json;
using System.Runtime.Serialization;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Controls the transformations of 3D shapes. Note that defaults change depending on where this class is used.
    /// </summary>
    /// <example>
    /// Use '.tfedit' in game to help customize these values, just make sure to copy them into your json file when you finish.
    /// <code language="json">
    ///"tpHandTransform": {
	///	"translation": {
	///		"x": -0.87,
	///		"y": -0.01,
	///		"z": -0.56
	///	},
	///	"rotation": {
	///		"x": -90,
	///		"y": 0,
	///		"z": 0
	///	},
	///	"origin": {
	///		"x": 0.5,
	///		"y": 0,
	///		"z": 0.5
	///	},
	///	"scale": 0.8
	///},
    /// </code>
    /// </example>
    [DocumentAsJson]
    [JsonObject(MemberSerialization.OptIn)]
    public class ModelTransformNoDefaults
    {
        public static readonly FastVec3f defaultTf = new FastVec3f(-0.000099f, -0.000099f, -0.000099f);

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional>-->
        /// Offsetting
        /// </summary>
        [JsonProperty]
        public FastVec3f Translation = defaultTf;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional>-->
        /// Rotation in degrees
        /// </summary>
        [JsonProperty]
        public FastVec3f Rotation = defaultTf;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional>-->
        /// Sets the same scale of an object for all axes.
        /// </summary>
        [JsonProperty]
        public float Scale { set { ScaleXYZ.Set(value, value, value); } }

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional>-->
        /// Rotation/Scaling Origin
        /// </summary>
        [JsonProperty]
        public FastVec3f Origin = new FastVec3f(0.5f, 0.5f, 0.5f);

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional>-->
        /// For Gui Transform: Whether to slowly spin in gui item preview 
        /// For Ground Transform: Whether to apply a random rotation to the dropped item
        /// No effect on other transforms
        /// </summary>
        [JsonProperty]
        public bool Rotate = true;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional>-->
        /// Scaling per axis
        /// </summary>
        [JsonProperty]
        public FastVec3f ScaleXYZ = new FastVec3f(1, 1, 1);


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

                if (Rotation.X != 0 || Rotation.Y != 0 || Rotation.Z != 0)
                {
                    Mat4f.RotateX(mat, mat, Rotation.X * GameMath.DEG2RAD);
                    Mat4f.RotateY(mat, mat, Rotation.Y * GameMath.DEG2RAD);
                    Mat4f.RotateZ(mat, mat, Rotation.Z * GameMath.DEG2RAD);
                }

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
    /// Used for transformations applied to a block or item model. Uses values from <see cref="ModelTransformNoDefaults"/> but will assign defaults if not included.
    /// </summary>
    /// <example>
    /// Use '.tfedit' in game to help customize these values, just make sure to copy them into your json file when you finish.
    /// <code language="json">
    ///"tpHandTransform": {
    ///	"translation": {
    ///		"x": -0.87,
    ///		"y": -0.01,
    ///		"z": -0.56
    ///	},
    ///	"rotation": {
    ///		"x": -90,
    ///		"y": 0,
    ///		"z": 0
    ///	},
    ///	"origin": {
    ///		"x": 0.5,
    ///		"y": 0,
    ///		"z": 0.5
    ///	},
    ///	"scale": 0.8
    ///},
    /// </code>
    /// </example>
    [DocumentAsJson]
    public class ModelTransform : ModelTransformNoDefaults
    {
        public ModelTransform(ModelTransformNoDefaults baseTf, ModelTransform defaults)
        {
            Rotate = baseTf.Rotate;

            if (baseTf.Translation.Equals(defaultTf)) Translation = defaults.Translation;
            else Translation = baseTf.Translation;

            if (baseTf.Rotation.Equals(defaultTf)) Rotation = defaults.Rotation;
            else Rotation = baseTf.Rotation;

            Origin = baseTf.Origin;

            ScaleXYZ = baseTf.ScaleXYZ;
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
                Translation = new FastVec3f(),
                Rotation = new FastVec3f(-22.6f, -45 - 0.3f, 0),
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
                Translation = new FastVec3f(0, -0.15f, 0.5f),
                Rotation = new FastVec3f(0, -20, 0),
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
                Translation = new FastVec3f(-2.1f, -1.8f, -1.5f),
                Rotation = new FastVec3f(0, -45, -25),
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
                Translation = new FastVec3f(),
                Rotation = new FastVec3f(0, -45, 0),
                Origin = new FastVec3f(0.5f, 0, 0.5f),
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
                Translation = new FastVec3f(3, 1, 0),
                Rotation = new FastVec3f(0, 0, 0),
                Origin = new FastVec3f(0.6f, 0.6f, 0),
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
                Translation = new FastVec3f(0.05f, 0, 0),
                Rotation = new FastVec3f(180, 90, -30),
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
                Translation = new FastVec3f(-1.5f, -1.6f, -1.4f),
                Rotation = new FastVec3f(0, -62, 18),
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
                Translation = new FastVec3f(),
                Rotation = new FastVec3f(90, 0, 0),
                Origin = new FastVec3f(0.5f, 0.5f, 0.53f),
                Scale = 1.5f
            };
        }




        /// <summary>
        /// Makes sure that Translation and Rotation is not null
        /// </summary>
        public ModelTransform EnsureDefaultValues()
        {
            // If we are using the common default Translation, provide a new Vec3f() here as calling code is probably then going to adjust it (why else call EnsureDefaultValues())
            if (Translation.Equals(defaultTf)) Translation = new FastVec3f();
            if (Rotation.Equals(defaultTf)) Rotation = new FastVec3f();
            return this;
        }

        public ModelTransform WithRotation(Vec3f rot)
        {
            Rotation = new FastVec3f(rot);
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
                Rotation = Rotation,
                Translation = Translation,
                ScaleXYZ = ScaleXYZ,
                Origin = Origin
            };
        }


        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (Translation.Equals(defaultTf)) Translation = new FastVec3f();
            if (Rotation.Equals(defaultTf)) Rotation = new FastVec3f();
        }
    }
}
