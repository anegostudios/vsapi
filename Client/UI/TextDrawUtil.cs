using Cairo;
using System;
using System.Collections.Generic;
using System.Text;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;

#nullable disable

namespace Vintagestory.API.Client
{
    public enum EnumLinebreakBehavior
    {
        /// <summary>
        /// Language specific default setting
        /// </summary>
        Default,
        /// <summary>
        /// After every word
        /// </summary>
        AfterWord,
        /// <summary>
        /// After any character
        /// </summary>
        AfterCharacter,
        /// <summary>
        /// Do not auto-line break, only explicitly after \n
        /// </summary>
        None
    }

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
        /// <summary>
        /// The text of the text line.
        /// </summary>
        public string Text;

        /// <summary>
        /// The bounds of the line of text.
        /// </summary>
        public LineRectangled Bounds;

        public double LeftSpace = 0;
        public double RightSpace = 0;

        public double NextOffsetX;
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
        bool gotSpace = false;

        #region Shorthand methods for simple box constrained multiline text

        public TextLine[] Lineize(Context ctx, string text, double boxwidth, double lineHeightMultiplier = 1f, EnumLinebreakBehavior linebreak = EnumLinebreakBehavior.Default, bool keepLinebreakChar = false)
            => Lineize(ctx, text, linebreak, new TextFlowPath[] { new TextFlowPath(boxwidth) }, 0, 0, lineHeightMultiplier, keepLinebreakChar);

        public int GetQuantityTextLines(CairoFont font, string text, double boxWidth, EnumLinebreakBehavior linebreak = EnumLinebreakBehavior.Default)
            => GetQuantityTextLines(font, text, linebreak, new TextFlowPath[] { new TextFlowPath(boxWidth) });

        public double GetMultilineTextHeight(CairoFont font, string text, double boxWidth, EnumLinebreakBehavior linebreak = EnumLinebreakBehavior.Default)
            => GetQuantityTextLines(font, text, boxWidth, linebreak) * GetLineHeight(font);

        public TextLine[] Lineize(CairoFont font, string fulltext, double boxWidth, EnumLinebreakBehavior linebreak = EnumLinebreakBehavior.Default, bool keepLinebreakChar = false)
            => Lineize(font, fulltext, linebreak, new TextFlowPath[] { new TextFlowPath(boxWidth) }, 0, 0, keepLinebreakChar);




        /// <summary>
        /// Use Matrix transformation to move the draw position
        /// </summary>
        /// <param name="ctx">The context of the text.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="text">The text itself.</param>
        /// <param name="boxWidth">The width of the box containing the text.</param>
        /// <param name="orientation">The orientation of the text.</param>
        public void AutobreakAndDrawMultilineText(Context ctx, CairoFont font, string text, double boxWidth, EnumTextOrientation orientation = EnumTextOrientation.Left)
            => AutobreakAndDrawMultilineText(ctx, font, text, 0, 0, new TextFlowPath[] { new TextFlowPath(boxWidth) }, orientation);

        /// <summary>
        /// Draws the text with matrix transformations.
        /// </summary>
        /// <param name="ctx">The context of the text.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="text">The text itself.</param>
        /// <param name="posX">The X position of the text.</param>
        /// <param name="posY">The Y position of the text.</param>
        /// <param name="boxWidth">The width of the box containing the text.</param>
        /// <param name="orientation">The orientation of the text.</param>
        /// <returns>The new height of the text.</returns>
        public double AutobreakAndDrawMultilineTextAt(Context ctx, CairoFont font, string text, double posX, double posY, double boxWidth, EnumTextOrientation orientation = EnumTextOrientation.Left)
        {
            ctx.Save();
            Matrix m = ctx.Matrix;
            m.Translate((int)posX, (int)posY);
            ctx.Matrix = m;

            double height = AutobreakAndDrawMultilineText(ctx, font, text, 0, 0, new TextFlowPath[] { new TextFlowPath(boxWidth) }, orientation);
            ctx.Restore();
            return height;
        }

