using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProtoBuf;
using System;
using System.IO;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;

namespace Vintagestory.API.Common;

/// <summary>
/// A crafting recipe ingredient
/// </summary>
[DocumentAsJson]
public class CraftingRecipeIngredient : IRecipeIngredient, IRecipeOutput
{
    #region From JSON
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Justification: loaded from json, so property/field being null is error on level of json
    /// <summary>
    /// Is the itemstack an item or a block?
    /// </summary>
    [DocumentAsJson("Recommended", "Block")]
    public EnumItemClass Type { get; set; } = EnumItemClass.Block;

    /// <summary>
    /// The code of the item or block.
    /// </summary>
    [DocumentAsJson("Optional", "All")]
    public AssetLocation? Code { get; set; } = new("*", "*");

    [DocumentAsJson("Optional", "None")]
    public ComplexTagCondition<TagSet> Tags { get; set; }

    /// <summary>
    /// Attaches a name to a wildcard in an ingredient. This is used to substitute the value into the output. Only required if using a wildcard.
    /// </summary>
    [DocumentAsJson("Required")]
    public string? Name { get; set; }

    /// <summary>
    /// Amount of items in this stacks
    /// </summary>
    [ProtoMember(3)]
    [DocumentAsJson("Optional", "1")]
    public int StackSize { get; set; } = 1;

    /// <summary>
    /// The quantity of the itemstack required for the recipe. Alias of <see cref="StackSize"/>.
    /// </summary>
    [DocumentAsJson("Recommended", "1")]
    public int Quantity
    {
        get { return StackSize; }
        set { StackSize = value; }
    }

    /// <summary>
    /// What attributes this itemstack must have to be a valid ingredient
    /// </summary>
    [JsonProperty, JsonConverter(typeof(JsonAttributesConverter))]
    [DocumentAsJson("Optional", "None")]
    public JsonObject? Attributes { get; set; }

    /// <summary>
    /// Optional attribute data that you can attach any data to. Used for some specific instances in code mods.
    /// </summary>
    [JsonProperty, JsonConverter(typeof(JsonAttributesConverter))]
    [DocumentAsJson("Optional", "None")]
    public JsonObject? RecipeAttributes { get; set; }

    /// <summary>
    /// Whether this crafting recipe ingredient should be regarded as a tool required to build this item.
    /// If true, the recipe will not consume the item but reduce its durability.
    /// </summary>
    [DocumentAsJson("Optional", "False")]
    public bool IsTool { get; set; } = false;


    /// <summary>
    /// If <see cref="Consume"/> is set to False, this is the durability cost when the recipe is created.
    /// </summary>
    [DocumentAsJson("Optional", "1")]
    public int ToolDurabilityCost { get; set; } = 1;

    /// <summary>
    /// When using a wildcard in the item/block code, setting this field will limit the allowed variants
    /// </summary>
    [DocumentAsJson("Optional", "Allow All")]
    public string[]? AllowedVariants { get; set; }


    /// <summary>
    /// When using a wildcard in the item/block code, setting this field will skip these variants
    /// </summary>
    [DocumentAsJson("Optional", "Skip None")]
    public string[]? SkipVariants { get; set; }

    /// <summary>
    /// If set, the crafting recipe will give back the consumed stack to be player upon crafting.
    /// Can also be used to produce multiple outputs for a recipe.
    /// </summary>
    [DocumentAsJson("Optional", "None")]
    public JsonItemStack? ReturnedStack { get; set; }

    /// <summary>
    /// Is used to reference recipe ingredient in the recipe, for example to specify where to copy attributes from.<br/>
    /// If not specified will be set to the index of this ingredient in the array (starting from 1), or key of this ingredient in the object.
    /// </summary>
    [DocumentAsJson("Optional", "None")]
    public string Id { get; set; }

    /// <summary>
    /// Defines if recipe ingredient should be consumed on crafting.<br/>
    /// Is equal to 'not IsTool'. Use this property instead of 'IsTool'.
    /// </summary>
    [DocumentAsJson("Optional", "True")]
    public bool Consume { get; set; } = true;

    /// <summary>
    /// Defines how item durability will be changed. Durability will be capped at maximum item durability, and wont reduce below 0, unless specific item class changes this behavior.
    /// </summary>
    [DocumentAsJson("Optional", "-1")]
    public int DurabilityChange { get; set; } = 0;

    /// <summary>
    /// Determines if and item will be destroyed when reaching zero durability.
    /// </summary>
    [DocumentAsJson("Optional", "True")]
    public bool Break { get; set; } = true;
#pragma warning restore CS8618
    #endregion

