using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cairo;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Draws an itemstack 
    /// </summary>
    public class ItemstackTextComponent : ItemstackComponentBase
    {
        ItemStack itemstack;
        double size;

        Common.Action<ItemStack> onStackClicked;

        public ItemstackTextComponent(ICoreClientAPI capi, ItemStack itemstack, double size, double sidePadding = 0, EnumFloat floatType = EnumFloat.Left, Common.Action<ItemStack> onStackClicked = null) : base(capi)
        {
            this.itemstack = itemstack;
            this.onStackClicked = onStackClicked;
            this.Float = floatType;
            this.size = size;
            this.BoundsPerLine = new LineRectangled[] { new LineRectangled(0, 0, size + sidePadding, size) };
            //PaddingRight = 0;

        }

        public override bool CalcBounds(TextFlowPath[] flowPath, double currentLineHeight, double lineX, double lineY)
        {
            TextFlowPath curfp = GetCurrentFlowPathSection(flowPath, lineY);
            bool requireLinebreak = lineX + BoundsPerLine[0].Width > curfp.X2;

            this.BoundsPerLine[0].X = requireLinebreak ? 0 : lineX;
            this.BoundsPerLine[0].Y = lineY + (requireLinebreak ? currentLineHeight : 0);

            if (Float == EnumFloat.Right)
            {
                BoundsPerLine[0].X = curfp.X2 - size;
            }

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

            api.Render.RenderItemstackToGui(
                itemstack, renderX + bounds.X + bounds.Width * 0.5f, renderY + bounds.Y + bounds.Height * 0.5f, 100, (float)size * 0.58f, ColorUtil.WhiteArgb, true, false, false);

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
                    onStackClicked?.Invoke(itemstack);
                }
            }
        }



    }
}
