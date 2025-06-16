using System;
using Cairo;

#nullable disable

namespace Vintagestory.API.Client
{
    public class GuiElementCompactScrollbar : GuiElementScrollbar
    {
        /// <summary>
        /// The padding around the scrollbar.
        /// </summary>
        public static int scrollbarPadding = 2;

        /// <summary>
        /// Can this GUIElement be focusable? (default: true).
        /// </summary>
        public override bool Focusable { get { return enabled; } }

        /// <summary>
        /// Scrollbar constructor.
        /// </summary>
        /// <param name="capi">Client API</param>
        /// <param name="onNewScrollbarValue">Event for the changing of the scrollbar or scrolling of the mousewheel.</param>
        /// <param name="bounds">the bounding box of the scrollbar.</param>
        public GuiElementCompactScrollbar(ICoreClientAPI capi, Action<float> onNewScrollbarValue, ElementBounds bounds) : base(capi, onNewScrollbarValue, bounds) {

        }

        /// <summary>
        /// Composes the element.
        /// </summary>
        /// <param name="ctxStatic">The context of the element</param>
        /// <param name="surface">The surface of the image for the element (Not used, can be null.)</param>
        public override void ComposeElements(Context ctxStatic, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();

            ctxStatic.SetSourceRGBA(0, 0, 0, 0.2);
            RoundRectangle(ctxStatic, Bounds.drawX, Bounds.drawY, Bounds.InnerWidth, Bounds.InnerHeight, 1);
            ctxStatic.Fill();

            EmbossRoundRectangleElement(ctxStatic, Bounds, true, 2, 1);

            RecomposeHandle();
        }

        public override void RecomposeHandle()
        {
            Bounds.CalcWorldBounds();

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)Bounds.InnerWidth-1, (int)currentHandleHeight + 1);
            Context ctx = genContext(surface);

            RoundRectangle(ctx, 0, 0, Bounds.InnerWidth - 1, currentHandleHeight, 2);
            ctx.SetSourceRGBA(GuiStyle.DialogDefaultBgColor[0], GuiStyle.DialogDefaultBgColor[1], GuiStyle.DialogDefaultBgColor[2], GuiStyle.DialogDefaultBgColor[3]);
            ctx.Fill();

            EmbossRoundRectangleElement(ctx, 0, 0, Bounds.InnerWidth - 1, currentHandleHeight, false, 2, 2);

            generateTexture(surface, ref handleTexture);

            ctx.Dispose();
            surface.Dispose();
        }

        /// <summary>
        /// Renders the element.
        /// </summary>
        /// <param name="deltaTime">The amount of time that has passed.</param>
        public override void RenderInteractiveElements(float deltaTime)
        {
            api.Render.Render2DTexturePremultipliedAlpha(
                handleTexture.TextureId,
                (float)(Bounds.renderX + Bounds.absPaddingX + 1),
                (float)(Bounds.renderY + Bounds.absPaddingY + currentHandlePosition),
                (float)Bounds.InnerWidth-1,
                currentHandleHeight + 1,
                200 + zOffset
            );
        }



    }

    public static partial class GuiComposerHelpers
    {
        /// <summary>
        /// Adds a compact vertical scrollbar to the current GUI.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="onNewScrollbarValue">The event fired for the change in the scrollbar.</param>
        /// <param name="bounds">the bounds of the scrollbar.</param>
        /// <param name="key">the internal name of the scrollbar.</param>
        public static GuiComposer AddCompactVerticalScrollbar(this GuiComposer composer, Action<float> onNewScrollbarValue, ElementBounds bounds, string key = null)
        {
            if (!composer.Composed)
            {
                composer.AddInteractiveElement(new GuiElementCompactScrollbar(composer.Api, onNewScrollbarValue, bounds), key);
            }
            return composer;
        }

        /// <summary>
        /// Gets the scrollbar from the dialogue.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="key">the internal name of the scrollbar to be gotten</param>
        /// <returns>The scrollbar with the given key.</returns>
        public static GuiElementCompactScrollbar GetCompactScrollbar(this GuiComposer composer, string key)
        {
            return (GuiElementCompactScrollbar)composer.GetElement(key);
        }
    }
}
