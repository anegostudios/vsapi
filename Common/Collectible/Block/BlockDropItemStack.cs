using Newtonsoft.Json;
using System;
using System.IO;
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
        public EnumItemClass Type = EnumItemClass.Block;
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

        /// <summary>
        /// The resulting ItemStack for this block being broken by a tool.
        /// </summary>
        public ItemStack ResolvedItemstack;

        /// <summary>
        /// If set, the drop quantity will be modified by the collecting entity stat code - entity.Stats.GetBlended(code)
        /// </summary>
        public string DropModbyStat;


        static Random random = new Random();


        public BlockDropItemStack()
        {

        }

        public BlockDropItemStack(ItemStack stack, float chance = 1)
        {
            this.Type = stack.Class;
            this.Code = stack.Collectible.Code;
            Quantity.avg = chance;
            ResolvedItemstack = stack;
        }

        /// <summary>
        /// Sets itemstack.block or itemstack.item
        /// </summary>
        /// <param name="resolver"></param>
        /// <param name="sourceForErrorLogging"></param>
        /// <param name="assetLoc"></param>
        /// <returns></returns>
        public bool Resolve(IWorldAccessor resolver, string sourceForErrorLogging, AssetLocation assetLoc)
        {
            if (Type == EnumItemClass.Block)
            {
                Block block = resolver.GetBlock(Code);
                if (block == null)
                {
                    resolver.Logger.Warning("Failed resolving a blocks block drop or smeltedstack with code {0} in {1}", Code, sourceForErrorLogging + assetLoc);
                    return false;
                }

                ResolvedItemstack = new ItemStack(block);
                
            }
            else
            {
                Item item = resolver.GetItem(Code);
                if (item == null)
                {
                    resolver.Logger.Warning("Failed resolving a blocks item drop or smeltedstack with code {0} in {1}", Code, sourceForErrorLogging + assetLoc);
                    return false;
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

            return true;
        }


        /// <summary>
        /// Returns an itemstack with random quantity as configured via the Quantity field
        /// </summary>
        /// <returns></returns>
        public ItemStack GetNextItemStack(float dropQuantityMultiplier = 1f)
        {
            if (ResolvedItemstack == null) return null;

            int quantity = GameMath.RoundRandom(random, Quantity.nextFloat() * dropQuantityMultiplier);

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
                Code = Code?.Clone(),
                Quantity = Quantity,
                Type = Type,
                LastDrop = LastDrop,
                Tool = Tool,
                ResolvedItemstack = ResolvedItemstack,
                DropModbyStat = DropModbyStat
            };

            if (Attributes != null) stack.Attributes = Attributes.Clone();

            return stack;
        }

        /// <summary>
        /// Reads the contents of the block bytes and converts it into a block.
        /// </summary>
        /// <param name="reader">The reader of the block</param>
        /// <param name="instancer">The block registry</param>
        public virtual void FromBytes(BinaryReader reader, IClassRegistryAPI instancer)
        {
            Type = (EnumItemClass)reader.ReadInt16();
            Code = new AssetLocation(reader.ReadString());
            Quantity = NatFloat.One;
            Quantity.FromBytes(reader);
            ResolvedItemstack = new ItemStack(reader);
            LastDrop = reader.ReadBoolean();
            if (reader.ReadBoolean())
            {
                DropModbyStat = reader.ReadString();
            }
        }

        /// <summary>
        /// The save data writer.
        /// </summary>
        /// <param name="writer">The writer to write blocks to.</param>
        public virtual void ToBytes(BinaryWriter writer)
        {
            writer.Write((short)Type);
            writer.Write(Code.ToShortString());
            Quantity.ToBytes(writer);
            ResolvedItemstack.ToBytes(writer);
            writer.Write(LastDrop);
            writer.Write(DropModbyStat != null);
            if (DropModbyStat != null)
            {
                writer.Write(DropModbyStat);
            }
        }
    }
}
