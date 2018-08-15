using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using Cairo;
using Vintagestory.API.Common;
using Vintagestory.API.Client;
using System.Drawing;
using Vintagestory.API.Config;

namespace Vintagestory.API.Client
{    
    public abstract class GuiElement
    {
        internal static string dirtTextureName = "backgrounds/soil.png";
        internal static string noisyMetalTextureName = "backgrounds/noisymetal.png";
        //internal static string woodTextureName = "backgrounds/wood.png";
        internal static string woodTextureName = "backgrounds/oak.png";
        internal static string stoneTextureName = "backgrounds/stone.png";
        internal static string waterTextureName = "backgrounds/water.png";
        internal static string paperTextureName = "backgrounds/signpaper.png";

        internal static Dictionary<string, KeyValuePair<SurfacePattern, ImageSurface>> cachedPatterns = new Dictionary<string, KeyValuePair<SurfacePattern, ImageSurface>>();
        
        internal string lastShownText = "";
        internal ImageSurface metalNail;

        public ElementBounds Bounds;
        public int TabIndex;
        protected bool hasFocus;
        public bool InsideClipElement;

        protected ICoreClientAPI api;

        public bool HasFocus {
            get { return hasFocus; }
        }

        /// <summary>
        /// 0 = draw first, 1 = draw last. Only for interactive elements.
        /// </summary>
        public virtual double DrawOrder
        {
            get { return 0; }
        }
        
        public virtual bool Focusable
        {
            get { return false; }
        }

        public virtual double Scale
        {
            get; set;
        } = 1;

        public virtual void OnFocusGained()
        {
            hasFocus = true;
        }

        public virtual void OnFocusLost()
        {
            hasFocus = false;
        }

        public GuiElement(ICoreClientAPI capi, ElementBounds bounds)
        {
            this.api = capi;
            this.Bounds = bounds;
        }


        public virtual void ComposeElements(Context ctxStatic, ImageSurface surface)
        {
            
        }

        public virtual void RenderInteractiveElements(float deltaTime)
        {

        }

        public virtual void PostRenderInteractiveElements(float deltaTime)
        {

        }

        public void RenderFocusOverlay(float deltaTime)
        {
          //  presenter.DrawRectangle((int)bounds.renderX, (int)bounds.renderY, 800, (int)bounds.OuterWidth, (int)bounds.OuterHeight, 255 + (255 << 8) + (255 << 16) + (96 << 24));
        }




        protected void generateTexture(ImageSurface surface, ref int textureId, bool linearMag = true)
        {
            int prevTexId = textureId;

            textureId = api.Gui.LoadCairoTexture(surface, linearMag);

            if (prevTexId > 0) api.Render.GLDeleteTexture(prevTexId);
        }

        protected void generateTexture(ImageSurface surface, ref LoadedTexture intoTexture, bool linearMag = true)
        {
            api.Gui.LoadOrUpdateCairoTexture(surface, linearMag, ref intoTexture);
        }


        public static double scaled(double value)
        {
            return value * RuntimeEnv.GUIScale;
        }


        protected ImageSurface getMetalNail()
        {
            if (metalNail == null)
            {
                metalNail = new ImageSurface(Format.Argb32, (int)scaled(8), (int)scaled(8));
                Context ctx = new Context(metalNail);

                RoundRectangle(ctx, 0, 0, scaled(5), scaled(5), scaled(3));
                fillWithPattern(api, ctx, noisyMetalTextureName);
                
                ctx.Fill();
                EmbossRoundRectangle(ctx, 0, 0, scaled(5), scaled(5), scaled(3), 1, 0.7f, 2, 0.7f);
                ctx.Dispose();
            }

            return metalNail;
        }


        protected Context genContext(ImageSurface surface)
        {
            Context ctx = new Context(surface);
            ctx.SetSourceRGBA(0, 0, 0, 0);
            ctx.Paint();
            ctx.Antialias = Antialias.Best;
            return ctx;
        }

