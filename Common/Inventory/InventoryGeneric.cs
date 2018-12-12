using System;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace Vintagestory.API.Common
{

    public delegate ItemSlot NewSlotDelegate(int slotId, InventoryGeneric self);

    /// <summary>
    /// A general purpose inventory
    /// </summary>
    public class InventoryGeneric : InventoryBase
    {
        ItemSlot[] slots;
        NewSlotDelegate onNewSlot = null;

        /// <summary>
        /// Create a new general purpose inventory
        /// </summary>
        /// <param name="quantitySlots"></param>
        /// <param name="className"></param>
        /// <param name="instanceId"></param>
        /// <param name="api"></param>
        /// <param name="onNewSlot"></param>
        public InventoryGeneric(int quantitySlots, string className, string instanceId, ICoreAPI api, NewSlotDelegate onNewSlot = null) : base(className, instanceId, api)
        {
            this.onNewSlot = onNewSlot;

            slots = GenEmptySlots(quantitySlots);
        }

        /// <summary>
        /// Create a new general purpose inventory
        /// </summary>
        /// <param name="quantitySlots"></param>
        /// <param name="invId"></param>
        /// <param name="api"></param>
        /// <param name="onNewSlot"></param>
        public InventoryGeneric(int quantitySlots, string invId, ICoreAPI api, NewSlotDelegate onNewSlot = null) : base (invId, api)
        {
            this.onNewSlot = onNewSlot;

            slots = GenEmptySlots(quantitySlots);
        }

        /// <summary>
        /// Amount of available slots
        /// </summary>
        public override int Count
        {
            get { return slots.Length; }
        }

        /// <summary>
        /// Get slot for given slot index
        /// </summary>
        /// <param name="slotId"></param>
        /// <returns></returns>
        public override ItemSlot this[int slotId]
        {
            get
            {
                if (slotId < 0 || slotId >= Count) return null;
                return slots[slotId];
            }
            set
            {
                if (slotId < 0 || slotId >= Count) throw new ArgumentOutOfRangeException(nameof(slotId));
                if (value == null) throw new ArgumentNullException(nameof(value));
                slots[slotId] = value;
            }
        }

        /// <summary>
        /// True if all slots are empty
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].Itemstack != null) return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Loads the slot contents from given treeAttribute
        /// </summary>
        /// <param name="treeAttribute"></param>
        public override void FromTreeAttributes(ITreeAttribute treeAttribute)
        {
            slots = SlotsFromTreeAttributes(treeAttribute);
        }

        /// <summary>
        /// Stores the slot contents to invtree
        /// </summary>
        /// <param name="invtree"></param>
        public override void ToTreeAttributes(ITreeAttribute invtree)
        {
            SlotsToTreeAttributes(slots, invtree);
        }

        /// <summary>
        /// Called when initializing the inventory or when loading the contents
        /// </summary>
        /// <param name="slotId"></param>
        /// <returns></returns>
        protected override ItemSlot NewSlot(int slotId)
        {
            if (onNewSlot != null) return onNewSlot(slotId, this);
            return new ItemSlotSurvival(this);
        }

    }
}
