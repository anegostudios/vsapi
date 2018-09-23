using System;
using Cairo;
using Vintagestory.API.MathTools;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public class GuiElementScrollbar : GuiElementControl
    {
        protected API.Common.Action<float> onNewScrollbarValue;

        public static int scrollbarWidth = 20;
        public static int scrollbarPadding = 2;

        internal bool mouseDownOnScrollbarHandle;
        internal int mouseDownStartY;

        protected float visibleHeight;
        protected float totalHeight;

        protected float currentHandlePosition;
        protected float currentHandleHeight = 0;


        protected LoadedTexture handleTexture;

        public override bool Focusable { get { return true; } }

        /// <summary>
        /// Moving 1 pixel of the scrollbar moves the content by ScrollConversionFactor of pixels
        /// </summary>
        public float ScrollConversionFactor
        {
            get {
                if (this.Bounds.InnerHeight - currentHandleHeight <= 0) return 1;

                float scrollbarMovableArea = (float)(this.Bounds.InnerHeight - currentHandleHeight);
                float innerMovableArea = totalHeight - visibleHeight;

                return innerMovableArea / scrollbarMovableArea;
            }
        }

        /// <summary>
        /// The current Y position of the inner element
        /// </summary>
        public float CurrentYPosition
        {
            get
            {
                return currentHandlePosition * ScrollConversionFactor;
            }
        }

        public GuiElementScrollbar(ICoreClientAPI capi, API.Common.Action<float> onNewScrollbarValue, ElementBounds bounds) : base(capi, bounds)
        {
            handleTexture = new LoadedTexture(capi);

            this.onNewScrollbarValue = onNewScrollbarValue;
        }

        public override void ComposeElements(Context ctxStatic, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();

            ctxStatic.SetSourceRGBA(0, 0, 0, 0.2);
            ElementRoundRectangle(ctxStatic, Bounds, false);
            ctxStatic.Fill();

            EmbossRoundRectangleElement(ctxStatic, Bounds, true);

            RecomposeHandle();
        }

        internal virtual void RecomposeHandle()
        {
            Bounds.CalcWorldBounds();

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)scaled(scrollbarWidth), (int)currentHandleHeight);
            Context ctx = genContext(surface);

            RoundRectangle(ctx, 0, 0, scaled(scrollbarWidth-1), currentHandleHeight, 3);
            ctx.SetSourceRGBA(ElementGeometrics.DialogHighlightColor);
            ctx.FillPreserve();
            ctx.SetSourceRGBA(0, 0, 0, 0.3);
            ctx.Fill();

            EmbossRoundRectangleElement(ctx, 0, 0, scaled(scrollbarWidth-1), currentHandleHeight, false, 2, 3);

            generateTexture(surface, ref handleTexture);

            ctx.Dispose();
            surface.Dispose();
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            api.Render.Render2DTexturePremultipliedAlpha(
                handleTexture.TextureId,
                (float)(Bounds.renderX + Bounds.absPaddingX),
                (float)(Bounds.renderY + Bounds.absPaddingY + currentHandlePosition),
                (float)(scaled(scrollbarWidth)),
                currentHandleHeight
            );
        }


        public void SetHeights(float visibleHeight, float totalHeight)
        {
            this.visibleHeight = visibleHeight;
            SetNewTotalHeight(totalHeight);
        }
        
        public void SetNewTotalHeight(float totalHeight)
        {
            this.totalHeight = totalHeight;

            float heightDiffFactor = GameMath.Clamp(this.visibleHeight / totalHeight, 0, 1);
            currentHandleHeight = (float)(heightDiffFactor * Bounds.InnerHeight);

            currentHandlePosition = (float)Math.Min(currentHandlePosition, Bounds.InnerHeight - currentHandleHeight);
            TriggerChanged();

            RecomposeHandle();
        }

        internal void SetScrollbarPosition(int pos)
        {
            currentHandlePosition = pos;
            onNewScrollbarValue(0);
        }


        public override void OnMouseWheel(ICoreClientAPI api, MouseWheelEventArgs args)
        {
            float y = currentHandlePosition - (float)scaled(102) * args.deltaPrecise / ScrollConversionFactor;

            double scrollbarMoveableHeight = Bounds.InnerHeight - currentHandleHeight;
            currentHandlePosition = (float)GameMath.Clamp(y, 0, scrollbarMoveableHeight);
            TriggerChanged();
            

            args.SetHandled(true);
        }

        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            if (Bounds.PointInside(args.X, args.Y))
            {
                mouseDownOnScrollbarHandle = true;
                mouseDownStartY = GameMath.Max(0, args.Y - (int)Bounds.renderY, 0);
                if (mouseDownStartY > currentHandleHeight) mouseDownStartY = (int)currentHandleHeight / 2;

                UpdateHandlePositionAbs(args.Y - (int)Bounds.renderY - mouseDownStartY);
                args.Handled = true;
            }
        }

        public override void OnMouseUp(ICoreClientAPI api, MouseEvent args)
        {
            mouseDownOnScrollbarHandle = false;
        }

        public override void OnMouseMove(ICoreClientAPI api, MouseEvent args)
        {
            if (mouseDownOnScrollbarHandle)
            {
                UpdateHandlePositionAbs(args.Y - (int)Bounds.renderY - mouseDownStartY);
            }
        }

        void UpdateHandlePositionAbs(float y)
        {
            double scrollbarMoveableHeight = Bounds.InnerHeight - currentHandleHeight;
            currentHandlePosition = (float)GameMath.Clamp(y, 0, scrollbarMoveableHeight);
            TriggerChanged();
        }

        public void TriggerChanged()
        {
            onNewScrollbarValue(CurrentYPosition);
        }

        public void ScrollToBottom()
        {
            // 0..1 
            float currentPositionRelative = 1;

            if (totalHeight < visibleHeight)
            {
                currentHandlePosition = 0;
                currentPositionRelative = 0;
            } else
            {
                currentHandlePosition = (float)(Bounds.InnerHeight - currentHandleHeight);
            }
            

            // 0..element movable height
            float currentPositionAbs = currentPositionRelative * (totalHeight - visibleHeight);

            onNewScrollbarValue(currentPositionAbs);
        }


        internal void EnsureVisible(double posX, double posY)
        {
            double startY = CurrentYPosition;
            double endY = CurrentYPosition + Bounds.InnerHeight;

            if (posY < startY)
            {
                float diff = (float)(startY - posY) / ScrollConversionFactor;                
                currentHandlePosition = Math.Max(0, currentHandlePosition - diff);
                TriggerChanged();
                return;
            }

            if (posY > endY)
            {
                float diff = (float)(posY - endY) / ScrollConversionFactor;
                currentHandlePosition = (float)Math.Min(Bounds.InnerHeight - currentHandleHeight, currentHandlePosition + diff);
                TriggerChanged();
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            handleTexture.Dispose();
        }
    }



    public static partial class GuiComposerHelpers
    {

        public static GuiComposer AddVerticalScrollbar(this GuiComposer composer, API.Common.Action<float> onNewScrollbarValue, ElementBounds bounds, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementScrollbar(composer.Api, onNewScrollbarValue, bounds), key);
            }
            return composer;
        }

        public static GuiElementScrollbar GetScrollbar(this GuiComposer composer, string key)
        {
            return (GuiElementScrollbar)composer.GetElement(key);
        }
    }
}
