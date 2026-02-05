using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vintagestory.API.Util;

namespace Vintagestory.API.Common;

/// <summary>
/// Represents a crafting recipe to be made on the crafting grid.
/// </summary>
/// <example><code language="json">
///{
///	"ingredientPattern": "GS,S_",
///	"ingredients": {
///		"G": {
///			"type": "item",
///			"code": "drygrass"
///		},
///		"S": {
///			"type": "item",
///			"code": "stick"
///		}
///	},
///	"width": 2,
///	"height": 2,
///	"output": {
///		"type": "item",
///		"code": "firestarter"
///	}
///}
/// </code></example>
[DocumentAsJson]
public class GridRecipe : RecipeBase, IByteSerializable, IConcreteCloneable<GridRecipe>
{
    #region From JSON
    /// <summary>
    /// The resulting stack when the recipe is created.
    /// </summary>
    [DocumentAsJson("Required")]
    public CraftingRecipeIngredient? Output { get; set; }

    /// <summary>
    /// The recipes ingredients in any order, including the code used in the ingredient pattern.
    /// <br/>Note: from game version 1.20.4, this becomes <b>null on server-side</b> after completion of recipe resolving during server start-up phase
    /// </summary>
    [DocumentAsJson("Required")]
    public Dictionary<string, CraftingRecipeIngredient>? Ingredients { get; set; }

    /// <summary>
    /// The pattern of the ingredient. Order for a 3x3 recipe:<br/>
    /// 1 2 3<br/>
    /// 4 5 6<br/>
    /// 7 8 9<br/>
    /// Order for a 2x2 recipe:<br/>
    /// 1 2<br/>
    /// 3 4<br/>
    /// Commas separate each horizontal row, and an underscore ( _ ) marks a space as empty.
    /// <br/>Note: from game version 1.20.4, this becomes <b>null on server-side</b> after completion of recipe resolving during server start-up phase
    /// </summary>
    [DocumentAsJson("Required")]
    public string? IngredientPattern { get; set; }

    /// <summary>
    /// Required grid width for crafting this recipe 
    /// </summary>
    [DocumentAsJson("Recommended", "3")]
    public int Width { get; set; } = 3;

    /// <summary>
    /// Required grid height for crafting this recipe 
    /// </summary>
    [DocumentAsJson("Recommended", "3")]
    public int Height { get; set; } = 3;

    /// <summary>
    /// Info used by the handbook. By default, all recipes for an object will appear in a single preview. This allows you to split grid recipe previews into multiple.
    /// </summary>
    [DocumentAsJson("Optional", "0")]
    public int RecipeGroup { get; set; } = 0;

    /// <summary>
    /// Whether the order of input items should be respected
    /// </summary>
    [DocumentAsJson("Optional", "False")]
    public bool Shapeless { get; set; } = false;
    #endregion

    /// <summary>
    /// List of ingredients for each slot of the crafting grid
    /// </summary>
    public CraftingRecipeIngredient?[]? ResolvedIngredients { get; set; }

    public override IEnumerable<IRecipeIngredient> RecipeIngredients => ResolvedIngredients?.OfType<IRecipeIngredient>() ?? Ingredients?.Values
        ?? throw new InvalidOperationException($"Grid recipe '{Name}' has no ingredients specified or ingredients are failed to resolve");

    public override IRecipeOutput RecipeOutput => Output ?? throw new InvalidOperationException($"Grid recipe '{Name}' has no output specified");

    public override void OnParsed(IWorldAccessor world)
    {
        if (Ingredients == null) return;

        foreach ((string id, CraftingRecipeIngredient ingredient) in Ingredients)
        {
            ingredient.Id ??= id;
        }
    }

