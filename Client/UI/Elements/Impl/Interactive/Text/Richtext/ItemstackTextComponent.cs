using System;
using Cairo;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Draws an itemstack 
    /// </summary>
    public class ItemstackTextComponent : ItemstackComponentBase
    {
        DummySlot slot;
        double size;
        public bool ShowStacksize = false;

        Action<ItemStack> onStackClicked;
        

        public ItemstackTextComponent(ICoreClientAPI capi, ItemStack itemstack, double size, double rightSidePadding = 0, EnumFloat floatType = EnumFloat.Left, Action<ItemStack> onStackClicked = null) : base(capi)
        {
            size = GuiElement.scaled(size);

            slot = new DummySlot(itemstack);
            this.onStackClicked = onStackClicked;
            this.Float = floatType;
            this.size = size;
            this.BoundsPerLine = new LineRectangled[] { new LineRectangled(0, 0, size, size) };
            PaddingRight = GuiElement.scaled(rightSidePadding);
        }

        public override EnumCalcBoundsResult CalcBounds(TextFlowPath[] flowPath, double currentLineHeight, double offsetX, double lineY, out double nextOffsetX)
        {
            TextFlowPath curfp = GetCurrentFlowPathSection(flowPath, lineY);
            offsetX += GuiElement.scaled(PaddingLeft);

            bool requireLinebreak = offsetX + BoundsPerLine[0].Width > curfp.X2;

            this.BoundsPerLine[0].X = requireLinebreak ? 0 : offsetX;
            this.BoundsPerLine[0].Y = lineY + (requireLinebreak ? currentLineHeight : 0);

            if (Float == EnumFloat.Right)
            {
                BoundsPerLine[0].X = curfp.X2 - size;
            }

            BoundsPerLine[0].Width = size + GuiElement.scaled(PaddingRight);

            nextOffsetX = (requireLinebreak ? 0 : offsetX) + BoundsPerLine[0].Width;

            return requireLinebreak ? EnumCalcBoundsResult.Nextline : EnumCalcBoundsResult.Continue;
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            /*ctx.SetSourceRGBA(1, 1, 1, 0.2);
            ctx.Rectangle(BoundsPerLine[0].X, BoundsPerLine[0].Y, BoundsPerLine[0].Width, BoundsPerLine[0].Height);
            ctx.Fill();*/
        }

        public override void RenderInteractiveElements(float deltaTime, double renderX, double renderY, double renderZ)
        {
            LineRectangled bounds = BoundsPerLine[0];

            double padLeft = GuiElement.scaled(PaddingLeft);
            double padRight = GuiElement.scaled(PaddingRight);
            double width = bounds.Width - padLeft - padRight;

            ElementBounds scibounds = ElementBounds.FixedSize((int)(bounds.Width / API.Config.RuntimeEnv.GUIScale), (int)(bounds.Height / API.Config.RuntimeEnv.GUIScale));
            scibounds.ParentBounds = capi.Gui.WindowBounds;

            scibounds.CalcWorldBounds();
            scibounds.absFixedX = renderX + bounds.X;
            scibounds.absFixedY = renderY + bounds.Y + offY;

            api.Render.PushScissor(scibounds, true);

            api.Render.RenderItemstackToGui(
                slot, renderX + bounds.X + padLeft + width * 0.5f + offX, renderY + bounds.Y + bounds.Height * 0.5f + offY, GuiElement.scaled(100), (float)size * 0.58f, ColorUtil.WhiteArgb, true, false, ShowStacksize);

            api.Render.PopScissor();


            int relx = (int)(api.Input.MouseX - renderX);
            int rely = (int)(api.Input.MouseY - renderY);

            if (bounds.PointInside(relx, rely))
            {
                RenderItemstackTooltip(slot, renderX + relx + offX, renderY + rely + offY, deltaTime);
            }
        }



        public override void OnMouseDown(MouseEvent args)
        {
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
