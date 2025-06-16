using ProtoBuf;
using System;

#nullable disable

namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// Represents a vector of 2 ints. Go bug Tyron if you need more utility methods in this class.
    /// </summary>
    [ProtoContract]
    public class Vec2i : IEquatable<Vec2i>
    {
        [ProtoMember(1)]
        public int X;
        [ProtoMember(2)]
        public int Y;

        public static Vec2i Zero => new Vec2i(0, 0);

        public Vec2i()
        {

        }

        public Vec2i(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vec2i(Vec3d pos)
        {
            this.X = (int)pos.X;
            this.Y = (int)pos.Z;
        }

        public bool Equals(Vec2i other)
        {
            return other != null && X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            Vec2i pos;
            if (obj is Vec2i)
            {
                pos = (Vec2i)obj;
            }
            else
            {
                return false;
            }

            return X == pos.X && Y == pos.Y;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + X.GetHashCode();
            hash = hash * 23 + Y.GetHashCode();
            return hash;
        }

        public int this[int index]
        {
            get { return index == 0 ? X : Y; }
            set { if (index == 0) X = value; else if (index == 1) Y = value; }
        }

        public int ManhattenDistance(Vec2i point)
        {
            return Math.Abs(X - point.X) + Math.Abs(Y - point.Y);
        }

        public int ManhattenDistance(int x, int y)
        {
            return Math.Abs(X - x) + Math.Abs(Y - y);
        }

        public Vec2i Set(int x, int y)
        {
            this.X = x;
            this.Y = y;
            return this;
        }

        public Vec2i Set(Vec2i vec)
        {
            this.X = vec.X;
            this.Y = vec.Y;
            return this;
        }

        public Vec2i Copy()
        {
            return new Vec2i(X, Y);
        }

        public override string ToString()
        {
            return X + " / " + Y;
        }

        public Vec2i Add(int dx, int dy)
        {
            this.X += dx;
            this.Y += dy;
            return this;
        }


        /// <summary>
        /// 27 lowest bits for X Coordinate, then 27 bits for Z coordinate
        /// </summary>
        /// <returns></returns>
        public ulong ToChunkIndex()
        {
            return ((ulong)Y << 27) | (uint)X;
        }


        #region Operators
        public static Vec2i operator -(Vec2i left, Vec2i right)
        {
            return new Vec2i(left.X - right.X, left.Y - right.Y);
        }

        public static Vec2i operator +(Vec2i left, Vec2i right)
        {
            return new Vec2i(left.X + right.X, left.Y + right.Y);
        }

        public static Vec2i operator -(Vec2i left, int right)
        {
            return new Vec2i(left.X - right, left.Y - right);
        }


        public static Vec2i operator -(int left, Vec2i right)
        {
            return new Vec2i(left - right.X, left - right.Y);
        }

        public static Vec2i operator +(Vec2i left, int right)
        {
            return new Vec2i(left.X + right, left.Y + right);
        }


        public static Vec2i operator *(Vec2i left, int right)
        {
            return new Vec2i(left.X * right, left.Y * right);
        }

        public static Vec2i operator *(int left, Vec2i right)
        {
            return new Vec2i(left * right.X, left * right.Y);
        }

        public static Vec2i operator *(Vec2i left, double right)
        {
            return new Vec2i((int)(left.X * right), (int)(left.Y * right));
        }


        public static Vec2i operator *(double left, Vec2i right)
        {
            return new Vec2i((int)(left * right.X), (int)(left * right.Y));
        }


        public static double operator *(Vec2i left, Vec2i right)
        {
            return left.X * right.X + left.Y * right.Y;
        }

        public static Vec2i operator /(Vec2i left, int right)
        {
            return new Vec2i(left.X / right, left.Y / right);
        }

        public static Vec2i operator /(Vec2i left, float right)
        {
            return new Vec2i((int)(left.X / right), (int)(left.Y / right));
        }


        public static bool operator ==(Vec2i left, Vec2i right)
        {
            if (object.ReferenceEquals(left, null))
            {
                return object.ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(Vec2i left, Vec2i right)
        {
            return !(left == right);
        }

        #endregion
    }



}
