using Cairo;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public class GuiElementCompactScrollbar : GuiElementScrollbar
    {
        public static new int scrollbarPadding = 2;

        public override bool Focusable { get { return true; } }

        public GuiElementCompactScrollbar(ICoreClientAPI capi, API.Common.Action<float> onNewScrollbarValue, ElementBounds bounds) : base(capi, onNewScrollbarValue, bounds) {

        }


        public override void ComposeElements(Context ctxStatic, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();

            ctxStatic.SetSourceRGBA(0, 0, 0, 0.2);
            RoundRectangle(ctxStatic, Bounds.drawX, Bounds.drawY, Bounds.InnerWidth, Bounds.InnerHeight, 1);
            ctxStatic.Fill();

            EmbossRoundRectangleElement(ctxStatic, Bounds, true, 2, 1);

            RecomposeHandle();
        }

        internal override void RecomposeHandle()
        {
            Bounds.CalcWorldBounds();

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)scaled(Bounds.InnerWidth - 1) + 1, (int)currentHandleHeight + 1);
            Context ctx = genContext(surface);

            RoundRectangle(ctx, 0, 0, scaled(Bounds.InnerWidth - 1), currentHandleHeight, 2);
            ctx.SetSourceRGBA(ElementGeometrics.DialogDefaultBgColor[0], ElementGeometrics.DialogDefaultBgColor[1], ElementGeometrics.DialogDefaultBgColor[2], ElementGeometrics.DialogDefaultBgColor[3]);
            ctx.Fill();

            EmbossRoundRectangleElement(ctx, 0, 0, scaled(Bounds.InnerWidth - 1), currentHandleHeight, false, 2, 2);

            generateTexture(surface, ref handleTexture);

            ctx.Dispose();
            surface.Dispose();
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            api.Render.Render2DTexturePremultipliedAlpha(
                handleTexture.TextureId,
                (float)(Bounds.renderX + Bounds.absPaddingX + 1),
                (float)(Bounds.renderY + Bounds.absPaddingY + currentHandlePosition),
                (float)scaled(Bounds.InnerWidth - 1),
                currentHandleHeight + 1,
                70
            );
        }



    }

    public static partial class GuiComposerHelpers
    {

        public static GuiComposer AddCompactVerticalScrollbar(this GuiComposer composer, API.Common.Action<float> onNewScrollbarValue, ElementBounds bounds, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementCompactScrollbar(composer.Api, onNewScrollbarValue, bounds), key);
            }
            return composer;
        }

        public static GuiElementCompactScrollbar GetCompactScrollbar(this GuiComposer composer, string key)
        {
            return (GuiElementCompactScrollbar)composer.GetElement(key);
        }
    }
}
