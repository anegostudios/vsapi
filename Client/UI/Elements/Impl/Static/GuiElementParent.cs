using Cairo;

#nullable disable

namespace Vintagestory.API.Client
{
    public class GuiElementParent : GuiElement
    {
        public GuiElementParent(ICoreClientAPI capi, ElementBounds bounds) : base(capi, bounds)
        {
        }

        public override void ComposeElements(Context ctxStatic, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();
        }
    }
}
