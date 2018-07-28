using Cairo;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    class GuiElementImageBackground : GuiElement
    {
        string textureName;
        float brightness;

        public GuiElementImageBackground(ICoreClientAPI capi, ElementBounds bounds, string textureName, float brightness = 1f) : base(capi, bounds)
        {
            this.textureName = textureName;
            this.brightness = brightness;
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();

            // Stone bg
            SurfacePattern pattern = getPattern(api, textureName);
            ctx.SetSource(pattern);
            ElementRoundRectangle(ctx, Bounds);
            

            ctx.Fill();

            if (brightness < 1)
            {
                ElementRoundRectangle(ctx, Bounds);
                ctx.SetSourceRGBA(0, 0, 0, 1 - brightness);
                ctx.Fill();
            }
        }

    }



    public static class GuiElementImageBackgroundHelper
    {
        public static GuiComposer AddBG(this GuiComposer composer, ElementBounds bounds, string textureName, float brightness = 1f)
        {
            if (!composer.composed)
            {
                composer.AddStaticElement(new GuiElementImageBackground(composer.Api, bounds, textureName, brightness));
            }
            return composer;
        }
        
    }
}
