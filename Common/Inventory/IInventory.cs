using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vintagestory.API.Common.Entities;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Basic interface representing an item inventory
    /// </summary>
    public interface IInventory
    {
        /// <summary>
        /// Called by item slot, if true, player cannot take items from this chest
        /// </summary>
        bool TakeLocked { get; }

        /// <summary>
        /// Called by item slot, if true, player cannot take items from this chest
        /// </summary>
        bool PutLocked { get; }

        /// <summary>
        /// Milliseconds since server startup when the inventory was last changed (not used currently)
        /// </summary>
        long LastChanged { get; }

        /// <summary>
        /// Returns the slot at given slot number. Returns null for invalid slot number (below 0 or above QuantitySlots), otherwise given slot.
        /// </summary>
        /// <param name="slotId"></param>
        /// <returns></returns>
        ItemSlot GetSlot(int slotId);

        /// <summary>
        /// Returns the amount of existing slots in this inventory
        /// </summary>
        int QuantitySlots { get; }


        string ClassName { get; }
        string InventoryID { get; }


        HashSet<int> DirtySlots { get; }

        /// <summary>
        /// Marks the inventory available for interaction for this player
        /// </summary>
        /// <param name="player"></param>
        object Open(IPlayer player);
        /// <summary>
        /// Removes ability to interact with this inventory for this player
        /// </summary>
        /// <param name="player"></param>
        object Close(IPlayer player);

        /// <summary>
        /// Checks if given player has this inventory currently opened
        /// </summary>
        /// <param name="player"></param>
        bool HasOpened(IPlayer player);


        /// <summary>
        /// Returns the best suited slot to hold the item from the source slot. Attached is also a weight, indicating how well the item is suited for it. If no suitable slot was found, the weight will be 0 and the slot will be null. A higher weight means the slot is better suited to hold the item. This method does not check if the player is actually allowed to access or modify this inventory.
        /// 
        /// Weight will be 1 for a default slot that is empty
        /// Weight will be 2 for a default slot that can take one or more items from the source slot
        /// Weight could be 10 for an empty armor slot and the source slot contains an armor itemtack
        /// </summary>
        /// <param name="sourceSlot"></param>
        /// <param name="skipSlots"></param>
        /// <returns></returns>
        WeightedSlot GetBestSuitedSlot(IPlayer actingPlayer, ItemSlot sourceSlot, List<IItemSlot> skipSlots = null);

        /// <summary>
        /// Will place quantity items from the source slot into the best fitting slot of this inventory. Might fill several of the inventories slots.
        /// </summary>
        /// <param name="sourceSlot"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        //bool TryPutItemStack(IItemSlot sourceSlot, int quantity);


        /// <summary>
        /// When the player clicks on this slot
        /// </summary>
        /// <param name="slotId"></param>
        /// <param name="sourceSlot"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        object ActivateSlot(int slotId, IItemSlot sourceSlot, ref ItemStackMoveOperation op);

        /// <summary>
        /// Attempts to flip the contents of both slots
        /// </summary>
        /// <param name="targetSlotId"></param>
        /// <param name="sourceSlot"></param>
        /// <returns></returns>
        object TryFlipItems(int targetSlotId, IItemSlot sourceSlot);

        /// <summary>
        /// Will return -1 if the slot is not found in this inventory
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        int GetSlotId(IItemSlot slot);

        /// <summary>
        /// Server Side: Will resent the slot contents to the client and mark them dirty there as well
        /// Client Side: Will refresh stack size, model and stuff if this stack is currently being rendered
        /// </summary>
        /// <param name="slotId"></param>
        void MarkSlotDirty(int slotId);
        
        /// <summary>
        /// Event that fires when a slot was modified
        /// </summary>
        event Action<int> SlotModified;

        /// <summary>
        /// Event that fires when NotifySlot was called 
        /// </summary>
        event Action<int> SlotNotified;
    }
}
