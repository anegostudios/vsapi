using Cairo;
using Vintagestory.API.Common;

#nullable disable

namespace Vintagestory.API.Client
{
    class GuiElementImageBackground : GuiElement
    {
        AssetLocation textureLoc;
        float brightness;
        float alpha;
        float scale;

        /// <summary>
        /// Creates a new Image Background for the GUI.
        /// </summary>
        /// <param name="capi">The Client API</param>
        /// <param name="bounds">The bounds of the element.</param>
        /// <param name="textureLoc">The name of the texture.</param>
        /// <param name="brightness">The brightness of the texture. (Default: 1f)</param>
        /// <param name="alpha"></param>
        /// <param name="scale"></param>
        public GuiElementImageBackground(ICoreClientAPI capi, ElementBounds bounds, AssetLocation textureLoc, float brightness = 1f, float alpha = 1, float scale = 1f) : base(capi, bounds)
        {
            this.alpha = alpha;
            this.scale = scale;
            this.textureLoc = textureLoc;
            this.brightness = brightness;
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();
            SurfacePattern pattern = getPattern(api, textureLoc, true, (int)(alpha * 255), scale);
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
        /// <param name="composer"></param>
        /// <param name="bounds">The bounds of the background</param>
        /// <param name="textureLoc">The name of the background texture.</param>
        /// <param name="brightness">The brightness of the texture (default: 1f)</param>
        /// <param name="alpha"></param>
        /// <param name="scale"></param>
        public static GuiComposer AddImageBG(this GuiComposer composer, ElementBounds bounds, AssetLocation textureLoc, float brightness = 1f, float alpha = 1, float scale = 1f)
        {
            if (!composer.Composed)
            {
                composer.AddStaticElement(new GuiElementImageBackground(composer.Api, bounds, textureLoc, brightness, alpha, scale));
            }
            return composer;
        }
        
    }
}
