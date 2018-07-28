using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Mapping of a key combination
    /// </summary>
    public class KeyCombination
    {
        public int KeyCode;
        public int? SecondKeyCode = null;
        public bool Ctrl = false;
        public bool Alt = false;
        public bool Shift = false;


        public override string ToString()
        {
            if (KeyCode < 0) return "?";

            List<string> keys = new List<string>();
            if (Ctrl) keys.Add("CTRL");
            if (Alt) keys.Add("ALT");
            if (Shift) keys.Add("SHIFT");
            keys.Add("" + GlKeyNames.ToString((GlKeys)KeyCode));
            if (SecondKeyCode != null && SecondKeyCode > 0) keys.Add("" + GlKeyNames.ToString((GlKeys)SecondKeyCode));

            return string.Join(" + ", keys.ToArray());
        }

        public KeyCombination Clone()
        {
            return (KeyCombination)MemberwiseClone();
        }
    }
}
