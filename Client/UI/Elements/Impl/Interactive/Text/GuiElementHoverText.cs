using Cairo;
using System;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    public class GuiElementHoverText : GuiElementTextBase
    {
        public static TextBackground DefaultBackground = new TextBackground() {
            Padding = 5,
            Radius = 1,
            FillColor = GuiStyle.DialogStrongBgColor,
            BorderColor = GuiStyle.DialogBorderColor,
            BorderWidth = 3,
            Shade = true
        };
        
        LoadedTexture hoverTexture;
        int unscaledMaxWidth;
        
        double hoverWidth, hoverHeight;

        bool autoDisplay = true;
        bool visible = false;
        bool isnowshown;
        bool followMouse = true;
        bool autoWidth = false;
        public bool fillBounds = false;

        public TextBackground Background;

        Vec4f rendercolor;
        public Vec4f RenderColor { 
            get { return rendercolor; }
            set { rendercolor = value; descriptionElement.RenderColor = value; }
        }

        double padding;
        public float ZPosition
        {
            get
            {
                return zPosition;
            }
            set
            {
                zPosition = value;
                descriptionElement.zPos = value;
            }
        }
        float zPosition = 500;

        GuiElementRichtext descriptionElement;

        public bool IsVisible => visible;
        public bool IsNowShown => isnowshown;

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
        /// <param name="maxWidth">The width of the text.</param>
        /// <param name="bounds">the bounds of the text.</param>
        /// <param name="background"></param>
        public GuiElementHoverText(ICoreClientAPI capi, string text, CairoFont font, int maxWidth, ElementBounds bounds, TextBackground background = null) : base(capi, text, font, bounds)
        {
            this.Background = background;
            if (background == null)
            {
                this.Background = GuiElementHoverText.DefaultBackground;
            }
            this.unscaledMaxWidth = maxWidth;

            hoverTexture = new LoadedTexture(capi);

            padding = Background.HorPadding;
            ElementBounds descBounds = bounds.CopyOnlySize();
            descBounds.WithFixedPadding(0);
            descBounds.WithParent(bounds);
            descBounds.IsDrawingSurface = true;
            descBounds.fixedWidth = maxWidth;

            descriptionElement = new GuiElementRichtext(capi, Array.Empty<RichTextComponentBase>(), descBounds);
            descriptionElement.zPos = 1001;
            
        }

        public override void BeforeCalcBounds()
        {
            base.BeforeCalcBounds();

            descriptionElement.BeforeCalcBounds();
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            
        }

        public override int OutlineColor()
        {
            return 0 + (255 << 8) + (255 << 16) + (128 << 24);
        }

        public override void RenderBoundsDebug()
        {
            api.Render.RenderRectangle((int)Bounds.renderX, (int)Bounds.renderY, 550, (int)Bounds.OuterWidth, (int)Bounds.OuterHeight, OutlineColor());
        }

        void RecalcBounds()
        {
            double currentWidth = descriptionElement.Bounds.fixedWidth;
            currentWidth =  Math.Min(autoWidth ? descriptionElement.MaxLineWidth / RuntimeEnv.GUIScale : currentWidth, unscaledMaxWidth);
            hoverWidth = currentWidth + 2 * padding;

            // Height depends on the width
            double descTextHeight = descriptionElement.Bounds.fixedHeight + 2 * padding;
            double currentHeight = Math.Max(descTextHeight, 20);
            
            hoverHeight = scaled(currentHeight);
            hoverWidth = scaled(hoverWidth);
        }


        void Recompose()
        {
            descriptionElement.SetNewText(text, Font);
            RecalcBounds();
            Bounds.CalcWorldBounds();

            ElementBounds textBounds = Bounds.CopyOnlySize();
            textBounds.CalcWorldBounds();

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)Math.Ceiling(hoverWidth), (int)Math.Ceiling(hoverHeight));
            Context ctx = genContext(surface);

            ctx.SetSourceRGBA(0, 0, 0, 0);
            ctx.Paint();

            if (Background?.FillColor != null)
            {
                ctx.SetSourceRGBA(Background.FillColor);
                RoundRectangle(ctx, 0, 0, hoverWidth, hoverHeight, Background.Radius);
                ctx.Fill();
            }

            if (Background?.Shade == true)
            {
                ctx.SetSourceRGBA(GuiStyle.DialogLightBgColor[0] * 1.4, GuiStyle.DialogStrongBgColor[1] * 1.4, GuiStyle.DialogStrongBgColor[2] * 1.4, 1);
                RoundRectangle(ctx, 0, 0, hoverWidth, hoverHeight, Background.Radius);
                ctx.LineWidth = Background.BorderWidth * 1.75;
                ctx.Stroke();
                surface.BlurFull(8.2);
            }

            if (Background?.BorderColor != null)
            {
                ctx.SetSourceRGBA(Background.BorderColor);
                RoundRectangle(ctx, 0, 0, hoverWidth, hoverHeight, Background.Radius);
                ctx.LineWidth = Background.BorderWidth;
                ctx.Stroke();
            }


            generateTexture(surface, ref hoverTexture);

            ctx.Dispose();
            surface.Dispose();
        }



        public override void RenderInteractiveElements(float deltaTime)
        {
            if (text == null || text.Length == 0) return;

            if (api.Render.ScissorStack.Count > 0)
            {
                api.Render.GlScissorFlag(false);
            }

            int mouseX = api.Input.MouseX;
            int mouseY = api.Input.MouseY;
            isnowshown = false;

            if ((autoDisplay && IsPositionInside(mouseX, mouseY)) || visible)
            {
                isnowshown = true;
                // Compose on demand only
                if (hoverTexture.TextureId == 0 && !hoverTexture.Disposed)
                {
                    Recompose();
                }

                int pad = (int)scaled(padding);

                double x = Bounds.renderX;
                double y = Bounds.renderY;

                if (followMouse)
                {
                    x = mouseX + GuiElement.scaled(10);
                    y = mouseY + GuiElement.scaled(15);
                }

                if (x + hoverWidth > api.Render.FrameWidth)
                {
                    x -= (x + hoverWidth) - api.Render.FrameWidth;
                }

                if (y + hoverHeight > api.Render.FrameHeight)
                {
                    y -= (y + hoverHeight) - api.Render.FrameHeight;
                }

                api.Render.Render2DTexture(hoverTexture.TextureId, (int)x + (int)Bounds.absPaddingX, (int)y + (int)Bounds.absPaddingY, (int)hoverWidth + 1, (int)hoverHeight + 1, zPosition, RenderColor);

                Bounds.renderOffsetX = x - Bounds.renderX + pad;
                Bounds.renderOffsetY = y - Bounds.renderY + pad;
                descriptionElement.RenderColor = rendercolor;
                descriptionElement.RenderAsPremultipliedAlpha = RenderAsPremultipliedAlpha;
                descriptionElement.RenderInteractiveElements(deltaTime);
                Bounds.renderOffsetX = 0;
                Bounds.renderOffsetY = 0;
            }

            if (api.Render.ScissorStack.Count > 0)
            {
                api.Render.GlScissorFlag(true);
            }
        }

        /// <summary>
        /// Sets the text of the component and changes it.
        /// </summary>
        /// <param name="text">The text to change.</param>
        public void SetNewText(string text)
        {
            this.text = text;
            Recompose();
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

        public void SetFollowMouse(bool on)
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
            descriptionElement.Dispose();
        }
    }



    public static partial class GuiComposerHelpers
    {
        /// <summary>
        /// Adds a hover text to the GUI.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="text">The text of the text.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="width">The width of the text.</param>
        /// <param name="bounds">The bounds of the text.</param>
        /// <param name="key">The name of this hover text component.</param>
        public static GuiComposer AddHoverText(this GuiComposer composer, string text, CairoFont font, int width, ElementBounds bounds, string key = null)
        {
            if (!composer.Composed)
            {
                GuiElementHoverText elem = new GuiElementHoverText(composer.Api, text, font, width, bounds, null);

                composer.AddInteractiveElement(elem, key);
            }
            return composer;
        }

        public static GuiComposer AddAutoSizeHoverText(this GuiComposer composer, string text, CairoFont font, int width, ElementBounds bounds, string key = null)
        {
            if (!composer.Composed)
            {
                GuiElementHoverText elem = new GuiElementHoverText(composer.Api, text, font, width, bounds, null);
                elem.SetAutoWidth(true);

                composer.AddInteractiveElement(elem, key);
            }
            return composer;
        }

        /// <summary>
        /// Adds a hover text to the GUI.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="text">The text of the text.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="width">The width of the text.</param>
        /// <param name="bounds">The bounds of the text.</param>
        /// <param name="key">The name of this hover text component.</param>
        public static GuiComposer AddTranspHoverText(this GuiComposer composer, string text, CairoFont font, int width, ElementBounds bounds, string key = null)
        {
            if (!composer.Composed)
            {
                GuiElementHoverText elem = new GuiElementHoverText(composer.Api, text, font, width, bounds, new TextBackground());
                
                composer.AddInteractiveElement(elem, key);
            }
            return composer;
        }

        /// <summary>
        /// Adds a hover text to the GUI.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="text">The text of the text.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="width">The width of the text.</param>
        /// <param name="bounds">The bounds of the text.</param>
        /// <param name="background"></param>
        /// <param name="key">The name of this hover text component.</param>
        public static GuiComposer AddHoverText(this GuiComposer composer, string text, CairoFont font, int width, ElementBounds bounds, TextBackground background, string key = null)
        {
            if (!composer.Composed)
            {
                GuiElementHoverText elem = new GuiElementHoverText(composer.Api, text, font, width, bounds, background);
                composer.AddInteractiveElement(elem, key);
            }
            return composer;
        }

        /// <summary>
        /// Fetches the hover text component by name.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="key">The name of the text component.</param>
        public static GuiElementHoverText GetHoverText(this GuiComposer composer, string key)
        {
            return (GuiElementHoverText)composer.GetElement(key);
        }
    }

}
