using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    public interface IStandardShaderProgram : IShaderProgram
    {
        /// <summary>
        /// The texture 2D for this mesh.
        /// </summary>
        int Tex2D { set; }

        /// <summary>
        /// The shadowmap ID for nearby shadows.
        /// </summary>
        int ShadowMapNear2D { set; }

        /// <summary>
        /// The shadowmap ID for far away shadows.
        /// </summary>
        int ShadowMapFar2D { set; }

        /// <summary>
        /// The Near plane distance.
        /// </summary>
        float ZNear { set; }

        /// <summary>
        /// The far plane distance.
        /// </summary>
        float ZFar { set; }

        /// <summary>
        /// The shader will discard things below this threshold.
        /// </summary>
        float AlphaTest { set; }

        /// <summary>
        /// The color of the ambient light source.
        /// </summary>
        Vec3f RgbaAmbientIn { set; }

        /// <summary>
        /// The color of the general light.
        /// </summary>
        Vec4f RgbaLightIn { set; }

        /// <summary>
        /// The color of the block.
        /// </summary>
        Vec4f RgbaBlockIn { set; }

        /// <summary>
        /// The color of the fog.
        /// </summary>
        Vec4f RgbaFogIn { set; }

        /// <summary>
        /// The color of the tint.
        /// </summary>
        Vec4f RgbaTint { set; }

        /// <summary>
        /// The minimum distance for fog.
        /// </summary>
        float FogMinIn { set; }

        /// <summary>
        /// The density level of the fog.
        /// </summary>
        float FogDensityIn { set; }

        /// <summary>
        /// The projection matrix.
        /// </summary>
        float[] ProjectionMatrix { set; }

        /// <summary>
        /// The model Matrix.
        /// </summary>
        float[] ModelMatrix { set; }

        /// <summary>
        /// The view matrix.
        /// </summary>
        float[] ViewMatrix { set; }

        int ExtraGlow { set; }

        /// <summary>
        /// The matrix for converting the vertex position from world space to far shadow space as supplied by IRenderAPI.CurrentShadowProjectionMatrix.
        /// </summary>
        float[] ToShadowMapSpaceMatrixFar { set; }

        /// <summary>
        /// The matrix for converting the vertex position from world space to near shadow space as supplied by IRenderAPI.CurrentShadowProjectionMatrix.
        /// </summary>
        float[] ToShadowMapSpaceMatrixNear { set; }
        
        /// <summary>
        /// If set to 1, the mesh will have a water waving effect applied.
        /// </summary>
        int WaterWave { set; }
        
        /// <summary>
        /// Required for water waving meshes.  Supplied the water counter as supplied as IRenderAPI.WaterWaveCounter.
        /// </summary>
        float WaterWaveCounter { set; }

        /// <summary>
        /// The position of the player.
        /// </summary>
        Vec3f Playerpos { set; }
    }
}