    #region Resolved
    public static ITreeAttribute defaultEmptyAttributes = new TreeAttribute();

    /// <summary>
    /// Defines how <see cref="Code"/> will be used to match
    /// </summary>
    public EnumRecipeMatchType MatchingType { get; set; } = EnumRecipeMatchType.Exact;

    /// <summary>
    /// The itemstack made from Code, Quantity and Attributes, populated by the engine.  For performance, consider referencing the ResolvedAttributes or Quantity of this CraftingRecipeIngredient instead of the Attributes or StackSize of the ResolvedItemStack, it may save having to clone the ResolvedItemStack
    /// </summary>
    public ItemStack? ResolvedItemStack
    {
        get {
            if (deduplicationIndex < 0 || resolvedItemStack != null)
            {
                return resolvedItemStack;   // Subtle point: resolvedItemStack will be non-null for a deduplicationIndex >= 0 if-and-only-if this is a clone of a recipe's ingredient: see also CloneTo() method below.  This results in the expected behaviour from a cloned CraftingRecipeIngredient, e.g. the StackSize and Attributes of the ResolvedItemStack can then be safely modified
            }
            // The following code is necessary if we need to construct a ResolvedItemStack from the ingredient in the de-duplicated list, where only one ResolvedItemStack is stored for each ingredient; for the stored ResolvedItemStack, we generally assume (and here, enforce) that .StackSize should be 1 and .Attributes should be empty.

            var stack = world!.FastSearchRecipesByIngredient.GetAt(deduplicationIndex).Key.ResolvedItemStack;   // world cannot be null if deduplicationIndex >= 0
            if (stack == null) return null;

            // We populate the StackSize and Attributes of the ResolvedItemStack correctly at the time of fetching it

            stack.StackSize = 1;
            if (StackSize != 1 || ResolvedAttributes != null)
            {
                stack = stack.Clone();   // We clone it only if we will change the StackSize or Attributes to non-defaults - not too much performance impact, both are uncommon in most recipes
                stack.StackSize = StackSize;
            }
            if (ResolvedAttributes != null)
            {
                stack.Attributes = ResolvedAttributes;
            }
            else
            {
                if (defaultEmptyAttributes.Count != 0) defaultEmptyAttributes = new TreeAttribute();    // Protective just in case a mod directly modified the Attributes of a ResolvedItemStack without cloning it first
                stack.Attributes = defaultEmptyAttributes;
            }

            return stack;
        }
        set {
            if (deduplicationIndex < 0)
            {
                resolvedItemStack = value;
                return;
            }
            world!.FastSearchRecipesByIngredient.GetAt(deduplicationIndex).Key.ResolvedItemStack = value;
        }
    }
    /// <summary>
    /// Only used if we have not de-duplicated this CraftingRecipeIngredient (e.g. for RightClickConstruction ingredients)
    /// </summary>
    protected ItemStack? resolvedItemStack = null;
    public ITreeAttribute? ResolvedAttributes = null;

    public RecipeIngredientConsumeProperties ConsumeProperties => GetConsumeProperties();

    [Obsolete("Use MatchingType")]
    public bool IsWildCard
    {
        get => MatchingType != EnumRecipeMatchType.Exact;
        set => MatchingType = value ? MatchingType : EnumRecipeMatchType.Exact;
    }

    private int deduplicationIndex = -1;
    private IWorldAccessor? world;
    #endregion

    /// <summary>
    /// For INPUT ingredients this should be used in place of the overload Resolve(IWorldAccessor world, string sourceForErrorLogging), to make use of the de-duplication / fast-search system for ingredients, essential for GridRecipes in the Handbook and in the Crafting Grid
    /// </summary>
    public virtual bool Resolve(IWorldAccessor world, string sourceForErrorLogging, IRecipeBase recipe)
    {
        if (deduplicationIndex == -1)
        {
            this.world = world;
            deduplicationIndex = AddToFastSearchRecipes(world, recipe);
        }
        else return true;   // Some recipes include the same ingredient in multiple positions, in GridRecipe.Resolve() this gets called for each position: the only difference will be the Id

        if (resolvedItemStack != null)
        {
            if (resolvedItemStack.Attributes is { } attr && attr.Count > 0) ResolvedAttributes = resolvedItemStack.Attributes;
            ResolvedItemStack = resolvedItemStack;          // Make sure the FastSearchCraftingRecipeIngredient.ResolvedItemStack is populated client-side - needed for some recipe matching
            return true;    // No need to re-resolve the ResolvedItemStack if it is already present (for example, client-side in FromBytes())
        }

        return Resolve(world, sourceForErrorLogging);
    }

