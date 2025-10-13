using Cairo;
using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable

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

        HashSet<int> skipValues = [];
        List<int> allowValues = [];

        bool mouseDownOnSlider = false;
        bool mouseOnSlider = false;

        bool triggerOnMouseUp = false;
        bool didChangeValue = false;
        public bool TooltipExceedClipBounds { get; set; } = false;
        public bool ShowTextWhenResting { get; set; } = false;

        LoadedTexture handleTexture;
        LoadedTexture hoverTextTexture;
        LoadedTexture restingTextTexture;
        LoadedTexture sliderFillTexture;
        LoadedTexture alarmValueTexture;

        GuiElementStaticText textElem;
        GuiElementStaticText textElemResting;


        Rectangled alarmTextureRect;

        ActionConsumable<int> onNewSliderValue;
        public SliderTooltipDelegate OnSliderTooltip;
        public SliderTooltipDelegate OnSliderRestingText;


        internal const int unscaledHeight = 20;
        internal const int unscaledPadding = 4;

        int unscaledHandleWidth = 15;
        int unscaledHandleHeight = 35;

        int unscaledHoverTextHeight = 50;

        double handleWidth;
        double handleHeight;
        double padding;


        public override bool Enabled
        {
            get => base.Enabled;
            set
            {
                enabled = value;
                ComposeHandleElement();
                if (alarmValue > minValue && alarmValue < maxValue) MakeAlarmValueTexture();
                ComposeHoverTextElement();
                ComposeRestingTextElement();
                ComposeFillTexture();
            }
        }

        public override bool Focusable { get { return enabled; } }

        /// <summary>
        /// Builds a slider.  A horizontal customizeable slider.
        /// </summary>
        /// <param name="capi">The Client API</param>
        /// <param name="onNewSliderValue">The event that's fired when the slider changed.</param>
        /// <param name="bounds">the bounds of the object.</param>
        public GuiElementSlider(ICoreClientAPI capi, ActionConsumable<int> onNewSliderValue, ElementBounds bounds) : base(capi, bounds)
        {
            handleTexture = new LoadedTexture(capi);
            hoverTextTexture = new LoadedTexture(capi);
            restingTextTexture = new LoadedTexture(capi);
            sliderFillTexture = new LoadedTexture(capi);
            alarmValueTexture = new LoadedTexture(capi);

            this.onNewSliderValue = onNewSliderValue;
        }


        public override void ComposeElements(Context ctxStatic, ImageSurface surfaceStatic)
        {
            handleWidth = scaled(unscaledHandleWidth) * Scale;
            handleHeight = scaled(unscaledHandleHeight) * Scale;
            padding = scaled(unscaledPadding) * Scale;

            Bounds.CalcWorldBounds();

            ctxStatic.SetSourceRGBA(0, 0, 0, 0.2);
            RoundRectangle(ctxStatic, Bounds.drawX, Bounds.drawY, Bounds.InnerWidth, Bounds.InnerHeight, 1);
            ctxStatic.Fill();
            EmbossRoundRectangleElement(ctxStatic, Bounds, true, 1, 1);


            double insetWidth = Bounds.InnerWidth - 2 * padding;
            double insetHeight = Bounds.InnerHeight - 2 * padding;



            ComposeHandleElement();
            ComposeFillTexture();
            ComposeHoverTextElement();
            ComposeRestingTextElement();
        }


        internal void ComposeHandleElement()
        {
            ImageSurface surface = new ImageSurface(Format.Argb32, (int)handleWidth + 4, (int)handleHeight + 4);
            Context ctx = genContext(surface);

            ctx.SetSourceRGBA(1, 1, 1, 0);
            ctx.Paint();

            RoundRectangle(ctx, 2, 2, handleWidth, handleHeight, 1);
            if (!enabled)
            {
                ctx.SetSourceRGB(43 / 255.0, 33 / 255.0, 24 / 255.0);
                ctx.FillPreserve();
            }
            fillWithPattern(api, ctx, woodTextureName, false, true, enabled ? 255 : 159, 0.5f);

            ctx.SetSourceRGB(43 / 255.0, 33 / 255.0, 24 / 255.0);
            ctx.LineWidth = 2;
            ctx.Stroke();

            generateTexture(surface, ref handleTexture);
            ctx.Dispose();
            surface.Dispose();
        }

        internal void ComposeHoverTextElement()
        {
            ElementBounds bounds = new ElementBounds().WithFixedPadding(7).WithParent(ElementBounds.Empty);

            string text = currentValue + unit;
            if (OnSliderTooltip != null) text = OnSliderTooltip(currentValue);

            textElem = new GuiElementStaticText(api, text, EnumTextOrientation.Center, bounds, CairoFont.WhiteMediumText().WithFontSize((float)GuiStyle.SubNormalFontSize));
            textElem.Font.UnscaledFontsize = GuiStyle.SmallishFontSize;
            textElem.AutoBoxSize();
            textElem.Bounds.CalcWorldBounds();

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)bounds.OuterWidth, (int)bounds.OuterHeight);
            Context ctx = genContext(surface);

            ctx.SetSourceRGBA(1, 1, 1, 0);
            ctx.Paint();
            ctx.SetSourceRGBA(GuiStyle.DialogStrongBgColor);
            RoundRectangle(ctx, 0, 0, bounds.OuterWidth, bounds.OuterHeight, GuiStyle.ElementBGRadius);
            ctx.FillPreserve();
            double[] color = GuiStyle.DialogStrongBgColor;
            ctx.SetSourceRGBA(color[0] / 2, color[1] / 2, color[2] / 2, color[3]);
            ctx.Stroke();

            textElem.ComposeElements(ctx, surface);

            generateTexture(surface, ref hoverTextTexture);
            ctx.Dispose();
            surface.Dispose();
        }

        internal void ComposeRestingTextElement()
        {
            ElementBounds bounds = new ElementBounds().WithFixedPadding(7).WithParent(ElementBounds.Empty);
            double sliderWidth = Bounds.InnerWidth - 2 * padding - handleWidth / 2;
            double handlePosition = sliderWidth * (1.0 * currentValue - minValue) / (maxValue - minValue);

            string text = currentValue + unit;
            if (OnSliderRestingText != null) text = OnSliderRestingText(currentValue);
            else if (OnSliderTooltip != null) text = OnSliderTooltip(currentValue);

            textElemResting = new GuiElementStaticText(api, text, EnumTextOrientation.Center, bounds, CairoFont.WhiteSmallText());
            textElemResting.AutoBoxSize();
            textElemResting.Bounds.CalcWorldBounds();
            textElemResting.Bounds.fixedY = ((int)(scaled(30) * Scale) - textElemResting.Font.GetFontExtents().Height) / 2 / RuntimeEnv.GUIScale;

            if (!enabled) textElemResting.Font.Color[3] = 0.35;
            if ((handlePosition - 10) >= textElemResting.Bounds.InnerWidth) textElemResting.Font.Color = [0, 0, 0, enabled? 1 : 0.5];

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)bounds.OuterWidth, (int)bounds.OuterHeight);
            Context ctx = genContext(surface);

            textElemResting.ComposeElements(ctx, surface);

            generateTexture(surface, ref restingTextTexture);
            ctx.Dispose();
            surface.Dispose();
        }


        internal void ComposeFillTexture()
        {
            double sliderWidth = Bounds.InnerWidth - 2 * padding - handleWidth / 2;
            double handlePosition = sliderWidth * (1.0 * currentValue - minValue) / (maxValue - minValue);
            double insetHeight = Bounds.InnerHeight - 2 * padding;

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)(handlePosition + 5), (int)insetHeight);
            Context ctx = genContext(surface);

            SurfacePattern pattern = getPattern(api, waterTextureName, true, enabled ? 255 : 127, 0.5f);
            RoundRectangle(ctx, 0, 0, surface.Width, surface.Height, 1);
            if (enabled) ctx.SetSourceRGBA(0, 0, 0, 1);
            else ctx.SetSourceRGBA(0.15, 0.15, 0, 0.65);
            ctx.FillPreserve();
            ctx.SetSource(pattern);
            ctx.Fill();

            generateTexture(surface, ref sliderFillTexture);
            ctx.Dispose();
            surface.Dispose();
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            if ((float)(alarmValue - minValue) / (maxValue - minValue) > 0 && alarmValueTexture.TextureId > 0)
            {
                float alarmValueRel = (float)alarmValue / maxValue;
                api.Render.RenderTexture(alarmValueTexture.TextureId, Bounds.renderX + alarmTextureRect.X, Bounds.renderY + alarmTextureRect.Y, alarmTextureRect.Width, alarmTextureRect.Height);
            }

            double sliderWidth = Bounds.InnerWidth - 2 * padding - handleWidth / 2;

            // Translate current value into position
            double handlePosition = sliderWidth * (1.0 * currentValue - minValue) / (maxValue - minValue);
            double insetHeight = Bounds.InnerHeight - 2 * padding;
            double dy = (handleHeight - Bounds.OuterHeight + padding) / 2;

            api.Render.Render2DTexturePremultipliedAlpha(sliderFillTexture.TextureId, Bounds.renderX + padding, Bounds.renderY + padding, (int)(handlePosition + 5), (int)insetHeight);
            api.Render.Render2DTexturePremultipliedAlpha(handleTexture.TextureId, Bounds.renderX + handlePosition, Bounds.renderY - dy, (int)handleWidth + 4, (int)handleHeight + 4);


            if (mouseDownOnSlider || mouseOnSlider)
            {
                if (TooltipExceedClipBounds) api.Render.PopScissor();
                ElementBounds elemBounds = textElem.Bounds;
                api.Render.Render2DTexturePremultipliedAlpha(
                    hoverTextTexture.TextureId,
                    (int)(Bounds.renderX + padding + handlePosition - elemBounds.OuterWidth / 2 + handleWidth / 2),
                    (int)(Bounds.renderY - scaled(20) - elemBounds.OuterHeight),
                    elemBounds.OuterWidthInt,
                    elemBounds.OuterHeightInt,
                    300
                );
                if (TooltipExceedClipBounds) api.Render.PushScissor(InsideClipBounds);
            }
            if (ShowTextWhenResting)
            {
                api.Render.PushScissor(Bounds, true);
                ElementBounds elemBounds = textElemResting.Bounds;
                double xOffset = (handlePosition - 10) < elemBounds.InnerWidth ? (Bounds.renderX + padding + handlePosition - elemBounds.OuterWidth / 2 + handleWidth / 2 + (restingTextTexture.Width / 2) + 10) : (int)Bounds.renderX;
                api.Render.Render2DTexturePremultipliedAlpha(
                    restingTextTexture.TextureId,
                    xOffset,
                    Bounds.renderY + (insetHeight - elemBounds.OuterHeight - (padding / 2)) / 2,
                    elemBounds.OuterWidthInt,
                    elemBounds.OuterHeightInt,
                    300
                );
                api.Render.PopScissor();
            }
        }





        void MakeAlarmValueTexture()
        {
            float alarmValueRel = (float)(alarmValue - minValue) / (maxValue - minValue);

            alarmTextureRect = new Rectangled() { X = padding + (Bounds.InnerWidth - 2 * padding) * alarmValueRel, Y = padding, Width = (Bounds.InnerWidth - 2 * padding) * (1 - alarmValueRel), Height = Bounds.InnerHeight - 2 * padding };


            ImageSurface surface = new ImageSurface(Format.Argb32, (int)alarmTextureRect.Width, (int)alarmTextureRect.Height);
            Context ctx = genContext(surface);

            ctx.SetSourceRGBA(1, 0, 1, enabled ? 0.4 : 0.25);

            RoundRectangle(ctx, 0, 0, alarmTextureRect.Width, alarmTextureRect.Height, GuiStyle.ElementBGRadius);
            ctx.Fill();

            generateTexture(surface, ref alarmValueTexture.TextureId);
            ctx.Dispose();
            surface.Dispose();
        }

        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            if (!enabled) return;

            if (!Bounds.PointInside(api.Input.MouseX, api.Input.MouseY)) return;

            args.Handled = updateValue(api.Input.MouseX);

            mouseDownOnSlider = true;
        }

        public override void OnMouseUp(ICoreClientAPI api, MouseEvent args)
        {
            mouseDownOnSlider = false;

            if (!enabled) return;

            if (onNewSliderValue != null && didChangeValue && triggerOnMouseUp)
            {
                onNewSliderValue(currentValue);
            }

            didChangeValue = false;
        }


        public override void OnMouseMove(ICoreClientAPI api, MouseEvent args)
        {
            mouseOnSlider = Bounds.PointInside(api.Input.MouseX, api.Input.MouseY);

            if (!enabled) return;

            if (mouseDownOnSlider)
            {
                args.Handled = updateValue(api.Input.MouseX);
            }
        }


        public override void OnMouseWheel(ICoreClientAPI api, MouseWheelEventArgs args)
        {
            if (!enabled) return;

            if (!Bounds.PointInside(api.Input.MouseX, api.Input.MouseY)) return;
            args.SetHandled(true);

            int curIndex = allowValues.IndexOf(currentValue);
            int newIndex = curIndex == -1 ? 0 : curIndex + Math.Sign(args.deltaPrecise);
            if (newIndex < 0 || newIndex >= allowValues.Count) return;
            currentValue = allowValues[newIndex];

            ComposeHoverTextElement();
            ComposeRestingTextElement();
            ComposeFillTexture();

            onNewSliderValue?.Invoke(currentValue);
        }

        public override void OnKeyDown(ICoreClientAPI api, KeyEvent args)
        {
            if (!HasFocus) return;

            int dir = 0;
            if (args.KeyCode == (int)GlKeys.Left)
            {
                if (currentValue <= allowValues.First()) return;
                dir = -1;
            }
            else if (args.KeyCode == (int)GlKeys.Right)
            {
                if (currentValue >= allowValues.Last()) return;
                dir = 1;
            }

            if (dir != 0)
            {
                currentValue = allowValues[allowValues.IndexOf(currentValue) + dir];

                ComposeHoverTextElement();
                ComposeRestingTextElement();
                ComposeFillTexture();

                onNewSliderValue?.Invoke(currentValue);
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
            int newValue = allowValues.Count == 0 ? currentValue : allowValues.OrderBy(item => Math.Abs(value - item)).First();

            if (newValue == currentValue) return true;
            didChangeValue = true;

            currentValue = newValue;

            ComposeHoverTextElement();
            ComposeRestingTextElement();
            ComposeFillTexture();

            if (onNewSliderValue != null && !triggerOnMouseUp)
            {
                return onNewSliderValue(currentValue);
            }

            return true;
        }


        /// <summary>
        /// Sets a value to warn the player that going over this is not a good idea.
        /// </summary>
        /// <param name="value">The maximum limit before things break down.</param>
        public void SetAlarmValue(int value)
        {
            alarmValue = value;
            MakeAlarmValueTexture();
        }

        public void SetSkipValues(HashSet<int> skipValues)
        {
            this.skipValues = skipValues;
            allowValues.Clear();
            for (int i = minValue; i <= maxValue; i += step) if (!skipValues.Contains(i)) allowValues.Add(i);
        }

        public void ClearSkipValues()
        {
            skipValues.Clear();
            allowValues.Clear();
            for (int i = minValue; i <= maxValue; i += step) allowValues.Add(i);
        }

        public void AddSkipValue(int skipValue)
        {
            skipValues.Add(skipValue);
            allowValues.Remove(skipValue);
        }

        public void RemoveSkipValue(int skipValue)
        {
            skipValues.Remove(skipValue);
            allowValues.Clear();
            for (int i = minValue; i <= maxValue; i += step) if (!skipValues.Contains(i)) allowValues.Add(i);
        }

        /// <summary>
        /// Sets the values of the slider.
        /// </summary>
        /// <param name="currentValue">The value the slider is now.</param>
        /// <param name="minValue">The lowest value.</param>
        /// <param name="maxValue">The highest value.</param>
        /// <param name="step">Each step between values.</param>
        /// <param name="unit">The units of the value. %, chunks, ect.</param>
        public void SetValues(int currentValue, int minValue, int maxValue, int step, string unit = "")
        {
            this.currentValue = currentValue;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.step = step;
            this.unit = unit;

            allowValues.Clear();
            for (int i = minValue; i <= maxValue; i += step) if (!skipValues.Contains(i)) allowValues.Add(i);

            ComposeHoverTextElement();
            ComposeRestingTextElement();
            ComposeFillTexture();
        }

        public void SetValue(int currentValue)
        {
            this.currentValue = currentValue;
        }

        /// <summary>
        /// Gets the current value of the slider.
        /// </summary>
        public int GetValue()
        {
            return currentValue;
        }

        public override void Dispose()
        {
            base.Dispose();

            handleTexture.Dispose();
            hoverTextTexture.Dispose();
            restingTextTexture.Dispose();
            sliderFillTexture.Dispose();
            alarmValueTexture.Dispose();
        }
    }



    public static partial class GuiComposerHelpers
    {
        /// <summary>
        /// Adds a slider to the current GUI.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="onNewSliderValue">The event that fires when the slider's value is changed.</param>
        /// <param name="bounds">The bounds of the slider.</param>
        /// <param name="key">the internal name of the slider.</param>
        public static GuiComposer AddSlider(this GuiComposer composer, ActionConsumable<int> onNewSliderValue, ElementBounds bounds, string key = null)
        {
            if (!composer.Composed)
            {
                composer.AddInteractiveElement(new GuiElementSlider(composer.Api, onNewSliderValue, bounds), key);
            }
            return composer;
        }

        /// <summary>
        /// Gets the slider by name from the GUI.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="key">the internal name of the slider.</param>
        /// <returns>the slider.</returns>
        public static GuiElementSlider GetSlider(this GuiComposer composer, string key)
        {
            return (GuiElementSlider)composer.GetElement(key);
        }
    }
}
