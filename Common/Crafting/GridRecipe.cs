using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Util;

namespace Vintagestory.API.Common
{
    public class GridRecipeIngredient : CraftingRecipeIngredient
    {
        public string PatternCode;

        public override void ToBytes(BinaryWriter writer)
        {
            base.ToBytes(writer);
            writer.Write(PatternCode);
        }

        public override void FromBytes(BinaryReader reader, IWorldAccessor resolver)
        {
            base.FromBytes(reader, resolver);
            PatternCode = reader.ReadString();
        }
    }

    /// <summary>
    /// Represents a crafting recipe
    /// </summary>
    public class GridRecipe : IByteSerializable
    {
        /// <summary>
        /// Set by the recipe loader during json deserialization, if false the recipe will never be loaded.
        /// If loaded however, you can use this field to disable recipes during runtime.
        /// </summary>
        public bool Enabled = true;

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
        /// Info used by the handbook. Allows you to split grid recipe previews into multiple.
        /// </summary>
        public int RecipeGroup = 0;

        /// <summary>
        /// Used by the handbook. If false, will not appear in the "Created by" section
        /// </summary>
        public bool ShowInCreatedBy = true;

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
        /// Optional attribute data that you can attach any data to
        /// </summary>
        [JsonConverter(typeof(JsonAttributesConverter))]
        public JsonObject Attributes;

        /// <summary>
        /// If set only players with given trait can use this recipe
        /// </summary>
        public string RequiresTrait;

        /// <summary>
        /// If true, the output item will have its durability averaged over the input items
        /// </summary>
        public bool AverageDurability = true;

        /// <summary>
        /// If set, it will copy over the itemstack attributes from given ingredient code
        /// </summary>
        public string CopyAttributesFrom = null;


        public GridRecipeIngredient[] resolvedIngredients;


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

            if (IngredientPattern == null)
            {
                world.Logger.Error("Grid Recipe with output {0} has no ingredient pattern.", Output);
                return false;
            }
            if (Width * Height != IngredientPattern.Length)
            {
                world.Logger.Error("Grid Recipe with output {0} has and incorrect ingredient pattern length. Ignoring recipe.", Output);
                return false;
            }

            resolvedIngredients = new GridRecipeIngredient[Width * Height];
            for (int i = 0; i < IngredientPattern.Length; i++)
            {
                string code = IngredientPattern[i].ToString();
                if (code == " " || code == "_") continue;

                if (!Ingredients.ContainsKey(code))
                {
                    world.Logger.Error("Grid Recipe with output {0} contains an ingredient pattern code {1} but supplies no ingredient for it.", Output, code);
                    return false;
                }

                if (!Ingredients[code].Resolve(world, "Grid recipe"))
                {
                    world.Logger.Error("Grid Recipe with output {0} contains an ingredient that cannot be resolved: {1}", Output, Ingredients[code]);
                    return false;
                }

                resolvedIngredients[i] = Ingredients[code].CloneTo<GridRecipeIngredient>();
                resolvedIngredients[i].PatternCode = code;
            }

