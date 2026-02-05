using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Vintagestory.API.Util;

namespace Vintagestory.API.Common;

/// <summary>
/// Creates a recipe using a 3D voxel-based system. Used for recipes types such as clayforming, smithing, or stone-knapping.
/// </summary>
[DocumentAsJson]
public abstract class LayeredVoxelRecipe : RecipeBase
{
    #region From JSON
    /// <summary>
    /// An array of ingredients for this recipe. If only using a single ingredient, see <see cref="Ingredient"/>.<br/>
    /// Required if not using <see cref="Ingredient"/>.
    /// </summary>
    [DocumentAsJson("Required")]
    public CraftingRecipeIngredient[] Ingredients;

    /// <summary>
    /// A single ingredient for this recipe. If you need to use more than one ingredient, see <see cref="Ingredients"/>.<br/>
    /// Required if not using <see cref="Ingredients"/>.
    /// </summary>
    [DocumentAsJson("Required")]
    public CraftingRecipeIngredient? Ingredient
    {
        get => Ingredients != null && Ingredients.Length > 0 ? Ingredients[0] : null;
        set => Ingredients = value == null ? [] : [value];
    }

    /// <summary>
    /// A 2D array of strings that are layered together to form the recipe. Use "#" for solid, and "_" or " " for a gap.
    /// </summary>
    [DocumentAsJson("Required")]

    public string[][] Pattern;

    /// <summary>
    /// The final output of this recipe.
    /// </summary>
    [DocumentAsJson("Required")]
    public JsonItemStack Output;

    #endregion

    /// <summary>
    /// An array of voxels, created from <see cref="Pattern"/> during loading. This array is cloned when a player starts creating the recipe.
    /// </summary>
    public bool[,,] Voxels;

    /// <summary>
    /// The number of layers in this recipe, in the Y-axis.
    /// </summary>
    public abstract int QuantityLayers { get; }

    /// <summary>
    /// A category code for this recipe type. Used for error logging.
    /// </summary>
    public abstract string RecipeCategoryCode { get; }

    /// <summary>
    /// If true, the recipe is rotated 90 degrees in the Y axis.
    /// </summary>
    protected virtual bool RotateRecipe { get; set; } = false;



#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Justification: loaded from json, so property/field being null is error on level of json
    protected LayeredVoxelRecipe()
    {
        Voxels = new bool[16, QuantityLayers, 16];
    }
#pragma warning restore CS8618



    /// <summary>
    /// Generates the voxels for the recipe.
    /// </summary>
    public void GenVoxels()
    {
        int length = Pattern[0][0].Length;
        int width = Pattern[0].Length;
        int height = Pattern.Length;

        if (width > 16 || height > QuantityLayers || length > 16)
        {
            throw new InvalidOperationException(string.Format("Invalid {1} recipe {0}! Either Width or length is beyond 16 voxels or height is beyond {2} voxels", this.Name, RecipeCategoryCode, QuantityLayers));
        }

        for (int i = 0; i < Pattern.Length; i++)
        {
            if (Pattern[i].Length != width)
            {
                throw new InvalidOperationException(string.Format("Invalid {4} recipe {3}! Layer {0} has a width of {1}, " +
                    "which is not the same as the first layer width of {2}. All layers need to be sized equally.", i, Pattern[i].Length, width, this.Name, RecipeCategoryCode));
            }

            for (int j = 0; j < Pattern[i].Length; j++)
            {
                if (Pattern[i][j].Length != length)
                {
                    throw new InvalidOperationException(string.Format("Invalid {5} recipe {3}! Layer {0}, line {4} has a length of {1}, " +
                    "which is not the same as the first layer length of {2}. All layers need to be sized equally.", i, Pattern[i][j].Length, length, this.Name, j, RecipeCategoryCode));
                }
            }
        }

        // We'll center the recipe to the horizontal middle
        int startX = (16 - width) / 2;
        int startZ = (16 - length) / 2;

        for (int x = 0; x < Math.Min(width, 16); x++)
        {
            for (int y = 0; y < Math.Min(height, QuantityLayers); y++)
            {
                for (int z = 0; z < Math.Min(length, 16); z++)
                {
                    if (RotateRecipe)
                    {
                        Voxels[z + startZ, y, x + startX] = Pattern[y][x][z] != '_' && Pattern[y][x][z] != ' ';
                    }
                    else
                    {
                        Voxels[x + startX, y, z + startZ] = Pattern[y][x][z] != '_' && Pattern[y][x][z] != ' ';
                    }

                }
            }
        }
    }

