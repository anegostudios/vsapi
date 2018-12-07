using System;
using Cairo;
using Vintagestory.API.MathTools;
using Vintagestory.API.Client;
using Vintagestory.API.Config;

namespace Vintagestory.API.Client
{
    public class GuiElementCell : GuiElementTextBase, IGuiElementCell
    {
        public static double unscaledRightBoxWidth = 40;

        /// <summary>
        /// The table cell information.
        /// </summary>
        public TableCell cell;
        double titleTextheight;

        bool showModifyIcons = true;

        LoadedTexture leftHighlightTexture;
        LoadedTexture rightHighlightTexture;

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
        public GuiElementCell(ICoreClientAPI capi, TableCell cell, ElementBounds bounds) : base(capi, "", null, bounds)
        {
            this.cell = cell;
            leftHighlightTexture = new LoadedTexture(capi);
            rightHighlightTexture = new LoadedTexture(capi);

            if (cell.TitleFont == null)
            {
                cell.TitleFont = CairoFont.MediumDialogText();
            }

            if (cell.DetailTextFont == null)
            {
                cell.DetailTextFont = CairoFont.SmallDialogText();
                cell.DetailTextFont.Color[3] *= 0.6;
            }

        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            double rightBoxWidth = scaled(unscaledRightBoxWidth);

            Bounds.CalcWorldBounds();

            if (cell.HighlightCell > 0)
            {
                RoundRectangle(ctx, Bounds.bgDrawX, Bounds.bgDrawY, Bounds.OuterWidth, Bounds.OuterHeight, 1);

                ctx.SetSourceRGB(GuiStyle.DialogDefaultBgColor[0], GuiStyle.DialogDefaultBgColor[1], GuiStyle.DialogDefaultBgColor[2]);
                ctx.Fill();
            }

            Font = cell.TitleFont;
            titleTextheight = textUtil.AutobreakAndDrawMultilineTextAt(ctx, Font, cell.Title, Bounds.drawX, Bounds.drawY + Bounds.absPaddingY, Bounds.InnerWidth);

            Font = cell.DetailTextFont;
            textUtil.AutobreakAndDrawMultilineTextAt(ctx, Font, cell.DetailText, Bounds.drawX, Bounds.drawY + titleTextheight + Bounds.absPaddingY, Bounds.InnerWidth);

            if (cell.RightTopText != null)
            {
                TextExtents extents = Font.GetTextExtents(cell.RightTopText);
                textUtil.AutobreakAndDrawMultilineTextAt(ctx, Font, cell.RightTopText, Bounds.drawX + Bounds.InnerWidth - extents.Width - rightBoxWidth - scaled(10), Bounds.drawY + Bounds.absPaddingY + scaled(cell.RightTopOffY), extents.Width + 1, EnumTextOrientation.Right);
            }

            if (cell.HighlightCell > 0)
            {
                EmbossRoundRectangleElement(ctx, Bounds.bgDrawX, Bounds.bgDrawY, Bounds.OuterWidth, Bounds.OuterHeight, false, 2);
            }

            if (showModifyIcons)
            {
                ctx.LineWidth = 1;

                
                double crossSize = scaled(20);
                double crossWidth = scaled(5);

                ctx.SetSourceRGBA(0, 0, 0, 0.4);
                ctx.NewPath();
                ctx.MoveTo(Bounds.bgDrawX + Bounds.InnerWidth - rightBoxWidth, Bounds.bgDrawY + 1);
                ctx.LineTo(Bounds.bgDrawX + Bounds.InnerWidth - rightBoxWidth, Bounds.bgDrawY + Bounds.OuterHeight - 2);
                ctx.ClosePath();
                ctx.Stroke();

                ctx.SetSourceRGBA(1, 1, 1, 0.3);
                ctx.NewPath();
                ctx.MoveTo(Bounds.bgDrawX + Bounds.InnerWidth - rightBoxWidth + 1, Bounds.bgDrawY + 1);
                ctx.LineTo(Bounds.bgDrawX + Bounds.InnerWidth - rightBoxWidth + 1, Bounds.bgDrawY + Bounds.OuterHeight - 2);
                ctx.ClosePath();
                ctx.Stroke();

                double crossX = Bounds.drawX + Bounds.InnerWidth - rightBoxWidth + scaled(5);
                double crossY = Bounds.drawY;

                ctx.Operator = Operator.Source;

                ctx.SetSourceRGBA(0, 0, 0, 0.8);
                //api.Gui.IconsDrawCross(ctx, crossX - 1, crossY - 1 + scaled(5), crossWidth, crossSize);
                api.Gui.Icons.DrawPen(ctx, crossX - 1, crossY - 1 + scaled(5), crossWidth, crossSize);

                ctx.SetSourceRGBA(1, 1, 1, 0.5);
                //api.Gui.IconsDrawCross(ctx, crossX + 1, crossY + 1 + scaled(5), crossWidth, crossSize);
                api.Gui.Icons.DrawPen(ctx, crossX + 1, crossY + 1 + scaled(5), crossWidth, crossSize);

                ctx.SetSourceRGBA(0, 0, 0, 0.4);
                //api.Gui.IconsDrawCross(ctx, crossX, crossY + scaled(5), crossWidth, crossSize);
                api.Gui.Icons.DrawPen(ctx, crossX, crossY + scaled(5), crossWidth, crossSize);

                ctx.Operator = Operator.Over;
            }
        }

