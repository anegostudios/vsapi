using Cairo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    public class RichTextComponent : RichTextComponentBase
    {
        protected TextDrawUtil textUtil;
        protected EnumLinebreakBehavior linebreak = EnumLinebreakBehavior.AfterWord;

        public string DisplayText;
        public CairoFont Font;
        public TextLine[] Lines;


        public RichTextComponent(ICoreClientAPI api, string displayText, CairoFont font) : base(api)
        {
            this.DisplayText = displayText;
            this.Font = font;
            this.linebreak = Lang.AvailableLanguages[Lang.CurrentLocale].LineBreakBehavior;

            init();
        }

        protected void init()
        {
            if (DisplayText.Length > 0)
            {
                // ok apparently text extents of " " is 0 on a mac? o.O
                if (DisplayText[DisplayText.Length - 1] == ' ')
                {
                    PaddingRight = (Font.GetTextExtents("a b").Width - Font.GetTextExtents("ab").Width) / RuntimeEnv.GUIScale;
                }
                if (DisplayText[0] == ' ')
                {
                    PaddingLeft = (Font.GetTextExtents("a b").Width - Font.GetTextExtents("ab").Width) / RuntimeEnv.GUIScale;
                }

                this.DisplayText = DisplayText.Trim(new char[] { ' ' });
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
            textUtil.DrawMultilineText(ctx, Font, Lines, Font.Orientation);

            /*ctx.LineWidth = 1f;
            ctx.SetSourceRGBA(0, 0, 0, 0.5);
            for (int i = 0; i < Lines.Length; i++)
            {
                TextLine line = Lines[i];
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
        public override void RenderInteractiveElements(float deltaTime, double renderX, double renderY, double renderZ)
        {
            /*for (int i = 0; i < Lines.Length; i++)
            {
                var bounds = Lines[i].Bounds;
                api.Render.RenderRectangle((float)renderX + (float)bounds.X, (float)renderY + (float)bounds.Y, 50, (float)bounds.Width, (float)bounds.Height, ColorUtil.ColorFromRgba(200, 255, 255, 128));
            }*/
        }


        /// <summary>
        /// Initializes the size and stuff. Return true if you had to enter the next line
        /// </summary>
        /// <param name="flowPath"></param>
        /// <param name="xPos"></param>
        /// <returns>True when longer than 1 line</returns>
        public override EnumCalcBoundsResult CalcBounds(TextFlowPath[] flowPath, double currentLineHeight, double offsetX, double lineY, out double nextOffsetX)
        {
            offsetX += GuiElement.scaled(PaddingLeft);

            Lines = textUtil.Lineize(Font, DisplayText, linebreak, flowPath, offsetX, lineY);

            nextOffsetX = offsetX;

            BoundsPerLine = new LineRectangled[Lines.Length];
            for (int i = 0; i < Lines.Length; i++)
            {
                TextLine line = Lines[i];
                BoundsPerLine[i] = line.Bounds;
            }

            if (Lines.Length > 0)
            {
                var lbnd = BoundsPerLine[Lines.Length-1];
                lbnd.Width += GuiElement.scaled(PaddingRight);
                
                nextOffsetX = Lines[Lines.Length - 1].NextOffsetX + lbnd.Width;
            }

            return Lines.Length > 1 ? EnumCalcBoundsResult.Multiline : EnumCalcBoundsResult.Continue;
        }
        

        protected double GetFontOrientOffsetX()
        {
            if (Lines.Length == 0) return 0;

            var textLine = Lines[Lines.Length - 1];
            double offsetX = 0; if (Font.Orientation == EnumTextOrientation.Center)
            {
                offsetX = (textLine.LeftSpace + textLine.RightSpace) / 2;
            }

            if (Font.Orientation == EnumTextOrientation.Right)
            {
                offsetX = textLine.LeftSpace + textLine.RightSpace;
            }

            return offsetX;
        }
    }
}
