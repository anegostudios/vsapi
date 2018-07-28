using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cairo;

namespace Vintagestory.API.Client
{
    public class TextComponent : TextComponentBase
    {
        internal string text;

        public TextComponent(string text)
        {
            this.text = text;
        }

        public override void UpdateBounds(CairoFont withFont, double startX, double startY)
        {
            withFont.AutoBoxSize(text, Bounds);
            Bounds.WithFixedPosition(startX, startY).CalcWorldBounds();
        }

        public override void ComposeElements(Context ctx, ImageSurface surface, CairoFont withFont)
        {
            withFont.SetupContext(ctx);
            DrawText(ctx, text, Bounds.drawX, Bounds.drawY);
        }
    }
}
