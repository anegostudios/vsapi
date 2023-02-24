using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Implement this interface if schematics containing this block entity needs to modify it's tree attribute data. Beware, this method is called without
    /// the block entity existing in the world (yet). The modified tree will then be used to actually create the block
    /// </summary>
    public interface IRotatable
    {
        /// <summary>
        /// If flipAxis is null it means it was not flipped, only horizontally rotated. Apply flip first, and then rotation.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="degreeRotation"></param>
        /// <param name="flipAxis"></param>
        void OnTransformed(ITreeAttribute tree, int degreeRotation, EnumAxis? flipAxis);
    }

    public interface IMaterialExchangeable
    {
        void ExchangeWith(ItemSlot fromSlot, ItemSlot toSlot);
    }
}
