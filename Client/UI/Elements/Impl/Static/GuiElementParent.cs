using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cairo;
using Vintagestory.API.Client;

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
