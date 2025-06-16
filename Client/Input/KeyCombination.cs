using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Globalization;
using Vintagestory.API.Config;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Mapping of an input key combination.   Note: the "key" might also be a mouse button if a hotkey has been configured to be activated by a mouse button
    /// </summary>
    public class KeyCombination
    {
        /// <summary>
        /// The first keycode representing a mouse button
        /// </summary>
        public const int MouseStart = 240;

        /// <summary>
        /// The KeyCode (from 1.19.4, the keycodes map to either keys or mouse buttons)
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

        public bool IsMouseButton(int KeyCode)
        {
            return KeyCode >= MouseStart && KeyCode < MouseStart + 8;
        }

        public bool OnKeyUp = false;

        /// <summary>
        /// Converts this key combination into a string.
        /// </summary>
        /// <returns>The string code for this Key Combination.</returns>
        public override string ToString()
        {
            if (KeyCode < 0) return "?";
            if (IsMouseButton(KeyCode)) return MouseButtonAsString(KeyCode);

            List<string> keys = new List<string>();
            if (Ctrl) keys.Add("CTRL");
            if (Alt) keys.Add("ALT");
            if (Shift) keys.Add("SHIFT");
            if (KeyCode == (int)GlKeys.Escape) keys.Add("Esc");
            else keys.Add("" + GlKeyNames.ToString((GlKeys)KeyCode));
            if (SecondKeyCode != null && SecondKeyCode > 0) keys.Add(SecondaryAsString());

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

        public string PrimaryAsString()
        {
            if (IsMouseButton(KeyCode)) return MouseButtonAsString(KeyCode);
            if (KeyCode == (int)GlKeys.Escape) return "Esc";
            return GlKeyNames.ToString((GlKeys)KeyCode);
        }

        public string SecondaryAsString()
        {
            if (IsMouseButton(SecondKeyCode.Value)) return MouseButtonAsString(SecondKeyCode.Value);
            return GlKeyNames.ToString((GlKeys)SecondKeyCode);
        }

        private string MouseButtonAsString(int keyCode)
        {
            int button = keyCode - MouseStart;
            switch (button)
            {
                case 0: return Lang.Get("Left mouse button");
                case 1: return Lang.Get("Middle mouse button");
                case 2: return Lang.Get("Right mouse button");
                default: return Lang.Get("Mouse button {0}", button + 1);   // e.g. "Button 4" for (int)3, see also EnumMouseButton.Button4
            }
        }
    }
}
