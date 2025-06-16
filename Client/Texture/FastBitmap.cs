using SkiaSharp;

#nullable disable

namespace Vintagestory.API.Client;

public class FastBitmap
{
    private unsafe byte* _ptr;

    public SKBitmap _bmp;

    public unsafe SKBitmap bmp
    {
        get => _bmp;
        set
        {
            _bmp = value;
            _ptr = (byte*)_bmp.GetPixels().ToPointer();
        }
    }

    public int Stride => bmp.RowBytes;

    public unsafe int GetPixel(int x, int y)
    {
        var row = (uint*)(_ptr + y);
        int d = (int)row[x];
        return d == 0 ? 0x8F8F8F : d;
    }

    internal unsafe void GetPixelRow(int width, int y, int[] bmpPixels, int baseX)
    {
        var row = (uint*)(_ptr + y);
        fixed (int* target = bmpPixels)
        {
            for (int x = 0; x < width; x++)
            {
                int d = (int)row[x];
                target[x + baseX] = d == 0 ? 0x8F8F8F : d;
            }
        }
    }

    public unsafe void SetPixel(int x, int y, int color)
    {
        var row = (uint*)(_ptr + y * Stride);
        row[x] = (uint)color;
    }
}
