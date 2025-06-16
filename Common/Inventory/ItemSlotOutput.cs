
#nullable disable
namespace Vintagestory.API.Common
{
    /// <summary>
    /// A slot from which the player can only take stuff out of, but not place anything in it
    /// </summary>
    public class ItemSlotOutput : ItemSlot
    {
        public ItemSlotOutput(InventoryBase inventory) : base(inventory)
        {
        }

        public override bool CanHold(ItemSlot itemstackFromSourceSlot)
        {
            return false;
        }

        public override bool CanTake()
        {
            return true;
        }

        public override bool CanTakeFrom(ItemSlot sourceSlot, EnumMergePriority priority = EnumMergePriority.AutoMerge)
        {
            return false;
        }

       
        public override void ActivateSlot(ItemSlot sourceSlot, ref ItemStackMoveOperation op)
        {
            if (Empty) return;
            if (sourceSlot.CanHold(this))
            {
                if (sourceSlot.Itemstack != null && sourceSlot.Itemstack != null && sourceSlot.Itemstack.Collectible.GetMergableQuantity(sourceSlot.Itemstack, itemstack, op.CurrentPriority) < itemstack.StackSize) return;

                op.RequestedQuantity = StackSize;

                TryPutInto(sourceSlot, ref op);

                if (op.MovedQuantity > 0)
                {
                    OnItemSlotModified(itemstack);
                }
            }
        }
    }
}
