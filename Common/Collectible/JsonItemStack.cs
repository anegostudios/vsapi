using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;

namespace Vintagestory.API.Common
{
    [ProtoContract]
    public class JsonItemStack : IRecipeOutput
    {
        /// <summary>
        /// Block or Item?
        /// </summary>
        [ProtoMember(1)]
        public EnumItemClass Type;
        /// <summary>
        /// Code of the block or item
        /// </summary>
        [ProtoMember(2)]
        public AssetLocation Code;
        /// <summary>
        /// Amount of items in this stacks
        /// </summary>
        [ProtoMember(3)]
        public int StackSize = 1;
        /// <summary>
        /// Alias of <see cref="StackSize"/>
        /// </summary>
        public int Quantity
        {
            get { return StackSize;  }
            set { StackSize = value; }
        }

        /// <summary>
        /// Tree Attributes that should be attached to the resulting itemstack
        /// </summary>
        [JsonProperty, JsonConverter(typeof(JsonAttributesConverter))]
        [ProtoMember(4)]
        public JsonObject Attributes;

        /// <summary>
        /// The resolved item after conversion.
        /// </summary>
        public ItemStack ResolvedItemstack;
        

        /// <summary>
        /// Sets itemstack.block or itemstack.item
        /// </summary>
        /// <param name="resolver"></param>
        public bool Resolve(IWorldAccessor resolver, string sourceForErrorLogging)
        {
            if (Type == EnumItemClass.Block)
            {
                Block block = resolver.GetBlock(Code);
                if (block == null || block.IsMissing)
                {
                    resolver.Logger.Warning("Failed resolving a blocks blockdrop or smeltedstack with code {0} in {1}", Code, sourceForErrorLogging);
                    return false;
                }

                ResolvedItemstack = new ItemStack(block, StackSize);

            }
            else
            {
                Item item = resolver.GetItem(Code);
                if (item == null || item.IsMissing)
                {
                    resolver.Logger.Warning("Failed resolving a blocks itemdrop or smeltedstack with code {0} in {1}", Code, sourceForErrorLogging);
                    return false;
                }
                ResolvedItemstack = new ItemStack(item, StackSize);
            }

            if (Attributes != null)
            {
                IAttribute attributes = Attributes.ToAttribute();
                if (attributes is ITreeAttribute)
                {
                    ResolvedItemstack.Attributes = (ITreeAttribute)attributes;
                }
            }

            return true;
        }


        public bool Matches(IWorldAccessor worldForResolve, ItemStack inputStack)
        {
            return ResolvedItemstack.Equals(worldForResolve, inputStack, GlobalConstants.IgnoredStackAttributes);
        }


        /// <summary>
        /// Creates a deep copy of this object
        /// </summary>
        /// <returns></returns>
        public JsonItemStack Clone()
        {
            JsonItemStack stack = new JsonItemStack()
            {
                Code = Code.Clone(),
                ResolvedItemstack = ResolvedItemstack?.Clone(),
                StackSize = StackSize,
                Type = Type,
            };

            if (Attributes != null) stack.Attributes = Attributes.Clone();

            return stack;
        }

        /// <summary>
        /// Loads the ItemStack from the reader.
        /// </summary>
        /// <param name="reader">The reader to get the ItemStack from</param>
        /// <param name="instancer">The instancer for the ItemStack.</param>
        public virtual void FromBytes(BinaryReader reader, IClassRegistryAPI instancer)
        {
            Type = (EnumItemClass)reader.ReadInt16();
            Code = new AssetLocation(reader.ReadString());
            StackSize = reader.ReadInt32();

            if (reader.ReadBoolean())
            {
                ResolvedItemstack = new ItemStack(reader);
            }
            
        }

        /// <summary>
        /// Saves the ItemStack to file.
        /// </summary>
        /// <param name="writer">The writer to save the item to.</param>
        public virtual void ToBytes(BinaryWriter writer)
        {
            writer.Write((short)Type);
            writer.Write(Code.ToShortString());
            writer.Write(StackSize);
            writer.Write(ResolvedItemstack != null);
            if (ResolvedItemstack != null)
            {
                ResolvedItemstack.ToBytes(writer);
            }
        }

        public void FillPlaceHolder(string key, string value)
        {
            Code = Code.CopyWithPath(Code.Path.Replace("{" + key + "}", value));
            Attributes?.FillPlaceHolder(key, value);
        }
    }
}
