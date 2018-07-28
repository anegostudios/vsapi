using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Represents an itemstack that is dropped by chance when breaking a block
    /// </summary>
    public class BlockDropItemStack
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
        /// Quantity to be dropped
        /// </summary>
        public NatFloat Quantity = NatFloat.One;
        /// <summary>
        /// Tree Attributes that should be attached to the resulting itemstack
        /// </summary>
        [JsonProperty, JsonConverter(typeof(JsonAttributesConverter))]
        public JsonObject Attributes;
        /// <summary>
        /// If true and the quantity dropped is >=1 any subsequent drop will be ignored
        /// </summary>
        public bool LastDrop = false;
        /// <summary>
        /// If not null then given tool is required to break this block
        /// </summary>
        public EnumTool? Tool;


        public IItemStack ResolvedItemstack;

        static Random random = new Random();


        /// <summary>
        /// Sets itemstack.block or itemstack.item
        /// </summary>
        /// <param name="resolver"></param>
        public void Resolve(IWorldAccessor resolver, string sourceForErrorLogging)
        {
            if (Type == EnumItemClass.Block)
            {
                Block block = resolver.GetBlock(Code);
                if (block == null)
                {
                    resolver.Logger.Warning("Failed resolving block blockdrop or smeltedstack with code {0} in {1}", Code, sourceForErrorLogging);
                    return;
                }

                ResolvedItemstack = new ItemStack(block);
                
            }
            else
            {
                Item item = resolver.GetItem(Code);
                if (item == null)
                {
                    resolver.Logger.Warning("Failed resolving block itemdrop or smeltedstack with code {0} in {1}", Code, sourceForErrorLogging);
                    return;
                }
                ResolvedItemstack = new ItemStack(item);
            }

            if (Attributes != null)
            {
                IAttribute attributes = Attributes.ToAttribute();
                if (attributes is ITreeAttribute)
                {
                    ResolvedItemstack.Attributes = (ITreeAttribute)attributes;
                }
            }
        }

        /// <summary>
        /// Returns an itemstack with random quantity as configured via the Quantity field
        /// </summary>
        /// <returns></returns>
        public ItemStack GetNextItemStack(float dropQuantityMultiplier = 1f)
        {
            if (ResolvedItemstack == null) return null;

            float val = Quantity.nextFloat() * dropQuantityMultiplier;
            int quantity = (int)val + (((val - (int)val) > random.NextDouble()) ? 1 : 0);
            if (quantity <= 0) return null;

            ItemStack cloned = ResolvedItemstack.Clone();
            cloned.StackSize = quantity;

            return cloned;
        }


        /// <summary>
        /// Creates a deep copy of this object
        /// </summary>
        /// <returns></returns>
        public BlockDropItemStack Clone()
        {
            BlockDropItemStack stack = new BlockDropItemStack()
            {
                Code = Code.Clone(),
                Quantity = Quantity,
                Type = Type,
                LastDrop = LastDrop,
                Tool = Tool,
                ResolvedItemstack = ResolvedItemstack
            };

            if (Attributes != null) stack.Attributes = Attributes.Clone();

            return stack;
        }


        public virtual void FromBytes(BinaryReader reader, IClassRegistryAPI instancer)
        {
            Type = (EnumItemClass)reader.ReadInt16();
            Code = new AssetLocation(reader.ReadString());
            Quantity = NatFloat.One;
            Quantity.FromBytes(reader);
            ResolvedItemstack = new ItemStack(reader);
            LastDrop = reader.ReadBoolean();
        }

        public virtual void ToBytes(BinaryWriter writer)
        {
            writer.Write((short)Type);
            writer.Write(Code.ToShortString());
            Quantity.ToBytes(writer);
            ResolvedItemstack.ToBytes(writer);
            writer.Write(LastDrop);
        }
    }
}
