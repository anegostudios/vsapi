using Cairo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Util;

namespace Vintagestory.API.Client
{
    public class TextFlowPath
    {
        public double X1, Y1, X2, Y2;

        public TextFlowPath() { }

        public TextFlowPath(double boxWidth)
        {
            this.X1 = 0;
            this.Y1 = 0;
            this.X2 = boxWidth;
            this.Y2 = 99999;
        }

        public TextFlowPath(double x1, double y1, double x2, double y2)
        {
            this.X1 = x1;
            this.Y1 = y1;
            this.X2 = x2;
            this.Y2 = y2;
        }
    }

    public class TextLine
    {
        public string Text;
        public LineRectangled Bounds;

        public double PaddingLeft;
        public double PaddingRight;

        //public double WidthWithoutTrailingSpace = 0;

        public double LeftSpace = 0;
        public double RightSpace = 0;
    }

    public class LineRectangled : Rectangled
    {
        public double Ascent = 0;

        public double AscentOrHeight
        {
            get
            {
                return Ascent > 0 ? Ascent : Height;
            }
        }

        public LineRectangled(double X, double Y, double width, double height) : base(X, Y, width, height)
        {
        }

        public LineRectangled() : base()
        {

        }
        
    }



    public class TextDrawUtil
    {
        int caretPos = 0;
        bool gotLinebreak = false;

        #region Shorthand methods for simple box constrained multiline text

        public TextLine[] Lineize(Context ctx, string text, double boxwidth, double lineHeightMultiplier = 1f)
            => Lineize(ctx, text, new TextFlowPath[] { new TextFlowPath(boxwidth) }, 0, 0, lineHeightMultiplier);

        public int GetQuantityTextLines(CairoFont font, string text, double boxWidth)
            => GetQuantityTextLines(font, text, new TextFlowPath[] { new TextFlowPath(boxWidth) });

        public double GetMultilineTextHeight(CairoFont font, string text, double boxWidth)
            => GetQuantityTextLines(font, text, boxWidth) * GetLineHeight(font);

        public TextLine[] Lineize(CairoFont font, string fulltext, double boxWidth)
            => Lineize(font, fulltext, new TextFlowPath[] { new TextFlowPath(boxWidth) }, 0, 0);

       

        /// <summary>
        /// Use Matrix transformation to move the draw position
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="font"></param>
        /// <param name="lines"></param>
        /// <param name="boxWidth"></param>
        /// <param name="orientation"></param>
        /// <param name="lineHeightMultiplier"></param>
        /// <returns></returns>
        public void AutobreakAndDrawMultilineText(Context ctx, CairoFont font, string text, double boxWidth, EnumTextOrientation orientation = EnumTextOrientation.Left)
            => AutobreakAndDrawMultilineText(ctx, font, text, 0, 0, new TextFlowPath[] { new TextFlowPath(boxWidth) }, orientation);


        public double AutobreakAndDrawMultilineTextAt(Context ctx, CairoFont font, string text, double posX, double posY, double boxWidth, EnumTextOrientation orientation = EnumTextOrientation.Left)
        {
            ctx.Save();
            Matrix m = ctx.Matrix;
            m.Translate(posX, posY);
            ctx.Matrix = m;

            double height = AutobreakAndDrawMultilineText(ctx, font, text, 0, 0, new TextFlowPath[] { new TextFlowPath(boxWidth) }, orientation);
            ctx.Restore();
            return height;
        }

        public void DrawMultilineTextAt(Context ctx, CairoFont font, TextLine[] lines, double posX, double posY, double boxWidth, EnumTextOrientation orientation = EnumTextOrientation.Left)
        {
            ctx.Save();
            Matrix m = ctx.Matrix;
            m.Translate(posX, posY);
            ctx.Matrix = m;

            font.SetupContext(ctx);

            DrawMultilineText(ctx, font, lines, orientation);
            ctx.Restore();
        }

        #endregion

        #region Text meta info and multiline splitting

        public double GetLineHeight(CairoFont font)
        {
            return font.GetFontExtents().Height * font.LineHeightMultiplier;
        }

        public int GetQuantityTextLines(CairoFont font, string text, TextFlowPath[] flowPath, double lineY = 0)
        {
            if (text == null || text.Length == 0) return 0;

            ImageSurface surface = new ImageSurface(Format.Argb32, 1, 1);
            Context ctx = new Context(surface);
            font.SetupContext(ctx);

            int quantityLines = Lineize(ctx, text, flowPath, 0, lineY, font.LineHeightMultiplier).Length;

            ctx.Dispose();
            surface.Dispose();

            return quantityLines;
        }


