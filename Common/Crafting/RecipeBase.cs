using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;

namespace Vintagestory.API.Common;

/// <summary>
/// Creates a new base recipe type. Almost all recipe types extend from this.
/// </summary>
/// <typeparam name="TRecipe">The resulting recipe type.</typeparam>
[DocumentAsJson]
public abstract class RecipeBase : IRecipeBase, IConcreteCloneable<RecipeBase>
{
    #region From JSON
    /// <summary>
    /// If set to false, the recipe will never be loaded.
    /// If loaded, you can use this field to disable recipes during runtime.
    /// </summary>
    [DocumentAsJson("Optional", "True")]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Adds a name to this. Used for logging, and determining helve hammer workability for smithing recipes. Recipes for repairing objects must contain 'repair' in the name.
    /// </summary>
    [DocumentAsJson("Optional", "Asset Path")]
    public AssetLocation? Name { get; set; }

    /// <summary>
    /// Optional attribute data that you can attach any data to. Useful for code mods, but also required when using liquid ingredients.<br/>
    /// See dough.json grid recipe file for example.
    /// </summary>
    [JsonConverter(typeof(JsonAttributesConverter))]
    [DocumentAsJson("Optional", "None")]
    public JsonObject? Attributes { get; set; }

    /// <summary>
    /// If set, only players with given trait can use this  See config/traits.json for a list of traits.
    /// </summary>
    [DocumentAsJson("Optional", "None")]
    public string? RequiresTrait { get; set; }

    /// <summary>
    /// If true, the output item will have its durability averaged over the input items
    /// </summary>
    [DocumentAsJson("Optional", "True")]
    public bool AverageDurability { get; set; } = true;

    /// <summary>
    /// If set, it will copy over the itemstack attributes from given ingredient code
    /// </summary>
    [DocumentAsJson("Optional", "None")]
    public string? CopyAttributesFrom { get; set; } = null;

    /// <summary>
    /// Attributes from these ingredients will be merged into output itemstack
    /// </summary>
    [DocumentAsJson("Optional", "None")]
    public string[] MergeAttributesFrom { get; set; } = [];

    /// <summary>
    /// If '{code}' is used in ingredient code, allowed variants for this code should be specified in this map
    /// </summary>
    [DocumentAsJson("Optional", "None")]
    public Dictionary<string, string[]> AllowedVariants { get; set; } = [];

    /// <summary>
    /// If '{code}' is used in ingredient code, skip variants for this code should be specified in this map
    /// </summary>
    [DocumentAsJson("Optional", "None")]
    public Dictionary<string, string[]> SkipVariants { get; set; } = [];

    /// <summary>
    /// Used by the handbook. If false, will not appear in the "Created by" section
    /// </summary>
    [DocumentAsJson("Optional", "True")]
    public bool ShowInCreatedBy { get; set; } = true;
    #endregion

    /// <summary>
    /// The ID of the  Automatically generated when the recipe is loaded.
    /// </summary>

    public int RecipeId { get; set; }

    public static readonly CollectiblePreSearchResultsCache CollectiblePreSearchResultsCache = new();

    public abstract IEnumerable<IRecipeIngredient> RecipeIngredients { get; }

    public abstract IRecipeOutput RecipeOutput { get; }



    public abstract bool Resolve(IWorldAccessor world, string sourceForErrorLogging);

    public abstract RecipeBase Clone();

    object ICloneable.Clone() => Clone();

    public virtual void ToBytes(BinaryWriter writer)
    {
        writer.Write(Name?.ToShortString() ?? "");
        writer.Write(ShowInCreatedBy);
        writer.Write(AverageDurability);

        writer.Write(Attributes == null);
        if (Attributes?.Token != null)
        {
            writer.Write(Attributes.Token.ToString());
        }

        writer.Write(RequiresTrait != null);
        if (RequiresTrait != null)
        {
            writer.Write(RequiresTrait);
        }

        writer.Write(CopyAttributesFrom != null);
        if (CopyAttributesFrom != null)
        {
            writer.Write(CopyAttributesFrom);
        }

        writer.Write(AllowedVariants.Count);
        foreach ((string key, string[] value) in AllowedVariants)
        {
            writer.Write(key);
            writer.Write(value.Length);
            foreach (string variant in value)
            {
                writer.Write(variant);
            }
        }

        writer.Write(SkipVariants.Count);
        foreach ((string key, string[] value) in SkipVariants)
        {
            writer.Write(key);
            writer.Write(value.Length);
            foreach (string variant in value)
            {
                writer.Write(variant);
            }
        }

        writer.Write(MergeAttributesFrom.Length);
        foreach (string code in MergeAttributesFrom)
        {
            writer.Write(code);
        }
    }

