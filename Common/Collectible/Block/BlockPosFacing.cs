using Vintagestory.API.MathTools;

namespace Vintagestory.Common.Collectible.Block;

public struct BlockPosFacing
{
    public BlockPos Position;
    public BlockFacing Facing;
    public string Constraints;

    public BlockPosFacing(BlockPos position, BlockFacing facing, string constraints)
    {
        Position = position;
        Facing = facing;
        Constraints = constraints;
    }

    public BlockPosFacing()
    {
    }
}
