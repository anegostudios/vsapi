using Cairo;

#nullable disable

namespace Vintagestory.API.Client
{
    public delegate void DrawDelegateWithBounds(Context ctx, ImageSurface surface, ElementBounds currentBounds);

    public class GuiElementCustomDraw : GuiElement
    {
        DrawDelegateWithBounds OnDraw;
        bool interactive = false;

        int texId;

        /// <summary>
        /// Adds a custom drawing element to the GUI
        /// </summary>
        /// <param name="capi">The Client API</param>
        /// <param name="bounds">The bounds of the Element</param>
        /// <param name="OnDraw">The event fired when the object is drawn.</param>
        /// <param name="interactive">Whether or not the element is able to be interacted with (Default: false)</param>
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

        /// <summary>
        /// Redraws the element.
        /// </summary>
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
        /// <summary>
        /// Adds a static custom draw component to the GUI.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="bounds">The bounds of the component.</param>
        /// <param name="onDraw">The event fired when the element is drawn.</param>
        public static GuiComposer AddStaticCustomDraw(this GuiComposer composer, ElementBounds bounds, DrawDelegateWithBounds onDraw)
        {
            if (!composer.Composed)
            {
                composer.AddStaticElement(new GuiElementCustomDraw(composer.Api, bounds, onDraw));
            }
            return composer;
        }

        /// <summary>
        /// Adds a dynamic custom draw component to the GUI.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="bounds">The bounds of the component.</param>
        /// <param name="onDraw">The event fired when the element is drawn.</param>
        /// <param name="key">The name of the element.</param>
        public static GuiComposer AddDynamicCustomDraw(this GuiComposer composer, ElementBounds bounds, DrawDelegateWithBounds onDraw, string key = null)
        {
            if (!composer.Composed)
            {
                composer.AddInteractiveElement(new GuiElementCustomDraw(composer.Api, bounds, onDraw, true), key);
            }
            return composer;
        }

        public static GuiElementCustomDraw GetCustomDraw(this GuiComposer composer, string key)
        {
            return (GuiElementCustomDraw)composer.GetElement(key);
        }

    }
}
