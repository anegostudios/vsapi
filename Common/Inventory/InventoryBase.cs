using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace Vintagestory.API.Common
{
    public delegate void OnInventoryOpened(IPlayer player);
    public delegate void OnInventoryClosed(IPlayer player);

    /// <summary>
    /// Basic class representing an item inventory
    /// </summary>
    public abstract class InventoryBase : IInventory
    {
        /// <summary>
        /// The world in which the inventory is operating in. Gives inventories access to block types, item types and the ability to drop items on the ground.
        /// </summary>
        public ICoreAPI Api;

        /// <summary>
        /// Is this inventory generally better suited to hold items? (e.g. set to 3 for armor in armor inventory, 2 for any item in hotbar inventory, 1 for any item in normal inventory)
        /// </summary>
        protected float baseWeight = 0;

        protected string className;
        protected string instanceID;

        public long lastChangedSinceServerStart;
        public HashSet<string> openedByPlayerGUIds;

        public IInventoryNetworkUtil InvNetworkUtil;

        /// <summary>
        /// Slots that have been recently modified. This list is used on the server to update the clients (then cleared) and on the client to redraw itemstacks in guis (then cleared)
        /// </summary>
        public HashSet<int> dirtySlots = new HashSet<int>();

        public string InventoryID { get { return className + "-" + instanceID; } }
        public string ClassName { get { return className; } }

        /// <summary>
        /// Milliseconds since server startup when the inventory was last changed (not used currently)
        /// </summary>
        public long LastChanged { get { return lastChangedSinceServerStart; } }


        /// <summary>
        /// Returns the number of slots in this inventory.
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// Gets or sets the slot at the given slot number.
        /// Returns null for invalid slot number (below 0 or above Count-1).
        /// The setter allows for replacing slots with custom ones, though caution is advised.
        /// </summary>
        public abstract ItemSlot this[int slotId] { get; set; }

        [Obsolete("Use Count instead.")]
        public int QuantitySlots => Count;

        [Obsolete("Use indexer instead.")]
        public ItemSlot GetSlot(int slotId) => this[slotId];

        /// <summary>
        /// True if this inventory has to be resent to the client or when the client has to redraw them
        /// </summary>
        public virtual bool IsDirty { get { return dirtySlots.Count > 0; } }

        /// <summary>
        /// The slots that have been modified server side and need to be resent to the client or need to be redrawn on the client
        /// </summary>
        public HashSet<int> DirtySlots { get { return dirtySlots; } }

        /// <summary>
        /// Called by item slot, if true, player cannot take items from this inventory
        /// </summary>
        public virtual bool TakeLocked { get; set; }

        /// <summary>
        /// Called by item slot, if true, player cannot put items into this inventory
        /// </summary>
        public virtual bool PutLocked { get; set; }

        /// <summary>
        /// If true, the inventory will be removed from the list of available inventories once closed (i.e. is not a personal inventory that the player carries with him)
        /// </summary>
        public virtual bool RemoveOnClose => true;

        /// <summary>
        /// Called whenever a slot has been modified
        /// </summary>
        public event API.Common.Action<int> SlotModified;

        /// <summary>
        /// Called whenever a slot notification event has been fired. Is used by the slot grid gui element to visually wiggle the slot contents
        /// </summary>
        public event API.Common.Action<int> SlotNotified;

        /// <summary>
        /// Called whenever this inventory was opened
        /// </summary>
        public event OnInventoryOpened OnInventoryOpened;

        /// <summary>
        /// Called whenever this inventory was closed
        /// </summary>
        public event OnInventoryClosed OnInventoryClosed;

        /// <summary>
        /// Create a new instance of an inventory
        /// </summary>
        /// <param name="className"></param>
        /// <param name="instanceID"></param>
        /// <param name="api"></param>
        public InventoryBase(string className, string instanceID, ICoreAPI api)
        {
            openedByPlayerGUIds = new HashSet<string>();

            this.instanceID = instanceID;
            this.className = className;
            this.Api = api;
            if (api != null)
            {
                InvNetworkUtil = api.ClassRegistry.CreateInvNetworkUtil(this, api);
            }
        }

        /// <summary>
        /// Create a new instance of an inventory. InvetoryID must have the format [className]-[instanceId]  (you may choose any value for those)
        /// </summary>
        /// <param name="inventoryID"></param>
        /// <param name="api"></param>
        public InventoryBase(string inventoryID, ICoreAPI api)
        {
            openedByPlayerGUIds = new HashSet<string>();

            if (inventoryID != null)
            {
                string[] elems = inventoryID.Split(new char[] { '-' }, 2);
                className = elems[0];
                instanceID = elems[1];
            }

            this.Api = api;
            if (api != null)
            {
                InvNetworkUtil = api.ClassRegistry.CreateInvNetworkUtil(this, api);
            }
        }
        
        /// <summary>
        /// You can initialize an InventoryBase with null as parameters and use LateInitialize to set these values later. This is sometimes required during chunk loading.
        /// </summary>
        /// <param name="inventoryID"></param>
        /// <param name="api"></param>
        public virtual void LateInitialize(string inventoryID, ICoreAPI api)
        {
            this.Api = api;
            string[] elems = inventoryID.Split(new char[] { '-' }, 2);
            className = elems[0];
            instanceID = elems[1];

            if (InvNetworkUtil == null)
            {
                InvNetworkUtil = api.ClassRegistry.CreateInvNetworkUtil(this, api);
            } else
            {
                InvNetworkUtil.Api = api;
            }

            AfterBlocksLoaded(api.World);
        }


        public virtual void AfterBlocksLoaded(IWorldAccessor world)
        {
            ResolveBlocksOrItems();
        }


        public virtual void ResolveBlocksOrItems()
        {
            foreach (var slot in this)
            {
                if (slot.Itemstack != null)
                {
                    if (!slot.Itemstack.ResolveBlockOrItem(Api.World))
                    {
                        slot.Itemstack = null;
                    }
                }
            }
        }

        /// <summary>
        /// Will return -1 if the slot is not found in this inventory
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public virtual int GetSlotId(IItemSlot slot)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i] == slot)
                {
                    return i;
                }
            }

            return -1;
        }


        public virtual WeightedSlot GetBestSuitedSlot(ItemSlot sourceSlot, List<IItemSlot> skipSlots = null)
        {
            WeightedSlot bestWSlot = new WeightedSlot();

            // Useless to put the item into the same inventory
            if (sourceSlot.Inventory == this) return bestWSlot;

            // 1. Prefer already filled slots
            foreach (var slot in this)
            {
                if (skipSlots.Contains(slot)) continue;

                if (slot.Itemstack != null && slot.CanTakeFrom(sourceSlot))
                {
                    float curWeight = GetSuitability(sourceSlot, slot, true);

                    if (bestWSlot.slot == null || bestWSlot.weight < curWeight)
                    {
                        bestWSlot.slot = slot;
                        bestWSlot.weight = curWeight;
                    }
                }
            }

            // 2. Otherwise use empty slots
            foreach (var slot in this)
            {
                if (skipSlots.Contains(slot)) continue;

                if (slot.Itemstack == null && slot.CanTakeFrom(sourceSlot))
                {
                    float curWeight = GetSuitability(sourceSlot, slot, false);

                    if (bestWSlot.slot == null || bestWSlot.weight < curWeight)
                    {
                        bestWSlot.slot = slot;
                        bestWSlot.weight = curWeight;
                    }
                }
            }

            return bestWSlot;
        }

        /// <summary>
        /// How well a stack fits into this inventory. 
        /// </summary>
        /// <param name="sourceSlot"></param>
        /// <param name="targetSlot"></param>
        /// <param name="isMerge"></param>
        /// <returns></returns>
        public virtual float GetSuitability(ItemSlot sourceSlot, ItemSlot targetSlot, bool isMerge)
        {
            return isMerge ? (baseWeight + 3) : (baseWeight + 1);
        }


        /// <summary>
        /// Attempts to flip the contents of both slots
        /// </summary>
        /// <param name="targetSlotId"></param>
        /// <param name="itemSlot"></param>
        /// <returns></returns>
        public object TryFlipItems(int targetSlotId, IItemSlot itemSlot)
        {
            IItemSlot targetSlot = this[targetSlotId];
            if (targetSlot != null && targetSlot.TryFlipWith(itemSlot))
            {
                return InvNetworkUtil.GetFlipSlotsPacket(itemSlot.Inventory, itemSlot.Inventory.GetSlotId(itemSlot), targetSlotId);
            }
            return null;
        }


        public virtual bool CanPlayerAccess(IPlayer player, EntityPos position)
        {
            return true;
        }

        public virtual bool CanPlayerModify(IPlayer player, EntityPos position)
        {
            return CanPlayerAccess(player, position) && HasOpened(player);
        }


        public virtual void OnSearchTerm(string text) { }

        /// <summary>
        /// Call when a player has clicked on this slot. The source slot is the mouse cursor slot. This handles the logic of either taking, putting or exchanging items.
        /// </summary>
        /// <param name="slotId"></param>
        /// <param name="sourceSlot"></param>
        /// <param name="op"></param>
        /// <returns>The appropriate packet needed to reflect the changes on the opposing side</returns>
        public virtual object ActivateSlot(int slotId, IItemSlot sourceSlot, ref ItemStackMoveOperation op)
        {
            object packet = InvNetworkUtil.GetActivateSlotPacket(slotId, op);

            this[slotId].ActivateSlot((ItemSlot)sourceSlot, ref op);

            return packet;
        }


        /// <summary>
        /// Called when one of the containing slots has been modified
        /// </summary>
        /// <param name="slot"></param>
        public virtual void OnItemSlotModified(IItemSlot slot)
        {

        }


        /// <summary>
        /// Called when one of the containing slots has been modified
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="extractedStack">If non null the itemstack that was taken out</param>
        public virtual void DidModifyItemSlot(IItemSlot slot, IItemStack extractedStack = null)
        {
            int slotId = GetSlotId(slot);
            MarkSlotDirty(slotId);
            OnItemSlotModified(slot);
            SlotModified?.Invoke(slotId);
        }

        /// <summary>
        /// Called when one of the containing slot was notified via NotifySlot
        /// </summary>
        /// <param name="slotId"></param>
        public virtual void PerformNotifySlot(int slotId)
        {
            ItemSlot slot = this[slotId];
            if (slot == null || slot.Inventory != this) return;

            SlotNotified?.Invoke(slotId);
        }



        /// <summary>
        /// Called when the game is loaded or loaded from server
        /// </summary>
        /// <param name="tree"></param>
        public abstract void FromTreeAttributes(ITreeAttribute tree);

        /// <summary>
        /// Called when the game is saved or sent to client
        /// </summary>
        /// <returns></returns>
        public abstract void ToTreeAttributes(ITreeAttribute tree);


        public virtual bool TryFlipItemStack(IPlayer owningPlayer, string[] invIds, int[] slotIds, long[] lastChanged)
        {
            // 0 = source slot
            // 1 = target slot
            IItemSlot[] slots = getSlotsIfExists(owningPlayer, invIds, slotIds);

            if (slots[0] == null || slots[1] == null) return false;

            InventoryBase targetInv = (InventoryBase)owningPlayer.InventoryManager.GetInventory(invIds[1]);

            // 4. Try to move the item stack
            return targetInv.TryFlipItems(slotIds[1], slots[0]) != null;
        }


        public virtual bool TryMoveItemStack(IPlayer player, string[] invIds, int[] slotIds, ref ItemStackMoveOperation op)
        {
            // 0 = source slot
            // 1 = target slot
            IItemSlot[] slots = getSlotsIfExists(player, invIds, slotIds);

            if (slots[0] == null || slots[1] == null) return false;

            InventoryBase targetInv = (InventoryBase)player.InventoryManager.GetInventory(invIds[1]);

            // 4. Try to move the item stack
            slots[0].TryPutInto(slots[1], ref op);

            return op.MovedQuantity == op.RequestedQuantity;
        }

        public virtual IItemSlot[] getSlotsIfExists(IPlayer player, string[] invIds, int[] slotIds)
        {
            IItemSlot[] slots = new IItemSlot[2];
           
            // 1. Both inventories must exist and be modifiable
            InventoryBase sourceInv = (InventoryBase)player.InventoryManager.GetInventory(invIds[0]);
            InventoryBase targetInv = (InventoryBase)player.InventoryManager.GetInventory(invIds[1]);

            if (sourceInv == null || targetInv == null) return slots;

            if (!sourceInv.CanPlayerModify(player, player.Entity.Pos) || !targetInv.CanPlayerModify(player, player.Entity.Pos))
            {
                return slots;
            }

            // 3. Source and Dest slot must exist
            slots[0] = sourceInv[slotIds[0]];
            slots[1] = targetInv[slotIds[1]];

            return slots;
        }


        public ItemSlot[] SlotsFromTreeAttributes(ITreeAttribute tree, ItemSlot[] slots = null, List<ItemSlot> modifiedSlots = null)
        {
            if (slots == null)
            {
                slots = new ItemSlot[tree.GetInt("qslots")];
                for (int i = 0; i < slots.Length; i++)
                {
                    slots[i] = NewSlot(i);
                }
            }

            for (int slotId = 0; slotId < slots.Length; slotId++)
            {
                ItemStack newstack = tree.GetTreeAttribute("slots")?.GetItemstack("" + slotId);
                ItemStack oldstack = slots[slotId].Itemstack;

                if (Api?.World == null)
                {
                    slots[slotId].Itemstack = newstack;
                    continue;
                }

                newstack?.ResolveBlockOrItem(Api.World);

                bool didModify =
                    (newstack != null && !newstack.Equals(oldstack)) ||
                    (oldstack != null && !oldstack.Equals(newstack))
                ;

                slots[slotId].Itemstack = newstack;

                if (didModify && modifiedSlots != null)
                {
                    modifiedSlots.Add(slots[slotId]);
                }
            }

            

            return slots;
        }


        public void SlotsToTreeAttributes(ItemSlot[] slots, ITreeAttribute tree)
        {
            tree.SetInt("qslots", slots.Length);

            TreeAttribute slotsTree = new TreeAttribute();

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].Itemstack == null) continue;
                slotsTree.SetItemstack(i + "", slots[i].Itemstack.Clone());
            }

            tree["slots"] = slotsTree;
        }
        


        public ItemSlot[] GenEmptySlots(int quantity)
        {
            ItemSlot[] slots = new ItemSlot[quantity];
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = NewSlot(i);
            }
            return slots;
        }

        protected virtual ItemSlot NewSlot(int i)
        {
            return new ItemSlot(this);
        }

        /// <summary>
        /// Server Side: Will resent the slot contents to the client and mark them dirty there as well
        /// Client Side: Will refresh stack size, model and stuff if this stack is currently being rendered
        /// </summary>
        /// <param name="slotId"></param>
        public virtual void MarkSlotDirty(int slotId)
        {
            if (slotId < 0) throw new Exception("Negative slotid?!");
            dirtySlots.Add(slotId);           
        }


        public virtual void DiscardAll()
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Itemstack != null)
                {
                    dirtySlots.Add(i);
                }

                this[i].Itemstack = null;
            }
        }

        public virtual void DropSlots(Vec3d pos, params int[] slotsIds)
        {
            foreach (int slotId in slotsIds)
            {
                if (slotId < 0) throw new Exception("Negative slotid?!");
                var slot = this[slotId];

                if (slot.Itemstack == null) continue;
                Api.World.SpawnItemEntity(slot.Itemstack, pos);
                slot.Itemstack = null;
                slot.MarkDirty();
            }
        }

        public virtual void DropAll(Vec3d pos)
        {
            foreach (var slot in this)
            {
                if (slot.Itemstack == null) continue;
                Api.World.SpawnItemEntity(slot.Itemstack, pos);
                slot.Itemstack = null;
                slot.MarkDirty();
            }
        }
    
        public virtual void OnOwningEntityDeath(Vec3d pos)
        {
            DropAll(pos);
        }


        /// <summary>
        /// Marks the inventory available for interaction for this player. Returns a open inventory packet that can be sent to the server for synchronization.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public virtual object Open(IPlayer player)
        {
            object packet = InvNetworkUtil.DidOpen(player);
            openedByPlayerGUIds.Add(player.PlayerUID);

            OnInventoryOpened?.Invoke(player);
            return packet;
        }

        /// <summary>
        /// Removes ability to interact with this inventory for this player. Returns a close inventory packet that can be sent to the server for synchronization.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public virtual object Close(IPlayer player)
        {
            object packet = InvNetworkUtil.DidClose(player);
            openedByPlayerGUIds.Remove(player.PlayerUID);

            OnInventoryClosed?.Invoke(player);

            return packet;
        }

        /// <summary>
        /// Checks if given player has this inventory currently opened
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public virtual bool HasOpened(IPlayer player)
        {
            return openedByPlayerGUIds.Contains(player.PlayerUID);
        }


        public IEnumerator<ItemSlot> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
