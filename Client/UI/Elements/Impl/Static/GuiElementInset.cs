using Cairo;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    class GuiElementInset : GuiElement
    {
        int depth;
        float brightness;

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
        public static GuiComposer AddInset(this GuiComposer composer, ElementBounds bounds, int depth, float brightness = 1f)
        {
            if (!composer.composed)
            {
                composer.AddStaticElement(new GuiElementInset(composer.Api, bounds, depth, brightness));
            }
            return composer;
        }
        
    }
}
