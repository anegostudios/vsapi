using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Used for blocks (e.g. chiselled blocks) where the basic block geometry: AO shading, side opaque etc - depends on the individual block or blockEntity
    /// </summary>
    public interface IGeometryTester
    {
        BlockEntity GetCurrentBlockEntityOnSide(BlockFacing side);
        BlockEntity GetCurrentBlockEntityOnSide(Vec3iAndFacingFlags vec);
    }
}
