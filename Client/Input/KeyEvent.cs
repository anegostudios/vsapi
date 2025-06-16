
#nullable disable

namespace Vintagestory.API.Client
{
    public class KeyEvent
    {
        /// <summary>
        /// the character for the given key.
        /// </summary>
        public char KeyChar { get; set; }

        /// <summary>
        /// The keycode value.  
        /// </summary>
        public int KeyCode { get; set; }

        /// <summary>
        /// If a player taps in quick succession, this is the second key
        /// </summary>
        public int? KeyCode2 { get; set; }

        /// <summary>
        /// Is this keypress/key combination handled?
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// Is control/Ctrl being held down?
        /// </summary>
        public bool CtrlPressed { get; set; }

        /// <summary>
        /// Is mac os command key being held down?
        /// </summary>
        public bool CommandPressed { get; set; }

        /// <summary>
        /// Is Shift being held down?
        /// </summary>
        public bool ShiftPressed { get; set; }

        /// <summary>
        /// Is Alt being held down?
        /// </summary>
        public bool AltPressed { get; set; }
    }
}
