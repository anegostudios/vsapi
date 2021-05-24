using System;
using Cairo;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public class GuiElementDialogBackground : GuiElement
    {
        public bool Shade = true;
        bool withTitlebar;
        double strokeWidth = 0;

        public float Alpha = 1;

        /// <summary>
        /// Adds a Background to the Dialog.
        /// </summary>
        /// <param name="capi">The Client API</param>
        /// <param name="bounds">The bounds of the element.</param>
        /// <param name="withTitlebar">Minor style adjustments to accomodate title bar</param>
        /// <param name="hotBarLayout">Whether or not the hotbar is rendered in this gui.</param>
        /// <param name="strokeWidth">The top padding area of the GUI</param>
        public GuiElementDialogBackground(ICoreClientAPI capi, ElementBounds bounds, bool withTitlebar, double strokeWidth = 0) : base(capi, bounds)
        {
            this.strokeWidth = strokeWidth;
            this.withTitlebar = withTitlebar;
        }


        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();

            double titleBarOffY = withTitlebar ? scaled(GuiStyle.TitleBarHeight) : 0;

            RoundRectangle(ctx, Bounds.bgDrawX, Bounds.bgDrawY + titleBarOffY, Bounds.OuterWidth, Bounds.OuterHeight - titleBarOffY, GuiStyle.DialogBGRadius);

            ctx.SetSourceRGBA(GuiStyle.DialogStrongBgColor[0], GuiStyle.DialogStrongBgColor[1], GuiStyle.DialogStrongBgColor[2], GuiStyle.DialogStrongBgColor[3] * Alpha);
            ctx.FillPreserve();

            if (Shade)
            {
                ctx.SetSourceRGBA(GuiStyle.DialogLightBgColor[0] * 1.6, GuiStyle.DialogStrongBgColor[1] * 1.6, GuiStyle.DialogStrongBgColor[2] * 1.6, Alpha);
                ctx.LineWidth = strokeWidth * 1.75;
                ctx.StrokePreserve();
                surface.Blur(5.2, (int)Bounds.bgDrawX, (int)(Bounds.bgDrawY + titleBarOffY), (int)(Bounds.bgDrawX + Bounds.OuterWidth), (int)(Bounds.bgDrawY + Bounds.OuterHeight - 1));

                ctx.SetSourceRGBA(new double[] { 45 / 255.0, 35 / 255.0, 33 / 255.0, Alpha*Alpha });
                ctx.LineWidth = strokeWidth;
                ctx.Stroke();
            } else
            {
                ctx.SetSourceRGBA(new double[] { 45 / 255.0, 35 / 255.0, 33 / 255.0, Alpha });
                ctx.LineWidth = 2;
                ctx.Stroke();
            }

            /*double off = strokeWidth / 2 + 1;
            RoundRectangle(ctx, Bounds.bgDrawX + off, Bounds.bgDrawY + titleBarOffY + (titleBarOffY > 0 ? 0 : off), Bounds.OuterWidth - 2*off, Bounds.OuterHeight - titleBarOffY - off - (titleBarOffY > 0 ? 0 : off), GuiStyle.DialogBGRadius);
            ctx.SetSourceRGBA(0, 0, 0, 0.4);
            ctx.LineWidth = 1.5;
            ctx.Stroke();*/
        }
    }


    public static partial class GuiComposerHelpers
    {

        // Single rectangle shape
        /// <summary>
        /// Adds a single rectangle background to the GUI.
        /// </summary>
        /// <param name="bounds">The bounds of the GUI</param>
        /// <param name="withTitleBar">Minor style adjustments to accomodate titlebars</param>
        /// <param name="topPadding">The amount of padding at the top of the gui.</param>
        public static GuiComposer AddShadedDialogBG(this GuiComposer composer, ElementBounds bounds, bool withTitleBar = true, double strokeWidth = 5)
        {
            if (!composer.composed)
            {
                composer.AddStaticElement(new GuiElementDialogBackground(composer.Api, bounds, withTitleBar, strokeWidth));
            }
            return composer;
        }

        public static GuiComposer AddDialogBG(this GuiComposer composer, ElementBounds bounds, bool withTitleBar = true)
        {
            if (!composer.composed)
            {
                GuiElementDialogBackground elem = new GuiElementDialogBackground(composer.Api, bounds, withTitleBar);
                elem.Shade = false;  
                composer.AddStaticElement(elem);
            }
            return composer;
        }
    }

}
