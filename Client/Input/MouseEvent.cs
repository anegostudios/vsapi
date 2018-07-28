using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace Vintagestory.API.Client
{
    public class MouseEvent
    {
        public int X;
        public int Y;
        public int MovementX;
        public int MovementY;
        public EnumMouseButton Button;
        public bool Handled { get; set; }

        /// <summary>
        /// This is apparently used for mouse move events (set to true if the mouse state has changed during constant polling, set to false if the move event came from opentk. This emulated state is apparantly used to determine the correct delta position to turn the camera.
        /// </summary>
        /// <returns></returns>
        /// 
        //public bool Emulated;
    }


    public class MouseWheelEventArgs
    {
        public int delta;
        public float deltaPrecise;
        public int value;
        public float valuePrecise;


        bool handled;
        public bool IsHandled { get { return handled; } }
        public void SetHandled(bool value = true) { handled = value; }
    }

}