        /// <summary>
        /// Draws the text with pre-set breaks.
        /// </summary>
        /// <param name="ctx">The context of the text.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="lines">The lines of text.</param>
        /// <param name="posX">The X position of the text.</param>
        /// <param name="posY">The Y position of the text.</param>
        /// <param name="boxWidth">The width of the box containing the text.</param>
        /// <param name="orientation">The orientation of the text.</param>
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

        /// <summary>
        /// Gets the height of the font to calculate the height of the line.
        /// </summary>
        /// <param name="font">The font to calculate from.</param>
        /// <returns>The height of the line.</returns>
        public double GetLineHeight(CairoFont font)
        {
            return font.GetFontExtents().Height * font.LineHeightMultiplier;
        }

        /// <summary>
        /// Gets the number of lines of text.
        /// </summary>
        /// <param name="font">The font of the text.</param>
        /// <param name="text">The text itself.</param>
        /// <param name="linebreak"></param>
        /// <param name="flowPath">The path for the text.</param>
        /// <param name="lineY">The height of the line</param>
        /// <returns>The number of lines.</returns>
        public int GetQuantityTextLines(CairoFont font, string text, EnumLinebreakBehavior linebreak, TextFlowPath[] flowPath, double lineY = 0)
        {
            if (text == null || text.Length == 0) return 0;

            ImageSurface surface = new ImageSurface(Format.Argb32, 1, 1);
            Context ctx = new Context(surface);
            font.SetupContext(ctx);

            int quantityLines = Lineize(ctx, text, linebreak, flowPath, 0, lineY, font.LineHeightMultiplier).Length;

            ctx.Dispose();
            surface.Dispose();

            return quantityLines;
        }

        /// <summary>
        /// Get the final height of the text.
        /// </summary>
        /// <param name="font">The font of the text.</param>
        /// <param name="text">The text itself.</param>
        /// <param name="linebreak"></param>
        /// <param name="flowPath">The path for the text.</param>
        /// <param name="lineY">The height of the line</param>
        /// <returns>The final height of the text.</returns>
        public double GetMultilineTextHeight(CairoFont font, string text, EnumLinebreakBehavior linebreak, TextFlowPath[] flowPath, double lineY = 0)
        {
            return GetQuantityTextLines(font, text, linebreak, flowPath, lineY) * GetLineHeight(font);
        }


        /// <summary>
        /// Turns the supplied text into line of text constrained by supplied flow path and starting at supplied start coordinates
        /// </summary>
        /// <param name="font">The font of the text.</param>
        /// <param name="fulltext">The text of the lines.</param>
        /// <param name="linebreak"></param>
        /// <param name="flowPath">The flow direction of text.</param>
        /// <param name="startOffsetX">The offset start position for X</param>
        /// <param name="startY">The offset start position for Y</param>
        /// <param name="keepLinebreakChar"></param>
        /// <returns>The text broken up into lines.</returns>
        public TextLine[] Lineize(CairoFont font, string fulltext, EnumLinebreakBehavior linebreak, TextFlowPath[] flowPath, double startOffsetX = 0, double startY = 0, bool keepLinebreakChar = false)
        {
            if (fulltext == null || fulltext.Length == 0) return Array.Empty<TextLine>();

            ImageSurface surface = new ImageSurface(Format.Argb32, 1, 1);
            Context ctx = new Context(surface);
            font.SetupContext(ctx);

            TextLine[] textlines = Lineize(ctx, fulltext, linebreak, flowPath, startOffsetX, startY, font.LineHeightMultiplier, keepLinebreakChar);
            
            ctx.Dispose();
            surface.Dispose();

            return textlines;
        }


