using Cairo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    public class RichTextComponent : RichTextComponentBase
    {
        protected TextDrawUtil textUtil;
        
        protected string displayText;
        protected CairoFont font;

        protected TextLine[] lines;


        public RichTextComponent(ICoreClientAPI api, string displayText, CairoFont font) : base(api)
        {
            this.displayText = displayText;
            this.font = font;

            if (displayText.Length > 0)
            { 
                // ok apparently text extents of " " is 0 on a mac? o.O
                if (displayText[displayText.Length - 1] == ' ') PaddingRight = 0.75 * (font.GetTextExtents("a b").Width - font.GetTextExtents("ab").Width); // added 0.75 multiplier because there is always too much spacing o.o
                if (displayText[0] == ' ') PaddingLeft = 0.75 * (font.GetTextExtents("a b").Width - font.GetTextExtents("ab").Width);
                displayText = displayText.Trim(new char[] { ' ' }); 
            }
            else
            {
                PaddingLeft = 0;
                PaddingRight = 0;
            }

            textUtil = new TextDrawUtil();
        }
        
        /// <summary>
        /// Composes the element.
        /// </summary>
        /// <param name="ctx">Context of the text component.</param>
        /// <param name="surface">The surface of the image.</param>
        /// <param name="withFont">The font for the element.</param>
        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            textUtil.DrawMultilineText(ctx, font, lines, EnumTextOrientation.Left);

           /* ctx.LineWidth = 1f;
            ctx.SetSourceRGBA(0, 0, 0, 0.5);
            for (int i = 0; i < lines.Length; i++)
            {
                TextLine line = lines[i];
                ctx.Rectangle(line.Bounds.X, line.Bounds.Y, line.Bounds.Width, line.Bounds.Height);
                ctx.Stroke();
            }*/
        }



        /// <summary>
        /// Renders the text component.
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="renderX"></param>
        /// <param name="renderY"></param>
        public override void RenderInteractiveElements(float deltaTime, double renderX, double renderY)
        {
            
        }


        /// <summary>
        /// Initializes the size and stuff. Return true if you had to enter the next line
        /// </summary>
        /// <param name="flowPath"></param>
        /// <param name="xPos"></param>
        /// <returns>True when longer than 1 line</returns>
        public override bool CalcBounds(TextFlowPath[] flowPath, double currentLineHeight, double lineX, double lineY)
        {
            double lineheight = textUtil.GetLineHeight(font);
            lines = textUtil.Lineize(font, displayText, flowPath, lineX + PaddingLeft, lineY);

            BoundsPerLine = new LineRectangled[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                TextLine line = lines[i];
                BoundsPerLine[i] = line.Bounds;
            }

            if (lines.Length > 0) {
                lines[0].PaddingLeft = PaddingLeft;
                lines[lines.Length - 1].PaddingRight = PaddingRight;
                lines[lines.Length - 1].Bounds.Width += PaddingRight;
            }

            return lines.Length > 1;
        }
        
    }
}
