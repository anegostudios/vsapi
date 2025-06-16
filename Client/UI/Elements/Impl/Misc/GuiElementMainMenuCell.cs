using System;
using Cairo;
using Vintagestory.API.MathTools;
using Vintagestory.API.Config;

#nullable disable

namespace Vintagestory.API.Client
{
    public class GuiElementMainMenuCell : GuiElementTextBase, IGuiElementCell
    {
        public static double unscaledRightBoxWidth = 40;
        static int unscaledDepth = 4;

        /// <summary>
        /// The table cell information.
        /// </summary>
        public SavegameCellEntry cellEntry;
        double titleTextheight;

        public bool ShowModifyIcons = true;

        LoadedTexture releasedButtonTexture;
        LoadedTexture pressedButtonTexture;

        LoadedTexture leftHighlightTexture;
        LoadedTexture rightHighlightTexture;

        double pressedYOffset;

        public double MainTextWidthSub = 0;

        public Action<int> OnMouseDownOnCellLeft;
        public Action<int> OnMouseDownOnCellRight;


        public double? FixedHeight = null;

        ElementBounds IGuiElementCell.Bounds
        {
            get { return Bounds; }
        }

        /// <summary>
        /// Creates a new Element Cell.  A container for TableCells.
        /// </summary>
        /// <param name="capi">The Client API</param>
        /// <param name="cell">The base cell</param>
        /// <param name="bounds">The bounds of the TableCell</param>
        public GuiElementMainMenuCell(ICoreClientAPI capi, SavegameCellEntry cell, ElementBounds bounds) : base(capi, "", null, bounds)
        {
            this.cellEntry = cell;
            leftHighlightTexture = new LoadedTexture(capi);
            rightHighlightTexture = new LoadedTexture(capi);

            releasedButtonTexture = new LoadedTexture(capi);
            pressedButtonTexture = new LoadedTexture(capi);

            if (cell.TitleFont == null)
            {
                cell.TitleFont = CairoFont.WhiteSmallishText();
            }

            if (cell.DetailTextFont == null)
            {
                cell.DetailTextFont = CairoFont.WhiteSmallText();
                cell.DetailTextFont.Color[3] *= 0.8;
                cell.DetailTextFont.LineHeightMultiplier = 1.1;
            }

        }


        public void Compose()
        { 
            Bounds.CalcWorldBounds();
            
            ImageSurface surface = new ImageSurface(Format.Argb32, Bounds.OuterWidthInt, Bounds.OuterHeightInt);
            Context ctx = new Context(surface);
            ComposeButton(ctx, surface, false);

            generateTexture(surface, ref releasedButtonTexture);

            ctx.Operator = Operator.Clear;
            ctx.Paint();
            ctx.Operator = Operator.Over;

            ComposeButton(ctx, surface, true);
            generateTexture(surface, ref pressedButtonTexture);

            ctx.Dispose();
            surface.Dispose();

            ComposeHover(true, ref leftHighlightTexture);
            if (ShowModifyIcons)
            {
                ComposeHover(false, ref rightHighlightTexture);
            }
        }


