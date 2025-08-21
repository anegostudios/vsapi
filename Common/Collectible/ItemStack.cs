using System;
using System.IO;
using System.Text;
using Vintagestory.API.Datastructures;
using System.Collections.Generic;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Common
{
    public class ItemStack : IItemStack
    {
        /// <summary>
        /// Wether its a block Block or Item
        /// </summary>
        public EnumItemClass Class;

        /// <summary>
        /// The id of the block or item
        /// </summary>
        public int Id;


        protected int stacksize;

        TreeAttribute stackAttributes = new TreeAttribute();
        TreeAttribute tempAttributes = new TreeAttribute();

        protected Item item;
        protected Block block;

        /// <summary>
        /// The item/block base class this stack is holding
        /// </summary>
        public CollectibleObject Collectible
        {
            get {
                if (Class == EnumItemClass.Block) { return block; }
                return item;
            }
        }

        /// <summary>
        /// If this is a stack of items, this is the type of items it's holding, otherwise null
        /// </summary>
        public Item Item
        {
            get { return item; }
        }

        /// <summary>
        /// If this is a stack of blocks, this is the type of block it's holding, otherwise null
        /// </summary>
        public Block Block
        {
            get { return block; }
        }

        /// <summary>
        /// The amount of items/blocks in this stack
        /// </summary>
        public int StackSize
        {
            get { return stacksize; }
            set { stacksize = value; }
        }


        /// <summary>
        /// The id of the block or item
        /// </summary>
        int IItemStack.Id
        {
            get { return Id; }
        }



        /// <summary>
        /// Attributes assigned to this particular itemstack which are saved and synchronized.
        /// </summary>
        public ITreeAttribute Attributes
        {
            get { return stackAttributes; }
            set { stackAttributes = (TreeAttribute)value; }
        }



        /// <summary>
        /// Temporary Attributes assigned to this particular itemstack, not synchronized, not saved! Modifiable.
        /// </summary>
        public ITreeAttribute TempAttributes
        {
            get { return tempAttributes; }
            set { tempAttributes = (TreeAttribute)value; }
        }


        /// <summary>
        /// The Attributes assigned to the underlying block/item. Should not be modified, as it applies to globally.
        /// </summary>
        public JsonObject ItemAttributes
        {
            get { return Collectible.Attributes; }
        }

        /// <summary>
        /// Is it a Block or an Item?
        /// </summary>
        EnumItemClass IItemStack.Class
        {
            get { return Class; }
        }

        /// <summary>
        /// Create a new empty itemstack
        /// </summary>
        public ItemStack()
        {
        }

        /// <summary>
        /// Create a new itemstack with given collectible id, itemclass, stacksize, attributes and a resolver to turn the collectibe + itemclass into an Item/Block
        /// </summary>
        /// <param name="id"></param>
        /// <param name="itemClass"></param>
        /// <param name="stacksize"></param>
        /// <param name="stackAttributes"></param>
        /// <param name="resolver"></param>
        public ItemStack(int id, EnumItemClass itemClass, int stacksize, TreeAttribute stackAttributes, IWorldAccessor resolver)
        {
            this.Id = id;
            this.Class = itemClass;
            this.stacksize = stacksize;
            this.stackAttributes = stackAttributes;

            if (itemClass == EnumItemClass.Block)
            {
                block = resolver.GetBlock(id);
            } else
            {
                item = resolver.GetItem(id);
            }
        }

        /// <summary>
        /// Create a new itemstack from a byte serialized stream (without resolving the block/item)
        /// </summary>
        /// <param name="reader"></param>
        public ItemStack(BinaryReader reader)
        {
            FromBytes(reader);
        }

        /// <summary>
        /// Create a new itemstack from a byte serialized array(without resolving the block/item)
        /// </summary>
        /// <param name="data"></param>
        public ItemStack(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                using (var reader = new BinaryReader(ms))
                {
                    FromBytes(reader);
                }
            }
        }

        /// <summary>
        /// Create a new itemstack from a byte serialized stream (with resolving the block/item)
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="resolver"></param>
        public ItemStack(BinaryReader reader, IWorldAccessor resolver)
        {
            FromBytes(reader);
            if (Class == EnumItemClass.Block)
            {
                block = resolver.GetBlock(this.Id);
            }
            else
            {
                item = resolver.GetItem(Id);
            }
        }


        /// <summary>
        /// Create a new itemstack from given block/item and given stack size
        /// </summary>
        /// <param name="collectible"></param>
        /// <param name="stacksize"></param>
        public ItemStack(CollectibleObject collectible, int stacksize = 1)
        {
            if (collectible == null)
            {
                throw new Exception("Can't create itemstack without collectible!");
            }

            if (collectible is Block)
            {
                Class = EnumItemClass.Block;
                Id = collectible.Id;
                this.block = collectible as Block;
                this.stacksize = stacksize;
            } else
            {
                Class = EnumItemClass.Item;
                Id = collectible.Id;
                this.item = collectible as Item;
                this.stacksize = stacksize;
            }
        }

        /// <summary>
        /// Create a new itemstack from given item and given stack size
        /// </summary>
        /// <param name="item"></param>
        /// <param name="stacksize"></param>
        public ItemStack(Item item, int stacksize = 1)
        {
            if (item == null)
            {
                throw new Exception("Can't create itemstack without item!");
            }
            Class = EnumItemClass.Item;
            Id = item.ItemId;
            this.item = item;
            this.stacksize = stacksize;
        }

        /// <summary>
        /// Create a new itemstack from given block and given stack size
        /// </summary>
        /// <param name="block"></param>
        /// <param name="stacksize"></param>
        public ItemStack(Block block, int stacksize = 1)
        {
            if (block == null)
            {
                throw new Exception("Can't create itemstack without block!");
            }

            Class = EnumItemClass.Block;
            Id = block.BlockId;
            this.block = block;
            this.stacksize = stacksize;
        }

        /// <summary>
        /// Returns true if both stacks exactly match
        /// </summary>
        /// <param name="worldForResolve"></param>
        /// <param name="sourceStack"></param>
        /// <param name="ignoreAttributeSubTrees"></param>
        /// <returns></returns>
        public bool Equals(IWorldAccessor worldForResolve, ItemStack sourceStack, params string[] ignoreAttributeSubTrees)
        {
            if (Collectible == null) ResolveBlockOrItem(worldForResolve);

            return
                sourceStack != null && Collectible != null &&
                Collectible.Equals(this, sourceStack, ignoreAttributeSubTrees)
            ;
        }

        /// <summary>
        /// Returns true if this item stack is a satisfactory replacement for given itemstack. It's basically an Equals() test, but ignores additional attributes of the sourceStack
        /// </summary>
        /// <param name="sourceStack"></param>
        /// <returns></returns>
        public bool Satisfies(ItemStack sourceStack)
        {
            return
                sourceStack != null &&
                Collectible.Satisfies(this, sourceStack)
            ;
        }

        /// <summary>
        /// Replace all the properties (id, class, attributes, stacksize, etc...) from this item stack by given stack
        /// </summary>
        /// <param name="stack"></param>
        public void SetFrom(ItemStack stack)
        {
            this.Id = stack.Collectible.Id;
            this.Class = stack.Class;
            this.item = stack.item;
            this.block = stack.block;
            this.stacksize = stack.stacksize;
            this.stackAttributes = stack.stackAttributes.Clone() as TreeAttribute;
            this.tempAttributes = stack.tempAttributes.Clone() as TreeAttribute;
        }


        /// <summary>
        /// Turn the itemstack into a simple string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return stacksize + "x " + (Class == EnumItemClass.Block ? "Block" : "Item") + " Id " + Id + ", Code " + Collectible?.Code;
        }

        /// <summary>
        /// Serializes the itemstack into a series of bytes, including its stack attributes
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            using (var ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    ToBytes(writer);
                }

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Serializes the itemstack into a series of bytes, including its stack attributes
        /// </summary>
        /// <param name="stream"></param>
        public void ToBytes(BinaryWriter stream)
        {
            stream.Write((int)Class);
            stream.Write(Id);
            stream.Write(stacksize);
            stackAttributes.ToBytes(stream);
        }

        /// <summary>
        /// Reads all the itemstacks properties from a series of bytes, including its stack attributes
        /// </summary>
        /// <param name="stream"></param>
        public void FromBytes(BinaryReader stream)
        {
            Class = (EnumItemClass)stream.ReadInt32();
            Id = stream.ReadInt32();
            stacksize = stream.ReadInt32();
            stackAttributes.FromBytes(stream);
        }

        /// <summary>
        /// Sets the item/block based on the currently set itemclass + id
        /// </summary>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public bool ResolveBlockOrItem(IWorldAccessor resolver)
        {
            if (Class == EnumItemClass.Block)
            {
                block = resolver.GetBlock(Id);
                if (block == null) return false;
            } else
            {
                item = resolver.GetItem(Id);
                if (item == null) return false;
            }
            return true;
        }

        /// <summary>
        /// Returns true if searchText is found in the item/block name as supplied from GetName()
        /// </summary>
        /// <param name="world"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public bool MatchesSearchText(IWorldAccessor world, string searchText)
        {
           return
                GetName().CaseInsensitiveContains(searchText)
                || GetDescription(world, new DummySlot(this), false).CaseInsensitiveContains(searchText)
            ;

        }

        /// <summary>
        /// Returns a human readable name of the item/block
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return Collectible.GetHeldItemName(this);
        }

        /// <summary>
        /// Returns a human readable description of the item/block
        /// </summary>
        /// <param name="world"></param>
        /// <param name="inSlot"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        public string GetDescription(IWorldAccessor world, ItemSlot inSlot, bool debug = false)
        {
            StringBuilder dsc = new StringBuilder();
            Collectible.GetHeldItemInfo(inSlot, dsc, world, debug);
            return dsc.ToString();
        }


        /// <summary>
        /// Creates a full copy of the item stack
        /// </summary>
        /// <returns></returns>
        public ItemStack Clone()
        {
            ItemStack itemstack = GetEmptyClone();
            itemstack.stacksize = stacksize;

            return itemstack;
        }

        /// <summary>
        /// Creates a full copy of the item stack, except for its stack size.
        /// </summary>
        /// <returns></returns>
        public ItemStack GetEmptyClone()
        {
            ItemStack stack = new ItemStack() { item = item, block = block, Id = Id, Class = Class };
            if (stackAttributes != null)
            {
                stack.Attributes = Attributes.Clone();
            }

            return stack;
        }


        /// <summary>
        /// This method should always be called when an itemstack got loaded from the savegame or when it got imported.
        /// When this method return false, you should discard the itemstack because it could not get resolved and a warning will be logged.
        /// </summary>
        /// <param name="oldBlockMapping"></param>
        /// <param name="oldItemMapping"></param>
        /// <param name="worldForNewMapping"></param>
        public bool FixMapping(Dictionary<int, AssetLocation> oldBlockMapping, Dictionary<int, AssetLocation> oldItemMapping, IWorldAccessor worldForNewMapping)
        {
            AssetLocation code;
            if (Class == EnumItemClass.Item)
            {
                if (oldItemMapping.TryGetValue(Id, out code) && code != null)
                {
                    item = worldForNewMapping.GetItem(code);
                    if (item == null)
                    {
                        worldForNewMapping.Logger.Warning(
                            "Cannot fix itemstack mapping, item code {0} not found item registry. Will delete stack.", code);
                        return false;
                    }

                    Id = item.Id;
                    return true;
                }
            }
            else
            {
                if (oldBlockMapping.TryGetValue(Id, out code) && code != null)
                {
                    block = worldForNewMapping.GetBlock(code);
                    if (block == null)
                    {
                        worldForNewMapping.Logger.Warning(
                            "Cannot fix itemstack mapping, block code {0} not found block registry. Will delete stack.", code);
                        return false;
                    }

                    Id = block.Id;
                    return true;
                }
            }

            worldForNewMapping.Logger.Warning(
                "Cannot fix itemstack mapping, item/block id {0} not found in old mapping list. Will delete stack. ({1})", Id, Collectible);
            return false;
        }


        public override int GetHashCode()
        {
            return GetHashCode(null);
        }

        public int GetHashCode(string[] ignoredAttributes)
        {
            if (Class == EnumItemClass.Item)
            {
                return 0 ^ this.Id ^ Attributes.GetHashCode(ignoredAttributes);
            }
            else
            {
                return (1 << 17) ^ this.Id ^ Attributes.GetHashCode(ignoredAttributes);
            }
        }
    }
}
