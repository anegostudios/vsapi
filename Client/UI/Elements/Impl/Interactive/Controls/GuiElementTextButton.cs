using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Vintagestory.API.Client
{
    public enum EnumButtonStyle
    {
        None,
        MainMenu,
        Normal
    }

    public class GuiElementTextButton : GuiElementControl
    {
        GuiElementStaticText normalText;
        GuiElementStaticText pressedText;


        LoadedTexture hoverTexture;
        
        ActionConsumable onClick;

        bool isOver;

        EnumButtonStyle buttonStyle;
        bool active = false;
        bool currentlyMouseDownOnElement = false;

        public bool PlaySound = true;

        public static double Padding = 2;

        double textOffsetY;

        public override bool Focusable { get { return true; } }

        /// <summary>
        /// Creates a button with text.
        /// </summary>
        /// <param name="capi">The Client API</param>
        /// <param name="text">The text of the button.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="hoverFont">The font of the text when the player is hovering over the button.</param>
        /// <param name="onClick">The event fired when the button is clicked.</param>
        /// <param name="bounds">The bounds of the button.</param>
        /// <param name="style">The style of the button.</param>
        public GuiElementTextButton(ICoreClientAPI capi, string text, CairoFont font, CairoFont hoverFont, ActionConsumable onClick, ElementBounds bounds, EnumButtonStyle style = EnumButtonStyle.Normal) : base(capi, bounds)
        {
            hoverTexture = new LoadedTexture(capi);
            this.buttonStyle = style;

            normalText = new GuiElementStaticText(capi, text, EnumTextOrientation.Center, bounds, font);
            normalText.AutoBoxSize(true);

            pressedText = new GuiElementStaticText(capi, text, EnumTextOrientation.Center, bounds.CopyOnlySize(), hoverFont);
            bounds = normalText.Bounds;

            this.onClick = onClick;
        }

        /// <summary>
        /// Sets the orientation of the text both when clicked and when idle.
        /// </summary>
        /// <param name="orientation">The orientation of the text.</param>
        public void SetOrientation(EnumTextOrientation orientation)
        {
            normalText.orientation = orientation;
            pressedText.orientation = orientation;
        }



        public override void ComposeElements(Context ctxStatic, ImageSurface surfaceStatic)
        {
            ComposeButton(ctxStatic, surfaceStatic, false);


            ImageSurface surface = new ImageSurface(Format.Argb32, (int)Bounds.OuterWidth, (int)Bounds.OuterHeight);

            Context ctx = genContext(surface);

            if (buttonStyle != EnumButtonStyle.None)
            {
                ctx.SetSourceRGBA(0, 0, 0, 0.4);
                ctx.Rectangle(0, 0, Bounds.OuterWidth, Bounds.OuterHeight);
                ctx.Fill();
            }

            pressedText.Bounds.fixedY += textOffsetY;
            pressedText.ComposeElements(ctx, surface);
            pressedText.Bounds.fixedY -= textOffsetY;

            generateTexture(surface, ref hoverTexture);

            ctx.Dispose();
            surface.Dispose();
        }


        void ComposeButton(Context ctx, ImageSurface surface, bool pressed)
        {
            double embossHeight = scaled(2.5);

            if (buttonStyle == EnumButtonStyle.Normal)
            {
                embossHeight = scaled(1.5);
            }


            Bounds.CalcWorldBounds();
            normalText.AutoBoxSize(true);
            pressedText.Bounds = normalText.Bounds.CopyOnlySize();

            if (buttonStyle != EnumButtonStyle.None)
            {
                // Brown background
                Rectangle(ctx, Bounds.bgDrawX, Bounds.bgDrawY, Bounds.OuterWidth, Bounds.OuterHeight);
                ctx.SetSourceRGBA(69 / 255.0, 52 / 255.0, 36 / 255.0, 1);
                ctx.Fill();
            }

            if (buttonStyle == EnumButtonStyle.MainMenu)
            {
                // Top shine
                Rectangle(ctx, Bounds.bgDrawX, Bounds.bgDrawY, Bounds.OuterWidth, embossHeight);
                ctx.SetSourceRGBA(1, 1, 1, 0.15);
                ctx.Fill();
            }

            if (buttonStyle == EnumButtonStyle.Normal)
            {
                // Top shine
                Rectangle(ctx, Bounds.bgDrawX, Bounds.bgDrawY, Bounds.OuterWidth - embossHeight, embossHeight);
                ctx.SetSourceRGBA(1, 1, 1, 0.15);
                ctx.Fill();

                // Left shine
                Rectangle(ctx, Bounds.bgDrawX, Bounds.bgDrawY + embossHeight, embossHeight, Bounds.OuterHeight - embossHeight);
                ctx.SetSourceRGBA(1, 1, 1, 0.15);
                ctx.Fill();
            }


            textOffsetY = 0;//       (bounds.InnerHeight - normalText.Font.GetFontExtents().Height) / 2;
            normalText.Bounds.fixedY += textOffsetY;
            normalText.ComposeElements(ctx, surface);
            normalText.Bounds.fixedY -= textOffsetY;

            // ShowMultilineText changes height
            Bounds.CalcWorldBounds();

            if (buttonStyle == EnumButtonStyle.MainMenu)
            {
                // Bottom shade
                Rectangle(ctx, Bounds.bgDrawX, Bounds.bgDrawY + Bounds.OuterHeight - embossHeight, Bounds.OuterWidth, embossHeight);
                ctx.SetSourceRGBA(0, 0, 0, 0.2);
                ctx.Fill();
            }

            if (buttonStyle == EnumButtonStyle.Normal)
            {
                // Bottom shade
                Rectangle(ctx, Bounds.bgDrawX + embossHeight, Bounds.bgDrawY + Bounds.OuterHeight - embossHeight, Bounds.OuterWidth - 2*embossHeight, embossHeight);
                ctx.SetSourceRGBA(0, 0, 0, 0.2);
                ctx.Fill();

                // Right shade
                Rectangle(ctx, Bounds.bgDrawX + Bounds.OuterWidth - embossHeight, Bounds.bgDrawY, embossHeight, Bounds.OuterHeight);
                ctx.SetSourceRGBA(0, 0, 0, 0.2);
                ctx.Fill();
            }
            

            if (buttonStyle == EnumButtonStyle.Normal)
            {
                //EmbossRoundRectangleElement(ctx, bounds.bgDrawX, bounds.bgDrawY, bounds.OuterWidth, bounds.OuterHeight, false, 2);
            }
        }


        public override void RenderInteractiveElements(float deltaTime)
        {
            if (isOver || currentlyMouseDownOnElement)
            {
                api.Render.Render2DTexturePremultipliedAlpha(
                    hoverTexture.TextureId,
                    normalText.Bounds.renderX,
                    normalText.Bounds.renderY,
                    normalText.Bounds.OuterWidthInt,
                    normalText.Bounds.OuterHeightInt
                );
            }
        }


        public override void OnMouseMove(ICoreClientAPI api, MouseEvent args)
        {
            if ((enabled && Bounds.PointInside(api.Input.MouseX, api.Input.MouseY)) || active)
            {
                if (!isOver && PlaySound) api.Gui.PlaySound("menubutton");
                isOver = true;

            }
            else
            {
                isOver = false;
            }
        }

        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            base.OnMouseDownOnElement(api, args);

            currentlyMouseDownOnElement = true;
        }

        public override void OnMouseUp(ICoreClientAPI api, MouseEvent args)
        {
            base.OnMouseUp(api, args);

            currentlyMouseDownOnElement = false;
        }


        public override void OnMouseUpOnElement(ICoreClientAPI api, MouseEvent args)
        {
            if (enabled && currentlyMouseDownOnElement && Bounds.PointInside(args.X, args.Y) && args.Button == EnumMouseButton.Left)
            {
                if (PlaySound)
                {
                    api.Gui.PlaySound("menubutton_press");
                }
                args.Handled = onClick();
            }

            currentlyMouseDownOnElement = false;
        }

        /// <summary>
        /// Sets the button as active or inactive.
        /// </summary>
        /// <param name="active">Active == clickable</param>
        public void SetActive(bool active)
        {
            this.active = active;
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
        /// Creates a button for the current GUI.
        /// </summary>
        /// <param name="text">The text on the button.</param>
        /// <param name="onClick">The event fired when the button is clicked.</param>
        /// <param name="bounds">The bounds of the button.</param>
        /// <param name="buttonFont">The font of the button.</param>
        /// <param name="style">The style of the button. (Default: Normal)</param>
        /// <param name="orientation">The orientation of the text. (Default: center)</param>
        /// <param name="key">The internal name of the button.</param>
        public static GuiComposer AddButton(this GuiComposer composer, string text, ActionConsumable onClick, ElementBounds bounds, CairoFont buttonFont, EnumButtonStyle style = EnumButtonStyle.Normal, EnumTextOrientation orientation = EnumTextOrientation.Center, string key = null)
        {
            if (!composer.composed)
            {
                CairoFont hoverFont = buttonFont.Clone().WithColor(GuiStyle.ActiveButtonTextColor);
                GuiElementTextButton elem = new GuiElementTextButton(composer.Api, text, buttonFont, hoverFont, onClick, bounds, style);
                elem.SetOrientation(orientation);
                composer.AddInteractiveElement(elem, key);

            }
            return composer;
        }

        /// <summary>
        /// Creates a button for the current GUI.
        /// </summary>
        /// <param name="text">The text on the button.</param>
        /// <param name="onClick">The event fired when the button is clicked.</param>
        /// <param name="bounds">The bounds of the button.</param>
        /// <param name="style">The style of the button. (Default: Normal)</param>
        /// <param name="orientation">The orientation of the text. (Default: center)</param>
        /// <param name="key">The internal name of the button.</param>
        public static GuiComposer AddButton(this GuiComposer composer, string text, ActionConsumable onClick, ElementBounds bounds, EnumButtonStyle style = EnumButtonStyle.Normal, EnumTextOrientation orientation = EnumTextOrientation.Center, string key = null)
        {
            if (!composer.composed)
            {
                GuiElementTextButton elem = new GuiElementTextButton(composer.Api, text, CairoFont.ButtonText(), CairoFont.ButtonPressedText(), onClick, bounds, style);
                elem.SetOrientation(orientation);
                composer.AddInteractiveElement(elem, key);
                
            }
            return composer;
        }

        /// <summary>
        /// Creates a small button for the current GUI.
        /// </summary>
        /// <param name="text">The text on the button.</param>
        /// <param name="onClick">The event fired when the button is clicked.</param>
        /// <param name="bounds">The bounds of the button.</param>
        /// <param name="style">The style of the button. (Default: Normal)</param>
        /// <param name="orientation">The orientation of the text. (Default: center)</param>
        /// <param name="key">The internal name of the button.</param>
        public static GuiComposer AddSmallButton(this GuiComposer composer, string text, ActionConsumable onClick, ElementBounds bounds, EnumButtonStyle style = EnumButtonStyle.Normal, EnumTextOrientation orientation = EnumTextOrientation.Center, string key = null)
        {
            if (!composer.composed)
            {
                CairoFont font1 = CairoFont.ButtonText();
                CairoFont font2 = CairoFont.ButtonPressedText();
                font1.Fontname = GuiStyle.StandardBoldFontName;
                font2.Fontname = GuiStyle.StandardBoldFontName;
                font1.FontWeight = FontWeight.Bold;
                font2.FontWeight = FontWeight.Bold;
                font1.UnscaledFontsize = GuiStyle.SmallFontSize;
                font2.UnscaledFontsize = GuiStyle.SmallFontSize;

                GuiElementTextButton elem = new GuiElementTextButton(composer.Api, text, font1, font2, onClick, bounds, style);
                elem.SetOrientation(orientation);
                composer.AddInteractiveElement(elem, key);
            }
            return composer;
        }

        /// <summary>
        /// Gets the button by name.
        /// </summary>
        /// <param name="key">The name of the button.</param>
        public static GuiElementTextButton GetButton(this GuiComposer composer, string key)
        {
            return (GuiElementTextButton)composer.GetElement(key);
        }
       

    }

}
