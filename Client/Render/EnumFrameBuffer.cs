
#nullable disable
namespace Vintagestory.API.Client
{
    public enum EnumFrameBuffer
    {
        Default = -1,
        Primary = 0,
        Transparent = 1,
        BlurHorizontalMedRes = 2,
        BlurVerticalMedRes = 3,
        FindBright = 4,
        LiquidDepth = 5,
        GodRays = 7,
        BlurVerticalLowRes = 8,
        BlurHorizontalLowRes = 9,
        Luma = 10,
        ShadowmapFar = 11,
        ShadowmapNear = 12,
        SSAO = 13,

        SSAOBlurVertical = 14,
        SSAOBlurHorizontal = 15,
        SSAOBlurVerticalHalfRes = 16,
        SSAOBlurHorizontalHalfRes = 17
    }
}
