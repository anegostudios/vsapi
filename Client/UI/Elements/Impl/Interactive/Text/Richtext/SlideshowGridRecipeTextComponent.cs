using Cairo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

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

            this.onStackClicked = onStackClicked;
            this.Float = floatType;
            this.BoundsPerLine = new LineRectangled[] { new LineRectangled(0, 0, 3 * (size + 3), 3 * (size + 3)) };
            this.size = size;

            Random fixedRand = new Random(123);

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

                            if (ingred.Name == null)
                            {
                                if (unnamedIngredients == null) unnamedIngredients = new Dictionary<int, ItemStack[]>();
                                unnamedIngredients[j] = ((ItemStack[])stacks.Clone()).Shuffle(fixedRand);
                                thisResolved = true;
                                continue;
                            }

                            if (stacks.Length == 0)
                            {
                                throw new ArgumentException("Attempted to resolve the recipe ingredient wildcard " + ingred.Type + " " + ingred.Code + " but there are no such items/blocks!");
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

            for (int i = 0; i < GridRecipesAndUnIn.Length; i++)
            {
                string trait = GridRecipesAndUnIn[i].Recipe.RequiresTrait;
                if (trait != null)
                {
                    extraTexts[i] = capi.Gui.TextTexture.GenTextTexture(Lang.Get("* Requires {0} trait", trait), CairoFont.WhiteDetailText());
                }
            }

            if (GridRecipesAndUnIn.Length == 0) throw new ArgumentException("Could not resolve any of the supplied grid recipes?");
        }

        

        ItemStack[] ResolveWildCard(IWorldAccessor world, CraftingRecipeIngredient ingred, ItemStack[] allStacks = null)
        {
            if (resolveCache.ContainsKey(ingred.Code)) return resolveCache[ingred.Code];

            List<ItemStack> matches = new List<ItemStack>();

            if (allStacks != null)
            {
                foreach (var val in allStacks)
                {
                    if (val.Collectible.Code == null) continue;
                    if (val.Class != ingred.Type) continue;
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



        public override bool CalcBounds(TextFlowPath[] flowPath, double currentLineHeight, double lineX, double lineY)
        {
            TextFlowPath curfp = GetCurrentFlowPathSection(flowPath, lineY);
            bool requireLinebreak = lineX + BoundsPerLine[0].Width > curfp.X2;

            this.BoundsPerLine[0].X = requireLinebreak ? 0 : lineX;
            this.BoundsPerLine[0].Y = lineY + (requireLinebreak ? currentLineHeight + GuiElement.scaled(UnscaledMarginTop) : 0);

            return requireLinebreak;
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

        public override void RenderInteractiveElements(float deltaTime, double renderX, double renderY)
        {
            LineRectangled bounds = BoundsPerLine[0];

            GridRecipeAndUnnamedIngredients recipeunin = GridRecipesAndUnIn[curItemIndex];

            if ((secondsVisible -= deltaTime) <= 0)
            {
                secondsVisible = 1;
                curItemIndex = (curItemIndex + 1) % GridRecipesAndUnIn.Length;
                secondCounter++;
            }

            LoadedTexture extraTextTexture;
            if (extraTexts.TryGetValue(curItemIndex, out extraTextTexture))
            {
                capi.Render.Render2DTexturePremultipliedAlpha(extraTextTexture.TextureId, (float)(renderX + bounds.X), (float)(renderY + bounds.Y + 3 * (size + 3)), extraTextTexture.Width, extraTextTexture.Height);
            }

            int mx = api.Input.MouseX;
            int my = api.Input.MouseY;
            double rx=0, ry=0;

            

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    int index = recipeunin.Recipe.GetGridIndex(y, x, recipeunin.Recipe.resolvedIngredients, recipeunin.Recipe.Width);

                    CraftingRecipeIngredient ingred = recipeunin.Recipe.GetElementInGrid(y, x, recipeunin.Recipe.resolvedIngredients, recipeunin.Recipe.Width);


                    if (ingred == null) continue;

                    rx = renderX + bounds.X + x * (size + GuiElement.scaled(3));
                    ry = renderY + bounds.Y + y * (size + GuiElement.scaled(3));

                    ItemStack[] unnamedWildcardStacklist = null;
                    if (recipeunin.unnamedIngredients?.TryGetValue(index, out unnamedWildcardStacklist) == true)
                    {
                        dummyslot.Itemstack = unnamedWildcardStacklist[secondCounter % unnamedWildcardStacklist.Length];
                        dummyslot.Itemstack.StackSize = ingred.Quantity;
                    } else
                    {
                        dummyslot.Itemstack = ingred.ResolvedItemstack.Clone();
                    }

                    var scale = RuntimeEnv.GUIScale;
                    ElementBounds scissorBounds = ElementBounds.Fixed(rx / scale, ry / scale, size / scale, size / scale).WithEmptyParent();
                    scissorBounds.CalcWorldBounds();
                    api.Render.PushScissor(scissorBounds, true);

                    // 1.16.0: Fugly (but backwards compatible) hack: We temporarily store the ingredient code in an unused field of ItemSlot so that OnHandbookRecipeRender() has access to that number. Proper solution would be to alter the method signature to pass on this value.
                    dummyslot.BackgroundIcon = index + "";
                    dummyslot.Itemstack.Collectible.OnHandbookRecipeRender(capi, recipeunin.Recipe, dummyslot, rx + size * 0.5f, ry + size * 0.5f, size);
                    dummyslot.BackgroundIcon = null;


                    api.Render.PopScissor();

                    // Super weird coordinates, no idea why
                    double dx = mx - rx + 1;
                    double dy = my - ry + 2;

                    if (dx >= 0 && dx < size && dy >= 0 && dy < size)
                    {
                        RenderItemstackTooltip(dummyslot, rx + dx, ry + dy, deltaTime);
                    }
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
                    ItemStack[] unnamedWildcardStacklist = null;
                    if (recipeunin.unnamedIngredients?.TryGetValue(index, out unnamedWildcardStacklist) == true)
                    {
                        onStackClicked?.Invoke(unnamedWildcardStacklist[secondCounter % unnamedWildcardStacklist.Length]);
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
        }

    }
}
