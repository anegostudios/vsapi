using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProtoBuf;
using System;
using System.Collections.Generic;
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

    /// <summary>
    /// If array of tags specified (["tag-1", "tag-2", "not-tag-3"]):<br/>
    /// - matched item has to have all these tags, and not have tags with 'not-' prefix;<br/>
    /// - if 'ReverseTagsCheck: true' specified, matched item has to have at least one of these tags, or not contain any tag with 'not-' prefix.<br/>
    /// <br/>
    /// If array of tag groups specified ([["tag-1", "tag-2"], ["not-tag-3", "tag-4]]):<br/>
    /// - matched item has to have all tags (and not have 'not-' tags) from at least of tag groups;<br/>
    /// - if 'ReverseTagsCheck: true' specified, matched item has to have at least one of these tags, or not contain any tag with 'not-' prefix from each tag group.<br/>
    /// <br/>
    /// This tags check can be treated as logic expression where each tag means of matched item having this tag:<br/>
    /// - [["tag-1", "tag-2"], ["not-tag-3", "tag-4]] == (tag-1 AND tag-2) OR (NOT tag-3 AND tag-4)<br/>
    /// - if 'ReverseTagsCheck: true' specified: [["tag-1", "tag-2"], ["not-tag-3", "tag-4]] == (tag-1 OR tag-2) AND (NOT tag-3 OR tag-4)<br/>
    /// </summary>
    [DocumentAsJson("Optional", "All")]
    public GeneralTagGroups? Tags { get; set; }

    /// <summary>
    /// When used with [] tags: matched item should contain at least on tag from the list
    /// When used with [[]] tags: matched item should contain at least one tag from each group
    /// </summary>
    [DocumentAsJson("Optional", "None")]
    public bool? ReverseTagsCheck { get; set; }

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
    /// <summary>
    /// Defines how <see cref="Code"/> will be used to match
    /// </summary>
    public EnumRecipeMatchType MatchingType { get; set; } = EnumRecipeMatchType.Exact;

    /// <summary>
    /// The itemstack made from Code, Quantity and Attributes, populated by the engine
    /// </summary>
    public ItemStack? ResolvedItemStack { get; set; }

    public RecipeIngredientConsumeProperties ConsumeProperties => GetConsumeProperties();

    [Obsolete("Use MatchingType")]
    public bool IsWildCard
    {
        get => MatchingType != EnumRecipeMatchType.Exact;
        set => MatchingType = value ? MatchingType : EnumRecipeMatchType.Exact;
    }
    #endregion

    #region IRecipeIngredient
    bool IRecipeIngredient.MatchTags(TagSet tags) => Tags?.Check(tags) == true;

    void IRecipeIngredient.ResolveTags(IWorldAccessor world) => Tags?.Resolve(world);

    IEnumerable<TagCondition<TagSet>>? IRecipeIngredient.ResolvedTags => Tags?.GetResolvedTags();
    #endregion



    public virtual bool Resolve(IWorldAccessor world, string sourceForErrorLogging)
    {
        Tags?.Resolve(world);
        if (ReverseTagsCheck != null && Tags != null)
        {
            Tags.ReverseCheck = ReverseTagsCheck.Value;
        }

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

        if (Type == EnumItemClass.Block)
        {
            Block? block = world.GetBlock(Code);
            if (block == null || block.IsMissing)
            {
                world.Logger.Warning($"Failed resolving crafting recipe ingredient with code {Code} in {sourceForErrorLogging}");
                return false;
            }

            ResolvedItemStack = new ItemStack(block, Quantity);
        }
        else
        {
            Item? item = world.GetItem(Code);
            if (item == null || item.IsMissing)
            {
                world.Logger.Warning($"Failed resolving crafting recipe ingredient with code {Code} in {sourceForErrorLogging}");
                return false;
            }
            ResolvedItemStack = new ItemStack(item, Quantity);
        }

        if (Attributes != null)
        {
            IAttribute? attributes = Attributes.ToAttribute();
            if (attributes is ITreeAttribute treeAttribute)
            {
                ResolvedItemStack.Attributes = treeAttribute;
            }
        }

        return true;
    }

    public virtual bool SatisfiesAsIngredient(ItemStack inputStack, bool checkStackSize = true)
    {
        if (inputStack == null || inputStack.Collectible == null) return false;

        if (MatchingType != EnumRecipeMatchType.Exact)
        {
            if (Type != inputStack.Class) return false;
            if (!CheckTags(inputStack, inputStack.Collectible)) return false;
            if (checkStackSize && inputStack.StackSize < Quantity) return false;
            if (Code != null && !WildcardUtil.Match(Code, inputStack.Collectible.Code, AllowedVariants)) return false;
            if (SkipVariants != null && WildcardUtil.Match(Code, inputStack.Collectible.Code, SkipVariants)) return false;
        }
        else
        {
            if (ResolvedItemStack == null) return false;
            if (!ResolvedItemStack.Satisfies(inputStack)) return false;
            if (checkStackSize && inputStack.StackSize < ResolvedItemStack.StackSize) return false;
        }

        return true;
    }

    public virtual bool CheckTags(ItemStack inputStack, CollectibleObject collectible)
    {
        if (Tags == null)
        {
            return true;
        }

        return Tags.Check(collectible.GetTags(inputStack));
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
        return Type + " code " + Code;
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
            writer.Write(ResolvedItemStack != null);
            ResolvedItemStack?.ToBytes(writer);
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

        if (ReverseTagsCheck != null && Tags != null)
        {
            Tags.ReverseCheck = ReverseTagsCheck.Value;
        }
        writer.Write(Tags != null);
        Tags?.ToBytes(writer);
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
            ResolvedItemStack = new ItemStack(reader, resolver);
        }

        Consume = reader.ReadBoolean();
        DurabilityChange = reader.ReadInt32();

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

        bool hasTags = reader.ReadBoolean();
        if (hasTags)
        {
            Tags = new GeneralTagGroups();
            Tags.FromBytes(reader, resolver);
            Tags.Resolve(resolver);
        }
    }



    protected virtual void CloneTo(object cloneTo)
    {
        if (cloneTo is CraftingRecipeIngredient ingredient)
        {
            ingredient.Code = Code?.Clone();
            ingredient.Tags = Tags?.Clone();
            ingredient.ReverseTagsCheck = ReverseTagsCheck;
            ingredient.Type = Type;
            ingredient.Name = Name;
            ingredient.Quantity = Quantity;
            ingredient.IsTool = IsTool;
            ingredient.ToolDurabilityCost = ToolDurabilityCost;
            ingredient.MatchingType = MatchingType;
            ingredient.Consume = Consume;
            ingredient.DurabilityChange = DurabilityChange;
            ingredient.Break = Break;
            ingredient.AllowedVariants = AllowedVariants == null ? null : (string[])AllowedVariants.Clone();
            ingredient.SkipVariants = SkipVariants == null ? null : (string[])SkipVariants.Clone();
            ingredient.ResolvedItemStack = ResolvedItemStack?.Clone();
            ingredient.ReturnedStack = ReturnedStack?.Clone();
            ingredient.RecipeAttributes = RecipeAttributes?.Clone();
            ingredient.Id = Id;
            ingredient.Attributes = Attributes?.Clone();
            ingredient.RecipeAttributes = RecipeAttributes?.Clone();
        }
    }



    object ICloneable.Clone() => Clone();
}

