using System;
using Cairo;
using Vintagestory.API.MathTools;
using Vintagestory.API.Client;

namespace Vintagestory.API.Client
{
    public class GuiElementStatbar : GuiElementTextBase
    {
        float minValue = 0;
        float maxValue = 100;
        float value = 32;
        float lineInterval = 10;

        double[] color;
        bool rightToLeft = false;

        LoadedTexture valueTexture;
        LoadedTexture flashTexture;

        int valueWidth;
        int valueHeight;

        public bool shouldFlash;
        public float flashTime;

        public static double DefaultHeight = 7;

        public GuiElementStatbar(ICoreClientAPI capi, ElementBounds bounds, double[] color, bool rightToLeft) : base(capi, "", CairoFont.WhiteDetailText(), bounds)
        {
            valueTexture = new LoadedTexture(capi);
            flashTexture = new LoadedTexture(capi);

            this.color = color;
            this.rightToLeft = rightToLeft;
            value = new Random(Guid.NewGuid().GetHashCode()).Next(100);
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();

            ctx.Operator = Operator.Over; // WTF man, somehwere within this code or within cairo the main context operator is being changed

            RoundRectangle(ctx, Bounds.drawX, Bounds.drawY, Bounds.InnerWidth, Bounds.InnerHeight, 3);

            ctx.SetSourceRGB(0.15, 0.15, 0.15);
            ctx.Fill();
            EmbossRoundRectangleElement(ctx, Bounds, false, 3, 2);

            ComposeValueOverlay();
            ComposeFlashOverlay();
        }


        void ComposeValueOverlay()
        {
            double widthRel = (double)value / (maxValue - minValue);
            valueWidth = (int)(widthRel * Bounds.OuterWidth) + 1;
            valueHeight = (int)Bounds.OuterHeight + 1;
            ImageSurface surface = new ImageSurface(Format.Argb32, Bounds.OuterWidthInt+1, valueHeight);
            Context ctx = new Context(surface);

            if (widthRel > 0.01)
            {
                double width = Bounds.OuterWidth * widthRel;
                double x = rightToLeft ? Bounds.OuterWidth - width : 0;

                RoundRectangle(ctx, x, 0, width, Bounds.OuterHeight, 2);
                ctx.SetSourceRGB(color[0], color[1], color[2]);
                ctx.Fill();

                width = Bounds.InnerWidth * widthRel;
                x = rightToLeft ? Bounds.InnerWidth - width : 0;

                EmbossRoundRectangleElement(ctx, x, 0, width, Bounds.InnerHeight, false, 2, 2);
            }

            ctx.SetSourceRGBA(0, 0, 0, 0.5);
            ctx.LineWidth = scaled(1.8);


            int lines = (int)((maxValue - minValue) / lineInterval);
            
            for (int i = 1; i < lines; i++)
            {
                ctx.NewPath();
                ctx.SetSourceRGBA(0, 0, 0, 0.5);

                double x = (Bounds.InnerWidth * i) / lines;

                ctx.MoveTo(x, 0);
                ctx.LineTo(x, Math.Max(3, Bounds.InnerHeight - 1));
                ctx.ClosePath();
                ctx.Stroke();
            }


            generateTexture(surface, ref valueTexture);
            
            ctx.Dispose();
            surface.Dispose();
        }

        private void ComposeFlashOverlay()
        {
            double widthRel = (double)value / (maxValue - minValue);
            valueHeight = (int)Bounds.OuterHeight + 1;
            ImageSurface surface = new ImageSurface(Format.Argb32, Bounds.OuterWidthInt + 28, Bounds.OuterHeightInt + 28);
            Context ctx = new Context(surface);

            ctx.SetSourceRGBA(0,0,0,0);
            ctx.Paint();

            RoundRectangle(ctx, 12, 12, Bounds.OuterWidthInt + 4, Bounds.OuterHeightInt + 4, 2);
            ctx.SetSourceRGB(color[0], color[1], color[2]);
            ctx.Fill();
            surface.Blur(11);

            RoundRectangle(ctx, 15, 15, Bounds.OuterWidthInt - 2, Bounds.OuterHeightInt - 2, 2);
            ctx.Operator = Operator.Clear;
            ctx.SetSourceRGBA(0, 0, 0, 0);
            ctx.Fill();

            generateTexture(surface, ref flashTexture);

            ctx.Dispose();
            surface.Dispose();
        }


        public override void RenderInteractiveElements(float deltaTime)
        {
            double x = Bounds.renderX;
            double y = Bounds.renderY;

            float alpha = 0;
            if (shouldFlash)
            {
                flashTime += 6*deltaTime;
                alpha = GameMath.Sin(flashTime);
                if (alpha < 0)
                {
                    shouldFlash = false;
                    flashTime = 0;
                }
            }

            if (alpha > 0)
            {
                api.Render.RenderTexture(flashTexture.TextureId, x - 14, y - 14, Bounds.OuterWidthInt + 28, Bounds.OuterHeightInt + 28, 50, new Vec4f(1.5f, 1, 1, alpha));
            }


            api.Render.RenderTexture(valueTexture.TextureId, x, y, Bounds.OuterWidthInt + 1, valueHeight);
        }

        public void SetLineInterval(float value)
        {
            lineInterval = value;
        }

        public void SetValue(float value)
        {
            this.value = value;
            ComposeValueOverlay();
            ComposeFlashOverlay();
        }

        public void SetValues(float value, float min, float max)
        {
            this.value = value;
            minValue = min;
            maxValue = max;
            ComposeValueOverlay();
            ComposeFlashOverlay();
        }

        public void SetMinMax(float min, float max)
        {
            minValue = min;
            maxValue = max;
            ComposeValueOverlay();
            ComposeFlashOverlay();
        }
        
        public override void Dispose()
        {
            base.Dispose();
            valueTexture.Dispose();
            flashTexture.Dispose();
        }
    }

    public static partial class GuiComposerHelpers
    {

        public static GuiComposer AddStatbar(this GuiComposer composer, ElementBounds bounds, double[] color, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementStatbar(composer.Api, bounds, color, false), key);
            }
            return composer;
        }

        public static GuiComposer AddInvStatbar(this GuiComposer composer, ElementBounds bounds, double[] color, string key = null)
        {
            if (!composer.composed)
            {
                composer.AddInteractiveElement(new GuiElementStatbar(composer.Api, bounds, color, true), key);
            }
            return composer;
        }


        public static GuiElementStatbar GetStatbar(this GuiComposer composer, string key)
        {
            return (GuiElementStatbar)composer.GetElement(key);
        }
    }
}