            if (!Output.Resolve(world, "Grid recipe"))
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
                    for (int i = 0; i < world.Blocks.Count; i++)
                    {
                        var block = world.Blocks[i];
                        if (block?.Code == null || block.IsMissing) continue;
                        if (val.Value.SkipVariants != null && WildcardUtil.MatchesVariants(val.Value.Code, block.Code, val.Value.SkipVariants)) continue;

                        if (WildcardUtil.Match(val.Value.Code, block.Code, val.Value.AllowedVariants))
                        {
                            string code = block.Code.Path.Substring(wildcardStartLen);
                            string codepart = code.Substring(0, code.Length - wildcardEndLen);
                            codes.Add(codepart);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < world.Items.Count; i++)
                    {
                        var item = world.Items[i];
                        if (item?.Code == null || item.IsMissing) continue;
                        if (val.Value.SkipVariants != null && WildcardUtil.MatchesVariants(val.Value.Code, item.Code, val.Value.SkipVariants)) continue;

                        if (WildcardUtil.Match(val.Value.Code, item.Code, val.Value.AllowedVariants))
                        {
                            string code = item.Code.Path.Substring(wildcardStartLen);
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
        /// <param name="byPlayer"></param>
        /// <param name="gridWidth"></param>
        public bool ConsumeInput(IPlayer byPlayer, ItemSlot[] inputSlots, int gridWidth)
        {
            if (Shapeless)
            {
                return ConsumeInputShapeLess(byPlayer, inputSlots);
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
                        return ConsumeInputAt(byPlayer, inputSlots, gridWidth, col, row);
                    }

                    i++;
                }
            }

            return false;
        }

        private bool ConsumeInputShapeLess(IPlayer byPlayer, ItemSlot[] inputSlots)
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
                        WildcardUtil.Match(ingredient.Code, inStack.Collectible.Code, ingredient.AllowedVariants) &&
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

        private bool ConsumeInputAt(IPlayer byPlayer, ItemSlot[] inputSlots, int gridWidth, int colStart, int rowStart)
        {
            int gridHeight = inputSlots.Length / gridWidth;

            for (int col = 0; col < gridWidth; col++)
            {
                for (int row = 0; row < gridHeight; row++)
                {
                    ItemSlot slot = GetElementInGrid(row, col, inputSlots, gridWidth);
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
        /// <param name="forPlayer">The player for trait testing. Can be null.</param>
        /// <param name="ingredients"></param>
        /// <param name="gridWidth"></param>
        /// <returns></returns>
        public bool Matches(IPlayer forPlayer, ItemSlot[] ingredients, int gridWidth)
        {
            if (!forPlayer.Entity.Api.Event.TriggerMatchesRecipe(forPlayer, this, ingredients, gridWidth))
            {
                return false;
            }
            
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
                    if (MatchesAtPosition(col, row, ingredients, gridWidth))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        private bool MatchesShapeLess(ItemSlot[] suppliedSlots, int gridWidth)
        {
            int gridHeight = suppliedSlots.Length / gridWidth;
            if (gridWidth < Width || gridHeight < Height) return false;

            List<KeyValuePair<ItemStack, CraftingRecipeIngredient>> ingredientStacks = new List<KeyValuePair<ItemStack, CraftingRecipeIngredient>>();
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
                            WildcardUtil.Match(ingredient.Code, inputStack.Collectible.Code, ingredient.AllowedVariants) &&
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
                    if (ingredientStacks[j].Key.Equals(world, stack, GlobalConstants.IgnoredStackAttributes) && ingredient.RecipeAttributes == null)
                    {
                        ingredientStacks[j].Key.StackSize += stack.StackSize;
                        found = true;
                        break;
                    }
                }

                if (!found) ingredientStacks.Add(new KeyValuePair<ItemStack, CraftingRecipeIngredient>(stack.Clone(), ingredient));
            }



            if (ingredientStacks.Count != suppliedStacks.Count) return false;


            bool equals = true;

            for (int i = 0; equals && i < ingredientStacks.Count; i++)
            {
                bool found = false;

                for (int j = 0; !found && j < suppliedStacks.Count; j++)
                {
                    found =
                        ingredientStacks[i].Key.Satisfies(suppliedStacks[j]) &&
                        ingredientStacks[i].Key.StackSize <= suppliedStacks[j].StackSize &&
                        suppliedStacks[j].Collectible.MatchesForCrafting(suppliedStacks[j], this, ingredientStacks[i].Value)
                    ;

                    if (found) suppliedStacks.RemoveAt(j);
                }

                equals &= found;
            }

            return equals;
        }


        public bool MatchesAtPosition(int colStart, int rowStart, ItemSlot[] inputSlots, int gridWidth)
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


        /// <summary>
        /// Returns only the first matching itemstack, there may be multiple
        /// </summary>
        /// <param name="patternCode"></param>
        /// <param name="inputSlots"></param>
        /// <param name="gridWidth"></param>
        /// <returns></returns>
        public ItemStack GetInputStackForPatternCode(string patternCode, ItemSlot[] inputSlots)
        {
            var ingredient = resolvedIngredients.FirstOrDefault(ig => ig?.PatternCode == patternCode);
            if (ingredient == null) return null;

            foreach (var slot in inputSlots)
            {
                if (slot.Empty) continue;
                var inputStack = slot.Itemstack;
                if (inputStack == null) continue;
                if (ingredient.SatisfiesAsIngredient(inputStack) && inputStack.Collectible.MatchesForCrafting(inputStack, this, ingredient)) return inputStack;
            }

            return null;
        }

        public void GenerateOutputStack(ItemSlot[] inputSlots, ItemSlot outputSlot)
        {
            var outstack = outputSlot.Itemstack = Output.ResolvedItemstack.Clone();

            if (CopyAttributesFrom != null)
            {
                var instack = GetInputStackForPatternCode(CopyAttributesFrom, inputSlots);
                if (instack != null)
                {
                    var attr = instack.Attributes.Clone();
                    attr.MergeTree(outstack.Attributes);
                    outstack.Attributes = attr;
                }
            }

            outputSlot.Itemstack.Collectible.OnCreatedByCrafting(inputSlots, outputSlot, this);
        }


        public T GetElementInGrid<T>(int row, int col, T[] stacks, int gridwidth)
        {
            int gridHeight = stacks.Length / gridwidth;
            if (row < 0 || col < 0 || row >= gridHeight || col >= gridwidth) return default(T);

            return stacks[row * gridwidth + col];
        }

        public int GetGridIndex<T>(int row, int col, T[] stacks, int gridwidth)
        {
            int gridHeight = stacks.Length / gridwidth;
            if (row < 0 || col < 0 || row >= gridHeight || col >= gridwidth) return -1;

            return row * gridwidth + col;
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

            writer.Write(RequiresTrait != null);
            if (RequiresTrait != null)
            {
                writer.Write(RequiresTrait);
            }

            writer.Write(RecipeGroup);
            writer.Write(AverageDurability);
            writer.Write(CopyAttributesFrom != null);
            if (CopyAttributesFrom != null)
            {
                writer.Write(CopyAttributesFrom);
            }

            writer.Write(ShowInCreatedBy);
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

            resolvedIngredients = new GridRecipeIngredient[Width * Height];
            for (int i = 0; i < resolvedIngredients.Length; i++)
            {
                bool isnull = reader.ReadBoolean();
                if (isnull) continue;

                resolvedIngredients[i] = new GridRecipeIngredient();
                resolvedIngredients[i].FromBytes(reader, resolver);
            }

            Name = new AssetLocation(reader.ReadString());

            if (!reader.ReadBoolean())
            {
                string json = reader.ReadString();
                Attributes = new JsonObject(JToken.Parse(json));
            }

            if (reader.ReadBoolean())
            {
                RequiresTrait = reader.ReadString();
            }

            RecipeGroup = reader.ReadInt32();
            AverageDurability = reader.ReadBoolean();
            if (reader.ReadBoolean())
            {
                CopyAttributesFrom = reader.ReadString();
            }

            ShowInCreatedBy = reader.ReadBoolean();
        }

        /// <summary>
        /// Creates a deep copy
        /// </summary>
        /// <returns></returns>
        public GridRecipe Clone()
        {
            GridRecipe recipe = new GridRecipe();

            recipe.RecipeGroup = RecipeGroup;
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
                recipe.resolvedIngredients = new GridRecipeIngredient[resolvedIngredients.Length];
                for (int i = 0; i < resolvedIngredients.Length; i++)
                {
                    recipe.resolvedIngredients[i] = resolvedIngredients[i]?.CloneTo<GridRecipeIngredient>();
                }
            }

            recipe.Shapeless = Shapeless;
            recipe.Output = Output.Clone();
            recipe.Name = Name;
            recipe.Attributes = Attributes?.Clone();
            recipe.RequiresTrait = RequiresTrait;
            recipe.AverageDurability = AverageDurability;
            recipe.CopyAttributesFrom = CopyAttributesFrom;
            recipe.ShowInCreatedBy = ShowInCreatedBy;
            return recipe;
        }
    }
}
