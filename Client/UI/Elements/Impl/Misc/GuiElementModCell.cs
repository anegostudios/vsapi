using System;
using Cairo;
using Vintagestory.API.MathTools;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using System.Drawing;
using Vintagestory.API.Config;

namespace Vintagestory.API.Client
{
    public class GuiElementModCell : GuiElementTextBase, IGuiElementCell
    {
        public static double unscaledRightBoxWidth = 40;

        /// <summary>
        /// The base cell.
        /// </summary>
        public ListCellEntry cell;
        IAssetManager assetManager;
        double titleTextheight;

        bool showModifyIcons = true;
        public bool On;

        internal int leftHighlightTextureId;
        internal int rightHighlightTextureId;
        internal int switchOnTextureId;

        internal double unscaledSwitchPadding = 4;
        internal double unscaledSwitchSize = 30;


        ElementBounds IGuiElementCell.Bounds
        {
            get { return Bounds; }
        }

        /// <summary>
        /// Adds a mod cell to the table.
        /// </summary>
        /// <param name="capi">The client API</param>
        /// <param name="cell">The table cell to add.</param>
        /// <param name="assetManager">The asset manager for the mod</param>
        /// <param name="bounds">The bounds of the cell</param>
        public GuiElementModCell(ICoreClientAPI capi, ListCellEntry cell, IAssetManager assetManager, ElementBounds bounds) : base(capi, "", null, bounds)
        {
            this.cell = cell;
            this.assetManager = assetManager;

            if (cell.TitleFont == null)
            {
                cell.TitleFont = CairoFont.WhiteSmallishText();
            }

            if (cell.DetailTextFont == null)
            {
                cell.DetailTextFont = CairoFont.WhiteSmallText();
                cell.DetailTextFont.Color[3] *= 0.6;
            }

        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            ComposeHover(true, ref leftHighlightTextureId);
            ComposeHover(false, ref rightHighlightTextureId);
            genOnTexture();


            double rightBoxWidth = scaled(unscaledRightBoxWidth);

            Bounds.CalcWorldBounds();

            var mod = (Mod)cell.Data;
            bool validMod = mod.Info != null;

            if (cell.DrawAsButton)
            {
                RoundRectangle(ctx, Bounds.bgDrawX, Bounds.bgDrawY, Bounds.OuterWidth, Bounds.OuterHeight, 1);

                ctx.SetSourceRGB(GuiStyle.DialogDefaultBgColor[0], GuiStyle.DialogDefaultBgColor[1], GuiStyle.DialogDefaultBgColor[2]);
                ctx.Fill();
            }

            double textOffset = 0;

            if (validMod && mod.Icon != null)
            {
                int imageSize = (int)(Bounds.InnerHeight - Bounds.absPaddingY * 2 - 10);
                textOffset = imageSize + 10;

                Bitmap bmp = mod.Icon;
                surface.Image(bmp, (int)Bounds.drawX + 5, (int)Bounds.drawY + 5, imageSize, imageSize);
                
                
                bmp.Dispose();
            }

            Font = cell.TitleFont;
            titleTextheight = textUtil.AutobreakAndDrawMultilineTextAt(ctx, Font, cell.Title, Bounds.drawX + textOffset, Bounds.drawY, Bounds.InnerWidth - textOffset);

            Font = cell.DetailTextFont;
            textUtil.AutobreakAndDrawMultilineTextAt(ctx, Font, cell.DetailText, Bounds.drawX + textOffset, Bounds.drawY + titleTextheight + Bounds.absPaddingY, Bounds.InnerWidth - textOffset);

            if (cell.RightTopText != null)
            {
                TextExtents extents = Font.GetTextExtents(cell.RightTopText);
                textUtil.AutobreakAndDrawMultilineTextAt(ctx, Font, cell.RightTopText, Bounds.drawX + Bounds.InnerWidth - extents.Width - rightBoxWidth - scaled(10), Bounds.drawY + scaled(cell.RightTopOffY), extents.Width + 1, EnumTextOrientation.Right);
            }

            if (cell.DrawAsButton)
            {
                EmbossRoundRectangleElement(ctx, Bounds.bgDrawX, Bounds.bgDrawY, Bounds.OuterWidth, Bounds.OuterHeight, false, 2);
            }

            if (!validMod)
            {
                ctx.SetSourceRGBA(0, 0, 0, 0.5);
                RoundRectangle(ctx, Bounds.bgDrawX, Bounds.bgDrawY, Bounds.OuterWidth, Bounds.OuterHeight, 1);

                ctx.Fill();
            }

            if (showModifyIcons)
            {
                double checkboxsize = scaled(unscaledSwitchSize);
                double padd = scaled(unscaledSwitchPadding);

                double x = Bounds.drawX + Bounds.InnerWidth - scaled(0) - checkboxsize - padd;
                double y = Bounds.drawY + Bounds.absPaddingY;
                

                ctx.SetSourceRGBA(0, 0, 0, 0.2);
                RoundRectangle(ctx, x, y, checkboxsize, checkboxsize, 3);
                ctx.Fill();
                EmbossRoundRectangleElement(ctx, x, y, checkboxsize, checkboxsize, true, 1, 2);
            }
        }
        

