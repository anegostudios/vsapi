using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;

namespace Vintagestory.API.Common
{
    public class JsonItemStack
    {
        /// <summary>
        /// Block or Item?
        /// </summary>
        public EnumItemClass Type;
        /// <summary>
        /// Code of the block or item
        /// </summary>
        public AssetLocation Code;
        /// <summary>
        /// Quantity
        /// </summary>
        public int StackSize = 1;
        /// <summary>
        /// Tree Attributes that should be attached to the resulting itemstack
        /// </summary>
        [JsonProperty, JsonConverter(typeof(JsonAttributesConverter))]
        public JsonObject Attributes;

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
                if (block == null)
                {
                    resolver.Logger.Warning("Failed resolving block blockdrop or smeltedstack with code {0} in {1}", Code, sourceForErrorLogging);
                    return false;
                }

                ResolvedItemstack = new ItemStack(block, StackSize);

            }
            else
            {
                Item item = resolver.GetItem(Code);
                if (item == null)
                {
                    resolver.Logger.Warning("Failed resolving block itemdrop or smeltedstack with code {0} in {1}", Code, sourceForErrorLogging);
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
