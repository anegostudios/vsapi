using System;
using Cairo;
using Vintagestory.API.MathTools;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

#nullable disable

namespace Vintagestory.API.Client
{
    public class GuiElementSliderOld : GuiElement
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
        GuiElementStaticText textElem;

        int alarmValueTextureId;
        Rectangled alarmTextureRect;

        ActionConsumable<int> onNewSliderValue;

        
        internal const int unscaledHeight = 20;
        internal const int unscaledPadding = 6;

        int unscaledHandleWidth = 15;
        int unscaledHandleHeight = 40;

        int unscaledHoverTextHeight = 50;

        double handleWidth;
        double handleHeight;
        double hoverTextWidth;
        double hoverTextHeight;
        double padding;

        public GuiElementSliderOld(ICoreClientAPI capi, ActionConsumable<int> onNewSliderValue, ElementBounds bounds) : base(capi, bounds)
        {
            this.onNewSliderValue = onNewSliderValue;
        }

        public override void ComposeElements(Context ctxStatic, ImageSurface surfaceStatic)
        {
      
            handleWidth = scaled(unscaledHandleWidth);
            handleHeight = scaled(unscaledHandleHeight);
            hoverTextWidth = scaled(unscaledHoverTextHeight);
            hoverTextHeight = scaled(unscaledHoverTextHeight);
            padding = scaled(unscaledPadding);

            Bounds.CalcWorldBounds();


            // Wood bg
            RoundRectangle(ctxStatic, Bounds.drawX, Bounds.drawY, Bounds.InnerWidth, Bounds.InnerHeight, GuiStyle.ElementBGRadius);
            fillWithPattern(api, ctxStatic, woodTextureName);

            EmbossRoundRectangleElement(ctxStatic, Bounds.drawX, Bounds.drawY, Bounds.InnerWidth, Bounds.InnerHeight);


            double insetWidth = Bounds.InnerWidth - 2 * padding;
            double insetHeight = Bounds.InnerHeight - 2 * padding;

            // Wood Inset 
            ctxStatic.SetSourceRGBA(0, 0, 0, 0.6);
            RoundRectangle(ctxStatic, Bounds.drawX + padding, Bounds.drawY + padding, insetWidth, insetHeight, GuiStyle.ElementBGRadius);
            ctxStatic.Fill();

            EmbossRoundRectangleElement(ctxStatic, Bounds.drawX + padding, Bounds.drawY + padding, insetWidth, insetHeight, true);


            if (alarmValue > 0 && alarmValue < maxValue)
            {
                float alarmValueRel = (float)alarmValue / maxValue;

                alarmTextureRect = new Rectangled() { X = padding + (Bounds.InnerWidth - 2 * padding) * alarmValueRel, Y = padding, Width = (Bounds.InnerWidth - 2 * padding) * (1 - alarmValueRel), Height = Bounds.InnerHeight - 2 * padding };

                ctxStatic.SetSourceRGBA(0.62, 0, 0, 0.4);
                
                RoundRectangle(ctxStatic, Bounds.drawX + padding + insetWidth * alarmValueRel, Bounds.drawY + padding, insetWidth * (1- alarmValueRel), insetHeight, GuiStyle.ElementBGRadius);
                ctxStatic.Fill();
            }


            /*** 2. Handle ***/
            ImageSurface surface = new ImageSurface(Format.Argb32, (int)handleWidth+5, (int)handleHeight+5);
            Context ctx = genContext(surface);

            ctx.SetSourceRGBA(1, 1, 1, 0);
            ctx.Paint();
            ctx.SetSourceRGBA(0, 0, 0, 0.5);
            RoundRectangle(ctx, 0, 0, handleWidth, handleHeight, GuiStyle.ElementBGRadius);
            ctx.Fill();
            surface.BlurFull(3);

            RoundRectangle(ctx, 0, 0, handleWidth, handleHeight, GuiStyle.ElementBGRadius);
            fillWithPattern(api, ctx, woodTextureName);

            EmbossRoundRectangleElement(ctx, 0, 0, handleWidth, handleHeight, false);

            generateTexture(surface, ref handleTextureId);
            ctx.Dispose();
            surface.Dispose();

            ComposeHoverTextElement();
        }


