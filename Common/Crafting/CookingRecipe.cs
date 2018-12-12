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
        public string GetNameForIngredients(IWorldAccessor worldForResolve, string recipeCode, ItemStack[] stacks)
        {
            OrderedDictionary<ItemStack, int> quantitiesByStack = new OrderedDictionary<ItemStack, int>();
            quantitiesByStack = mergeStacks(worldForResolve, stacks);

            CookingRecipe recipe = worldForResolve.CookingRecipes.FirstOrDefault(rec => rec.Code == recipeCode);

            switch (recipeCode)
            {
                case "soup":
                    {
                        string fullname = "";
                        List<string> mainingredients = new List<string>();
                        List<string> garnishnames = new List<string>();

                        int max = 0;
                        foreach (var val in quantitiesByStack)
                        {
                            if (val.Key.Collectible.Code.Path.Contains("waterportion")) continue;

                            max = Math.Max(val.Value, max);

                            if (val.Value > 1 || quantitiesByStack.Count == 2)
                            {
                                mainingredients.Add(ingredientName(val.Key));
                            }
                            else
                            {
                                garnishnames.Add(ingredientName(val.Key));
                            }
                        }

                        if (max == 3) fullname = Lang.Get("Hefty");
                        if (max == 2) fullname = Lang.Get("Hearty");


                        if (fullname.Length > 0) fullname += " ";
                        if (mainingredients.Count > 0) fullname += string.Join("-", mainingredients) + " soup";
                        else fullname += "soup";
                        if (garnishnames.Count > 0) fullname += " with ";
                        fullname += prettyList(garnishnames);

                        return fullname.UcFirst();
                    }

                case "porridge":
                    {
                        List<string> grainnames = new List<string>();
                        List<string> mashednames = new List<string>();
                        List<string> sprinklednames = new List<string>();
                        string topping = "";

                        foreach (var val in quantitiesByStack)
                        {
                            CookingRecipeIngredient ingred = recipe.GetIngrendientFor(val.Key);
                            if (ingred.Code == "topping")
                            {
                                topping = Lang.Get("Honey topped ");
                                continue;
                            }

                            if (getFoodCat(val.Key) == EnumFoodCategory.Grain)
                            {
                                grainnames.Add(ingredientName(val.Key));
                                continue;
                            }

                            if (getFoodCat(val.Key) == EnumFoodCategory.Fruit)
                            {
                                sprinklednames.Add(Lang.Get("{0}", ingredientName(val.Key)));
                                continue;
                            }

                            mashednames.Add(Lang.Get("{0}", ingredientName(val.Key)));
                        }


                        string fullname = string.Join("-", grainnames) + " " + Lang.Get("porridge");

                        if (mashednames.Count > 0 && sprinklednames.Count > 0)
                        {
                            if (mashednames.Count == 1 && sprinklednames.Count == 1)
                            {
                                fullname += Lang.Get(" with mashed {0} and sprinkled {1}", prettyList(mashednames), prettyList(sprinklednames));
                            } else
                            {
                                fullname += Lang.Get(" with mashed {0} as well as sprinkled {1}", prettyList(mashednames), prettyList(sprinklednames));
                            }
                            
                        } else
                        {
                            if (mashednames.Count > 0) fullname += Lang.Get(" with mashed {0}", prettyList(mashednames));
                            if (sprinklednames.Count > 0) fullname += Lang.Get(" with sprinkled {0}", prettyList(sprinklednames));
                        }

                        return topping + fullname.UcFirst();
                    }

                case "meatystew":
                    {
                        List<string> meatnames = new List<string>();
                        List<string> veggienames = new List<string>();
                        string topping="";

                        int max = 0;
                        string prefix = "";
                        foreach (var val in quantitiesByStack)
                        {
                            CookingRecipeIngredient ingred = recipe.GetIngrendientFor(val.Key);
                            if (ingred.Code == "topping")
                            {
                                topping = Lang.Get("Honey topped ");
                                continue;
                            }

                            EnumFoodCategory foodCat = getFoodCat(val.Key);

                            if (foodCat == EnumFoodCategory.Protein)
                            {
                                max = Math.Max(val.Value, max);
                                meatnames.Add(ingredientName(val.Key));
                                continue;
                            }

                            veggienames.Add(ingredientName(val.Key));
                        }

                        if (max == 4) prefix = Lang.Get("Hefty");
                        if (max == 3) prefix = Lang.Get("Hearty");
                        prefix = prefix.Length > 0 ? prefix + " ": "";

                        string boiledAdd = "";
                        if (veggienames.Count > 0) boiledAdd = Lang.Get(" with boiled {0}", prettyList(veggienames));

                        return Lang.Get("{0}{1}{2} Stew{3}", topping, prefix, string.Join("-", meatnames), boiledAdd).UcFirst();
                    }

                case "vegetablestew":
                    {
                        string fullname = "";
                        List<string> mainingredients = new List<string>();
                        List<string> garnishnames = new List<string>();

                        int max = 0;
                        foreach (var val in quantitiesByStack)
                        {
                            max = Math.Max(val.Value, max);

                            if (val.Value > 1 || quantitiesByStack.Count == 2)
                            {
                                mainingredients.Add(ingredientName(val.Key));
                            }
                            else
                            {
                                garnishnames.Add(ingredientName(val.Key));
                            }
                        }

                        if (max == 4) fullname = Lang.Get("Hefty");
                        if (max == 3) fullname = Lang.Get("Hearty");


                        if (fullname.Length > 0) fullname += " ";
                        fullname += string.Join("-", mainingredients) + " stew";
                        if (garnishnames.Count > 0) fullname += " with " + prettyList(garnishnames) + " garnish";

                        return fullname.UcFirst();
                    }

            }

            return "unknown meal";
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


        string prettyList(List<string> names)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < names.Count; i++)
            {
                if (i > 0)
                {
                    if (i < names.Count - 1) str.Append(", ");
                    else str.Append(" " + Lang.Get("and") + " ");
                }
                str.Append(names[i]);
            }

            return str.ToString();
        }
    }

    public interface ICookingRecipeNamingHelper
    {
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
