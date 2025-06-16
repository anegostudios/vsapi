
#nullable disable
namespace Vintagestory.API.Client
{
    public class TextBackground
    {
        public int Padding
        {
            set { HorPadding = value; VerPadding = value; }
        }

        /// <summary>
        /// The padding around the text.
        /// </summary>
        public int HorPadding = 0;

        public int VerPadding = 0;

        /// <summary>
        /// The radius of the text.
        /// </summary>
        public double Radius = 0;

        /// <summary>
        /// The fill color of the background
        /// </summary>
        public double[] FillColor = new double[] { 0, 0, 0, 0 };

        /// <summary>
        /// The stroke color of the border
        /// </summary>
        public double[] BorderColor = GuiStyle.DialogBorderColor;

        /// <summary>
        /// The thickness of the border
        /// </summary>
        public double BorderWidth = 0;

        /// <summary>
        /// Adds a blur to the background
        /// </summary>
        public bool Shade = false;

        public double[] ShadeColor = new double[] { GuiStyle.DialogLightBgColor[0] * 1.4, GuiStyle.DialogStrongBgColor[1] * 1.4, GuiStyle.DialogStrongBgColor[2] * 1.4, 1 };

        public TextBackground Clone()
        {
            return new TextBackground()
            {
                HorPadding = HorPadding,
                VerPadding = VerPadding,
                Radius = Radius,
                FillColor = (double[])FillColor.Clone(),
                BorderColor = (double[])BorderColor.Clone(),
                ShadeColor = (double[])ShadeColor.Clone(),
                Shade = Shade,
                BorderWidth = BorderWidth
            };
        }
    }
}
