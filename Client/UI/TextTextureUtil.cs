using Cairo;
using System;

#nullable disable

namespace Vintagestory.API.Client
{
    public class TextTextureUtil
    {
        TextBackground defaultBackground = new TextBackground();
        ICoreClientAPI capi;

        /// <summary>
        /// Text Texture Util constructor.
        /// </summary>
        /// <param name="capi">The Client API.</param>
        public TextTextureUtil(ICoreClientAPI capi)
        {
            this.capi = capi;
        }

        /// <summary>
        /// Takes a string of text and applies a texture to it.
        /// </summary>
        /// <param name="text">The text to texture.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="width">The width of the text.</param>
        /// <param name="height">The height of the text.</param>
        /// <param name="background">The background of the text. (default: none/null)</param>
        /// <param name="orientation">The orientation of the text. (default: left)</param>
        /// <param name="demulAlpha"></param>
        /// <returns>The texturized text.</returns>
        public LoadedTexture GenTextTexture(string text, CairoFont font, int width, int height, TextBackground background = null, EnumTextOrientation orientation = EnumTextOrientation.Left, bool demulAlpha = false)
        {
            LoadedTexture tex = new LoadedTexture(capi);
            GenOrUpdateTextTexture(text, font, width, height, ref tex, background, orientation);
            return tex;
        }

        /// <summary>
        /// Takes a texture and applies some text to it.
        /// </summary>
        /// <param name="text">The text to texture.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="width">The width of the text.</param>
        /// <param name="height">The height of the text.</param>
        /// <param name="loadedTexture">The texture to be loaded on to.</param>
        /// <param name="background">The background of the text. (default: none/null)</param>
        /// <param name="orientation">The orientation of the text. (default: left)</param>
        /// <param name="demulAlpha"></param>
        public void GenOrUpdateTextTexture(string text, CairoFont font, int width, int height, ref LoadedTexture loadedTexture, TextBackground background = null, EnumTextOrientation orientation = EnumTextOrientation.Left, bool demulAlpha = false)
        {
            if (background == null) background = defaultBackground;

            ElementBounds bounds = new ElementBounds().WithFixedSize(width, height);

            ImageSurface surface = new ImageSurface(Format.Argb32, width, height);
            Context ctx = new Context(surface);

            GuiElementTextBase elTeBa = new GuiElementTextBase(capi, text, font, bounds);

            ctx.SetSourceRGBA(background.FillColor);
            GuiElement.RoundRectangle(ctx, 0, 0, width, height, background.Radius);

            if (background.BorderWidth > 0)
            {
                ctx.FillPreserve();
                ctx.Operator = Operator.Atop;
                ctx.LineWidth = background.BorderWidth;
                ctx.SetSourceRGBA(background.BorderColor);
                ctx.Stroke();
                ctx.Operator = Operator.Over;
            } else
            {
                ctx.Fill();
            }

            elTeBa.textUtil.AutobreakAndDrawMultilineTextAt(ctx, font, text, background.HorPadding, background.VerPadding, width, orientation);

            if (demulAlpha)
            {
                surface.DemulAlpha();
            }

            capi.Gui.LoadOrUpdateCairoTexture(surface, false, ref loadedTexture);

            surface.Dispose();
            ctx.Dispose();
        }

        /// <summary>
        /// Takes a string of text and applies a texture to it.
        /// </summary>
        /// <param name="text">The text to texture.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="width">The width of the text.</param>
        /// <param name="height">The height of the text.</param>
        /// <param name="background">The background of the text. (default: none/null)</param>
        /// <returns>The texturized text.</returns>
        public LoadedTexture GenTextTexture(string text, CairoFont font, int width, int height, TextBackground background = null)
        {
            if (background == null) background = defaultBackground;

            ImageSurface surface = new ImageSurface(Format.Argb32, width, height);
            Context ctx = new Context(surface);

            if (background?.FillColor != null)
            {
                ctx.SetSourceRGBA(background.FillColor);
                GuiElement.RoundRectangle(ctx, 0, 0, width, height, background.Radius);
                ctx.Fill();
            }

            if (background?.Shade == true)
            {
                ctx.SetSourceRGBA(GuiStyle.DialogLightBgColor[0] * 1.4, GuiStyle.DialogStrongBgColor[1] * 1.4, GuiStyle.DialogStrongBgColor[2] * 1.4, 1);
                ctx.LineWidth = 5;// background.BorderWidth * 1.75;
                GuiElement.RoundRectangle(ctx, 0, 0, width, height, background.Radius);
                ctx.StrokePreserve();
                surface.BlurFull(6.2);

                ctx.SetSourceRGBA(new double[] { 45 / 255.0, 35 / 255.0, 33 / 255.0, 1 });
                ctx.LineWidth = background.BorderWidth;
                ctx.Stroke();
            }

            if (background?.BorderColor != null)
            {
                ctx.SetSourceRGBA(background.BorderColor);
                GuiElement.RoundRectangle(ctx, 0, 0, width, height, background.Radius);
                ctx.LineWidth = background.BorderWidth;
                ctx.Stroke();
            }



            font.SetupContext(ctx);

            double fontHeight = font.GetFontExtents().Height;

            string[] lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].TrimEnd();

