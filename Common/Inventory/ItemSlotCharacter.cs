using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace Vintagestory.API.Common
{
    public enum EnumCharacterDressType
    {
        Head,
        Shoulder,
        UpperBody,
        LowerBody,
        Foot,

        Hand,
        Necklace,
        Emblem,
        Ring,
        Waist
    }

    public class ItemSlotCharacter : ItemSlot
    {
        public override EnumItemStorageFlags StorageType => EnumItemStorageFlags.Outfit;

        EnumCharacterDressType type;

        public ItemSlotCharacter(EnumCharacterDressType type, InventoryBase inventory) : base(inventory)
        {
            this.type = type;
        }

        public override bool CanTakeFrom(IItemSlot sourceSlot)
        {
            if (!IsDressType(sourceSlot.Itemstack, type)) return false;
            return base.CanTakeFrom(sourceSlot);
        }

        public override bool CanHold(IItemSlot itemstackFromSourceSlot)
        {
            if (!IsDressType(itemstackFromSourceSlot.Itemstack, type)) return false;

            return base.CanHold(itemstackFromSourceSlot);
        }


        public static bool IsDressType(IItemStack itemstack, EnumCharacterDressType dressType)
        {
            if (itemstack == null || itemstack.Collectible.Attributes == null) return false;

            string stackDressType = itemstack.Collectible.Attributes["clothescategory"].AsString();

            return stackDressType != null && dressType.ToString().ToLowerInvariant().Equals(stackDressType);
        }
    }
}
