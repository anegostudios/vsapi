using System;
using System.IO;
using SkiaSharp;

#nullable disable

namespace Vintagestory.API.Util;

public static class BitmapExtensions
{
    public static unsafe void SetPixels(this SKBitmap bmp, int[] pixels)
    {
        if (bmp.Width * bmp.Height != pixels.Length)
        {
            throw new ArgumentException("Pixel array must be width*height length");
        }

        fixed (int* ptr = pixels)
        {
            bmp.SetPixels((IntPtr)ptr);
        }
    }

    public static void Save(this SKBitmap bmp, string filename)
    {
        using Stream fileStream = File.OpenWrite(filename);
        bmp.Encode(SKEncodedImageFormat.Png, 100).SaveTo(fileStream);
    }
}
