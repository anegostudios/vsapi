using Cairo;
using Vintagestory.API.Client;

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
               api.Render.BeginScissor(Bounds);
            } else
            {
                api.Render.EndScissor();
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
        /// Adds a starting clip to the GUI. Purely decorative.
        /// </summary>
        /// <param name="bounds">The bounds of the object.</param>
        public static GuiComposer BeginClip(this GuiComposer composer, ElementBounds bounds)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementClip(composer.Api, true, bounds));
                composer.InsideClipBounds = bounds;
                composer.BeginChildElements();
            }
            return composer;
        }

        /// <summary>
        /// Adds an ending clip to the GUI after the previous element.
        /// </summary>
        public static GuiComposer EndClip(this GuiComposer composer)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementClip(composer.Api, false, ElementBounds.Empty));
                composer.InsideClipBounds = null;
                composer.EndChildElements();
            }
            return composer;
        }


    }
}
