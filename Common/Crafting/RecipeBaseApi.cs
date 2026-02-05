using System;
using System.Collections.Generic;
using Vintagestory.API.Datastructures;

namespace Vintagestory.API.Common;

public enum EnumRecipeMatchType
{
    Exact,
    Wildcard,
    NamedWildcard,
    AdvancedWildcard,
    Regex,
    TagsOnly
}

public struct RecipeIngredientConsumeProperties
{
    /// <summary>
    /// Should ingredient be consumed on crafting.<br/>
    /// If set to true <see cref="Quantity"/> will be removed from ItemStack.
    /// Other wise <see cref="DurabilityChange"/> will be applied to item, if it is not equal to 0.
    /// </summary>
    public bool Consume { get; set; }
    /// <summary>
    /// How ItemStack size will be reduced.
    /// </summary>
    public int Quantity { get; set; }
    /// <summary>
    /// Durability change on being used for crafting.<br/>
    /// Positive number will result in durability increase. Negative in decrease.
    /// </summary>
    public int DurabilityChange { get; set; }
    /// <summary>
    /// Equal to -DurabilityChange. Durability of item will be reduced by this amount if it is not consumed
    /// </summary>
    public int DurabilityCost { get; set; }
    /// <summary>
    /// If set to False item will not break when it durability reaches zero.
    /// </summary>
    public bool BreakOnZeroDurability { get; set; }
}

public interface IRecipeIngredient : IByteSerializable, ICloneable
{
    AssetLocation? Code { get; set; }

    EnumItemClass Type { get; set; }

    int Quantity { get; set; }

    /// <summary>
    /// Used to reference ingredients, auto assigned if not specified
    /// </summary>
    string Id { get; set; }

    /// <summary>
    /// Attaches a name to a wildcard in an ingredient. This is used to substitute the value into the output. Only required if using a wildcard.
    /// </summary>
    string? Name { get; set; }

    EnumRecipeMatchType MatchingType { get; set; }

    string[]? AllowedVariants { get; set; }

    string[]? SkipVariants { get; set; }

    JsonItemStack? ReturnedStack { get; set; }

    JsonObject? RecipeAttributes { get; set; }

    ItemStack? ResolvedItemStack { get; set; }

    void ResolveTags(IWorldAccessor world);

    bool MatchTags(TagSet tags);

    IEnumerable<TagCondition<TagSet>>? ResolvedTags { get; }

    RecipeIngredientConsumeProperties ConsumeProperties { get; }

    bool SatisfiesAsIngredient(ItemStack inputStack, bool checkStackSize = true);

    void FillPlaceHolder(string key, string value);

    bool Resolve(IWorldAccessor world, string sourceForErrorLogging);
}

public interface IRecipeOutput : IByteSerializable, ICloneable
{
    ItemStack? ResolvedItemStack { get; }

    void FillPlaceHolder(string key, string value);

    bool Resolve(IWorldAccessor world, string sourceForErrorLogging);
}

public interface IRecipeBase : IByteSerializable, ICloneable
{
    int RecipeId { get; set; }

    AssetLocation? Name { get; set; }

    bool Enabled { get; set; }

    bool AverageDurability { get; set; }

    string? RequiresTrait { get; set; }

    bool ShowInCreatedBy { get; set; }

    IEnumerable<IRecipeIngredient> RecipeIngredients { get; }

    IRecipeOutput RecipeOutput { get; }

    bool Resolve(IWorldAccessor world, string sourceForErrorLogging);

    void OnParsed(IWorldAccessor world);

    IEnumerable<IRecipeBase> GenerateRecipesForAllIngredientCombinations(IWorldAccessor world);

    IRecipeBase CloneAsInterface() => (IRecipeBase)Clone();
}

public interface IConcreteCloneable<out TSelf> : ICloneable
{
    new TSelf Clone();
}
