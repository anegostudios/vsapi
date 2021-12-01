using System;

namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// Represents a vector of 4 unsigned shorts. Go bug Tyron if you need more utility methods in this class.
    /// </summary>
    public class Vec4us : IEquatable<Vec4us>
    {
        public ushort X;
        public ushort Y;
        public ushort Z;
        public ushort W;

        public Vec4us()
        {

        }

        public Vec4us(ushort x, ushort y, ushort z, ushort w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public bool Equals(Vec4us other)
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

    public class Vec4us<T>
    {
        public ushort X;
        public ushort Y;
        public ushort Z;
        public T Value;

        public Vec4us()
        {

        }

        public Vec4us(ushort x, ushort y, ushort z, T Value)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.Value = Value;
        }
    }
}