    /// <summary>
    /// Turns Ingredients into IItemStacks
    /// </summary>
    /// <param name="world"></param>
    /// <returns>True on successful resolve</returns>
    public override bool Resolve(IWorldAccessor world, string sourceForErrorLogging)
    {
        if (Output == null)
        {
            world.Logger.Error($"Grid Recipe '{Name}' has no output specified.");
            return false;
        }

        if (IngredientPattern == null)
        {
            world.Logger.Error($"Grid Recipe with output '{Output.Code}' has no ingredient pattern.");
            return false;
        }

        if (Ingredients == null)
        {
            world.Logger.Error($"Grid Recipe with output '{Output.Code}' has no ingredients.");
            return false;
        }

        IngredientPattern = IngredientPattern.Replace(",", "").Replace("\t", "").Replace("\r", "").Replace("\n", "").DeDuplicate();

        if (Width * Height != IngredientPattern.Length)
        {
            world.Logger.Error($"Grid Recipe with output '{Output.Code}' has and incorrect ingredient pattern length.");
            return false;
        }

        ResolvedIngredients = new CraftingRecipeIngredient?[Width * Height];
        for (int i = 0; i < IngredientPattern.Length; i++)
        {
            char codeCharacter = IngredientPattern[i];
            if (codeCharacter == ' ' || codeCharacter == '_')
            {
                continue;
            }
            string code = codeCharacter.ToString().DeDuplicate();

            if (!Ingredients.TryGetValue(code, out CraftingRecipeIngredient? craftingIngredient))
            {
                world.Logger.Error($"Grid Recipe with output '{Output.Code}' contains an ingredient pattern code '{code}' but supplies no ingredient for it.");
                return false;
            }

            if (!craftingIngredient.Resolve(world, "Grid recipe"))
            {
                world.Logger.Error($"Grid Recipe with output '{Output.Code}' contains an ingredient that cannot be resolved: {craftingIngredient}");
                return false;
            }

            CraftingRecipeIngredient ingredient = craftingIngredient.Clone();
            ingredient.Id = code;
            ResolvedIngredients[i] = ingredient;
        }

        if (!Output.Resolve(world, "Grid recipe"))
        {
            world.Logger.Error($"Grid Recipe '{Name}': Output {Output.Code} cannot be resolved.");
            return false;
        }

        return true;
    }

    public virtual void FreeRAMServer()
    {
        // From now on, we only require the resolvedIngredients
        IngredientPattern = null;
        Ingredients = null;
    }

