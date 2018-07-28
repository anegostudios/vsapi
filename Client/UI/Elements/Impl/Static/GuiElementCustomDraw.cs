using Cairo;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public delegate void DrawDelegateWithBounds(Context ctx, ImageSurface surface, ElementBounds currentBounds);

    public class GuiElementCustomDraw : GuiElement
    {
        DrawDelegateWithBounds OnDraw;
        bool interactive = false;

        int texId;

        public GuiElementCustomDraw(ICoreClientAPI capi, ElementBounds bounds, DrawDelegateWithBounds OnDraw, bool interactive = false) : base(capi, bounds)
        {
            this.OnDraw = OnDraw;
            this.interactive = interactive;
        }

        public override void ComposeElements(Context ctxStatic, ImageSurface surfaceStatic)
        {
            Bounds.CalcWorldBounds();

            if (!interactive)
            {                
                OnDraw(ctxStatic, surfaceStatic, Bounds);
            } else
            {
                Redraw();
            }
        }

        public void Redraw()
        {
            ImageSurface surface = new ImageSurface(Format.Argb32, Bounds.OuterWidthInt, Bounds.OuterHeightInt);
            Context ctx = new Context(surface);

            OnDraw(ctx, surface, Bounds);
            generateTexture(surface, ref texId);

            ctx.Dispose();
            surface.Dispose();
        }

        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            // Don't handle this event
        }


        public override void RenderInteractiveElements(float deltaTime)
        {
            if (interactive) api.Render.Render2DTexture(texId, Bounds);
        }
    }

    public static partial class GuiComposerHelpers
    {
        public static GuiComposer AddStaticCustomDraw(this GuiComposer composer, ElementBounds bounds, DrawDelegateWithBounds OnDraw)
        {
            if (!composer.composed)
            {
                composer.AddStaticElement(new GuiElementCustomDraw(composer.Api, bounds, OnDraw));
            }
            return composer;
        }

        public static GuiComposer AddDynamicCustomDraw(this GuiComposer composer, ElementBounds bounds, DrawDelegateWithBounds OnDraw, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementCustomDraw(composer.Api, bounds, OnDraw, true), key);
            }
            return composer;
        }

        public static GuiElementCustomDraw GetCustomDraw(this GuiComposer composer, string key)
        {
            return (GuiElementCustomDraw)composer.GetElement(key);
        }

    }
}
