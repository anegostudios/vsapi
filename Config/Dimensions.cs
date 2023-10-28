using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
