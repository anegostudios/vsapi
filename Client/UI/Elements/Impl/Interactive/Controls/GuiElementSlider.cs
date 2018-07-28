using System;
using Cairo;
using Vintagestory.API.MathTools;
using Vintagestory.API.Common;

namespace Vintagestory.API.Client
{
    public delegate string SliderTooltipDelegate(int currentValue);

    public class GuiElementSlider : GuiElementControl
    {
        int minValue = 0;
        int maxValue = 100;
        int step = 1;
        string unit = "";

        int currentValue;
        int alarmValue; // Shows red beyond this point

        bool mouseDownOnSlider = false;
        bool triggerOnMouseUp = false;
        bool didChangeValue = false;

        int handleTextureId;
        int hoverTextTextureId;
        int waterTextureId;

        GuiElementStaticText textElem;

        int alarmValueTextureId;
        Rectangled alarmTextureRect;

        ActionConsumable<int> onNewSliderValue;
        public SliderTooltipDelegate OnSliderTooltip;


        internal const int unscaledHeight = 20;
        internal const int unscaledPadding = 4;

        int unscaledHandleWidth = 15;
        int unscaledHandleHeight = 35;

        int unscaledHoverTextHeight = 50;

        double handleWidth;
        double handleHeight;
        double hoverTextWidth;
        double hoverTextHeight;
        double padding;


        public override bool Focusable { get { return true; } }


        public GuiElementSlider(ICoreClientAPI capi, ActionConsumable<int> onNewSliderValue, ElementBounds bounds) : base(capi, bounds)
        {
            this.onNewSliderValue = onNewSliderValue;
        }


        public override void ComposeElements(Context ctxStatic, ImageSurface surfaceStatic)
        {
            handleWidth = scaled(unscaledHandleWidth) * Scale;
            handleHeight = scaled(unscaledHandleHeight) * Scale;
            hoverTextWidth = scaled(unscaledHoverTextHeight);
            hoverTextHeight = scaled(unscaledHoverTextHeight);
            padding = scaled(unscaledPadding) * Scale;

            Bounds.CalcWorldBounds();

            ctxStatic.SetSourceRGBA(0, 0, 0, 0.2);
            RoundRectangle(ctxStatic, Bounds.drawX, Bounds.drawY, Bounds.InnerWidth, Bounds.InnerHeight, 3);
            ctxStatic.Fill();
            EmbossRoundRectangleElement(ctxStatic, Bounds, true, 1, 2);


            double insetWidth = Bounds.InnerWidth - 2 * padding;
            double insetHeight = Bounds.InnerHeight - 2 * padding;


            if (alarmValue > 0 && alarmValue < maxValue)
            {
                float alarmValueRel = (float)alarmValue / maxValue;

                alarmTextureRect = new Rectangled() { X = padding + (Bounds.InnerWidth - 2 * padding) * alarmValueRel, Y = padding, Width = (Bounds.InnerWidth - 2 * padding) * (1 - alarmValueRel), Height = Bounds.InnerHeight - 2 * padding };

                ctxStatic.SetSourceRGBA(0.62, 0, 0, 0.4);

                RoundRectangle(ctxStatic, Bounds.drawX + padding + insetWidth * alarmValueRel, Bounds.drawY + padding, insetWidth * (1 - alarmValueRel), insetHeight, 3);
                ctxStatic.Fill();
            }


            /*** 2. Handle ***/
            ImageSurface surface = new ImageSurface(Format.Argb32, (int)handleWidth + 4, (int)handleHeight + 4);
            Context ctx = genContext(surface);

            ctx.SetSourceRGBA(1, 1, 1, 0);
            ctx.Paint();
            
            RoundRectangle(ctx, 2, 2, handleWidth, handleHeight, 3);
            fillWithPattern(api, ctx, woodTextureName, true);

            ctx.SetSourceRGB(43 / 255.0, 33 / 255.0, 24 / 255.0);
            ctx.LineWidth = 2;
            ctx.Stroke();
            
            generateTexture(surface, ref handleTextureId);
            ctx.Dispose();
            surface.Dispose();

            ComposeWaterTexture();
            ComposeHoverTextElement();
        }


