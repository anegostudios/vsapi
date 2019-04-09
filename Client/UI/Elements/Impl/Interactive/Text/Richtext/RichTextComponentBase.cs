using Cairo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        Bottom
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
        /// When !IsInline this will be the Y-Advance into the next line
        /// </summary>
        public virtual double MarginTop { get; protected set; } = 10;

        /// <summary>
        /// Padding that is used when a richtextcomponent came before and needs some left spacing to it
        /// </summary>
        public virtual double PaddingRight { get; protected set; } = 0;

        public virtual double PaddingLeft { get; protected set; } = 0;


        /// <summary>
        /// When left or right, then this element can span over multiple text lines
        /// </summary>
        public virtual EnumFloat Float { get; protected set; } = EnumFloat.Inline;


        public virtual EnumVerticalAlign VerticalAlign { get; protected set; } = EnumVerticalAlign.Bottom;

        /// <summary>
        /// Composes the element.
        /// </summary>
        /// <param name="ctx">Context of the text component.</param>
        /// <param name="surface">The surface of the image.</param>
        /// <param name="withFont">The font for the element.</param>
        public virtual void ComposeElements(Context ctx, ImageSurface surface)
        {

        }


        /// <summary>
        /// Renders the text component.
        /// </summary>
        /// <param name="deltaTime">The change in time.</param>
        public virtual void RenderInteractiveElements(float deltaTime, double renderX, double renderY)
        {

        }


        /// <summary>
        /// Initializes the size and stuff. Return true if you had to enter the next line
        /// </summary>
        /// <param name="flowPath"></param>
        /// <param name="xPos"></param>
        /// <returns>Amount of lines passed over, if any</returns>
        public virtual bool CalcBounds(TextFlowPath[] flowPath, double currentLineHeight, double lineX, double lineY)
        {
            return false;
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
    }
}
