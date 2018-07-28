using Cairo;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    class GuiElementSignWithText : GuiElementTextBase
    {

        public GuiElementSignWithText(ICoreClientAPI capi, string text, ElementBounds bounds) : base(capi, text, CairoFont.WhiteMediumText(), bounds)
        {
        }

        public override void ComposeTextElements(Context ctx, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();


            // Shadow
            /*ImageSurface insetShadowSurface = new ImageSurface(Format.Argb32, dialogWidth, dialogHeight);
            Context ctxInsetShadow = new Context(insetShadowSurface);

            ctxInsetShadow.SetSourceRGBA(255, 255, 255, 0);
            ctxInsetShadow.Paint();
            ctxInsetShadow.SetSourceRGBA(0, 0, 0, 0.8);
            RoundRectangle(ctxInsetShadow, absOffsetX, absOffsetY, width, height, ElementGeometrics.dialogBGRadius / 2);
            ctxInsetShadow.Fill();

            insetShadowSurface.Blur(5, (int)Math.Max(0, absOffsetX - 4), (int)Math.Max(0, absOffsetY - 4), (int)Math.Min(dialogWidth, absOffsetX + width + 6), (int)Math.Min(dialogHeight, absOffsetY + height + 6));

            ctx.SetSourceSurface(insetShadowSurface, (int)scaled(3), (int)scaled(3));
            ctx.Paint();
            ctxInsetShadow.Dispose();
            insetShadowSurface.Dispose();*/


            // Wood bg
            RoundRectangle(ctx, Bounds.drawX, Bounds.drawY, Bounds.InnerWidth, Bounds.InnerHeight, ElementGeometrics.ElementBGRadius);
            fillWithPattern(api, ctx, woodTextureName);
            


            EmbossRoundRectangleElement(ctx, Bounds.drawX, Bounds.drawY, Bounds.InnerWidth, Bounds.InnerHeight);

            // Paper bg
            SurfacePattern pattern = getPattern(api, paperTextureName);
            pattern.Extend = Extend.Repeat;

            double facX = 1.0 * pattern.width / (Bounds.InnerWidth - scaled(8));
            double facY = 1.0 * pattern.height / (Bounds.InnerHeight - scaled(8));

            Matrix mat = new Matrix();
            mat.Scale(facX, facY);
            
            pattern.Matrix = mat;
            
            ctx.SetSource(pattern);
            RoundRectangle(ctx, Bounds.drawX + scaled(3.5), Bounds.drawY + scaled(3.5), Bounds.InnerWidth - scaled(8), Bounds.InnerHeight - scaled(8), ElementGeometrics.ElementBGRadius / 2);
            ctx.Fill();

            // Nails
            ctx.Operator = Operator.Over;
            ctx.SetSourceSurface(getMetalNail(), (int)(Bounds.drawX + scaled(3)), (int)(Bounds.drawY + scaled(3)));
            ctx.Paint();
            ctx.SetSourceSurface(getMetalNail(), (int)(Bounds.drawX + Bounds.OuterWidth - scaled(7.5)), (int)(Bounds.drawY + Bounds.InnerHeight - scaled(7.5)));
            ctx.Paint();
            ctx.SetSourceSurface(getMetalNail(), (int)(Bounds.drawX + Bounds.OuterWidth - scaled(7.5)), (int)(Bounds.drawY + scaled(3)));
            ctx.Paint();
            ctx.SetSourceSurface(getMetalNail(), (int)(Bounds.drawX + scaled(3)), (int)(Bounds.drawY + Bounds.InnerHeight - scaled(7.5)));
            ctx.Paint();

            // Text
            ctx.SetSourceRGBA(0, 0, 0, 1);
            ShowMultilineText(ctx, text, Bounds.drawX + scaled(7), Bounds.drawY + scaled(7), Bounds.InnerWidth - scaled(14), EnumTextOrientation.Center, 0.95f);
        }
    }


    public static class GuiElementSignWithTextHelper
    {
        public static GuiComposer AddSignWithText(this GuiComposer composer, string text, ElementBounds bounds)
        {
            if (!composer.composed)
            {
                composer.AddStaticElement(new GuiElementSignWithText(composer.Api, text, bounds));
            }
            return composer;
        }

    }
}
