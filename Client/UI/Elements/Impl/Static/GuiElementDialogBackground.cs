using System;
using Cairo;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public class GuiElementDialogBackground : GuiElement
    {
        //bool hotBarLayout = false;
        bool strongBackground;
        double topPadding = 0;

        /// <summary>
        /// Adds a Background to the Dialog.
        /// </summary>
        /// <param name="capi">The Client API</param>
        /// <param name="bounds">The bounds of the element.</param>
        /// <param name="strongBackground">Whether the background is strong or not.</param>
        /// <param name="hotBarLayout">Whether or not the hotbar is rendered in this gui.</param>
        /// <param name="topPadding">The top padding area of the GUI</param>
        public GuiElementDialogBackground(ICoreClientAPI capi, ElementBounds bounds, bool strongBackground, double topPadding = 0) : base(capi, bounds)
        {
            //this.hotBarLayout = hotBarLayout;
            this.topPadding = topPadding;
            this.strongBackground = strongBackground;
        }


        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();
            
            /*if (hotBarLayout)
            {
                double midBoxWidth = 0.15 * Bounds.ParentBounds.OuterWidth;
                double midBoxHeight = 0.2 * Bounds.OuterHeight;

                //bounds.fixedY = bounds.OuterHeight / 2;
                //bounds.calcWorldBounds();

                double radius = GuiStyle.DialogBGRadius;
                double x = Bounds.bgDrawX;
                double y = Bounds.bgDrawY + midBoxHeight + topPadding;
                double width = Bounds.OuterWidth;
                double height = Bounds.OuterHeight;
                double degrees = Math.PI / 180.0;

                ctx.Antialias = Antialias.Best;
                ctx.NewPath();
                ctx.Arc(x + width - radius, y + radius, radius, -90 * degrees, 0 * degrees);
                ctx.LineTo(x + width, y + height);
                ctx.LineTo(x, y + height);
                ctx.Arc(x + radius, y + radius, radius, 180 * degrees, 270 * degrees);

                ctx.ArcNegative(x + width / 2 - midBoxWidth / 2 - radius, y - radius, radius, 90 * degrees, 0 * degrees);
                ctx.Arc(x + width / 2 - midBoxWidth / 2 + radius, y - midBoxHeight + radius, radius, 180 * degrees, 270 * degrees);
                ctx.Arc(x + width / 2 + midBoxWidth / 2 - radius, y - midBoxHeight + radius, radius, 270 * degrees, 0 * degrees);
                ctx.ArcNegative(x + width / 2 + midBoxWidth / 2 + radius, y - radius, radius, -180 * degrees, -270 * degrees);

                ctx.ClosePath();
            }
            else*/
            {
                RoundRectangle(ctx, Bounds.bgDrawX, Bounds.bgDrawY + topPadding, Bounds.OuterWidth, Bounds.OuterHeight - topPadding, GuiStyle.DialogBGRadius);
            }

            if (strongBackground)
            {
                ctx.SetSourceRGBA(GuiStyle.DialogStrongBgColor);
            } else
            {
                ctx.SetSourceRGBA(GuiStyle.DialogDefaultBgColor);
            }
            
            ctx.FillPreserve();       

            // Shadings at the rectangle border
            ShadePath(ctx);
        }
    }


    public static partial class GuiComposerHelpers
    {

        // Single rectangle shape
        /// <summary>
        /// Adds a single rectangle background to the GUI.
        /// </summary>
        /// <param name="bounds">The bounds of the GUI</param>
        /// <param name="strongBackground">Whether or not the background is strong.</param>
        /// <param name="topPadding">The amount of padding at the top of the gui.</param>
        public static GuiComposer AddDialogBG(this GuiComposer composer, ElementBounds bounds, bool strongBackground = false, double topPadding = 0)
        {
            if (!composer.composed)
            {
                composer.AddStaticElement(new GuiElementDialogBackground(composer.Api, bounds, strongBackground, topPadding));
            }
            return composer;
        }

        // Multi rectangle shape
        /// <summary>
        /// Adds a multi-rectangle background.
        /// </summary>
        /// <param name="bounds">The bounds of the GUI</param>
        /// <param name="strongBackground">Whether or not the background is strong.</param>
        /*public static GuiComposer AddDialogBGHotBar(this GuiComposer composer, ElementBounds bounds, bool strongBackground = false)
        {
            if (!composer.composed)
            {
                composer.AddStaticElement(new GuiElementDialogBackground(composer.Api, bounds, strongBackground, true));
            }
            return composer;
        }*/

    }

}
