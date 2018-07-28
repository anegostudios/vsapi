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
        /// Gets the block if for a not yet commited block. If no block has been staged for this pos the original block is return
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <returns></returns>
        ushort GetStagedBlockId(int posX, int posY, int posZ);

        /// <summary>
        /// Gets the block if for a not yet commited block. If no block has been staged for this pos the original block is return
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <returns></returns>
        ushort GetStagedBlockId(BlockPos pos);

    }
}
