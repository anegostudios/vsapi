using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Vintagestory.API.Common
{
    public class ItemSlot : IItemSlot
    {
        protected ItemStack itemstack;
        protected InventoryBase inventory;
        
        public IInventory Inventory { get { return inventory; } }

        /// <summary>
        /// Icon name to be drawn in the slot background
        /// </summary>
        public string BackgroundIcon;

        public ItemStack Itemstack
        {
            get { return itemstack; }
            set { itemstack = (ItemStack)value; }
        }

        public int StackSize
        {
            get { return itemstack == null ? 0 : itemstack.StackSize; }
        }

        public virtual bool Empty { get { return itemstack == null;  } }
        public virtual EnumItemStorageFlags StorageType { get; set; } = EnumItemStorageFlags.General | EnumItemStorageFlags.Agriculture | EnumItemStorageFlags.Alchemy | EnumItemStorageFlags.Jewellery | EnumItemStorageFlags.Metallurgy | EnumItemStorageFlags.Outfit;
        
        public ItemSlot(InventoryBase inventory)
        {
            this.inventory = inventory;
        }



      
        public virtual bool CanTakeFrom(IItemSlot sourceSlot)
        {
            if (inventory?.PutLocked == true) return false;

            ItemStack sourceStack = sourceSlot.Itemstack;
            if (sourceStack == null) return false;

            bool flagsok = (sourceStack.Collectible.GetStorageFlags(sourceStack) & StorageType) > 0;

            return flagsok &&  (itemstack == null || itemstack.Collectible.GetMergableQuantity(itemstack, sourceStack) > 0);
        }

        public virtual bool CanHold(IItemSlot sourceSlot)
        {
            if (inventory?.PutLocked == true) return false;

            return sourceSlot?.Itemstack?.Collectible != null && ((sourceSlot.Itemstack.Collectible.GetStorageFlags(sourceSlot.Itemstack) & StorageType) > 0);
        }


        public virtual bool CanTake()
        {
            if (inventory?.TakeLocked == true) return false;

            return itemstack != null;
        }
        

        public virtual ItemStack TakeOutWhole()
        {
            ItemStack stack = itemstack.Clone();
            itemstack = null;
            OnItemSlotModified(stack);

            return stack;
        }

        public virtual ItemStack TakeOut(int quantity)
        {
            if (itemstack == null) return null;
            if (quantity >= itemstack.StackSize) return TakeOutWhole();
            ItemStack split = itemstack.GetEmptyClone();
            split.StackSize = quantity;
            itemstack.StackSize -= quantity;

            if (itemstack.StackSize <= 0) itemstack = null;

            return split;
        }

        public virtual void TryPutInto(IWorldAccessor world, IItemSlot sinkSlot)
        {
            ItemStackMoveOperation op = new ItemStackMoveOperation(world, EnumMouseButton.Left, 0, EnumMergePriority.AutoMerge, 1);
            TryPutInto(sinkSlot, ref op);
        }

        /// <summary>
        /// Returns the quantity of items that were not merged (left over in the source slot)
        /// </summary>
        /// <param name="sinkSlot"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public virtual void TryPutInto(IItemSlot sinkSlot, ref ItemStackMoveOperation op)
        {
            if (!sinkSlot.CanTakeFrom(this) || !CanTake() || itemstack == null)
            {
                return;
            }
            

            // Fill the destination slot with as many items as we can
            if (sinkSlot.Itemstack == null)
            {
                sinkSlot.Itemstack = TakeOut(op.RequestedQuantity);
                sinkSlot.OnItemSlotModified(sinkSlot.Itemstack);
                OnItemSlotModified(sinkSlot.Itemstack);

                op.MovedQuantity = op.MovableQuantity = Math.Min(sinkSlot.StackSize, op.RequestedQuantity);
                return;
            }

            ItemStackMergeOperation mergeop = op.ToMergeOperation(sinkSlot, this);
            op = mergeop;

            sinkSlot.Itemstack.Collectible.TryMergeStacks(mergeop);

            if (mergeop.MovedQuantity > 0)
            {
                sinkSlot.OnItemSlotModified(sinkSlot.Itemstack);
                OnItemSlotModified(sinkSlot.Itemstack);
            }
        }

        public virtual bool TryFlipWith(ItemSlot itemSlot)
        {
            bool canHoldHis = itemSlot.Empty || CanHold(itemSlot);
            bool canIExchange = canHoldHis && (Empty || CanTake());

            bool canHoldMine = Empty || itemSlot.CanHold(this);
            bool canHeExchange = canHoldMine && (itemSlot.Empty || itemSlot.CanTake());

            if (canIExchange && canHeExchange)
            {
                itemSlot.FlipWith(this);

                itemSlot.OnItemSlotModified(itemstack);
                OnItemSlotModified(itemSlot.itemstack);
                return true;
            }

            return false;
        }

        protected virtual void FlipWith(ItemSlot withSlot)
        {
            ItemStack temp = withSlot.itemstack;
            withSlot.itemstack = itemstack;
            itemstack = temp;
        }

        public bool TryFlipWith(IItemSlot itemSlot)
        {
            return TryFlipWith((ItemSlot)itemSlot);
        }


        /// <summary>
        /// Call when a player has clicked on this slot. The source slot is the mouse cursor slot. This handles the logic of either taking, putting or exchanging items.
        /// </summary>
        /// <param name="sourceSlot"></param>
        public void ActivateSlot(IItemSlot sourceSlot, ref ItemStackMoveOperation op)
        {
            ActivateSlot((ItemSlot)sourceSlot, ref op);
        }

        public virtual void ActivateSlot(ItemSlot sourceSlot, ref ItemStackMoveOperation op)
        {
            if (Empty && sourceSlot.Empty) return;

            switch (op.MouseButton)
            {
                case EnumMouseButton.Left:
                    ActivateSlotLeftClick(sourceSlot, ref op);
                    return;

                case EnumMouseButton.Middle:
                    ActivateSlotMiddleClick(sourceSlot, ref op);
                    return;

                case EnumMouseButton.Right:
                    ActivateSlotRightClick(sourceSlot, ref op);
                    return;
            }
        }


        protected virtual void ActivateSlotLeftClick(ItemSlot sourceSlot, ref ItemStackMoveOperation op)
        { 
            // 1. Current slot empty: Take items
            if (Empty) {
                if (!CanHold(sourceSlot)) return;

                itemstack = sourceSlot.TakeOutWhole();
                op.MovedQuantity = itemstack.StackSize;
                OnItemSlotModified(itemstack);
                return;
            }
            
            // 2. Current slot non empty, source slot empty: Put items
            if (sourceSlot.Empty) {
                op.RequestedQuantity = StackSize;
                TryPutInto(sourceSlot, ref op);
                return;
            }
            
            // 3. Both slots not empty, and they are stackable: Fill slot
            int maxq = itemstack.Collectible.GetMergableQuantity(itemstack, sourceSlot.itemstack);
            if (maxq > 0) {
                op.RequestedQuantity = Math.Min(maxq, sourceSlot.itemstack.StackSize);

                ItemStackMergeOperation mergeop = op.ToMergeOperation(this, sourceSlot);
                op = mergeop;

                itemstack.Collectible.TryMergeStacks(mergeop);

                sourceSlot.OnItemSlotModified(itemstack);
                OnItemSlotModified(itemstack);

                return;
            }

            // 4. Both slots not empty and not stackable: Exchange items
            TryFlipWith(sourceSlot);
        }


        protected virtual void ActivateSlotMiddleClick(ItemSlot sinkSlot, ref ItemStackMoveOperation op)
        {
            if (Empty) return;

            if (op.ActingPlayer?.WorldData?.CurrentGameMode == EnumGameMode.Creative)
            {
                sinkSlot.Itemstack = Itemstack.Clone();
                op.MovedQuantity = Itemstack.StackSize;
                sinkSlot.OnItemSlotModified(sinkSlot.Itemstack);
            }
        }



        protected virtual void ActivateSlotRightClick(ItemSlot sourceSlot, ref ItemStackMoveOperation op)
        {
            // 1. Current slot empty: Take 1 item
            if (Empty) {
                if (CanHold(sourceSlot))
                {
                    itemstack = (ItemStack)sourceSlot.TakeOut(1);
                    sourceSlot.OnItemSlotModified(itemstack);
                    OnItemSlotModified(itemstack);
                }
                return;
            }

            // 2. Current slot non empty, source slot empty: Put half items
            if (sourceSlot.Empty)
            {
                op.RequestedQuantity = (int)Math.Ceiling(itemstack.StackSize / 2f);
                TryPutInto(sourceSlot, ref op);
                return;
            }

            // 3. Both slots not empty, and they are stackable: Fill slot with 1 item
            op.RequestedQuantity = 1;
            sourceSlot.TryPutInto(this, ref op);
            if (op.MovedQuantity > 0) return;
            

            // 4. Both slots not empty and not stackable: Exchange items
            TryFlipWith(sourceSlot);
        }


        public virtual void OnItemSlotModified(IItemStack sinkStack)
        {
            if (inventory != null)
            {
                inventory.DidModifyItemSlot(this, sinkStack);
            }
            
        }

        public virtual void MarkDirty()
        {
            if (inventory != null)
            {
                inventory.DidModifyItemSlot(this);
            }
        }


        public virtual string GetStackName()
        {
            return itemstack?.GetName();
        }

        public virtual string GetStackDescription(IClientWorldAccessor world, bool extendedDebugInfo)
        {
            return itemstack?.GetDescription(world, extendedDebugInfo);
        }

    }
}
