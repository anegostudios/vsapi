
#nullable disable
namespace Vintagestory.API.Client
{
    public class FontConfig
    {
        /// <summary>
        /// The size of the font before scaling is applied.
        /// </summary>
        public double UnscaledFontsize;

        /// <summary>
        /// The name of the font.
        /// </summary>
        public string Fontname;

        /// <summary>
        /// The weight of the font.
        /// </summary>
        public Cairo.FontWeight FontWeight;

        /// <summary>
        /// The color of the font.
        /// </summary>
        public double[] Color;

        /// <summary>
        /// The color of the font outline.
        /// </summary>
        public double[] StrokeColor;

        /// <summary>
        /// The thickness of the outline.
        /// </summary>
        public double StrokeWidth;
    }
}
