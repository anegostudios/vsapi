using Cairo;
using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{


    public delegate ItemStack StackDisplayDelegate();

    /// <summary>
    /// Draws multiple itemstacks
    /// </summary>
    public class SlideshowItemstackTextComponent : ItemstackComponentBase
    {
        public bool ShowTooltip = true;

        public ItemStack[] Itemstacks;
        protected ItemSlot slot;

        protected Action<ItemStack> onStackClicked;

        protected float secondsVisible =1;
        protected int curItemIndex;

        public bool ShowStackSize { get; set; }
        public bool Background { get; set; } = false;

        public string ExtraTooltipText;

        public Vec3f renderOffset = new Vec3f();

        public float renderSize = 0.58f;

        double unscaledSize;

        public StackDisplayDelegate overrideCurrentItemStack;

        /// <summary>
        /// Flips through given array of item stacks every second
        /// </summary>
        /// <param name="capi"></param>
        /// <param name="itemstacks"></param>
        /// <param name="unscaledSize"></param>
        /// <param name="floatType"></param>
        /// <param name="onStackClicked"></param>
        public SlideshowItemstackTextComponent(ICoreClientAPI capi, ItemStack[] itemstacks, double unscaledSize, EnumFloat floatType, Action<ItemStack> onStackClicked = null) : base(capi)
        {
            initSlot();

            this.unscaledSize = unscaledSize;
            this.Itemstacks = itemstacks;
            this.Float = floatType;
            this.BoundsPerLine = new LineRectangled[] { new LineRectangled(0, 0, GuiElement.scaled(unscaledSize), GuiElement.scaled(unscaledSize)) };
            this.onStackClicked = onStackClicked;
        }

        /// <summary>
        /// Looks at the collectibles handbook groupBy attribute and makes a list of itemstacks from that
        /// </summary>
        /// <param name="capi"></param>
        /// <param name="itemstackgroup"></param>
        /// <param name="allstacks"></param>
        /// <param name="unscaleSize"></param>
        /// <param name="floatType"></param>
        /// <param name="onStackClicked"></param>
        public SlideshowItemstackTextComponent(ICoreClientAPI capi, ItemStack itemstackgroup, List<ItemStack> allstacks, double unscaleSize, EnumFloat floatType, Action<ItemStack> onStackClicked = null) : base(capi)
        {
            initSlot();
            this.onStackClicked = onStackClicked;
            this.unscaledSize = unscaleSize;

            string[] groups = itemstackgroup.Collectible.Attributes?["handbook"]?["groupBy"]?.AsArray<string>(null);

            List<ItemStack> nowGroupedStacks = new List<ItemStack>();
            List<ItemStack> stacks = new List<ItemStack>();

            nowGroupedStacks.Add(itemstackgroup);
            stacks.Add(itemstackgroup);

            if (allstacks != null)
            {
                if (groups != null)
                {
                    AssetLocation[] groupWildCards = new AssetLocation[groups.Length];
                    for (int i = 0; i < groups.Length; i++)
                    {
                        if (!groups[i].Contains(":"))
                        {
                            groupWildCards[i] = new AssetLocation(itemstackgroup.Collectible.Code.Domain, groups[i]);
                        }
                        else
                        {
                            groupWildCards[i] = new AssetLocation(groups[i]);
                        }

                    }

                    foreach (var val in allstacks)
                    {
                        if (val.Collectible.Attributes?["handbook"]?["isDuplicate"].AsBool(false) == true)
                        {
                            nowGroupedStacks.Add(val);
                            continue;
                        }
                        for (int i = 0; i < groupWildCards.Length; i++)
                        {
                            if (val.Collectible.WildCardMatch(groupWildCards[i]))
                            {
                                stacks.Add(val);
                                nowGroupedStacks.Add(val);
                                break;
                            }
                        }
                    }
                }

                foreach (var val in nowGroupedStacks)
                {
                    allstacks.Remove(val);
                }
            }

            this.Itemstacks = stacks.ToArray();
            this.Float = floatType;
            this.BoundsPerLine = new LineRectangled[] { new LineRectangled(0, 0, GuiElement.scaled(unscaleSize), GuiElement.scaled(unscaleSize)) };
        }



        void initSlot()
        {
            dummyInv = new DummyInventory(capi);
            dummyInv.OnAcquireTransitionSpeed += (transType, stack, mul) =>
            {
                return 0;
            };
            slot = new DummySlot(null, dummyInv);
        }

        public override EnumCalcBoundsResult CalcBounds(TextFlowPath[] flowPath, double currentLineHeight, double offsetX, double lineY, out double nextOffsetX)
        {
            TextFlowPath curfp = GetCurrentFlowPathSection(flowPath, lineY);
            offsetX += GuiElement.scaled(PaddingLeft);

            bool requireLinebreak = offsetX + BoundsPerLine[0].Width > curfp.X2;

            this.BoundsPerLine[0].X = requireLinebreak ? 0 : offsetX;
            this.BoundsPerLine[0].Y = lineY + (requireLinebreak ? currentLineHeight : 0);

            BoundsPerLine[0].Width = GuiElement.scaled(unscaledSize) + GuiElement.scaled(PaddingRight);

            nextOffsetX = (requireLinebreak ? 0 : offsetX) + BoundsPerLine[0].Width;

            return requireLinebreak ? EnumCalcBoundsResult.Nextline : EnumCalcBoundsResult.Continue;
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            if (Background)
            {
                ctx.SetSourceRGBA(1, 1, 1, 0.2);
                ctx.Rectangle(
                    BoundsPerLine[0].X, 
                    BoundsPerLine[0].Y,  /* - BoundsPerLine[0].Ascent / 2 */ /* why /2??? */ /* why this ascent at all???? wtf? */
                    BoundsPerLine[0].Width, 
                    BoundsPerLine[0].Height
                );
                ctx.Fill();
            }
        }

        protected override string OnRequireInfoText(ItemSlot slot)
        {
            return base.OnRequireInfoText(slot) + ExtraTooltipText;
        }

        public override void RenderInteractiveElements(float deltaTime, double renderX, double renderY, double renderZ)
        {
            int relx = (int)(api.Input.MouseX - renderX + renderOffset.X);
            int rely = (int)(api.Input.MouseY - renderY + renderOffset.Y);
            LineRectangled bounds = BoundsPerLine[0];
            bool mouseover = bounds.PointInside(relx, rely);

            ItemStack itemstack = Itemstacks[curItemIndex];

            if (!mouseover && (secondsVisible -= deltaTime) <= 0)
            {
                secondsVisible = 1;
                curItemIndex = (curItemIndex + 1) % Itemstacks.Length;
            }

            if (overrideCurrentItemStack != null) itemstack = overrideCurrentItemStack();

            slot.Itemstack = itemstack;

            ElementBounds scibounds = ElementBounds.FixedSize((int)(bounds.Width / RuntimeEnv.GUIScale), (int)(bounds.Height / RuntimeEnv.GUIScale));
            scibounds.ParentBounds = capi.Gui.WindowBounds;
            
            scibounds.CalcWorldBounds();
            scibounds.absFixedX = renderX + bounds.X + renderOffset.X;
            scibounds.absFixedY = renderY + bounds.Y + renderOffset.Y /*- BoundsPerLine[0].Ascent / 2 - why???? */;
            scibounds.absInnerWidth *= renderSize / 0.58f;
            scibounds.absInnerHeight *= renderSize / 0.58f;

            api.Render.PushScissor(scibounds, true);

            api.Render.RenderItemstackToGui(slot, 
                renderX + bounds.X + bounds.Width * 0.5f + renderOffset.X + offX, 
                renderY + bounds.Y + bounds.Height * 0.5f + renderOffset.Y + offY /*- BoundsPerLine[0].Ascent / 2 - why?????*/, 
                100 + renderOffset.Z, (float)bounds.Width * renderSize, 
                ColorUtil.WhiteArgb, true, false, ShowStackSize
            );

            api.Render.PopScissor();


            if (mouseover && ShowTooltip)
            {
                RenderItemstackTooltip(slot, renderX + relx, renderY + rely, deltaTime);
            }
        }

        public override void OnMouseDown(MouseEvent args)
        {
            if (slot.Itemstack == null) return;

            foreach (var val in BoundsPerLine)
            {
                if (val.PointInside(args.X, args.Y))
                {
                    onStackClicked?.Invoke(slot.Itemstack);
                }
            }
        }


    }
}
