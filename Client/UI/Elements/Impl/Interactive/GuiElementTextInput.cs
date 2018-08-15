using Cairo;
using System;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public class GuiElementTextInput : GuiElementEditableTextBase
    {
        protected LoadedTexture highlightTexture;
        protected ElementBounds highlightBounds;

        internal bool DeleteOnRefocusBackSpace;

        protected int refocusStage = 0;

        public GuiElementTextInput(ICoreClientAPI capi, ElementBounds bounds, API.Common.Action<string>OnTextChanged, CairoFont font) : base(capi, font, bounds)
        {
            this.OnTextChanged = OnTextChanged;
            highlightTexture = new LoadedTexture(capi);
        }

        public void HideCharacters()
        {
            hideCharacters = true;
        }

        public override void ComposeTextElements(Context ctx, ImageSurface surface)
        {
            EmbossRoundRectangleElement(ctx, Bounds, true, 2, 3);
            ctx.SetSourceRGBA(0, 0, 0, 0.3);
            ElementRoundRectangle(ctx, Bounds, false, 3);
            ctx.Fill();

            
            ImageSurface surfaceHighlight = new ImageSurface(Format.Argb32, (int)Bounds.OuterWidth, (int)Bounds.OuterHeight);
            Context ctxHighlight = genContext(surfaceHighlight);

            ctxHighlight.SetSourceRGBA(0, 0, 0, 0.2);
            ctxHighlight.Paint();

            generateTexture(surfaceHighlight, ref highlightTexture);

            ctxHighlight.Dispose();
            surfaceHighlight.Dispose();

            highlightBounds = Bounds.CopyOffsetedSibling().WithFixedPadding(0, 0).FixedGrow(2 * Bounds.absPaddingX, 2 * Bounds.absPaddingY);
            highlightBounds.CalcWorldBounds();

            RecomposeText();
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            if (HasFocus)
            {
                api.Render.Render2DTexture(highlightTexture.TextureId, highlightBounds);
            }

            api.Render.GlScissor(
                (int)(Bounds.renderX), 
                (int)(api.Render.FrameHeight - Bounds.renderY - Bounds.InnerHeight), 
                Math.Max(0, Bounds.OuterWidthInt + 1 - (int)rightSpacing), 
                Math.Max(0, Bounds.OuterHeightInt + 1 - (int)bottomSpacing)
            );

            api.Render.GlScissorFlag(true);
            api.Render.Render2DTexturePremultipliedAlpha(textTexture.TextureId, Bounds.renderX - renderLeftOffset, Bounds.renderY, textSize.X, textSize.Y);
            api.Render.GlScissorFlag(false);

            base.RenderInteractiveElements(deltaTime);
        }

        public override void OnFocusLost()
        {
            base.OnFocusLost();
        }

        public override void OnFocusGained()
        {
            base.OnFocusGained();
        }

        public override void OnKeyDown(ICoreClientAPI api, KeyEvent args)
        {
            if (DeleteOnRefocusBackSpace && args.KeyCode == (int)GlKeys.BackSpace)
            {
                SetValue("");
                return;
            }

            base.OnKeyDown(api, args);
        }

        public override void Dispose()
        {
            base.Dispose();
            highlightTexture.Dispose();
        }
    }



    public static partial class GuiComposerHelpers
    {
        public static GuiComposer AddTextInput(this GuiComposer composer, ElementBounds bounds, API.Common.Action<string> OnTextChanged, CairoFont font = null, string key = null)
        {
            if (font == null)
            {
                font = CairoFont.TextInput();
            }

            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementTextInput(composer.Api, bounds, OnTextChanged, font), key);
            }

            return composer;
        }

        public static GuiElementTextInput GetTextInput(this GuiComposer composer, string key)
        {
            return (GuiElementTextInput)composer.GetElement(key);
        }



    }

}
