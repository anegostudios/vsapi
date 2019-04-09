using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace Vintagestory.API.Common
{
    public class VanillaCookingRecipeNames : ICookingRecipeNamingHelper
    {
        /// <summary>
        /// Gets the name for ingredients in regards to food.
        /// </summary>
        /// <param name="worldForResolve">The world to resolve in.</param>
        /// <param name="recipeCode">The recipe code.</param>
        /// <param name="stacks">The stacks of items to add.</param>
        /// <returns>The name of the food type.</returns>
        public string GetNameForIngredients(IWorldAccessor worldForResolve, string recipeCode, ItemStack[] stacks)
        {
            OrderedDictionary<ItemStack, int> quantitiesByStack = new OrderedDictionary<ItemStack, int>();
            quantitiesByStack = mergeStacks(worldForResolve, stacks);

            CookingRecipe recipe = worldForResolve.CookingRecipes.FirstOrDefault(rec => rec.Code == recipeCode);

            int max = 1;
            string MealFormat = "meal";
            string topping = string.Empty;
            ItemStack PrimaryIngredient = null;
            ItemStack SecondaryIngredient = null;
            List<string> OtherIngredients = new List<string>();
            List<string> MashedNames = new List<string>();
            List<string> SprinkledNames = new List<string>();
            List<string> GarnishedNames = new List<string>();
            List<string> grainNames = new List<string>();
            string mainIngredients;
            string everythingelse = "";

            switch (recipeCode)
            {
                case "soup":
                    {
                        max = 0;
                        foreach (var val in quantitiesByStack)
                        {
                            CookingRecipeIngredient ingred = recipe.GetIngrendientFor(val.Key);
                            if (val.Key.Collectible.Code.Path.Contains("waterportion")) continue;
                            if(ingred.Code == "topping")
                            {
                                topping = "honeyportion";
                                continue;
                            }


                            if (max < val.Value)
                            {
                                max = val.Value;
                                if (PrimaryIngredient != null)
                                    SecondaryIngredient = PrimaryIngredient;
                                PrimaryIngredient = val.Key;
                                
                            }
                            else
                            {
                                OtherIngredients.Add(ingredientName(val.Key));
                            }

                        }

                        if (max == 2) max = 3;
                        else if (max == 3) max = 4;
                        else max = 2;

                        break;
                       
                    }

                case "porridge":
                    {
                        max = 0;
                        foreach (var val in quantitiesByStack)
                        {
                            CookingRecipeIngredient ingred = recipe.GetIngrendientFor(val.Key);
                            if (getFoodCat(val.Key) == EnumFoodCategory.Grain)
                            {
                                max++;
                                if (PrimaryIngredient == null)
                                    PrimaryIngredient = val.Key;
                                else if (SecondaryIngredient == null)
                                    SecondaryIngredient = val.Key;

                                continue;
                            }
                            if(ingred.Code == "topping")
                            {
                                topping = "honeyportion";
                                continue;
                            }

                            MashedNames.Add(ingredientName(val.Key));

                        }
                        break;
                    }

                case "meatystew":
                    {
                        max = 0;
                        foreach (var val in quantitiesByStack)
                        {
                            CookingRecipeIngredient ingred = recipe.GetIngrendientFor(val.Key);
                            
                            EnumFoodCategory foodCat = getFoodCat(val.Key);

                            if (foodCat == EnumFoodCategory.Protein)
                            {
                                if (PrimaryIngredient == val.Key || SecondaryIngredient == val.Key)
                                    continue;

                                if (PrimaryIngredient == null)
                                    PrimaryIngredient = val.Key;
                                else if (SecondaryIngredient == null)
                                    SecondaryIngredient = val.Key;
                                else
                                    OtherIngredients.Add(ingredientName(val.Key));

                                max += val.Value;

                                continue;
                            }
                            if(ingred.Code == "topping")
                            {
                                topping = "honeyportion";
                                continue;
                            }

                            OtherIngredients.Add(ingredientName(val.Key));
                        }

                        recipeCode = "stew";
                        break;
                    }

                case "vegetablestew":
                    {
                        max = 0;
                        
                        foreach (var val in quantitiesByStack)
                        {
                            CookingRecipeIngredient ingred = recipe.GetIngrendientFor(val.Key);

                            if (getFoodCat(val.Key) == EnumFoodCategory.Vegetable)
                            {
                                if (PrimaryIngredient == val.Key || SecondaryIngredient == val.Key)
                                    continue;

                                if (PrimaryIngredient == null)
                                    PrimaryIngredient = val.Key;
                                else if (SecondaryIngredient == null)
                                    SecondaryIngredient = val.Key;
                                else
                                    GarnishedNames.Add(ingredientName(val.Key));

                                max += val.Value;

                                continue;
                            }
                            GarnishedNames.Add(ingredientName(val.Key));

                        }
                        recipeCode = "stew";
                        break;
                    }

            }


            
            switch(max)
            {
                case 3:
                    MealFormat += "-hearty-" + recipeCode;
                    break;
                case 4:
                    MealFormat += "-hefty-" + recipeCode;
                    break;
                default:
                    MealFormat += "-normal-" + recipeCode;
                    break;
            }

            if (topping == "honeyportion")
            {
                MealFormat += "-honey";
            }
            //mealformat is done.  Time to do the main inredients.



            if (SecondaryIngredient != null)
            {
                mainIngredients = $"{getMainIngredientName(PrimaryIngredient, recipeCode)}-{getMainIngredientName(SecondaryIngredient, recipeCode, true)}";
            }
            else
                mainIngredients = getMainIngredientName(PrimaryIngredient, recipeCode);
            //Main ingredients are done.

            switch (recipeCode)
            {
                case "porridge":
                    if (MashedNames.Count > 0)
                    {
                        everythingelse = getMealAddsString("meal-adds-porridge-mashed", MashedNames);
                    } else
                    {
                        everythingelse = "";
                    }
                    break;
                case "stew":
                    if (OtherIngredients.Count > 0)
                    {
                        everythingelse = getMealAddsString("meal-adds-meatystew-boiled", OtherIngredients);
                    }
                    else if (GarnishedNames.Count > 0)
                    {
                        everythingelse = getMealAddsString("meal-adds-vegetablestew-garnish", GarnishedNames);
                    }
                    else
                    {
                        everythingelse = "";
                    }
                    break;
                case "soup":
                    if(OtherIngredients.Count > 0)
                    {
                        everythingelse = getMealAddsString("meal-adds-generic", OtherIngredients);
                    }
                    break;
            }
            //everything else is done.

            return Lang.Get(MealFormat, mainIngredients, everythingelse).Trim().UcFirst();
        }

        private EnumFoodCategory getFoodCat(ItemStack stack)
        {
            FoodNutritionProperties props = stack.Collectible.NutritionProps;
            if (props == null) props = stack.Collectible.CombustibleProps?.SmeltedStack?.ResolvedItemstack?.Collectible?.NutritionProps;

            if (props != null) return props.FoodCategory;

            return EnumFoodCategory.Dairy;
        }

        private string ingredientName(ItemStack stack)
        {
            string code = stack.Collectible.Code?.Domain + AssetLocation.LocationSeparator + "recipeingredient-" + stack.Class.ToString().ToLowerInvariant() + "-" + stack.Collectible.Code?.Path;

            return Lang.GetMatching(code);
        }

        private string getMainIngredientName(ItemStack itemstack, string code, bool secondary = false)
        { 
            if(secondary)
                return Lang.Get($"meal-ingredient-{code}-secondary-{getInternalName(itemstack)}");
            return Lang.Get($"meal-ingredient-{code}-primary-{getInternalName(itemstack)}");
        }

        private string getInternalName(ItemStack itemstack)
        {
            return itemstack.Collectible.Code.Path;
        }

        private string getMealAddsString(string code, List<string> ingredients1, List<string> ingredients2 = null)
        {
            if (ingredients2 == null)
                return Lang.Get(code, Lang.Get($"meal-ingredientlist-{ingredients1.Count}", ingredients1.ToArray()));
            return Lang.Get(code, Lang.Get($"meal-ingredientlist-{ingredients1.Count}", ingredients1.ToArray()), Lang.Get($"meal-ingredientlist-{ingredients2.Count}", ingredients2.ToArray()));
        }

        private OrderedDictionary<ItemStack, int> mergeStacks(IWorldAccessor worldForResolve, ItemStack[] stacks)
        {
            OrderedDictionary<ItemStack, int> dict = new OrderedDictionary<ItemStack, int>();

            List<ItemStack> stackslist = new List<ItemStack>(stacks);
            while (stackslist.Count > 0)
            {
                ItemStack stack = stackslist[0];
                stackslist.RemoveAt(0);
                int cnt = 1;

                while (true)
                {
                    ItemStack foundstack = stackslist.FirstOrDefault((otherstack) => otherstack.Equals(worldForResolve, stack, GlobalConstants.IgnoredStackAttributes));

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

    /// <summary>
    /// Interface for a helper for cooking various food in game.
    /// </summary>
    public interface ICookingRecipeNamingHelper
    {
        /// <summary>
        /// Gets the name for ingredients in regards to food.
        /// </summary>
        /// <param name="worldForResolve">The world to resolve in.</param>
        /// <param name="recipeCode">The recipe code.</param>
        /// <param name="stacks">The stacks of items to add.</param>
        /// <returns>The name of the food type.</returns>
        string GetNameForIngredients(IWorldAccessor worldForResolve, string recipeCode, ItemStack[] stacks);
    }

    public class CookingRecipe : ByteSerializable
    {
        public string Code;
        public CookingRecipeIngredient[] Ingredients;
        public bool Enabled = true;
        public CompositeShape Shape;

        public static Dictionary<string, ICookingRecipeNamingHelper> NamingRegistry = new Dictionary<string, ICookingRecipeNamingHelper>();

        static CookingRecipe()
        {
            NamingRegistry["porridge"] = new VanillaCookingRecipeNames();
            NamingRegistry["meatystew"] = new VanillaCookingRecipeNames();
            NamingRegistry["vegetablestew"] = new VanillaCookingRecipeNames();
            NamingRegistry["soup"] = new VanillaCookingRecipeNames();
        }

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
            /*StringBuilder name = new StringBuilder();
            for (int i = 0; i < NameComponentOrder.Length; i++)
            {
                int index = NameComponentOrder[i];

                CookingRecipeIngredient ingred = Ingredients[index];
                for (int j = 0; j < inputStacks.Length; j++)
                {
                    CookingRecipeStack crstack = ingred.GetMatchingStack(inputStacks[j]);
                    if (crstack != null)
                    {
                        if (name.Length > 0) name.Append(" ");
                        string namecomp = crstack.NameComponent.Replace("{stackcode}", inputStacks[j].Collectible.Code.Path);
                        name.Append(Lang.Get(namecomp));
                        break;
                    }
                }

            }

            return name.ToString();*/

            
            ICookingRecipeNamingHelper namer = null;
            if (NamingRegistry.TryGetValue(Code, out namer))
            {
                return namer.GetNameForIngredients(worldForResolve, Code, inputStacks);
            }

            return Lang.Get("unknown");
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

       //     CanBeServedInto = new JsonItemStack();
       //     CanBeServedInto.FromBytes(reader, resolver.ClassRegistry);
       //     CanBeServedInto.Resolve(resolver, "[FromBytes]");
        }

    }
    
}
