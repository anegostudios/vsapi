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
        /// <summary>
        /// The KeyCode
        /// </summary>
        public int KeyCode;

        /// <summary>
        /// The second key code (if it exists).
        /// </summary>
        public int? SecondKeyCode = null;

        /// <summary>
        /// Ctrl pressed condition.
        /// </summary>
        public bool Ctrl = false;

        /// <summary>
        /// Alt pressed condition.
        /// </summary>
        public bool Alt = false;

        /// <summary>
        /// Shift pressed condition.
        /// </summary>
        public bool Shift = false;

        /// <summary>
        /// Converts this key combination into a string.
        /// </summary>
        /// <returns>The string code for this Key Combination.</returns>
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

        /// <summary>
        /// Clones the current key combination.
        /// </summary>
        /// <returns>The cloned key combination.</returns>
        public KeyCombination Clone()
        {
            return (KeyCombination)MemberwiseClone();
        }
    }
}
