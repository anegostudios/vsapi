using System.Collections.Generic;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable

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
        /// <param name="worldAccessor"></param>
        /// <param name="tree"></param>
        /// <param name="degreeRotation"></param>
        /// <param name="oldBlockIdMapping">Used for rotation of schematics, so microblocks can update their materials</param>
        /// <param name="oldItemIdMapping"></param>
        /// <param name="flipAxis"></param>
        void OnTransformed(IWorldAccessor worldAccessor, ITreeAttribute tree, int degreeRotation,
            Dictionary<int, AssetLocation> oldBlockIdMapping, Dictionary<int, AssetLocation> oldItemIdMapping,
            EnumAxis? flipAxis);
    }

    public interface IMaterialExchangeable
    {
        bool ExchangeWith(ItemSlot fromSlot, ItemSlot toSlot);
    }
}