    public virtual void FromBytes(BinaryReader reader, IWorldAccessor resolver)
    {
        Name = new AssetLocation(reader.ReadString());
        ShowInCreatedBy = reader.ReadBoolean();
        AverageDurability = reader.ReadBoolean();

        if (!reader.ReadBoolean())
        {
            string json = reader.ReadString();
            Attributes = new JsonObject(JToken.Parse(json));
        }

        if (reader.ReadBoolean())
        {
            RequiresTrait = reader.ReadString().DeDuplicate();
        }

        if (reader.ReadBoolean())
        {
            CopyAttributesFrom = reader.ReadString().DeDuplicate();
        }

        int allowedVariantsCount = reader.ReadInt32();
        AllowedVariants = [];
        for (int i = 0; i < allowedVariantsCount; i++)
        {
            string key = reader.ReadString().DeDuplicate();
            int len = reader.ReadInt32();
            string[] variants = new string[len];
            for (int j = 0; j < len; j++)
            {
                variants[j] = reader.ReadString().DeDuplicate();
            }
            AllowedVariants[key] = variants;
        }

        int skipVariantsCount = reader.ReadInt32();
        SkipVariants = [];
        for (int i = 0; i < skipVariantsCount; i++)
        {
            string key = reader.ReadString().DeDuplicate();
            int len = reader.ReadInt32();
            string[] variants = new string[len];
            for (int j = 0; j < len; j++)
            {
                variants[j] = reader.ReadString().DeDuplicate();
            }
            SkipVariants[key] = variants;
        }

        int mergedAttributesCount = reader.ReadInt32();
        MergeAttributesFrom = new string[mergedAttributesCount];
        for (int i = 0; i < mergedAttributesCount; i++)
        {
            MergeAttributesFrom[i] = reader.ReadString().DeDuplicate();
        }
    }

    public virtual void OnParsed(IWorldAccessor world)
    {

    }

    public virtual IEnumerable<IRecipeBase> GenerateRecipesForAllIngredientCombinations(IWorldAccessor world)
    {
        Dictionary<string, HashSet<string>> nameToCodeMapping = GetNameToCodeMapping(world);

        if (nameToCodeMapping.Count <= 0)
        {
            return [this];
        }

        List<RecipeBase> subRecipes = [];

        int variantsCombinations = 1;
        foreach ((_, HashSet<string> mapping) in nameToCodeMapping)
        {
            variantsCombinations *= mapping.Count;
        }

        bool first = true;
        int variantCodeIndexDivider = 1;
        foreach ((string variantCode, HashSet<string> variantsSet) in nameToCodeMapping)
        {
            if (variantsSet.Count == 0) continue;

            string[] variants = variantsSet.ToArray();

            for (int i = 0; i < variantsCombinations; i++)
            {
                RecipeBase currentRecipe;
                string currentVariant = variants[i / variantCodeIndexDivider % variants.Length];

                if (first)
                {
                    currentRecipe = Clone();
                    subRecipes.Add(currentRecipe);
                }
                else
                {
                    currentRecipe = subRecipes[i];
                }

                foreach (IRecipeIngredient ingredient in currentRecipe.RecipeIngredients)
                {
                    FillIngredientPlaceHolders(ingredient, variantCode, currentVariant);
                    ingredient.SkipVariants = null;    // These can now be nullified, because we have expanded the wildcard successfully
                    ingredient.AllowedVariants = null;
                }

                currentRecipe.RecipeOutput.FillPlaceHolder(variantCode, currentVariant);
            }
            variantCodeIndexDivider *= variants.Length;
            first = false;
        }

        if (RecipeIngredients.Any(ingredient => ingredient.MatchingType == EnumRecipeMatchType.TagsOnly))
        {
            return subRecipes.SelectMany(recipe => GenerateRecipesForTagOnlyIngredients(world, recipe));
        }

        return subRecipes;
    }

    public virtual void GenerateOutputStack(ItemSlot[] inputSlots, ItemSlot outputSlot)
    {
        if (RecipeOutput.ResolvedItemStack == null)
        {
            throw new InvalidOperationException($"Missing or errored output result for recipe '{Name}'");
        }

        outputSlot.Itemstack = RecipeOutput.ResolvedItemStack.Clone();
        ItemStack outputStack = outputSlot.Itemstack;

        if (CopyAttributesFrom != null)
        {
            ItemStack? inputStack = GetInputStackForIngredientId(CopyAttributesFrom, inputSlots);
            if (inputStack != null)
            {
                ITreeAttribute attr = inputStack.Attributes.Clone();
                attr.MergeTree(outputStack.Attributes);
                outputStack.Attributes = attr;
            }
        }

        if (MergeAttributesFrom.Length > 0)
        {
            foreach (string ingredientId in MergeAttributesFrom)
            {
                ItemStack? inputStack = GetInputStackForIngredientId(ingredientId, inputSlots);
                if (inputStack != null)
                {
                    ITreeAttribute attr = inputStack.Attributes.Clone();
                    attr.MergeTree(outputStack.Attributes);
                    outputStack.Attributes = attr;
                }
            }
        }

        outputSlot.Itemstack.Collectible.OnCreatedByCrafting(inputSlots, outputSlot, this);
    }