        /// <summary>
        /// Turns the supplied text into line of text constrained by supplied flow path and starting at supplied start coordinates
        /// </summary>
        /// <param name="ctx">Contexts of the GUI.</param>
        /// <param name="text">The text to be split</param>
        /// <param name="linebreak"></param>
        /// <param name="flowPath">Sets the general flow of text.</param>
        /// <param name="startOffsetX">The offset start position for X</param>
        /// <param name="startY">The offset start position for Y</param>
        /// <param name="lineHeightMultiplier"></param>
        /// <param name="keepLinebreakChar"></param>
        /// <returns>The text broken up into lines.</returns>
        public TextLine[] Lineize(Context ctx, string text, EnumLinebreakBehavior linebreak, TextFlowPath[] flowPath, double startOffsetX = 0, double startY = 0, double lineHeightMultiplier = 1f, bool keepLinebreakChar = false)
        {
            if (text == null || text.Length == 0) return Array.Empty<TextLine>();

            if (linebreak == EnumLinebreakBehavior.Default)
            {
                linebreak = Lang.AvailableLanguages[Lang.CurrentLocale].LineBreakBehavior;
            }

            string word;
            StringBuilder lineTextBldr = new StringBuilder();

            List<TextLine> lines = new List<TextLine>();
            
            caretPos = 0;
            
            double lineheight = ctx.FontExtents.Height * lineHeightMultiplier;

            double curX = startOffsetX;
            double curY = startY;
            double usableWidth;
            TextFlowPath currentSection;
            
            while ((word = getNextWord(text, linebreak)) != null)
            {
                string spc = (gotLinebreak || caretPos >= text.Length || !gotSpace ? "" : " ");

                double nextWidth = ctx.TextExtents(lineTextBldr + word + spc).Width;

                currentSection = GetCurrentFlowPathSection(flowPath, curY);

                if (currentSection == null)
                {
                    Console.WriteLine("Flow path underflow. Something in the text flow system is incorrectly programmed.");
                    currentSection = new TextFlowPath(500);
                }

                usableWidth = currentSection.X2 - currentSection.X1 - curX;

                if (nextWidth >= usableWidth)
                {
                    // Hard cut-off a word if we are at the beginning of a line
                    // Tyron Mar11: Why only in the beginning of a line? It doesnt line break after a short word then
                    // Tyron Mar18: Turns out perhaps it was meant to be an || because a 2nd word in a line thats oversized and already got its own line can still overflow
                    // Tyron Mar20: || breaks normal word line breaking
                    // Tyron Apr10: This *needs* to check that we are not at the beginning of the line, i.e. startOffsetX==0, otherwise we break apart words that shouldnt be broken apart (In this case Shift -> Shif t in the tutorial)
                    if (word.Length > 0 && (lineTextBldr.Length == 0 && startOffsetX == 0 /* || curX == currentSection.X1*/)) //  && curX == currentSection.X1 - why?
                    {
                        int tries = 500;
                        while (word.Length > 0 && nextWidth >= usableWidth && tries-- > 0)
                        {
                            word = word.Substring(0, word.Length - 1);
                            nextWidth = ctx.TextExtents(lineTextBldr + word + spc).Width;
                            caretPos--;
                        }

                        //if (gotSpace) lineTextBldr.Append(" "); - what is this good for? we already add a space further down after the word. Also why before the word? dafuq?
                        lineTextBldr.Append(word);
                        word = "";
                    }

                    string linetext = lineTextBldr.ToString();
                    double withoutWidth = ctx.TextExtents(linetext).Width;

                    lines.Add(new TextLine()
                    {
                        Text = linetext,
                        Bounds = new LineRectangled(currentSection.X1 + curX, curY, withoutWidth, lineheight) {
                            Ascent = ctx.FontExtents.Ascent
                        },
                        LeftSpace = 0,
                        RightSpace = usableWidth - withoutWidth
                    });

                    lineTextBldr.Clear();
                    curY += lineheight;
                    curX = 0;
                    //gotSpace = false; - why the fuck is this here? It removes trailing spaces from the end of multiline text

                    if (gotLinebreak) currentSection = GetCurrentFlowPathSection(flowPath, curY);
                }

                lineTextBldr.Append(word);
                
                if (gotSpace /*&& word.Length > 0*/ /*- this checks breaks empty spaces in the beginning of new lines - they are completely trimmed */) lineTextBldr.Append(" ");

                if (currentSection == null) currentSection = new TextFlowPath();

                if (gotLinebreak)
                {
                    if (keepLinebreakChar) lineTextBldr.Append("\n");
                    string linetext = lineTextBldr.ToString();
                    double withoutWidth = ctx.TextExtents(linetext).Width;

                    lines.Add(new TextLine()
                    {
                        Text = linetext,
                        Bounds = new LineRectangled(currentSection.X1 + curX, curY, withoutWidth, lineheight) {
                            Ascent = ctx.FontExtents.Ascent
                        },
                        LeftSpace = 0,
                        NextOffsetX = curX,
                        RightSpace = usableWidth - withoutWidth
                    });

                    lineTextBldr.Clear();
                    curY += lineheight;
                    curX = 0;
                }
            }

            currentSection = GetCurrentFlowPathSection(flowPath, curY);
            if (currentSection == null) currentSection = new TextFlowPath();
            usableWidth = currentSection.X2 - currentSection.X1 - curX;
            string lineTextStr = lineTextBldr.ToString();
            double endWidth = ctx.TextExtents(lineTextStr).Width;

            lines.Add(new TextLine()
            {
                Text = lineTextStr,
                Bounds = new LineRectangled(currentSection.X1 + curX, curY, endWidth, lineheight) { Ascent = ctx.FontExtents.Ascent },
                LeftSpace = 0,
                NextOffsetX = curX,
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


        private string getNextWord(string fulltext, EnumLinebreakBehavior linebreakBh)
        {
            if (caretPos >= fulltext.Length) return null;

            StringBuilder word = new StringBuilder();

            char chr;
            gotLinebreak = false;
            gotSpace = false;

            bool autobreak = linebreakBh != EnumLinebreakBehavior.None;

            while (caretPos < fulltext.Length)
            {
                chr = fulltext[caretPos];
                caretPos++;

                if (chr == ' ' && autobreak)
                {
                    gotSpace = true;
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
                    if (caretPos <= fulltext.Length - 1 && fulltext[caretPos] == '\n') caretPos++;
                    break;
                }

                if (chr == '\n')
                {
                    gotLinebreak = true;
                    break;
                }

                word.Append(chr);

                if (linebreakBh == EnumLinebreakBehavior.AfterCharacter)
                {
                    break;
                }
            }

            return word.ToString();
        }

        #endregion


        #region Multiline text drawing

        public double AutobreakAndDrawMultilineText(Context ctx, CairoFont font, string text, double lineX, double lineY, TextFlowPath[] flowPath, EnumTextOrientation orientation = EnumTextOrientation.Left, EnumLinebreakBehavior linebreak = EnumLinebreakBehavior.AfterWord)
        {
            TextLine[] lines = Lineize(font, text, linebreak, flowPath, lineX, lineY);
            DrawMultilineText(ctx, font, lines, orientation);

            if (lines.Length == 0) return 0;
            return lines[lines.Length - 1].Bounds.Y + lines[lines.Length - 1].Bounds.Height;
        }

        /// <summary>
        /// lineX is set to 0 after the second line, lineY is advanced by line height for each line
        /// </summary>
        /// <param name="ctx">The context of the text.</param>
        /// <param name="lines">The preformatted lines of the text.</param>
        /// <param name="font">The font of the text</param>
        /// <param name="orientation">The orientation of text (Default: Left)</param>
        public void DrawMultilineText(Context ctx, CairoFont font, TextLine[] lines, EnumTextOrientation orientation = EnumTextOrientation.Left)
        {
            font.SetupContext(ctx);

            double offsetX = 0;
            
            for (int i = 0; i < lines.Length; i++)
            {
                TextLine textLine = lines[i];
                if (textLine.Text.Length == 0) continue;

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
        
        /// <summary>
        /// Draws a line of text on the screen.
        /// </summary>
        /// <param name="ctx">The context of the text.</param>
        /// <param name="font">The font of the text.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="offsetX">The X offset for the text start position. (Default: 0)</param>
        /// <param name="offsetY">The Y offset for the text start position. (Default: 0)</param>
        /// <param name="textPathMode">Whether or not to use TextPathMode.</param>
        public void DrawTextLine(Context ctx, CairoFont font, string text, double offsetX = 0, double offsetY = 0, bool textPathMode = false)
        {
            if (text == null || text.Length == 0) return;

            ctx.MoveTo((int)(offsetX), (int)(offsetY + ctx.FontExtents.Ascent));

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
                        ctx.ShowText(text);
                    }

                }
            }
        }
        #endregion
    }
}