    /// <summary>
    /// For INPUT slots you should normally call the overload Resolve(IWorldAccessor world, string sourceForErrorLogging, IRecipeBase recipe) otherwise you risk this ingredient/recipe being missed in the Handbook and in grid crafting.
    /// <br/>However, it's totally fine to use this overload for OUTPUT slots or for special situations (e.g. RightClickConstruction in the world)
    /// </summary>
    public virtual bool Resolve(IWorldAccessor world, string sourceForErrorLogging)
    {
        MatchingType = IRecipeIngredient.GetMatchType(Code?.ToString(), Name != null);
        
        if (ReturnedStack != null)
        {
            ReturnedStack.Resolve(world, $"{sourceForErrorLogging} recipe with output {Code}");
        }

        if (MatchingType != EnumRecipeMatchType.Exact)
        {
            return true;
        }

        if (MatchingType == EnumRecipeMatchType.Exact && (Code == null || Code == "*.*"))
        {
            world.Logger.Warning($"Failed resolving crafting recipe ingredient with unspecified code in {sourceForErrorLogging}");
            return false;
        }

        ItemStack resolvedItemStack;
        if (Type == EnumItemClass.Block)
        {
            Block? block = world.GetBlock(Code);
            if (block == null || block.IsMissing)
            {
                world.Logger.Warning($"Failed resolving crafting recipe ingredient with code {Code} in {sourceForErrorLogging}");
                return false;
            }

            resolvedItemStack = new ItemStack(block, Quantity);
        }
        else
        {
            Item? item = world.GetItem(Code);
            if (item == null || item.IsMissing)
            {
                world.Logger.Warning($"Failed resolving crafting recipe ingredient with code {Code} in {sourceForErrorLogging}");
                return false;
            }
            resolvedItemStack = new ItemStack(item, Quantity);
        }

        if (Attributes != null)
        {
            IAttribute? attributes = Attributes.ToAttribute();
            if (attributes is ITreeAttribute treeAttribute)
            {
                ResolvedAttributes = treeAttribute;
                if (deduplicationIndex < 0) resolvedItemStack.Attributes = treeAttribute;
            }
        }

        ResolvedItemStack = resolvedItemStack;

        return true;
    }

    private int AddToFastSearchRecipes(IWorldAccessor world, IRecipeBase recipe)
    {
        var dict = world.FastSearchRecipesByIngredient;
        var packed = new FastSearchCraftingRecipeIngredient(this);
        int index = dict.IndexOf(packed);
        if (index >= 0)
        {
            var existing = dict.GetAt(index);
            this.Code = existing.Key.Code;  // De-duplicate objects held on heap, as these fields must be the same if we found a matching index
            this.AllowedVariants = existing.Key.AllowedVariants;    
            this.SkipVariants = existing.Key.SkipVariants;
            existing.Value.AddIfNotPresent(recipe);   // Client-side a separate CraftingRecipeIngedient object can be deserialized several times per recipe, once for each unique position in the grid
            return index;
        }
        index = dict.Count;
        packed.MatchingType = IRecipeIngredient.GetMatchType(Code?.ToString(), Name != null);
        dict.Add(packed, [recipe]);
        return index;
    }

    public virtual bool SatisfiesAsIngredient(ItemStack inputStack, bool checkStackSize = true)
    {
        if (inputStack == null || inputStack.Collectible == null) return false;

        if (MatchingType != EnumRecipeMatchType.Exact)
        {
            if (Type != inputStack.Class) return false;
            if (Code != null && !WildcardUtil.Match(Code, inputStack.Collectible.Code, AllowedVariants)) return false;
            if (checkStackSize && inputStack.StackSize < Quantity) return false;
            if (SkipVariants != null && WildcardUtil.Match(Code, inputStack.Collectible.Code, SkipVariants)) return false;
            if (!CheckTags(inputStack, inputStack.Collectible)) return false;
        }
        else
        {
            var resolvedItemStack = ResolvedItemStack;
            if (resolvedItemStack == null) return false;
            if (!resolvedItemStack.Satisfies(inputStack)) return false;
            if (checkStackSize && inputStack.StackSize < Quantity) return false;
        }

        return true;
    }

    public virtual bool CheckTags(ItemStack inputStack, CollectibleObject collectible)
    {
        return CraftingRecipeIngredient.CheckTags(Tags, inputStack, collectible);
    }

    public static bool CheckTags(ComplexTagCondition<TagSet> tags, ItemStack inputStack, CollectibleObject collectible)
    {
        return tags.Matches(collectible.GetTags(inputStack));
    }

