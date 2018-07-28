
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Determines the kinds of storage types the item can be put into
    /// </summary>
    [Flags]
    public enum EnumItemStorageFlags
    {
        /// <summary>
        /// Of no particular type
        /// </summary>
        General = 1,
        /// <summary>
        /// The item can be placed into a backpack slot
        /// </summary>
        Backpack = 2,
        /// <summary>
        /// The item can be placed in a slot related to mining or smithing
        /// </summary>
        Metallurgy = 4,
        /// <summary>
        /// The item can be placed in a slot related to jewelcrafting
        /// </summary>
        Jewellery = 8,
        /// <summary>
        /// The item can be placed in a slot related to alchemy
        /// </summary>
        Alchemy = 16,
        /// <summary>
        /// The item can be placed in a slot related to farming
        /// </summary>
        Agriculture = 32,
        /// <summary>
        /// Moneys
        /// </summary>
        Currency = 64,
        /// <summary>
        /// Clothes, Armor and Accessories
        /// </summary>
        Outfit = 128,
    }
}