    public virtual TGridCellElement? GetElementInGrid<TGridCellElement>(int row, int column, TGridCellElement[] cells, int gridWidth)
    {
        int gridHeight = cells.Length / gridWidth;
        if (row < 0 || column < 0 || row >= gridHeight || column >= gridWidth)
        {
            return default;
        }

        return cells[row * gridWidth + column];
    }

    public virtual int GetGridIndex<TGridCellElement>(int row, int col, TGridCellElement[] cells, int gridWidth)
    {
        int gridHeight = cells.Length / gridWidth;
        if (row < 0 || col < 0 || row >= gridHeight || col >= gridWidth) return -1;

        return row * gridWidth + col;
    }



    static public string ReplaceVariantsToRegex(string value, out List<string> variants, Dictionary<string, string>? allowedVariants = null)
    {
        variants = [];

        string result = value;

        while (result.Contains('{') && result.Contains('}'))
        {
            int start = result.IndexOf('{');
            int end = result.IndexOf('}');
            string variant = result.Substring(start + 1, end - start - 1);
            if (allowedVariants?.TryGetValue(variant, out string? variantValue) == true)
            {
                result = result.Replace("{" + variant + "}", variantValue);
            }
            else
            {
                result = result.Replace("{" + variant + "}", "@");
            }
            variants.Add(variant);
        }

        result = WildCardToRegex(result);

        result = result.Replace("@", "([\\w-]+)");

        return "^" + result + "$";
    }

    static public string WildCardToRegex(string value) => Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*");



    protected virtual void CloneTo(object recipe)
    {
        if (recipe is not RecipeBase recipeBase)
        {
            throw new ArgumentException("CloneTo should take object of same class or it subclass");
        }

        recipeBase.Enabled = Enabled;
        recipeBase.Name = Name;
        recipeBase.Attributes = Attributes?.Clone();
        recipeBase.RequiresTrait = RequiresTrait;
        recipeBase.AverageDurability = AverageDurability;
        recipeBase.CopyAttributesFrom = CopyAttributesFrom;
        recipeBase.MergeAttributesFrom = MergeAttributesFrom; // should be immutable, no need to make deep copy
        recipeBase.AllowedVariants = AllowedVariants; // should be immutable, no need to make deep copy
        recipeBase.SkipVariants = SkipVariants; // should be immutable, no need to make deep copy
        recipeBase.ShowInCreatedBy = ShowInCreatedBy;
    }

    public virtual IRecipeIngredient? GetIngredientById(string id)
    {
        return RecipeIngredients.FirstOrDefault(ingredient => ingredient.Id == id);
    }

    protected virtual Dictionary<string, HashSet<string>> GetNameToCodeMapping(IWorldAccessor world)
    {
        Dictionary<string, HashSet<string>> mappings = [];

        foreach (IRecipeIngredient ingredient in RecipeIngredients)
        {
            ingredient.MatchingType = IRecipeIngredient.GetMatchType(ingredient.Code?.ToString(), ingredient.Name != null);

            switch (ingredient.MatchingType)
            {
                case EnumRecipeMatchType.NamedWildcard:
                    GetNameToCodeMappingForBasicWildcard(world, ingredient, mappings);
                    break;
                case EnumRecipeMatchType.AdvancedWildcard:
                    GetNameToCodeMappingForAdvancedWildcard(world, ingredient, mappings);
                    break;
                case EnumRecipeMatchType.Exact:
                case EnumRecipeMatchType.Wildcard:
                case EnumRecipeMatchType.Regex:
                case EnumRecipeMatchType.TagsOnly:
                default:
                    break;
            }
        }

        return mappings;
    }

    protected virtual ItemStack? GetInputStackForIngredientId(string id, ItemSlot[] inputSlots)
    {
        IRecipeIngredient? ingredient = GetIngredientById(id);
        if (ingredient == null)
        {
            return null;
        }

        foreach (ItemSlot slot in inputSlots)
        {
            if (slot.Empty)
            {
                continue;
            }

            ItemStack? inputStack = slot.Itemstack;
            if (inputStack == null)
            {
                continue;
            }

            if (ingredient.SatisfiesAsIngredient(inputStack) && inputStack.Collectible.MatchesForCrafting(inputStack, this, ingredient))
            {
                return inputStack;
            }
        }

        return null;
    }

