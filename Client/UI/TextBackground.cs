using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Client
{
    public class TextBackground
    {
        public int Padding = 0;
        public double Radius = 0;
        public double[] FillColor = new double[] { 0, 0, 0, 0 };
        public double[] StrokeColor = GuiStyle.DialogBorderColor;
        public double StrokeWidth = 0;
    }
}
