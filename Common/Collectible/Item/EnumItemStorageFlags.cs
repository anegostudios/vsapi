
using System;

#nullable disable

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
        /// <summary>
        /// Off hand slot
        /// </summary>
        Offhand = 256,
        /// <summary>
        /// Arrows
        /// </summary>
        Arrow = 512,
        /// <summary>
        /// Skill slot
        /// </summary>
        Skill = 1024,
        /// <summary>
        /// Custom storage flag for mods
        /// </summary>
        Custom1 = 2048,
        /// <summary>
        /// Custom storage flag for mods
        /// </summary>
        Custom2 = 4096,
        /// <summary>
        /// Custom storage flag for mods
        /// </summary>
        Custom3 = 8192,
        /// <summary>
        /// Custom storage flag for mods
        /// </summary>
        Custom4 = 16384,
        /// <summary>
        /// Custom storage flag for mods
        /// </summary>
        Custom5 = 32768,
        /// <summary>
        /// Custom storage flag for mods
        /// </summary>
        Custom6 = 65536,
        /// <summary>
        /// Custom storage flag for mods
        /// </summary>
        Custom7 = 131072,
        /// <summary>
        /// Custom storage flag for mods
        /// </summary>
        Custom8 = 262144,
        /// <summary>
        /// Custom storage flag for mods
        /// </summary>
        Custom9 = 524288,
        /// <summary>
        /// Custom storage flag for mods
        /// </summary>
        Custom10 = 1048576,
    }
}
