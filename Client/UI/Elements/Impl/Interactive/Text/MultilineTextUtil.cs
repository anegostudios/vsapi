using System;
using System.Text;
using Cairo;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// A helper class to draw multi line text
    /// </summary>
 /*   public class MultilineTextUtil
    {
        public bool TextPathMode = false;
        public CairoFont Font;
        public TextDrawUtil Prober;

        /// <summary>
        /// Amount of text lines drawn. Is set after calling DrawMultilineText()
        /// </summary>
        public int DrawnQuantityTextLines { get; private set; }

        public MultilineTextUtil(CairoFont font)
        {
            this.Font = font;
            Prober = new TextDrawUtil();
        }

        public void SetupContext(Context ctx)
        {
            Font.SetupContext(ctx);
        }

        public int GetQuantityTextLines(string text, double boxWidth, double lineHeightMultiplier = 1f)
        {
            return Prober.GetQuantityTextLines(Font, text, boxWidth, lineHeightMultiplier);
        }

        public int GetQuantityTextLines(string text, TextFlowPath[] lineBounds, double lineHeightMultiplier = 1f)
        {
            return Prober.GetQuantityTextLines(Font, text, lineBounds, lineHeightMultiplier);
        }

        public double GetLineHeight(double lineHeightMultiplier = 1f)
        {
            return Prober.GetLineHeight(Font, 1f);
        }

        public double GetMultilineTextHeight(string text, double boxWidth, double lineHeightMultiplier = 1f)
        {
            return Prober.GetMultilineTextHeight(Font, text, new TextFlowPath[] { new TextFlowPath(boxWidth) }, lineHeightMultiplier);
        }

        public double GetMultilineTextHeight(string text, TextFlowPath[] linebouds, double lineHeightMultiplier = 1f)
        {
            return Prober.GetMultilineTextHeight(Font, text, linebouds, lineHeightMultiplier);
        }


        public double DrawMultilineText(Context ctx, string text, double posX, double posY, double boxWidth, EnumTextOrientation orientation = EnumTextOrientation.Left, double lineHeightMultiplier = 1f)
        {
            return DrawMultilineText(ctx, text, posX, posY, new TextFlowPath[] { new TextFlowPath(boxWidth) }, orientation, lineHeightMultiplier);
        }
        /*
        public double DrawMultilineText(Context ctx, string[] lines, double posX, double posY, double boxWidth, EnumTextOrientation orientation = EnumTextOrientation.Left, double lineHeightMultiplier = 1f, double firstLineX = 0)
        {
            Font.SetupContext(ctx);
            DrawnQuantityTextLines = lines.Length;
            return drawMultilineText(ctx, lines, posX, posY, new TextFlowPath[] { new TextFlowPath(boxWidth) }, orientation, lineHeightMultiplier, firstLineX);
        }


        public double DrawMultilineText(Context ctx, string text, double posX, double posY, TextFlowPath[] lineBounds, EnumTextOrientation orientation = EnumTextOrientation.Left, double lineHeightMultiplier = 1f, double firstLineX = 0)
        {
            if (text == null || text.Length == 0) return 0;

            Font.SetupContext(ctx);

            string[] lines = Prober.InsertAutoLineBreaks(ctx, new StringBuilder(text), lineBounds, firstLineX).Split('\n');
            DrawnQuantityTextLines = lines.Length;

            return drawMultilineText(ctx, lines, posX, posY, lineBounds, orientation, lineHeightMultiplier, firstLineX);
        }
        


    }*/
}
