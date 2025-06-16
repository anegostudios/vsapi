using Cairo;

#nullable disable

namespace Vintagestory.API.Client
{
    class GuiElementInsetShadedText : GuiElementTextBase
    {
        public GuiElementInsetShadedText(ICoreClientAPI capi, string text, CairoFont font, ElementBounds bounds) : base(capi, text, font, bounds)
        {
            
        }

        public override void ComposeTextElements(Context ctx, ImageSurface surface)
        {
            // Looks almost just the same without this stuff
            /*ctx.MoveTo(absOffsetX + scaled(2), absOffsetY + scaled(2));
            ctx.SetSourceRGBA(0.31, 0.31, 0.31, 0.5);
            ShowTextCorrectly(ctx, text);

            SurfaceTransformBlur blur = new SurfaceTransformBlur(6);
            blur.Perform((ImageSurface)ctx.GetTarget());
            
            ctx.Operator = Operator.Atop;
            ctx.SetSourceRGBA(tintRGB[0], tintRGB[1], tintRGB[2], tintRGB[3]);
            ctx.MoveTo(absOffsetX, absOffsetY);
            ShowTextCorrectly(ctx, text);*/

            Bounds.CalcWorldBounds();

            ctx.Operator = Operator.Over;
            Font.SetupContext(ctx);
            ctx.MoveTo((int)Bounds.drawX, (int)Bounds.drawY);
            DrawTextLineAt(ctx, text, 0, 0);
        }

    }
}
