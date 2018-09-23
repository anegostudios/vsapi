using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace Vintagestory.API.Common
{
    public delegate ItemSlot NewSlotDelegate(int slotId, InventoryGeneric self);

    public class InventoryGeneric : InventoryBase
    {
        ItemSlot[] slots;
        NewSlotDelegate onNewSlot = null;

        public InventoryGeneric(int quantitySlots, string className, string instanceId, ICoreAPI api, NewSlotDelegate onNewSlot = null) : base(className, instanceId, api)
        {
            this.onNewSlot = onNewSlot;

            slots = GenEmptySlots(quantitySlots);
        }

        public InventoryGeneric(int quantitySlots, string invId, ICoreAPI api, NewSlotDelegate onNewSlot = null) : base (invId, api)
        {
            this.onNewSlot = onNewSlot;

            slots = GenEmptySlots(quantitySlots);
        }

        public override int QuantitySlots
        {
            get { return slots.Length; }
        }

        public bool IsEmpty { get {
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].Itemstack != null) return false;
                }

                return true;
        } }

        public override ItemSlot GetSlot(int slotId)
        {
            return slots[slotId];
        }

        public override void FromTreeAttributes(ITreeAttribute treeAttribute)
        {
            slots = SlotsFromTreeAttributes(treeAttribute);
        }

        public override void ToTreeAttributes(ITreeAttribute invtree)
        {
            SlotsToTreeAttributes(slots, invtree);
        }

        protected override ItemSlot NewSlot(int slotId)
        {
            if (onNewSlot != null) return onNewSlot(slotId, this);
            return new ItemSlotSurvival(this);
        }

    }
}