        /// <summary>
        /// Creates the dynamic parts of the cell.
        /// </summary>
        public void CreateDynamicParts()
        {
            ComposeHover(true, ref leftHighlightTexture);
            ComposeHover(false, ref rightHighlightTexture);
        }


        void ComposeHover(bool left, ref LoadedTexture textureId)
        {
            ImageSurface surface = new ImageSurface(Format.Argb32, (int)Bounds.OuterWidth, (int)Bounds.OuterHeight);
            Context ctx = genContext(surface);

            double boxWidth = scaled(unscaledRightBoxWidth);

            if (left)
            {
                ctx.NewPath();
                ctx.LineTo(0, 0);
                ctx.LineTo(Bounds.InnerWidth - boxWidth, 0);
                ctx.LineTo(Bounds.InnerWidth - boxWidth, Bounds.OuterHeight);
                ctx.LineTo(0, Bounds.OuterHeight);
                ctx.ClosePath();
            }
            else
            {
                ctx.NewPath();
                ctx.LineTo(Bounds.InnerWidth - boxWidth, 0);
                ctx.LineTo(Bounds.OuterWidth, 0);
                ctx.LineTo(Bounds.OuterWidth, Bounds.OuterHeight);
                ctx.LineTo(Bounds.InnerWidth - boxWidth, Bounds.OuterHeight);
                ctx.ClosePath();
            }

            ctx.SetSourceRGBA(0, 0, 0, 0.15);
            ctx.Fill();

            generateTexture(surface, ref textureId);

            ctx.Dispose();
            surface.Dispose();
        }

        /// <summary>
        /// Updates the height of the cell based off the contents.
        /// </summary>
        public void UpdateCellHeight()
        {
            Bounds.CalcWorldBounds();

            double padding = Bounds.absPaddingY;
            double unscaledPadding = Bounds.absPaddingY / RuntimeEnv.GUIScale;
            double boxwidth = Bounds.InnerWidth;

            this.Font = cell.TitleFont;
            this.text = cell.Title;
            titleTextheight = textUtil.GetMultilineTextHeight(Font, cell.Title, boxwidth) / RuntimeEnv.GUIScale; // Need unscaled values here

            this.Font = cell.DetailTextFont;
            this.text = cell.DetailText;
            double detailTextHeight = textUtil.GetMultilineTextHeight(Font, cell.DetailText, boxwidth) / RuntimeEnv.GUIScale; // Need unscaled values here

            Bounds.fixedHeight = unscaledPadding + titleTextheight + unscaledPadding + detailTextHeight + unscaledPadding;

            if (showModifyIcons && Bounds.fixedHeight < 73)
            {
                Bounds.fixedHeight = 73;
            }
        }

        /// <summary>
        /// The event fired when the interactive element is ready to be rendered.
        /// </summary>
        /// <param name="api">The Client API</param>
        /// <param name="parentBounds">The bounds of the parent table.</param>
        /// <param name="deltaTime">The change in time.</param>
        public void OnRenderInteractiveElements(ICoreClientAPI api, ElementBounds parentBounds, float deltaTime)
        {
            int dx = api.Input.MouseX - (int)parentBounds.absX;
            int dy = api.Input.MouseY - (int)parentBounds.absY;
            Vec2d pos = Bounds.PositionInside(dx, dy);

            if (pos == null) return;

            if (pos.X > Bounds.InnerWidth - scaled(GuiElementCell.unscaledRightBoxWidth))
            {
                api.Render.Render2DTexturePremultipliedAlpha(rightHighlightTexture.TextureId, parentBounds.absX + Bounds.absX, parentBounds.absY + Bounds.absY, Bounds.OuterWidth, Bounds.OuterHeight);
            }
            else
            {
                api.Render.Render2DTexturePremultipliedAlpha(leftHighlightTexture.TextureId, parentBounds.absX + Bounds.absX, parentBounds.absY + Bounds.absY, Bounds.OuterWidth, Bounds.OuterHeight);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            leftHighlightTexture.Dispose();
            rightHighlightTexture.Dispose();
        }
    }
}