
#nullable disable
namespace Vintagestory.API.Common
{
    public interface IBlockFlowing
    {
        string Flow { get; set; }
        MathTools.Vec3i FlowNormali { get; set; }
        bool IsLava { get; }
    }
}
