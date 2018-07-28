using Cairo;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public class GuiElementHoverText : GuiElementTextBase
    {
        int hoverTextureId;
        int unscaledWidth;
        int unscaledPadding = 5;

        int width;
        int height;

        bool autoDisplay = true;
        bool visible = false;
        bool followMouse = true;
        bool autoWidth = false;

        public EnumTextOrientation textOrientation = EnumTextOrientation.Left;

        public override double DrawOrder
        {
            get { return 0.9; }
        }

        public GuiElementHoverText(ICoreClientAPI capi, string text, CairoFont font, int width, ElementBounds bounds) : base(capi, text, font, bounds)
        {
            unscaledWidth = width;
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            ComposeHoverElement();
        }

        private void ComposeHoverElement()
        {
            double padding = scaled(unscaledPadding);

            if (autoWidth)
            {
                Bounds.fixedWidth = (int)(Font.GetTextExtents(text).Width + 2 * padding + 1);
                Bounds.fixedHeight = (int)(Font.GetFontExtents().Height + 2 * padding + 1);
            } else
            {
                width = (int)(scaled(unscaledWidth) + 1);
                height = (int)(GetMultilineTextHeight(text, width - 2 * padding) + 2 * padding + 1);
            }

            
            Bounds.CalcWorldBounds();

            if (autoWidth)
            {
                width = (int)Bounds.InnerWidth;
                height = (int)Bounds.InnerHeight;
            }

            ImageSurface surface = new ImageSurface(Format.Argb32, width, height);
            Context ctx = genContext(surface);

            ctx.SetSourceRGBA(0, 0, 0, 0);
            ctx.Paint();


            double[] color = ElementGeometrics.DialogStrongBgColor;

            ctx.SetSourceRGBA(color[0], color[1], color[2], color[3]);

            RoundRectangle(ctx, 0, 0, width, height, ElementGeometrics.DialogBGRadius);
            ctx.FillPreserve();
            ctx.SetSourceRGBA(color[0] / 2, color[1] / 2, color[2] / 2, color[3]);
            ctx.Stroke();

            ShowMultilineText(ctx, text, (int)padding, (int)padding, width - 2 * padding, textOrientation);

            generateTexture(surface, ref hoverTextureId);

            ctx.Dispose();
            surface.Dispose();
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            int mouseX = api.Input.GetMouseCurrentX();
            int mouseY = api.Input.GetMouseCurrentY();

            if ((autoDisplay && Bounds.PointInside(mouseX, mouseY)) || visible)
            {
                double x = Bounds.renderX;
                double y = Bounds.renderY;

                if (followMouse)
                {
                    x = mouseX + 20;
                    y = mouseY + 20;
                }

                if (x + width > api.Render.FrameWidth)
                {
                    x -= (x + width) - api.Render.FrameWidth;
                }

                if (y+height > api.Render.FrameHeight)
                {
                    y -= (y + height) - api.Render.FrameHeight;
                }

                api.Render.Render2DTexturePremultipliedAlpha(hoverTextureId, (int)x, (int)y, width, height);
            }
        }

        public void SetNewText(string text)
        {
            this.text = text;
            ComposeHoverElement();
        }

        public void SetAutoDisplay(bool on)
        {
            autoDisplay = on;
        }

        public void SetVisible(bool on)
        {
            visible = on;
        }

        public void SetAutoWidth(bool on)
        {
            autoWidth = on;
        }

        internal void SetFollowMouse(bool on)
        {
            followMouse = on;
        }
    }



    public static partial class GuiComposerHelpers
    {

        public static GuiComposer AddHoverText(this GuiComposer composer, string text, CairoFont font, int width, ElementBounds bounds, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementHoverText(composer.Api, text, font, width, bounds), key);
            }
            return composer;
        }

        public static GuiElementHoverText GetHoverText(this GuiComposer composer, string key)
        {
            return (GuiElementHoverText)composer.GetElement(key);
        }
    }

}
