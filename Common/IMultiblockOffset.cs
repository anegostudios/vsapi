using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common;

/// <summary>
/// Used to get the control / main block of a multiblock structure and to test for access right in land claims
/// </summary>
public interface IMultiblockOffset
{
    // This will change the passed in position
    public BlockPos GetControlBlockPos(BlockPos pos);
}