        public double GetMultilineTextHeight(CairoFont font, string text, TextFlowPath[] flowPath, double lineY = 0)
        {
            return GetQuantityTextLines(font, text, flowPath, lineY) * GetLineHeight(font);
        }


        /// <summary>
        /// Turns the supplied text into line of text constrained by supplied flow path and starting at supplied start coordinates
        /// </summary>
        /// <param name="font"></param>
        /// <param name="fulltext"></param>
        /// <param name="flowPath"></param>
        /// <param name="lineX"></param>
        /// <param name="lineY"></param>
        /// <returns></returns>
        public TextLine[] Lineize(CairoFont font, string fulltext, TextFlowPath[] flowPath, double startOffsetX = 0, double startY = 0)
        {
            if (fulltext == null || fulltext.Length == 0) return new TextLine[0];

            ImageSurface surface = new ImageSurface(Format.Argb32, 1, 1);
            Context ctx = new Context(surface);
            font.SetupContext(ctx);

            TextLine[] textlines = Lineize(ctx, fulltext, flowPath, startOffsetX, startY, font.LineHeightMultiplier);
                
            ctx.Dispose();
            surface.Dispose();

            return textlines;
        }



        /// <summary>
        /// Turns the supplied text into line of text constrained by supplied flow path and starting at supplied start coordinates
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="text"></param>
        /// <param name="flowPath"></param>
        /// <param name="startOffsetX"></param>
        /// <param name="startY"></param>
        /// <returns></returns>
        public TextLine[] Lineize(Context ctx, string text, TextFlowPath[] flowPath, double startOffsetX = 0, double startY = 0, double lineHeightMultiplier = 1f)
        {
            if (text == null || text.Length == 0) return new TextLine[0];

            string word;
            StringBuilder lineTextBldr = new StringBuilder();

            List<TextLine> lines = new List<TextLine>();

            caretPos = 0;
            int previousCaretPos = 0;
            
            double lineheight = ctx.FontExtents.Height * lineHeightMultiplier;

            double curX = startOffsetX;
            double curY = startY;
            double usableWidth;
            TextFlowPath currentSection = null;

            while ((word = getNextWord(text)) != null)
            {
                double width = ctx.TextExtents(lineTextBldr + (gotLinebreak || caretPos >= text.Length ? "" : " ") + word).Width;

                currentSection = GetCurrentFlowPathSection(flowPath, curY);

                if (currentSection == null)
                {
                    Console.WriteLine("Flow path underflow. Something in the text flow system is incorrectly programmed.");
                    currentSection = new TextFlowPath(500);
                }


                usableWidth = currentSection.X2 - currentSection.X1 - curX;

                if (width >= usableWidth)
                {
                    double withoutWidth = ctx.TextExtents(lineTextBldr.ToString()).Width;

                    lines.Add(new TextLine()
                    {
                        Text = lineTextBldr.ToString(),
                        Bounds = new LineRectangled(currentSection.X1 + curX, curY, withoutWidth, lineheight) { Ascent = ctx.FontExtents.Ascent },
                        LeftSpace = 0,
                        RightSpace = usableWidth - withoutWidth
                    });

                    lineTextBldr.Clear();
                    curY += lineheight;
                    curX = 0;

                    if (gotLinebreak) currentSection = GetCurrentFlowPathSection(flowPath, curY);
                }

                if (lineTextBldr.Length > 0)
                {
                    lineTextBldr.Append(" ");
                }

                lineTextBldr.Append(word);

                if (gotLinebreak)
                {
                    double withoutWidth = ctx.TextExtents(lineTextBldr.ToString()).Width;

                    lines.Add(new TextLine()
                    {
                        Text = lineTextBldr.ToString(),
                        Bounds = new LineRectangled(currentSection.X1 + curX, curY, withoutWidth, lineheight) { Ascent = ctx.FontExtents.Ascent },
                        LeftSpace = 0,
                        RightSpace = usableWidth - withoutWidth
                    });

                    lineTextBldr.Clear();
                    curY += lineheight;
                    curX = 0;
                }

                previousCaretPos = caretPos;
            }

            currentSection = GetCurrentFlowPathSection(flowPath, curY);
            usableWidth = currentSection.X2 - currentSection.X1 - curX;
            string lineTextStr = lineTextBldr.ToString();
            double endWidth = ctx.TextExtents(lineTextStr).Width;


            lines.Add(new TextLine()
            {
                Text = lineTextStr,
                Bounds = new LineRectangled(currentSection.X1 + curX, curY, endWidth, lineheight) { Ascent = ctx.FontExtents.Ascent },
                LeftSpace = 0,
                RightSpace = usableWidth - endWidth
            });
            
            return lines.ToArray();
        }


