using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Client
{
    public class KeyEvent
    {
        char keyChar;
        int keyCode;

        
        bool modifierCtrl;
        bool modifierShift;
        bool modifierAlt;
        bool handled;

        public char KeyChar
        {
            get { return keyChar; }
            set { keyChar = value; }
        }

        public int KeyCode
        {
            get { return keyCode; }
            set { keyCode = value; }
        }

        /// <summary>
        /// If a player taps in quick succession, this is the second key
        /// </summary>
        public int? KeyCode2 { get; set; }

        public bool Handled
        {
            get { return handled; }
            set { handled = value; }
        }

        public bool CtrlPressed
        {
            get { return modifierCtrl; }
            set { modifierCtrl = value; }
        }

        public bool ShiftPressed
        {
            get { return modifierShift; }
            set { modifierShift = value; }
        }

        public bool AltPressed
        {
            get { return modifierAlt; }
            set { modifierAlt = value; }
        }
    }

}
