using Cairo;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public class GuiElementSwitch : GuiElementControl
    {
        API.Common.Action<bool> handler;

        LoadedTexture onTexture;

        public bool On;

        internal double unscaledPadding;
        internal double unscaledSize;

        public override bool Focusable { get { return true; } }

        public GuiElementSwitch(ICoreClientAPI capi, API.Common.Action<bool> OnToggled, ElementBounds bounds, double size = 30, double padding = 5) : base(capi, bounds)
        {
            onTexture = new LoadedTexture(capi);

            bounds.fixedWidth = size;
            bounds.fixedHeight = size;

            this.unscaledPadding = padding;
            this.unscaledSize = size;

            this.handler = OnToggled;
        }

        public override void ComposeElements(Context ctxStatic, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();

            ctxStatic.SetSourceRGBA(0, 0, 0, 0.2);
            RoundRectangle(ctxStatic, Bounds.drawX, Bounds.drawY, Bounds.InnerWidth, Bounds.InnerHeight, 3);
            ctxStatic.Fill();
            EmbossRoundRectangleElement(ctxStatic, Bounds, true, 1, 2);

            genOnTexture();
        }

        private void genOnTexture()
        {
            double size = scaled(unscaledSize - 2 * unscaledPadding);

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)size, (int)size);
            Context ctx = genContext(surface);

            RoundRectangle(ctx, 0, 0, size, size, 3);
            fillWithPattern(api, ctx, waterTextureName);

            generateTexture(surface, ref onTexture);

            ctx.Dispose();
            surface.Dispose();
        }


        public override void RenderInteractiveElements(float deltaTime)
        {
            if (On)
            {
                double size = scaled(unscaledSize - 2 * unscaledPadding);
                double padding = scaled(unscaledPadding);

                api.Render.Render2DTexturePremultipliedAlpha(onTexture.TextureId, Bounds.renderX + padding, Bounds.renderY + padding, (int)size, (int)size);
            }
            
        }

        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            base.OnMouseDownOnElement(api, args);

            On = !On;
            handler(On);
            api.Gui.PlaySound("toggleswitch");
        }

        public void SetValue(bool on)
        {
            On = on;
        }

        public override void Dispose()
        {
            base.Dispose();

            onTexture.Dispose();
        }

    }

    public static partial class GuiComposerHelpers
    {

        public static GuiComposer AddSwitch(this GuiComposer composer, API.Common.Action<bool> onToggle, ElementBounds bounds, string key = null, double size = 30, double padding = 5)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementSwitch(composer.Api, onToggle, bounds, size, padding), key);
            }
            return composer;
        }

        public static GuiElementSwitch GetSwitch(this GuiComposer composer, string key)
        {
            return (GuiElementSwitch)composer.GetElement(key);
        }
    }
}
