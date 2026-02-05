using Cairo;
using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Util;

namespace Vintagestory.API.Client
{
    public class StackAndWildCard
    {
        public ItemStack? Stack;
        public AssetLocation? WildCard;
    }

    public class GridRecipeAndUnnamedIngredients
    {
        public GridRecipe? Recipe;
        public Dictionary<int, ItemStack[]>? UnnamedIngredients;
    }

    /// <summary>
    /// Draws multiple ItemStack
    /// </summary>
    public sealed class SlideshowGridRecipeTextComponent : ItemstackComponentBase
    {
        public GridRecipeAndUnnamedIngredients[] GridRecipesAndUnnamedIngredients;
        public GridRecipeAndUnnamedIngredients? CurrentVisibleRecipe;


        private readonly Action<ItemStack?>? onStackClicked;
        private readonly ItemSlot dummySlot = new DummySlot();
        private readonly double size;
        private readonly Dictionary<string, ItemStack[]> resolvedIngredientsCache = [];
        private readonly Dictionary<int, LoadedTexture> extraTexts = [];
        private readonly ItemSlot currentVisibleOutputSlot = new DummySlot();
        private readonly ItemSlot[] currentVisibleInputs = [new DummySlot(), new DummySlot(), new DummySlot(), new DummySlot(), new DummySlot(), new DummySlot(), new DummySlot(), new DummySlot(), new DummySlot()];
        private LoadedTexture? hoverTexture;
        private int secondCounter = 0;
        private float secondsVisible = 1;
        private int currentItemIndex;
        private static readonly int[][,] variantDisplaySequence = new int[30][,];



        /// <summary>
        /// Flips through given array of grid recipes every second
        /// </summary>
        /// <param name="api"></param>
        /// <param name="gridRecipes"></param>
        /// <param name="size"></param>
        /// <param name="floatType"></param>
        /// <param name="onStackClicked"></param>
        /// <param name="allStacks">If set, will resolve wildcards based on this list, otherwise will search all available blocks/items</param>
        public SlideshowGridRecipeTextComponent(ICoreClientAPI api, GridRecipe[] gridRecipes, double size, EnumFloat floatType, Action<ItemStack>? onStackClicked = null, ItemStack[]? allStacks = null) : base(api)
        {
            size = GuiElement.scaled(size);
            double innerMargin = GuiElement.scaled(3);

            this.onStackClicked = onStackClicked;
            Float = floatType;
            BoundsPerLine = [new(0, 0, 2 * (size + innerMargin) + size, 2 * (size + innerMargin) + size)];
            this.size = size;

            Random fixedRand = new(123);

            for (int i = 0; i < 30; i++)
            {
                int[,] sq = variantDisplaySequence[i] = new int[3, 3];
                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        sq[x, y] = api.World.Rand.Next(99999);
                    }
                }
            }

            List<GridRecipeAndUnnamedIngredients> resolvedGridRecipes = [];

            ResolveRecipes(resolvedGridRecipes, gridRecipes, allStacks, fixedRand);


            resolvedIngredientsCache.Clear();
            GridRecipesAndUnnamedIngredients = resolvedGridRecipes.ToArray();


            GridRecipesAndUnnamedIngredients.Shuffle(fixedRand);
            bool extraLine = false;

            for (int i = 0; i < GridRecipesAndUnnamedIngredients.Length; i++)
            {
                string? trait = GridRecipesAndUnnamedIngredients[i]?.Recipe?.RequiresTrait;
                if (trait != null)
                {
                    extraTexts[i] = api.Gui.TextTexture.GenTextTexture(Lang.Get("gridrecipe-requirestrait", Lang.Get("traitname-" + trait)), CairoFont.WhiteDetailText());
                    if (!extraLine) BoundsPerLine[0].Height += GuiElement.scaled(20);
                    extraLine = true;
                }
            }

            if (GridRecipesAndUnnamedIngredients.Length == 0) throw new ArgumentException("Could not resolve any of the supplied grid recipes?");

            GenerateHover();