    protected virtual void GetNameToCodeMappingForBasicWildcard(IWorldAccessor world, IRecipeIngredient ingredient, Dictionary<string, HashSet<string>> mappings)
    {
        List<CollectibleObject> collectibles = CollectiblePreSearchResultsCache.GetOrCreate(world, ingredient.Code, ingredient.Type);

        if (collectibles.Count == 0)
        {
            world.Logger.Warning("Crafting recipe {0} {1} does not resolve to any collectible. Grid recipe {2} will never match.", ingredient.Type, ingredient.Code, this.Name);
        }

        HashSet<string> codes = [];
        int wildcardStartLen = ingredient.Code.Path.IndexOf('*');
        int wildcardEndLen = ingredient.Code.Path.Length - wildcardStartLen - 1;

        bool hasTagsFilter = !ingredient.Tags.IsEmpty;

        foreach (CollectibleObject collectible in collectibles)
        {
            var code = collectible.Code;
            if (code == null) continue;

            if (ingredient.SkipVariants != null && WildcardUtil.MatchesVariants(ingredient.Code, code, ingredient.SkipVariants))
            {
                continue;
            }

            if (!WildcardUtil.Match(ingredient.Code, code, ingredient.AllowedVariants))
            {
                continue;
            }

            if (hasTagsFilter && !ingredient.Tags.Matches(in collectible.Tags))
            {
                continue;
            }

            string path = code.Path[wildcardStartLen..];  // abc-[*-def]
            string codePart = path[..^wildcardEndLen].DeDuplicate(); // [*]-def
            codes.Add(codePart);
        }

        ingredient.Name ??= ingredient.Code;
        mappings[ingredient.Name] = codes;
    }

    protected virtual void GetNameToCodeMappingForAdvancedWildcard(IWorldAccessor world, IRecipeIngredient ingredient, Dictionary<string, HashSet<string>> mappings)
    {
        IEnumerable<CollectibleObject> collectibles = CollectiblePreSearchResultsCache.GetOrCreate(world, ingredient.Code, ingredient.Type);

        List<Dictionary<string, string>> allowedVariantsCombinations = GenerateAllowedVariantsCombinations(AllowedVariants);

        if (allowedVariantsCombinations.Count > 0)
        {
            foreach (Dictionary<string, string> combination in allowedVariantsCombinations)
            {
                Dictionary<string, HashSet<string>> variantValues = GetVariantValuesFromAdvancedWildcard(ingredient, collectibles, combination);

                AddVariantValuesToMapping(mappings, variantValues);
            }
        }
        else
        {
            Dictionary<string, HashSet<string>> variantValues = GetVariantValuesFromAdvancedWildcard(ingredient, collectibles, []);

            AddVariantValuesToMapping(mappings, variantValues);
        }
    }

    protected virtual Dictionary<string, HashSet<string>> GetVariantValuesFromAdvancedWildcard(IRecipeIngredient ingredient, IEnumerable<CollectibleObject> collectibles, Dictionary<string, string> allowedVariantsCombination)
    {
        Dictionary<string, HashSet<string>> variantValues = [];
        string regexTemplate = ReplaceVariantsToRegex(ingredient.Code.Path, out List<string> variants, allowedVariantsCombination);
        Regex regex = new(regexTemplate);

        bool hasTagsFilter = !ingredient.Tags.IsEmpty;

        foreach (CollectibleObject collectible in collectibles.Where(collectible => WildcardUtil.Match(ingredient.Code.Domain, collectible.Code.Domain)))
        {
            Match match = regex.Match(collectible.Code.Path);

            if (!match.Success)
            {
                continue;
            }

            if (hasTagsFilter && !ingredient.Tags.Matches(in collectible.Tags))
            {
                continue;
            }

            AddVariantCombination(variantValues, allowedVariantsCombination);

            CollectVariantValues(match, variants, variantValues);
        }

        return variantValues;
    }

    protected virtual void AddVariantCombination(Dictionary<string, HashSet<string>> variantValues, Dictionary<string, string> combination)
    {
        foreach ((string variant, string variantValue) in combination)
        {
            if (!variantValues.ContainsKey(variant))
            {
                variantValues[variant] = [];
            }
            variantValues[variant].Add(variantValue);
        }
    }

    protected virtual List<Dictionary<string, string>> GenerateAllowedVariantsCombinations(Dictionary<string, string[]> allowedVariants)
    {
        if (allowedVariants.Count == 0)
        {
            return [];
        }
        string[] variants = allowedVariants.Keys.ToArray();
        List<Dictionary<string, string>> combinations = [];
        GenerateAllowedVariantsCombinationsRecursive(0, variants, [], allowedVariants, combinations);
        return combinations;
    }

