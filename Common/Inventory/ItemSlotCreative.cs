using System;
using Vintagestory.API.Config;

#nullable disable

namespace Vintagestory.API.Common
{
    public class ItemSlotCreative : ItemSlot
    {
        public ItemSlotCreative(InventoryBase inventory) : base(inventory)
        {
        }

        public override ItemStack TakeOutWhole()
        {
            ItemStack stack = Itemstack.Clone();
            return stack;
        }

        public override ItemStack TakeOut(int quantity)
        {
            ItemStack split = Itemstack.GetEmptyClone();
            split.StackSize = quantity;
            return split;
        }


        public override int TryPutInto(ItemSlot sinkSlot, ref ItemStackMoveOperation op)
        {
            if (!sinkSlot.CanTakeFrom(this) || !CanTake() || Itemstack == null) return 0;

            if (sinkSlot.Inventory?.CanContain(sinkSlot, this) == false) return 0;

            // Fill up sink slot
            if (op.ShiftDown)
            {
                if (Empty) return 0;

                int maxstacksize = Itemstack.Collectible.MaxStackSize;

                if (sinkSlot.Itemstack == null)
                {
                    op.RequestedQuantity = maxstacksize;
                }
                else
                {
                    op.RequestedQuantity = maxstacksize - sinkSlot.StackSize;
                }
            }

            // Fill the destination slot with as many items as we can
            if (sinkSlot.Itemstack == null)
            {
                int q = Math.Min(sinkSlot.GetRemainingSlotSpace(itemstack), op.RequestedQuantity);

                sinkSlot.Itemstack = TakeOut(q);
                sinkSlot.OnItemSlotModified(sinkSlot.Itemstack);
                op.MovedQuantity = sinkSlot.StackSize;
                return op.MovedQuantity;
            }

            ItemStack ownStack = Itemstack.Clone();

            ItemStackMergeOperation mergeop = op.ToMergeOperation(sinkSlot, this);
            op = mergeop;
            int origRequestedQuantity = op.RequestedQuantity;
            op.RequestedQuantity = Math.Min(sinkSlot.GetRemainingSlotSpace(itemstack), op.RequestedQuantity);

            sinkSlot.Itemstack.Collectible.TryMergeStacks(mergeop);

            // Ignore any changes made to our slot
            Itemstack = ownStack;

            if (mergeop.MovedQuantity > 0)
            {
                sinkSlot.OnItemSlotModified(sinkSlot.Itemstack);
            }

            op.RequestedQuantity = origRequestedQuantity; //ensures op.NotMovedQuantity will be correct in calling code if used with slots with limited slot maxStackSize, e.g. InventorySmelting with a cooking container has slots with maxStackSize == 6
            return op.MovedQuantity;
        }

        protected override void ActivateSlotLeftClick(ItemSlot sinkSlot, ref ItemStackMoveOperation op)
        {
            // 1. Current slot empty: Remove items
            if (Empty) {
                sinkSlot.TakeOutWhole();
                return;
            }

            // 2. Current slot non empty, source slot empty: Put items
            if (sinkSlot.Empty) {
                op.RequestedQuantity = StackSize;
                TryPutInto(sinkSlot, ref op);
                return;
            }

            // 3. Both slots not empty, and they are the same: Fill source slot
            if (sinkSlot.Itemstack.Equals(op.World, Itemstack, GlobalConstants.IgnoredStackAttributes))
            {
                op.RequestedQuantity = 1;
                ItemStackMergeOperation mergeop = op.ToMergeOperation(sinkSlot, this);
                op = mergeop;

                ItemStack ownStack = Itemstack.Clone();

                sinkSlot.Itemstack.Collectible.TryMergeStacks(mergeop);

                // Ignore any changes made to our slot
                Itemstack = ownStack;

                return;
            }

            // 4. Both slots not empty and not stackable: Remove items
            sinkSlot.TakeOutWhole();
        }

        protected override void ActivateSlotMiddleClick(ItemSlot sinkSlot, ref ItemStackMoveOperation op)
        {
            if (Empty) return;

            sinkSlot.Itemstack = Itemstack.Clone();
            sinkSlot.Itemstack.StackSize = Itemstack.Collectible.MaxStackSize;
            op.MovedQuantity = Itemstack.Collectible.MaxStackSize;
            sinkSlot.OnItemSlotModified(sinkSlot.Itemstack);
        }

        protected override void ActivateSlotRightClick(ItemSlot sourceSlot, ref ItemStackMoveOperation op)
        {
            // 1. Current slot empty: Take 1 item
            if (Empty)
            {
                return;
            }

            // 2. Current slot non empty, source slot empty: Put half items
            if (sourceSlot.Empty)
            {
                op.RequestedQuantity = 1;
                sourceSlot.TryPutInto(this, ref op);
                return;
            }

            // 3. Both slots not empty, and they are stackable: Fill slot with 1 item
            sourceSlot.TakeOut(1);
        }


        public override bool TryFlipWith(ItemSlot itemSlot)
        {
            bool canHoldHis = itemSlot.Empty || CanHold(itemSlot);
            bool canIExchange = canHoldHis && (Empty || CanTake());

            bool canHoldMine = Empty || itemSlot.CanHold(this);
            bool canHeExchange = canHoldMine && (itemSlot.Empty || itemSlot.CanTake());

            if (canIExchange && canHeExchange)
            {
                itemSlot.Itemstack = Itemstack.Clone();
                itemSlot.OnItemSlotModified(itemSlot.Itemstack);
                return true;
            }

            return false;
        }

        protected override void FlipWith(ItemSlot withslot)
        {
            ItemStack returnedStack = Itemstack.Clone();

            if (withslot.Itemstack != null && withslot.Itemstack.Equals(Itemstack))
            {
                returnedStack.StackSize += withslot.Itemstack.StackSize;
            }

            withslot.Itemstack = returnedStack;
        }

        public override void OnItemSlotModified(ItemStack sinkStack)
        {
            if (itemstack != null) itemstack.StackSize = 1;
            base.OnItemSlotModified(sinkStack);
        }
    }
}
