using Cairo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public abstract class TextComponentBase
    {
        public ElementBounds Bounds;
        

        public virtual TextComponentBase[] RequestLineBreakAt(float width)
        {
            return null;
        }

        public virtual void UpdateBounds(CairoFont withFont, double startX, double startY)
        {
            Bounds.WithFixedPosition(startX, startY).CalcWorldBounds();
        }

        public virtual void ComposeElements(Context ctx, ImageSurface surface, CairoFont withFont)
        {

        }

        public virtual void RenderInteractiveElements(ICoreClientAPI api, float deltaTime)
        {
        }

        internal void DrawText(Context ctx, string text, double offsetX = 0, double offsetY = 0)
        {
            if (text == null || text.Length == 0) return;

            PointD point = ctx.CurrentPoint;
            ctx.MoveTo(offsetX + point.X, offsetY + point.Y + ctx.FontExtents.Ascent);
            ctx.ShowText(text);            
        }
    }
}
