using Vintagestory.API.MathTools;
using VintagestoryAPI.Math.Vector;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Used for blocks (e.g. crops and dead crops) where the y-position might need to be adjusted at the time of tesselating an individual block
    /// </summary>
    public interface IDrawYAdjustable
    {
        float AdjustYPosition(Block[] chunkExtBlocks, int extIndex3d);
    }
}
