using Cairo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Draws multiple itemstacks
    /// </summary>
    public class SlideshowItemstackTextComponent : ItemstackComponentBase
    {
        public ItemStack[] Itemstacks;
        ItemSlot slot;
        
        Common.Action<ItemStack> onStackClicked;

        float secondsVisible=1;
        int curItemIndex;

        public bool ShowStackSize { get; set; }

        /// <summary>
        /// Flips through given array of item stacks every second
        /// </summary>
        /// <param name="itemstacks"></param>
        /// <param name="size"></param>
        /// <param name="floatType"></param>
        public SlideshowItemstackTextComponent(ICoreClientAPI capi, ItemStack[] itemstacks, double size, EnumFloat floatType, Common.Action<ItemStack> onStackClicked = null) : base(capi)
        {
            initSlot();

            this.Itemstacks = itemstacks;
            this.Float = floatType;
            this.BoundsPerLine = new LineRectangled[] { new LineRectangled(0, 0, size, size) };
            this.onStackClicked = onStackClicked;
        }

        /// <summary>
        /// Looks at the collectibles handbook groupBy attribute and makes a list of itemstacks from that
        /// </summary>
        /// <param name="itemstackgroup"></param>
        /// <param name="size"></param>
        /// <param name="floatType"></param>
        public SlideshowItemstackTextComponent(ICoreClientAPI capi, ItemStack itemstackgroup, List<ItemStack> allstacks, double size, EnumFloat floatType, Common.Action<ItemStack> onStackClicked = null) : base(capi)
        {
            initSlot();
            this.onStackClicked = onStackClicked;

            string[] groups = itemstackgroup.Collectible.Attributes?["handbook"]?["groupBy"]?.AsArray<string>(null);

            List<ItemStack> nowGroupedStacks = new List<ItemStack>();
            List<ItemStack> stacks = new List<ItemStack>();

            nowGroupedStacks.Add(itemstackgroup);
            stacks.Add(itemstackgroup);

            if (groups != null)
            {
                AssetLocation[] groupWildCards = new AssetLocation[groups.Length];
                for (int i = 0; i < groups.Length; i++)
                {
                    if (!groups[i].Contains(":"))
                    {
                        groupWildCards[i] = new AssetLocation(itemstackgroup.Collectible.Code.Domain, groups[i]);
                    } else
                    {
                        groupWildCards[i] = new AssetLocation(groups[i]);
                    }
                    
                }

                foreach (var val in allstacks)
                {
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

            this.Itemstacks = stacks.ToArray();
            this.Float = floatType;
            this.BoundsPerLine = new LineRectangled[] { new LineRectangled(0, 0, size, size) };
            //PaddingRight = 0;
        }



        void initSlot()
        {
            dummyInv = new DummyInventory(capi);
            dummyInv.OnAcquireTransitionSpeed = (transType, stack, mul) =>
            {
                return 0;
            };
            slot = new DummySlot(null, dummyInv);
        }

        public override bool CalcBounds(TextFlowPath[] flowPath, double currentLineHeight, double lineX, double lineY)
        {
            TextFlowPath curfp = GetCurrentFlowPathSection(flowPath, lineY);
            bool requireLinebreak = lineX + BoundsPerLine[0].Width > curfp.X2;

            this.BoundsPerLine[0].X = requireLinebreak ? 0 : lineX;
            this.BoundsPerLine[0].Y = lineY + (requireLinebreak ? currentLineHeight : 0);

            return requireLinebreak;
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            /*ctx.SetSourceRGBA(1, 1, 1, 0.2);
            ctx.Rectangle(BoundsPerLine[0].X, BoundsPerLine[0].Y, BoundsPerLine[0].Width, BoundsPerLine[0].Height);
            ctx.Fill();*/
        }

        public override void RenderInteractiveElements(float deltaTime, double renderX, double renderY)
        {
            LineRectangled bounds = BoundsPerLine[0];

            ItemStack itemstack = Itemstacks[curItemIndex];

            if ((secondsVisible -= deltaTime) <= 0)
            {
                secondsVisible = 1;
                curItemIndex = (curItemIndex + 1) % Itemstacks.Length;
            }

            slot.Itemstack = itemstack;

            ElementBounds scibounds = ElementBounds.FixedSize((int)bounds.Width, (int)bounds.Height);
            scibounds.ParentBounds = capi.Gui.WindowBounds;
            scibounds.CalcWorldBounds();
            scibounds.absFixedX = renderX + bounds.X;
            scibounds.absFixedY = renderY + bounds.Y;

            api.Render.PushScissor(scibounds);

            api.Render.RenderItemstackToGui(slot, renderX + bounds.X + bounds.Width * 0.5f, renderY + bounds.Y + bounds.Height * 0.5f, 100, (float)bounds.Width * 0.58f, ColorUtil.WhiteArgb, true, false, ShowStackSize);

            api.Render.PopScissor();

            int relx = (int)(api.Input.MouseX - renderX);
            int rely = (int)(api.Input.MouseY - renderY);
            if (bounds.PointInside(relx, rely))
            {
                RenderItemstackTooltip(slot, renderX + relx, renderY + rely, deltaTime);
            }
        }

        public override void OnMouseDown(MouseEvent args)
        {
            foreach (var val in BoundsPerLine)
            {
                if (val.PointInside(args.X, args.Y))
                {
                    onStackClicked?.Invoke(Itemstacks[curItemIndex]);
                }
            }
        }


    }
}
