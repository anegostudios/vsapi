using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// A crafting recipe ingredient
    /// </summary>
    [DocumentAsJson]
    public class CraftingRecipeIngredient : IRecipeIngredient
    {
        /// <summary>
        /// <!--<jsonoptional>Recommended</jsonoptional><jsondefault>Block</jsondefault>-->
        /// Is the itemstack an item or a block?
        /// </summary>
        [DocumentAsJson] public EnumItemClass Type = EnumItemClass.Block;

        /// <summary>
        /// <!--<jsonoptional>Required</jsonoptional>-->
        /// The code of the item or block
        /// </summary>
        [DocumentAsJson] public AssetLocation Code { get; set; }

        /// <summary>
        /// <!--<jsonoptional>Required</jsonoptional>-->
        /// Attaches a name to a wildcard in an ingredient. This is used to substitute the value into the output. Only required if using a wildcard.
        /// </summary>
        [DocumentAsJson] public string Name { get; set; }

        /// <summary>
        /// <!--<jsonoptional>Recommended</jsonoptional><jsondefault>1</jsondefault>-->
        /// The quantity of the itemstack required for the recipe.
        /// </summary>
        [DocumentAsJson] public int Quantity = 1;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// What attributes this itemstack must have to be a valid ingredient
        /// </summary>
        [JsonProperty, JsonConverter(typeof(JsonAttributesConverter))]
        public JsonObject Attributes;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// Optional attribute data that you can attach any data to. Used for some specific instances in code mods.
        /// </summary>
        [JsonProperty, JsonConverter(typeof(JsonAttributesConverter))]
        public JsonObject RecipeAttributes;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>False</jsondefault>-->
        /// Whether this crafting recipe ingredient should be regarded as a tool required to build this item.
        /// If true, the recipe will not consume the item but reduce its durability.
        /// </summary>
        [DocumentAsJson] public bool IsTool;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>1</jsondefault>-->
        /// If <see cref="IsTool"/> is set, this is the durability cost when the recipe is created.
        /// </summary>
        [DocumentAsJson] public int ToolDurabilityCost = 1;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>Allow All</jsondefault>-->
        /// When using a wildcard in the item/block code, setting this field will limit the allowed variants
        /// </summary>
        [DocumentAsJson] public string[] AllowedVariants;


        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>Skip None</jsondefault>-->
        /// When using a wildcard in the item/block code, setting this field will skip these variants
        /// </summary>
        [DocumentAsJson] public string[] SkipVariants;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>None</jsondefault>-->
        /// If set, the crafting recipe will give back the consumed stack to be player upon crafting.
        /// Can also be used to produce multiple outputs for a recipe.
        /// </summary>
        [DocumentAsJson] public JsonItemStack ReturnedStack;

        /// <summary>
        /// The itemstack made from Code, Quantity and Attributes, populated by the engine
        /// </summary>
        public ItemStack ResolvedItemstack; 

        /// <summary>
        /// Whether this recipe contains a wildcard, populated by the engine
        /// </summary>
        public bool IsWildCard = false;

        public bool IsBasicWildCard = false;
        public bool IsAdvancedWildCard = false;
        public bool IsRegex = false;

        /// <summary>
        /// Turns Type, Code and Attributes into an IItemStack
        /// </summary>
        /// <param name="resolver"></param>
        /// <param name="sourceForErrorLogging"></param>
        public bool Resolve(IWorldAccessor resolver, string sourceForErrorLogging)
        {
            if (ReturnedStack != null)
            {
                ReturnedStack.Resolve(resolver, sourceForErrorLogging + " recipe with output ", Code);
            }

            if (IsBasicWildCard || IsAdvancedWildCard || IsRegex)
            {
                IsWildCard = true;
                return true;
            }

            if (Code.Path.Contains('*'))
            {
                IsWildCard = true;
                return true;
            }

            if (Type == EnumItemClass.Block)
            {
                Block block = resolver.GetBlock(Code);
                if (block == null || block.IsMissing)
                {
                    resolver.Logger.Warning("Failed resolving crafting recipe ingredient with code {0} in {1}", Code, sourceForErrorLogging);
                    return false;
                }

                ResolvedItemstack = new ItemStack(block, Quantity);
            }
            else
            {
                Item item = resolver.GetItem(Code);
                if (item == null || item.IsMissing)
                {
                    resolver.Logger.Warning("Failed resolving crafting recipe ingredient with code {0} in {1}", Code, sourceForErrorLogging);
                    return false;
                }
                ResolvedItemstack = new ItemStack(item, Quantity);
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
        /// Checks whether or not the input satisfies as an ingredient for the recipe.
        /// </summary>
        /// <param name="inputStack"></param>
        /// <param name="checkStacksize"></param>
        /// <returns></returns>
        public bool SatisfiesAsIngredient(ItemStack inputStack, bool checkStacksize = true)
        {
            if (inputStack == null) return false;

            if (IsWildCard)
            {
                if (Type != inputStack.Class) return false;
                if (checkStacksize && inputStack.StackSize < Quantity) return false;
                if (!WildcardUtil.Match(Code, inputStack.Collectible.Code, AllowedVariants)) return false;
                if (SkipVariants != null && WildcardUtil.Match(Code, inputStack.Collectible.Code, SkipVariants)) return false;
            }
            else
            {
                if (!ResolvedItemstack.Satisfies(inputStack)) return false;
                if (checkStacksize && inputStack.StackSize < ResolvedItemstack.StackSize) return false;
            }

            return true;
        }




        public CraftingRecipeIngredient Clone()
        {
            return CloneTo<CraftingRecipeIngredient>();
        }

        public T CloneTo<T>()  where T:CraftingRecipeIngredient, new()
        {
            T stack = new T()
            {
                Code = Code.Clone(),
                Type = Type,
                Name = Name,
                Quantity = Quantity,
                IsWildCard = IsWildCard,
                IsBasicWildCard = IsBasicWildCard,
                IsAdvancedWildCard = IsAdvancedWildCard,
                IsRegex = IsRegex,
                IsTool = IsTool,
                ToolDurabilityCost = ToolDurabilityCost,
                AllowedVariants = AllowedVariants == null ? null : (string[])AllowedVariants.Clone(),
                SkipVariants = SkipVariants == null ? null : (string[])SkipVariants.Clone(),
                ResolvedItemstack = ResolvedItemstack?.Clone(),
                ReturnedStack = ReturnedStack?.Clone(),
                RecipeAttributes = RecipeAttributes?.Clone()
            };

            if (Attributes != null) stack.Attributes = Attributes.Clone();

            return stack;
        }

        public override string ToString()
        {
            return Type + " code " + Code;
        }
        

        /// <summary>
        /// Fills in the placeholder ingredients for the crafting recipe.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void FillPlaceHolder(string key, string value)
        {
            Code = Code.CopyWithPath(Code.Path.Replace("{" + key + "}", value));
            Attributes?.FillPlaceHolder(key, value);
            RecipeAttributes?.FillPlaceHolder(key, value);
        }

        public virtual void ToBytes(BinaryWriter writer)
        {
            writer.Write(IsWildCard);
            writer.Write(IsBasicWildCard);
            writer.Write(IsAdvancedWildCard);
            writer.Write(IsRegex);
            writer.Write((int)Type);
            writer.Write(Code.ToShortString());
            writer.Write(Quantity);
            if (!IsWildCard)
            {
                writer.Write(ResolvedItemstack != null);
                ResolvedItemstack?.ToBytes(writer);
            }

            writer.Write(IsTool);
            writer.Write(ToolDurabilityCost);

            writer.Write(AllowedVariants != null);
            if (AllowedVariants != null)
            {
                writer.Write(AllowedVariants.Length);
                for (int i = 0; i < AllowedVariants.Length; i++)
                {
                    writer.Write(AllowedVariants[i]);
                }
            }

            writer.Write(SkipVariants != null);
            if (SkipVariants != null)
            {
                writer.Write(SkipVariants.Length);
                for (int i = 0; i < SkipVariants.Length; i++)
                {
                    writer.Write(SkipVariants[i]);
                }
            }

            writer.Write(ReturnedStack?.ResolvedItemstack != null);
            if (ReturnedStack?.ResolvedItemstack != null)
            {
                ReturnedStack.ToBytes(writer);
            }

            if (RecipeAttributes != null)
            {
                writer.Write(true);
                writer.Write(RecipeAttributes.ToString());
            } else
            {
                writer.Write(false);
            }
        }

        public virtual void FromBytes(BinaryReader reader, IWorldAccessor resolver)
        {
            IsWildCard = reader.ReadBoolean();
            IsBasicWildCard = reader.ReadBoolean();
            IsAdvancedWildCard = reader.ReadBoolean();
            IsRegex = reader.ReadBoolean();
            Type = (EnumItemClass)reader.ReadInt32();
            Code = new AssetLocation(reader.ReadString());
            Quantity = reader.ReadInt32();
            if (!IsWildCard && reader.ReadBoolean())
            {
                ResolvedItemstack = new ItemStack(reader, resolver);
            }

            IsTool = reader.ReadBoolean();
            ToolDurabilityCost = reader.ReadInt32();

            bool haveVariants = reader.ReadBoolean();
            if (haveVariants)
            {
                AllowedVariants = new string[reader.ReadInt32()];
                for (int i = 0; i < AllowedVariants.Length; i++)
                {
                    AllowedVariants[i] = reader.ReadString();
                }
            }

            bool haveSkipVariants = reader.ReadBoolean();
            if (haveSkipVariants)
            {
                SkipVariants = new string[reader.ReadInt32()];
                for (int i = 0; i < SkipVariants.Length; i++)
                {
                    SkipVariants[i] = reader.ReadString();
                }
            }

            bool haveConsumedStack = reader.ReadBoolean();
            if (haveConsumedStack)
            {
                ReturnedStack = new JsonItemStack();
                ReturnedStack.FromBytes(reader, resolver.ClassRegistry);
                ReturnedStack.ResolvedItemstack.ResolveBlockOrItem(resolver);
            }

            if (reader.ReadBoolean())
            {
                RecipeAttributes = new JsonObject(JToken.Parse(reader.ReadString()));
            }
        }
    }
}
