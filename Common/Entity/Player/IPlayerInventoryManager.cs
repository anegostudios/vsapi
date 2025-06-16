using System;
using System.Collections.Generic;
using System.Text;

#nullable disable

namespace Vintagestory.API.Common
{
    public interface ISkillItemRenderer
    {
        void Render(float dt, float x, float y, float z);
    }

    /// <summary>
    /// Let's you do various interesting things with the players inventory.
    /// </summary>
    public interface IPlayerInventoryManager
    {
        /// <summary>
        /// If the player currently holds a tool in the active hand, this value will be set
        /// </summary>
        EnumTool? ActiveTool { get; }

        /// <summary>
        /// If the player currently holds a tool in the off hand, this value will be set
        /// </summary>
        EnumTool? OffhandTool { get; }

        /// <summary>
        /// The players currently active hot bar slot
        /// </summary>
        int ActiveHotbarSlotNumber { get; set; }

        /// <summary>
        /// Returns the currently selected hotbar slot. Might return null if there is no hotbar!
        /// </summary>
        /// <returns></returns>
        ItemSlot ActiveHotbarSlot { get; }

        /// <summary>
        /// Returns the off hand hotbar slot. Might return null if there is no hotbar!
        /// </summary>
        /// <returns></returns>
        ItemSlot OffhandHotbarSlot { get; }

        /// <summary>
        /// Dictionary of all inventories currently available to the player (some may however not be opened)
        /// <br/>Note: for iterating through these, Inventories.Values will not be ordered.  Instead use InventoriesOrdered if you want consistent ordering on server and client e.g. for shift-click operations
        /// </summary>
        Dictionary<string, IInventory> Inventories { get; }

        /// <summary>
        /// An iterable collection of all inventories currently available to the player, arranged in the same order (by creation order) on both server and client to prevent syncing / ghosting issues
        /// </summary>
        IEnumerable<InventoryBase> InventoriesOrdered { get; }

        /// <summary>
        /// List of inventories currently opened by the player
        /// </summary>
        List<IInventory> OpenedInventories { get; }

        

        /// <summary>
        /// Returns the slot that holds the currently dragged itemstack
        /// </summary>
        /// <returns></returns>
        ItemSlot MouseItemSlot { get; }

        /// <summary>
        /// The slot the player currently hovers over with his mouse cursor
        /// </summary>
        ItemSlot CurrentHoveredSlot { get; set; }


        /// <summary>
        /// Drops the current contents of the mouse slot onto the ground
        /// </summary>
        /// <param name="dropAll">If false, will only drop 1</param>
        /// <returns></returns>
        bool DropMouseSlotItems(bool dropAll);

        /// <summary>
        /// Drops the current contents of given slot onto the ground
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="fullStack"></param>
        /// <returns></returns>
        bool DropItem(ItemSlot slot, bool fullStack);

        /// <summary>
        /// Produces a visual cue on given slot, if it's currently part of an inventory and visible to the player
        /// If called on server side, the server will send a network packet to notify the client.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="slot"></param>
        void NotifySlot(IPlayer player, ItemSlot slot);

        /// <summary>
        /// Returns the full inventory id for given inventory class name, e.g. GlobalConstants.creativeInvClassName
        /// </summary>
        /// <param name="inventoryClassName"></param>
        /// <returns></returns>
        string GetInventoryName(string inventoryClassName);


        /// <summary>
        /// Same as GetInventory() with playeruid appended to the inventoryClassName. Returns null if not found. You can use GlobalConstants.*ClassName to get the vanilla player inventories.
        /// </summary>
        /// <param name="inventoryClassName"></param>
        /// <returns></returns>
        IInventory GetOwnInventory(string inventoryClassName);

        /// <summary>
        /// Retrieve a players inventory. Returns null if not found.
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <returns></returns>
        IInventory GetInventory(string inventoryId);
        

        /// <summary>
        /// Gets the itemstack that in the given slot number of the players hotbar
        /// </summary>
        /// <param name="slotId"></param>
        /// <returns></returns>
        ItemStack GetHotbarItemstack(int slotId);
        
        /// <summary>
        /// Returns the hotbar inventory object. Obvious comment is being obvious.
        /// </summary>
        /// <returns></returns>
        IInventory GetHotbarInventory();

        /// <summary>
        /// Returns true if the invID is found, and the found IInventory value is in invFound; similar to Dictionary.TryGetValue, invFound is undefined if the result is false
        /// </summary>
        /// <param name="invID"></param>
        /// <param name="invFound"></param>
        /// <returns></returns>
        bool GetInventory(string invID, out InventoryBase invFound);


