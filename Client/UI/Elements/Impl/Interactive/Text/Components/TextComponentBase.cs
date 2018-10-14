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
        
        /// <summary>
        /// Requests a line break, if applicable.
        /// </summary>
        /// <param name="width">The new width.</param>
        /// <returns>a new TextComponentBase.  Null by default.</returns>
        public virtual TextComponentBase[] RequestLineBreakAt(float width)
        {
            return null;
        }

        /// <summary>
        /// Updates the bounds of the Text Component.
        /// </summary>
        /// <param name="withFont">The font to use.</param>
        /// <param name="startX">The X position of the text.</param>
        /// <param name="startY">The Y position of the text</param>
        public virtual void UpdateBounds(CairoFont withFont, double startX, double startY)
        {
            Bounds.WithFixedPosition(startX, startY).CalcWorldBounds();
        }

        /// <summary>
        /// Composes the element.
        /// </summary>
        /// <param name="ctx">Context of the text component.</param>
        /// <param name="surface">The surface of the image.</param>
        /// <param name="withFont">The font for the element.</param>
        public virtual void ComposeElements(Context ctx, ImageSurface surface, CairoFont withFont)
        {

        }

        /// <summary>
        /// Renders the text component.
        /// </summary>
        /// <param name="api">The client API.</param>
        /// <param name="deltaTime">The change in time.</param>
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
