using SkiaSharp;

#nullable disable

namespace Vintagestory.Util;

public static class SKColorExtensions
{
    public static int ToArgb(this SKColor color)
    {
        return (int)(uint)color;
    }
}
