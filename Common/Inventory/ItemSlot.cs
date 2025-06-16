using System;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// The default item slot to item stacks
    /// </summary>
    public class ItemSlot
    {
        /// <summary>
        /// Can be used to interecept marked dirty calls. 
        /// </summary>
        public event ActionConsumable MarkedDirty;


        /// <summary>
        /// The upper holding limit of the slot itself. Standard slots are only limited by the item stacks maxstack size.
        /// </summary>
        public virtual int MaxSlotStackSize { get; set; } = 999999;

        protected ItemStack itemstack;
        protected InventoryBase inventory;
        
        /// <summary>
        /// Gets the inventory attached to this ItemSlot.
        /// </summary>
        public InventoryBase Inventory { get { return inventory; } }


        /// <summary>
        /// Icon name to be drawn in the slot background
        /// </summary>
        public string BackgroundIcon;

        public virtual bool DrawUnavailable { get; set; }

        /// <summary>
        /// If set will be used as the background color
        /// </summary>
        public string HexBackgroundColor;

        /// <summary>
        /// The ItemStack contained within the slot.
        /// </summary>
        public ItemStack Itemstack
        {
            get { return itemstack; }
            set { itemstack = value; }
        }

        /// <summary>
        /// The number of items in the stack.
        /// </summary>
        public int StackSize
        {
            get { return itemstack == null ? 0 : itemstack.StackSize; }
        }

        /// <summary>
        /// Whether or not the stack is empty.
        /// </summary>
        public virtual bool Empty { get { return itemstack == null;  } }

        /// <summary>
        /// The storage type of this slot.
        /// </summary>
        public virtual EnumItemStorageFlags StorageType { get; set; } = EnumItemStorageFlags.General | EnumItemStorageFlags.Agriculture | EnumItemStorageFlags.Alchemy | EnumItemStorageFlags.Jewellery | EnumItemStorageFlags.Metallurgy | EnumItemStorageFlags.Outfit;
        
        /// <summary>
        /// Create a new instance of an item slot
        /// </summary>
        /// <param name="inventory"></param>
        public ItemSlot(InventoryBase inventory)
        {
            this.inventory = inventory;
        }

        /// <summary>
        /// Amount of space left, independent of item MaxStacksize 
        /// </summary>
        public virtual int GetRemainingSlotSpace(ItemStack forItemstack)
        {
            return Math.Max(0, MaxSlotStackSize - StackSize);
        }


        /// <summary>
        /// Whether or not this slot can take the item from the source slot.
        /// </summary>
        /// <param name="sourceSlot"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public virtual bool CanTakeFrom(ItemSlot sourceSlot, EnumMergePriority priority = EnumMergePriority.AutoMerge)
        {
            if (inventory?.PutLocked == true) return false;

            ItemStack sourceStack = sourceSlot.Itemstack;
            if (sourceStack == null) return false;

            bool flagsok = (sourceStack.Collectible.GetStorageFlags(sourceStack) & StorageType) > 0;

            return flagsok &&  (itemstack == null || itemstack.Collectible.GetMergableQuantity(itemstack, sourceStack, priority) > 0) && GetRemainingSlotSpace(sourceStack) > 0;
        }

        /// <summary>
        /// Whether or not this slot can hold the item from the source slot.
        /// </summary>
        /// <param name="sourceSlot"></param>
        /// <returns></returns>
        public virtual bool CanHold(ItemSlot sourceSlot)
        {
            if (inventory?.PutLocked == true) return false;

            return 
                sourceSlot?.Itemstack?.Collectible != null 
                && ((sourceSlot.Itemstack.Collectible.GetStorageFlags(sourceSlot.Itemstack) & StorageType) > 0)
                && inventory.CanContain(this, sourceSlot)
            ;
        }

        /// <summary>
        /// Whether or not this slots item can be retrieved.
        /// </summary>
        /// <returns></returns>
        public virtual bool CanTake()
        {
            if (inventory?.TakeLocked == true) return false;

            return itemstack != null;
        }
        
        /// <summary>
        /// Gets the entire contents of the stack, setting the base stack to null.
        /// </summary>
        /// <returns></returns>
        public virtual ItemStack TakeOutWhole()
        {
            ItemStack stack = itemstack.Clone();
            itemstack.StackSize = 0;
            itemstack = null;
            OnItemSlotModified(stack);

            return stack;
        }

        /// <summary>
        /// Gets some of the contents of the stack.
        /// </summary>
        /// <param name="quantity">The amount to get from the stack.</param>
        /// <returns>The stack with the quantity take out (or as much as was available)</returns>
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

        /// <summary>
        /// Attempts to place item in this slot into the target slot.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="sinkSlot"></param>
        /// <param name="quantity"></param>
        /// <returns>Amount of moved items</returns>
        public virtual int TryPutInto(IWorldAccessor world, ItemSlot sinkSlot, int quantity = 1)
        {
            ItemStackMoveOperation op = new ItemStackMoveOperation(world, EnumMouseButton.Left, 0, EnumMergePriority.AutoMerge, quantity);
            return TryPutInto(sinkSlot, ref op);
        }

        /// <summary>
        /// Returns the quantity of items that were not merged (left over in the source slot)
        /// </summary>
        /// <param name="sinkSlot"></param>
        /// <param name="op"></param>
        /// <returns>Amount of moved items</returns>
        public virtual int TryPutInto(ItemSlot sinkSlot, ref ItemStackMoveOperation op)
        {
            if (!sinkSlot.CanTakeFrom(this) || !CanTake() || itemstack == null)
            {
                return 0;
            }

            if (sinkSlot.inventory?.CanContain(sinkSlot, this) == false) return 0;
            

            // Fill the destination slot with as many items as we can
            if (sinkSlot.Itemstack == null)
            {
                int q = Math.Min(sinkSlot.GetRemainingSlotSpace(itemstack), op.RequestedQuantity);

                if (q > 0)
                {
                    sinkSlot.Itemstack = TakeOut(q);

                    // Has to be above the modified calls because e.g. when moving stuff into the ground slot this will eject the item 
                    // onto the ground and sinkSlot.StackSize is 0 right after
                    op.MovedQuantity = op.MovableQuantity = Math.Min(sinkSlot.StackSize, q);

                    sinkSlot.OnItemSlotModified(sinkSlot.Itemstack);
                    OnItemSlotModified(sinkSlot.Itemstack);
                }

                return op.MovedQuantity;
            }

            ItemStackMergeOperation mergeop = op.ToMergeOperation(sinkSlot, this);
            op = mergeop;
            int origRequestedQuantity = op.RequestedQuantity;
            op.RequestedQuantity = Math.Min(sinkSlot.GetRemainingSlotSpace(itemstack), op.RequestedQuantity);

            sinkSlot.Itemstack.Collectible.TryMergeStacks(mergeop);

            if (mergeop.MovedQuantity > 0)
            {
                sinkSlot.OnItemSlotModified(sinkSlot.Itemstack);
                OnItemSlotModified(sinkSlot.Itemstack);
            }

            op.RequestedQuantity = origRequestedQuantity; //ensures op.NotMovedQuantity will be correct in calling code if used with slots with limited slot maxStackSize, e.g. InventorySmelting with a cooking container has slots with maxStackSize == 6
            return mergeop.MovedQuantity;
        }

        /// <summary>
        /// Attempts to flip the ItemSlots.
        /// </summary>
        /// <param name="itemSlot"></param>
        /// <returns>Whether or no the flip was successful.</returns>
        public virtual bool TryFlipWith(ItemSlot itemSlot)
        {
            if (itemSlot.StackSize > MaxSlotStackSize) return false;

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

        /// <summary>
        /// Forces a flip with the given ItemSlot
        /// </summary>
        /// <param name="withSlot"></param>
        protected virtual void FlipWith(ItemSlot withSlot)
        {
            if (withSlot.StackSize > MaxSlotStackSize)
            {
                if (!Empty) return;
                this.itemstack = withSlot.TakeOut(MaxSlotStackSize);
                return;
            }
            ItemStack temp = withSlot.itemstack;
            withSlot.itemstack = itemstack;
            itemstack = temp;
        }


        /// <summary>
        /// Called when a player has clicked on this slot.  The source slot is the mouse cursor slot.  This handles the logic of either taking, putting or exchanging items.
        /// </summary>
        /// <param name="sourceSlot"></param>
        /// <param name="op"></param>
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

                case EnumMouseButton.Wheel:
                    if (op.WheelDir > 0)
                    {
                        sourceSlot.TryPutInto(this, ref op);
                    }
                    else
                    {
                        TryPutInto(sourceSlot, ref op);
                    }
                    return;
            }
        }

        /// <summary>
        /// Activates the left click functions of the given slot.
        /// </summary>
        /// <param name="sourceSlot"></param>
        /// <param name="op"></param>
        protected virtual void ActivateSlotLeftClick(ItemSlot sourceSlot, ref ItemStackMoveOperation op)
        { 
            // 1. Current slot empty: Take items
            if (Empty) {
                if (!CanHold(sourceSlot)) return;

                int q = Math.Min(sourceSlot.StackSize, MaxSlotStackSize);
                q = Math.Min(q, GetRemainingSlotSpace(sourceSlot.itemstack));

                itemstack = sourceSlot.TakeOut(q);
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
            int maxq = itemstack.Collectible.GetMergableQuantity(itemstack, sourceSlot.itemstack, op.CurrentPriority);
            if (maxq > 0) {
                int origRequestedQuantity = op.RequestedQuantity;
                op.RequestedQuantity = GameMath.Min(maxq, sourceSlot.itemstack.StackSize, GetRemainingSlotSpace(sourceSlot.itemstack));

                ItemStackMergeOperation mergeop = op.ToMergeOperation(this, sourceSlot);
                op = mergeop;

                itemstack.Collectible.TryMergeStacks(mergeop);

                sourceSlot.OnItemSlotModified(itemstack);
                OnItemSlotModified(itemstack);

                op.RequestedQuantity = origRequestedQuantity; //ensures op.NotMovedQuantity will be correct in calling code if used with slots with limited slot maxStackSize, e.g. InventorySmelting with a cooking container has slots with maxStackSize == 6
                return;
            }

            // 4. Both slots not empty and not stackable: Exchange items
            TryFlipWith(sourceSlot);
        }

        /// <summary>
        /// Activates the middle click functions of the given slot.
        /// </summary>
        /// <param name="sinkSlot"></param>
        /// <param name="op"></param>
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


        /// <summary>
        /// Activates the right click functions of the given slot.
        /// </summary>
        /// <param name="sourceSlot"></param>
        /// <param name="op"></param>
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

        /// <summary>
        /// The event fired when the slot is modified.
        /// </summary>
        /// <param name="sinkStack"></param>
        public virtual void OnItemSlotModified(ItemStack sinkStack)
        {
            if (inventory != null)
            {
                inventory.DidModifyItemSlot(this, sinkStack);

                if (itemstack?.Collectible != null)
                {
                    itemstack.Collectible.UpdateAndGetTransitionStates(inventory.Api.World, this);
                }
            }

        }

        /// <summary>
        /// Marks the slot as dirty which  queues it up for saving and resends it to the clients. Does not sync from client to server.
        /// </summary>
        public virtual void MarkDirty()
        {
            if (MarkedDirty != null)
            {
                if (MarkedDirty.Invoke())
                {
                    return;
                }
            }

            if (inventory != null)
            {
                inventory.DidModifyItemSlot(this);

                if (itemstack?.Collectible != null)
                {
                    itemstack.Collectible.UpdateAndGetTransitionStates(inventory.Api.World, this);
                }
            }
        }

        /// <summary>
        /// Gets the name of the itemstack- if it exists.
        /// </summary>
        /// <returns>The name of the itemStack or null.</returns>
        public virtual string GetStackName()
        {
            return itemstack?.GetName();
        }

        /// <summary>
        /// Gets the StackDescription for the item.
        /// </summary>
        /// <param name="world">The world the item resides in.</param>
        /// <param name="extendedDebugInfo">Whether or not we have Extended Debug Info enabled.</param>
        /// <returns></returns>
        public virtual string GetStackDescription(IClientWorldAccessor world, bool extendedDebugInfo)
        {
            return itemstack?.GetDescription(world, this, extendedDebugInfo);
        }


        public override string ToString()
        {
            if (Empty) {
                return base.ToString();
            } else
            {
                return base.ToString() + " (" + itemstack.ToString() + ")";
            }
        }

        public virtual WeightedSlot GetBestSuitedSlot(ItemSlot sourceSlot, ItemStackMoveOperation op = null, List<ItemSlot> skipSlots = null)
        {
            return inventory.GetBestSuitedSlot(sourceSlot, op, skipSlots);
        }

        public virtual void OnBeforeRender(ItemRenderInfo renderInfo)
        {
            // The default is to do nothing, but classes can override this to do something with the renderInfo.Transform            
        }
    }
}
