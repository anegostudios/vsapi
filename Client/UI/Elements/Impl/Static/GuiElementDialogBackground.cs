using Cairo;

#nullable disable

namespace Vintagestory.API.Client
{
    public class GuiElementDialogBackground : GuiElement
    {
        public bool Shade = true;
        bool withTitlebar;
        double strokeWidth = 0;

        public float Alpha = 1;

        public bool FullBlur = false;

        /// <summary>
        /// Adds a Background to the Dialog.
        /// </summary>
        /// <param name="capi">The Client API</param>
        /// <param name="bounds">The bounds of the element.</param>
        /// <param name="withTitlebar">Minor style adjustments to accomodate title bar</param>
        /// <param name="strokeWidth">The top padding area of the GUI</param>
        /// <param name="alpha"></param>
        public GuiElementDialogBackground(ICoreClientAPI capi, ElementBounds bounds, bool withTitlebar, double strokeWidth = 0, float alpha = 1) : base(capi, bounds)
        {
            this.strokeWidth = strokeWidth;
            this.withTitlebar = withTitlebar;
            this.Alpha = alpha;
        }


        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();
            double titleBarOffY = withTitlebar ? scaled(GuiStyle.TitleBarHeight) : 0;
            
            RoundRectangle(ctx, Bounds.bgDrawX, Bounds.bgDrawY + titleBarOffY, Bounds.OuterWidth, Bounds.OuterHeight - titleBarOffY - 1, GuiStyle.DialogBGRadius);

            ctx.SetSourceRGBA(GuiStyle.DialogStrongBgColor[0] * 1, GuiStyle.DialogStrongBgColor[1] * 1, GuiStyle.DialogStrongBgColor[2] * 1, GuiStyle.DialogStrongBgColor[3] * 1);
            ctx.FillPreserve();
            
            if (Shade)
            {
                ctx.SetSourceRGBA(GuiStyle.DialogLightBgColor[0] * 2.1, GuiStyle.DialogStrongBgColor[1] * 2.1, GuiStyle.DialogStrongBgColor[2] * 2.1, 1);

                ctx.LineWidth = strokeWidth * 2;
                ctx.StrokePreserve();

                var r = scaled(9);
                if (FullBlur)
                {
                    surface.BlurFull(r);
                } else
                {
                    surface.BlurPartial(r, (int)(2 * r + 1), (int)Bounds.bgDrawX, (int)(Bounds.bgDrawY + titleBarOffY), (int)Bounds.OuterWidth, (int)Bounds.OuterHeight);
                }
            }

            SurfacePattern pattern = getPattern(api, dirtTextureName, true, 64, 0.125f);
            ctx.SetSource(pattern);
            ctx.FillPreserve();
            ctx.Operator = Operator.Over;

            if (Shade)
            {
                ctx.SetSourceRGBA(new double[] { 45 / 255.0, 35 / 255.0, 33 / 255.0, Alpha * Alpha });
                ctx.LineWidth = strokeWidth;
                ctx.Stroke();
            } else
            {
                ctx.SetSourceRGBA(new double[] { 45 / 255.0, 35 / 255.0, 33 / 255.0, Alpha });
                ctx.LineWidth = scaled(2);
                ctx.Stroke();
            }
        }
    }


    public static partial class GuiComposerHelpers
    {
        /// <summary>
        /// Adds shaded, slighlty dirt textured background to the GUI.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="bounds"></param>
        /// <param name="withTitleBar"></param>
        /// <param name="strokeWidth"></param>
        /// <param name="alpha"></param>
        /// <returns></returns>
        public static GuiComposer AddShadedDialogBG(this GuiComposer composer, ElementBounds bounds, bool withTitleBar = true, double strokeWidth = 5, float alpha = 0.75f)
        {
            if (!composer.Composed)
            {
                composer.AddStaticElement(new GuiElementDialogBackground(composer.Api, bounds, withTitleBar, strokeWidth, alpha));
            }
            return composer;
        }

        public static GuiComposer AddDialogBG(this GuiComposer composer, ElementBounds bounds, bool withTitleBar = true, float alpha = 1)
        {
            if (!composer.Composed)
            {
                GuiElementDialogBackground elem = new GuiElementDialogBackground(composer.Api, bounds, withTitleBar, 0, alpha);
                elem.Shade = false;  
                composer.AddStaticElement(elem);
            }
            return composer;
        }
    }

}
