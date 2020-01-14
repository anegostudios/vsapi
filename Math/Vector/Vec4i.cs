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
    public class Vec4i : IEquatable<Vec4i>
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

        public bool Equals(Vec4i other)
        {
            return other != null && other.X == X && other.Y == Y && other.Z == Z && other.W == W;
        }


        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + X.GetHashCode();
            hash = hash * 23 + Y.GetHashCode();
            hash = hash * 23 + Z.GetHashCode();
            hash = hash * 23 + W.GetHashCode();
            return hash;
        }
    }


    public class Vec4i<T>
    {
        public int X;
        public int Y;
        public int Z;
        public T Value;

        public Vec4i()
        {

        }

        public Vec4i(int x, int y, int z, T Value)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Value = Value;
        }
    }
}
