using System;
using System.Linq;
using System.Threading;
using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.Config;

namespace Vintagestory.API.Client
{
    public class CairoFont : FontConfig, ICairoFont, IDisposable
    {
        static ImageSurface surface;
        public static Context FontMeasuringContext;
        
        public bool RenderTwice;

        FontOptions CairoFontOptions;

        static CairoFont()
        {
            surface = new ImageSurface(Format.Argb32, 1, 1);
            FontMeasuringContext = new Context(surface);
        }

        public CairoFont()
        {
        }

        public CairoFont(FontConfig config)
        {
            UnscaledFontsize = config.UnscaledFontsize;
            Fontname = config.Fontname;
            FontWeight = config.FontWeight;
            Color = config.Color;
            StrokeColor = config.StrokeColor;
            StrokeWidth = config.StrokeWidth;
        }   


        public CairoFont(double unscaledFontSize, string fontName)
        {
            this.UnscaledFontsize = unscaledFontSize;
            this.Fontname = fontName;
        }


        public CairoFont(double unscaledFontSize, string fontName, double[] color, double[] strokeColor = null)
        {
            this.UnscaledFontsize = unscaledFontSize;
            this.Fontname = fontName;
            this.Color = color;
            this.StrokeColor = strokeColor;
            if (StrokeColor != null) StrokeWidth = 1;
        }

        /// <summary>
        /// Adjust font size so that it fits given bounds
        /// </summary>
        /// <param name="text"></param>
        /// <param name="bounds"></param>
        public void AutoFontSize(string text, ElementBounds bounds)
        {
            UnscaledFontsize = 50;
            UnscaledFontsize *= bounds.InnerWidth / GetTextExtents(text).Width;
        }



        /// <summary>
        /// Adjust the bounds so that it fits given text in one line
        /// </summary>
        /// <param name="text"></param>
        /// <param name="bounds"></param>
        /// <param name="onlyGrow">If true, the box will not be made smaller</param>
        public void AutoBoxSize(string text, ElementBounds bounds, bool onlyGrow = false)
        {
            double textWidth=0;
            double textHeight=0;

            FontExtents fontExtents = GetFontExtents();

            if (text.Contains('\n'))
            {
                string[] lines = text.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Length == 0 && i == lines.Length - 1) break;

                    TextExtents extents = GetTextExtents(lines[i]);
                    
                    textWidth = Math.Max(textWidth, extents.Width);
                    textHeight += fontExtents.Height;
                }
            } else
            {
                TextExtents extents = GetTextExtents(text);
                textWidth = extents.Width;
                textHeight = fontExtents.Height;
            }

            if (text.Length == 0)
            {
                textWidth = 0;
                textHeight = 0;
            }



