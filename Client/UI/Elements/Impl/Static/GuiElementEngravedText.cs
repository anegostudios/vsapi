using Cairo;
using Vintagestory.API.Config;

#nullable disable

namespace Vintagestory.API.Client
{
    class GuiElementEngravedText : GuiElementTextBase
    {
        EnumTextOrientation orientation;

        /// <summary>
        /// Creates a new Engraved Text element.
        /// </summary>
        /// <param name="capi">The client API.</param>
        /// <param name="text">The text on the element.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="bounds">The bounds of the Text Element.</param>
        /// <param name="orientation">The orientation of the text.</param>
        public GuiElementEngravedText(ICoreClientAPI capi, string text, CairoFont font, ElementBounds bounds, EnumTextOrientation orientation = EnumTextOrientation.Left) : base(capi, text, font, bounds) 
        {
            this.orientation = orientation;
        }


        //FreeTypeFontFace fontFace = FreeTypeFontFace.Create(LoadtimeSettings.AssetPath + "/font/" + LoadtimeSettings.GUIFontName, 0);
        //ctx.SetContextFontFace(fontFace);

        public override void ComposeTextElements(Context ctxStatic, ImageSurface surfaceStatic)
        {
            Font.SetupContext(ctxStatic);

            Bounds.CalcWorldBounds();

            ImageSurface insetShadowSurface = new ImageSurface(Format.Argb32, Bounds.ParentBounds.OuterWidthInt, Bounds.ParentBounds.OuterHeightInt);
            Context ctxInsetShadow = new Context(insetShadowSurface);

            ctxInsetShadow.SetSourceRGB(0, 0, 0);
            ctxInsetShadow.Paint();
            Font.Color = new double[] { 20, 20, 20, 0.35f };
            Font.SetupContext(ctxInsetShadow);

            DrawMultilineTextAt(ctxInsetShadow, Bounds.drawX + scaled(2), Bounds.drawY + scaled(2), orientation);


            insetShadowSurface.BlurFull(7);

            ImageSurface surface = new ImageSurface(Format.Argb32, Bounds.ParentBounds.OuterWidthInt, Bounds.ParentBounds.OuterHeightInt);
            Context ctxText = new Context(surface);

            ctxText.Operator = Operator.Source;

            ctxText.Antialias = Antialias.Best;

            Font.Color = new double[] { 0, 0, 0, 0.4 };
            Font.SetupContext(ctxText);

            ctxText.SetSourceRGBA(0, 0, 0, 0.4);
            DrawMultilineTextAt(ctxText, Bounds.drawX - scaled(0.5), Bounds.drawY - scaled(0.5), orientation);

            ctxText.SetSourceRGBA(1, 1, 1, 1);
            DrawMultilineTextAt(ctxText, Bounds.drawX + scaled(1), Bounds.drawY + scaled(1), orientation);

            ctxText.Operator = Operator.Atop;
            ctxText.SetSourceSurface(insetShadowSurface, 0, 0);
            ctxText.Paint();

            ctxInsetShadow.Dispose();
            insetShadowSurface.Dispose();

            ctxText.Operator = Operator.Over;
            Font.Color = new double[] { 0, 0, 0, 0.35 };
            Font.SetupContext(ctxText);
            DrawMultilineTextAt(ctxText, (int)Bounds.drawX, (int)Bounds.drawY, orientation);

            ctxStatic.Antialias = Antialias.Best;
            ctxStatic.Operator = Operator.HardLight;
            ctxStatic.SetSourceSurface(surface, 0, 0);
            ctxStatic.Paint();

            surface.Dispose();
            ctxText.Dispose();

        }


        internal void TextWithSpacing(Context ctx, string text, double x, double y, float spacing)
        {
            foreach (char c in text)
            {
                TextExtents extents = ctx.TextExtents("" + c);
                ctx.MoveTo(x - extents.XBearing, x - extents.YBearing);
                ctx.ShowText("" + c);
                
                x += extents.Width + spacing * RuntimeEnv.GUIScale;
            }
        }

        
    }

    /*public static partial class GuiComposerHelpers
    {
        public static GuiComposer addEngravedText(this GuiComposer composer, string text, CairoFont font, ElementBounds bounds, EnumTextOrientation orientation = EnumTextOrientation.Left, string key = null)
        {
            if (!composer.composed)
            {
                GuiElementEngravedText element = new GuiElementEngravedText(text, font, bounds, orientation);

                composer.AddStaticElement(element, key);
            }
            return composer;
        }
    }*/
}
