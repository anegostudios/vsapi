using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// Represents a vector of 2 doubles. Go bug Tyron of you need more utility methods in this class.
    /// </summary>
    public class Size2d
    {
        public double Width;
        public double Height;


        public Size2d()
        {

        }

        public Size2d(double width, double height)
        {
            this.Width = width;
            this.Height = height;
        }
    }



    /// <summary>
    /// Represents a vector of 2 doubles. Go bug Tyron of you need more utility methods in this class.
    /// </summary>
    public class Size2i
    {
        public int Width;
        public int Height;


        public Size2i()
        {

        }

        public Size2i(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }
    }
}
