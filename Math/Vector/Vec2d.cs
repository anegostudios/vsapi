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
    public class Vec2d
    {
        public double X;
        public double Y;
        

        public Vec2d()
        {

        }

        public Vec2d(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
