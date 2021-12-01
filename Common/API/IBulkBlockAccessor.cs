using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{

    /// <summary>
    /// Useful for setting many blocks at once efficiently
    /// </summary>
    public interface IBulkBlockAccessor : IBlockAccessor
    {
        /// <summary>
        /// The full list of staged blocks that will get commited after calling Commit()
        /// </summary>
        Dictionary<BlockPos, BlockUpdate> StagedBlocks { get; }

        /// <summary>
        /// If set to true, the methods GetBlock() and GetBlockId() will behave like GetStagedBlockId() until the next commit
        /// </summary>
        bool ReadFromStagedByDefault { get; set; }

        /// <summary>
        /// Gets the block for a not yet commited block. If no block has been staged for this pos the original block is returned
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <returns></returns>
        int GetStagedBlockId(int posX, int posY, int posZ);

        /// <summary>
        /// Gets the block for a not yet commited block. If no block has been staged for this pos the original block is returned
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <returns></returns>
        int GetStagedBlockId(BlockPos pos);

        /// <summary>
        /// Implemented only by BlockAccessorMapChunkLoading
        /// </summary>
        void SetChunks(Vec2i chunkCoord, IWorldChunk[] chunksCol);
        void SetDecorsBulk(long chunkIndex, Dictionary<int, Block> newDecors);
    }
}
