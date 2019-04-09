using System;
using Cairo;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public class GuiElementTextArea : GuiElementEditableTextBase
    {
        double minHeight;
        LoadedTexture highlightTexture;
        ElementBounds highlightBounds;

        /// <summary>
        /// Creates a new text area.
        /// </summary>
        /// <param name="capi">The client API</param>
        /// <param name="bounds">The bounds of the text area.</param>
        /// <param name="OnTextChanged">The event fired when the text is changed.</param>
        /// <param name="font">The font of the text.</param>
        public GuiElementTextArea(ICoreClientAPI capi, ElementBounds bounds, API.Common.Action<string> OnTextChanged, CairoFont font) : base(capi, font, bounds)
        {
            highlightTexture = new LoadedTexture(capi);
            multilineMode = true;
            minHeight = bounds.fixedHeight;
            this.OnTextChanged = OnTextChanged;
        }
        
        internal override void TextChanged()
        {
            Bounds.fixedHeight = Math.Max(minHeight, textUtil.GetMultilineTextHeight(Font, string.Join("\n", lines), Bounds.InnerWidth));
            Bounds.CalcWorldBounds();
            base.TextChanged();
        }

        public override void ComposeTextElements(Context ctx, ImageSurface surface)
        {
            EmbossRoundRectangleElement(ctx, Bounds, true, 3);
            ctx.SetSourceRGBA(0, 0, 0, 0.2f);
            ElementRoundRectangle(ctx, Bounds, true, 3);
            ctx.Fill();

            GenerateHighlight();

            RecomposeText();
        }

        void GenerateHighlight()
        {
            ImageSurface surfaceHighlight = new ImageSurface(Format.Argb32, (int)Bounds.OuterWidth, (int)Bounds.OuterHeight);
            Context ctxHighlight = genContext(surfaceHighlight);

            ctxHighlight.SetSourceRGBA(1, 1, 1, 0.1);
            ctxHighlight.Paint();

            generateTexture(surfaceHighlight, ref highlightTexture);

            ctxHighlight.Dispose();
            surfaceHighlight.Dispose();

            highlightBounds = Bounds.FlatCopy().FixedGrow(6, 6);
            highlightBounds.CalcWorldBounds();
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            if (HasFocus)
            {
                api.Render.Render2DTexturePremultipliedAlpha(highlightTexture.TextureId, highlightBounds);
            }

            api.Render.Render2DTexturePremultipliedAlpha(textTexture.TextureId, Bounds);

            base.RenderInteractiveElements(deltaTime);
        }

        /// <summary>
        /// Sets the number of lines in the Text Area.
        /// </summary>
        /// <param name="maxlines">The maximum number of lines.</param>
        public void SetMaxLines(int maxlines)
        {
            this.maxlines = maxlines;
        }

        public override void Dispose()
        {
            base.Dispose();
            highlightTexture.Dispose();
        }
    }



    public static partial class GuiComposerHelpers
    {
        /// <summary>
        /// Adds a text area to the GUI.  
        /// </summary>
        /// <param name="bounds">The bounds of the Text Area</param>
        /// <param name="OnTextChanged">The event fired when the text is changed.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="key">The name of the text area.</param>
        public static GuiComposer AddTextArea(this GuiComposer composer, ElementBounds bounds, API.Common.Action<string> OnTextChanged, CairoFont font = null, string key = null)
        {
            if (font == null)
            {
                font = CairoFont.SmallTextInput();
            }

            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementTextArea(composer.Api, bounds, OnTextChanged, font), key);
            }

            return composer;
        }

        /// <summary>
        /// Gets the text area by name.
        /// </summary>
        /// <param name="key">The name of the text area.</param>
        /// <returns>The named Text Area.</returns>
        public static GuiElementTextArea GetTextArea(this GuiComposer composer, string key)
        {
            return (GuiElementTextArea)composer.GetElement(key);
        }



    }
}
