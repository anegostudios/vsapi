using Vintagestory.API.Common;

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
            this.BackgroundIcon = "offhand";
        }

    }
}