    protected virtual void GenerateAllowedVariantsCombinationsRecursive(int index, string[] variants, IEnumerable<(string variant, string value)> combination, Dictionary<string, string[]> allowedVariants, List<Dictionary<string, string>> combinations)
    {
        if (index >= variants.Length)
        {
            combinations.Add(combination.ToDictionary(entry => entry.variant, entry => entry.value));
            return;
        }

        string currentVariant = variants[index];

        foreach (string variantValue in allowedVariants[currentVariant])
        {
            IEnumerable<(string, string)> newCombination = combination.Append((currentVariant, variantValue));
            GenerateAllowedVariantsCombinationsRecursive(index + 1, variants, newCombination, allowedVariants, combinations);
        }
    }

    protected virtual void CollectVariantValues(Match match, List<string> variants, Dictionary<string, HashSet<string>> variantCodes)
    {
        for (int variantIndex = 0; variantIndex < variants.Count; variantIndex++)
        {
            int groupIndex = variantIndex + 1;

            if (groupIndex >= match.Groups.Count)
            {
                break;
            }

            string variant = match.Groups[variantIndex + 1].Value;
            string variantCode = variants[variantIndex];

            if (AllowedVariants.ContainsKey(variantCode) && !AllowedVariants[variantCode].Contains(variant))
            {
                continue;
            }

            if (SkipVariants.ContainsKey(variantCode) && SkipVariants[variantCode].Contains(variant))
            {
                continue;
            }

            if (!variantCodes.ContainsKey(variantCode))
            {
                variantCodes[variantCode] = [];
            }
            variantCodes[variantCode].Add(variant);
        }
    }

    protected virtual void AddVariantValuesToMapping(Dictionary<string, HashSet<string>> mappings, Dictionary<string, HashSet<string>> variantValues)
    {
        foreach ((string variantCode, HashSet<string> variantsList) in variantValues)
        {
            if (mappings.TryGetValue(variantCode, out HashSet<string>? mapping))
            {
                foreach (string variant in variantsList)
                {
                    mapping.Add(variant);
                }
            }
            else
            {
                mappings[variantCode] = variantsList;
            }
        }
    }

    protected virtual void FillIngredientPlaceHolders(IRecipeIngredient ingredient, string variantCode, string variantValue)
    {
        switch (ingredient.MatchingType)
        {
            case EnumRecipeMatchType.NamedWildcard:
                if (ingredient.Name == variantCode)
                {
                    ingredient.FillPlaceHolder(variantCode, variantValue);
                    ingredient.Code.Path = ingredient.Code.Path.Replace("*", variantValue);
                    ingredient.MatchingType = IRecipeIngredient.GetMatchType(ingredient.Code.ToString(), false);
                }
                break;
            case EnumRecipeMatchType.AdvancedWildcard:
                ingredient.FillPlaceHolder(variantCode, variantValue);
                ingredient.MatchingType = IRecipeIngredient.GetMatchType(ingredient.Code.ToString(), false);
                break;
            case EnumRecipeMatchType.Exact:
            case EnumRecipeMatchType.Wildcard:
            case EnumRecipeMatchType.Regex:
            case EnumRecipeMatchType.TagsOnly:
            default:
                break;
        }

        if (ingredient.ReturnedStack?.Code != null)
        {
            ingredient.ReturnedStack.Code.Path = ingredient.ReturnedStack.Code.Path.Replace("{" + variantCode + "}", variantValue);
        }
    }

    protected virtual bool MatchesAtPosition(ItemSlot[] inputSlots, IRecipeIngredient?[] ingredients)
    {
        int width = inputSlots.Length;
        return MatchesAtPosition(0, 0, inputSlots, width, width, ingredients);
    }

    protected virtual bool MatchesAtPosition(int colStart, int rowStart, ItemSlot[] inputSlots, int gridWidth, int recipeWidth, IRecipeIngredient?[] ingredients)
    {
        int gridHeight = inputSlots.Length / gridWidth;

        for (int col = 0; col < gridWidth; col++)
        {
            for (int row = 0; row < gridHeight; row++)
            {
                ItemStack? inputStack = GetElementInGrid(row, col, inputSlots, gridWidth)?.Itemstack;
                IRecipeIngredient? ingredient = GetElementInGrid(row - rowStart, col - colStart, ingredients, recipeWidth);

                if (!MatchStackToIngredient(inputStack, ingredient))
                {
                    return false;
                }
            }
        }

        return true;
    }

    protected virtual bool MatchStackToIngredient(ItemStack? inputStack, IRecipeIngredient? ingredient)
    {
        if (inputStack != null && ingredient != null)
        {
            if (!ingredient.SatisfiesAsIngredient(inputStack))
            {
                return false;
            }

            if (!inputStack.Collectible.MatchesForCrafting(inputStack, this, ingredient))
            {
                return false;
            }
        }
        else if (inputStack != null || ingredient != null)
        {
            return false;
        }

        return true;
    }