    /// <summary>
    /// Puts the crafted ItemStack into the output slot and 
    /// consumes the required items from the input slots
    /// </summary>
    /// <param name="inputSlots"></param>
    /// <param name="byPlayer"></param>
    /// <param name="gridWidth"></param>
    public virtual bool ConsumeInput(IPlayer byPlayer, ItemSlot[] inputSlots, int gridWidth)
    {
        if (ResolvedIngredients == null)
        {
            return false;
        }

        if (Shapeless)
        {
            return ConsumeInputShapeLess(byPlayer, inputSlots, ResolvedIngredients);
        }

        int gridHeight = inputSlots.Length / gridWidth;
        if (gridWidth < Width || gridHeight < Height)
        {
            return false;
        }

        for (int column = 0; column <= gridWidth - Width; column++)
        {
            for (int row = 0; row <= gridHeight - Height; row++)
            {
                if (MatchesAtPosition(column, row, inputSlots, gridWidth, Width, ResolvedIngredients) && !ConsumeInputAt(byPlayer, inputSlots, gridWidth, Width, column, row, ResolvedIngredients))
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Check if this recipe matches given ingredients
    /// </summary>
    /// <param name="forPlayer">The player for trait testing. Can be null.</param>
    /// <param name="ingredients"></param>
    /// <param name="gridWidth"></param>
    /// <returns></returns>
    public virtual bool Matches(IPlayer forPlayer, IWorldAccessor world, ItemSlot[] ingredients, int gridWidth)
    {
        if (ResolvedIngredients == null)
        {
            return false;
        }

        if (!forPlayer.Entity.Api.Event.TriggerMatchesRecipe(forPlayer, this, ingredients))
        {
            return false;
        }

        if (!forPlayer.Entity.Api.Event.TriggerMatchesRecipe(forPlayer, this, ingredients, gridWidth))
        {
            return false;
        }

        int gridHeight = ingredients.Length / gridWidth;
        if (gridWidth < Width || gridHeight < Height) return false;

        if (Shapeless)
        {
            return MatchesShapeLess(ingredients, world, ResolvedIngredients);
        }

        for (int col = 0; col <= gridWidth - Width; col++)
        {
            for (int row = 0; row <= gridHeight - Height; row++)
            {
                if (MatchesAtPosition(col, row, ingredients, gridWidth, Width, ResolvedIngredients))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Serialized the recipe
    /// </summary>
    /// <param name="writer"></param>
    public override void ToBytes(BinaryWriter writer)
    {
        if (Ingredients == null || ResolvedIngredients == null || IngredientPattern == null || Output == null)
        {
            throw new InvalidOperationException("Some of required grid recipe fields are null, cant serialize it to bytes");
        }

        base.ToBytes(writer);

        writer.Write(Width);
        writer.Write(Height);
        writer.Write(Shapeless);

        Output.ToBytes(writer);

        for (int i = 0; i < ResolvedIngredients.Length; i++)
        {
            if (ResolvedIngredients[i] == null)
            {
                writer.Write(true);
                continue;
            }

            writer.Write(false);
            ResolvedIngredients[i]?.ToBytes(writer);
        }

        writer.Write(RecipeGroup);

        writer.Write(Ingredients.Count);
        foreach ((string key, CraftingRecipeIngredient value) in Ingredients)
        {
            writer.Write(key);
            value.ToBytes(writer);
        }

        writer.Write(IngredientPattern);
    }

    /// <summary>
    /// Deserializes the recipe
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="resolver"></param>
    public override void FromBytes(BinaryReader reader, IWorldAccessor resolver)
    {
        base.FromBytes(reader, resolver);

        Width = reader.ReadInt32();
        Height = reader.ReadInt32();
        Shapeless = reader.ReadBoolean();

        Output = new CraftingRecipeIngredient();
        Output.FromBytes(reader, resolver);

        ResolvedIngredients = new CraftingRecipeIngredient[Width * Height];
        for (int i = 0; i < ResolvedIngredients.Length; i++)
        {
            bool isNull = reader.ReadBoolean();
            if (isNull) continue;

            CraftingRecipeIngredient ingredient = new();
            ingredient.FromBytes(reader, resolver);
            ResolvedIngredients[i] = ingredient;
        }

        RecipeGroup = reader.ReadInt32();

        int ingredientsCount = reader.ReadInt32();
        Ingredients = [];
        for (int i = 0; i < ingredientsCount; i++)
        {
            string key = reader.ReadString();
            CraftingRecipeIngredient ingredient = new();
            ingredient.FromBytes(reader, resolver);
            Ingredients[key] = ingredient;
        }

        IngredientPattern = reader.ReadString();
    }

    /// <summary>
    /// Creates a deep copy
    /// </summary>
    /// <returns></returns>
    public override GridRecipe Clone()
    {
        GridRecipe recipe = new();

        CloneTo(recipe);

        return recipe;
    }

    protected override void CloneTo(object recipe)
    {
        base.CloneTo(recipe);

        if (recipe is not GridRecipe gridRecipe)
        {
            throw new ArgumentException("CloneTo should take object of same class or it subclass");
        }

        gridRecipe.Output = Output?.Clone();
        gridRecipe.Ingredients = [];
        if (Ingredients != null)
        {
            foreach ((string key, CraftingRecipeIngredient value) in Ingredients)
            {
                gridRecipe.Ingredients[key] = value.Clone();
            }
        }
        gridRecipe.IngredientPattern = IngredientPattern;
        gridRecipe.Width = Width;
        gridRecipe.Height = Height;
        gridRecipe.RecipeGroup = RecipeGroup;
        gridRecipe.Shapeless = Shapeless;

        if (ResolvedIngredients != null)
        {
            gridRecipe.ResolvedIngredients = new CraftingRecipeIngredient[ResolvedIngredients.Length];
            for (int i = 0; i < ResolvedIngredients.Length; i++)
            {
                gridRecipe.ResolvedIngredients[i] = ResolvedIngredients[i]?.Clone();
            }
        }
    }
}
