using System;
using Cairo;

#nullable disable

namespace Vintagestory.API.Client
{
    public class GuiElementSwitchOld : GuiElementTextBase
    {
        Action<bool> handler;

        internal const double unscaledWidth = 60;
        internal const double unscaledHandleWidth = 30;
        internal const double unscaledHeight = 30;
        internal const double unscaledPadding = 3;

        int offHandleTextureId;
        int onHandleTextureId;

        public bool On;

        public GuiElementSwitchOld(ICoreClientAPI capi, Action<bool> OnToggled, ElementBounds bounds) : base(capi, "", null, bounds)
        {
            Font = CairoFont.WhiteSmallText().WithFontSize((float)GuiStyle.SubNormalFontSize);

            handler = OnToggled;

            bounds.fixedWidth = unscaledWidth;
            bounds.fixedHeight = unscaledHeight;
        }

        public override void ComposeElements(Context ctxStatic, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();

            ctxStatic.SetSourceRGBA(0, 0, 0, 0.2);
            RoundRectangle(ctxStatic, Bounds.drawX, Bounds.drawY, Bounds.InnerWidth, Bounds.InnerHeight, GuiStyle.ElementBGRadius);
            ctxStatic.Fill();
            EmbossRoundRectangleElement(ctxStatic, Bounds, true, 2);

            createHandle("0", ref offHandleTextureId);
            createHandle("1", ref onHandleTextureId);
        }

        private void createHandle(string text, ref int textureId)
        {
            double handleWidth = scaled(unscaledHandleWidth);
            double handleHeight = scaled(unscaledHeight) - 2 * scaled(unscaledPadding);

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)Math.Ceiling(handleWidth), (int)Math.Ceiling(handleHeight));
            Context ctx = genContext(surface);

            RoundRectangle(ctx, 0, 0, handleWidth, handleHeight, 1);
            fillWithPattern(api, ctx, stoneTextureName);

            EmbossRoundRectangleElement(ctx, 0, 0, handleWidth, handleHeight, false, 2, 1);

            Font.SetupContext(ctx);
            //LineTextUtil.DrawTextLine(ctx, text, 0, -scaled(3), handleWidth, EnumTextOrientation.Center);

            generateTexture(surface, ref textureId);

            ctx.Dispose();
            surface.Dispose();
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            double handleWidth = scaled(unscaledHandleWidth);
            double handleHeight = scaled(unscaledHeight) - 2 * scaled(unscaledPadding);
            double padding = scaled(unscaledPadding);

            api.Render.RenderTexture(On ? onHandleTextureId : offHandleTextureId, Bounds.renderX + (On ? scaled(unscaledWidth) - handleWidth - 2*padding  : 0) + padding, Bounds.renderY + padding, handleWidth, handleHeight);
        }

        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            base.OnMouseDownOnElement(api, args);

            On = !On;
            handler(On);
            api.Gui.PlaySound("toggleswitch");
        }
    }



    

}
