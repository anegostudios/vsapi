using System;
using OpenTK.Windowing.GraphicsLibraryFramework;

#nullable disable

namespace Vintagestory.API.Client
{
    public static class GlKeyNames
    {
        /// <summary>
        /// Converts the given key to a string.
        /// </summary>
        /// <param name="key">the key being passed in.</param>
        /// <returns>the string name of the key.</returns>
        public static string ToString(GlKeys key)
        {
            return key switch
            {
                GlKeys.Keypad0 => "Keypad 0",
                GlKeys.Keypad1 => "Keypad 1",
                GlKeys.Keypad2 => "Keypad 2",
                GlKeys.Keypad3 => "Keypad 3",
                GlKeys.Keypad4 => "Keypad 4",
                GlKeys.Keypad5 => "Keypad 5",
                GlKeys.Keypad6 => "Keypad 6",
                GlKeys.Keypad7 => "Keypad 7",
                GlKeys.Keypad8 => "Keypad 8",
                GlKeys.Keypad9 => "Keypad 9",
                GlKeys.KeypadDivide => "Keypad Divide",
                GlKeys.KeypadMultiply => "Keypad Multiply",
                GlKeys.KeypadSubtract => "Keypad Subtract",
                GlKeys.KeypadAdd => "Keypad Add",
                GlKeys.KeypadDecimal => "Keypad Decimal",
                GlKeys.KeypadEnter => "Keypad Enter",
                GlKeys.Unknown => "Unknown",
                GlKeys.LShift => "Shift",    // Replace the default "LShift" which is hideous and confusing (it's not what the key cap says, especially for non-English languages)
                GlKeys.LControl => "Ctrl",    // Replace the default "LCtrl" which is hideous and confusing (it's not what the key cap says, especially for non-English languages)
                GlKeys.LAlt => "Alt",    // Replace the default "LAlt" which is hideous and confusing (it's not what the key cap says, especially for non-English languages)
                _ => GetKeyName(key)
            };
            ;
        }

        /// <summary>
        /// Gets the string the key would produce upon pressing it without considering any modifiers (but single keys get converted to uppercase).
        /// So GlKeys.W on QWERTY Keyboard layout returns W, GlKeys.Space returns Space etc.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetKeyName(GlKeys key)
        {
            var keyName = GetPrintableChar((int)key);
            if (string.IsNullOrWhiteSpace(keyName))
            {
                return key.ToString();
            }
            return keyName.ToUpperInvariant();
        }

