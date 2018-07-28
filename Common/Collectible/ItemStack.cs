using System;
using Vintagestory.API;
using ProtoBuf;
using System.IO;
using System.Text;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Config;
using System.Collections.Generic;

namespace Vintagestory.API.Common
{
    public class ItemStack : IItemStack
    {
        public EnumItemClass Class;
        public int Id;


        protected int stacksize;

        TreeAttribute stackAttributes = new TreeAttribute();
        TreeAttribute tempAttributes = new TreeAttribute();

        protected Item item;
        protected Block block;

        public CollectibleObject Collectible
        {
            get {
                if (Class == EnumItemClass.Block) { return block; }
                return item;
            }
        }

        public Item Item
        {
            get { return item; }
        }

        public Block Block
        {
            get { return block; }
        }

        public int StackSize
        {
            get { return stacksize; }
            set { stacksize = value; }
        }
        

        int IItemStack.Id
        {
            get { return Id; }
        }



        /// <summary>
        /// Attributes assigned to this particular itemstack. Modifiable.
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
        /// Attributes assigned to the collectiable. Should not be modified.
        /// </summary>
        public JsonObject ItemAttributes
        {
            get { return Collectible.Attributes; }
        }

        EnumItemClass IItemStack.Class
        {
            get { return Class; }
        }

        public ItemStack()
        {
        }

        public ItemStack(int id, EnumItemClass itemClass, int stacksize, TreeAttribute stackAttributes, IWorldAccessor resolver)
        {
            this.Id = id;
            this.Class = itemClass;
            this.stacksize = stacksize;
            this.stackAttributes = stackAttributes;
            
            if (Class == EnumItemClass.Block)
            {
                block = resolver.GetBlock((ushort)this.Id);
            } else
            {
                item = resolver.GetItem(Id);
            }
        }

        public ItemStack(BinaryReader reader)
        {
            FromBytes(reader);
        }

        public ItemStack(BinaryReader reader, IWorldAccessor resolver)
        {
            FromBytes(reader);
            if (Class == EnumItemClass.Block)
            {
                block = resolver.GetBlock((ushort)this.Id);
            }
            else
            {
                item = resolver.GetItem(Id);
            }
        }


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
        /// <param name="sourceStack"></param>
        /// <param name="ignoreAttributeSubTrees"></param>
        /// <returns></returns>
        public bool Equals(ItemStack sourceStack, params string[] ignoreAttributeSubTrees)
        {
            return
                sourceStack != null &&
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


        public override string ToString()
        {
            return stacksize + "x " + (Class == EnumItemClass.Block ? "Block" : "Item") + " Id " + Id + ", Code " + Collectible?.Code;
        }
        


        public void ToBytes(BinaryWriter stream)
        {
            stream.Write((int)Class);
            stream.Write(Id);
            stream.Write(stacksize);
            stackAttributes.ToBytes(stream);
        }

        public void FromBytes(BinaryReader stream)
        {
            Class = (EnumItemClass)stream.ReadInt32();
            Id = stream.ReadInt32();
            stacksize = stream.ReadInt32();
            stackAttributes.FromBytes(stream);
        }


        public bool ResolveBlockOrItem(IWorldAccessor resolver)
        {
            if (Class == EnumItemClass.Block)
            {
                block = resolver.GetBlock((ushort)Id);
                if (block == null) return false;
            } else
            {
                item = resolver.GetItem(Id);
                if (item == null) return false;
            }
            return true;
        }


        public bool MatchesSearchText(string searchText)
        {
            if (Class == EnumItemClass.Block)
            {
                return GetName().ToLowerInvariant().Contains(searchText.ToLowerInvariant());
            }
            else
            {
                return GetName().ToLowerInvariant().Contains(searchText.ToLowerInvariant());
            }
        }

        public string GetName()
        {
            string name = "";
            string type = Class == EnumItemClass.Block ? "block" : "item";

            name = Lang.GetMatching(Collectible.Code?.Domain + AssetLocation.LocationSeparator + type + "-" + Collectible.Code?.Path);

            return name;
        }

        public string GetDescription(IWorldAccessor world, bool debug = false)
        {
            StringBuilder dsc = new StringBuilder();
            Collectible.GetHeldItemInfo(this, dsc, world, debug);
            return dsc.ToString();
        }



        public ItemStack Clone()
        {
            ItemStack itemstack = GetEmptyClone();
            itemstack.stacksize = stacksize;

            return itemstack;
        }

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
        /// If this itemstack was imported from another savegame you might want to call this method to correct the blockid/itemid for this savegame
        /// </summary>
        /// <param name="oldItemIdMapping"></param>
        /// <param name="worldForNewMapping"></param>
        public void FixMapping(Dictionary<int, AssetLocation> oldBlockMapping, Dictionary<int, AssetLocation> oldItemMapping, IWorldAccessor worldForNewMapping)
        {
            AssetLocation code = null;
            
            if (Class == EnumItemClass.Item)
            {
                if (oldItemMapping.TryGetValue(Id, out code))
                {
                    item = worldForNewMapping.GetItem(code);
                    Id = item.Id;
                    return;
                }
            } else
            {
                if (oldBlockMapping.TryGetValue(Id, out code))
                {
                    block = worldForNewMapping.GetBlock(code);
                    Id = block.Id;
                    return;
                }
            }

            Console.WriteLine("missing mapping?");
        }
    }
}