            stackInfo.onRenderStack = () =>
            {
                GridRecipeAndUnnamedIngredients recipeunin = GridRecipesAndUnnamedIngredients[currentItemIndex];

                double offset = (int)GuiElement.scaled(30 + GuiElementItemstackInfo.ItemStackSize / 2);
                ItemSlot slot = stackInfo.curSlot;
                if (slot?.Itemstack == null)
                {
                    return;
                }
                int stacksize = slot.StackSize;
                slot.Itemstack.StackSize = 1;
                slot.Itemstack.Collectible.OnHandbookRecipeRender(
                    api,
                    recipeunin.Recipe,
                    slot,
                    (int)stackInfo.Bounds.renderX + offset,
                    (int)stackInfo.Bounds.renderY + offset + (int)GuiElement.scaled(GuiElementItemstackInfo.MarginTop),
                    1000 + GuiElement.scaled(GuiElementPassiveItemSlot.unscaledItemSize) * 2,
                    (float)GuiElement.scaled(GuiElementItemstackInfo.ItemStackSize) * 1 / 0.58f
                );
                slot.Itemstack.StackSize = stacksize;
            };
        }

        public ItemStack? GenerateCurrentVisibleOutputStack()
        {
            CurrentVisibleRecipe?.Recipe?.GenerateOutputStack(currentVisibleInputs, currentVisibleOutputSlot);
            return currentVisibleOutputSlot.Itemstack;
        }

        public override EnumCalcBoundsResult CalcBounds(TextFlowPath[] flowPath, double currentLineHeight, double offsetX, double lineY, out double nextOffsetX)
        {
            TextFlowPath currentFlowPath = GetCurrentFlowPathSection(flowPath, lineY);

            offsetX += GuiElement.scaled(PaddingLeft);

            bool requireLineBreak = offsetX + BoundsPerLine[0].Width > currentFlowPath.X2;

            BoundsPerLine[0].X = requireLineBreak ? 0 : offsetX;
            BoundsPerLine[0].Y = lineY + (requireLineBreak ? currentLineHeight + GuiElement.scaled(UnscaledMarginTop) : 0);
            BoundsPerLine[0].Width = 3 * (size + 3) + GuiElement.scaled(PaddingRight);

            nextOffsetX = (requireLineBreak ? 0 : offsetX) + BoundsPerLine[0].Width;

            return requireLineBreak ? EnumCalcBoundsResult.Nextline : EnumCalcBoundsResult.Continue;
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            ctx.SetSourceRGBA(1, 1, 1, 0.2);

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    ctx.Rectangle(BoundsPerLine[0].X + x * (size + GuiElement.scaled(3)), BoundsPerLine[0].Y + y * (size + GuiElement.scaled(3)), size, size);
                    ctx.Fill();
                }
            }
        }

        public override void RenderInteractiveElements(float deltaTime, double renderX, double renderY, double renderZ)
        {
            LineRectangled bounds = BoundsPerLine[0];
            int relx = (int)(api.Input.MouseX - renderX);
            int rely = (int)(api.Input.MouseY - renderY);
            bool mouseover = bounds.PointInside(relx, rely);

            if (!mouseover && (secondsVisible -= deltaTime) <= 0)
            {
                secondsVisible = 1;
                currentItemIndex = (currentItemIndex + 1) % GridRecipesAndUnnamedIngredients.Length;
                secondCounter++;
            }

            GridRecipeAndUnnamedIngredients recipeunin = CurrentVisibleRecipe = GridRecipesAndUnnamedIngredients[currentItemIndex];

            if (extraTexts.TryGetValue(currentItemIndex, out LoadedTexture? extraTextTexture))
            {
                capi.Render.Render2DTexturePremultipliedAlpha(extraTextTexture.TextureId, (float)(renderX + bounds.X), (float)(renderY + bounds.Y + 3 * (size + 3)), extraTextTexture.Width, extraTextTexture.Height);
            }

            double rx = 0, ry = 0;
            int mx = api.Input.MouseX;
            int my = api.Input.MouseY;

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    int index = recipeunin.Recipe.GetGridIndex(y, x, recipeunin.Recipe.ResolvedIngredients, recipeunin.Recipe.Width);

                    rx = renderX + bounds.X + x * (size + GuiElement.scaled(3));
                    ry = renderY + bounds.Y + y * (size + GuiElement.scaled(3));

                    float scale = RuntimeEnv.GUIScale;
                    ElementBounds scissorBounds = ElementBounds.Fixed(rx / scale, ry / scale, size / scale, size / scale).WithEmptyParent();
                    scissorBounds.CalcWorldBounds();

                    if (mouseover)
                    {
                        //capi.Render.Render2DTexture(hoverTexture.TextureId, (float)(scissorBounds.renderX), (float)(scissorBounds.renderY), (float)scissorBounds.InnerWidth, (float)scissorBounds.InnerHeight, (float)renderZ);
                    }

                    CraftingRecipeIngredient? ingred = recipeunin.Recipe.GetElementInGrid(y, x, recipeunin.Recipe.ResolvedIngredients, recipeunin.Recipe.Width);
                    if (ingred == null)
                    {
                        currentVisibleInputs[x + y * 3].Itemstack = null;
                        continue;
                    }

                    api.Render.PushScissor(scissorBounds, true);

                    if (recipeunin.UnnamedIngredients?.TryGetValue(index, out ItemStack[]? unnamedWildcardStacklist) == true && unnamedWildcardStacklist.Length > 0)
                    {
                        dummySlot.Itemstack = unnamedWildcardStacklist[variantDisplaySequence[secondCounter % 30][x, y] % unnamedWildcardStacklist.Length];
                        dummySlot.Itemstack.StackSize = ingred.Quantity;
                        currentVisibleInputs[x + y * 3].Itemstack = dummySlot.Itemstack;
                    }
                    else
                    {
                        dummySlot.Itemstack = ingred.ResolvedItemStack?.Clone();
                        currentVisibleInputs[x + y * 3].Itemstack = dummySlot.Itemstack;
                    }


                    // 1.16.0: Fugly (but backwards compatible) hack: We temporarily store the ingredient code in an unused field of ItemSlot so that OnHandbookRecipeRender() has access to that number. Proper solution would be to alter the method signature to pass on this value.
                    dummySlot.BackgroundIcon = index + "";
                    dummySlot.Itemstack?.Collectible.OnHandbookRecipeRender(capi, recipeunin.Recipe, dummySlot, rx + size * 0.5f, ry + size * 0.5f, 100, size);

                    api.Render.PopScissor();

                    // Super weird coordinates, no idea why
                    double dx = mx - rx + 1;
                    double dy = my - ry + 2;

                    if (dx >= 0 && dx < size && dy >= 0 && dy < size)
                    {
                        RenderItemstackTooltip(dummySlot, rx + dx, ry + dy, deltaTime);
                    }

                    dummySlot.BackgroundIcon = null;
                }
            }
        }

        public override void OnMouseDown(MouseEvent args)
        {
            if (BoundsPerLine == null)
            {
                return;
            }

            GridRecipeAndUnnamedIngredients recipeAndIngredients = GridRecipesAndUnnamedIngredients[currentItemIndex];
            GridRecipe? recipe = recipeAndIngredients.Recipe;

            foreach (LineRectangled? val in BoundsPerLine)
            {
                if (val.PointInside(args.X, args.Y))
                {
                    int x = (int)((args.X - val.X) / (size + 3));
                    int y = (int)((args.Y - val.Y) / (size + 3));

                    CraftingRecipeIngredient? ingredient = recipe?.GetElementInGrid(y, x, recipe.ResolvedIngredients ?? [], recipe.Width);
                    if (recipe == null || ingredient == null) return;

                    int index = recipe.GetGridIndex(y, x, recipe.ResolvedIngredients ?? [], recipe.Width);
                    if (recipeAndIngredients.UnnamedIngredients?.TryGetValue(index, out ItemStack[]? unnamedWildcardStacklist) == true)
                    {
                        onStackClicked?.Invoke(unnamedWildcardStacklist[variantDisplaySequence[secondCounter % 30][x, y] % unnamedWildcardStacklist.Length]);
                    }
                    else
                    {
                        onStackClicked?.Invoke(ingredient.ResolvedItemStack);
                    }
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            foreach (KeyValuePair<int, LoadedTexture> val in extraTexts)
            {
                val.Value.Dispose();
            }

            hoverTexture?.Dispose();
        }



        private void GenerateHover()
        {
            ImageSurface surface = new(Format.Argb32, 1, 1);
            Context context = new(surface);

            context.SetSourceRGBA(1, 1, 1, 0.6);
            context.Paint();

            hoverTexture = new LoadedTexture(capi);
            api.Gui.LoadOrUpdateCairoTexture(surface, false, ref hoverTexture);

            context.Dispose();
            surface.Dispose();
        }

        private string GenerateCacheKey(IRecipeIngredient ingredient)
        {
            string tags = ingredient.ResolvedTags?.Any() == true ?
                ingredient.ResolvedTags
                    .Select(element => $"{element.Tags.GetHashCode()}-{element.InvertedTags.GetHashCode()}")
                    .Aggregate((f, s) => $"{f}:{s}") :
                "-";
            return $"{ingredient.Code}:{tags}";
        }

        private ItemStack[] MatchIngredients(IWorldAccessor world, IRecipeIngredient ingredient, ItemStack[]? allStacks = null)
        {
            string cacheKey = GenerateCacheKey(ingredient);

            if (resolvedIngredientsCache.TryGetValue(cacheKey, out ItemStack[]? result))
            {
                return result;
            }

            ItemStack[] matches;

            if (allStacks != null)
            {
                matches = allStacks
                    .Where(stack => ingredient.SatisfiesAsIngredient(stack, checkStackSize: false))
                    .ToArray();
            }
            else
            {
                matches = world.Collectibles
                    .Select(collectible => new ItemStack(collectible, ingredient.Quantity))
                    .Where(stack => ingredient.SatisfiesAsIngredient(stack, checkStackSize: false))
                    .ToArray();
            }

            if (matches.Length > 0)
            {
                resolvedIngredientsCache[cacheKey] = matches;
            }

            return matches;
        }

        private void ResolveRecipes(List<GridRecipeAndUnnamedIngredients> resolvedGridRecipes, GridRecipe[] gridRecipes, ItemStack[]? allStacks, Random fixedRand)
        {
            Queue<GridRecipe> halfResolvedRecipes = new(gridRecipes);

            bool allResolved = false;
            while (!allResolved)
            {
                allResolved = true;

                int recipesToResolveCount = halfResolvedRecipes.Count;

                while (recipesToResolveCount-- > 0)
                {
                    GridRecipe toTestRecipe = halfResolvedRecipes.Dequeue();
                    Dictionary<int, ItemStack[]>? unnamedIngredients = null;

                    bool thisResolved = ResolveIngredients(ref unnamedIngredients, halfResolvedRecipes, toTestRecipe, ref allResolved, allStacks, fixedRand);

                    if (thisResolved)
                    {
                        resolvedGridRecipes.Add(new GridRecipeAndUnnamedIngredients() { Recipe = toTestRecipe, UnnamedIngredients = unnamedIngredients });
                    }
                }
            }
        }

        private bool ResolveIngredients(ref Dictionary<int, ItemStack[]>? unnamedIngredients, Queue<GridRecipe> halfResolvedRecipes, GridRecipe toTestRecipe, ref bool allResolved, ItemStack[]? allStacks, Random fixedRand)
        {
            bool thisResolved = true;

            if (toTestRecipe.ResolvedIngredients == null)
            {
                return false;
            }

            for (int j = 0; j < toTestRecipe.ResolvedIngredients.Length; j++)
            {
                CraftingRecipeIngredient? ingredient = toTestRecipe.ResolvedIngredients[j];

                if (ingredient == null || ingredient.MatchingType == EnumRecipeMatchType.Exact)
                {
                    continue;
                }

                allResolved = false;
                thisResolved = false;

                ItemStack[] stacks = MatchIngredients(api.World, ingredient, allStacks);
                if (stacks.Length == 0)
                {
                    resolvedIngredientsCache.Remove(ingredient.Code);
                    stacks = MatchIngredients(api.World, ingredient, null);
                    if (stacks.Length == 0)
                    {
                        throw new ArgumentException("Attempted to resolve the recipe ingredient wildcard " + ingredient.Type + " " + ingredient.Code + " but there are no such items/blocks!");
                    }
                }

                if (ingredient.Name == null)
                {
                    unnamedIngredients ??= [];
                    unnamedIngredients[j] = ((ItemStack[])stacks.Clone()).Shuffle(fixedRand);
                    thisResolved = true;
                    continue;
                }

                for (int k = 0; k < stacks.Length; k++)
                {
                    GridRecipe cloned = toTestRecipe.Clone();

                    if (cloned.ResolvedIngredients == null)
                    {
                        continue;
                    }

                    for (int m = 0; m < cloned.ResolvedIngredients.Length; m++)
                    {
                        CraftingRecipeIngredient? clonedIngredient = cloned.ResolvedIngredients[m];
                        if (clonedIngredient != null && clonedIngredient.Code?.Equals(ingredient.Code) == true)
                        {
                            clonedIngredient.Code = stacks[k].Collectible.Code;
                            clonedIngredient.MatchingType = EnumRecipeMatchType.Exact;
                            clonedIngredient.ResolvedItemStack = stacks[k];
                        }
                    }

                    halfResolvedRecipes.Enqueue(cloned);
                }
            }

            return thisResolved;
        }
    }
}