    public override IEnumerable<IRecipeIngredient> RecipeIngredients => Ingredients ?? throw new InvalidOperationException($"Recipe '{Name}' has no ingredients specified");

    public override IRecipeOutput RecipeOutput => Output ?? throw new InvalidOperationException($"Recipe '{Name}' has no output specified");

    public override void OnParsed(IWorldAccessor world)
    {
        if (Ingredients == null) return;

        int ingredientIndex = 1;
        foreach (CraftingRecipeIngredient ingredient in Ingredients)
        {
            ingredient.Id ??= ingredientIndex++.ToString();
        }
    }

    /// <summary>
    /// Resolves the recipe.
    /// </summary>
    /// <param name="world"></param>
    /// <param name="sourceForErrorLogging"></param>
    /// <returns></returns>
    public override bool Resolve(IWorldAccessor world, string sourceForErrorLogging)
    {
        if (Pattern == null || Ingredient == null || RecipeOutput == null)
        {
            world.Logger.Error("{1} Recipe with output {0} has no ingredient pattern or missing ingredient/output. Ignoring recipe.", RecipeOutput, RecipeCategoryCode);
            return false;
        }

        if (!Ingredient.Resolve(world, RecipeCategoryCode + " recipe"))
        {
            world.Logger.Error("{2} Recipe with output {0}: Cannot resolve ingredient in {1}.", RecipeOutput, sourceForErrorLogging, RecipeCategoryCode);
            return false;
        }

        if (!RecipeOutput.Resolve(world, sourceForErrorLogging + " " + Ingredient.Code))
        {
            return false;
        }

        GenVoxels();
        return true;
    }

    /// <summary>
    /// Serialized the recipe
    /// </summary>
    /// <param name="writer"></param>
    public override void ToBytes(BinaryWriter writer)
    {
        if (Ingredient == null || Name == null)
        {
            throw new InvalidOperationException("Trying to write voxel recipe into bytes, but recipe ");
        }

        writer.Write(RecipeId);
        writer.Write(Ingredients.Length);
        foreach (CraftingRecipeIngredient ingredient in Ingredients)
        {
            ingredient.ToBytes(writer);
        }

        writer.Write(Pattern.Length);
        for (int i = 0; i < Pattern.Length; i++)
        {
            writer.WriteArray(Pattern[i]);
        }

        writer.Write(Name.ToShortString());

        RecipeOutput.ToBytes(writer);
    }

    /// <summary>
    /// Deserializes the alloy
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="resolver"></param>
    public override void FromBytes(BinaryReader reader, IWorldAccessor resolver)
    {
        RecipeId = reader.ReadInt32();

        int ingredientsCount = reader.ReadInt32();
        Ingredients = new CraftingRecipeIngredient[ingredientsCount];
        for (int i = 0; i < ingredientsCount; i++)
        {
            Ingredients[i] = new CraftingRecipeIngredient();
            Ingredients[i].FromBytes(reader, resolver);
        }

        int len = reader.ReadInt32();
        Pattern = new string[len][];
        for (int i = 0; i < Pattern.Length; i++)
        {
            Pattern[i] = reader.ReadStringArray();
        }

        Name = new AssetLocation(reader.ReadString());

        Output = new JsonItemStack();
        RecipeOutput.FromBytes(reader, resolver);
        RecipeOutput.Resolve(resolver, "[Voxel recipe FromBytes] " + Ingredient.Code);
        GenVoxels();
    }

    /// <summary>
    /// Matches the wildcards for the clay recipe.
    /// </summary>
    /// <param name="wildCard"></param>
    /// <param name="blockCode"></param>
    /// <returns></returns>
    public static bool WildCardMatch(AssetLocation wildCard, AssetLocation blockCode)
    {
        if (blockCode == null || !wildCard.Domain.Equals(blockCode.Domain)) return false;
        if (wildCard.Equals(blockCode)) return true;

        string pattern = Regex.Escape(wildCard.Path).Replace(@"\*", @"(.*)");

        return Regex.IsMatch(blockCode.Path, @"^" + pattern + @"$");
    }



    protected override void CloneTo(object recipe)
    {
        base.CloneTo(recipe);

        if (recipe is not LayeredVoxelRecipe voxelRecipe)
        {
            throw new ArgumentException("CloneTo should take object of same class or it subclass");
        }

        voxelRecipe.Ingredients = Ingredients?.Select(ingredient => ingredient.Clone()).ToArray();
        voxelRecipe.Ingredient = Ingredient?.Clone();
        voxelRecipe.Pattern = Pattern;
        voxelRecipe.Output = Output.Clone();
        voxelRecipe.Voxels = Voxels;
    }
}
