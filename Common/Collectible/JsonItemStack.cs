using Newtonsoft.Json;
using ProtoBuf;
using System.IO;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// This is a representation of an item stack in JSON.
    /// It resembles a standard in-game item stack but can be stored before the game is loaded.
    /// </summary>
    /// <example>
    /// <code language="json">
    ///	"output": {
    ///		"type": "item",
    ///		"code": "knifeblade-flint",
    ///		"stacksize": 1
    ///	},
    /// </code>
    /// <code language="json">
    ///	"output": {
    ///		"type": "block",
    ///		"code": "ladder-wood-north",
    ///		"quantity": 3
    ///	},
    /// </code>
    /// </example>
    [DocumentAsJson]
    [ProtoContract]
    public class JsonItemStack : IRecipeOutput
    {
        /// <summary>
        /// <!--<jsonoptional>Recommended</jsonoptional><jsondefault>Block</jsondefault>-->
        /// Block or Item?
        /// </summary>
        [ProtoMember(1)]
        [DocumentAsJson] public EnumItemClass Type;

        /// <summary>
        /// <!--<jsonoptional>Required</jsonoptional>-->
        /// The asset location code of the block or item.
        /// </summary>
        [ProtoMember(2)]
        [DocumentAsJson] public AssetLocation Code;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>1</jsondefault>-->
        /// Amount of items in this stacks
        /// </summary>
        [ProtoMember(3)]
        [DocumentAsJson] public int StackSize = 1;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>1</jsondefault>-->
        /// Alias of <see cref="StackSize"/>. No real need to use this instead of it.
        /// </summary>
        [DocumentAsJson]
        public int Quantity
        {
            get { return StackSize;  }
            set { StackSize = value; }
        }

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// Tree Attributes that should be attached to the resulting itemstack.
        /// </summary>
        [JsonProperty, JsonConverter(typeof(JsonAttributesConverter))]
        [ProtoMember(4)]
        public JsonObject Attributes;

        /// <summary>
        /// The resolved item after conversion.
        /// </summary>
        public ItemStack ResolvedItemstack;
        

        public static JsonItemStack FromString(string jsonItemstack)
        {
            return JsonObject.FromJson(jsonItemstack).AsObject<JsonItemStack>();
        }

        /// <summary>
        /// Sets itemstack.block or itemstack.item
        /// </summary>
        /// <param name="resolver"></param>
        /// <param name="sourceForErrorLogging"></param>
        /// <param name="assetLoc"></param>
        /// <param name="printWarningOnError"></param>
        public bool Resolve(IWorldAccessor resolver, string sourceForErrorLogging, AssetLocation assetLoc, bool printWarningOnError = true)
        {
            if (Type == EnumItemClass.Block)
            {
                Block block = resolver.GetBlock(Code);
                if (block == null || block.IsMissing)
                {
                    if (printWarningOnError)
                    {
                        resolver.Logger.Warning("Failed resolving a blocks blockdrop or smeltedstack with code {0} in {1}", Code, sourceForErrorLogging + assetLoc);
                    }
                    return false;
                }

                ResolvedItemstack = new ItemStack(block, StackSize);

            }
            else
            {
                Item item = resolver.GetItem(Code);
                if (item == null || item.IsMissing)
                {
                    if (printWarningOnError)
                    {
                        resolver.Logger.Warning("Failed resolving a blocks itemdrop or smeltedstack with code {0} in {1}", Code, sourceForErrorLogging + assetLoc);
                    }
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

        public bool Resolve(IWorldAccessor resolver, string sourceForErrorLogging, bool printWarningOnError = true)
        {
            if (Type == EnumItemClass.Block)
            {
                Block block = resolver.GetBlock(Code);
                if (block == null || block.IsMissing)
                {
                    if (printWarningOnError)
                    {
                        resolver.Logger.Warning("Failed resolving a blocks blockdrop or smeltedstack with code {0} in {1}", Code, sourceForErrorLogging);
                    }
                    return false;
                }

                ResolvedItemstack = new ItemStack(block, StackSize);

            }
            else
            {
                Item item = resolver.GetItem(Code);
                if (item == null || item.IsMissing)
                {
                    if (printWarningOnError)
                    {
                        resolver.Logger.Warning("Failed resolving a blocks itemdrop or smeltedstack with code {0} in {1}", Code, sourceForErrorLogging);
                    }
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
