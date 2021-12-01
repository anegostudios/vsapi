using System;

namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// Represents a vector of 4 shorts. Go bug Tyron if you need more utility methods in this class.
    /// </summary>
    public class Vec4s : IEquatable<Vec4s>
    {
        public short X;
        public short Y;
        public short Z;
        public short W;

        public Vec4s()
        {

        }

        public Vec4s(short x, short y, short z, short w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public bool Equals(Vec4s other)
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

    public class Vec4s<T>
    {
        public short X;
        public short Y;
        public short Z;
        public T Value;

        public Vec4s()
        {

        }

        public Vec4s(short x, short y, short z, T Value)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Value = Value;
        }
    }
}