    protected virtual bool ConsumeInputShapeLess(IPlayer byPlayer, ItemSlot[] inputSlots, IRecipeIngredient?[] ingredients)
    {
        List<IRecipeIngredient> exactMatchIngredients = [];
        List<IRecipeIngredient> wildcardAndToolIngredients = [];

        // This can modify the stacksize of each ingredient's resolvedItemStack within exactMatchIngredients  (e.g. sets it to stacksize 2 if there are two ingredients the same in the recipe grid)
        SeparateAndMergeIngredients(ingredients, exactMatchIngredients: exactMatchIngredients, wildcardAndToolIngredients: wildcardAndToolIngredients);

        foreach ((ItemStack? inputStack, ItemSlot inputSlot) in inputSlots.Select(slot => (slot.Itemstack, slot)))
        {
            if (inputStack == null) continue;

            ConsumeExactMatchIngredient(inputStack, inputSlot, inputSlots, exactMatchIngredients, byPlayer);

            ConsumeWildcardMatchIngredient(inputStack, inputSlot, inputSlots, wildcardAndToolIngredients, byPlayer);
        }

        return exactMatchIngredients.Count == 0;
    }

    protected virtual bool ConsumeInputAt(IPlayer byPlayer, ItemSlot[] inputSlots, IRecipeIngredient?[] ingredients)
    {
        int width = inputSlots.Length;
        return ConsumeInputAt(byPlayer, inputSlots, width, width, 0, 0, ingredients);
    }

    protected virtual bool ConsumeInputAt(IPlayer byPlayer, ItemSlot[] inputSlots, int gridWidth, int recipeWidth, int colStart, int rowStart, IRecipeIngredient?[] ingredients)
    {
        int gridHeight = inputSlots.Length / gridWidth;

        for (int col = 0; col < gridWidth; col++)
        {
            for (int row = 0; row < gridHeight; row++)
            {
                ItemSlot? slot = GetElementInGrid(row, col, inputSlots, gridWidth);
                IRecipeIngredient? ingredient = GetElementInGrid(row - rowStart, col - colStart, ingredients, recipeWidth);

                if (ingredient == null) continue;
                if (ingredient.ResolvedItemStack == null && ingredient.MatchingType == EnumRecipeMatchType.Exact) continue;
                if (slot?.Itemstack == null) return false;

                int quantity = ingredient.MatchingType != EnumRecipeMatchType.Exact ? ingredient.Quantity : (ingredient.ResolvedItemStack?.StackSize ?? 0);

                slot.Itemstack.Collectible.OnConsumedByCrafting(inputSlots, slot, this, ingredient, byPlayer, quantity);
            }
        }

        return true;
    }

    protected virtual bool MatchesShapeLess(ItemSlot[] suppliedSlots, IWorldAccessor world, IRecipeIngredient?[] ingredients)
    {
        List<ItemStack> suppliedStacks = [];

        MergeStacks(suppliedSlots, suppliedStacks);

        if (!MatchWildcardIngredients(suppliedStacks, ingredients))
        {
            return false;
        }

        List<(ItemStack stack, IRecipeIngredient ingredient)> ingredientStacks = [];
        MergeIngredientStacks(suppliedSlots, ingredientStacks, ingredients, world);

        if (ingredientStacks.Count != suppliedStacks.Count) return false;

        return MatchIngredientStacks(ingredientStacks, suppliedStacks);
    }

    protected virtual void MergeStacks(ItemSlot[] slots, List<ItemStack> stacks)
    {
        foreach (ItemStack suppliedStack in slots.Select(slot => slot.Itemstack).OfType<ItemStack>())
        {
            ItemStack? similarStack = stacks.Find(stack => stack.Satisfies(suppliedStack));
            if (similarStack != null)
            {
                similarStack.StackSize += suppliedStack.StackSize;
            }
            else
            {
                stacks.Add(suppliedStack.Clone());
            }
        }
    }

    protected virtual void MergeIngredientStacks(ItemSlot[] slots, List<(ItemStack stack, IRecipeIngredient ingredient)> stacks, IRecipeIngredient?[] ingredients, IWorldAccessor world)
    {
        foreach (IRecipeIngredient? ingredient in ingredients)
        {
            if (ingredient == null || ingredient.MatchingType != EnumRecipeMatchType.Exact) continue;
            ItemStack? ingredientStack = ingredient.ResolvedItemStack;
            if (ingredientStack == null)
            {
                continue;
            }

            ItemStack? similarStack = null;
            foreach (var entry in stacks)
            {
                if (entry.stack.Equals(world, ingredientStack, GlobalConstants.IgnoredStackAttributes) && ingredient.RecipeAttributes == null)
                {
                    similarStack = entry.stack;
                    break;
                }
            }

            if (similarStack != null)
            {
                similarStack.StackSize += ingredientStack.StackSize;
            }
            else
            {
                stacks.Add((ingredientStack.Clone(), ingredient));
            }
        }
    }

