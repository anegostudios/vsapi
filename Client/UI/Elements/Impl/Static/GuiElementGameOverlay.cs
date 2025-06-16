using Cairo;

#nullable disable

namespace Vintagestory.API.Client
{
    public class GuiElementGameOverlay : GuiElement
    {
        double[] bgcolor;

        /// <summary>
        /// Creates a new overlay element.
        /// </summary>
        /// <param name="capi">The client API.</param>
        /// <param name="bounds">The bounds of the element.</param>
        /// <param name="bgcolor">The background color of the element.</param>
        public GuiElementGameOverlay(ICoreClientAPI capi, ElementBounds bounds, double[] bgcolor) : base(capi, bounds)
        {
            this.bgcolor = bgcolor;
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();

            ctx.SetSourceRGBA(bgcolor);
            ctx.Rectangle(Bounds.bgDrawX, Bounds.bgDrawY, Bounds.OuterWidth, Bounds.OuterHeight);
            ctx.FillPreserve();
            ShadePath(ctx, 2);
        }


    }



    public static class GuiElementGameOverlyHelper
    {
        /// <summary>
        /// Adds an overlay to the current GUI.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="bounds">The bounds of the overlay.</param>
        /// <param name="backgroundColor">The background color of the overlay.</param>
        public static GuiComposer AddGameOverlay(this GuiComposer composer, ElementBounds bounds, double[] backgroundColor = null)
        {
            if (!composer.Composed)
            {
                if (backgroundColor == null) backgroundColor = GuiStyle.DialogDefaultBgColor;
                composer.AddStaticElement(new GuiElementGameOverlay(composer.Api, bounds, backgroundColor));
            }
            return composer;
        }

    }
}
