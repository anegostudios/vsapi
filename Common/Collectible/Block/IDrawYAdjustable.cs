using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Used for blocks (e.g. crops and dead crops) where the y-position might need to be adjusted at the time of tesselating an individual block
    /// </summary>
    public interface IDrawYAdjustable
    {
        float AdjustYPosition(BlockPos pos, Block[] chunkExtBlocks, int extIndex3d);
    }
}