        private void genOnTexture()
        {
            double size = scaled(unscaledSwitchSize - 2 * unscaledSwitchPadding);

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)size, (int)size);
            Context ctx = genContext(surface);

            RoundRectangle(ctx, 0, 0, size, size, 2);
            fillWithPattern(api, ctx, waterTextureName);

            generateTexture(surface, ref switchOnTextureId);

            ctx.Dispose();
            surface.Dispose();
        }

        void ComposeHover(bool left, ref int textureId)
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
        /// Updates the height of the given cell based off information.
        /// </summary>
        public void UpdateCellHeight()
        {
            Bounds.CalcWorldBounds();

            double padding = Bounds.absPaddingY;
            double unscaledPadding = Bounds.absPaddingY / RuntimeEnv.GUIScale;
            double boxwidth = Bounds.InnerWidth;

            var mod = (Mod)cell.Data;
            bool validMod = mod.Info != null;
            if (validMod && mod.Icon != null)
            {
                int imageSize = (int)(Bounds.InnerHeight - Bounds.absPaddingY * 2 - 10);
                boxwidth -= imageSize + 10;
            }
            

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
        /// Renders the interactive element.
        /// </summary>
        /// <param name="api">The Client API</param>
        /// <param name="parentBounds">The parent bounds of the cell.</param>
        /// <param name="deltaTime">The change in time.</param>
        public void OnRenderInteractiveElements(ICoreClientAPI api, float deltaTime)
        {
            int mx = api.Input.MouseX;
            int my = api.Input.MouseY;
            Vec2d pos = Bounds.PositionInside(mx, my);

            var mod = (Mod)cell.Data;
            if (mod.Info != null && pos != null)
            {
                if (pos.X > Bounds.InnerWidth - scaled(GuiElementCell.unscaledRightBoxWidth))
                {
                    api.Render.Render2DTexturePremultipliedAlpha(rightHighlightTextureId, Bounds.absX, Bounds.absY, Bounds.OuterWidth, Bounds.OuterHeight);
                }
                else
                {
                    api.Render.Render2DTexturePremultipliedAlpha(leftHighlightTextureId, Bounds.absX, Bounds.absY, Bounds.OuterWidth, Bounds.OuterHeight);
                }
            }

            if (On)
            {
                double size = scaled(unscaledSwitchSize - 2 * unscaledSwitchPadding);
                double padding = scaled(unscaledSwitchPadding);

                double x = Bounds.renderX + Bounds.InnerWidth - size + padding - scaled(5);
                double y = Bounds.renderY + scaled(8) + padding;


                api.Render.Render2DTexturePremultipliedAlpha(switchOnTextureId, x, y, (int)size, (int)size);
            }
            else
            {
                api.Render.Render2DTexturePremultipliedAlpha(rightHighlightTextureId, Bounds.renderX, Bounds.renderY, Bounds.OuterWidth, Bounds.OuterHeight);
                api.Render.Render2DTexturePremultipliedAlpha(leftHighlightTextureId, Bounds.renderX, Bounds.renderY, Bounds.OuterWidth, Bounds.OuterHeight);
            }
        }
    }
}