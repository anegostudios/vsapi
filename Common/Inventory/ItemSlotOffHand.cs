
#nullable disable
namespace Vintagestory.API.Common
{
    /// <summary>
    /// A slot that only accepts collectibles designated for the off-hand slot
    /// </summary>
    public class ItemSlotOffhand : ItemSlot
    {
        public override EnumItemStorageFlags StorageType { get { return EnumItemStorageFlags.Offhand; } }

        public ItemSlotOffhand(InventoryBase inventory) : base(inventory)
        {
            this.BackgroundIcon = "left_hand";
        }
    }

    public class ItemSlotSkill : ItemSlot
    {
        public override EnumItemStorageFlags StorageType { get { return EnumItemStorageFlags.Skill; } }

        public ItemSlotSkill(InventoryBase inventory) : base(inventory)
        {
            this.BackgroundIcon = "skill";
        }
    }
}
