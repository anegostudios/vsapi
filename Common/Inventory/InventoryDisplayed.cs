
#nullable disable
namespace Vintagestory.API.Common
{

    /// <summary>
    /// A general purpose inventory which sends update packets to nearby players (used for rendering Display Case, Pulveriser, etc)
    /// </summary>
    public class InventoryDisplayed : InventoryGeneric
    {
        private readonly BlockEntity container;

        public InventoryDisplayed(BlockEntity be, int quantitySlots, string invId, ICoreAPI api, NewSlotDelegate onNewSlot = null) : base(quantitySlots, invId, api, onNewSlot)
        {
            this.container = be;
        }

        /// <summary>
        /// Called when one of the containing slots has been modified (the base version of this is empty)
        /// </summary>
        public override void OnItemSlotModified(ItemSlot slot)
        {
            base.OnItemSlotModified(slot);
            container.MarkDirty(true);
        }
    }
}
