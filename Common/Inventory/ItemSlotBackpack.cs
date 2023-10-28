namespace Vintagestory.API.Common
{
    /// <summary>
    /// A slot that can hold mobile containers
    /// </summary>
    public class ItemSlotBackpack : ItemSlot
    {
        public override EnumItemStorageFlags StorageType { get { return EnumItemStorageFlags.Backpack; } }

        public ItemSlotBackpack(InventoryBase inventory) : base(inventory)
        {
            this.BackgroundIcon = "basket";
        }
    }
}
