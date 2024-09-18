using Vintagestory.API.MathTools;

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
        /// The dimension dedicated for storage of blocks in an alt-world, such as the Devastation
        /// </summary>
        public const int AltWorld = 2;


        /// <summary>
        /// This represents the XZ size of mini-dimensions (individual BlockAccessorMovables) within an overall dimension - we can pack 16 million of these into one dimension, which should be enough!
        /// </summary>
        public const int subDimensionSize = BlockPos.DimensionBoundary / 2;    // DimensionBoundary is intentionally set to twice the subDimensionSize, to help geometry separation between dimensions for better safety
        public const int subDimensionIndexZMultiplier = GlobalConstants.MaxWorldSizeXZ / subDimensionSize;  // CHANGING THESE CONSTANTS COULD DAMAGE EXISTING SAVE GAMES, making mini dimensions unfindable

        public static int SubDimensionIdForPos(int posX, int posZ)
        {
            return posZ / subDimensionSize * subDimensionIndexZMultiplier + posX / subDimensionSize;
        }
    }
}
