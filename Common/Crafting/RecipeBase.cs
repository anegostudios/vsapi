using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Creates a new base recipe type.  
    /// </summary>
    /// <typeparam name="T">The resulting recipe type.</typeparam>
    public abstract class RecipeBase<T>
    {
        public int RecipeId;

        /// <summary>
        /// For recipes with only one ingredient...
        /// </summary>
        public CraftingRecipeIngredient Ingredient;

        /// <summary>
        /// ...or alternatively for recipes with multiple ingredients
        /// </summary>
        public CraftingRecipeIngredient[] Ingredients;

        public JsonItemStack Output;
        public AssetLocation Name;
        public bool Enabled = true;

        public abstract Dictionary<string, string[]> GetNameToCodeMapping(IWorldAccessor world);

        public abstract bool Resolve(IWorldAccessor world, string sourceForErrorLogging);

        public abstract T Clone();
    }
}
