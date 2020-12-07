using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cairo;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Draws an icon 
    /// </summary>
    public class IconComponent : RichTextComponentBase
    {
        protected ICoreClientAPI capi;

        protected ElementBounds parentBounds;

        public double offY = 0;

        protected string iconName;
        protected CairoFont font;

        public IconComponent(ICoreClientAPI capi, string iconName, CairoFont font) : base(capi)
        {
            this.capi = capi;
            this.iconName = iconName;
            this.font = font;
        }


        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            capi.Gui.Icons.DrawIcon(ctx, iconName, BoundsPerLine[0].X, BoundsPerLine[0].Y, GuiElement.scaled(font.UnscaledFontsize), GuiElement.scaled(font.UnscaledFontsize), font.Color);
        }


        public override bool CalcBounds(TextFlowPath[] flowPath, double currentLineHeight, double lineX, double lineY)
        {
            BoundsPerLine = new LineRectangled[]
            {
                new LineRectangled(lineX, lineY, GuiElement.scaled(font.UnscaledFontsize), GuiElement.scaled(font.UnscaledFontsize))
            };

            return false;
        }


        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
