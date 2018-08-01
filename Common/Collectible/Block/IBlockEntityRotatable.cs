using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Datastructures;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Implement this interface if schematics containing this block entity needs to modify it's tree attribute data. Beware, this method is called without
    /// the block entity existing in the world (yet). The modified tree will then be used to actually create the block
    /// </summary>
    public interface IBlockEntityRotatable
    {
        void OnRotated(ITreeAttribute tree, int byDegrees, bool flipVertical = false);
    }
}