        public static SurfacePattern getPattern(Bitmap bitmap)
        {
            ImageSurface patternSurface = getImageSurfaceFromAsset(bitmap);

            SurfacePattern pattern = new SurfacePattern(patternSurface);
            pattern.Extend = Extend.Repeat;
            return pattern;
        }

        public static ImageSurface getImageSurfaceFromAsset(Bitmap bitmap)
        {
            BitmapData bmp_data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            ImageSurface imageSurface = new ImageSurface(Format.Argb32, bitmap.Width, bitmap.Height);

            unsafe
            {
                uint* destPixels = (uint*)imageSurface.DataPtr.ToPointer();
                uint* sourcePixels = (uint*)bmp_data.Scan0.ToPointer();

                int size = bitmap.Width * bitmap.Height;

                for (int i = 0; i < size; i++)
                {
                    destPixels[i] = sourcePixels[i];
                }
            }

            imageSurface.MarkDirty();
            bitmap.UnlockBits(bmp_data);
            return imageSurface;
        }


        public static ImageSurface getImageSurfaceFromAsset(Bitmap bitmap, int width, int height)
        {
            ImageSurface imageSurface = new ImageSurface(Format.Argb32, width, height);
            imageSurface.Image(bitmap, 0, 0, width, height);
            return imageSurface;
        }

        public static SurfacePattern getPattern(ICoreClientAPI capi, string texFileName, bool doCache = true)
        {
            if (cachedPatterns.ContainsKey(texFileName) && cachedPatterns[texFileName].Key.HandleValid)
            {
                return cachedPatterns[texFileName].Key;
            }

            ImageSurface patternSurface = getImageSurfaceFromAsset(capi, texFileName);

            SurfacePattern pattern = new SurfacePattern(patternSurface);
            pattern.Extend = Extend.Repeat;

            if (doCache) cachedPatterns[texFileName] = new KeyValuePair<SurfacePattern, ImageSurface>(pattern, patternSurface);

            return pattern;
        }

        public static ImageSurface getImageSurfaceFromAsset(ICoreClientAPI capi, string texFileName)
        {
            byte[] pngdata = capi.Assets.Get("textures/gui/" + texFileName).Data;

            BitmapExternal bitmap = (BitmapExternal)capi.Render.BitmapCreateFromPng(pngdata);

            BitmapData bmp_data = bitmap.bmp.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.bmp.Width, bitmap.bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            ImageSurface imageSurface = new ImageSurface(Format.Argb32, bitmap.bmp.Width, bitmap.bmp.Height);

            unsafe
            {
                uint* destPixels = (uint*)imageSurface.DataPtr.ToPointer();
                uint* sourcePixels = (uint*)bmp_data.Scan0.ToPointer();

                int size = bitmap.bmp.Width * bitmap.bmp.Height;

                for (int i = 0; i < size; i++)
                {
                    destPixels[i] = sourcePixels[i];
                }
            }

            imageSurface.MarkDirty();


            bitmap.bmp.UnlockBits(bmp_data);
            bitmap.Dispose();

            return imageSurface;
        }

        public static SurfacePattern fillWithPattern(ICoreClientAPI capi, Context ctx, string texFileName, bool preserve = false)
        {
            SurfacePattern pattern = getPattern(capi, texFileName);
            ctx.SetSource(pattern);
            if (preserve)
            {
                ctx.FillPreserve();
            } else
            {
                ctx.Fill();
            }
            return pattern;
        }

        public static void DiscardPattern(string texFilename)
        {
            if (cachedPatterns.ContainsKey(texFilename))
            {
                var val = cachedPatterns[texFilename];
                val.Key.Dispose();
                val.Value.Dispose();
                cachedPatterns.Remove(texFilename);
            }
        }


