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
        int Tex2D { set; }
        int ShadowMapNear2D { set; }
        int ShadowMapFar2D { set; }
        float ZNear { set; }
        float ZFar { set; }
        float AlphaTest { set; }
        Vec3f RgbaAmbientIn { set; }
        Vec4f RgbaLightIn { set; }
        Vec4f RgbaBlockIn { set; }
        Vec4f RgbaFogIn { set; }
        Vec4f RgbaTint { set; }
        float FogMinIn { set; }
        float FogDensityIn { set; }
        float[] ProjectionMatrix { set; }
        float[] ModelMatrix { set; }
        float[] ViewMatrix { set; }

        int ExtraGlow { set; }

        float[] ToShadowMapSpaceMatrixFar { set; }
        float[] ToShadowMapSpaceMatrixNear { set; }
        
        int WaterWave { set; }
        float WaterWaveCounter { set; }
        Vec3f Playerpos { set; }
    }
}
