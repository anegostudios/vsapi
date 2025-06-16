using Cairo;

#nullable disable

namespace Vintagestory.API.Client
{
    public class ClearFloatTextComponent : RichTextComponent
    {
        
        public ClearFloatTextComponent(ICoreClientAPI api, float unScaleMarginTop = 0) : base(api, "", CairoFont.WhiteDetailText())
        {
            this.Float = EnumFloat.None;
            UnscaledMarginTop = unScaleMarginTop;
        }

        public override EnumCalcBoundsResult CalcBounds(TextFlowPath[] flowPath, double currentLineHeight, double offsetX, double lineY, out double nextOffsetX)
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

            nextOffsetX = 0;

            return this.Float == EnumFloat.None ? EnumCalcBoundsResult.Nextline : EnumCalcBoundsResult.Continue;
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

        public override void RenderInteractiveElements(float deltaTime, double renderX, double renderY, double renderZ)
        {
            
        }
    }
}
