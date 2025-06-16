using System;

#nullable disable

namespace Vintagestory.API.Client
{
    [Flags]
    public enum EnumFontStyle
    {
        Regular = 0,
        Bold = 1,
        Italic = 2,
        Underline = 4,
        Strikeout = 8,
    }
}