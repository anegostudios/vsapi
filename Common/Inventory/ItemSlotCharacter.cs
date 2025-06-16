using System;

#nullable disable

namespace Vintagestory.API.Common
{
    public enum EnumCharacterDressType
    {
        Unknown = -1,
        Head = 0,
        Shoulder = 1,
        UpperBody = 2,
        UpperBodyOver = 11,
        LowerBody = 3,
        Foot = 4,

        Neck = 6,
        Emblem = 7,
        Face = 8,
        Arm = 10,
        Hand = 5,
        Waist = 9,

        ArmorHead = 12,
        ArmorBody = 13,
        ArmorLegs = 14
    }

    public class ItemSlotCharacter : ItemSlot
    {
        public override EnumItemStorageFlags StorageType => EnumItemStorageFlags.Outfit;

        public EnumCharacterDressType Type;

        public ItemSlotCharacter(EnumCharacterDressType type, InventoryBase inventory) : base(inventory)
        {
            this.Type = type;
        }

        public override bool CanTakeFrom(ItemSlot sourceSlot, EnumMergePriority priority = EnumMergePriority.AutoMerge)
        {
            if (!IsDressType(sourceSlot.Itemstack, Type)) return false;
            return base.CanTakeFrom(sourceSlot, priority);
        }

        public override bool CanHold(ItemSlot itemstackFromSourceSlot)
        {
            if (!IsDressType(itemstackFromSourceSlot.Itemstack, Type)) return false;

            return base.CanHold(itemstackFromSourceSlot);
        }

        /// <summary>
        /// Checks to see what dress type the given item is.
        /// </summary>
        /// <param name="itemstack"></param>
        /// <param name="dressType"></param>
        /// <returns></returns>
        public static bool IsDressType(IItemStack itemstack, EnumCharacterDressType dressType)
        {
            if (itemstack == null || itemstack.Collectible.Attributes == null) return false;

            string stackDressType = itemstack.Collectible.Attributes["clothescategory"].AsString() ?? itemstack.Collectible.Attributes["attachableToEntity"]["categoryCode"].AsString();

            return stackDressType != null && dressType.ToString().Equals(stackDressType, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
