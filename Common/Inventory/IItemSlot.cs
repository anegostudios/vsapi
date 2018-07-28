namespace Vintagestory.API.Common
{
    public interface IItemSlot
    {
        /// <summary>
        /// The inventory the slot belongs to
        /// </summary>
        IInventory Inventory { get; }

        /// <summary>
        /// The contained itemstack
        /// </summary>
        ItemStack Itemstack { get; set; }

        /// <summary>
        /// Returns that stack size of the itemstack. Returns 0 if the itemstack is null. Might however also return 0 if the itemstack is non-null but stacksize is set to 0.
        /// </summary>
        int StackSize { get; }

        /// <summary>
        /// Short verison of Itemstack == null
        /// </summary>
        bool Empty { get; }

        /// <summary>
        /// Returns what kind of items it can hold. 0 if it can hold any item.
        /// </summary>
        EnumItemStorageFlags StorageType { get; }

        /// <summary>
        /// Returns true if items can be taken from this slot
        /// </summary>
        /// <returns></returns>
        bool CanTake();

        /// <summary>
        /// Returns true if the slot can hold one ore more items from the source slot
        /// </summary>
        /// <returns></returns>
        bool CanTakeFrom(IItemSlot sourceSlot);

        /// <summary>
        /// Splits the itemstack and returns a copy of the new itemstack with given quantity
        /// </summary>
        /// <param name="quantity"></param>
        /// <returns></returns>
        ItemStack TakeOut(int quantity);

        /// <summary>
        /// Tries to fill the destination slot with whatever we have in our slot. Uses standard move operaton
        /// </summary>
        /// <param name="destinationSlot"></param>
        /// <param name="quantity"></param>
        /// <param name="isShiftTransfer"></param>
        /// <returns></returns>
        void TryPutInto(IWorldAccessor world, IItemSlot destinationSlot);


        /// <summary>
        /// Tries to fill the destination slot with whatever we have in our slot
        /// </summary>
        /// <param name="destinationSlot"></param>
        /// <param name="quantity"></param>
        /// <param name="isShiftTransfer"></param>
        /// <returns></returns>
        void TryPutInto(IItemSlot destinationSlot, ref ItemStackMoveOperation op);
        

        /// <summary>
        /// Exchanges the slot contents with the target slot
        /// </summary>
        /// <param name="itemSlot"></param>
        /// <returns></returns>
        bool TryFlipWith(IItemSlot itemSlot);

        /// <summary>
        /// Call when a player has clicked on this slot. The source slot must always be the mouse cursor slot. This handles the logic of either taking, putting, stacking or exchanging items.
        /// </summary>
        /// <param name="sourceSlot"></param>
        /// <param name="button"></param>
        /// <param name="modifiers"></param>
        void ActivateSlot(IItemSlot sourceSlot, ref ItemStackMoveOperation op);


        /// <summary>
        /// Called from another slot when items have been placed into it.
        /// </summary>
        /// <param name="sinkStack">If items were moved, this is the stack they were moved into (may belong to another slot)</param>
        void OnItemSlotModified(IItemStack sinkStack);

        /// <summary>
        /// Marks this slot dirty so that it gets synchronized
        /// </summary>
        void MarkDirty();
    }
}
