using Cairo;
using System;
using System.Drawing;
using System.Drawing.Imaging;
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
        public Bitmap bmp;
        BitmapData bmp_data;

        public BitmapExternal()
        {
        }

        public BitmapExternal(int width, int height)
        {
            bmp = new Bitmap(width, height);
        }

        public BitmapExternal(MemoryStream ms, ILogger logger, AssetLocation loc = null)
        {
            try
            {
                using (ms)
                {
                    bmp = new Bitmap(ms);
                }

            }
            catch (Exception e)
            {
                if (loc != null)
                {
                    logger.Error("Failed loading bitmap from png file {0}: {1}. Will default to an empty 1x1 bitmap.", loc, e);
                }
                else logger.Error("Failed loading bitmap, error was: {0}. Will default to an empty 1x1 bitmap.", e);
                bmp = new Bitmap(1, 1);
                bmp.SetPixel(0, 0, System.Drawing.Color.Orange);
            }
        }

        public BitmapExternal(Bitmap newBmp)
        {
            bmp = newBmp;
        }

        /// <summary>
        /// Create a BitmapExternal from a path to an existing image file.  Calling code should check that the file exists
        /// </summary>
        /// <param name="filePath"></param>
        public BitmapExternal(string filePath)
        {
            bmp = new Bitmap(filePath);
        }

        /// <summary>
        /// Create a BitmapExternal from a stream.  Calling code should close the stream
        /// </summary>
        /// <param name="stream"></param>
        public BitmapExternal(Stream stream)
        {
            bmp = new Bitmap(stream);
        }

        public override int Height
        {
            get { return bmp.Height; }
        }

        public override int Width
        {
            get { return bmp.Width; }
        }

        public override int[] Pixels
        {
            get
            {

                int[] data = new int[Width * Height];

                System.Runtime.InteropServices.Marshal.Copy(PixelsPtrAndLock, data, 0, bmp.Width * bmp.Height);

                PixelsUnlock();

                return data;
            }
        }


        public IntPtr PixelsPtrAndLock
        {
            get
            {
                bmp_data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                return bmp_data.Scan0;
            }
        }
        public void PixelsUnlock()
        {
            bmp.UnlockBits(bmp_data);
        }

        public override void Dispose()
        {
            bmp.Dispose();
        }

        public override void Save(string filename)
        {
            bmp.Save(filename);
        }

        public void SetPixels(byte[] data, int len)
        {
            bmp.SetPixels(data, len);
        }

        /// <summary>
        /// Retrives the ARGB value from given coordinate
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override System.Drawing.Color GetPixel(int x, int y)
        {
            return bmp.GetPixel(x, y);
        }

        /// <summary>
        /// Retrives the ARGB value from given coordinate using normalized coordinates (0..1)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override System.Drawing.Color GetPixelRel(float x, float y)
        {
            return bmp.GetPixel((int)Math.Min(bmp.Width - 1, x * bmp.Width), (int)Math.Min(bmp.Height - 1, (y * bmp.Height)));
        }

        public override void MulAlpha(int alpha = 255)
        {
            BitmapData bmp_data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int len = Width * Height;
            float af = alpha / 255f;

            unsafe {
                byte* colp = (byte*)bmp_data.Scan0.ToPointer();

                for (int i = 0; i < len; i++)
                {
                    int a = colp[3];
                    colp[0] = (byte)(colp[0] * af);
                    colp[1] = (byte)(colp[1] * af);
                    colp[2] = (byte)(colp[2] * af);

                    colp[3] = (byte)(a * af);
                    colp+=4;
                }
            }

            bmp.UnlockBits(bmp_data);
        }

        public override int[] GetPixelsTransformed(int rot = 0, int mulAlpha = 255)
        {
            int[] bmpPixels = new int[Width * Height];
            int width = bmp.Width;
            int height = bmp.Height;

            FastBitmap fastbmp = new FastBitmap();
            fastbmp.bmp = bmp;
            fastbmp.Lock();
            int stride = fastbmp.Stride;

            // Could be more compact, but this is therefore more efficient
            switch (rot)
            {
                case 0:
                    for (int y = 0; y < height; y++)
                    {
                        fastbmp.GetPixelRow(width, y * stride, bmpPixels, y * width);
                    }
                    break;
                case 90:
                    for (int x = 0; x < width; x++)
                    {
                        int baseY = x * width;
                        for (int y = 0; y < height; y++)
                        {
                            bmpPixels[y + baseY] = fastbmp.GetPixel(width - x - 1, y * stride);
                        }
                    }
                    break;
                case 180:
                    for (int y = 0; y < height; y++)
                    {
                        int baseX = y * width;
                        int yStride = (height - y - 1) * stride;
                        for (int x = 0; x < width; x++)
                        {
                            bmpPixels[x + baseX] = fastbmp.GetPixel(width - x - 1, yStride);
                        }
                    }
                    break;

                case 270:
                    for (int x = 0; x < width; x++)
                    {
                        int baseY = x * width;
                        for (int y = 0; y < height; y++)
                        {
                            bmpPixels[y + baseY] = fastbmp.GetPixel(x, (height - y - 1) * stride);
                        }
                    }
                    break;
            }


            if (mulAlpha != 255)
            {
                float af = mulAlpha / 255f;
                int clearAlpha = ~(0xff << 24);
                for (int i = 0; i < bmpPixels.Length; i++)
                {
                    int col = bmpPixels[i];
                    int a = (col >> 24) & 0xff;
                    col &= clearAlpha;

                    bmpPixels[i] = col | ((int)(a * af) << 24);
                }
            }


            fastbmp.Unlock();

            return bmpPixels;
        }
    }
}
