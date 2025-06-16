using Cairo;

#nullable disable

namespace Vintagestory.API.Client
{
    class GuiElementClip : GuiElement
    {
        bool clip;

        /// <summary>
        /// Adds a clipped area to the GUI.
        /// </summary>
        /// <param name="capi">The Client API</param>
        /// <param name="clip">Do we clip?</param>
        /// <param name="bounds">The bounds of the element.</param>
        public GuiElementClip(ICoreClientAPI capi, bool clip, ElementBounds bounds) : base(capi, bounds)
        {
            this.clip = clip;
        }

        public override void ComposeElements(Context ctxStatic, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            if (clip)
            {
               api.Render.PushScissor(Bounds);
            } else
            {
                api.Render.PopScissor();
            }
        }

        public override int OutlineColor()
        {
            return (255 << 16) + (255 << 24);
        }


        public override void OnMouseDown(ICoreClientAPI api, MouseEvent mouse)
        {
            // Can't be interacted with
        }

    }



    public static class GuiElementClipHelpler
    {
        /// <summary>
        /// Add a clip area. Thhis select an area to be rendered, where anything outside will be invisible. Useful for scrollable content. Can be called multiple times, to reduce the render area further, but needs an equal amount of calls to EndClip()
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="bounds">The bounds of the object.</param>
        public static GuiComposer BeginClip(this GuiComposer composer, ElementBounds bounds)
        {
            if (!composer.Composed)
            {
                composer.AddInteractiveElement(new GuiElementClip(composer.Api, true, bounds));
                composer.InsideClipBounds = bounds;
                composer.BeginChildElements();
            }
            return composer;
        }

        /// <summary>
        /// Remove a previously added clip area.
        /// </summary>
        public static GuiComposer EndClip(this GuiComposer composer)
        {
            if (!composer.Composed)
            {
                composer.AddInteractiveElement(new GuiElementClip(composer.Api, false, ElementBounds.Empty));
                composer.InsideClipBounds = null;
                composer.EndChildElements();
            }
            return composer;
        }


    }
}
