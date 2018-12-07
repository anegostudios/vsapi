using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Represents a crafting recipe
    /// </summary>
    public class GridRecipe : ByteSerializable
    {
        /// <summary>
        /// The pattern of the ingredient. Order for a 3x3 recipe: 
        /// 1 2 3
        /// 4 5 6
        /// 7 8 9
        /// Order for a 2x2 recipe:
        /// 1 2
        /// 3 4
        /// </summary>
        public string IngredientPattern;

        /// <summary>
        /// The recipes ingredients in any order
        /// </summary>
        public Dictionary<string, CraftingRecipeIngredient> Ingredients;

        /// <summary>
        /// Required grid width for crafting this recipe 
        /// </summary>
        public int Width = 3;

        /// <summary>
        /// Required grid height for crafting this recipe 
        /// </summary>
        public int Height = 3;

        /// <summary>
        /// The resulting Stack
        /// </summary>
        public CraftingRecipeIngredient Output;

        /// <summary>
        /// Whether the order of input items should be respected
        /// </summary>
        public bool Shapeless = false;

        /// <summary>
        /// Name of the recipe, optional
        /// </summary>
        public AssetLocation Name;

        /// <summary>
        /// Optional attributes
        /// </summary>
        [JsonConverter(typeof(JsonAttributesConverter))]
        public JsonObject Attributes;


        public CraftingRecipeIngredient[] resolvedIngredients;


        IWorldAccessor world;

        /// <summary>
        /// Turns Ingredients into IItemStacks
        /// </summary>
        /// <param name="world"></param>
        /// <returns>True on successful resolve</returns>
        public bool ResolveIngredients(IWorldAccessor world)
        {
            this.world = world;

            IngredientPattern = IngredientPattern.Replace(",", "").Replace("\t", "").Replace("\r", "").Replace("\n", "");

            if (IngredientPattern == null || Width * Height != IngredientPattern.Length)
            {
                world.Logger.Error("Grid Recipe with output {0} has no ingredient pattern or incorrect ingredient pattern length. Ignoring recipe.", Output);
                return false;
            }

            resolvedIngredients = new CraftingRecipeIngredient[Width * Height];
            for (int i = 0; i < IngredientPattern.Length; i++)
            {
                string code = IngredientPattern[i].ToString();
                if (code == " " || code == "_") continue;

                if (!Ingredients.ContainsKey(code))
                {
                    world.Logger.Error("Grid Recipe with output {0} contains an ingredient pattern code {1} but supplies no ingredient for it.", Output, code);
                    return false;
                }

                if (!Ingredients[code].Resolve(world))
                {
                    world.Logger.Error("Grid Recipe with output {0} contains an ingredient that cannot be resolved: {1}", Output, Ingredients[code]);
                    return false;
                }

                resolvedIngredients[i] = Ingredients[code];
            }

            if (!Output.Resolve(world))
            {
                world.Logger.Error("Grid Recipe '{0}': Output {1} cannot be resolved", Name, Output);
                return false;
            }

            return true;
        }


        /// <summary>
        /// Resolves Wildcards in the ingredients
        /// </summary>
        /// <param name="world"></param>
        /// <returns></returns>
        public Dictionary<string, string[]> GetNameToCodeMapping(IWorldAccessor world)
        {
            Dictionary<string, string[]> mappings = new Dictionary<string, string[]>();

            foreach (var val in Ingredients)
            {
                if (val.Value.Name == null || val.Value.Name.Length == 0) continue;
                if (!val.Value.Code.Path.Contains("*")) continue;
                int wildcardStartLen = val.Value.Code.Path.IndexOf("*");
                int wildcardEndLen = val.Value.Code.Path.Length - wildcardStartLen - 1;

                List<string> codes = new List<string>();
                if (val.Value.Type == EnumItemClass.Block)
                {
                    for (int i = 0; i < world.Blocks.Length; i++)
                    {
                        if (world.Blocks[i] == null || world.Blocks[i].IsMissing) continue;

                        if (CraftingRecipeIngredient.WildCardMatch(val.Value.Code, world.Blocks[i].Code, val.Value.AllowedVariants))
                        {
                            string code = world.Blocks[i].Code.Path.Substring(wildcardStartLen);
                            string codepart = code.Substring(0, code.Length - wildcardEndLen);
                            codes.Add(codepart);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < world.Items.Length; i++)
                    {
                        if (world.Items[i] == null || world.Items[i].IsMissing) continue;

                        if (CraftingRecipeIngredient.WildCardMatch(val.Value.Code, world.Items[i].Code, val.Value.AllowedVariants))
                        {
                            string code = world.Items[i].Code.Path.Substring(wildcardStartLen);
                            string codepart = code.Substring(0, code.Length - wildcardEndLen);
                            codes.Add(codepart);
                        }
                    }
                }

                mappings[val.Value.Name] = codes.ToArray();
            }

            return mappings;
        }


        /// <summary>
        /// Puts the crafted itemstack into the output slot and 
        /// consumes the required items from the input slots
        /// </summary>
        /// <param name="inputSlots"></param>
        /// <param name="world"></param>
        /// <param name="byPlayer"></param>
        /// <param name="gridWidth"></param>
        public bool ConsumeInput(IWorldAccessor world, IPlayer byPlayer, IItemSlot[] inputSlots, int gridWidth)
        {
            if (Shapeless)
            {
                return ConsumeInputShapeLess(world, byPlayer, inputSlots);
            }

            int gridHeight = inputSlots.Length / gridWidth;
            if (gridWidth < Width || gridHeight < Height) return false;
            int i = 0;
            for (int col = 0; col <= gridWidth - Width; col++)
            {
                for (int row = 0; row <= gridHeight - Height; row++)
                {
                    if (MatchesAtPosition(col, row, inputSlots, gridWidth))
                    {
                        return ConsumeInputAt(world, byPlayer, inputSlots, gridWidth, col, row);
                    }

                    i++;
                }
            }

            return false;
        }

        private bool ConsumeInputShapeLess(IWorldAccessor world, IPlayer byPlayer, IItemSlot[] inputSlots)
        {
            List<CraftingRecipeIngredient> exactMatchIngredients = new List<CraftingRecipeIngredient>();
            List<CraftingRecipeIngredient> wildcardIngredients = new List<CraftingRecipeIngredient>();

            for (int i = 0; i < resolvedIngredients.Length; i++)
            {
                CraftingRecipeIngredient ingredient = resolvedIngredients[i];
                if (ingredient == null) continue;

                if (ingredient.IsWildCard || ingredient.IsTool)
                {
                    wildcardIngredients.Add(ingredient.Clone());
                    continue;
                }

                ItemStack stack = ingredient.ResolvedItemstack;

                bool found = false;
                for (int j = 0; j < exactMatchIngredients.Count; j++)
                {
                    if (exactMatchIngredients[j].ResolvedItemstack.Satisfies(stack))
                    {
                        exactMatchIngredients[j].ResolvedItemstack.StackSize += stack.StackSize;
                        found = true;
                        break;
                    }
                }
                if (!found) exactMatchIngredients.Add(ingredient.Clone());
            }

            for (int i = 0; i < inputSlots.Length; i++)
            {
                ItemStack inStack = inputSlots[i].Itemstack;
                if (inStack == null) continue;

                for (int j = 0; j < exactMatchIngredients.Count; j++)
                {
                    if (exactMatchIngredients[j].ResolvedItemstack.Satisfies(inStack))
                    {
                        int quantity = Math.Min(exactMatchIngredients[j].ResolvedItemstack.StackSize, inStack.StackSize);

                        inStack.Collectible.OnConsumedByCrafting(inputSlots, inputSlots[i], this, exactMatchIngredients[j], byPlayer, quantity);

                        exactMatchIngredients[j].ResolvedItemstack.StackSize -= quantity;

                        if (exactMatchIngredients[j].ResolvedItemstack.StackSize <= 0)
                        {
                            exactMatchIngredients.RemoveAt(j);
                        }

                        break;
                    }
                }

                for (int j = 0; j < wildcardIngredients.Count; j++)
                {
                    CraftingRecipeIngredient ingredient = wildcardIngredients[j];

                    if (
                        ingredient.Type == inStack.Class &&
                        CraftingRecipeIngredient.WildCardMatch(ingredient.Code, inStack.Collectible.Code, ingredient.AllowedVariants) &&
                        inStack.StackSize >= ingredient.Quantity
                    )
                    {
                        int quantity = Math.Min(ingredient.Quantity, inStack.StackSize);

                        inStack.Collectible.OnConsumedByCrafting(inputSlots, inputSlots[i], this, ingredient, byPlayer, quantity);

                        if (ingredient.IsTool)
                        {
                            wildcardIngredients.RemoveAt(j);
                        }
                        else
                        {
                            ingredient.Quantity -= quantity;

                            if (ingredient.Quantity <= 0)
                            {
                                wildcardIngredients.RemoveAt(j);
                            }
                        }

                        break;
                    }
                }
            }

            return exactMatchIngredients.Count == 0;
        }

        private bool ConsumeInputAt(IWorldAccessor world, IPlayer byPlayer, IItemSlot[] inputSlots, int gridWidth, int colStart, int rowStart)
        {
            int gridHeight = inputSlots.Length / gridWidth;

            for (int col = 0; col < gridWidth; col++)
            {
                for (int row = 0; row < gridHeight; row++)
                {
                    IItemSlot slot = GetElementInGrid(row, col, inputSlots, gridWidth);
                    CraftingRecipeIngredient ingredient = GetElementInGrid(row - rowStart, col - colStart, resolvedIngredients, Width);

                    if (ingredient == null) continue;
                    if (slot.Itemstack == null) return false;

                    int quantity = ingredient.IsWildCard ? ingredient.Quantity : ingredient.ResolvedItemstack.StackSize;

                    slot.Itemstack.Collectible.OnConsumedByCrafting(inputSlots, slot, this, ingredient, byPlayer, quantity);
                }
            }

            return true;


        }


        /// <summary>
        /// Check if this recipe matches given ingredients
        /// </summary>
        /// <param name="ingredients"></param>
        /// <param name="gridWidth"></param>
        /// <returns></returns>
        public bool Matches(IItemSlot[] ingredients, int gridWidth)
        {
            if (Shapeless)
            {
                return MatchesShapeLess(ingredients, gridWidth);
            }

            int gridHeight = ingredients.Length / gridWidth;
            if (gridWidth < Width || gridHeight < Height) return false;

            for (int col = 0; col <= gridWidth - Width; col++)
            {
                for (int row = 0; row <= gridHeight - Height; row++)
                {
                    if (MatchesAtPosition(col, row, ingredients, gridWidth)) return true;
                }
            }

            return false;
        }


        private bool MatchesShapeLess(IItemSlot[] suppliedSlots, int gridWidth)
        {
            int gridHeight = suppliedSlots.Length / gridWidth;
            if (gridWidth < Width || gridHeight < Height) return false;

            List<ItemStack> ingredientStacks = new List<ItemStack>();
            List<ItemStack> suppliedStacks = new List<ItemStack>();

            // Step 1: Merge all stacks from the supplied slots
            for (int i = 0; i < suppliedSlots.Length; i++)
            {
                if (suppliedSlots[i].Itemstack != null)
                {
                    bool found = false;
                    for (int j = 0; j < suppliedStacks.Count; j++)
                    {
                        if (suppliedStacks[j].Satisfies(suppliedSlots[i].Itemstack))
                        {
                            suppliedStacks[j].StackSize += suppliedSlots[i].Itemstack.StackSize;
                            found = true;
                            break;
                        }
                    }
                    if (!found) suppliedStacks.Add(suppliedSlots[i].Itemstack.Clone());
                }
            }

            // Step 2: Merge all stacks from the recipe reference
            for (int i = 0; i < resolvedIngredients.Length; i++)
            {
                CraftingRecipeIngredient ingredient = resolvedIngredients[i];

                if (ingredient == null) continue;

                if (ingredient.IsWildCard)
                {
                    bool foundw = false;
                    int j = 0;
                    for (; !foundw && j < suppliedStacks.Count; j++)
                    {
                        ItemStack inputStack = suppliedStacks[j];

                        foundw =
                            ingredient.Type == inputStack.Class &&
                            CraftingRecipeIngredient.WildCardMatch(ingredient.Code, inputStack.Collectible.Code, ingredient.AllowedVariants) &&
                            inputStack.StackSize >= ingredient.Quantity
                        ;

                        foundw &= inputStack.Collectible.MatchesForCrafting(inputStack, this, ingredient);
                    }

                    if (!foundw) return false;
                    suppliedStacks.RemoveAt(j - 1);
                    continue;
                }

                ItemStack stack = ingredient.ResolvedItemstack;
                bool found = false;
                for (int j = 0; j < ingredientStacks.Count; j++)
                {
                    if (ingredientStacks[j].Equals(world, stack, GlobalConstants.IgnoredStackAttributes))
                    {
                        ingredientStacks[j].StackSize += stack.StackSize;
                        found = true;
                        break;
                    }
                }

                if (!found) ingredientStacks.Add(stack.Clone());
            }



            if (ingredientStacks.Count != suppliedStacks.Count) return false;


            bool equals = true;

            for (int i = 0; equals && i < ingredientStacks.Count; i++)
            {
                bool found = false;

                for (int j = 0; !found && j < suppliedStacks.Count; j++)
                {
                    found =
                        ingredientStacks[i].Satisfies(suppliedStacks[j]) &&
                        ingredientStacks[i].StackSize <= suppliedStacks[j].StackSize &&
                        suppliedStacks[j].Collectible.MatchesForCrafting(suppliedStacks[j], this, null)
                    ;

                    if (found) suppliedStacks.RemoveAt(j);
                }

                equals &= found;
            }

            return equals;
        }


        internal bool MatchesAtPosition(int colStart, int rowStart, IItemSlot[] inputSlots, int gridWidth)
        {
            int gridHeight = inputSlots.Length / gridWidth;

            for (int col = 0; col < gridWidth; col++)
            {
                for (int row = 0; row < gridHeight; row++)
                {
                    ItemStack inputStack = GetElementInGrid(row, col, inputSlots, gridWidth)?.Itemstack;
                    CraftingRecipeIngredient ingredient = GetElementInGrid(row - rowStart, col - colStart, resolvedIngredients, Width);

                    if ((inputStack == null) ^ (ingredient == null)) return false;
                    if (inputStack == null) continue;

                    if (!ingredient.SatisfiesAsIngredient(inputStack)) return false;

                    if (!inputStack.Collectible.MatchesForCrafting(inputStack, this, ingredient)) return false;
                }
            }

            return true;
        }




        internal T GetElementInGrid<T>(int row, int col, T[] stacks, int gridwidth)
        {
            int gridHeight = stacks.Length / gridwidth;
            if (row < 0 || col < 0 || row >= gridHeight || col >= gridwidth) return default(T);

            return stacks[row * gridwidth + col];
        }



        /// <summary>
        /// Serialized the recipe
        /// </summary>
        /// <param name="writer"></param>
        public void ToBytes(BinaryWriter writer)
        {
            writer.Write(Width);
            writer.Write(Height);
            Output.ToBytes(writer);
            writer.Write(Shapeless);

            for (int i = 0; i < resolvedIngredients.Length; i++)
            {
                if (resolvedIngredients[i] == null)
                {
                    writer.Write(true);
                    continue;
                }

                writer.Write(false);
                resolvedIngredients[i].ToBytes(writer);
            }

            writer.Write(Name.ToShortString());

            writer.Write(Attributes == null);
            if (Attributes != null)
            {
                writer.Write(Attributes.Token.ToString());
            }
        }

        /// <summary>
        /// Deserializes the recipe
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="resolver"></param>
        public void FromBytes(BinaryReader reader, IWorldAccessor resolver)
        {
            Width = reader.ReadInt32();
            Height = reader.ReadInt32();
            Output = new CraftingRecipeIngredient();
            Output.FromBytes(reader, resolver);
            Shapeless = reader.ReadBoolean();

            resolvedIngredients = new CraftingRecipeIngredient[Width * Height];
            for (int i = 0; i < resolvedIngredients.Length; i++)
            {
                bool isnull = reader.ReadBoolean();
                if (isnull) continue;

                resolvedIngredients[i] = new CraftingRecipeIngredient();
                resolvedIngredients[i].FromBytes(reader, resolver);
            }

            Name = new AssetLocation(reader.ReadString());

            if (!reader.ReadBoolean())
            {
                Attributes = new JsonObject(JToken.Parse(reader.ReadString()));
            }
        }

        /// <summary>
        /// Creates a deep copy
        /// </summary>
        /// <returns></returns>
        public GridRecipe Clone()
        {
            GridRecipe recipe = new GridRecipe();

            recipe.Width = Width;
            recipe.Height = Height;
            recipe.IngredientPattern = IngredientPattern;
            recipe.Ingredients = new Dictionary<string, CraftingRecipeIngredient>();
            if (Ingredients != null)
            {
                foreach (var val in Ingredients)
                {
                    recipe.Ingredients[val.Key] = val.Value.Clone();
                }
            }
            if (resolvedIngredients != null)
            {
                recipe.resolvedIngredients = new CraftingRecipeIngredient[resolvedIngredients.Length];
                for (int i = 0; i < resolvedIngredients.Length; i++)
                {
                    recipe.resolvedIngredients[i] = resolvedIngredients[i]?.Clone();
                }
            }

            recipe.Shapeless = Shapeless;
            recipe.Output = Output.Clone();
            recipe.Name = Name;
            recipe.Attributes = Attributes?.Clone();

            return recipe;
        }
    }
}
