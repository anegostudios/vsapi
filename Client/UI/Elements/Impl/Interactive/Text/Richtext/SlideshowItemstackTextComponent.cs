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
        Common.Action<ItemStack> onStackClicked;

        float secondsVisible=1;
        int curItemIndex;

        /// <summary>
        /// Flips through given array of item stacks every second
        /// </summary>
        /// <param name="itemstacks"></param>
        /// <param name="size"></param>
        /// <param name="floatType"></param>
        public SlideshowItemstackTextComponent(ICoreClientAPI capi, ItemStack[] itemstacks, double size, EnumFloat floatType, Common.Action<ItemStack> onStackClicked = null) : base(capi)
        {
            this.Itemstacks = itemstacks;
            this.Float = floatType;
            this.BoundsPerLine = new LineRectangled[] { new LineRectangled(0, 0, size, size) };
            this.onStackClicked = onStackClicked;
            //PaddingRight = 0;
        }

        /// <summary>
        /// Looks at the collectibles handbook groupBy attribute and makes a list of itemstacks from that
        /// </summary>
        /// <param name="itemstackgroup"></param>
        /// <param name="size"></param>
        /// <param name="floatType"></param>
        public SlideshowItemstackTextComponent(ICoreClientAPI capi, ItemStack itemstackgroup, Dictionary<AssetLocation, ItemStack> allstacks, double size, EnumFloat floatType, Common.Action<ItemStack> onStackClicked = null) : base(capi)
        {
            this.onStackClicked = onStackClicked;

            string[] groups = itemstackgroup.Collectible.Attributes?["handbook"]?["groupBy"]?.AsStringArray(null);

            HashSet<AssetLocation> collectibleLocs = new HashSet<AssetLocation>();
            List<ItemStack> stacks = new List<ItemStack>();

            collectibleLocs.Add(itemstackgroup.Collectible.Code);
            stacks.Add(itemstackgroup);

            if (groups != null)
            {
                AssetLocation[] groupWildCards = new AssetLocation[groups.Length];
                for (int i = 0; i < groups.Length; i++)
                {
                    groupWildCards[i] = new AssetLocation(groups[i]);
                }

                foreach (var val in allstacks)
                {
                    for (int i = 0; i < groupWildCards.Length; i++)
                    {
                        if (val.Value.Collectible.WildCardMatch(groupWildCards[i]) && !collectibleLocs.Contains(val.Value.Collectible.Code))
                        {
                            stacks.Add(val.Value);
                            collectibleLocs.Add(val.Value.Collectible.Code);
                            break;
                        }
                    }
                }
            }
            
            foreach (var val in collectibleLocs)
            {
                allstacks.Remove(val);
            }

            this.Itemstacks = stacks.ToArray();
            this.Float = floatType;
            this.BoundsPerLine = new LineRectangled[] { new LineRectangled(0, 0, size, size) };
            //PaddingRight = 0;
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

            api.Render.RenderItemstackToGui(
                itemstack, renderX + bounds.X + bounds.Width * 0.5f, renderY + bounds.Y + bounds.Height * 0.5f, 100, (float)bounds.Width * 0.58f, ColorUtil.WhiteArgb, true, false, false);

            int relx = (int)(api.Input.MouseX - renderX);
            int rely = (int)(api.Input.MouseY - renderY);
            if (bounds.PointInside(relx, rely))
            {
                RenderItemstackTooltip(itemstack, renderX + relx, renderY + rely, deltaTime);
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
