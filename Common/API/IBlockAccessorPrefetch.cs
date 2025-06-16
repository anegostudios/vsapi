using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Useful for when you have to scan multiple times over the same set of blocks
    /// </summary>
    public interface IBlockAccessorPrefetch : IBlockAccessor
    {
        /// <summary>
        /// Pre-loads all blocks inside given area which can then be accessed very quickly using .GetBlock(). 
        /// This method must be called before using GetBlock()
        /// </summary>
        /// <param name="minPos"></param>
        /// <param name="maxPos"></param>
        void PrefetchBlocks(BlockPos minPos, BlockPos maxPos);

    }
}
