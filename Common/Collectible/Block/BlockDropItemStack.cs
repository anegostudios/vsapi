using Newtonsoft.Json;
using System;
using System.IO;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Represents an itemstack that is dropped when breaking a block, with a potentially random quantity.
    /// </summary>
    /// <example>
    /// <code language="json">
    ///"drops": [
	///	{
	///		"type": "item",
	///		"code": "bone",
	///		"quantity": {
	///			"avg": 4,
	///			"var": 2
	///		}
	///	}
	///]
    /// </code>
    /// </example>
    [DocumentAsJson]
    public class BlockDropItemStack
    {
        /// <summary>
        /// <!--<jsonoptional>Recommended</jsonoptional><jsondefault>Block</jsondefault>-->
        /// Block or Item?
        /// </summary>
        [DocumentAsJson] public EnumItemClass Type = EnumItemClass.Block;

        /// <summary>
        /// <!--<jsonoptional>Required</jsonoptional>-->
        /// Code of the block or item
        /// </summary>
        [DocumentAsJson] public AssetLocation Code;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>1</jsondefault>-->
        /// Quantity to be dropped
        /// </summary>
        [DocumentAsJson] public NatFloat Quantity = NatFloat.One;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// Tree Attributes that should be attached to the resulting itemstack
        /// </summary>
        [JsonProperty, JsonConverter(typeof(JsonAttributesConverter))]
        public JsonObject Attributes;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>false</jsondefault>-->
        /// If true, and this drop occurs, no further drops will happen.
        /// </summary>
        [DocumentAsJson] public bool LastDrop = false;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// If set, then the given tool is required to make this block drop anything.
        /// </summary>
        [DocumentAsJson] public EnumTool? Tool;

        /// <summary>
        /// The resulting ItemStack for this block being broken by a tool.
        /// </summary>
        public ItemStack ResolvedItemstack;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// If set, the drop quantity will be modified by the collecting entity stat code - entity.Stats.GetBlended(code).
        /// </summary>
        [DocumentAsJson] public string DropModbyStat;


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

            int quantity = GameMath.RoundRandom(random, Quantity.nextFloat(dropQuantityMultiplier));

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

        public ItemStack ToRandomItemstackForPlayer(IPlayer byPlayer, IWorldAccessor world, float dropQuantityMultiplier)
        {
            if (Tool != null && (byPlayer == null || Tool != byPlayer.InventoryManager.ActiveTool)) return null;

            float extraMul = 1f;
            if (byPlayer != null && DropModbyStat != null)
            {
                // If the stat does not exist, then GetBlended returns 1 \o/
                extraMul = byPlayer.Entity.Stats.GetBlended(DropModbyStat);
            }

            ItemStack stack = GetNextItemStack(dropQuantityMultiplier * extraMul);

            if (stack?.Collectible is IResolvableCollectible irc)
            {
                var slot = new DummySlot(stack);
                irc.Resolve(slot, world);
                stack = slot.Itemstack;
            }

            return stack;
        }
    }
}