    protected virtual bool MatchWildcardIngredients(List<ItemStack> suppliedStacks, IRecipeIngredient?[] ingredients)
    {
        foreach (IRecipeIngredient ingredient in ingredients.OfType<IRecipeIngredient>().Where(ingredient => ingredient.MatchingType != EnumRecipeMatchType.Exact))
        {
            bool found = false;
            int j = 0;
            for (; !found && j < suppliedStacks.Count; j++)
            {
                ItemStack inputStack = suppliedStacks[j];

                found =
                    ingredient.Type == inputStack.Class &&
                    WildcardUtil.Match(ingredient.Code, inputStack.Collectible.Code, ingredient.AllowedVariants) &&
                    inputStack.StackSize >= ingredient.Quantity &&
                    ingredient.Tags.Matches(inputStack.Collectible.Tags);

                found &= inputStack.Collectible.MatchesForCrafting(inputStack, this, ingredient);
            }

            if (!found)
            {
                return false;
            }

            suppliedStacks.RemoveAt(j - 1);
        }

        return true;
    }

    protected virtual bool MatchIngredientStacks(List<(ItemStack stack, IRecipeIngredient ingredient)> ingredientStacks, List<ItemStack> suppliedStacks)
    {
        bool equals = true;

        foreach ((ItemStack stack, IRecipeIngredient ingredient) in ingredientStacks)
        {
            bool found = false;

            for (int j = 0; !found && j < suppliedStacks.Count; j++)
            {
                found =
                    stack.Satisfies(suppliedStacks[j]) &&
                    stack.StackSize <= suppliedStacks[j].StackSize &&
                    suppliedStacks[j].Collectible.MatchesForCrafting(suppliedStacks[j], this, ingredient)
                ;

                if (found)
                {
                    suppliedStacks.RemoveAt(j);
                }
            }

            equals &= found;
        }

        return equals;
    }

    protected virtual void SeparateAndMergeIngredients(IRecipeIngredient?[] ingredients, List<IRecipeIngredient> exactMatchIngredients, List<IRecipeIngredient> wildcardAndToolIngredients)
    {
        foreach (IRecipeIngredient ingredient in ingredients.OfType<IRecipeIngredient>())
        {
            if (ingredient.MatchingType != EnumRecipeMatchType.Exact || !ingredient.ConsumeProperties.Consume)
            {
                wildcardAndToolIngredients.Add((IRecipeIngredient)ingredient.Clone());
                continue;
            }

            ItemStack? ingredientStack = ingredient.ResolvedItemStack;
            if (ingredientStack == null)
            {
                continue;
            }

            ItemStack? similarIngredientStack = exactMatchIngredients
                .Select(element => element.ResolvedItemStack)
                .OfType<ItemStack>()
                .FirstOrDefault(element => element.Satisfies(ingredientStack));

            if (similarIngredientStack != null)
            {
                similarIngredientStack.StackSize += ingredientStack.StackSize;
            }
            else
            {
                exactMatchIngredients.Add((IRecipeIngredient)ingredient.Clone());
            }
        }
    }

    protected virtual void ConsumeExactMatchIngredient(ItemStack inputStack, ItemSlot inputSlot, ItemSlot[] inputSlots, List<IRecipeIngredient> exactMatchIngredients, IPlayer byPlayer)
    {
        IRecipeIngredient? ingredient = exactMatchIngredients
            .Find(ingredient => ingredient.ResolvedItemStack?.Satisfies(inputStack) == true);

        ItemStack? ingredientStack = ingredient?.ResolvedItemStack;
        if (ingredientStack == null) return;
        int ingredientStackSize = ingredientStack.StackSize;  // Do not alter the .StackSize of the original ingredient

        int quantity = Math.Min(ingredientStackSize, inputStack.StackSize);

        inputStack.Collectible.OnConsumedByCrafting(inputSlots, inputSlot, this, ingredient, byPlayer, quantity);

        ingredientStackSize -= quantity;

        if (ingredientStackSize <= 0)
        {
            exactMatchIngredients.Remove(ingredient!);    // We know this is not null if ingredient?.ResolvedItemStack is not null. Stupid compiler.
        }
    }