            if (onlyGrow)
            {
                // Divide by guiscale because calcWorldBounds will multiply it again with guiscale
                bounds.fixedWidth = Math.Max(bounds.fixedWidth, textWidth / RuntimeEnv.GUIScale + 1);
                bounds.fixedHeight = Math.Max(bounds.fixedHeight, textHeight / RuntimeEnv.GUIScale);
            }
            else
            {
                // Divide by guiscale because calcWorldBounds will multiply it again with guiscale
                bounds.fixedWidth = Math.Max(1, textWidth / RuntimeEnv.GUIScale + 1);
                bounds.fixedHeight = Math.Max(1, textHeight / RuntimeEnv.GUIScale);
            }
        }


        public CairoFont WithColor(double[] color)
        {
            this.Color = color;
            return this;
        }

        public CairoFont WithWeight(FontWeight weight)
        {
            this.FontWeight = weight;
            return this;
        }

        public CairoFont WithRenderTwice()
        {
            this.RenderTwice = true;
            return this;
        }

        public void SetupContext(Context ctx)
        {
            if (Thread.CurrentThread.ManagedThreadId != RuntimeEnv.MainThreadId)
            {
                throw new Exception("Programming Error. SetupContext not called from main thread. Not a thread safe method");
            }

            ctx.SetFontSize(GuiElement.scaled(UnscaledFontsize));
            ctx.SelectFontFace(Fontname, FontSlant.Normal, FontWeight);
            CairoFontOptions = new FontOptions();
            CairoFontOptions.Antialias = Antialias.Gray;
            ctx.FontOptions = CairoFontOptions;

            if (Color != null)
            {
                if (Color.Length == 3)
                {
                    ctx.SetSourceRGB(Color[0], Color[1], Color[2]);
                }
                if (Color.Length == 4)
                {
                    ctx.SetSourceRGBA(Color[0], Color[1], Color[2], Color[3]);
                }
            }
        }

        public FontExtents GetFontExtents()
        {
            SetupContext(FontMeasuringContext);
            return FontMeasuringContext.FontExtents;
        }

        public TextExtents GetTextExtents(string text)
        {
            SetupContext(FontMeasuringContext);
            return FontMeasuringContext.TextExtents(text);
        }

        public CairoFont Clone()
        {
            CairoFont font = (CairoFont)MemberwiseClone();
            Array.Copy(Color, font.Color, Color.Length);
            return font;
        }


        #region Presets

       
        public CairoFont WithFontSize(float fontSize)
        {
            UnscaledFontsize = fontSize;
            return this;
        }

        public static CairoFont ButtonText()
        {
            return new CairoFont()
            {
                Color = ElementGeometrics.LightBrownTextColor,
                FontWeight = FontWeight.Bold,
                Fontname = ElementGeometrics.decorativeFontName,
                UnscaledFontsize = 24
            };
        }

        public static CairoFont ButtonPressedText()
        {
            return new CairoFont()
            {
                Color = ElementGeometrics.LightBrownHoverTextColor,
                FontWeight = FontWeight.Bold,
                Fontname = ElementGeometrics.decorativeFontName,
                UnscaledFontsize = 24
            };
        }

        public static CairoFont TextInput()
        {
            return new CairoFont()
            {
                Color = new double[] { 1, 1, 1, 0.9 },
                Fontname = ElementGeometrics.standardFontName,
                UnscaledFontsize = 18
            };
        }

        public static CairoFont SmallTextInput()
        {
            return new CairoFont()
            {
                Color = new double[] { 0, 0, 0, 0.9 },
                Fontname = ElementGeometrics.standardFontName,
                UnscaledFontsize = ElementGeometrics.SmallFontSize
            };
        }

        public static CairoFont SmallDialogText()
        {
            return new CairoFont()
            {
                Color = new double[] { 234 / 255.0, 220 / 255.0, 206 / 255.0, 1 },
                Fontname = ElementGeometrics.decorativeFontName,
                UnscaledFontsize = ElementGeometrics.SmallFontSize,
                FontWeight = FontWeight.Bold
            };
        }

        public static CairoFont MediumDialogText()
        {
            return new CairoFont()
            {
                Color = new double[] { 234 / 255.0, 220 / 255.0, 206 / 255.0, 1 },
                Fontname = ElementGeometrics.decorativeFontName,
                UnscaledFontsize = ElementGeometrics.SmallishFontSize,
                FontWeight = FontWeight.Bold
            };
        }


        public static CairoFont WhiteMediumText()
        {
            return new CairoFont()
            {
                Color = ElementGeometrics.DialogDefaultTextColor,
                Fontname = ElementGeometrics.standardFontName,
                UnscaledFontsize = ElementGeometrics.NormalFontSize
            };
        }


        public static CairoFont WhiteSmallishText()
        {
            return new CairoFont()
            {
                Color = ElementGeometrics.DialogDefaultTextColor,
                Fontname = ElementGeometrics.standardFontName,
                UnscaledFontsize = ElementGeometrics.SmallishFontSize
            };
        }


        public static CairoFont WhiteSmallText()
        {
            return new CairoFont()
            {
                Color = ElementGeometrics.DialogDefaultTextColor,
                Fontname = ElementGeometrics.standardFontName,
                UnscaledFontsize = ElementGeometrics.SmallFontSize
            };

        }


        public static CairoFont WhiteDetailText()
        {
            return new CairoFont()
            {
                Color = ElementGeometrics.DialogDefaultTextColor,
                Fontname = ElementGeometrics.standardSemiBoldFontName,
                UnscaledFontsize = ElementGeometrics.DetailFontSize
            };

        }

        public void Dispose()
        {
            CairoFontOptions.Dispose();
        }
        
        #endregion

    }
}
