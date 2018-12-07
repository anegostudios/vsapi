using Cairo;
using System;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public class TextTextureUtil
    {
        TextBackground defaultBackground = new TextBackground();
        ICoreClientAPI capi;

        public TextTextureUtil(ICoreClientAPI capi)
        {
            this.capi = capi;
        }

        public LoadedTexture GenTextTexture(string text, CairoFont font, int width, int height, TextBackground background = null, EnumTextOrientation orientation = EnumTextOrientation.Left)
        {
            LoadedTexture tex = new LoadedTexture(capi);
            GenOrUpdateTextTexture(text, font, width, height, ref tex, background, orientation);
            return tex;
        }

        public void GenOrUpdateTextTexture(string text, CairoFont font, int width, int height, ref LoadedTexture loadedTexture, TextBackground background = null, EnumTextOrientation orientation = EnumTextOrientation.Left)
        {
            if (background == null) background = defaultBackground;

            ElementBounds bounds = new ElementBounds().WithFixedSize(width, height);

            ImageSurface surface = new ImageSurface(Format.Argb32, width, height);
            Context ctx = new Context(surface);

            GuiElementTextBase elTeBa = new GuiElementTextBase(capi, text, font, bounds);

            ctx.SetSourceRGBA(background.FillColor);
            GuiElement.RoundRectangle(ctx, 0, 0, width, height, background.Radius);

            if (background.StrokeWidth > 0)
            {
                ctx.FillPreserve();

                ctx.Operator = Operator.Atop;
                ctx.LineWidth = background.StrokeWidth;
                ctx.SetSourceRGBA(background.StrokeColor);
                ctx.Stroke();
                ctx.Operator = Operator.Over;
            } else
            {
                ctx.Fill();
            }
            

            elTeBa.textUtil.AutobreakAndDrawMultilineTextAt(ctx, font, text, background.Padding, background.Padding, width, orientation);

            int textureId = capi.Gui.LoadCairoTexture(surface, true);

            capi.Gui.LoadOrUpdateCairoTexture(surface, true, ref loadedTexture);

            surface.Dispose();
            ctx.Dispose();
        }


        public LoadedTexture GenTextTexture(string text, CairoFont font, int width, int height, TextBackground background = null)
        {
            if (background == null) background = defaultBackground;

            ImageSurface surface = new ImageSurface(Format.Argb32, width, height);
            Context ctx = new Context(surface);
            

            ctx.SetSourceRGBA(background.FillColor);
            GuiElement.RoundRectangle(ctx, 0, 0, width, height, background.Radius);

            if (background.StrokeWidth > 0)
            {
                ctx.FillPreserve();

                ctx.Operator = Operator.Atop;
                ctx.LineWidth = background.StrokeWidth;
                ctx.SetSourceRGBA(background.StrokeColor);
                ctx.Stroke();
                ctx.Operator = Operator.Over;
            }
            else
            {
                ctx.Fill();
            }

            font.SetupContext(ctx);

            double fontHeight = font.GetFontExtents().Height;

            string[] lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].TrimEnd();

                ctx.MoveTo(background.Padding, background.Padding + ctx.FontExtents.Ascent + i * fontHeight);

                if (font.StrokeWidth > 0)
                {
                    ctx.TextPath(lines[i]);

                    ctx.LineWidth = font.StrokeWidth;
                    ctx.SetSourceRGBA(font.StrokeColor);
                    ctx.StrokePreserve();

                    ctx.SetSourceRGBA(font.Color);
                    ctx.Fill();
                }
                else
                {

                    ctx.ShowText(lines[i]);

                    if (font.RenderTwice)
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

        
        public LoadedTexture GenTextTexture(string text, CairoFont font, TextBackground background = null)
        {
            LoadedTexture tex = new LoadedTexture(capi);
            GenOrUpdateTextTexture(text, font, ref tex, background);
            return tex;
        }

        public void GenOrUpdateTextTexture(string text, CairoFont font, ref LoadedTexture loadedTexture, TextBackground background = null)
        {
            if (background == null) background = defaultBackground;

            ElementBounds bounds = new ElementBounds();
            font.AutoBoxSize(text, bounds);


            int width = (int)GuiElement.scaled(bounds.fixedWidth + 1 + 2 * background.Padding);
            int height = (int)GuiElement.scaled(bounds.fixedHeight + 1 + 2 * background.Padding);

            GenOrUpdateTextTexture(text, font, width, height, ref loadedTexture, background);
        }



        public LoadedTexture GenUnscaledTextTexture(string text, CairoFont font, TextBackground background = null)
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

            int width = (int)maxwidth + 1 + 2 * background.Padding;
            int height = (int)fextents.Height * lines.Length + 1 + 2 * background.Padding;

            return GenTextTexture(text, font, width, height, background);
        }

        public LoadedTexture GenTextTexture(string text, CairoFont font, int maxWidth, TextBackground background = null, EnumTextOrientation orientation = EnumTextOrientation.Left)
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

            int width = (int)Math.Min(maxWidth, fullTextWidth) + 2 * background.Padding;

            TextDrawUtil prober = new TextDrawUtil();
            double height = prober.GetMultilineTextHeight(font as CairoFont, text, width) + 2 * background.Padding;

            return GenTextTexture(text, font, width, (int)height + 1, background, orientation);
        }
    }
}
