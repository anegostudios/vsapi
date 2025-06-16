
#nullable disable
namespace Vintagestory.API.Client
{
    /// <summary>
    /// First word = X Alignment (left, center, right or fixed)
    /// Second word = Y Alignment (top, middle, bottom or fixed)
    /// </summary>
    public enum EnumDialogArea
    {
        None,
        LeftTop,
        LeftMiddle,
        LeftBottom,
        LeftFixed,

        CenterTop,
        CenterMiddle,
        CenterBottom,
        CenterFixed,

        RightTop,
        RightMiddle,
        RightBottom,
        RightFixed,

        FixedTop,
        FixedMiddle,
        FixedBottom,

        TextBaselineOffset,
    }
}