    protected virtual void ConsumeWildcardMatchIngredient(ItemStack inputStack, ItemSlot inputSlot, ItemSlot[] inputSlots, List<IRecipeIngredient> wildcardAndToolIngredients, IPlayer byPlayer)
    {
        IRecipeIngredient? ingredient = wildcardAndToolIngredients
            .Find(ingredient =>
                   ingredient.Type == inputStack.Class
                && WildcardUtil.Match(ingredient.Code, inputStack.Collectible.Code, ingredient.AllowedVariants)
                && ingredient.Tags.Matches(inputStack.Collectible.GetTags(inputStack))
             );

        if (ingredient == null) return;

        int quantity = Math.Min(ingredient.Quantity, inputStack.StackSize);

        inputStack.Collectible.OnConsumedByCrafting(inputSlots, inputSlot, this, ingredient, byPlayer, quantity);

        if (!ingredient.ConsumeProperties.Consume)
        {
            wildcardAndToolIngredients.Remove(ingredient);
        }
        else
        {
            ingredient.Quantity -= quantity;

            if (ingredient.Quantity <= 0)
            {
                wildcardAndToolIngredients.Remove(ingredient);
            }
        }
    }

    protected virtual IEnumerable<IRecipeBase> GenerateRecipesForTagOnlyIngredients(IWorldAccessor world, IRecipeBase recipe)
    {
        var concreteIngredientsMap = new Dictionary<string, List<IRecipeIngredient>>();
        foreach (IRecipeIngredient ingredient in recipe.RecipeIngredients.Where(ingredient => ingredient.MatchingType == EnumRecipeMatchType.TagsOnly))
        {
            var concreteIngredients = FindConcreteIngredientsForTagOnlyIngredient(world, ingredient).ToList();
            concreteIngredientsMap.Add(ingredient.Id, concreteIngredients);
        }

        const int COMBINATORIAL_LIMIT = 10;     // With a higher combinatorial limit, many simple grid recipes which use tools (e.g. figurehead or debarked log) add e.g. 63 permutations (7x9 tool variants) for each different wood-typed output. This increases the number of grid recipes registered to very high levels like 40k, which then causes lag spikes when recipe matching. Better to match a smaller number of recipes even if that involves tag-matching at runtime
        var totalCombinations = 1;
        foreach (var (id, validIngredients) in concreteIngredientsMap)
        {
            if (validIngredients.Count == 0)
            {
                world.Logger.VerboseDebug("Ingredient {0} on {1} has no valid concrete ingredients, cannot generate permutations.", id, recipe.Name);
                return [recipe];
            }

            totalCombinations *= validIngredients.Count;
            if (totalCombinations > COMBINATORIAL_LIMIT)
            {
                world.Logger.VerboseDebug("Generating permutations for {0} would result in {1} concrete generated recipes, which is above the current threshold of {2}. Will match this recipe during gameplay instead.", recipe.Name, totalCombinations, COMBINATORIAL_LIMIT);
                return [recipe];
            }
        }

        world.Logger.VerboseDebug("Generating permutations for  {0} with {1} virtual ingredients into {2} concrete recipes...", recipe.Name, concreteIngredientsMap.Count, totalCombinations);
        return GeneratePermutations(recipe, concreteIngredientsMap);

        static IEnumerable<IRecipeBase> GeneratePermutations(IRecipeBase recipe, Dictionary<string, List<IRecipeIngredient>> concreteIngredientsMap)
        {
            bool hadTagIngredient = false;
            foreach (IRecipeIngredient tagIngredient in recipe.RecipeIngredients.Where(ingredient => ingredient.MatchingType == EnumRecipeMatchType.TagsOnly))
            {
                hadTagIngredient = true;

                var replacements = concreteIngredientsMap[tagIngredient.Id];
                foreach (IRecipeIngredient concreteIngredient in replacements)
                {
                    // Replace the tag only ingred with a concrete version. This will happen repeatedly.
                    tagIngredient.Code = concreteIngredient.Code;
                    tagIngredient.Type = concreteIngredient.Type;
                    tagIngredient.MatchingType = EnumRecipeMatchType.Exact;

                    // Clone the recipe with this ingredient replaced by a concrete one:
                    IRecipeBase partialRecipe = recipe.CloneAsInterface();

                    // Recurse until all ingredients are de-virtualized:
                    foreach(IRecipeBase finalRecipe in GeneratePermutations(partialRecipe, concreteIngredientsMap))
                    {
                        yield return finalRecipe;
                    }
                }
            }

            if (!hadTagIngredient)
            {
                // Fully de-virtualized, return this recipe:
                yield return recipe;
            }
        }

        static IEnumerable<IRecipeIngredient> FindConcreteIngredientsForTagOnlyIngredient(IWorldAccessor world, IRecipeIngredient ingredientSpecification)
        {
            return world.Items
                .Where(collectible => collectible?.Code != null)
                .Where(collectible => ingredientSpecification.Tags.Matches(collectible.Tags))
                .Select(collectible =>
                {
                    IRecipeIngredient newIngredient = (IRecipeIngredient)ingredientSpecification.Clone();
                    newIngredient.Code = collectible.Code;
                    newIngredient.Type = collectible.ItemClass;
                    return newIngredient;
                });
        }
    }
}
