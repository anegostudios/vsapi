using System.Text;
using Cairo;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public class GuiElementTextBase : GuiElement
    {
        protected string text;
        internal bool textPathMode = false;
        internal CairoFont Font;
        internal TextSizeProber Prober;

        public GuiElementTextBase(ICoreClientAPI capi, string text, CairoFont font, ElementBounds bounds) : base(capi, bounds)
        {
            this.text = text;
            this.Font = font;
            Prober = new TextSizeProber();
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            Font.SetupContext(ctx);

            Bounds.CalcWorldBounds();

            ComposeTextElements(ctx, surface);
        }

        public virtual void ComposeTextElements(Context ctx, ImageSurface surface) { }


        internal void ShowTextCorrectly(Context ctx, string text, double offsetX = 0, double offsetY = 0)
        {
            if (text == null || text.Length == 0) return;

            PointD point = ctx.CurrentPoint;
            ctx.MoveTo(offsetX + point.X, offsetY + point.Y + ctx.FontExtents.Ascent);

            if (textPathMode || Font.StrokeWidth > 0)
            {
                ctx.TextPath(text);
                if (!textPathMode)
                {
                    ctx.SetSourceRGBA(Font.Color);
                    ctx.FillPreserve();
                    ctx.LineWidth = Font.StrokeWidth;
                    ctx.SetSourceRGBA(Font.StrokeColor);
                    ctx.Stroke();
                }
            } else
            {
                ctx.ShowText(text);

                if (Font.RenderTwice) {
                    ctx.MoveTo(offsetX + point.X, offsetY + point.Y + ctx.FontExtents.Ascent);
                    ctx.ShowText(text);
                }
            }
        }



        //int caretPos = 0;
        //bool gotLinebreak = false;


        internal double GetMultilineTextHeight(string text, double boxWidth, double lineHeightMultiplier = 1f)
        {
            return Prober.GetMultilineTextHeight(Font, text, boxWidth, lineHeightMultiplier);
        }

        internal double GetMultilineTextHeight(string[] lines, double boxWidth, double lineHeightMultiplier = 1f)
        {
            return Prober.GetMultilineTextHeight(Font, lines, boxWidth, lineHeightMultiplier);
        }

        internal double ShowMultilineText(Context ctx, string text, double posX, double posY, double boxWidth, EnumTextOrientation orientation = EnumTextOrientation.Left, double lineHeightMultiplier = 1f)
        {
            if (text == null || text.Length == 0) return 0;

            Font.SetupContext(ctx);

            string[] lines = Prober.InsertAutoLineBreaks(ctx, new StringBuilder(text), boxWidth).Split('\n');

            return ShowMultilineText(ctx, lines, posX, posY, boxWidth, orientation, lineHeightMultiplier);
        }

        internal double ShowMultilineText(Context ctx, string[] lines, double posX, double posY, double boxWidth, EnumTextOrientation orientation = EnumTextOrientation.Left, double lineHeightMultiplier = 1f)
        {
            double offsetX = 0;
            double offsetY = ctx.FontExtents.Height;

            for (int i = 0; i < lines.Length; i++)
            {
                if (orientation == EnumTextOrientation.Center)
                {
                    offsetX = (boxWidth - ctx.TextExtents(lines[i]).Width) / 2;
                }

                if (orientation == EnumTextOrientation.Right)
                {
                    offsetX = boxWidth - ctx.TextExtents(lines[i]).Width;
                }

                ctx.MoveTo(posX + offsetX, posY + i * offsetY * lineHeightMultiplier);
                ShowTextCorrectly(ctx, lines[i]);
            }


            return lines.Length * offsetY * lineHeightMultiplier;
        }

     


        public virtual void SetValue(string text)
        {
            this.text = text;
        }

        public virtual string GetText()
        {
            return text;
        }

        internal virtual void setFont(CairoFont font)
        {
            this.Font = font;
        }

    }
}
