using System;
using Cairo;
using Vintagestory.API.MathTools;
using Vintagestory.API.Common;

#nullable disable

namespace Vintagestory.API.Client
{
    public delegate string StatbarValueDelegate();

    /// <summary>
    /// A stat bar to the GUI for keeping track of progress and numbers.
    /// </summary>
    public class GuiElementStatbar : GuiElementTextBase
    {
        float minValue = 0;
        float maxValue = 100;
        float value = 32;
        float? futureValue;
        float? previousValue;
        float valueChangeVelocity;
        float lineInterval = 10;
        float prevValueDisplayRemainingSec;
        float prevValueSize = 0f;

        float? nowRenderingPreviousValue;

        double[] color;
        bool rightToLeft = false;

        public bool HideWhenFull { get; set; }

        public bool PrevValueBeingDisplayed => prevValueDisplayRemainingSec > 0;

        public float PreviousValueDisplayTime { get; set; } = 2;

        LoadedTexture baseTexture;
        LoadedTexture barTexture;
        LoadedTexture flashTexture;
        LoadedTexture valueTexture;
        LoadedTexture previousValueTexture;

        int valueHeight;

        public bool ShouldFlash;
        public float FlashTime;
        public bool ShowValueOnHover=true;
        bool valuesSet;
        bool hideable;

        public StatbarValueDelegate onGetStatbarValue;
        public CairoFont valueFont = CairoFont.WhiteSmallText().WithStroke(ColorUtil.BlackArgbDouble, 0.75);

        public static double DefaultHeight = 8;

        /// <summary>
        /// Creates a new stat bar for the GUI.
        /// </summary>
        /// <param name="capi">The client API</param>
        /// <param name="bounds">The bounds of the stat bar.</param>
        /// <param name="color">The color of the stat bar.</param>
        /// <param name="rightToLeft">Determines the direction that the bar fills.</param>
        /// <param name="hideable"></param>
        public GuiElementStatbar(ICoreClientAPI capi, ElementBounds bounds, double[] color, bool rightToLeft, bool hideable) : base(capi, "", CairoFont.WhiteDetailText(), bounds)
        {
            barTexture = new LoadedTexture(capi);
            flashTexture = new LoadedTexture(capi);
            valueTexture = new LoadedTexture(capi);
            previousValueTexture = new LoadedTexture(capi);

            if (hideable) baseTexture = new LoadedTexture(capi);

            this.hideable = hideable;
            this.color = color;
            this.rightToLeft = rightToLeft;

            onGetStatbarValue = () => { return (float)Math.Round((float)value, 1) + " / " + (int)this.maxValue; };
        }

        public override void ComposeElements(Context ctx, ImageSurface surface)
        {
            Bounds.CalcWorldBounds();

            if (hideable)
            {
                surface = new ImageSurface(Format.Argb32, Bounds.OuterWidthInt+1, Bounds.OuterHeightInt+1);
                ctx = new Context(surface);

                RoundRectangle(ctx, 0, 0, Bounds.InnerWidth, Bounds.InnerHeight, 1);

                ctx.SetSourceRGBA(0.15, 0.15, 0.15, 1);
                ctx.Fill();
                EmbossRoundRectangleElement(ctx, 0, 0, Bounds.InnerWidth, Bounds.InnerHeight, false, 3, 1);
            } else
            {
                ctx.Operator = Operator.Over; // WTF man, somewhere within this code or within cairo the main context operator is being changed
                RoundRectangle(ctx, Bounds.drawX, Bounds.drawY, Bounds.InnerWidth, Bounds.InnerHeight, 1);

                ctx.SetSourceRGBA(0.15, 0.15, 0.15, 1);
                ctx.Fill();
                EmbossRoundRectangleElement(ctx, Bounds, false, 3, 1);
            }


            if (valuesSet)
            {
                recomposeOverlays();
            }

            if (hideable)
            {
                generateTexture(surface, ref baseTexture);
                surface.Dispose();
                ctx.Dispose();
            }
        }

