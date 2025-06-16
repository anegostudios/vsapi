using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Common
{
    public interface IOwnedInventory
    {
        Entity Owner { get; }
    }

    /// <summary>
    /// Bag is a non-placed block container, usually one that is attached to an entity
    /// </summary>
    public interface IHeldBag
    {

        /// <summary>
        /// Should true if this this bag is empty
        /// </summary>
        /// <param name="bagstack"></param>
        /// <returns></returns>
        bool IsEmpty(ItemStack bagstack);
        /// <summary>
        /// Amount of slots this bag provides
        /// </summary>
        /// <param name="bagstack"></param>
        /// <returns></returns>
        int GetQuantitySlots(ItemStack bagstack);
        /// <summary>
        /// Should return all contents of this bag
        /// </summary>
        /// <param name="bagstack"></param>
        /// <param name="world"></param>
        /// <returns></returns>
        ItemStack[] GetContents(ItemStack bagstack, IWorldAccessor world);

        List<ItemSlotBagContent> GetOrCreateSlots(ItemStack bagstack, InventoryBase parentinv, int bagIndex, IWorldAccessor world);

        /// <summary>
        /// Save given itemstack into this bag
        /// </summary>
        /// <param name="bagstack"></param>
        /// <param name="slot"></param>
        void Store(ItemStack bagstack, ItemSlotBagContent slot);
        /// <summary>
        /// Delete all contents of this bag
        /// </summary>
        /// <param name="bagstack"></param>
        void Clear(ItemStack bagstack);
        /// <summary>
        /// The Hex color the bag item slot should take, return null for default
        /// </summary>
        /// <param name="bagstack"></param>
        /// <returns></returns>
        string GetSlotBgColor(ItemStack bagstack);
        /// <summary>
        /// The types of items that can be stored in this bag
        /// </summary>
        /// <param name="bagstack"></param>
        /// <returns></returns>
        EnumItemStorageFlags GetStorageFlags(ItemStack bagstack);
    }

    public class ItemSlotBagContent : ItemSlotSurvival
    {
        public int BagIndex;
        public int SlotIndex;

        public EnumItemStorageFlags storageType;

        public override EnumItemStorageFlags StorageType => storageType;


        public ItemSlotBagContent(InventoryBase inventory, int BagIndex, int SlotIndex, EnumItemStorageFlags storageType) : base(inventory)
        {
            this.BagIndex = BagIndex;
            this.storageType = storageType;
            this.SlotIndex = SlotIndex;
        }
    }

    /// <summary>
    /// The contents of one or more bags
    /// </summary>
    public class BagInventory : IReadOnlyCollection<ItemSlot>
    {
        protected ICoreAPI Api;

        protected List<ItemSlot> bagContents = new List<ItemSlot>();
        public int Count => bagContents.Count;

        public ItemSlot[] BagSlots { get => bagSlots; set => bagSlots = value; }

        private ItemSlot[] bagSlots;

        public BagInventory(ICoreAPI api, ItemSlot[] bagSlots)
        {
            this.BagSlots = bagSlots;
            this.Api = api;
        }


        public ItemSlot this[int slotId]
        {
            get
            {
                return bagContents[slotId];
            }
            set
            {
                bagContents[slotId] = value;
            }
        }

        public void SaveSlotIntoBag(ItemSlotBagContent slot)
        {
            ItemStack backPackStack = BagSlots[slot.BagIndex].Itemstack;

            backPackStack?.Collectible.GetCollectibleInterface<IHeldBag>().Store(backPackStack, slot);
        }

        public void SaveSlotsIntoBags()
        {
            if (BagSlots == null) return;

            /*foreach (var bagslot in BagSlots)
            {
                if (bagslot.Empty) continue;
                ItemStack backPackStack = bagslot.Itemstack;
                backPackStack.Collectible.GetCollectibleInterface<IHeldBag>().Clear(backPackStack);
            }*/
            
            foreach (var slot in bagContents)
            {
                SaveSlotIntoBag((ItemSlotBagContent)slot);
            }
        }

        public void ReloadBagInventory(InventoryBase parentinv, ItemSlot[] bagSlots)
        {
            this.BagSlots = bagSlots;

            if (BagSlots == null || BagSlots.Length == 0)
            {
                bagContents.Clear();
                return;
            } 

            bagContents.Clear();
            for (int bagIndex = 0; bagIndex < BagSlots.Length; bagIndex++)
            {
                ItemStack bagstack = BagSlots[bagIndex].Itemstack;
                if (bagstack == null || bagstack.ItemAttributes == null) continue;

                bagstack.ResolveBlockOrItem(Api.World);

                var bag = bagstack.Collectible.GetCollectibleInterface<IHeldBag>();
                if (bag != null)
                {
                    var slots = bag.GetOrCreateSlots(bagstack, parentinv, bagIndex, Api.World);
                    bagContents.AddRange(slots);
                }
            }


            if (Api is ICoreClientAPI capi)
            {
                ItemSlotBagContent currentHoveredSlot = capi.World.Player?.InventoryManager.CurrentHoveredSlot as ItemSlotBagContent;

                if (currentHoveredSlot?.Inventory == parentinv)
                {
                    var hslot = bagContents.FirstOrDefault((slot) => (slot as ItemSlotBagContent).SlotIndex == currentHoveredSlot.SlotIndex && (slot as ItemSlotBagContent).BagIndex == currentHoveredSlot.BagIndex);
                    if (hslot != null)
                    {
                        capi.World.Player.InventoryManager.CurrentHoveredSlot = hslot;
                    }
                }
            }
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

    }
}
