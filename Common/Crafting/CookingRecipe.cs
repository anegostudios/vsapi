using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace Vintagestory.API.Common
{
    public abstract class VanillaCookingRecipe : CookingRecipe
    {
        protected class CookingIngredientState
        {
            public int max = 1;
            public string topping = string.Empty;
            public ItemStack PrimaryIngredient = null;
            public ItemStack SecondaryIngredient = null;
            public List<string> OtherIngredients = new List<string>();
            public List<string> MashedNames = new List<string>();
            public List<string> GarnishedNames = new List<string>();
            public List<string> grainNames = new List<string>();
            public string mainIngredients;
        }

        /// <summary>
        /// Gets the name for ingredients in regards to food.
        /// </summary>
        /// <param name="worldForResolve">The world to resolve in.</param>
        /// <param name="recipeCode">The recipe code.</param>
        /// <param name="stacks">The stacks of items to add.</param>
        /// <returns>The name of the food type.</returns>
        public override string GetNameForIngredients(IWorldAccessor worldForResolve, string recipeCode, ItemStack[] stacks)
        {
            OrderedDictionary<ItemStack, int> quantitiesByStack = mergeStacks(worldForResolve, stacks);
            string mealFormat = "meal";
            var state = GetCookingIngredientsState(quantitiesByStack);

            switch(state.max)
            {
                case 3:
                    mealFormat += "-hearty-" + recipeCode;
                    break;
                case 4:
                    mealFormat += "-hefty-" + recipeCode;
                    break;
                default:
                    mealFormat += "-normal-" + recipeCode;
                    break;
            }

            if (state.topping == "honeyportion")
            {
                mealFormat += "-honey";
            }
            //mealformat is done.  Time to do the main inredients.

            if (state.SecondaryIngredient != null)
            {
                state.mainIngredients = Lang.Get("multi-main-ingredients-format", getMainIngredientName(state.PrimaryIngredient, recipeCode), getMainIngredientName(state.SecondaryIngredient, recipeCode, true));
            }
            else
                state.mainIngredients = state.PrimaryIngredient == null ? "" : getMainIngredientName(state.PrimaryIngredient, recipeCode);
            // Main ingredients are done.

            return Lang.Get(mealFormat, state.mainIngredients, GetMealAdditions(state)).Trim().UcFirst();
        }

        protected abstract CookingIngredientState GetCookingIngredientsState(OrderedDictionary<ItemStack, int> quantitiesByStack);

        protected virtual string GetMealAdditions(CookingIngredientState state)
        {
            return "";
        }

        protected EnumFoodCategory getFoodCat(ItemStack stack)
        {
            FoodNutritionProperties props = stack.Collectible.NutritionProps;
            if (props == null) props = stack.Collectible.CombustibleProps?.SmeltedStack?.ResolvedItemstack?.Collectible?.NutritionProps;

            if (props != null) return props.FoodCategory;

            return EnumFoodCategory.Dairy;
        }

        protected string ingredientName(ItemStack stack, bool InsturmentalCase = false)
        {
            string code;

            code = stack.Collectible.Code?.Domain + AssetLocation.LocationSeparator + "recipeingredient-" + stack.Class.ToString().ToLowerInvariant() + "-" + stack.Collectible.Code?.Path;

            if (InsturmentalCase)
                code += "-insturmentalcase";

            return Lang.GetMatching(code);
        }

        protected string getMainIngredientName(ItemStack itemstack, string code, bool secondary = false)
        { 
            if(secondary)
                return Lang.Get($"meal-ingredient-{code}-secondary-{getInternalName(itemstack)}");
            return Lang.Get($"meal-ingredient-{code}-primary-{getInternalName(itemstack)}");
        }

        protected string getInternalName(ItemStack itemstack)
        {
            return itemstack.Collectible.Code.Path;
        }

        protected string getMealAddsString(string code, List<string> ingredients1, List<string> ingredients2 = null)
        {
            if (ingredients2 == null)
                return Lang.Get(code, Lang.Get($"meal-ingredientlist-{ingredients1.Count}", ingredients1.ToArray()));
            return Lang.Get(code, Lang.Get($"meal-ingredientlist-{ingredients1.Count}", ingredients1.ToArray()), Lang.Get($"meal-ingredientlist-{ingredients2.Count}", ingredients2.ToArray()));
        }
    }

    public class JamCookingRecipe : CookingRecipe
    {

        public override string GetNameForIngredients(IWorldAccessor worldForResolve, string recipeCode, ItemStack[] stacks)
        {
            var quantitiesByStack = mergeStacks(worldForResolve, stacks);
            foreach (var val in quantitiesByStack)
            {
                if (val.Key.Collectible.NutritionProps?.FoodCategory == EnumFoodCategory.Fruit)
                {
                    return Lang.Get("{0} jam", val.Key.GetName());
                }
            }

            return "unknown";
        }
    }

    public class SoupCookingRecipe : VanillaCookingRecipe
    {
        protected override CookingIngredientState GetCookingIngredientsState(OrderedDictionary<ItemStack, int> quantitiesByStack)
        {
            var state = new CookingIngredientState();
            state.max = 0;
            foreach (var val in quantitiesByStack)
            {
                CookingRecipeIngredient ingred = GetIngrendientFor(val.Key);
                if (val.Key.Collectible.Code.Path.Contains("waterportion")) continue;
                if (ingred?.Code == "topping")
                {
                    state.topping = "honeyportion";
                    continue;
                }


                if (state.max < val.Value)
                {
                    state.max = val.Value;
                    if (state.PrimaryIngredient != null)
                    {
                        state.SecondaryIngredient = state.PrimaryIngredient;
                    }
                    state.PrimaryIngredient = val.Key;
                                
                }
                else
                {
                    state.OtherIngredients.Add(ingredientName(val.Key, true));
                }

            }

            if (state.max == 2) state.max = 3;
            else if (state.max == 3) state.max = 4;
            else state.max = 2;
            return state;
        }

        protected override string GetMealAdditions(CookingIngredientState state)
        {
            return state.OtherIngredients.Count > 0 ? getMealAddsString("meal-adds-generic", state.OtherIngredients) : "";
        }
    }

    public class StewCookingRecipe : VanillaCookingRecipe
    {
        protected override CookingIngredientState GetCookingIngredientsState(OrderedDictionary<ItemStack, int> quantitiesByStack)
        {
            var state = new CookingIngredientState();
            if (Code == "meatystew")
            {
                state.max = 0;
                foreach (var val in quantitiesByStack)
                {
                    CookingRecipeIngredient ingred = GetIngrendientFor(val.Key);
                            
                    EnumFoodCategory foodCat = getFoodCat(val.Key);

                    if (foodCat == EnumFoodCategory.Protein)
                    {
                        if (state.PrimaryIngredient == val.Key || state.SecondaryIngredient == val.Key)
                            continue;

                        if (state.PrimaryIngredient == null)
                            state.PrimaryIngredient = val.Key;
                        else if (state.SecondaryIngredient == null)
                            state.SecondaryIngredient = val.Key;
                        else
                            state.OtherIngredients.Add(ingredientName(val.Key, true));

                        state.max += val.Value;

                        continue;
                    }

                    if (ingred?.Code == "topping")
                    {
                        state.topping = "honeyportion";
                        continue;
                    }

                    state.OtherIngredients.Add(ingredientName(val.Key, true));
                }
            }
            else
            {
                state.max = 0;
                        
                foreach (var val in quantitiesByStack)
                {
                    if (getFoodCat(val.Key) == EnumFoodCategory.Vegetable)
                    {
                        if (state.PrimaryIngredient == val.Key || state.SecondaryIngredient == val.Key)
                            continue;

                        if (state.PrimaryIngredient == null)
                            state.PrimaryIngredient = val.Key;
                        else if (state.SecondaryIngredient == null)
                            state.SecondaryIngredient = val.Key;
                        else
                            state.GarnishedNames.Add(ingredientName(val.Key, true));
                        state.max += val.Value;

                        continue;
                    }
                    state.GarnishedNames.Add(ingredientName(val.Key, true));
                }

                // Slightly ugly hack for soybean stew
                if (state.PrimaryIngredient == null)
                {
                    foreach (var val in quantitiesByStack)
                    {
                        //CookingRecipeIngredient ingred = recipe.GetIngrendientFor(val.Key); - whats this for?
                        state.PrimaryIngredient = val.Key;
                        state.max += val.Value;
                    }
                }   
            }

            return state;
        }

        protected override string GetMealAdditions(CookingIngredientState state)
        {
            if (state.OtherIngredients.Count > 0)
            {
                return getMealAddsString("meal-adds-meatystew-boiled", state.OtherIngredients);
            }
            if (state.GarnishedNames.Count > 0)
            {
                return getMealAddsString("meal-adds-vegetablestew-garnish", state.GarnishedNames);
            }
            return "";
        }
    }

    public class PorridgeCookingRecipe : VanillaCookingRecipe
    {
        protected override CookingIngredientState GetCookingIngredientsState(OrderedDictionary<ItemStack, int> quantitiesByStack)
        {
            var cookingState = new CookingIngredientState();
            cookingState.max = 0;
            foreach (var val in quantitiesByStack)
            {
                CookingRecipeIngredient ingred = GetIngrendientFor(val.Key);
                if (getFoodCat(val.Key) == EnumFoodCategory.Grain)
                {
                    cookingState.max++;
                    if (cookingState.PrimaryIngredient == null)
                        cookingState.PrimaryIngredient = val.Key;
                    else if (cookingState.SecondaryIngredient == null && val.Key != cookingState.PrimaryIngredient)
                        cookingState.SecondaryIngredient = val.Key;

                    continue;
                }
                if (ingred?.Code == "topping")
                {
                    cookingState.topping = "honeyportion";
                    continue;
                }
                cookingState.MashedNames.Add(ingredientName(val.Key, true));
            }

            return cookingState;
        }

        protected override string GetMealAdditions(CookingIngredientState state)
        {
            if (state.MashedNames.Count > 0)
            {
                return getMealAddsString("meal-adds-porridge-mashed", state.MashedNames);
            }
            return "";
        }
    }

    public abstract class CookingRecipe : IByteSerializable
    {
        public string Code;
        public CookingRecipeIngredient[] Ingredients;
        public bool Enabled = true;
        public CompositeShape Shape;
        public TransitionableProperties PerishableProps;

        // public static Dictionary<string, ICookingRecipeNamingHelper> NamingRegistry = new Dictionary<string, ICookingRecipeNamingHelper>();

        // static CookingRecipe()
        // {
        //     // TODO These have to be registered in the essentials project with
        //     api.RegisterCookingRecipe("porridge", typeof(PorridgeCookingRecipe));
        //     api.RegisterCookingRecipe("stew", typeof(StewCookingRecipe));
        //     api.RegisterCookingRecipe("soup", typeof(SoupCookingRecipe));
        //     api.RegisterCookingRecipe("jam", typeof(JamCookingRecipe));
        //     TODO END
        //     // These we will register instead because why else look for them
        //     NamingRegistry["porridge"] = new VanillaCookingRecipeNames();
        //     NamingRegistry["meatystew"] = new VanillaCookingRecipeNames();
        //     NamingRegistry["vegetablestew"] = new VanillaCookingRecipeNames();
        //     NamingRegistry["soup"] = new VanillaCookingRecipeNames();
        //     NamingRegistry["jam"] = new VanillaCookingRecipeNames();
        // }

        public bool Matches(ItemStack[] inputStacks)
        {
            int useless = 0;
            return Matches(inputStacks, ref useless);
        }

        public int GetQuantityServings(ItemStack[] stacks)
        {
            int quantity = 0;
            Matches(stacks, ref quantity);
            return quantity;
        }

        /// <summary>
        /// Gets the name of the output food if one exists.
        /// </summary>
        /// <param name="worldForResolve"></param>
        /// <param name="inputStacks"></param>
        /// <returns></returns>
        public string GetOutputName(IWorldAccessor worldForResolve, ItemStack[] inputStacks)
        {
            bool rotten = inputStacks.Any(stack => stack?.Collectible.Code.Path == "rot");
            if (rotten)
            {
                return Lang.Get("Rotten Food");
            }
            CookingRecipe recipe = worldForResolve.CookingRecipes.FirstOrDefault(rec => rec.Code == Code);
            return recipe != null ? GetNameForIngredients(worldForResolve, Code, inputStacks) : Lang.Get("unknown");
        }

        public bool Matches(ItemStack[] inputStacks, ref int quantityServings)
        {
            List<ItemStack> inputStacksList = new List<ItemStack>(inputStacks);
            List<CookingRecipeIngredient> ingredientList = new List<CookingRecipeIngredient>(Ingredients);

            int totalOutputQuantity = 99999;

            int[] curQuantities = new int[ingredientList.Count];
            for (int i = 0; i < curQuantities.Length; i++) curQuantities[i] = 0;

            while (inputStacksList.Count > 0)
            {
                ItemStack inputStack = inputStacksList[0];
                inputStacksList.RemoveAt(0);
                if (inputStack == null) continue;

                bool found = false;
                for (int i = 0; i < ingredientList.Count; i++)
                {
                    CookingRecipeIngredient ingred = ingredientList[i];
                    
                    if (ingred.Matches(inputStack))
                    {
                        if (curQuantities[i] >= ingred.MaxQuantity) continue;

                        totalOutputQuantity = Math.Min(totalOutputQuantity, inputStack.StackSize);
                        curQuantities[i]++;
                        found = true;
                        break;
                    }
                }

                // This input stack does not fit in this cooking recipe
                if (!found) return false;
            }

            // Any required ingredients left?
            for (int i = 0; i < ingredientList.Count; i++)
            {
                if (curQuantities[i] < ingredientList[i].MinQuantity) return false;
            }

            quantityServings = totalOutputQuantity;

            // Too many ingredients?
            for (int i = 0; i < inputStacks.Length; i++)
            {
                if (inputStacks[i] == null) continue;
                if (inputStacks[i].StackSize > quantityServings) return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the name for ingredients in regards to food.
        /// </summary>
        /// <param name="worldForResolve">The world to resolve in.</param>
        /// <param name="recipeCode">The recipe code.</param>
        /// <param name="stacks">The stacks of items to add.</param>
        /// <returns>The name of the food type.</returns>
        public abstract string GetNameForIngredients(IWorldAccessor worldForResolve, string recipeCode, ItemStack[] stacks);

        public CookingRecipeIngredient GetIngrendientFor(ItemStack stack, params CookingRecipeIngredient[] ingredsToskip)
        {
            if (stack == null) return null;

            for (int i = 0; i < Ingredients.Length; i++)
            {
                if (Ingredients[i].Matches(stack) && !ingredsToskip.Contains(Ingredients[i])) return Ingredients[i];
            }

            return null;
        }




        public void Resolve(IServerWorldAccessor world, string sourceForErrorLogging)
        {
            for (int i = 0; i < Ingredients.Length; i++)
            {
                Ingredients[i].Resolve(world, sourceForErrorLogging);
            }

          //  CanBeServedInto.Resolve(world, sourceForErrorLogging);
        }






        /// <summary>
        /// Serialized the alloy
        /// </summary>
        /// <param name="writer"></param>
        public void ToBytes(BinaryWriter writer)
        {
            writer.Write(Code);
            writer.Write(Ingredients.Length);
            for (int i = 0; i < Ingredients.Length; i++)
            {
                Ingredients[i].ToBytes(writer);
            }
            //writer.WriteArray(NameComponentOrder);

            writer.Write(Shape == null);
            if (Shape != null) writer.Write(Shape.Base.ToString());

            PerishableProps.ToBytes(writer);
            
      //      CanBeServedInto.ToBytes(writer);
        }

        /// <summary>
        /// Deserializes the alloy
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="resolver"></param>
        public void FromBytes(BinaryReader reader, IWorldAccessor resolver)
        {
            Code = reader.ReadString();
            Ingredients = new CookingRecipeIngredient[reader.ReadInt32()];

            for (int i = 0; i < Ingredients.Length; i++)
            {
                Ingredients[i] = new CookingRecipeIngredient();
                Ingredients[i].FromBytes(reader, resolver.ClassRegistry);
                Ingredients[i].Resolve(resolver, "[FromBytes]");
            }

            //NameComponentOrder = reader.ReadIntArray();

            if (!reader.ReadBoolean())
            {
                Shape = new CompositeShape() { Base = new AssetLocation(reader.ReadString()) };
            }

            PerishableProps = new TransitionableProperties();
            PerishableProps.FromBytes(reader, resolver.ClassRegistry);

       //     CanBeServedInto = new JsonItemStack();
       //     CanBeServedInto.FromBytes(reader, resolver.ClassRegistry);
       //     CanBeServedInto.Resolve(resolver, "[FromBytes]");
        }

        protected OrderedDictionary<ItemStack, int> mergeStacks(IWorldAccessor worldForResolve, ItemStack[] stacks)
        {
            OrderedDictionary<ItemStack, int> dict = new OrderedDictionary<ItemStack, int>();

            List<ItemStack> stackslist = new List<ItemStack>(stacks);
            while (stackslist.Count > 0)
            {
                ItemStack stack = stackslist[0];
                stackslist.RemoveAt(0);
                if (stack == null) continue;

                int cnt = 1;

                while (true)
                {
                    ItemStack foundstack = stackslist.FirstOrDefault((otherstack) => otherstack != null && otherstack.Equals(worldForResolve, stack, GlobalConstants.IgnoredStackAttributes));

                    if (foundstack != null)
                    {
                        stackslist.Remove(foundstack);
                        cnt++;
                        continue;
                    }

                    break;
                }

                dict[stack] = cnt;
            }

            return dict;
        }

    }
    
}