        void recomposeOverlays()
        {
            TyronThreadPool.QueueTask(() =>
            {
                ComposeValueOverlay();
                ComposeFlashOverlay();
            });

            if (ShowValueOnHover)
            {
                api.Gui.TextTexture.GenOrUpdateTextTexture(onGetStatbarValue(), valueFont, ref valueTexture, new TextBackground()
                {
                    FillColor = GuiStyle.DialogStrongBgColor,
                    Padding = 5,
                    BorderWidth = 2
                });
            }
        }

        
        void ComposeValueOverlay()
        {
            Bounds.CalcWorldBounds();

            double widthRel = (double)value / (maxValue - minValue);

            valueHeight = (int)Bounds.OuterHeight + 1;
            ImageSurface surface = new ImageSurface(Format.Argb32, Bounds.OuterWidthInt+1, valueHeight);
            Context ctx = new Context(surface);

            double mainValueX = 0;
            
            if (widthRel > 0.01)
            {
                double width = Bounds.OuterWidth * widthRel;
                double x = rightToLeft ? Bounds.OuterWidth - width : 0;

                mainValueX = rightToLeft ? Bounds.OuterWidth - width : width;

                RoundRectangle(ctx, x, 0, width, Bounds.OuterHeight, 1);
                ctx.SetSourceRGB(color[0], color[1], color[2]);
                ctx.FillPreserve();

                ctx.SetSourceRGB(color[0] * 0.4, color[1] * 0.4, color[2] * 0.4);
                ctx.LineWidth = scaled(2);
                ctx.StrokePreserve();
                surface.BlurFull(2);

                width = Bounds.InnerWidth * widthRel;
                x = rightToLeft ? Bounds.InnerWidth - width : 0;
    
                EmbossRoundRectangleElement(ctx, x, 0, width, Bounds.InnerHeight, false, 2, 1);
            }

            ImageSurface surfacePrev = null;
            Context ctxPrev = null;
            if (previousValue != null)
            {
                float pval = (float)previousValue;

                if (pval > value && (getMs() - visibleSinceMs < PreviousValueDisplayTime * 1000)) // Might be no longer relevant to display?
                {
                    double widthRelPrev = pval / (maxValue - minValue);

                    if (widthRelPrev > 0.01)
                    {
                        surfacePrev = new ImageSurface(Format.Argb32, Bounds.OuterWidthInt + 1, valueHeight);
                        ctxPrev = new Context(surfacePrev);

                        double width = Bounds.OuterWidth * widthRelPrev;
                        double x = rightToLeft ? Bounds.OuterWidth - width : 0;

                        RoundRectangle(ctxPrev, x, 0, width, Bounds.OuterHeight, 1);
                        ctxPrev.SetSourceRGB(color[0], color[1], color[2]);
                        ctxPrev.FillPreserve();

                        ctxPrev.SetSourceRGB(color[0] * 0.4, color[1] * 0.4, color[2] * 0.4);
                        ctxPrev.LineWidth = scaled(2);
                        ctxPrev.StrokePreserve();
                        surfacePrev.BlurFull(2);

                        width = Bounds.InnerWidth * widthRelPrev;
                        x = rightToLeft ? Bounds.InnerWidth - width : 0;

                        EmbossRoundRectangleElement(ctxPrev, x, 0, width, Bounds.InnerHeight, false, 2, 1);
                    }
                }
            }



            if (futureValue != null)
            {
                float gval = (float)futureValue;
                double widthRelFt = (gval - value) / (maxValue - minValue);
                if (widthRelFt > 0.01)
                {
                    double x1 = mainValueX;
                    double width = Bounds.OuterWidth * widthRelFt;

                    if (rightToLeft)
                    {
                        x1 = mainValueX - width;
                    }

                    RoundRectangle(ctx, x1, 0, width, Bounds.OuterHeight, 1);
                    ctx.SetSourceRGBA(0, 1, 0, 0.35);// color[0], color[1], color[2]);
                    ctx.FillPreserve();

                    ctx.SetSourceRGBA(0, 0.5, 0, 0.35);// color[0] * 0.4, color[1] * 0.4, color[2] * 0.4);
                    ctx.LineWidth = scaled(2);
                    ctx.StrokePreserve();
                    surface.BlurFull(2);

                    width = Bounds.InnerWidth * widthRel;
                    x1 = rightToLeft ? Bounds.InnerWidth - width : 0;

                    EmbossRoundRectangleElement(ctx, x1, 0, width, Bounds.InnerHeight, false, 2, 1);
                }
            }


            ctx.SetSourceRGBA(0, 0, 0, 0.5);
            ctx.LineWidth = scaled(2.2);

            int lines = Math.Min(50, (int)((maxValue - minValue) / lineInterval));
            
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

            api.Event.EnqueueMainThreadTask(() =>
            {
                generateTexture(surface, ref barTexture);
                if (surfacePrev != null)
                {
                    generateTexture(surfacePrev, ref previousValueTexture);
                    surfacePrev.Dispose();
                    ctxPrev.Dispose();
                    prevValueSize = 1;
                }
                ctx.Dispose();
                surface.Dispose();
            }, "recompstatbar");
        }

