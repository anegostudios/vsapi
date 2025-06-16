using Cairo;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    public enum EnumFloat
    {
        None,
        Inline,
        Left,
        Right
    }

    public enum EnumVerticalAlign
    {
        Top,
        Middle,
        Bottom,
        FixedOffset
    }

    public enum EnumCalcBoundsResult
    {
        /// <summary>
        /// Can continue on the same line
        /// </summary>
        Continue,
        /// <summary>
        /// Element was split between current and next line
        /// </summary>
        Multiline,
        /// <summary>
        /// Element was put on next line
        /// </summary>
        Nextline,
    }

    public abstract class RichTextComponentBase
    {
        public string MouseOverCursor { get; protected set; } = null;

        protected ICoreClientAPI api;

        public RichTextComponentBase(ICoreClientAPI api)
        {
            this.api = api;
        }

        /// <summary>
        /// The width/height boundaries of this text component per line
        /// </summary>
        public virtual LineRectangled[] BoundsPerLine { get; protected set; }

        /// <summary>
        /// This will be the Y-Advance into the next line. Unscaled value. Also used as the offset with EnumVerticalAlign.FixedOffset
        /// </summary>
        public virtual double UnscaledMarginTop { get; set; } = 0;

        /// <summary>
        /// Padding that is used when a richtextcomponent came before and needs some left spacing to it. Unscaled value
        /// </summary>
        public virtual double PaddingRight { get; set; } = 0;

        /// <summary>
        /// Unscaled value
        /// </summary>
        public virtual double PaddingLeft { get; set; } = 0;


        /// <summary>
        /// When left or right, then this element can span over multiple text lines
        /// </summary>
        public virtual EnumFloat Float { get; set; } = EnumFloat.Inline;

        public virtual Vec4f RenderColor { get; set; }

        public virtual EnumVerticalAlign VerticalAlign { get; set; } = EnumVerticalAlign.Bottom;

        /// <summary>
        /// Composes the element.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="surface"></param>
        public virtual void ComposeElements(Context ctx, ImageSurface surface)
        {

        }


        /// <summary>
        /// Renders the text component.
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="renderX"></param>
        /// <param name="renderY"></param>
        /// <param name="renderZ"></param>
        public virtual void RenderInteractiveElements(float deltaTime, double renderX, double renderY, double renderZ)
        {

        }


        /// <summary>
        /// Initializes the size and stuff. Return true if you had to enter the next line
        /// </summary>
        /// <param name="flowPath"></param>
        /// <param name="currentLineHeight"></param>
        /// <param name="offsetX"></param>
        /// <param name="lineY"></param>
        /// <param name="nextOffsetX"></param>
        /// <returns>A</returns>
        public virtual EnumCalcBoundsResult CalcBounds(TextFlowPath[] flowPath, double currentLineHeight, double offsetX, double lineY, out double nextOffsetX)
        {
            nextOffsetX = offsetX;
            return EnumCalcBoundsResult.Continue;
        }


        protected virtual TextFlowPath GetCurrentFlowPathSection(TextFlowPath[] flowPath, double posY)
        {
            for (int i = 0; i < flowPath.Length; i++)
            {
                if (flowPath[i].Y1 <= posY && flowPath[i].Y2 >= posY)
                {
                    return flowPath[i];
                }
            }
            return null;
        }

        public virtual void OnMouseMove(MouseEvent args)
        {

        }

        public virtual void OnMouseDown(MouseEvent args)
        {

        }

        public virtual void OnMouseUp(MouseEvent args)
        {

        }

        public virtual void Dispose()
        {

        }

        public virtual bool UseMouseOverCursor(ElementBounds richtextBounds)
        {
            int relx = (int)(api.Input.MouseX - richtextBounds.absX);
            int rely = (int)(api.Input.MouseY - richtextBounds.absY);

            for (int j = 0; j < BoundsPerLine.Length; j++)
            {
                LineRectangled rec = BoundsPerLine[j];

                if (rec.PointInside(relx, rely))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
