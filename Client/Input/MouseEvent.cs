using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

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
        public int X;

        /// <summary>
        /// Current Y position of the mouse.
        /// </summary>
        public int Y;

        /// <summary>
        /// The X movement of the mouse.
        /// </summary>
        public int MovementX;

        /// <summary>
        /// The Y movement of the mouse.
        /// </summary>
        public int MovementY;

        /// <summary>
        /// The current state of the mouse buttons.
        /// </summary>
        public EnumMouseButton Button;

        /// <summary>
        /// Am I handled?
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// This is apparently used for mouse move events (set to true if the mouse state has changed during constant polling, set to false if the move event came from opentk. This emulated state is apparantly used to determine the correct delta position to turn the camera.
        /// </summary>
        /// <returns></returns>
        /// 
        //public bool Emulated;
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
