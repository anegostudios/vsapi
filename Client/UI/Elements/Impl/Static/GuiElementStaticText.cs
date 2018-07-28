using Cairo;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public class GuiElementStaticText : GuiElementTextBase
    {
        internal EnumTextOrientation orientation;
        public double offsetX;
        public double offsetY;

        public GuiElementStaticText(ICoreClientAPI capi, string text, EnumTextOrientation orientation, ElementBounds bounds, CairoFont font) : base(capi, text, font, bounds)
        {
            this.orientation = orientation;
        }

        public override void ComposeTextElements(Context ctx, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();

            Bounds.absInnerHeight = ShowMultilineText(ctx, text, (int)(offsetX + Bounds.drawX), (int)(offsetY + Bounds.drawY), Bounds.InnerWidth, orientation);
        }

        internal double GetMultilineTextHeight(double lineHeightMultiplier = 1f)
        {
            return GetMultilineTextHeight(text, Bounds.InnerWidth, lineHeightMultiplier);
        }

        internal void AutoBoxSize(bool onlyGrow = false)
        {
            Font.AutoBoxSize(text, Bounds, onlyGrow);
        }

        public override void SetValue(string text)
        {
            this.text = text;
        }
    }


    public static partial class GuiComposerHelpers
    {

        public static GuiComposer AddStaticText(this GuiComposer composer, string text, CairoFont font, ElementBounds bounds, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddStaticElement(new GuiElementStaticText(composer.Api, text, EnumTextOrientation.Left, bounds, font), key);
            }
            return composer;
        }


        public static GuiComposer AddStaticText(this GuiComposer composer, string text, CairoFont font, EnumTextOrientation orientation, ElementBounds bounds, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddStaticElement(new GuiElementStaticText(composer.Api, text, orientation, bounds, font), key);
            }
            return composer;
        }

        public static GuiComposer AddStaticTextAutoBoxSize(this GuiComposer composer, string text, CairoFont font, EnumTextOrientation orientation, ElementBounds bounds, string key = null)
        {
            if (!composer.composed)
            {
                GuiElementStaticText elem = new GuiElementStaticText(composer.Api, text, orientation, bounds, font);
                composer.AddStaticElement(elem, key);
                elem.AutoBoxSize();
            }
            return composer;
        }

        public static GuiElementStaticText GetStaticText(this GuiComposer composer, string key)
        {
            return (GuiElementStaticText)composer.GetElement(key);
        }

    }

}
