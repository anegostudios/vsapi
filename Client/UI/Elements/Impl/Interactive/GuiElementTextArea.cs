using System;
using Cairo;

#nullable disable

namespace Vintagestory.API.Client
{
    public class GuiElementTextArea : GuiElementEditableTextBase
    {
        double minHeight;
        LoadedTexture highlightTexture;
        ElementBounds highlightBounds;

        public bool Autoheight = true;

        /// <summary>
        /// Creates a new text area.
        /// </summary>
        /// <param name="capi">The client API</param>
        /// <param name="bounds">The bounds of the text area.</param>
        /// <param name="OnTextChanged">The event fired when the text is changed.</param>
        /// <param name="font">The font of the text.</param>
        public GuiElementTextArea(ICoreClientAPI capi, ElementBounds bounds, Action<string> OnTextChanged, CairoFont font) : base(capi, font, bounds)
        {
            highlightTexture = new LoadedTexture(capi);
            multilineMode = true;
            minHeight = bounds.fixedHeight;
            this.OnTextChanged = OnTextChanged;
        }
        
        internal override void TextChanged()
        {
            if (Autoheight)
            {
                Bounds.fixedHeight = Math.Max(minHeight, textUtil.GetMultilineTextHeight(Font, string.Join("\n", lines), Bounds.InnerWidth));
            }
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

            highlightBounds = Bounds.FlatCopy();
            highlightBounds.CalcWorldBounds();
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            if (HasFocus)
            {
                api.Render.Render2DTexturePremultipliedAlpha(highlightTexture.TextureId, highlightBounds);
            }

            RenderTextSelection();
            api.Render.Render2DTexturePremultipliedAlpha(textTexture.TextureId, Bounds);

            base.RenderInteractiveElements(deltaTime);
        }


        public override void Dispose()
        {
            base.Dispose();
            highlightTexture.Dispose();
        }

        public void SetFont(CairoFont cairoFont)
        {
            this.Font = cairoFont;
            caretHeight = cairoFont.GetFontExtents().Height;
        }
    }



    public static partial class GuiComposerHelpers
    {
        /// <summary>
        /// Adds a text area to the GUI.  
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="bounds">The bounds of the Text Area</param>
        /// <param name="onTextChanged">The event fired when the text is changed.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="key">The name of the text area.</param>
        public static GuiComposer AddTextArea(this GuiComposer composer, ElementBounds bounds, Action<string> onTextChanged, CairoFont font = null, string key = null)
        {
            if (font == null)
            {
                font = CairoFont.SmallTextInput();
            }

            if (!composer.Composed)
            {
                composer.AddInteractiveElement(new GuiElementTextArea(composer.Api, bounds, onTextChanged, font), key);
            }

            return composer;
        }

        /// <summary>
        /// Gets the text area by name.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="key">The name of the text area.</param>
        /// <returns>The named Text Area.</returns>
        public static GuiElementTextArea GetTextArea(this GuiComposer composer, string key)
        {
            return (GuiElementTextArea)composer.GetElement(key);
        }



    }
}
