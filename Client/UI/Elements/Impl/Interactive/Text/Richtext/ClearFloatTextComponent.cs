using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cairo;

namespace Vintagestory.API.Client
{
    public class ClearFloatTextComponent : RichTextComponent
    {
        
        public ClearFloatTextComponent(float marginTop = 0) : base("", CairoFont.WhiteDetailText())
        {
            this.Float = EnumFloat.None;
            MarginTop = marginTop;
        }

        public override bool CalcBounds(TextFlowPath[] flowPath, double currentLineHeight, double lineX, double lineY)
        {
            double y2 = lineY;

            for (int i = 0; i < flowPath.Length; i++)
            {
                TextFlowPath curFp = flowPath[i];
                if (curFp.Y1 <= lineY && curFp.Y2 >= lineY)
                {
                    if (curFp.X1 > 0)
                    {
                        y2 = curFp.Y2;
                    } else
                    {
                        break;
                    }
                }
            }

            this.BoundsPerLine = new LineRectangled[] { new LineRectangled(0, lineY, 10, y2 - lineY + 1) };

            return this.Float == EnumFloat.None;
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            /*ctx.LineWidth = 1f;
            ctx.SetSourceRGBA(0, 0, 1, 0.5);
            for (int i = 0; i < BoundsPerLine.Length; i++)
            {
                LineRectangled rect = BoundsPerLine[i];
                ctx.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
                ctx.Stroke();
            }*/
        }

        public override void RenderInteractiveElements(ICoreClientAPI api, float deltaTime, double renderX, double renderY)
        {
            
        }
    }
}
