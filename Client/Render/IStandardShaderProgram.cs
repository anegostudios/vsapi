using Vintagestory.API.MathTools;

#nullable disable

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

        int NormalShaded { set; }
        int TempGlowMode { set; }

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

        float DamageEffect { set; }

        float ExtraZOffset { set; }

        float ExtraGodray { set; }

        /// <summary>
        /// The color of the ambient light source.
        /// </summary>
        Vec3f RgbaAmbientIn { set; }

        /// <summary>
        /// The color of the general light.
        /// </summary>
        Vec4f RgbaLightIn { set; }

        /// <summary>
        /// The color of the glow light.
        /// </summary>
        Vec4f RgbaGlowIn { set; }

        /// <summary>
        /// The color of the fog.
        /// </summary>
        Vec4f RgbaFogIn { set; }

        /// <summary>
        /// The color of the tint.
        /// </summary>
        Vec4f RgbaTint { set; }

        /// <summary>
        /// When TempGlowMode==1 this color is used as a reference
        /// </summary>
        Vec4f AverageColor { set; }

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
        /// Required for water waving meshes.  Supplied the water counter as supplied as IRenderAPI.WaterWaveCounter.
        /// </summary>
        float WaterWaveCounter { set; }

        /// <summary>
        /// The position of the player.
        /// </summary>
        //Vec3f Playerpos { set; }


        int DontWarpVertices { set; }

        int AddRenderFlags { set; }


        int Tex2dOverlay2D { set; }
        float OverlayOpacity { set; }

        float SsaoAttn { set; }

        Vec2f OverlayTextureSize { set; }
        Vec2f BaseTextureSize { set; }
        Vec2f BaseUvOrigin { set; }

    }
}
