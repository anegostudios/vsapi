using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// Represents a vector of 4 ints. Go bug Tyron if you need more utility methods in this class.
    /// </summary>
    public class Vec4i
    {
        public int X;
        public int Y;
        public int Z;
        public int W;

        public Vec4i()
        {

        }

        public Vec4i(int x, int y, int z, int w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

    }
}