                ctx.MoveTo(background.HorPadding, background.VerPadding + ctx.FontExtents.Ascent + i * fontHeight);

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

        /// <summary>
        /// Takes a string of text and applies a texture to it.
        /// </summary>
        /// <param name="text">The text to texture.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="background">The background of the text. (default: none/null)</param>
        /// <returns>The texturized text.</returns>
        public LoadedTexture GenTextTexture(string text, CairoFont font, TextBackground background = null)
        {
            LoadedTexture tex = new LoadedTexture(capi);
            GenOrUpdateTextTexture(text, font, ref tex, background);
            return tex;
        }


        /// <summary>
        /// Takes a texture and applies some text to it.
        /// </summary>
        /// <param name="text">The text to texture.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="loadedTexture">The texture to be loaded on to.</param>
        /// <param name="background">The background of the text. (default: none/null)</param>
        public void GenOrUpdateTextTexture(string text, CairoFont font, ref LoadedTexture loadedTexture, TextBackground background = null)
        {
            if (background == null)
            {
                background = defaultBackground.Clone();
                if (font.StrokeWidth > 0)
                {
                    background.Padding = (int)Math.Ceiling(font.StrokeWidth);
                }
            }

            ElementBounds bounds = new ElementBounds();
            font.AutoBoxSize(text, bounds);


            int width = (int)Math.Ceiling(GuiElement.scaled(bounds.fixedWidth + 1 + 2 * background.HorPadding));
            int height = (int)Math.Ceiling(GuiElement.scaled(bounds.fixedHeight + 1 + 2 * background.VerPadding));

            GenOrUpdateTextTexture(text, font, width, height, ref loadedTexture, background);
        }


        /// <summary>
        /// Generates an unscaled text texture.
        /// </summary>
        /// <param name="text">The text to texture.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="background">The background of the text (Default: none/null)</param>
        /// <returns>The loaded unscaled texture.</returns>
        public LoadedTexture GenUnscaledTextTexture(string text, CairoFont font, TextBackground background = null)
        {
            if (background == null) background = defaultBackground;

            double maxwidth = 0;
            string[] lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].TrimEnd();

                TextExtents textents = font.GetTextExtents(lines[i]);
                maxwidth = Math.Max(textents.Width, maxwidth);
            }

            FontExtents fextents = font.GetFontExtents();

            int width = (int)maxwidth + 1 + 2 * background.HorPadding;
            int height = (int)fextents.Height * lines.Length + 1 + 2 * background.VerPadding;

            return GenTextTexture(text, font, width, height, background);
        }

        /// <summary>
        /// Takes a string of text and applies a texture to it.
        /// </summary>
        /// <param name="text">The text to texture.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="maxWidth">The maximum width of the text.</param>
        /// <param name="background">The background of the text. (default: none/null)</param>
        /// <param name="orientation">The orientation of the text. (default: left)</param>
        /// <returns>The texturized text.</returns>
        public LoadedTexture GenTextTexture(string text, CairoFont font, int maxWidth, TextBackground background = null, EnumTextOrientation orientation = EnumTextOrientation.Left)
        {
            if (background == null) background = defaultBackground;


            double fullTextWidth = 0;
            string[] lines = text.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].TrimEnd();

                TextExtents textents = font.GetTextExtents(lines[i]);
                fullTextWidth = Math.Max(textents.Width, fullTextWidth);
            }

            int width = (int)Math.Min(maxWidth, fullTextWidth) + 2 * background.HorPadding;

            TextDrawUtil prober = new TextDrawUtil();
            double height = prober.GetMultilineTextHeight(font, text, width) + 2 * background.VerPadding;

            return GenTextTexture(text, font, width, (int)height + 1, background, orientation);
        }
    }
}