        /// <summary>
        /// Returns a slot that would best fit the contents of the source slot. This checks all inventories currently opened by the player.
        /// </summary>
        /// <param name="sourceSlot"></param>
        /// <param name="onlyPlayerInventory"></param>
        /// <param name="op"></param>
        /// <param name="skipSlots"></param>
        /// <returns></returns>
        ItemSlot GetBestSuitedSlot(ItemSlot sourceSlot, bool onlyPlayerInventory, ItemStackMoveOperation op = null, List<ItemSlot> skipSlots = null);
        [Obsolete("Use GetBestSuitedSlot(ItemSlot sourceSlot, bool onlyPlayerInventory, ItemStackMoveOperation op, List<ItemSlot> skipSlots = null) instead")]
        ItemSlot GetBestSuitedSlot(ItemSlot sourceSlot, ItemStackMoveOperation op, List<ItemSlot> skipSlots);

        /// <summary>
        /// Tries to move away items from the source slot into any other slot of another inventory
        /// </summary>
        /// <param name="sourceSlot"></param>
        /// <param name="op"></param>
        /// <param name="onlyPlayerInventory">Forces to place the item only into the players personal inventory</param>
        /// <param name="slotNotifyEffect"></param>
        /// <returns>One or more client packets that may be sent to the server for synchronisation</returns>
        object[] TryTransferAway(ItemSlot sourceSlot, ref ItemStackMoveOperation op, bool onlyPlayerInventory, bool slotNotifyEffect = false);

        object[] TryTransferAway(ItemSlot sourceSlot, ref ItemStackMoveOperation op, bool onlyPlayerInventory, StringBuilder shiftClickDebugText, bool slotNotifyEffect = false);

        /// <summary>
        /// Tries to move items from source slot to target slot (useful for client side inventory utilities)
        /// </summary>
        /// <param name="sourceSlot"></param>
        /// <param name="targetSlot"></param>
        /// <param name="op">If successfull, a client packet that may be sent to the server for synchronisation</param>
        /// <returns></returns>
        object TryTransferTo(ItemSlot sourceSlot, ItemSlot targetSlot, ref ItemStackMoveOperation op);

        /// <summary>
        /// Tries to add given itemstack to the players inventory
        /// </summary>
        /// <param name="itemstack"></param>
        /// <param name="slotNotifyEffect"></param>
        /// <returns></returns>
        bool TryGiveItemstack(ItemStack itemstack, bool slotNotifyEffect = false);


        /// <summary>
        /// Notifies the inventory manager that the player has opened an inventory. Should always be called on both sides (client and server).
        /// Only then interaction with other inventories becomes possible
        /// </summary>
        /// <param name="inventory"></param>
        object OpenInventory(IInventory inventory);

        /// <summary>
        /// Notifies the inventory manager that the player has closed an inventory. Should always be called on both sides (client and server). After closing interaction with given inventory becomes no longer possible until reopened
        /// </summary>
        /// <param name="inventory"></param>
        object CloseInventory(IInventory inventory);

        /// <summary>
        /// Like CloseInventory() but also sends the necessary packet to the server, if called from the client side: exactly like CloseInventory() if called from the server side
        /// </summary>
        /// <param name="inventory"></param>
        void CloseInventoryAndSync(IInventory inventory);

        /// <summary>
        /// Iterates over all inventory slots, returns true if your matcher returns true
        /// </summary>
        /// <param name="matcher"></param>
        /// <returns></returns>
        bool Find(System.Func<ItemSlot, bool> matcher);

        /// <summary>
        /// Shorthand for Inventories.ContainsValue(inventory)
        /// </summary>
        /// <param name="inventory"></param>
        /// <returns></returns>
        bool HasInventory(IInventory inventory);

        /// <summary>
        /// Will discard all of the players inventory contents
        /// </summary>
        void DiscardAll();

        /// <summary>
        /// Will drop all of the players inventory contents
        /// </summary>
        void OnDeath();

        /// <summary>
        /// Drops the contents of given inventory
        /// </summary>
        /// <param name="inv"></param>
        void DropAllInventoryItems(IInventory inv);

        /// <summary>
        /// Server Side: Resends the hotbar slot contents to all other clients to make sure they render the correct held item
        /// Client side: No effect
        /// </summary>
        void BroadcastHotbarSlot();
    }
}
