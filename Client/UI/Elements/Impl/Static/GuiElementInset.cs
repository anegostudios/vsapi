using Cairo;

#nullable disable

namespace Vintagestory.API.Client
{
    class GuiElementInset : GuiElement
    {
        int depth;
        float brightness;

        /// <summary>
        /// Creates a new inset for the GUI.
        /// </summary>
        /// <param name="capi">The Client API</param>
        /// <param name="bounds">The bounds of the Element.</param>
        /// <param name="depth">The depth of the element.</param>
        /// <param name="brightness">The brightness of the inset.</param>
        public GuiElementInset(ICoreClientAPI capi, ElementBounds bounds, int depth, float brightness) : base(capi, bounds)
        {
            this.depth = depth;
            this.brightness = brightness;
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();

            if (brightness < 1)
            {
                ctx.SetSourceRGBA(0, 0, 0, 1 - brightness);
                Rectangle(ctx, Bounds);
                ctx.Fill();
            }

            EmbossRoundRectangleElement(ctx, Bounds, true, depth);
        }
    }



    public static class GuiElementInsetHelper
    {
        /// <summary>
        /// Adds an inset to the current GUI.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="bounds">The bounds of the inset.</param>
        /// <param name="depth">The depth of the inset.</param>
        /// <param name="brightness">The brightness of the inset.</param>
        public static GuiComposer AddInset(this GuiComposer composer, ElementBounds bounds, int depth = 4, float brightness = 0.85f)
        {
            if (!composer.Composed)
            {
                composer.AddStaticElement(new GuiElementInset(composer.Api, bounds, depth, brightness));
            }
            return composer;
        }
        
    }
}
