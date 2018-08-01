using System.Collections.Generic;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Let's you do various interesting things with the players inventory.
    /// </summary>
    public interface IPlayerInventoryManager
    {
        /// <summary>
        /// If the player currently holds a tool in his hands, this value will be set
        /// </summary>
        EnumTool? ActiveTool { get; }

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
        /// List of inventories currently available to the player (may however not be opened)
        /// </summary>
        Dictionary<string, IInventory> Inventories { get; }

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
        ItemSlot CurrentHoveredSlot { get; }


        /// <summary>
        /// Drops the current contents of the mouse slot onto the ground
        /// </summary>
        /// <param name="dropAll">If false, will only drop 1</param>
        /// <returns></returns>
        bool DropMouseSlotItems(bool dropAll);

        /// <summary>
        /// Produces a visual cue on given slot, if it's currently part of an inventory and visible to the player
        /// If called on server side, the server will send a network packet to notify the client.
        /// </summary>
        /// <param name="slot"></param>
        void NotifySlot(IPlayer player, ItemSlot slot);

        /// <summary>
        /// Returns the full inventory id for given inventory class name, e.g. GlobalConstants.creativeInvClassName
        /// </summary>
        /// <param name="inventoryClassName"></param>
        /// <returns></returns>
        string GetInventoryName(string inventoryClassName);

        /// <summary>
        /// Same as GetInventory() with playeruid appended to the inventoryClassName
        /// </summary>
        /// <param name="inventoryClassName"></param>
        /// <returns></returns>
        IInventory GetOwnInventory(string inventoryClassName);

        /// <summary>
        /// Retrieve a players inventory
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
        /// Returns a slot that would best fit the contents of the source slot. Only tries to place the itemstack into the hotbar.
        /// </summary>
        /// <param name="sourceInventory"></param>
        /// <param name="sourceSlot"></param>
        /// <returns></returns>
        ItemSlot GetBestSuitedHotbarSlot(IPlayer actingPlayer, IInventory sourceInventory, ItemSlot sourceSlot);

        /// <summary>
        /// Returns the hotbar inventory object. Obvious comment is being obvious.
        /// </summary>
        /// <returns></returns>
        IInventory GetHotbarInventory();

        /// <summary>
        /// Returns a slot that would best fit the contents of the source slot. This checks all inventories currently opened by the player.
        /// </summary>
        /// <param name="sourceSlot"></param>
        /// <returns></returns>
        ItemSlot GetBestSuitedSlot(IPlayer actingPlayer, ItemSlot sourceSlot, bool onlyPlayerInventory, List<IItemSlot> skipSlots = null);

        /// <summary>
        /// Tries to move away items from the source slot into any other slot of another inventory
        /// </summary>
        /// <param name="sourceSlot"></param>
        /// <param name="op"></param>
        /// <param name="onlyPlayerInventory">Forces to place the item only into the players personal inventory</param>
        /// <returns>One or more client packets that may be sent to the server for synchronisation</returns>
        object[] TryTransferItemFrom(ItemSlot sourceSlot, ref ItemStackMoveOperation op, bool onlyPlayerInventory, bool slotNotifyEffect = false);


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
        /// <param name="craftingInv"></param>
        void DropAllInventoryItems(IInventory inv);
    }
}
