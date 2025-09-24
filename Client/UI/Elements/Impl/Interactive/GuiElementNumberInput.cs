using Cairo;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Vintagestory.API.Config;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// A numerical input field for inputting numbers.
    /// </summary>
    public class GuiElementNumberInput : GuiElementTextInput
    {
        //public double Scale = 1;
        public float Interval = 1f;


        public LoadedTexture buttonHighlightTexture;
        private bool focusable = true;
        public bool IntMode { get; set; } = false;

        public override bool Focusable => focusable && enabled;

        /// <summary>
        /// When enabled and a button is clicked it wont focus on it, leaving your focus on the game to move around 
        /// </summary>
        public bool DisableButtonFocus;

        /// <summary>
        /// Creates a numerical input field.
        /// </summary>
        /// <param name="capi">The Client API</param>
        /// <param name="bounds">The bounds of the GUI.</param>
        /// <param name="OnTextChanged">The event fired when the number is changed.</param>
        /// <param name="font">The font of the numbers.</param>
        public GuiElementNumberInput(ICoreClientAPI capi, ElementBounds bounds, Action<string> OnTextChanged, CairoFont font) : base(capi, bounds, OnTextChanged, font)
        {
            buttonHighlightTexture = new LoadedTexture(capi);
        }

        /// <summary>
        /// Gets the current value of the number.
        /// </summary>
        /// <returns>A float representing the value.</returns>
        public float GetValue()
        {
            float.TryParse(GetText(), NumberStyles.Any, GlobalConstants.DefaultCultureInfo, out float val);
            return val;
        }


        public override void ComposeTextElements(Context ctx, ImageSurface surface)
        {
            rightSpacing = scaled(17);

            EmbossRoundRectangleElement(ctx, Bounds, true, 2, 1);
            ctx.SetSourceRGBA(0, 0, 0, 0.2);
            ElementRoundRectangle(ctx, Bounds, false, 1);
            ctx.Fill();

            GenTextHighlightTexture();
            GenButtonHighlightTexture();

            if (!enabled) Font.Color[3] = 0.35f;

            highlightBounds = Bounds.CopyOffsetedSibling().WithFixedPadding(0, 0).FixedGrow(2 * Bounds.absPaddingX, 2 * Bounds.absPaddingY);
            highlightBounds.CalcWorldBounds();

            RecomposeText();

            double heightHalf = Bounds.OuterHeight / 2 - 1;
            double[] buttonColor = GuiStyle.DialogHighlightColor.ToArray();
            if (!enabled) buttonColor[3] = 0.315;

            // Arrow up
            ctx.SetSourceRGBA(buttonColor);
            RoundRectangle(ctx, Bounds.drawX + Bounds.InnerWidth - scaled(17 + 1) * Scale, Bounds.drawY, rightSpacing * Scale, heightHalf, 1);
            ctx.Fill();

            EmbossRoundRectangleElement(ctx, Bounds.drawX + Bounds.InnerWidth - scaled(17 + 1) * Scale, Bounds.drawY, rightSpacing * Scale, heightHalf, false, 2, 1);

            ctx.NewPath();
            ctx.LineTo(Bounds.drawX + Bounds.InnerWidth - scaled(9) * Scale, Bounds.drawY + scaled(1) * Scale);
            ctx.LineTo(Bounds.drawX + Bounds.InnerWidth - scaled(14) * Scale, Bounds.drawY + (heightHalf - scaled(2)) * Scale);
            ctx.LineTo(Bounds.drawX + Bounds.InnerWidth - scaled(4) * Scale, Bounds.drawY + (heightHalf - scaled(2)) * Scale);
            ctx.ClosePath();
            ctx.SetSourceRGBA(1, 1, 1, enabled ? 0.4 : 0.14);
            ctx.Fill();


            // Arrow down
            ctx.SetSourceRGBA(buttonColor);
            RoundRectangle(ctx, Bounds.drawX + Bounds.InnerWidth - (rightSpacing + scaled(1)) * Scale, Bounds.drawY + heightHalf + scaled(1) * Scale, rightSpacing * Scale, heightHalf, 1);
            ctx.Fill();

            EmbossRoundRectangleElement(ctx, Bounds.drawX + Bounds.InnerWidth - (rightSpacing + scaled(1)) * Scale, Bounds.drawY + heightHalf + scaled(1) * Scale, rightSpacing * Scale, heightHalf, false, 2, 1);

            ctx.NewPath();
            ctx.LineTo(Bounds.drawX + Bounds.InnerWidth - scaled(14) * Scale, Bounds.drawY + (heightHalf + scaled(3)) * Scale);
            ctx.LineTo(Bounds.drawX + Bounds.InnerWidth - scaled(4) * Scale, Bounds.drawY + (heightHalf + scaled(3)) * Scale);
            ctx.LineTo(Bounds.drawX + Bounds.InnerWidth - scaled(9) * Scale, Bounds.drawY + (heightHalf * 2) * Scale);
            ctx.ClosePath();
            ctx.SetSourceRGBA(1, 1, 1, enabled ? 0.4 : 0.14);
            ctx.Fill();

            highlightBounds.fixedWidth -= rightSpacing / RuntimeEnv.GUIScale;
            highlightBounds.CalcWorldBounds();
        }

        private void GenButtonHighlightTexture()
        {
            double heightHalf = Bounds.OuterHeight / 2 - 1;

            ImageSurface surfaceHighlight = new ImageSurface(Format.Argb32, (int)(rightSpacing), (int)heightHalf);
            Context ctxHighlight = genContext(surfaceHighlight);

            ctxHighlight.SetSourceRGBA(1, 1, 1, 0.2);
            ctxHighlight.Paint();

            generateTexture(surfaceHighlight, ref buttonHighlightTexture);

            ctxHighlight.Dispose();
            surfaceHighlight.Dispose();
        }

    
        private void GenTextHighlightTexture()
        {
            ImageSurface surfaceHighlight = new ImageSurface(Format.Argb32, (int)(Bounds.OuterWidth - rightSpacing), (int)Bounds.OuterHeight);
            Context ctxHighlight = genContext(surfaceHighlight);

            ctxHighlight.SetSourceRGBA(1, 1, 1, 0.2);
            ctxHighlight.Paint();

            generateTexture(surfaceHighlight, ref highlightTexture);

            ctxHighlight.Dispose();
            surfaceHighlight.Dispose();
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            base.RenderInteractiveElements(deltaTime);

            if (!enabled) return;

            int mouseX = api.Input.MouseX;
            int mouseY = api.Input.MouseY;

            MouseOverCursor = "textselect";

            if (mouseX >= Bounds.absX + Bounds.InnerWidth - scaled(21) && mouseX <= Bounds.absX + Bounds.OuterWidth && mouseY >= Bounds.absY && mouseY <= Bounds.absY + Bounds.OuterHeight)
            {
                MouseOverCursor = null;

                double heightHalf = Bounds.OuterHeight / 2 - 1;

                if (mouseY > Bounds.absY + heightHalf + 1)
                {
                    api.Render.Render2DTexturePremultipliedAlpha(buttonHighlightTexture.TextureId, Bounds.renderX + Bounds.OuterWidth - rightSpacing - 1, Bounds.renderY + heightHalf + 1, rightSpacing, heightHalf);
                } else
                {
                    api.Render.Render2DTexturePremultipliedAlpha(buttonHighlightTexture.TextureId, Bounds.renderX + Bounds.OuterWidth - rightSpacing - 1, Bounds.renderY, rightSpacing, heightHalf);
                }
            }
        }


        public override void OnMouseWheel(ICoreClientAPI api, MouseWheelEventArgs args)
        {
            if (!enabled) return;
            if (!IsPositionInside(api.Input.MouseX, api.Input.MouseY)) return;

            rightSpacing = scaled(17);

            float size = args.deltaPrecise > 0 ? 1 : -1;
            size *= Interval;

            if (api.Input.KeyboardKeyStateRaw[(int)GlKeys.ShiftLeft]) size /= 10;
            if (api.Input.KeyboardKeyStateRaw[(int)GlKeys.ControlLeft]) size /= 100;

            UpdateValue(size);
            args.SetHandled(true);
        }

        private void UpdateValue(float size)
        {
            if (IntMode) size = (int)(size > 0 ? Math.Ceiling(size) : Math.Floor(size));
            double.TryParse(lines[0], NumberStyles.Any, GlobalConstants.DefaultCultureInfo, out var val);
            val += size;
            lines[0] = Math.Round(val, 4).ToString(GlobalConstants.DefaultCultureInfo);
            SetValue(lines[0]);
        }

        public override void LoadValue(List<string> newLines)
        {
            // Disallow edit if not a valid number for this input
            if (newLines.Any(line => !isValidText(line)))
            {
                // Revert edits
                linesStaging = [.. lines];
                return;
            }

            base.LoadValue(newLines);
        }

        bool isValidText(string text)
        {
            if (text == string.Empty) return true; // Allow an empty box, we'll set it to 0 if left empty when it loses focus
            if (text == "-") return true; // We want to allow typing the negative sign into an empty box, as well
            if (!IntMode && !double.TryParse(text, NumberStyles.Float, GlobalConstants.DefaultCultureInfo, out _)) return false;
            if (IntMode && !int.TryParse(text, NumberStyles.Integer, GlobalConstants.DefaultCultureInfo, out _)) return false;

            return true;
        }

        public override void OnFocusLost()
        {
            base.OnFocusLost();
            if (GetText() == string.Empty) SetValue(0);
        }

        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            base.OnMouseDownOnElement(api, args);

            rightSpacing = scaled(17);
            int mouseX = args.X;
            int mouseY = args.Y;

            float size = Interval;

            if (api.Input.KeyboardKeyState[(int)GlKeys.ShiftLeft]) size /= 10;
            if (api.Input.KeyboardKeyState[(int)GlKeys.ControlLeft]) size /= 100;

            if (mouseX >= Bounds.absX + Bounds.OuterWidth - rightSpacing && mouseX <= Bounds.absX + Bounds.OuterWidth && mouseY >= Bounds.absY && mouseY <= Bounds.absY + Bounds.OuterHeight)
            {
                if (DisableButtonFocus) focusable = false;

                double heightHalf = Bounds.OuterHeight / 2 - 1;

                if (mouseY > Bounds.absY + heightHalf + 1) UpdateValue(-size);
                else UpdateValue(size);

                api.Gui.PlaySound("tick");
            }
            else if (DisableButtonFocus) focusable = true;
        }

        public override void Dispose()
        {
            base.Dispose();
            buttonHighlightTexture.Dispose();
        }


    }


    public static partial class GuiComposerHelpers
    {
        /// <summary>
        /// Adds a numeric input for the current GUI.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="bounds">The bounds of the number input.</param>
        /// <param name="onTextChanged">The event fired when the number is changed.</param>
        /// <param name="font">The font for the numbers.</param>
        /// <param name="key">The name for this GuiElementNumberInput</param>
        public static GuiComposer AddNumberInput(this GuiComposer composer, ElementBounds bounds, Action<string> onTextChanged, CairoFont font = null, string key = null)
        {
            if (font == null)
            {
                font = CairoFont.TextInput();
            }

            if (!composer.Composed)
            {
                composer.AddInteractiveElement(new GuiElementNumberInput(composer.Api, bounds, onTextChanged, font), key);
            }

            return composer;
        }

        /// <summary>
        /// Gets the number input by name.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="key">The internal name of the numeric input.</param>
        /// <returns>The named numeric input.</returns>
        public static GuiElementNumberInput GetNumberInput(this GuiComposer composer, string key)
        {
            return (GuiElementNumberInput)composer.GetElement(key);
        }



    }

}
