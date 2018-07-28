using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    class GuiElementImage : GuiElement
    {
        public GuiElementImage(ICoreClientAPI capi, string text, double relFontSize, ElementBounds bounds) : base(capi, bounds)
        {
        }

    }
}