        void ComposeButton(Context ctx, ImageSurface surface, bool pressed) {

            double rightBoxWidth = ShowModifyIcons ? scaled(unscaledRightBoxWidth) : 0;
            pressedYOffset = 0;
            
            if (cellEntry.DrawAsButton)
            {
                RoundRectangle(ctx, 0, 0, Bounds.OuterWidthInt, Bounds.OuterHeightInt, 1);

                ctx.SetSourceRGB(GuiStyle.DialogDefaultBgColor[0], GuiStyle.DialogDefaultBgColor[1], GuiStyle.DialogDefaultBgColor[2]);
                ctx.Fill();

                if (pressed)
                {
                    pressedYOffset = scaled(unscaledDepth) / 2;
                }

                EmbossRoundRectangleElement(ctx, 0, 0, Bounds.OuterWidthInt, Bounds.OuterHeightInt, pressed, (int)scaled(unscaledDepth)); 
            }

            Font = cellEntry.TitleFont;
            titleTextheight = textUtil.AutobreakAndDrawMultilineTextAt(ctx, Font, cellEntry.Title, Bounds.absPaddingX, Bounds.absPaddingY + Bounds.absPaddingY + scaled(cellEntry.LeftOffY) + pressedYOffset, Bounds.InnerWidth - rightBoxWidth - MainTextWidthSub);

            Font = cellEntry.DetailTextFont;
            textUtil.AutobreakAndDrawMultilineTextAt(ctx, Font, cellEntry.DetailText, Bounds.absPaddingX, Bounds.absPaddingY + cellEntry.DetailTextOffY + titleTextheight + 2 + Bounds.absPaddingY + scaled(cellEntry.LeftOffY) + pressedYOffset, Bounds.InnerWidth - rightBoxWidth - MainTextWidthSub);

            if (cellEntry.RightTopText != null)
            {
                TextExtents extents = Font.GetTextExtents(cellEntry.RightTopText);
                textUtil.AutobreakAndDrawMultilineTextAt(ctx, Font, cellEntry.RightTopText, Bounds.absPaddingX + Bounds.InnerWidth - extents.Width - rightBoxWidth - scaled(10), Bounds.absPaddingY + Bounds.absPaddingY + scaled(cellEntry.RightTopOffY) + pressedYOffset, extents.Width + 1, EnumTextOrientation.Right);
            }


            if (ShowModifyIcons)
            {
                ctx.LineWidth = scaled(1);
                
                double crossSize = scaled(20);
                double crossWidth = scaled(5);

                ctx.SetSourceRGBA(0, 0, 0, 0.4);
                ctx.NewPath();
                ctx.MoveTo(Bounds.InnerWidth - rightBoxWidth, scaled(1));
                ctx.LineTo(Bounds.InnerWidth - rightBoxWidth, Bounds.OuterHeight - scaled(2));
                ctx.ClosePath();
                ctx.Stroke();

                ctx.SetSourceRGBA(1, 1, 1, 0.3);
                ctx.NewPath();
                ctx.MoveTo(Bounds.InnerWidth - rightBoxWidth + scaled(1), scaled(1));
                ctx.LineTo(Bounds.InnerWidth - rightBoxWidth + scaled(1), Bounds.OuterHeight - scaled(2));
                ctx.ClosePath();
                ctx.Stroke();

                double crossX = Bounds.absPaddingX + Bounds.InnerWidth - rightBoxWidth + scaled(5);
                double crossY = Bounds.absPaddingY;

                ctx.Operator = Operator.Source;

                ctx.SetSourceRGBA(0, 0, 0, 0.8);
                api.Gui.Icons.DrawPen(ctx, crossX - 1, crossY - 1 + scaled(5), crossWidth, crossSize);
                ctx.SetSourceRGBA(1, 1, 1, 0.5);
                api.Gui.Icons.DrawPen(ctx, crossX + 1, crossY + 1 + scaled(5), crossWidth, crossSize);
                ctx.SetSourceRGBA(0, 0, 0, 0.4);
                api.Gui.Icons.DrawPen(ctx, crossX, crossY + scaled(5), crossWidth, crossSize);

                ctx.Operator = Operator.Over;
            }


            if (cellEntry.DrawAsButton && pressed)
            {
                RoundRectangle(ctx, 0, 0, Bounds.OuterWidthInt, Bounds.OuterHeightInt, 1);
                ctx.SetSourceRGBA(0, 0, 0, 0.15);
                ctx.Fill();
            }

        }



        void ComposeHover(bool left, ref LoadedTexture texture)
        {
            ImageSurface surface = new ImageSurface(Format.Argb32, (int)Bounds.OuterWidth, (int)Bounds.OuterHeight);
            Context ctx = genContext(surface);

            double rightBoxWidth = scaled(unscaledRightBoxWidth);

            if (!ShowModifyIcons) rightBoxWidth = -Bounds.OuterWidth+Bounds.InnerWidth;

            if (left)
            {
                ctx.NewPath();
                ctx.LineTo(0, 0);
                ctx.LineTo(Bounds.InnerWidth - rightBoxWidth, 0);
                ctx.LineTo(Bounds.InnerWidth - rightBoxWidth, Bounds.OuterHeight);
                ctx.LineTo(0, Bounds.OuterHeight);
                ctx.ClosePath();
            }
            else
            {
                ctx.NewPath();
                ctx.LineTo(Bounds.InnerWidth - rightBoxWidth, 0);
                ctx.LineTo(Bounds.OuterWidth, 0);
                ctx.LineTo(Bounds.OuterWidth, Bounds.OuterHeight);
                ctx.LineTo(Bounds.InnerWidth - rightBoxWidth, Bounds.OuterHeight);
                ctx.ClosePath();
            }

            ctx.SetSourceRGBA(0, 0, 0, 0.15);
            ctx.Fill();

            generateTexture(surface, ref texture);

            ctx.Dispose();
            surface.Dispose();
        }

