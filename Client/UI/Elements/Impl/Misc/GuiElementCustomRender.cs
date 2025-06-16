using Cairo;

#nullable disable

namespace Vintagestory.API.Client
{
    public delegate void RenderDelegateWithBounds(float deltaTime, ElementBounds currentBounds);

    public class GuiElementCustomRender : GuiElement
    {
        RenderDelegateWithBounds onRender;

        /// <summary>
        /// Adds a custom drawing element to the GUI
        /// </summary>
        /// <param name="capi">The Client API</param>
        /// <param name="bounds">The bounds of the Element</param>
        /// <param name="onRender">The event fired when the object is drawn.</param>
        public GuiElementCustomRender(ICoreClientAPI capi, ElementBounds bounds, RenderDelegateWithBounds onRender) : base(capi, bounds)
        {
            this.onRender = onRender;
        }

        public override void ComposeElements(Context ctxStatic, ImageSurface surfaceStatic)
        {
            Bounds.CalcWorldBounds();
        }

        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            // Don't handle this event
        }


        public override void RenderInteractiveElements(float deltaTime)
        {
            onRender(deltaTime, Bounds);
        }
    }

    public static partial class GuiComposerHelpers
    {
        /// <summary>
        /// Adds a static custom draw component to the GUI.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="bounds">The bounds of the component.</param>
        /// <param name="onRender">The event fired when the element is drawn.</param>
        public static GuiComposer AddCustomRender(this GuiComposer composer, ElementBounds bounds, RenderDelegateWithBounds onRender)
        {
            if (!composer.Composed)
            {
                composer.AddInteractiveElement(new GuiElementCustomRender(composer.Api, bounds, onRender));
            }
            return composer;
        }
        

        public static GuiElementCustomRender GetCustomRender(this GuiComposer composer, string key)
        {
            return (GuiElementCustomRender)composer.GetElement(key);
        }

    }
}
