using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// A universal item slot type that can hold anything.
    /// </summary>
    public class ItemSlotUniversal : ItemSlot
    {
        public ItemSlotUniversal(InventoryBase inventory) : base(inventory)
        {
        }

        public override EnumItemStorageFlags StorageType
        {
            get
            {
                return EnumItemStorageFlags.Agriculture | EnumItemStorageFlags.Alchemy | EnumItemStorageFlags.Backpack | EnumItemStorageFlags.General | EnumItemStorageFlags.Jewellery | EnumItemStorageFlags.Currency | EnumItemStorageFlags.Metallurgy | EnumItemStorageFlags.Outfit;
            }
        }
    }
}
