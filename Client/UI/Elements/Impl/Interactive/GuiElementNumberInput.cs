using Cairo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace Vintagestory.API.Client
{
    public class GuiElementNumberInput : GuiElementTextInput
    {
        //public double Scale = 1;

        public LoadedTexture buttonHighlightTexture;
        
        public GuiElementNumberInput(ICoreClientAPI capi, ElementBounds bounds, API.Common.Action<string> OnTextChanged, CairoFont font) : base(capi, bounds, OnTextChanged, font)
        {
            buttonHighlightTexture = new LoadedTexture(capi);
        }

        public float GetValue()
        {
            float val;
            float.TryParse(GetText(), out val);
            return val;
        }

        public override void ComposeTextElements(Context ctx, ImageSurface surface)
        {
            rightSpacing = scaled(17);

            EmbossRoundRectangleElement(ctx, Bounds, true, 2, 3);
            ctx.SetSourceRGBA(0, 0, 0, 0.3);
            ElementRoundRectangle(ctx, Bounds, false, 3);
            ctx.Fill();

            GenTextHighlightTexture();
            GenButtonHighlightTexture();



            highlightBounds = Bounds.CopyOffsetedSibling().WithFixedPadding(0, 0).FixedGrow(2 * Bounds.absPaddingX, 2 * Bounds.absPaddingY);
            highlightBounds.CalcWorldBounds();

            RecomposeText(rightSpacing, 0);

            double heightHalf = Bounds.OuterHeight / 2 - 1;

            ctx.SetSourceRGBA(ElementGeometrics.DialogHighlightColor);
            RoundRectangle(ctx, Bounds.drawX + Bounds.InnerWidth - scaled(17 + 1) * Scale, Bounds.drawY, rightSpacing * Scale, heightHalf, 1);
            ctx.Fill();

            ctx.NewPath();
            ctx.LineTo(Bounds.drawX + Bounds.InnerWidth - scaled(9) * Scale, Bounds.drawY + scaled(4) * Scale);
            ctx.LineTo(Bounds.drawX + Bounds.InnerWidth - scaled(14) * Scale, Bounds.drawY + scaled(12) * Scale);
            ctx.LineTo(Bounds.drawX + Bounds.InnerWidth - scaled(4) * Scale, Bounds.drawY + scaled(12) * Scale);
            ctx.ClosePath();
            ctx.SetSourceRGBA(1, 1, 1, 0.4);
            ctx.Fill();


            ctx.SetSourceRGBA(ElementGeometrics.DialogHighlightColor);
            RoundRectangle(ctx, Bounds.drawX + Bounds.InnerWidth - (rightSpacing + scaled(1)) * Scale, Bounds.drawY + heightHalf + scaled(1) * Scale, rightSpacing * Scale, heightHalf, 1);
            ctx.Fill();

            ctx.NewPath();
            ctx.LineTo(Bounds.drawX + Bounds.InnerWidth - scaled(14) * Scale, Bounds.drawY + scaled(19) * Scale);
            ctx.LineTo(Bounds.drawX + Bounds.InnerWidth - scaled(4) * Scale, Bounds.drawY + scaled(19) * Scale);
            ctx.LineTo(Bounds.drawX + Bounds.InnerWidth - scaled(9) * Scale, Bounds.drawY + scaled(27) * Scale);
            ctx.ClosePath();
            ctx.SetSourceRGBA(1, 1, 1, 0.4);
            ctx.Fill();

            highlightBounds.fixedWidth -= rightSpacing;
            highlightBounds.CalcWorldBounds();
        }

        private void GenButtonHighlightTexture()
        {
            double heightHalf = Bounds.OuterHeight / 2 - 1;

            ImageSurface surfaceHighlight = new ImageSurface(Format.Argb32, (int)(rightSpacing), (int)heightHalf);
            Context ctxHighlight = genContext(surfaceHighlight);

            ctxHighlight.SetSourceRGBA(0, 0, 0, 0.2);
            ctxHighlight.Paint();

            generateTexture(surfaceHighlight, ref highlightTexture);

            ctxHighlight.Dispose();
            surfaceHighlight.Dispose();
        }

        private void GenTextHighlightTexture()
        {
            ImageSurface surfaceHighlight = new ImageSurface(Format.Argb32, (int)(Bounds.OuterWidth - rightSpacing), (int)Bounds.OuterHeight);
            Context ctxHighlight = genContext(surfaceHighlight);

            ctxHighlight.SetSourceRGBA(0, 0, 0, 0.2);
            ctxHighlight.Paint();

            generateTexture(surfaceHighlight, ref buttonHighlightTexture);

            ctxHighlight.Dispose();
            surfaceHighlight.Dispose();
        }

        public override void RenderInteractiveElements(float deltaTime)
        {
            base.RenderInteractiveElements(deltaTime);

            int mouseX = api.Input.MouseX;
            int mouseY = api.Input.MouseY;

            if (mouseX >= Bounds.absX + Bounds.OuterWidth - rightSpacing && mouseX <= Bounds.absX + Bounds.OuterWidth && mouseY >= Bounds.absY && mouseY <= Bounds.absY + Bounds.OuterHeight)
            {
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
            if (!IsPositionInside(api.Input.MouseX, api.Input.MouseY)) return;

            rightSpacing = scaled(17);

            float size = args.deltaPrecise > 0 ? 1 : -1;

            if (api.Input.KeyboardKeyStateRaw[(int)GlKeys.LShift]) size /= 10;
            if (api.Input.KeyboardKeyStateRaw[(int)GlKeys.ControlLeft]) size /= 100;

            double val;
            double.TryParse(lines[0], out val);
            val -= size;
            lines[0] = "" + Math.Round(val, 4);
            SetValue(lines[0]);

            args.SetHandled(true);
        }


        public override void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            base.OnMouseDownOnElement(api, args);

            rightSpacing = scaled(17);
            int mouseX = args.X;
            int mouseY = args.Y;

            float size = 1f;

            if (api.Input.KeyboardKeyStateRaw[(int)GlKeys.LShift]) size /= 10;
            if (api.Input.KeyboardKeyStateRaw[(int)GlKeys.ControlLeft]) size /= 100;

            if (mouseX >= Bounds.absX + Bounds.OuterWidth - rightSpacing && mouseX <= Bounds.absX + Bounds.OuterWidth && mouseY >= Bounds.absY && mouseY <= Bounds.absY + Bounds.OuterHeight)
            {
                double heightHalf = Bounds.OuterHeight / 2 - 1;

                if (mouseY > Bounds.absY + heightHalf + 1)
                {
                    double val;
                    double.TryParse(lines[0], out val);
                    val-=size;
                    lines[0] = "" + Math.Round(val, 4);
                    SetValue(lines[0]);
                }
                else
                {
                    double val;
                    double.TryParse(lines[0], out val);
                    val+=size;
                    lines[0] = "" + Math.Round(val, 4);
                    SetValue(lines[0]);
                }

                
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            buttonHighlightTexture.Dispose();
        }


    }


    public static partial class GuiComposerHelpers
    {
        public static GuiComposer AddNumberInput(this GuiComposer composer, ElementBounds bounds, API.Common.Action<string> OnTextChanged, CairoFont font = null, string key = null)
        {
            if (font == null)
            {
                font = CairoFont.TextInput();
            }

            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementNumberInput(composer.Api, bounds, OnTextChanged, font), key);
            }

            return composer;
        }

        public static GuiElementNumberInput GetNumberInput(this GuiComposer composer, string key)
        {
            return (GuiElementNumberInput)composer.GetElement(key);
        }



    }

}
