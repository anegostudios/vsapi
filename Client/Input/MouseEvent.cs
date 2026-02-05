using Vintagestory.API.Common;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// This contains the data for what the mouse is currently doing.
    /// </summary>
    public class MouseEvent
    {
        /// <summary>
        /// Current X position of the mouse.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Current Y position of the mouse.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// The X movement of the mouse.
        /// </summary>
        public int DeltaX { get; }

        /// <summary>
        /// The Y movement of the mouse.
        /// </summary>
        public int DeltaY { get; }

        /// <summary>
        /// Gets the current mouse button pressed.
        /// </summary>
        public EnumMouseButton Button { get; }

        public int Modifiers { get; }

        /// <summary>
        /// Am I handled?
        /// </summary>
        public bool Handled { get; set; }


        public MouseEvent(int x, int y, int deltaX, int deltaY, EnumMouseButton button, int modifiers)
        {
            X = x; Y = y; DeltaX = deltaX; DeltaY = deltaY; Button = button; Modifiers = modifiers;
        }

        public MouseEvent(int x, int y, int deltaX, int deltaY) : this(x, y, deltaX, deltaY, EnumMouseButton.None, 0) { }

        public MouseEvent(int x, int y, EnumMouseButton button, int modifiers) : this(x, y, 0, 0, button, (int)modifiers) { }

        public MouseEvent(int x, int y) : this(x, y, 0, 0, EnumMouseButton.None, 0) { }
    }

    /// <summary>
    /// The event arguments for the mouse.
    /// </summary>
    public class MouseWheelEventArgs
    {
        /// <summary>
        /// The rough change in time since last called.
        /// </summary>
        public int delta;

        /// <summary>
        /// The precise change in time since last called.
        /// </summary>
        public float deltaPrecise;

        /// <summary>
        /// The rough change in value.
        /// </summary>
        public int value;

        /// <summary>
        /// The precise change in value.
        /// </summary>
        public float valuePrecise;

        /// <summary>
        /// Is the current event being handled?
        /// </summary>
        public bool IsHandled { get; private set; }

        /// <summary>
        /// Changes or sets the current handled state.
        /// </summary>
        /// <param name="value">Should the event be handled?</param>
        public void SetHandled(bool value = true) { IsHandled = value; }
    }

}
