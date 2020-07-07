using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VintagestoryAPI.Util
{
    public static class BitmapUtil
    {
        public static Bitmap GrayscaleBitmapFromPixels(byte[] pixels, int width, int height)
        {
            //Create 8bpp bitmap and look bitmap data
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            //bmp.SetResolution(horizontalResolution, verticalResolution);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            //Create grayscale color table
            ColorPalette palette = bmp.Palette;
            for (int i = 0; i < 256; i++)
                palette.Entries[i] = Color.FromArgb(i, i, i);
            bmp.Palette = palette;

            //write data to bitmap
            int dataCount = 0;
            int stride = bmpData.Stride < 0 ? -bmpData.Stride : bmpData.Stride;
            unsafe
            {
                byte* row = (byte*)bmpData.Scan0;
                for (int f = 0; f < height; f++)
                {
                    for (int w = 0; w < width; w++)
                    {
                        row[w] = pixels[dataCount];
                        dataCount++;
                    }
                    row += stride;
                }
            }

            //Unlock bitmap data
            bmp.UnlockBits(bmpData);

            return bmp;
        }
    }


    public static class BitmapExtensions
    {

        public static void SetPixels(this Bitmap bmp, int[] pixels)
        {
            if (bmp.Width * bmp.Height != pixels.Length) throw new ArgumentException("Pixel array must be width*height length");

            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var bitmapData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);

            Marshal.Copy(pixels, 0, bitmapData.Scan0, pixels.Length);

            bmp.UnlockBits(bitmapData);
        }

    }


}
