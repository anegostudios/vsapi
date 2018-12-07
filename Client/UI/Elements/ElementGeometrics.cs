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
        public static double TitleBarHeight = 30;

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
        public static string StandardFontName = "Montserrat";

        /// <summary>
        /// The standard font with slight bold. "Monserrat".
        /// </summary>
        public static string StandardSemiBoldFontName = "Montserrat";

        /// <summary>
        /// The standard font with bold. "Montserrat"
        /// </summary>
        public static string StandardBoldFontName = "Montserrat";

        /// <summary>
        /// The light background color for dialogs.
        /// </summary>
        public static double[] DialogLightBgColor = new double[] { 64 / 255.0, 53 / 255.0, 41.0 / 255.0, 0.35 };
        /// <summary>
        /// The default background color for dialogs.
        /// </summary>
        public static double[] DialogDefaultBgColor = new double[] { 64 / 255.0, 53 / 255.0, 41.0 / 255.0, 0.8 };
        /// <summary>
        /// The strong background color for dialogs.
        /// </summary>
        public static double[] DialogStrongBgColor = new double[] { 64 / 255.0, 53 / 255.0, 41.0 / 255.0, 0.94 };
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
        public static double[] DialogBlueBgColor = new double[] { 161 / 255.0, 222 / 255.0, 227 / 255.0, 0.85 };

        /// <summary>
        /// The default text color for any given dialog.
        /// </summary>
        public static double[] DialogDefaultTextColor = new double[] { 224 / 255.0, 207 / 255.0, 187 / 255.0, 1 };
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
        public static double[] LightBrownTextColor = new double[] { 224 / 255.0, 207 / 255.0, 187 / 255.0, 1 };
        /// <summary>
        /// A hover color for the light brown text.
        /// </summary>
        public static double[] LightBrownHoverTextColor = new double[] { 197 / 255.0, 137 / 255.0, 72 / 255.0, 1 };
        /// <summary>
        /// The text color for a disabled object.
        /// </summary>
        public static double[] DisabledTextColor = new double[] { 1, 1, 1, 0.35 };
        /// <summary>
        /// A color for a darker brown.
        /// </summary>
        public static double[] DarkBrownColor = new double[] { 73 / 255.0, 58 / 255.0, 41 / 255.0, 1 };

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