        /// <summary>
        /// Returns the printable character for a key. Does return null on none printable keys like <see cref="GlKeys.Enter"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetPrintableChar(int key)
        {
            try
            {
                var keycode = KeyConverter.GlKeysToNew[key];
                return GLFW.GetKeyName((Keys)keycode, 0);
            }
            catch (IndexOutOfRangeException)
            {
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// Converts key code from OpenTK 4 to GlKeys
    /// </summary>
    public static class KeyConverter
    {
        public static readonly int[] NewKeysToGlKeys = new int[349];
        public static readonly int[] GlKeysToNew = new int[131];

        static KeyConverter()
        {
            NewKeysToGlKeys[32]=(int)GlKeys.Space;
            NewKeysToGlKeys[39]=(int)GlKeys.Quote;
            NewKeysToGlKeys[44]=(int)GlKeys.Comma;
            NewKeysToGlKeys[45]=(int)GlKeys.Minus;
            NewKeysToGlKeys[46]=(int)GlKeys.Period;
            NewKeysToGlKeys[47]=(int)GlKeys.Slash;
            NewKeysToGlKeys[48]=(int)GlKeys.Number0;
            NewKeysToGlKeys[49]=(int)GlKeys.Number1;
            NewKeysToGlKeys[50]=(int)GlKeys.Number2;
            NewKeysToGlKeys[51]=(int)GlKeys.Number3;
            NewKeysToGlKeys[52]=(int)GlKeys.Number4;
            NewKeysToGlKeys[53]=(int)GlKeys.Number5;
            NewKeysToGlKeys[54]=(int)GlKeys.Number6;
            NewKeysToGlKeys[55]=(int)GlKeys.Number7;
            NewKeysToGlKeys[56]=(int)GlKeys.Number8;
            NewKeysToGlKeys[57]=(int)GlKeys.Number9;
            NewKeysToGlKeys[59]=(int)GlKeys.Semicolon;
            NewKeysToGlKeys[61]=(int)GlKeys.Plus;
            NewKeysToGlKeys[65]=(int)GlKeys.A;
            NewKeysToGlKeys[66]=(int)GlKeys.B;
            NewKeysToGlKeys[67]=(int)GlKeys.C;
            NewKeysToGlKeys[68]=(int)GlKeys.D;
            NewKeysToGlKeys[69]=(int)GlKeys.E;
            NewKeysToGlKeys[70]=(int)GlKeys.F;
            NewKeysToGlKeys[71]=(int)GlKeys.G;
            NewKeysToGlKeys[72]=(int)GlKeys.H;
            NewKeysToGlKeys[73]=(int)GlKeys.I;
            NewKeysToGlKeys[74]=(int)GlKeys.J;
            NewKeysToGlKeys[75]=(int)GlKeys.K;
            NewKeysToGlKeys[76]=(int)GlKeys.L;
            NewKeysToGlKeys[77]=(int)GlKeys.M;
            NewKeysToGlKeys[78]=(int)GlKeys.N;
            NewKeysToGlKeys[79]=(int)GlKeys.O;
            NewKeysToGlKeys[80]=(int)GlKeys.P;
            NewKeysToGlKeys[81]=(int)GlKeys.Q;
            NewKeysToGlKeys[82]=(int)GlKeys.R;
            NewKeysToGlKeys[83]=(int)GlKeys.S;
            NewKeysToGlKeys[84]=(int)GlKeys.T;
            NewKeysToGlKeys[85]=(int)GlKeys.U;
            NewKeysToGlKeys[86]=(int)GlKeys.V;
            NewKeysToGlKeys[87]=(int)GlKeys.W;
            NewKeysToGlKeys[88]=(int)GlKeys.X;
            NewKeysToGlKeys[89]=(int)GlKeys.Y;
            NewKeysToGlKeys[90]=(int)GlKeys.Z;
            NewKeysToGlKeys[91]=(int)GlKeys.BracketLeft;
            NewKeysToGlKeys[92]=(int)GlKeys.BackSlash;
            NewKeysToGlKeys[93]=(int)GlKeys.BracketRight;
            NewKeysToGlKeys[96]=(int)GlKeys.Tilde;
            NewKeysToGlKeys[256]=(int)GlKeys.Escape;
            NewKeysToGlKeys[257]=(int)GlKeys.Enter;
            NewKeysToGlKeys[258]=(int)GlKeys.Tab;
            NewKeysToGlKeys[259]=(int)GlKeys.Back;
            NewKeysToGlKeys[260]=(int)GlKeys.Insert;
            NewKeysToGlKeys[261]=(int)GlKeys.Delete;
            NewKeysToGlKeys[262]=(int)GlKeys.Right;
            NewKeysToGlKeys[263]=(int)GlKeys.Left;
            NewKeysToGlKeys[264]=(int)GlKeys.Down;
            NewKeysToGlKeys[265]=(int)GlKeys.Up;
            NewKeysToGlKeys[266]=(int)GlKeys.PageUp;
            NewKeysToGlKeys[267]=(int)GlKeys.PageDown;
            NewKeysToGlKeys[268]=(int)GlKeys.Home;
            NewKeysToGlKeys[269]=(int)GlKeys.End;
            NewKeysToGlKeys[280]=(int)GlKeys.CapsLock;
            NewKeysToGlKeys[281]=(int)GlKeys.ScrollLock;
            NewKeysToGlKeys[282]=(int)GlKeys.NumLock;
            NewKeysToGlKeys[283]=(int)GlKeys.PrintScreen;
            NewKeysToGlKeys[284]=(int)GlKeys.Pause;
            NewKeysToGlKeys[290]=(int)GlKeys.F1;
            NewKeysToGlKeys[291]=(int)GlKeys.F2;
            NewKeysToGlKeys[292]=(int)GlKeys.F3;
            NewKeysToGlKeys[293]=(int)GlKeys.F4;
            NewKeysToGlKeys[294]=(int)GlKeys.F5;
            NewKeysToGlKeys[295]=(int)GlKeys.F6;
            NewKeysToGlKeys[296]=(int)GlKeys.F7;
            NewKeysToGlKeys[297]=(int)GlKeys.F8;
            NewKeysToGlKeys[298]=(int)GlKeys.F9;
            NewKeysToGlKeys[299]=(int)GlKeys.F10;
            NewKeysToGlKeys[300]=(int)GlKeys.F11;
            NewKeysToGlKeys[301]=(int)GlKeys.F12;
            NewKeysToGlKeys[302]=(int)GlKeys.F13;
            NewKeysToGlKeys[303]=(int)GlKeys.F14;
            NewKeysToGlKeys[304]=(int)GlKeys.F15;
            NewKeysToGlKeys[305]=(int)GlKeys.F16;
            NewKeysToGlKeys[306]=(int)GlKeys.F17;
            NewKeysToGlKeys[307]=(int)GlKeys.F18;
            NewKeysToGlKeys[308]=(int)GlKeys.F19;
            NewKeysToGlKeys[309]=(int)GlKeys.F20;
            NewKeysToGlKeys[310]=(int)GlKeys.F21;
            NewKeysToGlKeys[311]=(int)GlKeys.F22;
            NewKeysToGlKeys[312]=(int)GlKeys.F23;
            NewKeysToGlKeys[313]=(int)GlKeys.F24;
            NewKeysToGlKeys[314]=(int)GlKeys.F25;
            NewKeysToGlKeys[320]=(int)GlKeys.Keypad0;
            NewKeysToGlKeys[321]=(int)GlKeys.Keypad1;
            NewKeysToGlKeys[322]=(int)GlKeys.Keypad2;
            NewKeysToGlKeys[323]=(int)GlKeys.Keypad3;
            NewKeysToGlKeys[324]=(int)GlKeys.Keypad4;
            NewKeysToGlKeys[325]=(int)GlKeys.Keypad5;
            NewKeysToGlKeys[326]=(int)GlKeys.Keypad6;
            NewKeysToGlKeys[327]=(int)GlKeys.Keypad7;
            NewKeysToGlKeys[328]=(int)GlKeys.Keypad8;
            NewKeysToGlKeys[329]=(int)GlKeys.Keypad9;
            NewKeysToGlKeys[330]=(int)GlKeys.KeypadDecimal;
            NewKeysToGlKeys[331]=(int)GlKeys.KeypadDivide;
            NewKeysToGlKeys[332]=(int)GlKeys.KeypadMultiply;
            NewKeysToGlKeys[333]=(int)GlKeys.KeypadSubtract;
            NewKeysToGlKeys[334]=(int)GlKeys.KeypadAdd;
            NewKeysToGlKeys[335]=(int)GlKeys.KeypadEnter;
            NewKeysToGlKeys[340]=(int)GlKeys.ShiftLeft;
            NewKeysToGlKeys[341]=(int)GlKeys.ControlLeft;
            NewKeysToGlKeys[342]=(int)GlKeys.AltLeft;
            NewKeysToGlKeys[343]=(int)GlKeys.WinLeft;
            NewKeysToGlKeys[344]=(int)GlKeys.ShiftRight;
            NewKeysToGlKeys[345]=(int)GlKeys.ControlRight;
            NewKeysToGlKeys[346]=(int)GlKeys.AltRight;
            NewKeysToGlKeys[347]=(int)GlKeys.WinRight;
            NewKeysToGlKeys[348]=(int)GlKeys.Menu;


            // set all to Keys.Unknown
            for (int i = 0; i < GlKeysToNew.Length; i++)
            {
                GlKeysToNew[i] = -1;
            }
            // reverse the mapping
            for (int i = 0; i < NewKeysToGlKeys.Length; i++)
            {
                if (NewKeysToGlKeys[i] != 0)
                {
                    GlKeysToNew[NewKeysToGlKeys[i]] = i;
                }
            }
        }
    }

    /// <summary>
    /// Internally the game uses OpenTK and their Keys are by default mapped to US QWERTY Keyboard layout which the GlKeys also do.
    /// Upon typing text in a Text input field it will produce the correct characters according to your keyboard layout.
    ///
    /// If you need to get the character for the current Keyboard layout use <see cref="GlKeyNames.GetKeyName"/>
    /// </summary>
     public enum GlKeys
    {
        Unknown = 0,
        LShift = 1,
        ShiftLeft = 1,
        RShift = 2,
        ShiftRight = 2,
        LControl = 3,
        ControlLeft = 3,
        RControl = 4,
        ControlRight = 4,
        AltLeft = 5,
        LAlt = 5,
        AltRight = 6,
        RAlt = 6,
        WinLeft = 7,
        LWin = 7,
        RWin = 8,
        WinRight = 8,
        Menu = 9,
        F1 = 10,
        F2 = 11,
        F3 = 12,
        F4 = 13,
        F5 = 14,
        F6 = 15,
        F7 = 16,
        F8 = 17,
        F9 = 18,
        F10 = 19,
        F11 = 20,
        F12 = 21,
        F13 = 22,
        F14 = 23,
        F15 = 24,
        F16 = 25,
        F17 = 26,
        F18 = 27,
        F19 = 28,
        F20 = 29,
        F21 = 30,
        F22 = 31,
        F23 = 32,
        F24 = 33,
        F25 = 34,
        F26 = 35,
        F27 = 36,
        F28 = 37,
        F29 = 38,
        F30 = 39,
        F31 = 40,
        F32 = 41,
        F33 = 42,
        F34 = 43,
        F35 = 44,
        Up = 45,
        Down = 46,
        Left = 47,
        Right = 48,
        Enter = 49,
        Escape = 50,
        Space = 51,
        Tab = 52,
        Back = 53,
        BackSpace = 53,
        Insert = 54,
        Delete = 55,
        PageUp = 56,
        PageDown = 57,
        Home = 58,
        End = 59,
        CapsLock = 60,
        ScrollLock = 61,
        PrintScreen = 62,
        Pause = 63,
        NumLock = 64,
        Clear = 65,
        Sleep = 66,
        Keypad0 = 67,
        Keypad1 = 68,
        Keypad2 = 69,
        Keypad3 = 70,
        Keypad4 = 71,
        Keypad5 = 72,
        Keypad6 = 73,
        Keypad7 = 74,
        Keypad8 = 75,
        Keypad9 = 76,
        KeypadDivide = 77,
        KeypadMultiply = 78,
        KeypadMinus = 79,
        KeypadSubtract = 79,
        KeypadAdd = 80,
        KeypadPlus = 80,
        KeypadDecimal = 81,
        KeypadEnter = 82,
        A = 83,
        B = 84,
        C = 85,
        D = 86,
        E = 87,
        F = 88,
        G = 89,
        H = 90,
        I = 91,
        J = 92,
        K = 93,
        L = 94,
        M = 95,
        N = 96,
        O = 97,
        P = 98,
        Q = 99,
        R = 100,
        S = 101,
        T = 102,
        U = 103,
        V = 104,
        W = 105,
        X = 106,
        Y = 107,
        Z = 108,
        Number0 = 109,
        Number1 = 110,
        Number2 = 111,
        Number3 = 112,
        Number4 = 113,
        Number5 = 114,
        Number6 = 115,
        Number7 = 116,
        Number8 = 117,
        Number9 = 118,
        Tilde = 119,
        Minus = 120,
        Plus = 121,
        LBracket = 122,
        BracketLeft = 122,
        BracketRight = 123,
        RBracket = 123,
        Semicolon = 124,
        Quote = 125,
        Comma = 126,
        Period = 127,
        Slash = 128,
        BackSlash = 129,
        LastKey = 130,

        // We reserve 240-248 for mouse buttons - see also KeyCombination.MouseStart
    }
}