        internal void ComposeHoverTextElement()
        {
            ElementBounds bounds = new ElementBounds().WithFixedPadding(7).WithParent(ElementBounds.Empty);

            string text = currentValue + unit;
            if (OnSliderTooltip != null)
            {
                text = OnSliderTooltip(currentValue);
            }

            textElem = new GuiElementStaticText(api, text, EnumTextOrientation.Center, bounds, CairoFont.WhiteMediumText().WithFontSize((float)ElementGeometrics.SubNormalFontSize));
            textElem.Font.UnscaledFontsize = ElementGeometrics.SmallishFontSize;
            textElem.AutoBoxSize();
            textElem.Bounds.CalcWorldBounds();

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)bounds.OuterWidth, (int)bounds.OuterHeight);
            Context ctx = genContext(surface);

            ctx.SetSourceRGBA(1, 1, 1, 0);
            ctx.Paint();
            ctx.SetSourceRGBA(ElementGeometrics.DialogStrongBgColor);
            RoundRectangle(ctx, 0, 0, bounds.OuterWidth, bounds.OuterHeight, ElementGeometrics.ElementBGRadius);
            ctx.FillPreserve();
            double[] color = ElementGeometrics.DialogStrongBgColor;
            ctx.SetSourceRGBA(color[0] / 2, color[1] / 2, color[2] / 2, color[3]);
            ctx.Stroke();

            textElem.ComposeElements(ctx, surface);

