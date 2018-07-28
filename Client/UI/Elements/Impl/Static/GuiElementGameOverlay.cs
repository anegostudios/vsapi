using Cairo;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public class GuiElementGameOverlay : GuiElement
    {
        double[] bgcolor;

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
        public static GuiComposer AddGameOverlay(this GuiComposer composer, ElementBounds bounds, double[] backgroundColor = null)
        {
            if (!composer.composed)
            {
                if (backgroundColor == null) backgroundColor = ElementGeometrics.DialogDefaultBgColor;
                composer.AddStaticElement(new GuiElementGameOverlay(composer.Api, bounds, backgroundColor));
            }
            return composer;
        }

    }
}