        void ComposeFlashOverlay()
        {
            valueHeight = (int)Bounds.OuterHeight + 1;

            ImageSurface surface = new ImageSurface(Format.Argb32, Bounds.OuterWidthInt + 28, Bounds.OuterHeightInt + 28);
            Context ctx = new Context(surface);

            ctx.SetSourceRGBA(0,0,0,0);
            ctx.Paint();

            RoundRectangle(ctx, 12, 12, Bounds.OuterWidthInt + 4, Bounds.OuterHeightInt + 4, 1);
            ctx.SetSourceRGB(color[0], color[1], color[2]);
            ctx.FillPreserve();
            surface.BlurFull(3);
            ctx.Fill();
            surface.BlurFull(2);

            RoundRectangle(ctx, 15, 15, Bounds.OuterWidthInt - 2, Bounds.OuterHeightInt - 2, 1);
            ctx.Operator = Operator.Clear;
            ctx.SetSourceRGBA(0, 0, 0, 0);
            ctx.Fill();

            api.Event.EnqueueMainThreadTask(() =>
            {
                generateTexture(surface, ref flashTexture);
                ctx.Dispose();
                surface.Dispose();
            }, "recompstatbar");
        }


        public override void RenderInteractiveElements(float deltaTime)
        {
            double x = Bounds.renderX;
            double y = Bounds.renderY;

            if (value == maxValue && HideWhenFull) return;

            if (hideable)
            {
                api.Render.RenderTexture(baseTexture.TextureId, x, y, Bounds.OuterWidthInt + 1, Bounds.OuterHeightInt + 1);
            }

            float alpha = 0;
            if (ShouldFlash)
            {
                FlashTime += 6*deltaTime;
                alpha = GameMath.Sin(FlashTime);
                if (alpha < 0)
                {
                    ShouldFlash = false;
                    FlashTime = 0;
                }
                if (FlashTime < GameMath.PIHALF)
                {
                    alpha = Math.Min(1, alpha * 3);
                }
            }

            if (alpha > 0)
            {
                api.Render.RenderTexture(flashTexture.TextureId, x - 14, y - 14, Bounds.OuterWidthInt + 28, Bounds.OuterHeightInt + 28, 50, new Vec4f(1.5f, 1, 1, alpha));
            }

            if (previousValue != null && previousValueTexture.TextureId > 0 && prevValueSize > 0.01)
            {
                if (getMs() - visibleSinceMs > PreviousValueDisplayTime*1000)
                {
                    prevValueSize = Math.Max(0, prevValueSize - deltaTime);
                }
                api.Render.RenderTexture(previousValueTexture.TextureId, x, y, Bounds.OuterWidthInt * prevValueSize, valueHeight, 50, new Vec4f(1,1,1,0.35f));
            }

            if (barTexture.TextureId > 0)
            {
                api.Render.RenderTexture(barTexture.TextureId, x, y, Bounds.OuterWidthInt + 1, valueHeight);
            }

            if (ShowValueOnHover && Bounds.PointInside(api.Input.MouseX, api.Input.MouseY))
            {
                double tx = api.Input.MouseX + 16;
                double ty = api.Input.MouseY + valueTexture.Height - 4;
                api.Render.RenderTexture(valueTexture.TextureId, tx, ty, valueTexture.Width, valueTexture.Height, 2000);
            }
        }

