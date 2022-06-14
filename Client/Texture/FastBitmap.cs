using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Vintagestory.API.Client
{
    public class FastBitmap
    {
        public Bitmap bmp { get; set; }
        BitmapData bmd;

        public int Stride { get => bmd.Stride; }

        public void Lock()
        {
            if (bmd != null)
            {
                throw new Exception("Already locked.");
            }
            if (bmp.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                bmp = new Bitmap(bmp);
            }

            bmd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
        }

        public int GetPixel(int x, int y)
        {
            unsafe
            {
                int* row = (int*)((byte*)bmd.Scan0 + y);
                return row[x];
            }
        }

        internal void GetPixelRow(int width, int y, int[] bmpPixels, int baseX)
        {
            unsafe
            {
                int* row = (int*)((byte*)bmd.Scan0 + y);
                fixed (int* target = bmpPixels)
                {
                    for (int x = 0; x < width; x++)
                    {
                        target[x + baseX] = row[x];
                    }
                }
            }
        }

        public void SetPixel(int x, int y, int color)
        {
            if (bmd == null)
            {
                throw new Exception();
            }
            unsafe
            {
                int* row = (int*)((byte*)bmd.Scan0 + (y * bmd.Stride));
                row[x] = color;
            }
        }

        public void Unlock()
        {
            if (bmd == null)
            {
                throw new Exception("Not locked.");
            }
            bmp.UnlockBits(bmd);
            bmd = null;
        }
    }
    
}
