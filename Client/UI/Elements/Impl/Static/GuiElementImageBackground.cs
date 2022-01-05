using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Vintagestory.API.Client
{
    class GuiElementImageBackground : GuiElement
    {
        AssetLocation textureLoc;
        float brightness;

        /// <summary>
        /// Creates a new Image Background for the GUI.
        /// </summary>
        /// <param name="capi">The Client API</param>
        /// <param name="bounds">The bounds of the element.</param>
        /// <param name="textureLoc">The name of the texture.</param>
        /// <param name="brightness">The brightness of the texture. (Default: 1f)</param>
        public GuiElementImageBackground(ICoreClientAPI capi, ElementBounds bounds, AssetLocation textureLoc, float brightness = 1f) : base(capi, bounds)
        {
            this.textureLoc = textureLoc;
            this.brightness = brightness;
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();

            // Stone bg
            SurfacePattern pattern = getPattern(api, textureLoc);
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
        /// <summary>
        /// Adds a background to the current GUI
        /// </summary>
        /// <param name="bounds">The bounds of the background</param>
        /// <param name="textureLoc">The name of the background texture.</param>
        /// <param name="brightness">The brightness of the texture (default: 1f)</param>
        public static GuiComposer AddImageBG(this GuiComposer composer, ElementBounds bounds, AssetLocation textureLoc, float brightness = 1f)
        {
            if (!composer.Composed)
            {
                composer.AddStaticElement(new GuiElementImageBackground(composer.Api, bounds, textureLoc, brightness));
            }
            return composer;
        }
        
    }
}
