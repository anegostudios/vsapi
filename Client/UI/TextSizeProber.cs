using Cairo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Client
{
    public class TextSizeProber
    {
        int caretPos = 0;
        bool gotLinebreak = false;

        public double GetLineHeight(CairoFont font, double lineHeightMultiplier = 1f)
        {
            return font.GetFontExtents().Height * lineHeightMultiplier;
        }

        public double GetMultilineTextHeight(CairoFont font, string text, double boxWidth, double lineHeightMultiplier = 1f)
        {
            ImageSurface surface = new ImageSurface(Format.Argb32, 1, 1);
            Context ctx = new Context(surface);
            font.SetupContext(ctx);

            string[] lines = InsertAutoLineBreaks(ctx, new StringBuilder(text), boxWidth).Split('\n');
            double lineheight = ctx.FontExtents.Height;

            ctx.Dispose();
            surface.Dispose();

            return lines.Length * lineheight * lineHeightMultiplier;
        }


        public double GetMultilineTextHeight(CairoFont font, string[] lines, double boxWidth, double lineHeightMultiplier = 1f)
        {
            return lines.Length * font.GetFontExtents().Height * lineHeightMultiplier;
        }


        public string[] InsertAutoLineBreaks(CairoFont font, StringBuilder fulltext, double boxWidth)
        {
            ImageSurface surface = new ImageSurface(Format.Argb32, 1, 1);
            Context ctx = new Context(surface);
            font.SetupContext(ctx);

            string[] lines = InsertAutoLineBreaks(ctx, fulltext, boxWidth).Split('\n');
            double offsetY = ctx.FontExtents.Height;

            ctx.Dispose();
            surface.Dispose();

            return lines;
        }

        public string InsertAutoLineBreaks(Context ctx, StringBuilder fulltext, double boxWidth)
        {
            string word;
            StringBuilder line = new StringBuilder();
            StringBuilder output = new StringBuilder();

            caretPos = 0;
            int previousCaretPos = 0;

            while ((word = getNextWord(fulltext)) != null)
            {
                double width = ctx.TextExtents(line + (gotLinebreak ? "" : " ") + word).Width;

                if (width >= boxWidth && previousCaretPos > 0)
                {
                    output.Append(line);
                    output.Append("\n");

                    line = new StringBuilder();
                }

                if (line.Length > 0)
                {
                    line.Append(" ");
                }
                line.Append(word);


                if (gotLinebreak)
                {
                    output.Append(line);
                    output.Append("\n");
                    line = new StringBuilder();
                }

                previousCaretPos = caretPos;
            }

            output.Append(line);

            return output.ToString();
        }


        private string getNextWord(StringBuilder fulltext)
        {
            if (caretPos >= fulltext.Length) return null;

            StringBuilder word = new StringBuilder();

            char chr;
            gotLinebreak = false;

            while (caretPos < fulltext.Length)
            {
                chr = fulltext[caretPos];
                caretPos++;

                if (chr == ' ') break;

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
    }
}
