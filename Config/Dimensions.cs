using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

#nullable disable

namespace Vintagestory.API.Config
{
    public class Dimensions
    {
        /// <summary>
        /// Used to make explicit all places in the code where the default dimension, dimension 0, is assumed, in case we need to search and replace them later
        /// </summary>
        public const int NormalWorld = 0;
        /// <summary>
        /// The dimension dedicated for storage of 'mini-dimensions', i.e. many separate 16k cubes of block space used for WorldEdit preview, vehicles etc
        /// </summary>
        public const int MiniDimensions = 1;
        /// <summary>
        /// The dimension dedicated for storage of blocks in an timeswitched alt-world, such as the Devastation
        /// </summary>
        public const int AltWorld = 2;
        /// <summary>
        /// The subdimension Id, within the MiniDimensions system, for the World Edit Blocks Preview dimension.  Accessed client-side only
        /// </summary>
        public static int BlocksPreviewSubDimension_Client = -1;


        /// <summary>
        /// This represents the XZ size of mini-dimensions (individual BlockAccessorMovables) within an overall dimension - we can pack 16 million of these into one dimension, which should be enough!
        /// </summary>
        public const int subDimensionSize = BlockPos.DimensionBoundary / 2;    // DimensionBoundary is intentionally set to twice the subDimensionSize, to help geometry separation between dimensions for better safety
        public const int subDimensionIndexZMultiplier = GlobalConstants.MaxWorldSizeXZ / subDimensionSize;  // CHANGING THESE CONSTANTS COULD DAMAGE EXISTING SAVE GAMES, making mini dimensions unfindable

        public static int SubDimensionIdForPos(int posX, int posZ)
        {
            return posZ / subDimensionSize * subDimensionIndexZMultiplier + posX / subDimensionSize;
        }

        /// <summary>
        /// Indicates whether a given BlockPos should not be ticked due to being in an unusual dimension, such as the preview minidimension
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="api"></param>
        /// <returns></returns>
        public static bool ShouldNotTick(BlockPos pos, ICoreAPI api)
        {
            if (pos.dimension != MiniDimensions) return false;
            int subId = SubDimensionIdForPos(pos.X, pos.Z);
            if (!(api is ICoreServerAPI sapi)) return subId == BlocksPreviewSubDimension_Client;
            IMiniDimension dim = sapi.Server.GetMiniDimension(subId);
            if (dim == null) return false;
            return dim.BlocksPreviewSubDimension_Server == subId;
        }


        /// <summary>
        /// Indicates whether a given BlockPos should not be ticked due to being in an unusual dimension, such as the preview minidimension
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="api"></param>
        /// <returns></returns>
        public static bool ShouldNotTick(EntityPos pos, ICoreAPI api)
        {
            if (pos.Dimension != MiniDimensions) return false;
            int subId = SubDimensionIdForPos(pos.XInt, pos.ZInt);
            if (!(api is ICoreServerAPI sapi)) return subId == BlocksPreviewSubDimension_Client;
            IMiniDimension dim = sapi.Server.GetMiniDimension(subId);
            if (dim == null) return false;
            return dim.BlocksPreviewSubDimension_Server == subId;
        }
    }
}
