using Cairo;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public class GuiElementGrayBackground : GuiElement
    {
        public GuiElementGrayBackground(ICoreClientAPI capi, ElementBounds bounds) : base(capi, bounds)
        {

        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();

            ctx.SetSourceRGBA(0, 0, 0, 0.35);
            ctx.Fill();
            ctx.Paint();
        }
    }


    public static class GuiElementGrayBackgroundHelpber
    {

        public static GuiComposer AddGrayBG(this GuiComposer composer, ElementBounds bounds)
        {
            if (!composer.composed)
            {
                composer.AddStaticElement(new GuiElementGrayBackground(composer.Api, bounds));
            }
            return composer;
        }
        
    }
}
