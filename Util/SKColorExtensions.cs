using SkiaSharp;

namespace VintagestoryAPI.Util;

public static class SKColorExtensions
{
    public static int ToArgb(this SKColor color)
    {
        return (int)(uint)color;
    }
}