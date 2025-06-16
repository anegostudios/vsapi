using Cairo;
using Vintagestory.API.Common;

#nullable disable

namespace Vintagestory.API.Client
{
    public class GuiElementImage : GuiElementTextBase
    {
        private readonly AssetLocation imageAsset;

        public GuiElementImage(ICoreClientAPI capi, ElementBounds bounds, AssetLocation imageAsset) : base(capi, "", null, bounds)
        {
            this.imageAsset = imageAsset;
        }

        public override void ComposeElements(Context context, ImageSurface surface)
        {
            context.Save();

            var imageSurface = getImageSurfaceFromAsset(api, imageAsset);

            var pattern = getPattern(api, imageAsset);
            pattern.Filter = Filter.Best;

            context.SetSource(pattern);
            context.Rectangle(Bounds.drawX, Bounds.drawY, Bounds.OuterWidth, Bounds.OuterHeight);
            context.SetSourceSurface(imageSurface, (int)Bounds.drawX, (int)Bounds.drawY);
            context.FillPreserve();

            context.Restore();

            pattern.Dispose();
            imageSurface.Dispose();
        }
    }

    public static class GuiElementHelpers
    {
        public static GuiComposer AddImage(this GuiComposer composer, ElementBounds bounds, AssetLocation imageAsset)
        {
            if (!composer.Composed)
            {
                composer.AddStaticElement(new GuiElementImage(composer.Api, bounds, imageAsset));
            }
            return composer;
        }
    }

}
