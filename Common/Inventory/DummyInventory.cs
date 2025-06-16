using Vintagestory.API.Datastructures;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// A place holder inventory, useful, e.g., for when you want to render an itemstack and not have it spoil
    /// </summary>
    public class DummyInventory : InventoryBase
    {
        static int dummyId = 1;
        ItemSlot[] slots;
        int quantitySlots = 1;

        public ItemSlot[] Slots => slots;

        public DummyInventory(ICoreAPI api, int quantitySlots = 1) : this("dummy-" + (dummyId++), api)
        {
            this.quantitySlots = quantitySlots;

            slots = GenEmptySlots(quantitySlots);
        }

        private DummyInventory(string inventoryID, ICoreAPI api) : base(inventoryID, api)
        {
        }

        private DummyInventory(string className, string instanceID, ICoreAPI api) : base(className, instanceID, api)
        {
        }

        public override ItemSlot this[int slotId] { get => slots[slotId]; set => slots[slotId] = value; }
        public override int Count => quantitySlots;

        public override void FromTreeAttributes(ITreeAttribute tree)
        {
            slots = SlotsFromTreeAttributes(tree, slots);
        }

        public override void ToTreeAttributes(ITreeAttribute tree)
        {
            SlotsToTreeAttributes(slots, tree);
        }

        public override float GetTransitionSpeedMul(EnumTransitionType transType, ItemStack stack)
        {
            return base.GetTransitionSpeedMul(transType, stack);
        }
    }
}

