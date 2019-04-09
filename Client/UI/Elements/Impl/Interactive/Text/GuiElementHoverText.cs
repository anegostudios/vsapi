using Cairo;
using System;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    public class GuiElementHoverText : GuiElementTextBase
    {
        public static TextBackground DefaultBackGround = new TextBackground() {
            Padding = 5,
            Radius = 1,
            FillColor = GuiStyle.DialogStrongBgColor,
            BorderColor = GuiStyle.DialogBorderColor,
            BorderWidth = 3,
            Shade = true
        };
        
        LoadedTexture hoverTexture;
        int unscaledWidth;
        int unscaledPadding = 5;

        int width;
        int height;

        bool autoDisplay = true;
        bool visible = false;
        bool followMouse = true;
        bool autoWidth = false;
        public bool fillBounds = false;

        public TextBackground Background;


        public EnumTextOrientation textOrientation = EnumTextOrientation.Left;

        public override double DrawOrder
        {
            get { return 0.9; }
        }

        /// <summary>
        /// Creates a new instance of hover text.
        /// </summary>
        /// <param name="capi">The client API.</param>
        /// <param name="text">The text of the text.</param>
        /// <remarks>For the text and the text.</remarks>
        /// <param name="font">The font of the text.</param>
        /// <param name="width">The width of the text.</param>
        /// <param name="bounds">the bounds of the text.</param>
        public GuiElementHoverText(ICoreClientAPI capi, string text, CairoFont font, int width, ElementBounds bounds) : base(capi, text, font, bounds)
        {
            unscaledWidth = width;

            hoverTexture = new LoadedTexture(capi);
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
        }

        private void ComposeHoverElement()
        {
            double padding = scaled(unscaledPadding);

            if (fillBounds)
            {

                if (autoWidth)
                {
                    double textWidth = Font.GetTextExtents(text).Width;
                    Bounds.fixedWidth = (int)(textWidth / RuntimeEnv.GUIScale + 2 * unscaledPadding + 1);
                    Bounds.fixedHeight = (int)(Font.GetFontExtents().Height / RuntimeEnv.GUIScale + 2 * unscaledPadding + 1);
                }
                else
                {
                    width = (int)(scaled(unscaledWidth) + 1);
                    height = (int)(textUtil.GetMultilineTextHeight(Font, text, width - 2 * padding) + 2 * padding + 1);
                }


                Bounds.CalcWorldBounds();

                if (autoWidth)
                {
                    width = (int)Math.Ceiling(Bounds.InnerWidth);
                    height = (int)Math.Ceiling(Bounds.InnerHeight);
                }

            }
            else
            {

                if (autoWidth)
                {
                    string[] lines = text.Split('\n');
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (i == 0)
                        {
                            width = (int)(Font.GetTextExtents(lines[0]).Width + 2 * padding + 1);
                        }
                        else
                        {
                            width = Math.Max(width, (int)(Font.GetTextExtents(lines[0]).Width + 2 * padding + 1));
                        }
                    }

                }
                else
                {
                    width = (int)(scaled(unscaledWidth) + 1);
                }


                height = (int)(textUtil.GetMultilineTextHeight(Font, text, width - 2 * padding) + 2 * padding + 1);

                Bounds.CalcWorldBounds();
            }




            
            

            ImageSurface surface = new ImageSurface(Format.Argb32, width, height);
            Context ctx = genContext(surface);

            ctx.SetSourceRGBA(0, 0, 0, 0);
            ctx.Paint();

            if (Background?.FillColor != null)
            {
                ctx.SetSourceRGBA(Background.FillColor);
                RoundRectangle(ctx, 0, 0, width, height, Background.Radius);
                ctx.Fill();
            }


            if (Background?.Shade == true)
            {
                ctx.SetSourceRGBA(GuiStyle.DialogLightBgColor[0] * 1.4, GuiStyle.DialogStrongBgColor[1] * 1.4, GuiStyle.DialogStrongBgColor[2] * 1.4, 1);
                RoundRectangle(ctx, 0, 0, width, height, Background.Radius);
                ctx.LineWidth = Background.BorderWidth * 1.75;
                ctx.Stroke();
                surface.Blur(8.2);
            }

            if (Background?.BorderColor != null)
            {
                ctx.SetSourceRGBA(Background.BorderColor);
                RoundRectangle(ctx, 0, 0, width, height, Background.Radius);
                ctx.LineWidth = Background.BorderWidth;
                ctx.Stroke();
            }

            textUtil.AutobreakAndDrawMultilineTextAt(ctx, Font, text, (int)padding, (int)padding, width - 2 * padding, textOrientation);

            generateTexture(surface, ref hoverTexture);

            ctx.Dispose();
            surface.Dispose();
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            if (text.Length == 0) return;

            // Compose on demand only
            if (hoverTexture.TextureId == 0 && !hoverTexture.Disposed)
            {
                ComposeHoverElement();
            }

            int mouseX = api.Input.MouseX;
            int mouseY = api.Input.MouseY;

            if ((autoDisplay && Bounds.PointInside(mouseX, mouseY)) || visible)
            {
                double x = Bounds.renderX;
                double y = Bounds.renderY;

                if (followMouse)
                {
                    x = mouseX + 20;
                    y = mouseY + 20;
                }

                if (x + width > api.Render.FrameWidth)
                {
                    x -= (x + width) - api.Render.FrameWidth;
                }

                if (y+height > api.Render.FrameHeight)
                {
                    y -= (y + height) - api.Render.FrameHeight;
                }

                api.Render.Render2DTexturePremultipliedAlpha(hoverTexture.TextureId, (int)x, (int)y, width, height, 50);
            }
        }

        /// <summary>
        /// Sets the text of the component and changes it.
        /// </summary>
        /// <param name="text">The text to change.</param>
        public void SetNewText(string text)
        {
            this.text = text;
            ComposeHoverElement();
        }

        /// <summary>
        /// Sets whether the text automatically displays or not.
        /// </summary>
        /// <param name="on">Whether the text is displayed.</param>
        public void SetAutoDisplay(bool on)
        {
            autoDisplay = on;
        }

        /// <summary>
        /// Sets the visibility to the 
        /// </summary>
        /// <param name="on"></param>
        public void SetVisible(bool on)
        {
            visible = on;
        }

        /// <summary>
        /// Sets whether or not the width of the component should automatiocally adjust.
        /// </summary>
        public void SetAutoWidth(bool on)
        {
            autoWidth = on;
        }

        internal void SetFollowMouse(bool on)
        {
            followMouse = on;
        }

        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            // Can't click this so don't set the Handled value
        }

        public override void Dispose()
        {
            base.Dispose();

            hoverTexture.Dispose();
        }
    }



    public static partial class GuiComposerHelpers
    {

        /// <summary>
        /// Adds a hover text to the GUI.
        /// </summary>
        /// <param name="text">The text of the text.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="width">The width of the text.</param>
        /// <param name="bounds">The bounds of the text.</param>
        /// <param name="key">The name of this hover text component.</param>
        public static GuiComposer AddHoverText(this GuiComposer composer, string text, CairoFont font, int width, ElementBounds bounds, string key = null)
        {
            if (!composer.composed)
            {
                GuiElementHoverText elem = new GuiElementHoverText(composer.Api, text, font, width, bounds);
                elem.Background = GuiElementHoverText.DefaultBackGround;
                composer.AddInteractiveElement(elem, key);
            }
            return composer;
        }

        /// <summary>
        /// Adds a hover text to the GUI.
        /// </summary>
        /// <param name="text">The text of the text.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="width">The width of the text.</param>
        /// <param name="bounds">The bounds of the text.</param>
        /// <param name="key">The name of this hover text component.</param>
        public static GuiComposer AddHoverText(this GuiComposer composer, string text, CairoFont font, int width, ElementBounds bounds, TextBackground background, string key = null)
        {
            if (!composer.composed)
            {
                GuiElementHoverText elem = new GuiElementHoverText(composer.Api, text, font, width, bounds);
                elem.Background = background;
                composer.AddInteractiveElement(elem, key);
            }
            return composer;
        }

        /// <summary>
        /// Fetches the hover text component by name.
        /// </summary>
        /// <param name="key">The name of the text component.</param>
        public static GuiElementHoverText GetHoverText(this GuiComposer composer, string key)
        {
            return (GuiElementHoverText)composer.GetElement(key);
        }
    }

}