        internal SurfacePattern paintWithPattern(ICoreClientAPI capi, Context ctx, string texFileName)
        {
            SurfacePattern pattern = getPattern(capi, texFileName);
            ctx.SetSource(pattern);
            ctx.Paint();
            return pattern;
        }


        protected void Lamp(Context ctx, double x, double y, float[] color)
        {
            ctx.SetSourceRGBA(color[0], color[1], color[2], 1);
            RoundRectangle(ctx, x, y, scaled(10), scaled(10), ElementGeometrics.ElementBGRadius);
            ctx.Fill();
            EmbossRoundRectangleElement(ctx, x, y, scaled(10), scaled(10));
        }


        public static void Rectangle(Context ctx, ElementBounds bounds)
        {
            ctx.NewPath();
            ctx.LineTo(bounds.drawX, bounds.drawY);
            ctx.LineTo(bounds.drawX + bounds.OuterWidth, bounds.drawY);
            ctx.LineTo(bounds.drawX + bounds.OuterWidth, bounds.drawY + bounds.OuterHeight);
            ctx.LineTo(bounds.drawX, bounds.drawY + bounds.OuterHeight);
            ctx.ClosePath();
        }

        public static void Rectangle(Context ctx, double x, double y, double width, double height)
        {
            ctx.NewPath();
            ctx.LineTo(x, y);
            ctx.LineTo(x + width, y);
            ctx.LineTo(x + width, y + height);
            ctx.LineTo(x, y + height);
            ctx.ClosePath();
        }


        public void DialogRoundRectangle(Context ctx, ElementBounds bounds)
        {
            RoundRectangle(ctx, bounds.bgDrawX, bounds.bgDrawY, bounds.OuterWidth, bounds.OuterHeight, ElementGeometrics.DialogBGRadius);
        }


        public void ElementRoundRectangle(Context ctx, ElementBounds bounds, bool isBackground = false, double radius = -1)
        {
            if (radius == -1) radius = ElementGeometrics.ElementBGRadius;
            if (isBackground)
            {
                RoundRectangle(ctx, bounds.bgDrawX, bounds.bgDrawY, bounds.OuterWidth, bounds.OuterHeight, radius);
            } else
            {
                RoundRectangle(ctx, bounds.drawX, bounds.drawY, bounds.InnerWidth, bounds.InnerHeight, radius);
            }
            
        }

        public static void RoundRectangle(Context ctx, double x, double y, double width, double height, double radius)
        {
            double degrees = Math.PI / 180.0;

            ctx.Antialias = Antialias.Best;
            ctx.NewPath();
            ctx.Arc(x + width - radius, y + radius, radius, -90 * degrees, 0 * degrees);
            ctx.Arc(x + width - radius, y + height - radius, radius, 0 * degrees, 90 * degrees);
            ctx.Arc(x + radius, y + height - radius, radius, 90 * degrees, 180 * degrees);
            ctx.Arc(x + radius, y + radius, radius, 180 * degrees, 270 * degrees);
            ctx.ClosePath();
        }


        public void ShadePath(Context ctx, int thickness = 3)
        {
            ctx.Operator = Operator.Atop;

            ctx.SetSourceRGBA(0, 0, 0, 0.4);
            ctx.LineWidth = 2.0;
            ctx.Stroke();

            ctx.Operator = Operator.Over;
        }


        public void EmbossRoundRectangleDialog(Context ctx, double x, double y, double width, double height, bool inverse = false)
        {
            EmbossRoundRectangle(ctx, x, y, width, height, ElementGeometrics.DialogBGRadius, 4, 0.5f, 1.5f, 0.5f, inverse, 0.25f);
        }


        public void EmbossRoundRectangleElement(Context ctx, double x, double y, double width, double height, bool inverse = false, int depth = 2, int radius = -1)
        {
            EmbossRoundRectangle(ctx, x, y, width, height, radius == - 1 ? ElementGeometrics.ElementBGRadius : radius, depth, 0.7f, 2.0f, 0.5f, inverse, 0.25f);
        }

