using System;
using Cairo;
using Vintagestory.API.MathTools;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using System.Drawing;

namespace Vintagestory.API.Client
{
    public class GuiElementModCell : GuiElementTextBase, IGuiElementCell
    {
        public static double unscaledRightBoxWidth = 40;


        public TableCell cell;
        IAssetManager assetManager;
        double titleTextheight;

        bool showModifyIcons = true;
        public bool On;

        internal int leftHighlightTextureId;
        internal int rightHighlightTextureId;
        internal int switchOnTextureId;

        internal double unscaledSwitchPadding = 5;
        internal double unscaledSwitchSize = 30;


        ElementBounds IGuiElementCell.Bounds
        {
            get { return Bounds; }
        }

        public GuiElementModCell(ICoreClientAPI capi, TableCell cell, IAssetManager assetManager, ElementBounds bounds) : base(capi, "", null, bounds)
        {
            this.cell = cell;
            this.assetManager = assetManager;

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

            var mod = (Mod)cell.Data;
            bool validMod = mod.Info != null;

            if (cell.HighlightCell > 0)
            {
                RoundRectangle(ctx, Bounds.bgDrawX, Bounds.bgDrawY, Bounds.OuterWidth, Bounds.OuterHeight, 1);

                ctx.SetSourceRGB(ElementGeometrics.DialogDefaultBgColor[0], ElementGeometrics.DialogDefaultBgColor[1], ElementGeometrics.DialogDefaultBgColor[2]);
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
            titleTextheight = ShowMultilineText(ctx, cell.Title, Bounds.drawX + textOffset, Bounds.drawY, Bounds.InnerWidth - textOffset);

            Font = cell.DetailTextFont;
            ShowMultilineText(ctx, cell.DetailText, Bounds.drawX + textOffset, Bounds.drawY + titleTextheight + Bounds.absPaddingY, Bounds.InnerWidth - textOffset);

            if (cell.RightTopText != null)
            {
                TextExtents extents = Font.GetTextExtents(cell.RightTopText);
                ShowMultilineText(ctx, cell.RightTopText, Bounds.drawX + Bounds.InnerWidth - extents.Width - rightBoxWidth - scaled(10), Bounds.drawY + Bounds.absPaddingY + scaled(cell.RightTopOffY), extents.Width + 1, EnumTextOrientation.Right);
            }

            if (cell.HighlightCell > 0)
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
                double y = Bounds.drawY + Bounds.absPaddingY + scaled(5);
                

                ctx.SetSourceRGBA(0, 0, 0, 0.2);
                RoundRectangle(ctx, x, y, checkboxsize, checkboxsize, 3);
                ctx.Fill();
                EmbossRoundRectangleElement(ctx, x, y, checkboxsize, checkboxsize, true, 1, 2);
            }
        }


        public void CreateDynamicParts()
        {
            ComposeHover(true, ref leftHighlightTextureId);
            ComposeHover(false, ref rightHighlightTextureId);
            genOnTexture();
        }

        private void genOnTexture()
        {
            double size = scaled(unscaledSwitchSize - 2 * unscaledSwitchPadding);

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)size, (int)size);
            Context ctx = genContext(surface);

            RoundRectangle(ctx, 0, 0, size, size, 3);
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


        public void UpdateCellHeight()
        {
            Bounds.CalcWorldBounds();

            double padding = Bounds.absPaddingY;
            double unscaledPadding = Bounds.absPaddingY / ClientSettingsApi.GUIScale;
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
            titleTextheight = GetMultilineTextHeight(cell.Title, boxwidth) / ClientSettingsApi.GUIScale; // Need unscaled values here

            this.Font = cell.DetailTextFont;
            this.text = cell.DetailText;
            double detailTextHeight = GetMultilineTextHeight(cell.DetailText, boxwidth) / ClientSettingsApi.GUIScale; // Need unscaled values here

            Bounds.fixedHeight = unscaledPadding + titleTextheight + unscaledPadding + detailTextHeight + unscaledPadding;

            if (showModifyIcons && Bounds.fixedHeight < 73)
            {
                Bounds.fixedHeight = 73;
            }
        }

        public void OnRenderInteractiveElements(ICoreClientAPI api, ElementBounds parentBounds, float deltaTime)
        {
            int dx = api.Input.GetMouseCurrentX() - (int)parentBounds.absX;
            int dy = api.Input.GetMouseCurrentY() - (int)parentBounds.absY;
            Vec2d pos = Bounds.PositionInside(dx, dy);

            var mod = (Mod)cell.Data;
            if (mod.Info != null && pos != null)
            {
                if (pos.x > Bounds.InnerWidth - scaled(GuiElementCell.unscaledRightBoxWidth))
                {
                    api.Render.Render2DTexturePremultipliedAlpha(rightHighlightTextureId, parentBounds.absX + Bounds.absX, parentBounds.absY + Bounds.absY, Bounds.OuterWidth, Bounds.OuterHeight);
                }
                else
                {
                    api.Render.Render2DTexturePremultipliedAlpha(leftHighlightTextureId, parentBounds.absX + Bounds.absX, parentBounds.absY + Bounds.absY, Bounds.OuterWidth, Bounds.OuterHeight);
                }
            }

            if (On)
            {
                double size = scaled(unscaledSwitchSize - 2 * unscaledSwitchPadding);
                double padding = scaled(unscaledSwitchPadding);

                double x = parentBounds.renderX + Bounds.InnerWidth - size + padding;
                double y = parentBounds.renderY + Bounds.drawY + parentBounds.absPaddingY + Bounds.absPaddingY + scaled(4) + padding;


                api.Render.Render2DTexturePremultipliedAlpha(switchOnTextureId, x, y, (int)size, (int)size);
            }
            else
            {
                api.Render.Render2DTexturePremultipliedAlpha(rightHighlightTextureId, parentBounds.absX + Bounds.absX, parentBounds.absY + Bounds.absY, Bounds.OuterWidth, Bounds.OuterHeight);
                api.Render.Render2DTexturePremultipliedAlpha(leftHighlightTextureId, parentBounds.absX + Bounds.absX, parentBounds.absY + Bounds.absY, Bounds.OuterWidth, Bounds.OuterHeight);
            }
        }
    }
}