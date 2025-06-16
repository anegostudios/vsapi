using Cairo;

#nullable disable

namespace Vintagestory.API.Client
{
    public class GuiElementStaticText : GuiElementTextBase
    {
        internal EnumTextOrientation orientation;
        public double offsetX;
        public double offsetY;

        /// <summary>
        /// Creates a new GUIElementStaticText.
        /// </summary>
        /// <param name="capi">The Client API</param>
        /// <param name="text">The text of the Element</param>
        /// <param name="orientation">The orientation of the text.</param>
        /// <param name="bounds">The bounds of the element.</param>
        /// <param name="font">The font of the text.</param>
        public GuiElementStaticText(ICoreClientAPI capi, string text, EnumTextOrientation orientation, ElementBounds bounds, CairoFont font) : base(capi, text, font, bounds)
        {
            this.orientation = orientation;
        }

        public double GetTextHeight()
        {
            return textUtil.GetMultilineTextHeight(Font, text, Bounds.InnerWidth);
        }

        public override void ComposeTextElements(Context ctx, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();

            Bounds.absInnerHeight = textUtil.AutobreakAndDrawMultilineTextAt(
                ctx, Font, text, (int)(offsetX + Bounds.drawX), (int)(offsetY + Bounds.drawY), 
                Bounds.InnerWidth, 
                orientation
            );
        }


        /// <summary>
        /// Resize element bounds so that the text fits in one line
        /// </summary>
        /// <param name="onlyGrow"></param>
        public void AutoBoxSize(bool onlyGrow = false)
        {
            Font.AutoBoxSize(text, Bounds, onlyGrow);
        }

        public void SetValue(string text)
        {
            this.text = text;
        }

        /// <summary>
        /// Resize the font so that the text fits in one line
        /// </summary>
        public void AutoFontSize(bool onlyShrink = true)
        {
            Bounds.CalcWorldBounds();
            Font.AutoFontSize(text, Bounds, onlyShrink);
        }
    }


    public static partial class GuiComposerHelpers
    {
        /// <summary>
        /// Adds a static text component to the GUI
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="text">The text of the text component.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="bounds">The bounds of the text container.</param>
        /// <param name="key">The name of the component.</param>
        public static GuiComposer AddStaticText(this GuiComposer composer, string text, CairoFont font, ElementBounds bounds, string key = null)
        {
            if (!composer.Composed)
            {
                composer.AddStaticElement(new GuiElementStaticText(composer.Api, text, font.Orientation, bounds, font), key);
            }
            return composer;
        }

        /// <summary>
        /// Adds a static text component to the GUI
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="text">The text of the text component.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="orientation">The orientation of the text.</param>
        /// <param name="bounds">The bounds of the text container.</param>
        /// <param name="key">The name of the component.</param>
        public static GuiComposer AddStaticText(this GuiComposer composer, string text, CairoFont font, EnumTextOrientation orientation, ElementBounds bounds, string key = null)
        {
            if (!composer.Composed)
            {
                composer.AddStaticElement(new GuiElementStaticText(composer.Api, text, orientation, bounds, font), key);
            }
            return composer;
        }

        /// <summary>
        /// Adds a static text component to the GUI that automatically resizes as necessary.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="text">The text of the text component.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="orientation">The orientation of the text.</param>
        /// <param name="bounds">The bounds of the text container.</param>
        /// <param name="key">The name of the component.</param>
        public static GuiComposer AddStaticTextAutoBoxSize(this GuiComposer composer, string text, CairoFont font, EnumTextOrientation orientation, ElementBounds bounds, string key = null)
        {
            if (!composer.Composed)
            {
                GuiElementStaticText elem = new GuiElementStaticText(composer.Api, text, orientation, bounds, font);
                composer.AddStaticElement(elem, key);
                elem.AutoBoxSize();
            }
            return composer;
        }


        /// <summary>
        /// Adds a static text component to the GUI that automatically resizes as necessary.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="text">The text of the text component.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="bounds">The bounds of the text container.</param>
        /// <param name="key">The name of the component.</param>
        public static GuiComposer AddStaticTextAutoFontSize(this GuiComposer composer, string text, CairoFont font, ElementBounds bounds, string key = null)
        {
            if (!composer.Composed)
            {
                GuiElementStaticText elem = new GuiElementStaticText(composer.Api, text, font.Orientation, bounds, font);
                composer.AddStaticElement(elem, key);
                elem.AutoFontSize();
            }
            return composer;
        }

        /// <summary>
        /// Gets the static text component by name.
        /// </summary>
        /// <param name="composer" />
        /// <param name="key">The name of the component.</param>
        public static GuiElementStaticText GetStaticText(this GuiComposer composer, string key)
        {
            return (GuiElementStaticText)composer.GetElement(key);
        }

    }

}