        internal void ComposeHoverTextElement()
        {
            ElementBounds bounds = new ElementBounds().WithFixedPadding(7).WithParent(ElementBounds.Empty);

            textElem = new GuiElementStaticText(api, currentValue + unit, EnumTextOrientation.Center, bounds, CairoFont.WhiteMediumText());
            textElem.Font.UnscaledFontsize = GuiStyle.SmallishFontSize;
            textElem.AutoBoxSize();
            textElem.Bounds.CalcWorldBounds();

            ImageSurface surface = new ImageSurface(Format.Argb32, (int)bounds.OuterWidth, (int)bounds.OuterHeight);
            Context ctx = genContext(surface);

            ctx.SetSourceRGBA(1, 1, 1, 0);
            ctx.Paint();
            ctx.SetSourceRGBA(0, 0, 0, 0.3);
            RoundRectangle(ctx, 0, 0, bounds.OuterWidth, bounds.OuterHeight, GuiStyle.ElementBGRadius);
            ctx.Fill();

            textElem.ComposeElements(ctx, surface);

            generateTexture(surface, ref hoverTextTextureId);
            ctx.Dispose();
            surface.Dispose();
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            if ((float)(alarmValue - minValue) / (maxValue - minValue) > 0)
            {
                float alarmValueRel = (float)alarmValue / maxValue;
                api.Render.RenderTexture(alarmValueTextureId, Bounds.renderX + alarmTextureRect.X, Bounds.renderY + alarmTextureRect.Y, alarmTextureRect.Width, alarmTextureRect.Height);
            }

            double sliderWidth = Bounds.InnerWidth - 2 * padding - handleWidth/2;

            // Translate current value into position
            double handlePosition = sliderWidth * (1.0 * currentValue - minValue) / (maxValue - minValue);

            double dy = (handleHeight - Bounds.InnerHeight) / 2;

            api.Render.RenderTexture(handleTextureId, Bounds.renderX + padding + handlePosition, Bounds.renderY - dy, (int)handleWidth+5, (int)handleHeight+5);

            if (mouseDownOnSlider || Bounds.PointInside(api.Input.MouseX, api.Input.MouseY))
            {
                ElementBounds elemBounds = textElem.Bounds;
                api.Render.RenderTexture(
                    hoverTextTextureId, 
                    Bounds.renderX + padding + handlePosition - elemBounds.OuterWidth / 2 + handleWidth / 2, 
                    Bounds.renderY - scaled(20) - elemBounds.OuterHeight,
                    elemBounds.OuterWidth,
                    elemBounds.OuterHeight);
            }
        }


        void MakeAlarmValueTexture()
        {
            float alarmValueRel = (float)(alarmValue - minValue) / (maxValue - minValue);

            alarmTextureRect = new Rectangled() { X = padding + (Bounds.InnerWidth - 2 * padding) * alarmValueRel, Y = padding, Width = (Bounds.InnerWidth - 2 * padding) * (1 - alarmValueRel), Height = Bounds.InnerHeight - 2 * padding };


            ImageSurface surface = new ImageSurface(Format.Argb32, (int)alarmTextureRect.Width, (int)alarmTextureRect.Height);
            Context ctx = genContext(surface);



            ctx.SetSourceRGBA(1, 0, 1, 0.4);

            RoundRectangle(ctx, 0, 0, alarmTextureRect.Width, alarmTextureRect.Height, GuiStyle.ElementBGRadius);
            ctx.Fill();

            generateTexture(surface, ref alarmValueTextureId);
            ctx.Dispose();
            surface.Dispose();
        }

        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            if (!Bounds.PointInside(api.Input.MouseX, api.Input.MouseY)) return;

            args.Handled = updateValue(api.Input.MouseX);

            mouseDownOnSlider = true;
        }

        public override void OnMouseUp(ICoreClientAPI api, MouseEvent args)
        {
            mouseDownOnSlider = false;
            
            if (onNewSliderValue != null && didChangeValue && triggerOnMouseUp)
            {
                onNewSliderValue(currentValue);
            }

            didChangeValue = false;
        }


        public override void OnMouseMove(ICoreClientAPI api, MouseEvent args)
        {
            if (mouseDownOnSlider)
            {
                args.Handled = updateValue(api.Input.MouseX);
            }
        }

        


        /// <summary>
        /// Trigger event only once user release the mouse
        /// </summary>
        /// <param name="trigger"></param>
        internal void triggerOnlyOnMouseUp(bool trigger = true)
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
            if (newValue != currentValue) didChangeValue = true;
            currentValue = newValue;

            ComposeHoverTextElement();

            if (onNewSliderValue != null && !triggerOnMouseUp)
            {
                return onNewSliderValue(currentValue);
            }


            return false;
        }



        public void SetAlarmValue(int value)
        {
            alarmValue = value;
            MakeAlarmValueTexture();
        }


        public void setValues(int currentValue, int minValue, int maxValue, int step, string unit = "")
        {
            this.currentValue = currentValue;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.step = step;
            this.unit = unit;

            ComposeHoverTextElement();
        }

        public int GetValue()
        {
            return currentValue;
        }

    }


}
