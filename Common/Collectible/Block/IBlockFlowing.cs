
#nullable disable
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public interface IBlockFlowing
    {
        string Flow { get; set; }
        MathTools.Vec3i FlowNormali { get; set; }
        bool IsLava { get; }
        bool IsStill { get; }
        bool HasNormalWaves { get; }
        FastVec3f GetPushVector(BlockPos pos);
        float FlowRate(BlockPos pos);
    }
}