        /// <summary>
        /// Updates the height of the cell based off the contents.
        /// </summary>
        public void UpdateCellHeight()
        {
            Bounds.CalcWorldBounds();

            if (FixedHeight != null)
            {
                Bounds.fixedHeight = (double)FixedHeight;
                return;
            }

            double unscaledPadding = Bounds.absPaddingY / RuntimeEnv.GUIScale;
            double boxwidth = Bounds.InnerWidth;

            this.Font = cellEntry.TitleFont;
            this.text = cellEntry.Title;
            titleTextheight = textUtil.GetMultilineTextHeight(Font, cellEntry.Title, boxwidth - MainTextWidthSub) / RuntimeEnv.GUIScale; // Need unscaled values here

            this.Font = cellEntry.DetailTextFont;
            this.text = cellEntry.DetailText;
            double detailTextHeight = textUtil.GetMultilineTextHeight(Font, cellEntry.DetailText, boxwidth - MainTextWidthSub) / RuntimeEnv.GUIScale; // Need unscaled values here

            Bounds.fixedHeight = unscaledPadding + titleTextheight + unscaledPadding + detailTextHeight + unscaledPadding;

            if (ShowModifyIcons && Bounds.fixedHeight < 73)
            {
                Bounds.fixedHeight = 73;
            }
        }

        /// <summary>
        /// Renders the main menu cell
        /// </summary>
        /// <param name="api"></param>
        /// <param name="deltaTime"></param>
        public void OnRenderInteractiveElements(ICoreClientAPI api, float deltaTime)
        {
            if (pressedButtonTexture.TextureId == 0)
            {
                Compose();
            }

            if (cellEntry.Selected)
            {
                api.Render.Render2DTexturePremultipliedAlpha(
                    pressedButtonTexture.TextureId, 
                    (int)(Bounds.absX),
                    (int)(Bounds.absY), 
                    Bounds.OuterWidthInt, 
                    Bounds.OuterHeightInt
                );
            } else
            {
                api.Render.Render2DTexturePremultipliedAlpha(
                    releasedButtonTexture.TextureId, 
                    (int)(Bounds.absX),
                    (int)(Bounds.absY), 
                    Bounds.OuterWidthInt, 
                    Bounds.OuterHeightInt
                );
            }


            int dx = api.Input.MouseX;
            int dy = api.Input.MouseY;
            Vec2d pos = Bounds.PositionInside(dx, dy);

            if (pos == null || !IsPositionInside(api.Input.MouseX, api.Input.MouseY)) return;

            if (ShowModifyIcons && pos.X > Bounds.InnerWidth - scaled(unscaledRightBoxWidth))
            {
                api.Render.Render2DTexturePremultipliedAlpha(rightHighlightTexture.TextureId, Bounds.absX, Bounds.absY, Bounds.OuterWidth, Bounds.OuterHeight);
            }
            else
            {
                api.Render.Render2DTexturePremultipliedAlpha(leftHighlightTexture.TextureId, Bounds.absX, Bounds.absY, Bounds.OuterWidth, Bounds.OuterHeight);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            leftHighlightTexture.Dispose();
            rightHighlightTexture.Dispose();
            releasedButtonTexture.Dispose();
            pressedButtonTexture.Dispose();
        }

        public void OnMouseUpOnElement(MouseEvent args, int elementIndex)
        {
            int mousex = api.Input.MouseX;
            int mousey = api.Input.MouseY;

            Vec2d pos = Bounds.PositionInside(mousex, mousey);
            api.Gui.PlaySound("toggleswitch");

            if (pos.X > Bounds.InnerWidth - scaled(GuiElementMainMenuCell.unscaledRightBoxWidth))
            {
                OnMouseDownOnCellRight?.Invoke(elementIndex);
                args.Handled = true;
                return;
            }
            else
            {
                OnMouseDownOnCellLeft?.Invoke(elementIndex);
                args.Handled = true;
                return;
            }

        }

        public void OnMouseMoveOnElement(MouseEvent args, int elementIndex)
        {
            
        }

        public void OnMouseDownOnElement(MouseEvent args, int elementIndex)
        {
            
        }
    }
}