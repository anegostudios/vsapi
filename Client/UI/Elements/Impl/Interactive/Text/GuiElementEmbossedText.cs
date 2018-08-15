using Cairo;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public class GuiElementEmbossedText : GuiElementTextBase
    {
        public static int Padding = 4;

        bool enabled;

        LoadedTexture texture;

            
        public GuiElementEmbossedText(ICoreClientAPI capi, string text, CairoFont font, ElementBounds bounds) : base(capi, text, font, bounds) 
        {
            texture = new LoadedTexture(capi);
            enabled = true;
        }

        public bool IsEnabled()
        {
            return enabled;
        }

        public void SetEnabled(bool enabled)
        {
            this.enabled = enabled;
            Compose();
        }

        public override void ComposeTextElements(Context ctxStatic, ImageSurface surfaceStatic)
        {
            Compose();
        }

        internal void AutoBoxSize()
        {
            Font.AutoBoxSize(text, Bounds);
            Bounds.FixedGrow(2 * Padding);
        }

        void Compose() {
            Bounds.CalcWorldBounds();

            double shadowOffset = scaled(1.5) / 2;
            double shineOffset = -scaled(1.5) / 2;
            double padding = scaled(Padding);
            
            if (!enabled)
            {
                shadowOffset /= 2;
                shineOffset /= 2;
            }

            ImageSurface surface = new ImageSurface(Format.Argb32, Bounds.OuterWidthInt, Bounds.OuterHeightInt);
            Context ctxText = new Context(surface);

            ctxText.SetSourceRGBA(0, 0, 0, 0);
            ctxText.Paint();

            ctxText.Operator = Operator.Source;
            ctxText.Antialias = Antialias.Best;

            Font.Color = new double[] { 0, 0, 0, 0.4 };
            Font.SetupContext(ctxText);

            ctxText.SetSourceRGBA(0, 0, 0, 0.95);
            ctxText.MoveTo(padding + shadowOffset, padding + shadowOffset);
            ShowTextCorrectly(ctxText, text);

            ctxText.SetSourceRGBA(255, 255, 255, enabled ? 0.95 : 0.5);
            ctxText.MoveTo(padding + shineOffset, padding + shineOffset);
            ShowTextCorrectly(ctxText, text);


            surface.Blur(3, 0, 0, Bounds.OuterWidthInt, Bounds.OuterHeightInt);

            ctxText.Operator = Operator.Source;
            if (enabled)
            {
                Font.Color = new double[] { 0, 0, 0, 0.5 };
            } else
            {
                Font.Color = new double[] { 0.5, 0.5, 0.5, 0.75 };
            }
            
            Font.SetupContext(ctxText);
            ctxText.MoveTo(padding, padding);
            ShowTextCorrectly(ctxText, text);

            generateTexture(surface, ref texture);
            
            surface.Dispose();
            ctxText.Dispose();
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            api.Render.Render2DTexturePremultipliedAlpha(texture.TextureId, Bounds);
        }


        public override void Dispose()
        {
            base.Dispose();

            texture.Dispose();
        }

    }


    public static partial class GuiComposerHelpers
    {

        public static GuiComposer AddEmbossedText(this GuiComposer composer, string text, CairoFont font, ElementBounds bounds, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementEmbossedText(composer.Api, text, font, bounds), key);
            }
            return composer;
        }

        public static GuiElementEmbossedText GetEmbossedText(this GuiComposer composer, string key)
        {
            return (GuiElementEmbossedText)composer.GetElement(key);
        }

    }

}
