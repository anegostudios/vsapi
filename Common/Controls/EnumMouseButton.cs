using System;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Vintagestory.API.Common
{
    public enum EnumMouseButton
    {
        Left = 0,
        Middle = 1,
        Right = 2,
        Button1 = 3,
        Button2 = 4,
        Button3 = 5,
        Button4 = 6,
        Button5 = 7,
        Button6 = 8,
        Button7 = 9,
        Button8 = 10,
        Button9 = 11,
        LastButton = 12,

        Wheel = 13,

        None = 255,
    }

    public class MouseButtonConverter
    {
        public static EnumMouseButton ToEnumMouseButton(MouseButton button)
        {
            return button switch
            {
                MouseButton.Button1 => EnumMouseButton.Left,
                MouseButton.Button2 => EnumMouseButton.Right,
                MouseButton.Button3 => EnumMouseButton.Middle,
                MouseButton.Button4 => EnumMouseButton.Button4,
                MouseButton.Button5 => EnumMouseButton.Button5,
                MouseButton.Button6 => EnumMouseButton.Button6,
                MouseButton.Button7 => EnumMouseButton.Button7,
                MouseButton.Button8 => EnumMouseButton.Button8,
                _ => throw new ArgumentOutOfRangeException(nameof(button), button, null)
            };
        }
    }
}