            generateTexture(surface, ref hoverTextTextureId);
            ctx.Dispose();
            surface.Dispose();
        }

        internal void ComposeWaterTexture()
        {
            double sliderWidth = Bounds.InnerWidth - 2 * padding - handleWidth / 2;
            double handlePosition = sliderWidth * (1.0 * currentValue - minValue) / (maxValue - minValue);
            double insetHeight = Bounds.InnerHeight - 2 * padding;

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)(handlePosition + 5), (int)insetHeight);
            Context ctx = genContext(surface);

            SurfacePattern pattern = getPattern(api, waterTextureName);
            RoundRectangle(ctx, 0, 0, surface.Width, surface.Height, 2);
            ctx.SetSource(pattern);
            ctx.Fill();           

            generateTexture(surface, ref waterTextureId);
            ctx.Dispose();
            surface.Dispose();
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            if ((float)(alarmValue - minValue) / (maxValue - minValue) > 0 && alarmValueTextureId > 0)
            {
                float alarmValueRel = (float)alarmValue / maxValue;
                api.Render.RenderTexture(alarmValueTextureId, Bounds.renderX + alarmTextureRect.X, Bounds.renderY + alarmTextureRect.Y, alarmTextureRect.Width, alarmTextureRect.Height);
            }

            double sliderWidth = Bounds.InnerWidth - 2 * padding - handleWidth / 2;

            // Translate current value into position
            double handlePosition = sliderWidth * (1.0 * currentValue - minValue) / (maxValue - minValue);
            double insetHeight = Bounds.InnerHeight - 2 * padding;
            double dy = (handleHeight - Bounds.OuterHeight + padding) / 2;

            api.Render.Render2DTexturePremultipliedAlpha(waterTextureId, Bounds.renderX + padding, Bounds.renderY + padding, (int)(handlePosition + 5), (int)insetHeight);

            api.Render.Render2DTexturePremultipliedAlpha(handleTextureId, Bounds.renderX + handlePosition, Bounds.renderY - dy, (int)handleWidth + 4, (int)handleHeight + 4);


            if (mouseDownOnSlider || Bounds.PointInside(api.Input.GetMouseCurrentX(), api.Input.GetMouseCurrentY()))
            {
                ElementBounds elemBounds = textElem.Bounds;
                api.Render.Render2DTexturePremultipliedAlpha(
                    hoverTextTextureId,
                    (int)(Bounds.renderX + padding + handlePosition - elemBounds.OuterWidth / 2 + handleWidth / 2),
                    (int)(Bounds.renderY - scaled(20) - elemBounds.OuterHeight),
                    elemBounds.OuterWidthInt,
                    elemBounds.OuterHeightInt);
            }
        }


        void MakeAlarmValueTexture()
        {
            float alarmValueRel = (float)(alarmValue - minValue) / (maxValue - minValue);

            alarmTextureRect = new Rectangled() { X = padding + (Bounds.InnerWidth - 2 * padding) * alarmValueRel, Y = padding, Width = (Bounds.InnerWidth - 2 * padding) * (1 - alarmValueRel), Height = Bounds.InnerHeight - 2 * padding };


            ImageSurface surface = new ImageSurface(Format.Argb32, (int)alarmTextureRect.Width, (int)alarmTextureRect.Height);
            Context ctx = genContext(surface);

            ctx.SetSourceRGBA(1, 0, 1, 0.4);

            RoundRectangle(ctx, 0, 0, alarmTextureRect.Width, alarmTextureRect.Height, ElementGeometrics.ElementBGRadius);
            ctx.Fill();

            generateTexture(surface, ref alarmValueTextureId);
            ctx.Dispose();
            surface.Dispose();
        }

        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            if (!enabled) return;

            if (!Bounds.PointInside(api.Input.GetMouseCurrentX(), api.Input.GetMouseCurrentY())) return;

            args.Handled = updateValue(api.Input.GetMouseCurrentX());

            mouseDownOnSlider = true;
        }

        public override void OnMouseUp(ICoreClientAPI api, MouseEvent args)
        {
            if (!enabled) return;

            mouseDownOnSlider = false;

            if (onNewSliderValue != null && didChangeValue && triggerOnMouseUp)
            {
                onNewSliderValue(currentValue);
            }

            didChangeValue = false;
        }


        public override void OnMouseMove(ICoreClientAPI api, MouseEvent args)
        {
            if (!enabled) return;

            if (mouseDownOnSlider)
            {
                args.Handled = updateValue(api.Input.GetMouseCurrentX());
            }
        }


        public override void OnMouseWheel(ICoreClientAPI api, MouseWheelEventArgs args)
        {
            if (!Bounds.PointInside(api.Input.GetMouseCurrentX(), api.Input.GetMouseCurrentY())) return;
            args.SetHandled(true);

            int dir = Math.Sign(args.deltaPrecise);
            currentValue = Math.Max(minValue, Math.Min(maxValue, currentValue + dir * step));

            ComposeHoverTextElement();
            ComposeWaterTexture();

            if (onNewSliderValue != null)
            {    
                onNewSliderValue(currentValue);
            }
        }


        /// <summary>
        /// Trigger event only once user release the mouse
        /// </summary>
        /// <param name="trigger"></param>
        internal void TriggerOnlyOnMouseUp(bool trigger = true)
        {
            this.triggerOnMouseUp = trigger;
        }



        bool updateValue(int mouseX)
        {
            double sliderWidth = Bounds.InnerWidth - 2 * padding - handleWidth / 2;
            // Translate mouse position into current value
            double mouseDeltaX = GameMath.Clamp(mouseX - Bounds.renderX - padding, 0, sliderWidth);

            double value = minValue + (maxValue - minValue) * mouseDeltaX / sliderWidth;

            // Round to next step
            int newValue = Math.Max(minValue, Math.Min(maxValue, step * (int)Math.Round(1.0 * value / step)));

            bool didChangeNow = newValue != currentValue;

            if (didChangeNow) didChangeValue = true;
            currentValue = newValue;

            ComposeHoverTextElement();

            if (onNewSliderValue != null)
            {
                ComposeWaterTexture();
                if (!triggerOnMouseUp && didChangeNow) return onNewSliderValue(currentValue);
            }

            return true;
        }



        public void SetAlarmValue(int value)
        {
            alarmValue = value;
            MakeAlarmValueTexture();
        }


        public void SetValues(int currentValue, int minValue, int maxValue, int step, string unit = "")
        {
            this.currentValue = currentValue;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.step = step;
            this.unit = unit;

            ComposeHoverTextElement();
            ComposeWaterTexture();
        }

        public int GetValue()
        {
            return currentValue;
        }
    }



    public static partial class GuiComposerHelpers
    {

        public static GuiComposer AddSlider(this GuiComposer composer, ActionConsumable<int> onNewSliderValue, ElementBounds bounds, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementSlider(composer.Api, onNewSliderValue, bounds), key);
            }
            return composer;
        }

        public static GuiElementSlider GetSlider(this GuiComposer composer, string key)
        {
            return (GuiElementSlider)composer.GetElement(key);
        }
    }
}
