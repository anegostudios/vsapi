using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    /// Creates a new base recipe type.  
    /// </summary>
    /// <typeparam name="T">The resulting recipe type.</typeparam>
    public abstract class RecipeBase<T> : IRecipeBase<T>
    {
        public int RecipeId;

        /// <summary>
        /// ...or alternatively for recipes with multiple ingredients
        /// </summary>
        public CraftingRecipeIngredient[] Ingredients;

        public CraftingRecipeIngredient Ingredient
        {
            get { return Ingredients != null && Ingredients.Length > 0 ? Ingredients[0] : null; }
            set { Ingredients = new CraftingRecipeIngredient[] { value }; }
        }

        public JsonItemStack Output;

        public AssetLocation Name { get; set; }
        public bool Enabled { get; set; } = true;

        IRecipeIngredient[] IRecipeBase<T>.Ingredients => Ingredients.Select(i => i as IRecipeIngredient).ToArray();
        IRecipeOutput IRecipeBase<T>.Output => Output;

        public abstract T Clone();
        public abstract Dictionary<string, string[]> GetNameToCodeMapping(IWorldAccessor world);
        public abstract bool Resolve(IWorldAccessor world, string sourceForErrorLogging);


    }
}