        public void EmbossRoundRectangleElement(Context ctx, ElementBounds bounds, bool inverse = false, int depth = 2, int radius = -1)
        {
            EmbossRoundRectangle(ctx, bounds.drawX, bounds.drawY, bounds.InnerWidth, bounds.InnerHeight, radius, depth, 0.5f, 1.2f, 0.8f, inverse, 0.25f);
        }
        

        protected void EmbossRoundRectangle(Context ctx, double x, double y, double width, double height, double radius, int thickness = 3, float intensity = 0.4f, float fallOff = 2, float lightDarkBalance = 1f, bool inverse = false, float alphaOffset = 0)
        {
            double degrees = Math.PI / 180.0;

            int i = thickness;
            int linewidth = 1;
            ctx.Antialias = Antialias.Best;

            int light = 255;
            int dark = 0;

            if (inverse)
            {
                light = 0;
                dark = 255;
                lightDarkBalance = 2 - lightDarkBalance;
            }

            while (i-- > 0)
            {
                x += 0.5f;
                y += 0.5f;
                width -= 1;
                height -= 1;

                // Light part
                ctx.NewPath();

                ctx.Arc(x + radius, y + height - radius, radius, 135 * degrees, 180 * degrees);
                ctx.Arc(x + radius, y + radius, radius, 180 * degrees, 270 * degrees);
                ctx.Arc(x + width - radius, y + radius, radius, -90 * degrees, -45 * degrees);
                
                double alpha = Math.Min(1, lightDarkBalance * intensity / Math.Pow(fallOff, linewidth - 1)) - alphaOffset;
                ctx.SetSourceRGBA(light, light, light, alpha);
                
                ctx.LineWidth = 1;
                ctx.Stroke();

                // Dark part
                ctx.NewPath();
                ctx.Arc(x + width - radius, y + radius, radius, -45 * degrees, 0 * degrees);
                ctx.Arc(x + width - radius, y + height - radius, radius, 0 * degrees, 90 * degrees);
                ctx.Arc(x + radius, y + height - radius, radius, 90 * degrees, 135 * degrees);

                alpha = Math.Min(1, (2 - lightDarkBalance) * intensity / Math.Pow(fallOff, linewidth - 1)) - alphaOffset;
                ctx.SetSourceRGBA(dark, dark, dark, alpha);
                ctx.LineWidth = 1;
                ctx.Stroke();


                linewidth++;
            }
        }





        public virtual void OnMouseDown(ICoreClientAPI api, MouseEvent mouse)
        {
            if (IsPositionInside(mouse.X, mouse.Y))
            {
                OnMouseDownOnElement(api, mouse);
                
            }
        }

        public virtual void OnMouseDownOnElement(ICoreClientAPI api, MouseEvent args)
        {
            args.Handled = true;
        }

        public virtual void OnMouseUpOnElement(ICoreClientAPI api, MouseEvent args) { }
        public virtual void OnMouseUp(ICoreClientAPI api, MouseEvent args)
        {
            if (IsPositionInside(args.X, args.Y))
            {
                OnMouseUpOnElement(api, args);
            }
        }

        public virtual void OnMouseMove(ICoreClientAPI api, MouseEvent args) { }
        public virtual void OnMouseWheel(ICoreClientAPI api, MouseWheelEventArgs args) { }

        public virtual void OnKeyDown(ICoreClientAPI api, KeyEvent args) { }
        public virtual void OnKeyPress(ICoreClientAPI api, KeyEvent args) { }

        public virtual bool IsPositionInside(int posX, int posY)
        {
            return 
                InsideClipElement ? Bounds.ParentBounds.PointInside(posX, posY) : Bounds.PointInside(posX, posY);
        }

        public virtual int OutlineColor()
        {
            return 255 + (255 << 8) + (255 << 16) + (128 << 24);
        }

        public virtual void Dispose()
        {

        }
    }
}
