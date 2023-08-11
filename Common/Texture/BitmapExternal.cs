using Cairo;
using SkiaSharp;
using System;
using System.IO;
using Vintagestory.API.Client;
using Vintagestory.API.Util;


namespace Vintagestory.API.Common
{
    public static class SurfaceDrawImage
    {
        public static void Image(this ImageSurface surface, BitmapRef bmp, int xPos, int yPos, int width, int height)
        {
            surface.Image(((BitmapExternal)bmp).bmp, xPos, yPos, width, height);
        }
    }

    public class BitmapExternal : BitmapRef
    {
        public SKBitmap bmp;

        public override int Height => bmp.Height;

        public override int Width => bmp.Width;

        public override int[] Pixels => Array.ConvertAll(bmp.Pixels, p => (int)(uint)p);

        public IntPtr PixelsPtrAndLock => bmp.GetPixels();

        public BitmapExternal()
        {
        }

        public BitmapExternal(int width, int height)
        {
            bmp = new SKBitmap(width, height);
        }

        public BitmapExternal(MemoryStream ms, ILogger logger, AssetLocation loc = null)
        {
            try
            {
                using (ms)
                {
                    bmp = SKBitmap.Decode(ms);
                }

            }
            catch (Exception e)
            {
                if (loc != null)
                {
                    logger.Error("Failed loading bitmap from png file {0}: {1}. Will default to an empty 1x1 bitmap.", loc, e);
                }
                else logger.Error("Failed loading bitmap, error was: {0}. Will default to an empty 1x1 bitmap.", e);
                bmp = new SKBitmap(1, 1);
                bmp.SetPixel(0, 0, SKColors.Orange);
            }
        }

        ///// <summary>
        ///// Create a BitmapExternal from a path to an existing image file.  Calling code should check that the file exists
        ///// </summary>
        ///// <param name="filePath"></param>
        public BitmapExternal(string filePath)
        {
            bmp = SKBitmap.Decode(filePath);
        }

        ///// <summary>
        ///// Create a BitmapExternal from a stream.  Calling code should close the stream
        ///// </summary>
        ///// <param name="stream"></param>
        public BitmapExternal(Stream stream)
        {
            bmp = SKBitmap.Decode(stream);
        }

        /// <summary>
        /// Create a BitmapExternal from a byte array
        /// </summary>
        /// <param name="data"></param>
        /// <param name="dataLength"></param>
        /// <param name="logger"></param>
        public BitmapExternal(byte[] data, int dataLength, ILogger logger)
        {
            try
            {
                var decode = SKBitmap.Decode(new ReadOnlySpan<byte>(data, 0, dataLength));
                bmp = new SKBitmap(decode.Width, decode.Height, SKColorType.Bgra8888, SKAlphaType.Unpremul);
                using (var canvas = new SKCanvas(bmp))
                {
                    canvas.DrawBitmap(decode, 0, 0);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed loading bitmap from data: {0}. Will default to an empty 1x1 bitmap.", ex);
                bmp = new SKBitmap(1, 1);
                bmp.SetPixel(0, 0, SKColors.Orange);
            }
        }

        public override void Dispose()
        {
            bmp.Dispose();
        }

        public override void Save(string filename)
        {
            bmp.Save(filename);
        }

        /// <summary>
        /// Retrives the ARGB value from given coordinate
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override SKColor GetPixel(int x, int y)
        {
            return bmp.GetPixel(x, y);
        }

        /// <summary>
        /// Retrives the ARGB value from given coordinate using normalized coordinates (0..1)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override SKColor GetPixelRel(float x, float y)
        {
            return bmp.GetPixel((int)Math.Min(bmp.Width - 1, x * bmp.Width), (int)Math.Min(bmp.Height - 1, (y * bmp.Height)));
        }

        public override unsafe void MulAlpha(int alpha = 255)
        {
            var len = Width * Height;
            var af = alpha / 255f;
            var colp = (byte*)bmp.GetPixels().ToPointer();
            for (var i = 0; i < len; i++)
            {
                int a = colp[3];
                colp[0] = (byte)(colp[0] * af);
                colp[1] = (byte)(colp[1] * af);
                colp[2] = (byte)(colp[2] * af);

                colp[3] = (byte)(a * af);
                colp += 4;
            }
        }

        public override int[] GetPixelsTransformed(int rot = 0, int mulAlpha = 255)
        {
            int[] bmpPixels = new int[Width * Height];
            int width = bmp.Width;
            int height = bmp.Height;
            FastBitmap fastBitmap = new FastBitmap();
            fastBitmap.bmp = bmp;
            int stride = fastBitmap.Stride;
            switch (rot)
            {
                case 0:
                {
                    for (int y = 0; y < height; y++)
                    {
                        fastBitmap.GetPixelRow(width, y * stride, bmpPixels, y * width);
                    }

                    break;
                }
                case 90:
                {
                    for (int x = 0; x < width; x++)
                    {
                        int baseY = x * width;
                        for (int y = 0; y < height; y++)
                        {
                            bmpPixels[y + baseY] = fastBitmap.GetPixel(width - x - 1, y * stride);
                        }
                    }

                    break;
                }
                case 180:
                {
                    for (int y = 0; y < height; y++)
                    {
                        int baseX = y * width;
                        int yStride = (height - y - 1) * stride;
                        for (int x = 0; x < width; x++)
                        {
                            bmpPixels[x + baseX] = fastBitmap.GetPixel(width - x - 1, yStride);
                        }
                    }

                    break;
                }
                case 270:
                {
                    for (int x = 0; x < width; x++)
                    {
                        int baseY = x * width;
                        for (int y = 0; y < height; y++)
                        {
                            bmpPixels[y + baseY] = fastBitmap.GetPixel(x, (height - y - 1) * stride);
                        }
                    }

                    break;
                }
            }

            if (mulAlpha != 255)
            {
                var alpaP = mulAlpha / 100f;
                var white = (int)(uint)SKColors.White;
                for (int i = 0; i < bmpPixels.Length; i++)
                {
                    var current = bmpPixels[i];
                    var currAlpha = (current >> 24);
                    current &= white;
                    bmpPixels[i] = (current | ((int)(currAlpha * alpaP) << 24));
                }
            }

            return bmpPixels;
        }
    }
}
