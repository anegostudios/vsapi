using Cairo;

#nullable disable

namespace Vintagestory.API.Client
{
    public class GuiElementEmbossedText : GuiElementTextBase
    {
        public static int Padding = 4;
        LoadedTexture texture;

        /// <summary>
        /// Creates a new embossed text element.
        /// </summary>
        /// <param name="capi">The Client API</param>
        /// <param name="text">The text of the component.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="bounds">The bounds of the component.</param>
        public GuiElementEmbossedText(ICoreClientAPI capi, string text, CairoFont font, ElementBounds bounds) : base(capi, text, font, bounds) 
        {
            texture = new LoadedTexture(capi);
            enabled = true;
        }

        /// <summary>
        /// Whether or not the component is enabled.
        /// </summary>
        public bool IsEnabled()
        {
            return enabled;
        }

        /// <summary>
        /// Sets whether or not the component is enabled.
        /// </summary>
        /// <param name="enabled"></param>
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
            DrawTextLineAt(ctxText, text, 0, 0);

            ctxText.SetSourceRGBA(255, 255, 255, enabled ? 0.95 : 0.5);
            ctxText.MoveTo(padding + shineOffset, padding + shineOffset);
            DrawTextLineAt(ctxText, text, 0, 0);


            surface.BlurPartial(3, 6);

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
            DrawTextLineAt(ctxText, text, 0, 0);

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
        /// <summary>
        /// Adds an embossed text component to the GUI.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="text">The text of the component.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="bounds">The bounds of the component.</param>
        /// <param name="key">The name of the component.</param>
        public static GuiComposer AddEmbossedText(this GuiComposer composer, string text, CairoFont font, ElementBounds bounds, string key = null)
        {
            if (!composer.Composed)
            {
                composer.AddInteractiveElement(new GuiElementEmbossedText(composer.Api, text, font, bounds), key);
            }
            return composer;
        }

        /// <summary>
        /// Gets the EmbossedText component by name.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="key">The name of the component.</param>
        /// <returns>the named component of the text.</returns>
        public static GuiElementEmbossedText GetEmbossedText(this GuiComposer composer, string key)
        {
            return (GuiElementEmbossedText)composer.GetElement(key);
        }

    }

}
