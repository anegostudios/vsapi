using System;
using OpenTK.Windowing.GraphicsLibraryFramework;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// A list of mouse buttons.
    /// </summary>
    [DocumentAsJson]
    public enum EnumMouseButton
    {
        Left = 0,
        Middle = 1,
        Right = 2,
        Button4 = 3,
        Button5 = 4,
        Button6 = 5,
        Button7 = 6,
        Button8 = 7,

        /// <summary>
        /// Used to signal to event handlers, but not actually a button: activated when the wheel is scrolled.
        /// </summary>
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
                MouseButton.Last => EnumMouseButton.Button8,
                _ => throw new ArgumentOutOfRangeException(nameof(button), button, null)
            };
        }
    }
}