        /// <summary>
        /// Sets the line interval for the Status Bar.
        /// </summary>
        /// <param name="value">The value to set for the line interval/</param>
        public void SetLineInterval(float value)
        {
            lineInterval = value;
        }

        /// <summary>
        /// Sets the value for the status bar and updates the bar.
        /// </summary>
        /// <param name="value">The new value of the status bar.</param>
        public void SetValue(float value)
        {
            this.value = value;
            valuesSet = true;
            recomposeOverlays();
        }

        public float GetValue()
        {
            return this.value;
        }

        /// <summary>
        /// Sets the value for the status bar as well as the minimum and maximum values.
        /// </summary>
        /// <param name="value">The new value of the status bar.</param>
        /// <param name="min">The minimum value of the status bar.</param>
        /// <param name="max">The maximum value of the status bar.</param>
        public void SetValues(float value, float min, float max)
        {
            valuesSet = true;
            this.value = value;
            minValue = min;
            maxValue = max;
            recomposeOverlays();
        }

        /// <summary>
        /// Sets the minimum and maximum values of the status bar.
        /// </summary>
        /// <param name="min">The minimum value of the status bar.</param>
        /// <param name="max">The maximum value of the status bar.</param>
        public void SetMinMax(float min, float max)
        {
            minValue = min;
            maxValue = max;
            recomposeOverlays();
        }
        
        public override void Dispose()
        {
            base.Dispose();
            baseTexture?.Dispose();
            barTexture.Dispose();
            previousValueTexture.Dispose();
            flashTexture.Dispose();
            valueTexture.Dispose();
        }

        public void SetFutureValues(float? futureValue, float velocity)
        {
            this.futureValue = futureValue;
            this.valueChangeVelocity = velocity;
        }


        long visibleSinceMs;
        Func<long> getMs;
        public void SetPrevValue(float? previousValue, long visibleSinceMs, Func<long> getMs)
        {
            this.previousValue = previousValue;
            this.visibleSinceMs = visibleSinceMs;
            this.getMs = getMs;
        }
    }

    public static partial class GuiComposerHelpers
    {
        /// <summary>
        /// Adds a stat bar to the current GUI with a minimum of 0 and a maximum of 100.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="bounds">The bounds of the stat bar.</param>
        /// <param name="color">The color of the stat bar.</param>
        /// <param name="hideable">If true, the element can be fully hidden without recompose.</param>
        /// <param name="key">The internal name of the stat bar.</param>
        public static GuiComposer AddStatbar(this GuiComposer composer, ElementBounds bounds, double[] color, bool hideable, string key = null)
        {
            if (!composer.Composed)
            {
                composer.AddInteractiveElement(new GuiElementStatbar(composer.Api, bounds, color, false, hideable), key);
            }
            return composer;
        }

        public static GuiComposer AddStatbar(this GuiComposer composer, ElementBounds bounds, double[] color, string key = null)
        {
            if (!composer.Composed)
            {
                composer.AddInteractiveElement(new GuiElementStatbar(composer.Api, bounds, color, false, false), key);
            }
            return composer;
        }

        /// <summary>
        /// Adds a stat bar with filling in the opposite direction. Default values are from 0 to 100.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="bounds">the bounds of the stat bar.</param>
        /// <param name="color">the color of the stat bar.</param>
        /// <param name="key">The internal name of the stat bar.</param>
        public static GuiComposer AddInvStatbar(this GuiComposer composer, ElementBounds bounds, double[] color, string key = null)
        {
            if (!composer.Composed)
            {
                composer.AddInteractiveElement(new GuiElementStatbar(composer.Api, bounds, color, true, false), key);
            }
            return composer;
        }

        /// <summary>
        /// Gets the stat bar by name.
        /// </summary>
        /// <param name="composer"></param>
        /// <param name="key">The internal name of the stat bar to fetch.</param>
        /// <returns>The named stat bar.</returns>
        public static GuiElementStatbar GetStatbar(this GuiComposer composer, string key)
        {
            return (GuiElementStatbar)composer.GetElement(key);
        }
    }
}
