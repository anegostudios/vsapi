using System.Collections.Generic;
using System.Linq;

#nullable disable

namespace Vintagestory.API.Common
{
    public interface IRecipeIngredient
    {
        string Name { get; }
        AssetLocation Code { set; get; }
    }

    public interface IRecipeOutput
    {
        void FillPlaceHolder(string key, string value);
    }

    public interface IRecipeBase<T>
    {
        AssetLocation Name { get; set; }
        bool Enabled { get; set; }


        Dictionary<string, string[]> GetNameToCodeMapping(IWorldAccessor world);
        bool Resolve(IWorldAccessor world, string sourceForErrorLogging);
        T Clone();

        IRecipeIngredient[] Ingredients { get; }

        IRecipeOutput Output { get; }
    }

    /// <summary>
    /// Creates a new base recipe type. Almost all recipe types extend from this.
    /// </summary>
    /// <typeparam name="T">The resulting recipe type.</typeparam>
    [DocumentAsJson]
    public abstract class RecipeBase<T> : IRecipeBase<T>
    {
        /// <summary>
        /// The ID of the recipe. Automatically generated when the recipe is loaded.
        /// </summary>
        public int RecipeId;

        /// <summary>
        /// <!--<jsonoptional>Required</jsonoptional>-->
        /// An array of ingredients for this recipe. If only using a single ingredient, see <see cref="Ingredient"/>.<br/>
        /// Required if not using <see cref="Ingredient"/>.
        /// </summary>
        [DocumentAsJson] public CraftingRecipeIngredient[] Ingredients;

        /// <summary>
        /// <!--<jsonoptional>Required</jsonoptional>-->
        /// A single ingredient for this recipe. If you need to use more than one ingredient, see <see cref="Ingredients"/>.<br/>
        /// Required if not using <see cref="Ingredients"/>.
        /// </summary>
        [DocumentAsJson] public CraftingRecipeIngredient Ingredient
        {
            get { return Ingredients != null && Ingredients.Length > 0 ? Ingredients[0] : null; }
            set { Ingredients = new CraftingRecipeIngredient[] { value }; }
        }

        /// <summary>
        /// <!--<jsonoptional>Required</jsonoptional>-->
        /// The output when the recipe is successful. 
        /// </summary>
        [DocumentAsJson] public JsonItemStack Output;

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>Asset Path</jsondefault>-->
        /// Adds a name to this recipe. Used for logging, and determining helve hammer workability for smithing recipes.
        /// </summary>
        [DocumentAsJson] public AssetLocation Name { get; set; }

        /// <summary>
        /// <!--<jsonoptional>Optional</jsonoptional><jsondefault>True</jsondefault>-->
        /// Should this recipe be loaded by the game?
        /// </summary>
        [DocumentAsJson] public bool Enabled { get; set; } = true;

        IRecipeIngredient[] IRecipeBase<T>.Ingredients => Ingredients.Select(i => i as IRecipeIngredient).ToArray();
        IRecipeOutput IRecipeBase<T>.Output => Output;

        public abstract T Clone();
        public abstract Dictionary<string, string[]> GetNameToCodeMapping(IWorldAccessor world);
        public abstract bool Resolve(IWorldAccessor world, string sourceForErrorLogging);


    }
}