    public virtual RecipeIngredientConsumeProperties GetConsumeProperties()
    {
        if (!Consume)
        {
            return new()
            {
                Consume = false,
                DurabilityChange = DurabilityChange,
                DurabilityCost = -DurabilityChange,
                Quantity = Quantity,
                BreakOnZeroDurability = Break
            };
        }

        if (IsTool)
        {
            return new()
            {
                Consume = false,
                DurabilityChange = -ToolDurabilityCost,
                DurabilityCost = ToolDurabilityCost,
                Quantity = Quantity,
                BreakOnZeroDurability = Break
            };
        }

        return new()
        {
            Consume = true,
            DurabilityChange = 0,
            DurabilityCost = 0,
            Quantity = Quantity,
            BreakOnZeroDurability = Break
        };
    }

    public virtual CraftingRecipeIngredient Clone()
    {
        CraftingRecipeIngredient result = new();

        CloneTo(result);

        return result;
    }

    [Obsolete("Use 'Clone()' instead")]
    public TResult CloneTo<TResult>() where TResult : CraftingRecipeIngredient, new()
    {
        return (TResult)Clone();
    }

    public override string ToString()
    {
        // Unfortunately we don't have a registry reference here to resolve the tags.
        return MatchingType != EnumRecipeMatchType.TagsOnly ? $"{Type} code {Code}" : $"{Type} tagged {Tags}";
    }

    /// <summary>
    /// Replaces {<paramref name="key"/>} placeholders with <paramref name="value"/> in code an attributes
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public virtual void FillPlaceHolder(string key, string value)
    {
        if (Code != null)
        {
            Code = Code.CopyWithPath(Code.Path.Replace("{" + key + "}", value).DeDuplicate());
        }
        Attributes?.FillPlaceHolder(key, value);
        RecipeAttributes?.FillPlaceHolder(key, value);
    }

    public virtual void ToBytes(BinaryWriter writer)
    {
        writer.Write((int)MatchingType);
        writer.Write((int)Type);
        writer.Write(Code != null);
        if (Code != null)
        {
            writer.Write(Code.ToShortString());
        }
        writer.Write(Quantity);
        if (MatchingType == EnumRecipeMatchType.Exact)
        {
            var resolvedItemStack = this.resolvedItemStack;
            writer.Write(resolvedItemStack != null);
            resolvedItemStack?.ToBytes(writer);
        }

        writer.Write(Consume);
        writer.Write(DurabilityChange);

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
        }
        else
        {
            writer.Write(false);
        }

        writer.Write(Id ?? "");

        Tags.ToBytes(writer);
    }

    public virtual void FromBytes(BinaryReader reader, IWorldAccessor resolver)
    {
        MatchingType = (EnumRecipeMatchType)reader.ReadInt32();
        Type = (EnumItemClass)reader.ReadInt32();
        bool hasCode = reader.ReadBoolean();
        if (hasCode)
        {
            Code = new AssetLocation(reader.ReadString());
        }
        Quantity = reader.ReadInt32();
        if (MatchingType == EnumRecipeMatchType.Exact && reader.ReadBoolean())
        {
            resolvedItemStack = new ItemStack(reader, resolver);
        }

        Consume = reader.ReadBoolean();
        DurabilityChange = reader.ReadInt32();

        bool haveVariants = reader.ReadBoolean();
        if (haveVariants)
        {
            AllowedVariants = new string[reader.ReadInt32()];
            for (int i = 0; i < AllowedVariants.Length; i++)
            {
                AllowedVariants[i] = reader.ReadString().DeDuplicate();
            }
        }

        bool haveSkipVariants = reader.ReadBoolean();
        if (haveSkipVariants)
        {
            SkipVariants = new string[reader.ReadInt32()];
            for (int i = 0; i < SkipVariants.Length; i++)
            {
                SkipVariants[i] = reader.ReadString().DeDuplicate();
            }
        }

        bool haveConsumedStack = reader.ReadBoolean();
        if (haveConsumedStack)
        {
            JsonItemStack returnedStack = new();
            returnedStack.FromBytes(reader, resolver.ClassRegistry);
            returnedStack.ResolvedItemStack?.ResolveBlockOrItem(resolver);
            ReturnedStack = returnedStack;
        }

        if (reader.ReadBoolean())
        {
            RecipeAttributes = new JsonObject(JToken.Parse(reader.ReadString()));
        }

        Id = reader.ReadString().DeDuplicate();

        Tags = ComplexTagConditionExtensions.FromBytes(reader);
    }



    protected virtual void CloneTo(object cloneTo)
    {
        if (cloneTo is CraftingRecipeIngredient ingredient)
        {
            ingredient.Code = Code?.Clone();
            ingredient.Tags = Tags;
            ingredient.Type = Type;
            ingredient.Name = Name;
            ingredient.Quantity = Quantity;
            ingredient.IsTool = IsTool;
            ingredient.ToolDurabilityCost = ToolDurabilityCost;
            ingredient.MatchingType = MatchingType;
            ingredient.Consume = Consume;
            ingredient.DurabilityChange = DurabilityChange;
            ingredient.Break = Break;
            ingredient.AllowedVariants = AllowedVariants;   // These do not need a deep clone, they are never written to [except originally when deserializing or parsing JSON]
            ingredient.SkipVariants = SkipVariants;   // These do not need a deep clone, they are never written to [except originally when deserializing or parsing JSON]
            ingredient.resolvedItemStack = ResolvedItemStack?.Clone();
            ingredient.ReturnedStack = ReturnedStack?.Clone();
            ingredient.RecipeAttributes = RecipeAttributes?.Clone();
            ingredient.Id = Id;
            ingredient.Attributes = Attributes?.Clone();
            ingredient.RecipeAttributes = RecipeAttributes?.Clone();
            ingredient.world = world;
            ingredient.deduplicationIndex = deduplicationIndex;
            ingredient.ResolvedAttributes = ResolvedAttributes?.Clone();
        }
    }

    object ICloneable.Clone() => Clone();
}

