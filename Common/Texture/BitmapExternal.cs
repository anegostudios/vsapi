using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;

namespace Vintagestory.API.Common
{
    public class BitmapExternal : BitmapRef
    {
        public Bitmap bmp;

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
                BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                int[] data = new int[Width * Height];

                System.Runtime.InteropServices.Marshal.Copy(bmp_data.Scan0, data, 0, bmp.Width * bmp.Height);

                bmp.UnlockBits(bmp_data);

                return data;
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
        public override Color GetPixel(int x, int y)
        {
            return bmp.GetPixel(x, y);
        }

        /// <summary>
        /// Retrives the ARGB value from given coordinate using normalized coordinates (0..1)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override Color GetPixelRel(float x, float y)
        {
            return bmp.GetPixel((int)Math.Min(bmp.Width - 1, x * bmp.Width), (int)Math.Min(bmp.Height - 1, (y * bmp.Height)));
        }


        public override int[] GetPixelsRotated(int rot = 0)
        {
            int[] bmpPixels = new int[Width * Height];
            int width = bmp.Width;
            int height = bmp.Height;

            FastBitmap fastbmp = new FastBitmap();
            fastbmp.bmp = bmp;
            fastbmp.Lock();

            // Could be more compact, but this is therefore more efficient
            switch (rot)
            {
                case 0:
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            bmpPixels[x + y * width] = fastbmp.GetPixel(x, y);
                        }
                    }
                    break;
                case 90:
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            bmpPixels[y + x * width] = fastbmp.GetPixel(width - x - 1, y);
                        }
                    }
                    break;
                case 180:
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            bmpPixels[x + y * width] = fastbmp.GetPixel(width - x - 1, height - y - 1);
                        }
                    }
                    break;

                case 270:
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            bmpPixels[y + x * width] = fastbmp.GetPixel(x, height - y - 1);
                        }
                    }
                    break;
            }


            fastbmp.Unlock();

            return bmpPixels;
        }
    }
}
