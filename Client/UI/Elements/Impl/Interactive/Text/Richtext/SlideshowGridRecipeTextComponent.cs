using Cairo;
using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Client
{
    public class StackAndWildCard
    {
        public ItemStack Stack;
        public AssetLocation WildCard;
    }

    public class GridRecipeAndUnnamedIngredients
    {
        public GridRecipe Recipe;
        public Dictionary<int, ItemStack[]> unnamedIngredients;
    }

    

    /// <summary>
    /// Draws multiple itemstacks
    /// </summary>
    public class SlideshowGridRecipeTextComponent : ItemstackComponentBase
    {
        public GridRecipeAndUnnamedIngredients[] GridRecipesAndUnIn;

        Action<ItemStack> onStackClicked;
        ItemSlot dummyslot = new DummySlot();

        double size;

        float secondsVisible = 1;
        int curItemIndex;
        Dictionary<AssetLocation, ItemStack[]> resolveCache = new Dictionary<AssetLocation, ItemStack[]>();

        Dictionary<int, LoadedTexture> extraTexts = new Dictionary<int, LoadedTexture>();

        LoadedTexture hoverTexture;

        public GridRecipeAndUnnamedIngredients CurrentVisibleRecipe;

        static int[][,] variantDisplaySequence = new int[30][,];
        

        /// <summary>
        /// Flips through given array of grid recipes every second
        /// </summary>
        /// <param name="capi"></param>
        /// <param name="gridrecipes"></param>
        /// <param name="size"></param>
        /// <param name="floatType"></param>
        /// <param name="onStackClicked"></param>
        /// <param name="allStacks">If set, will resolve wildcards based on this list, otherwise will search all available blocks/items</param>
        public SlideshowGridRecipeTextComponent(ICoreClientAPI capi, GridRecipe[] gridrecipes, double size, EnumFloat floatType, Action<ItemStack> onStackClicked = null, ItemStack[] allStacks = null) : base(capi)
        {
            size = GuiElement.scaled(size);
            var innerMargin = GuiElement.scaled(3);

            this.onStackClicked = onStackClicked;
            this.Float = floatType;
            this.BoundsPerLine = [new(0, 0, 2 * (size + innerMargin) + size, 2 * (size + innerMargin) + size)];
            this.size = size;

            Random fixedRand = new Random(123);

            for (int i = 0; i < 30; i++)
            {
                var sq = variantDisplaySequence[i] = new int[3, 3];
                for (int x = 0; x < 3; x++)
                    for (int y = 0; y < 3; y++)
                        sq[x, y] = capi.World.Rand.Next(99999);
            };

            // Expand wild cards
            List<GridRecipeAndUnnamedIngredients> resolvedGridRecipes = new List<GridRecipeAndUnnamedIngredients>();
            Queue<GridRecipe> halfResolvedRecipes = new Queue<GridRecipe>(gridrecipes);

            bool allResolved = false;

            while (!allResolved)
            {
                allResolved = true;

                int cnt = halfResolvedRecipes.Count;

                while (cnt-- > 0)
                {
                    GridRecipe toTestRecipe = halfResolvedRecipes.Dequeue();
                    Dictionary<int, ItemStack[]> unnamedIngredients=null;

                    bool thisResolved = true;

                    for (int j = 0; j < toTestRecipe.resolvedIngredients.Length; j++)
                    {
                        CraftingRecipeIngredient ingred = toTestRecipe.resolvedIngredients[j];

                        if (ingred != null && ingred.IsWildCard)
                        {
                            allResolved = false;
                            thisResolved = false;
                            ItemStack[] stacks = ResolveWildCard(capi.World, ingred, allStacks);
                            if (stacks.Length == 0)
                            {
                                resolveCache.Remove(ingred.Code);
                                stacks = ResolveWildCard(capi.World, ingred, null);
                                if (stacks.Length == 0)
                                {
                                    throw new ArgumentException("Attempted to resolve the recipe ingredient wildcard " + ingred.Type + " " + ingred.Code + " but there are no such items/blocks!");
                                }
                            }

                            if (ingred.Name == null)
                            {
                                if (unnamedIngredients == null) unnamedIngredients = new Dictionary<int, ItemStack[]>();
                                unnamedIngredients[j] = ((ItemStack[])stacks.Clone()).Shuffle(fixedRand);
                                thisResolved = true;
                                continue;
                            }

                            for (int k = 0; k < stacks.Length; k++)
                            {
                                GridRecipe cloned = toTestRecipe.Clone();

                                for (int m = 0; m < cloned.resolvedIngredients.Length; m++)
                                {
                                    CraftingRecipeIngredient clonedingred = cloned.resolvedIngredients[m];
                                    if (clonedingred != null && clonedingred.Code.Equals(ingred.Code))
                                    {
                                        clonedingred.Code = stacks[k].Collectible.Code;
                                        clonedingred.IsWildCard = false;
                                        clonedingred.ResolvedItemstack = stacks[k];
                                    }
                                }

                                halfResolvedRecipes.Enqueue(cloned);
                            }

                            break;
                        }
                    }

                    if (thisResolved)
                    {
                        resolvedGridRecipes.Add(new GridRecipeAndUnnamedIngredients() { Recipe = toTestRecipe, unnamedIngredients = unnamedIngredients });
                    }
                }
            }

            resolveCache.Clear();
            this.GridRecipesAndUnIn = resolvedGridRecipes.ToArray();

            
            this.GridRecipesAndUnIn.Shuffle(fixedRand);
            bool extraline = false;

            for (int i = 0; i < GridRecipesAndUnIn.Length; i++)
            {
                string trait = GridRecipesAndUnIn[i].Recipe.RequiresTrait;
                if (trait != null)
                {
                    extraTexts[i] = capi.Gui.TextTexture.GenTextTexture(Lang.Get("gridrecipe-requirestrait", Lang.Get("traitname-" + trait)), CairoFont.WhiteDetailText());
                    if (!extraline) BoundsPerLine[0].Height += GuiElement.scaled(20);
                    extraline = true;
                }
            }

            if (GridRecipesAndUnIn.Length == 0) throw new ArgumentException("Could not resolve any of the supplied grid recipes?");

            genHover();

            stackInfo.onRenderStack = () =>
            {
                GridRecipeAndUnnamedIngredients recipeunin = GridRecipesAndUnIn[curItemIndex];

                double offset = (int)GuiElement.scaled(30 + GuiElementItemstackInfo.ItemStackSize / 2);
                var slot = stackInfo.curSlot;
                var stacksize = slot.StackSize;
                slot.Itemstack.StackSize = 1;
                slot.Itemstack.Collectible.OnHandbookRecipeRender(
                    capi,
                    recipeunin.Recipe,
                    slot,
                    (int)stackInfo.Bounds.renderX + offset,
                    (int)stackInfo.Bounds.renderY + offset + (int)GuiElement.scaled(GuiElementItemstackInfo.MarginTop),
                    1000 + GuiElement.scaled(GuiElementPassiveItemSlot.unscaledItemSize) * 2,
                    (float)GuiElement.scaled(GuiElementItemstackInfo.ItemStackSize) * 1/0.58f
                );
                slot.Itemstack.StackSize = stacksize;
            };
        }

        private void genHover()
        {
            ImageSurface surface = new ImageSurface(Format.Argb32, 1, 1);
            Context ctx = new Context(surface);

            ctx.SetSourceRGBA(1, 1, 1, 0.6);
            ctx.Paint();

            hoverTexture = new LoadedTexture(capi);
            api.Gui.LoadOrUpdateCairoTexture(surface, false, ref hoverTexture);

            ctx.Dispose();
            surface.Dispose();
        }

        ItemStack[] ResolveWildCard(IWorldAccessor world, CraftingRecipeIngredient ingred, ItemStack[] allStacks = null)
        {
            if (resolveCache.ContainsKey(ingred.Code)) return resolveCache[ingred.Code];

            List<ItemStack> matches = new List<ItemStack>();

            if (allStacks != null)
            {
                foreach (var val in allStacks)
                {
                    if (val.Class != ingred.Type) continue;
                    if (val.Collectible.Code == null) continue;
                    if (WildcardUtil.Match(ingred.Code, val.Collectible.Code, ingred.AllowedVariants)) matches.Add(new ItemStack(val.Collectible, ingred.Quantity));
                }

                resolveCache[ingred.Code] = matches.ToArray();
                return matches.ToArray();
            }


            foreach (var val in world.Collectibles)
            {
                if (WildcardUtil.Match(ingred.Code, val.Code, ingred.AllowedVariants)) matches.Add(new ItemStack(val, ingred.Quantity));
            }

            resolveCache[ingred.Code] = matches.ToArray();

            return resolveCache[ingred.Code];
        }



        public override EnumCalcBoundsResult CalcBounds(TextFlowPath[] flowPath, double currentLineHeight, double offsetX, double lineY, out double nextOffsetX)
        {
            TextFlowPath curfp = GetCurrentFlowPathSection(flowPath, lineY);

            offsetX += GuiElement.scaled(PaddingLeft);

            bool requireLinebreak = offsetX + BoundsPerLine[0].Width > curfp.X2;

            this.BoundsPerLine[0].X = requireLinebreak ? 0 : offsetX;
            this.BoundsPerLine[0].Y = lineY + (requireLinebreak ? currentLineHeight + GuiElement.scaled(UnscaledMarginTop) : 0);

            BoundsPerLine[0].Width = 3 * (size + 3) + GuiElement.scaled(PaddingRight);

            nextOffsetX = (requireLinebreak ? 0 : offsetX) + BoundsPerLine[0].Width;

            return requireLinebreak ? EnumCalcBoundsResult.Nextline : EnumCalcBoundsResult.Continue;
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            ctx.SetSourceRGBA(1, 1, 1, 0.2);

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    ctx.Rectangle(BoundsPerLine[0].X + x*(size+GuiElement.scaled(3)), BoundsPerLine[0].Y + y*(size+ GuiElement.scaled(3)), size, size);
                    ctx.Fill();
                }
            }
        }

        int secondCounter = 0;

        public override void RenderInteractiveElements(float deltaTime, double renderX, double renderY, double renderZ)
        {
            LineRectangled bounds = BoundsPerLine[0];
            int relx = (int)(api.Input.MouseX - renderX);
            int rely = (int)(api.Input.MouseY - renderY);
            bool mouseover = bounds.PointInside(relx, rely);

            if (!mouseover && (secondsVisible -= deltaTime) <= 0)
            {
                secondsVisible = 1;
                curItemIndex = (curItemIndex + 1) % GridRecipesAndUnIn.Length;
                secondCounter++;
            }

            GridRecipeAndUnnamedIngredients recipeunin = CurrentVisibleRecipe = GridRecipesAndUnIn[curItemIndex];

            if (extraTexts.TryGetValue(curItemIndex, out LoadedTexture extraTextTexture))
            {
                capi.Render.Render2DTexturePremultipliedAlpha(extraTextTexture.TextureId, (float)(renderX + bounds.X), (float)(renderY + bounds.Y + 3 * (size + 3)), extraTextTexture.Width, extraTextTexture.Height);
            }

            double rx=0, ry=0;
            int mx = api.Input.MouseX;
            int my = api.Input.MouseY;

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    int index = recipeunin.Recipe.GetGridIndex(y, x, recipeunin.Recipe.resolvedIngredients, recipeunin.Recipe.Width);

                    rx = renderX + bounds.X + x * (size + GuiElement.scaled(3));
                    ry = renderY + bounds.Y + y * (size + GuiElement.scaled(3));

                    var scale = RuntimeEnv.GUIScale;
                    ElementBounds scissorBounds = ElementBounds.Fixed(rx / scale, ry / scale, size / scale, size / scale).WithEmptyParent();
                    scissorBounds.CalcWorldBounds();

                    if (mouseover)
                    {
                        //capi.Render.Render2DTexture(hoverTexture.TextureId, (float)(scissorBounds.renderX), (float)(scissorBounds.renderY), (float)scissorBounds.InnerWidth, (float)scissorBounds.InnerHeight, (float)renderZ);
                    }

                    CraftingRecipeIngredient ingred = recipeunin.Recipe.GetElementInGrid(y, x, recipeunin.Recipe.resolvedIngredients, recipeunin.Recipe.Width);
                    if (ingred == null) continue;

                    api.Render.PushScissor(scissorBounds, true);

                    if (recipeunin.unnamedIngredients?.TryGetValue(index, out ItemStack[] unnamedWildcardStacklist) == true && unnamedWildcardStacklist.Length > 0)
                    {
                        dummyslot.Itemstack = unnamedWildcardStacklist[variantDisplaySequence[secondCounter % 30][x, y] % unnamedWildcardStacklist.Length];
                        dummyslot.Itemstack.StackSize = ingred.Quantity;
                    }
                    else
                    {
                        dummyslot.Itemstack = ingred.ResolvedItemstack.Clone();
                    }


                    // 1.16.0: Fugly (but backwards compatible) hack: We temporarily store the ingredient code in an unused field of ItemSlot so that OnHandbookRecipeRender() has access to that number. Proper solution would be to alter the method signature to pass on this value.
                    dummyslot.BackgroundIcon = index + "";
                    dummyslot.Itemstack.Collectible.OnHandbookRecipeRender(capi, recipeunin.Recipe, dummyslot, rx + size * 0.5f, ry + size * 0.5f, 100, size);
                    
                    api.Render.PopScissor();

                    // Super weird coordinates, no idea why
                    double dx = mx - rx + 1;
                    double dy = my - ry + 2;

                    if (dx >= 0 && dx < size && dy >= 0 && dy < size)
                    {
                        RenderItemstackTooltip(dummyslot, rx + dx, ry + dy, deltaTime);
                    }

                    dummyslot.BackgroundIcon = null;
                }
            }
        }


        public override void OnMouseDown(MouseEvent args)
        {
            GridRecipeAndUnnamedIngredients recipeunin = GridRecipesAndUnIn[curItemIndex];
            GridRecipe recipe = recipeunin.Recipe;

            foreach (var val in BoundsPerLine)
            {
                if (val.PointInside(args.X, args.Y))
                {
                    int x = (int)((args.X - val.X) / (size + 3));
                    int y = (int)((args.Y - val.Y) / (size + 3));

                    CraftingRecipeIngredient ingred = recipe.GetElementInGrid(y, x, recipe.resolvedIngredients, recipe.Width);
                    if (ingred == null) return;

                    int index = recipe.GetGridIndex(y, x, recipe.resolvedIngredients, recipe.Width);
                    if (recipeunin.unnamedIngredients?.TryGetValue(index, out ItemStack[] unnamedWildcardStacklist) == true)
                    {
                        onStackClicked?.Invoke(unnamedWildcardStacklist[variantDisplaySequence[secondCounter % 30][x, y] % unnamedWildcardStacklist.Length]);
                    }
                    else
                    {
                        onStackClicked?.Invoke(ingred.ResolvedItemstack);
                    }
                }
            }
        }


        public override void Dispose()
        {
            base.Dispose();

            foreach (var val in extraTexts)
            {
                val.Value.Dispose();
            }

            hoverTexture?.Dispose();
        }

    }
}
