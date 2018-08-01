using Cairo;
using System;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public class TextUtil
    {
        TextBackground defaultBackground = new TextBackground();
        ICoreClientAPI capi;

        public TextUtil(ICoreClientAPI capi)
        {
            this.capi = capi;
        }

        public LoadedTexture GenTextTexture(string text, ICairoFont font, int width, int height, TextBackground background = null, EnumTextOrientation orientation = EnumTextOrientation.Left, float lineHeight = 1f)
        {
            if (background == null) background = defaultBackground;

            ElementBounds bounds = new ElementBounds().WithFixedSize(width, height);

            ImageSurface surface = new ImageSurface(Format.Argb32, width, height);
            Context ctx = new Context(surface);

            GuiElementTextBase elTeBa = new GuiElementTextBase(capi, text, (CairoFont)font, bounds);

            ctx.SetSourceRGBA(background.color);
            GuiElement.RoundRectangle(ctx, 0, 0, width, height, background.radius);
            ctx.Fill();

            elTeBa.ShowMultilineText(ctx, text, background.padding, background.padding, width, orientation, lineHeight);

            int textureId = capi.Gui.LoadCairoTexture(surface, true);

            surface.Dispose();
            ctx.Dispose();

            return new LoadedTexture(capi)
            {
                TextureId = textureId,
                Width = width,
                Height = height
            };
        }


        public LoadedTexture GenTextTexture(string text, ICairoFont font, int width, int height, TextBackground background = null)
        {
            if (background == null) background = defaultBackground;

            ImageSurface surface = new ImageSurface(Format.Argb32, width, height);
            Context ctx = new Context(surface);

            CairoFont cfont = font as CairoFont;

            ctx.SetSourceRGBA(background.color);
            GuiElement.RoundRectangle(ctx, 0, 0, width, height, background.radius);
            ctx.Fill();

            cfont.SetupContext(ctx);

            double fontHeight = cfont.GetFontExtents().Height;

            string[] lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].TrimEnd();

                ctx.MoveTo(background.padding, background.padding + ctx.FontExtents.Ascent + i * fontHeight);

                if (cfont.StrokeWidth > 0)
                {
                    ctx.TextPath(lines[i]);
                    ctx.SetSourceRGBA(cfont.Color);
                    ctx.FillPreserve();
                    ctx.LineWidth = cfont.StrokeWidth;
                    ctx.SetSourceRGBA(cfont.StrokeColor);
                    ctx.Stroke();
                }
                else
                {
                    ctx.ShowText(lines[i]);

                    if (cfont.RenderTwice)
                    {
                        ctx.ShowText(lines[i]);
                    }
                }
            }

            int textureId = capi.Gui.LoadCairoTexture(surface, true);

            surface.Dispose();
            ctx.Dispose();

            return new LoadedTexture(capi)
            {
                TextureId = textureId,
                Width = width,
                Height = height
            };
        }

        public LoadedTexture GenTextTexture(string text, ICairoFont font, TextBackground background = null)
        {
            if (background == null) background = defaultBackground;

            ElementBounds bounds = new ElementBounds();
            ((CairoFont)font).AutoBoxSize(text, bounds);


            int width = (int)GuiElement.scaled(bounds.fixedWidth + 1 + 2 * background.padding);
            int height = (int)GuiElement.scaled(bounds.fixedHeight + 1 + 2 * background.padding);

            return GenTextTexture(text, font, width, height, background);
        }



        public LoadedTexture GenUnscaledTextTexture(string text, ICairoFont font, TextBackground background = null)
        {
            if (background == null) background = defaultBackground;

            double maxwidth = 0;
            string[] lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].TrimEnd();

                TextExtents textents = ((CairoFont)font).GetTextExtents(lines[i]);
                maxwidth = Math.Max(textents.Width, maxwidth);
            }

            FontExtents fextents = ((CairoFont)font).GetFontExtents();

            int width = (int)maxwidth + 1 + 2 * background.padding;
            int height = (int)fextents.Height * lines.Length + 1 + 2 * background.padding;

            return GenTextTexture(text, font, width, height, background);
        }

        public LoadedTexture GenTextTexture(string text, ICairoFont font, int maxWidth, TextBackground background = null, EnumTextOrientation orientation = EnumTextOrientation.Left)
        {
            if (background == null) background = defaultBackground;


            double fullTextWidth = 0;
            string[] lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].TrimEnd();

                TextExtents textents = ((CairoFont)font).GetTextExtents(lines[i]);
                fullTextWidth = Math.Max(textents.Width, fullTextWidth);
            }

            int width = (int)Math.Min(maxWidth, fullTextWidth) + 2 * background.padding;

            TextSizeProber prober = new TextSizeProber();
            double height = prober.GetMultilineTextHeight(font as CairoFont, text, width) + 2 * background.padding;

            return GenTextTexture(text, font, width, (int)height + 1, background, orientation);
        }
    }
}
