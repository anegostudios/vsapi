using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

#nullable disable

namespace Vintagestory.API.Common
{
    public delegate void OnInventoryOpenedDelegate(IPlayer player);
    public delegate void OnInventoryClosedDelegate(IPlayer player);

    /// <summary>
    /// Custom transition speed handler
    /// </summary>
    /// <param name="transType"></param>
    /// <param name="stack"></param>
    /// <param name="mulByConfig">Multiplier set by other configuration, if any, otherwise 1</param>
    /// <returns></returns>
    public delegate float CustomGetTransitionSpeedMulDelegate(EnumTransitionType transType, ItemStack stack, float mulByConfig);



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
        /// Optional field that can be used to define in-world position of the inventory. Is set by most container block entities. Might be null!
        /// </summary>
        public BlockPos Pos;

        /// <summary>
        /// Optional field, if set, will check against the collectible dimensions and deny placecment if too large
        /// </summary>
        public virtual Size3f MaxContentDimensions { get; set; } = null;

        /// <summary>
        /// Is this inventory generally better suited to hold items? (e.g. set to 3 for armor in armor inventory, 2 for any item in hotbar inventory, 1 for any item in normal inventory)
        /// </summary>
        protected float baseWeight = 0;

        /// <summary>
        /// The name of the class for the invnentory.
        /// </summary>
        protected string className;

        /// <summary>
        /// the ID of the instance for the inventory.
        /// </summary>
        protected string instanceID;

        /// <summary>
        /// (Not implemented!) The time it was last changed since the server was started.
        /// </summary>
        public long lastChangedSinceServerStart;

        /// <summary>
        /// The players that had opened the inventory.
        /// </summary>
        public HashSet<string> openedByPlayerGUIds;

        /// <summary>
        /// The network utility for the inventory
        /// </summary>
        public IInventoryNetworkUtil InvNetworkUtil;

        /// <summary>
        /// Slots that have been recently modified. This list is used on the server to update the clients (then cleared) and on the client to redraw itemstacks in guis (then cleared)
        /// </summary>
        public HashSet<int> dirtySlots = new HashSet<int>();

        /// <summary>
        /// The internal name of the inventory instance.
        /// </summary>
        public string InventoryID { get { return className + "-" + instanceID; } }

        /// <summary>
        /// The class name of the inventory.
        /// </summary>
        public string ClassName { get { return className; } }

        /// <summary>
        /// Milliseconds since server startup when the inventory was last changed
        /// </summary>
        public long LastChanged { get { return lastChangedSinceServerStart; } }


        /// <summary>
        /// Returns the number of slots in this inventory.
        /// </summary>
        public abstract int Count { get; }

        public virtual int CountForNetworkPacket => Count;



        /// <summary>
        /// Gets or sets the slot at the given slot number.
        /// Returns null for invalid slot number (below 0 or above Count-1).
        /// The setter allows for replacing slots with custom ones, though caution is advised.
        /// </summary>
        public abstract ItemSlot this[int slotId] { get; set; }



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
        public event Action<int> SlotModified;

        /// <summary>
        /// Called whenever a slot notification event has been fired. Is used by the slot grid gui element to visually wiggle the slot contents
        /// </summary>
        public event Action<int> SlotNotified;

        /// <summary>
        /// Called whenever this inventory was opened
        /// </summary>
        public event OnInventoryOpenedDelegate OnInventoryOpened;

        /// <summary>
        /// Called whenever this inventory was closed
        /// </summary>
        public event OnInventoryClosedDelegate OnInventoryClosed;

        /// <summary>
        /// If set, the value is returned when GetTransitionSpeedMul() is called instead of the default value.
        /// </summary>
        public event CustomGetTransitionSpeedMulDelegate OnAcquireTransitionSpeed;


