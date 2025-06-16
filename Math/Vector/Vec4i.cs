using ProtoBuf;
using System;

#nullable disable

namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// Represents a vector of 4 ints. Go bug Tyron if you need more utility methods in this class.
    /// </summary>
    [ProtoContract]
    public class Vec4i : IEquatable<Vec4i>
    {
        [ProtoMember(1)]
        public int X;
        [ProtoMember(2)]
        public int Y;
        [ProtoMember(3)]
        public int Z;
        [ProtoMember(4)]
        public int W;

        public Vec4i()
        {

        }

        public Vec4i(BlockPos pos, int w)
        {
            this.X = pos.X;
            this.Y = pos.InternalY;
            this.Z = pos.Z;
            this.W = w;
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

        /// <summary>
        /// Returns the squared Euclidean horizontal distance to between this and given position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public float HorDistanceSqTo(double x, double z)
        {
            double dx = x - X;
            double dz = z - Z;

            return (float)(dx * dx + dz * dz);
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