/// <summary>
/// A simplified version of a CraftingRecipeIngredient, for faster searching and ingredient matching; note this object stores the actual ResolvedItemStack object (if any - there may be none for a Tags based ingredient)
/// </summary>
public class FastSearchCraftingRecipeIngredient : IRecipeIngredientBase, IEquatable<FastSearchCraftingRecipeIngredient>
{
    public EnumItemClass Type { get; set; }
    public AssetLocation? Code { get; set; }
    public ComplexTagCondition<TagSet> Tags { get; set; }
    public string[]? AllowedVariants { get; set; }
    public string[]? SkipVariants { get; set; }
    public EnumRecipeMatchType MatchingType { get; set; }

    public ItemStack? ResolvedItemStack { get; set; }

    public FastSearchCraftingRecipeIngredient(CraftingRecipeIngredient parent)
    {
        Type = parent.Type;
        Code = parent.Code;
        Tags = parent.Tags;
        AllowedVariants = parent.AllowedVariants;
        SkipVariants = parent.SkipVariants;
    }

    public override int GetHashCode()           // Important because these will be Key in an OrderedDictionary
    {
        return Code == null ? 0 : Code.Path.GetHashCode();
    }

    public override bool Equals(Object? obj)
    {
        return (obj is FastSearchCraftingRecipeIngredient other) && this.Equals(other);
    }

    public virtual bool Equals(FastSearchCraftingRecipeIngredient? other)
    {
        if (other == null) return false;
        if (!Tags.Equals(other.Tags)) return false;   // Test this first, because if hashcodes match then Code is likely to match; if Tags are not specified then this will return true quickly

        if (Code == null)
        {
            if (other.Code != null) return false;
        }
        else if (!Code.Equals(other.Code)) return false;
        
        if (!Type.Equals(other.Type)) return false;

        if (AllowedVariants == null)
        {
            if (other.AllowedVariants != null) return false;
        }
        else if (!AllowedVariants.DeepEquals(other.AllowedVariants)) return false;

        if (SkipVariants == null)
        {
            if (other.SkipVariants != null) return false;
        }
        else if (!SkipVariants.DeepEquals(other.SkipVariants)) return false;

        return true;
    }

    public bool SatisfiesAsIngredient(ItemStack inputStack, bool unused = default)
    {
        if (inputStack == null || inputStack.Collectible == null) return false;

        if (MatchingType != EnumRecipeMatchType.Exact)
        {
            if (Type != inputStack.Class) return false;
            if (Code != null && !WildcardUtil.Match(Code, inputStack.Collectible.Code, AllowedVariants)) return false;
            if (SkipVariants != null && WildcardUtil.Match(Code, inputStack.Collectible.Code, SkipVariants)) return false;
            if (!CraftingRecipeIngredient.CheckTags(Tags, inputStack, inputStack.Collectible)) return false;
        }
        else
        {
            var resolvedItemStack = ResolvedItemStack;
            if (resolvedItemStack == null) return false;
            if (!resolvedItemStack.Satisfies(inputStack)) return false;
        }

        return true;
    }
}