        /// <summary>
        /// Convenience method to check if this inventory contains anything
        /// </summary>
        public virtual bool Empty
        {
            get
            {
                foreach (ItemSlot slot in this)
                {
                    if (!slot.Empty) return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Returns the first slot that is not empty or null
        /// </summary>
        public ItemSlot FirstNonEmptySlot
        {
            get
            {
                foreach (ItemSlot slot in this)
                {
                    if (!slot.Empty) return slot;
                }

                return null;
            }
        }

        /// <summary>
        /// If opening or closing should produce a log line in the audit log.
        /// Since when items are moved the source and destination is logged already
        /// </summary>
        public virtual bool AuditLogAccess { get; set; } = false;


        /// <summary>
        /// Create a new instance of an inventory. You may choose any value for className and instanceID, but if more than one of these inventories can be opened at the same time, make sure for both of them to have a different id
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
        /// Create a new instance of an inventory. InvetoryID must have the format [className]-[instanceId]. You may choose any value for className and instanceID, but if more than one of these inventories can be opened at the same time, make sure for both of them to have a different id
        /// </summary>
        /// <param name="inventoryID"></param>
        /// <param name="api"></param>
        public InventoryBase(string inventoryID, ICoreAPI api)
        {
            openedByPlayerGUIds = new HashSet<string>();

            if (inventoryID != null)
            {
                string[] elems = inventoryID.Split('-', 2);
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
            string[] elems = inventoryID.Split('-', 2);
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

        /// <summary>
        /// The event fired after all the blocks have loaded.
        /// </summary>
        /// <param name="world"></param>
        public virtual void AfterBlocksLoaded(IWorldAccessor world)
        {
            ResolveBlocksOrItems();
        }

        /// <summary>
        /// Tells the invnetory to update blocks and items within the invnetory.
        /// </summary>
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
        public virtual int GetSlotId(ItemSlot slot)
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

        [Obsolete("Use GetBestSuitedSlot(ItemSlot sourceSlot, ItemStackMoveOperation op, List<ItemSlot> skipSlots = null) instead")]
        public WeightedSlot GetBestSuitedSlot(ItemSlot sourceSlot, List<ItemSlot> skipSlots)
        {
            return GetBestSuitedSlot(sourceSlot, null, skipSlots);
        }

        /// <summary>
        /// Gets the best sorted slot for the given item.
        /// </summary>
        /// <param name="sourceSlot">The source item slot.</param>
        /// <param name="op">Can be null. If provided allows the inventory to make a better guess at suitability</param>
        /// <param name="skipSlots">The slots to skip.</param>
        /// <returns>A weighted slot set.</returns>
        public virtual WeightedSlot GetBestSuitedSlot(ItemSlot sourceSlot, ItemStackMoveOperation op = null, List<ItemSlot> skipSlots = null)
        {
            WeightedSlot bestWSlot = new WeightedSlot();

            // Useless to put the item into the same inventory
            if (PutLocked || sourceSlot.Inventory == this) return bestWSlot;

            // 1. Prefer already filled slots
            foreach (var slot in this)
            {
                if (skipSlots != null && skipSlots.Contains(slot)) continue;

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
                if (skipSlots != null && skipSlots.Contains(slot)) continue;

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
        /// How well a stack fits into this inventory. By default 1 for new itemstacks and 3 for an itemstack merge. Chests and other stationary container also add a +1 to the suitability if the source slot is from the players inventory.
        /// </summary>
        /// <param name="sourceSlot"></param>
        /// <param name="targetSlot"></param>
        /// <param name="isMerge"></param>
        /// <returns></returns>
        public virtual float GetSuitability(ItemSlot sourceSlot, ItemSlot targetSlot, bool isMerge)
        {
            float extraWeight = targetSlot is ItemSlotBackpack && (sourceSlot.Itemstack.Collectible.GetStorageFlags(sourceSlot.Itemstack) & EnumItemStorageFlags.Backpack) > 0 ? 2 : 0;

            return baseWeight + extraWeight + (isMerge ? 3 : 1);
        }

        public virtual bool CanContain(ItemSlot sinkSlot, ItemSlot sourceSlot)
        {
            return MaxContentDimensions?.CanContain(sourceSlot.Itemstack.Collectible.Dimensions) ?? true;
        }

        /// <summary>
        /// Attempts to flip the contents of both slots
        /// </summary>
        /// <param name="targetSlotId"></param>
        /// <param name="itemSlot"></param>
        /// <returns></returns>
        public object TryFlipItems(int targetSlotId, ItemSlot itemSlot)
        {
            ItemSlot targetSlot = this[targetSlotId];
            if (targetSlot != null && targetSlot.TryFlipWith(itemSlot))
            {
                return InvNetworkUtil.GetFlipSlotsPacket(itemSlot.Inventory, itemSlot.Inventory.GetSlotId(itemSlot), targetSlotId);
            }

            return null;
        }

        /// <summary>
        /// Determines whether or not the player can access the invnetory.
        /// </summary>
        /// <param name="player">The player attempting access.</param>
        /// <param name="position">The postion of the entity.</param>
        public virtual bool CanPlayerAccess(IPlayer player, EntityPos position)
        {
            return true;
        }

        /// <summary>
        /// Determines whether or not the player can modify the invnetory.
        /// </summary>
        /// <param name="player">The player attempting access.</param>
        /// <param name="position">The postion of the entity.</param>
        public virtual bool CanPlayerModify(IPlayer player, EntityPos position)
        {
            return CanPlayerAccess(player, position) && HasOpened(player);
        }

        /// <summary>
        /// The event fired when the search is applied to the item.
        /// </summary>
        /// <param name="text"></param>
        public virtual void OnSearchTerm(string text) { }

        /// <summary>
        /// Call when a player has clicked on this slot. The source slot is the mouse cursor slot. This handles the logic of either taking, putting or exchanging items.
        /// </summary>
        /// <param name="slotId"></param>
        /// <param name="sourceSlot"></param>
        /// <param name="op"></param>
        /// <returns>The appropriate packet needed to reflect the changes on the opposing side</returns>
        public virtual object ActivateSlot(int slotId, ItemSlot sourceSlot, ref ItemStackMoveOperation op)
        {
            object packet = InvNetworkUtil.GetActivateSlotPacket(slotId, op);

            if (op.ShiftDown)
            {
                sourceSlot = this[slotId];
                string stackName = sourceSlot.Itemstack?.GetName();
                string sourceInv = sourceSlot.Inventory?.InventoryID;

                StringBuilder shiftClickDebugText = new StringBuilder();

                op.RequestedQuantity = sourceSlot.StackSize;
                op.ActingPlayer.InventoryManager.TryTransferAway(sourceSlot, ref op, false, shiftClickDebugText);

                Api.World.Logger.Audit("{0} shift clicked slot {1} in {2}. Moved {3}x{4} to ({5})", op.ActingPlayer?.PlayerName, slotId, sourceInv, op.MovedQuantity, stackName, shiftClickDebugText.ToString());
            }
            else
            {
                this[slotId].ActivateSlot(sourceSlot, ref op);
            }

            return packet;
        }

        /// <summary>
        /// Called when one of the containing slots has been modified
        /// </summary>
        /// <param name="slot"></param>
        public virtual void OnItemSlotModified(ItemSlot slot)
        {

        }


        /// <summary>
        /// Called when one of the containing slots has been modified
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="extractedStack">If non null the itemstack that was taken out</param>
        public virtual void DidModifyItemSlot(ItemSlot slot, ItemStack extractedStack = null)
        {
            int slotId = GetSlotId(slot);

            if (slotId < 0)
            {
                throw new ArgumentException(string.Format("Supplied slot is not part of this inventory ({0})!", InventoryID));
            }

            MarkSlotDirty(slotId);
            OnItemSlotModified(slot);
            SlotModified?.Invoke(slotId);

            slot.Itemstack?.Collectible?.OnModifiedInInventorySlot(Api.World, slot, extractedStack);
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

        /// <summary>
        /// Attempts to flip the inventory slots.
        /// </summary>
        /// <param name="owningPlayer">The player owner of the invnetory slots.</param>
        /// <param name="invIds">The IDs of the player inventory.</param>
        /// <param name="slotIds">The IDs of the target inventory.</param>
        /// <param name="lastChanged">The times these ids were last changed.</param>
        public virtual bool TryFlipItemStack(IPlayer owningPlayer, string[] invIds, int[] slotIds, long[] lastChanged)
        {
            // 0 = source slot
            // 1 = target slot
            ItemSlot[] slots = GetSlotsIfExists(owningPlayer, invIds, slotIds);

            if (slots[0] == null || slots[1] == null) return false;

            InventoryBase targetInv = (InventoryBase)owningPlayer.InventoryManager.GetInventory(invIds[1]);

            // 4. Try to move the item stack
            return targetInv.TryFlipItems(slotIds[1], slots[0]) != null;
        }

        /// <summary>
        /// Attempts to move the item stack from the inventory to another slot.
        /// </summary>
        /// <param name="player">The player moving the items</param>
        /// <param name="invIds">The player inventory IDs</param>
        /// <param name="slotIds">The target Ids</param>
        /// <param name="op">The operation type.</param>
        public virtual bool TryMoveItemStack(IPlayer player, string[] invIds, int[] slotIds, ref ItemStackMoveOperation op)
        {
            // 0 = source slot
            // 1 = target slot
            ItemSlot[] slots = GetSlotsIfExists(player, invIds, slotIds);

            if (slots[0] == null || slots[1] == null) return false;

            // 4. Try to move the item stack
            slots[0].TryPutInto(slots[1], ref op);

            return op.MovedQuantity == op.RequestedQuantity;
        }

        /// <summary>
        /// Attempts to get specified slots if the slots exists.
        /// </summary>
        /// <param name="player">The player owning the slots</param>
        /// <param name="invIds">The inventory IDs</param>
        /// <param name="slotIds">The slot ids</param>
        /// <returns>The slots obtained.</returns>
        public virtual ItemSlot[] GetSlotsIfExists(IPlayer player, string[] invIds, int[] slotIds)
        {
            ItemSlot[] slots = new ItemSlot[2];

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

        /// <summary>
        /// Creates a collection of slots from a tree.
        /// </summary>
        /// <param name="tree">The tree to build slots from</param>
        /// <param name="slots">pre-existing slots. (default: null)</param>
        /// <param name="modifiedSlots">Pre-modified slots. (default: null)</param>
        /// <returns></returns>
        public virtual ItemSlot[] SlotsFromTreeAttributes(ITreeAttribute tree, ItemSlot[] slots = null, List<ItemSlot> modifiedSlots = null)
        {
            if (tree == null)
            {
                return slots;
            }

            if (slots == null /*|| slots.Length != tree.GetInt("qslots") - wtf is this good for? The slot count from tree attr might be outdated!*/)
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
                slots[slotId].Itemstack = newstack;

                if (Api?.World == null) continue;
                newstack?.ResolveBlockOrItem(Api.World);

                if (modifiedSlots != null)
                {
                    ItemStack oldstack = slots[slotId].Itemstack;
                    bool a = (newstack != null && !newstack.Equals(Api.World, oldstack));
                    bool b = (oldstack != null && !oldstack.Equals(Api.World, newstack));

                    bool didModify = a || b;
                    if (didModify) modifiedSlots.Add(slots[slotId]);
                }
            }



            return slots;
        }

        /// <summary>
        /// Sets the tree attribute using the slots.
        /// </summary>
        /// <param name="slots"></param>
        /// <param name="tree"></param>
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


        /// <summary>
        /// Gets a specified number of empty slots.
        /// </summary>
        /// <param name="quantity">the number of empty slots to get.</param>
        /// <returns>The pre-specified slots.</returns>
        public ItemSlot[] GenEmptySlots(int quantity)
        {
            ItemSlot[] slots = new ItemSlot[quantity];
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i] = NewSlot(i);
            }
            return slots;
        }

        /// <summary>
        /// A command to build a new empty slot.
        /// </summary>
        /// <param name="i">the index of the slot.</param>
        /// <returns>An empty slot bound to this inventory.</returns>
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

        /// <summary>
        /// Discards everything in the item slots.
        /// </summary>
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


        public virtual void DropSlotIfHot(ItemSlot slot, IPlayer player = null)
        {
            if (Api.Side == EnumAppSide.Client) return;
            if (slot.Empty) return;
            if (player != null && player.WorldData.CurrentGameMode == EnumGameMode.Creative) return;

            if (slot.Itemstack.Collectible.Attributes?.IsTrue("allowHotCrafting") != true && slot.Itemstack.Collectible.GetTemperature(Api.World, slot.Itemstack) > 300 && !hasHeatResistantHandGear(player))
            {
                (Api as ICoreServerAPI).SendIngameError(player as IServerPlayer, "requiretongs", Lang.Get("Requires tongs to hold"));
                player.Entity.ReceiveDamage(new DamageSource() { DamageTier = 0, Source = EnumDamageSource.Player, SourceEntity = player.Entity, Type = EnumDamageType.Fire }, 0.25f);
                player.InventoryManager.DropItem(slot, true);
            }
        }

        private bool hasHeatResistantHandGear(IPlayer player)
        {
            if (player == null) return false;
            return player.Entity.LeftHandItemSlot?.Itemstack?.Collectible.Attributes?.IsTrue("heatResistant") == true;
        }


        /// <summary>
        /// Drops the contents of the specified slots in the world.
        /// </summary>
        /// <param name="pos">The position of the inventory attached to the slots.</param>
        /// <param name="slotsIds">The slots to have their inventory drop.</param>
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

        /// <summary>
        /// Drops the contents of all the slots into the world.
        /// </summary>
        /// <param name="pos">Where to drop all this stuff.</param>
        /// <param name="maxStackSize">If non-zero, will split up the stacks into stacks of give max stack size</param>
        public virtual void DropAll(Vec3d pos, int maxStackSize = 0)
        {
            foreach (var slot in this)
            {
                if (slot.Itemstack == null) continue;

                if (maxStackSize > 0)
                {
                    while (slot.StackSize > 0)
                    {
                        ItemStack split = slot.TakeOut(GameMath.Clamp(slot.StackSize, 1, maxStackSize));
                        Api.World.SpawnItemEntity(split, pos);
                    }
                }
                else
                {

                    Api.World.SpawnItemEntity(slot.Itemstack, pos);
                }

                slot.Itemstack = null;
                slot.MarkDirty();
            }
        }

        /// <summary>
        /// Deletes the contents of all the slots
        /// </summary>
        public void Clear()
        {
            foreach (var slot in this) slot.Itemstack = null;
        }



        public virtual void OnOwningEntityDeath(Vec3d pos)
        {
            DropAll(pos);
        }


        /// <summary>
        /// Does this inventory speed up or slow down a transition for given itemstack? (Default: 1 for perish and 0 otherwise)
        /// </summary>
        /// <param name="transType"></param>
        /// <param name="stack"></param>
        /// <returns></returns>
        public virtual float GetTransitionSpeedMul(EnumTransitionType transType, ItemStack stack)
        {
            float mul = GetDefaultTransitionSpeedMul(transType);
            mul = InvokeTransitionSpeedDelegates(transType, stack, mul);

            return mul;
        }

        public float InvokeTransitionSpeedDelegates(EnumTransitionType transType, ItemStack stack, float mul)
        {
            if (OnAcquireTransitionSpeed != null)
            {
                var invocs = OnAcquireTransitionSpeed.GetInvocationList();
                foreach (CustomGetTransitionSpeedMulDelegate dele in invocs)
                {
                    mul *= dele.Invoke(transType, stack, mul);
                }
            }

            return mul;
        }

        protected virtual float GetDefaultTransitionSpeedMul(EnumTransitionType transitionType)
        {
            return transitionType switch
            {
                EnumTransitionType.Perish =>1,
                EnumTransitionType.Dry => 1,
                EnumTransitionType.Cure => 1,
                EnumTransitionType.Ripen => 1,
                EnumTransitionType.Melt => 1,
                EnumTransitionType.Harden => 1,
                _ => 0,
            };
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

            if(AuditLogAccess)
            {
                Api.World.Logger.Audit("{0} opened inventory {1}", player.PlayerName, InventoryID);
            }

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

            if(AuditLogAccess)
            {
                Api.World.Logger.Audit("{0} closed inventory {1}", player.PlayerName, InventoryID);
            }

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

        /// <summary>
        /// Gets the enumerator for the inventory.
        /// </summary>
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


        /// <summary>
        /// Return the slot where a chute may push items into. Return null if it shouldn't move items into this inventory.
        /// </summary>
        /// <param name="atBlockFace"></param>
        /// <param name="fromSlot"></param>
        /// <returns></returns>
        public virtual ItemSlot GetAutoPushIntoSlot(BlockFacing atBlockFace, ItemSlot fromSlot)
        {
            WeightedSlot wslot = GetBestSuitedSlot(fromSlot);
            return wslot.slot;
        }

        /// <summary>
        /// Return the slot where a chute may pull items from. Return null if it is now allowed to pull any items from this inventory
        /// </summary>
        /// <param name="atBlockFace"></param>
        /// <returns></returns>
        public virtual ItemSlot GetAutoPullFromSlot(BlockFacing atBlockFace)
        {
            return null;
        }
    }
}
