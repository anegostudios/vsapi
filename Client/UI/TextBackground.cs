using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Client
{
    public class TextBackground
    {
        /// <summary>
        /// The padding around the text.
        /// </summary>
        public int Padding = 0;

        /// <summary>
        /// The radius of the text.
        /// </summary>
        public double Radius = 0;

        /// <summary>
        /// The fill color of the background.
        /// </summary>
        public double[] FillColor = new double[] { 0, 0, 0, 0 };

        /// <summary>
        /// The stroke color of the text.
        /// </summary>
        public double[] StrokeColor = GuiStyle.DialogBorderColor;

        /// <summary>
        /// The thickness of the text.
        /// </summary>
        public double StrokeWidth = 0;
    }
}