        TextFlowPath GetCurrentFlowPathSection(TextFlowPath[] flowPath, double posY)
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


        private string getNextWord(string fulltext)
        {
            if (caretPos >= fulltext.Length) return null;

            StringBuilder word = new StringBuilder();

            char chr;
            gotLinebreak = false;

            while (caretPos < fulltext.Length)
            {
                chr = fulltext[caretPos];
                caretPos++;

                if (chr == ' ')
                {
                    break;
                }

                if (chr == '\t')
                {
                    if (word.Length > 0) { caretPos--; break; }
                    else { return "  "; }
                }

                if (chr == '\r')
                {
                    gotLinebreak = true;
                    if (caretPos < fulltext.Length - 1 && fulltext[caretPos] == '\n') caretPos++;
                    break;
                }

                if (chr == '\n')
                {
                    gotLinebreak = true;
                    break;
                }

                word.Append(chr);
                
            }

            return word.ToString();
        }

        #endregion


        #region Multiline text drawing

        public double AutobreakAndDrawMultilineText(Context ctx, CairoFont font, string text, double lineX, double lineY, TextFlowPath[] flowPath, EnumTextOrientation orientation = EnumTextOrientation.Left)
        {
            TextLine[] lines = Lineize(font, text, flowPath, lineX, lineY);
            DrawMultilineText(ctx, font, lines, orientation);

            if (lines.Length == 0) return 0;
            return lines[lines.Length - 1].Bounds.Y + lines[lines.Length - 1].Bounds.Height;
        }

        /// <summary>
        /// lineX is set to 0 after the second line, lineY is advanced by line height for each line
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="lines"></param>
        /// <param name="lineX"></param>
        /// <param name="lineY"></param>
        /// <param name="flowPath"></param>
        /// <param name="orientation"></param>
        /// <param name="lineHeightMultiplier"></param>
        /// <returns></returns>
        public void DrawMultilineText(Context ctx, CairoFont font, TextLine[] lines, EnumTextOrientation orientation = EnumTextOrientation.Left)
        {
            double offsetX = 0;
            double lineHeight = ctx.FontExtents.Height;

            font.SetupContext(ctx);

            for (int i = 0; i < lines.Length; i++)
            {
                TextLine textLine = lines[i];

                if (orientation == EnumTextOrientation.Center)
                {
                    offsetX = (textLine.LeftSpace + textLine.RightSpace) / 2;
                }

                if (orientation == EnumTextOrientation.Right)
                {
                    offsetX = textLine.LeftSpace + textLine.RightSpace;
                }

                DrawTextLine(ctx, font, textLine.Text, offsetX + textLine.Bounds.X, textLine.Bounds.Y);
            }
        }
        

        public void DrawTextLine(Context ctx, CairoFont font, string text, double offsetX = 0, double offsetY = 0, bool textPathMode = false)
        {
            if (text == null || text.Length == 0) return;

            PointD point = ctx.CurrentPoint;
            //ctx.MoveTo(offsetX + point.X, offsetY + point.Y + ctx.FontExtents.Ascent);
            ctx.MoveTo(offsetX, offsetY + ctx.FontExtents.Ascent);

            if (textPathMode)
            {
                ctx.TextPath(text);
            }
            else
            {
                if (font.StrokeWidth > 0)
                {
                    ctx.TextPath(text);
                    ctx.LineWidth = font.StrokeWidth;
                    ctx.SetSourceRGBA(font.StrokeColor);
                    ctx.StrokePreserve();

                    ctx.SetSourceRGBA(font.Color);
                    ctx.Fill();
                }
                else
                {
                    ctx.ShowText(text);

                    if (font.RenderTwice)
                    {
                        ctx.MoveTo(offsetX + point.X, offsetY + point.Y + ctx.FontExtents.Ascent);
                        ctx.ShowText(text);
                    }

                }
            }
        }
        #endregion
    }
}
