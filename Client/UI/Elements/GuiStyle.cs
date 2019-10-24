namespace Vintagestory.API.Client
{
    //public static class ElementGeometrics
    /// <summary>
    /// A class containing common values for elements before scaling is applied.
    /// </summary>
    public static class GuiStyle
    {
        // In unscaled values!

        /// <summary>
        /// The padding between the element and the dialogue. 20f.
        /// </summary>
        public static double ElementToDialogPadding = 20f;

        /// <summary>
        /// The padding between other things.  5f.
        /// </summary>
        public static double HalfPadding = 5f;

        /// <summary>
        /// The padding between the dialogue and the screen. 10f.
        /// </summary>
        public static double DialogToScreenPadding = 10f;

        /// <summary>
        /// The height of the title bar. 30.
        /// </summary>
        public static double TitleBarHeight = 31;

        /// <summary>
        /// The radius of the dialogue background. 1.
        /// </summary>
        public static double DialogBGRadius = 1;

        /// <summary>
        /// The radius of the element background. 1.
        /// </summary>
        public static double ElementBGRadius = 1;

        /// <summary>
        /// The size of the large font. 40.
        /// </summary>
        public static double LargeFontSize = 40; 

        // Used for text boxes
        /// <summary>
        /// The size of the normal fonts.  Used for text boxes. 30.
        /// </summary>
        public static double NormalFontSize = 30;

        /// <summary>
        /// The fonts that are slightly smaller than normal fonts. 24.
        /// </summary>
        public static double SubNormalFontSize = 24;

        /// <summary>
        /// The smaller fonts. 20.
        /// </summary>
        public static double SmallishFontSize = 20;

        /// <summary>
        /// The smallest font size used in the game that isn't used with itemstacks. 16.
        /// </summary>
        public static double SmallFontSize = 16;

        // Used for itemstack quantities and in itemstack info box 
        /// <summary>
        /// The font size used for specific details like Item Stack size info. 14.
        /// </summary>
        public static double DetailFontSize = 14;

        /// <summary>
        /// The decorative font type. "Lora".
        /// </summary>
        public static string DecorativeFontName = "Lora";

        /// <summary>
        /// The standard font "Montserrat".
        /// </summary>
        public static string StandardFontName = "sans-serif";

        /// <summary>
        /// The standard font with slight bold. "Monserrat".
        /// </summary>
        public static string StandardSemiBoldFontName = "sans-serif";

        /// <summary>
        /// The standard font with bold. "Montserrat"
        /// </summary>
        public static string StandardBoldFontName = "sans-serif";


        #region VS Color Palette 
        private static double[] Time1 = new double[] { 56 / 255.0, 232 / 255.0, 183 / 255.0, 1 };
        private static double[] Time2 = new double[] { 79 / 255.0, 98 / 255.0, 94 / 255.0, 1 };

        private static double[] Rust1 = new double[] { 208 / 255.0, 91 / 255.0, 12 / 255.0, 1 };
        private static double[] Rust2 = new double[] { 143 / 255.0, 47 / 255.0, 0 / 255.0, 1 };
        private static double[] Rust3 = new double[] { 116 / 255.0, 49 / 255.0, 4 / 255.0, 1 };

        private static double[] ColorWood = new double[] { 132 / 255.0, 92 / 255.0, 67 / 255.0, 1 };

        private static double[] ColorParchment = new double[] { 237 / 255.0, 206 / 255.0, 152 / 255.0, 1 };
        private static double[] ColorSchematic = new double[] { 255 / 255.0, 226 / 255.0, 194 / 255.0, 1 };

        private static double[] Rot1 = new double[] { 98 / 255.0, 69 / 255.0, 65 / 255.0, 1 };
        private static double[] Rot2 = new double[] { 102 / 255.0, 110 / 255.0, 112 / 255.0, 1 };
        private static double[] Rot3 = new double[] { 98 / 255.0, 74 / 255.0, 64 / 255.0, 1 };
        private static double[] Rot4 = new double[] { 45 / 255.0, 35 / 255.0, 33 / 255.0, 1 };
        private static double[] Rot5 = new double[] { 25 / 255.0, 15 / 255.0, 13 / 255.0, 1 };
        #endregion


        public static double[] DialogSlotBackColor = ColorSchematic;
        public static double[] DialogSlotFrontColor = ColorWood;

        /// <summary>
        /// The light background color for dialogs.
        /// </summary>
        public static double[] DialogLightBgColor = new double[] { 64 / 255.0, 53 / 255.0, 41.0 / 255.0, 0.75 };
        /// <summary>
        /// The default background color for dialogs.
        /// </summary>
        public static double[] DialogDefaultBgColor = new double[] { 64 / 255.0, 53 / 255.0, 41.0 / 255.0, 0.8 };
        /// <summary>
        /// The strong background color for dialogs.
        /// </summary>
        public static double[] DialogStrongBgColor = new double[] { 64 / 255.0, 53 / 255.0, 41.0 / 255.0, 1 };
        /// <summary>
        /// The default dialog border color
        /// </summary>
        public static double[] DialogBorderColor = new double[] { 0, 0, 0, 0.4 };

        /// <summary>
        /// The Highlight color for dialogs.
        /// </summary>
        public static double[] DialogHighlightColor = new double[] { 168 / 255.0, 139 / 255.0, 108.0 / 255.0, 0.9 };
        /// <summary>
        /// The alternate background color for dialogs.
        /// </summary>
        public static double[] DialogAlternateBgColor = new double[] { 181 / 255.0, 174 / 255.0, 166 / 255.0, 0.93 };
        /// <summary>
        /// A blue color for dialog backgrounds.
        /// </summary>
        public static double[] DialogBlueBgColor = new double[] { 52 / 255.0, 55 / 255.0, 184 / 255.0, 1 };

        /// <summary>
        /// The default text color for any given dialog.
        /// </summary>
        public static double[] DialogDefaultTextColor = new double[] { 232 / 255.0, 214 / 255.0, 193 / 255.0, 1 };
        /// <summary>
        /// The color of the error text.
        /// </summary>
        public static double[] ErrorTextColor = new double[] { 1, 0.5, 0.5, 1 };
        /// <summary>
        /// The color of the the link text.
        /// </summary>
        public static double[] LinkTextColor = new double[] { 0.5, 0.5, 1, 1 };
        /// <summary>
        /// A light brown text color.
        /// </summary>
        public static double[] ButtonTextColor = new double[] { 224 / 255.0, 207 / 255.0, 187 / 255.0, 1 };
        /// <summary>
        /// A hover color for the light brown text.
        /// </summary>
        public static double[] ActiveButtonTextColor = new double[] { 197 / 255.0, 137 / 255.0, 72 / 255.0, 1 };
        /// <summary>
        /// The text color for a disabled object.
        /// </summary>
        public static double[] DisabledTextColor = new double[] { 1, 1, 1, 0.35 };
        /// <summary>
        /// A color for a darker brown.
        /// </summary>
        public static double[] DarkBrownColor = new double[] { 73 / 255.0, 58 / 255.0, 41 / 255.0, 1 };

        /// <summary>
        /// The color of the 1..9 numbers on the hotbar slots
        /// </summary>
        public static double[] HotbarNumberTextColor = new double[] { 73 / 255.0, 58 / 255.0, 41 / 255.0, 0.5 };

        /// <summary>
        /// The color of the actively selected slot overlay
        /// </summary>
        public static double[] ActiveSlotColor = new double[] { 98 / 255.0, 197 / 255.0, 219 / 255.0, 1 };

        /// <summary>
        /// The color of the health bar.
        /// </summary>
        public static double[] HealthBarColor = new double[] { 0.659, 0, 0, 1 };
        /// <summary>
        /// The color of the food bar.
        /// </summary>
        public static double[] FoodBarColor = new double[] { 0.482, 0.521, 0.211, 1 };
        //public static double[] FatBarColor = new double[] { 224 / 255.0, 222 / 255.0, 186 / 255.0, 1 };
        /// <summary>
        /// The color of the XP bar.
        /// </summary>
        public static double[] XPBarColor = new double[] { 0.745, 0.61, 0f, 1 };

        /// <summary>
        /// The color of the title bar.
        /// </summary>
        public static double[] TitleBarColor = new double[] { 0, 0, 0, 0.2 };
        
        /// <summary>
        /// The color of the macro icon.
        /// </summary>
        public static double[] MacroIconColor = new double[] { 1, 1, 1, 1 };
        
    }
}
