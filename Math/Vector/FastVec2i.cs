using ProtoBuf;
using System;

namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// Represents a vector of 2 ints. Go bug Tyron if you need more utility methods in this class.
    /// </summary>
    [ProtoContract]
    public struct FastVec2i : IEquatable<FastVec2i>
    {
        [ProtoMember(1)]
        public ulong val;
        public int X { get { return (int)val; } set { val = (val & 0xFFFF_FFFF_0000_0000) + (uint)value; } }
        public int Y { get { return (int)(val >>> 32); } set { val = (uint)val + ((ulong)value << 32); } }

        public static FastVec2i Zero => new FastVec2i(0, 0);

        public FastVec2i()
        {

        }

        public FastVec2i(int x, int y)
        {
            this.val = (uint)x + ((ulong)y << 32);
        }

        public FastVec2i(Vec3d pos) : this((int)pos.X, (int)pos.Z)
        {
        }

        public bool Equals(FastVec2i other)
        {
            return val == other.val;
        }

        public override bool Equals(object? obj)
        {
            if (obj is FastVec2i other)
            {
                return val == other.val;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return (int)((uint)(val >> 32) * 23 * 17 + (uint)val);
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

        public FastVec2i Set(int x, int y)
        {
            this.val = (uint)x + ((ulong)y << 32);
            return this;
        }

        public FastVec2i Set(Vec2i vec)
        {
            this.val = (uint)vec.X + ((ulong)vec.Y << 32);
            return this;
        }

        public FastVec2i Copy()
        {
            return this;
        }

        public override string ToString()
        {
            return X + " / " + Y;
        }

        public FastVec2i Add(int dx, int dy)
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
        public static FastVec2i operator -(FastVec2i left, FastVec2i right)
        {
            return new FastVec2i(left.X - right.X, left.Y - right.Y);
        }

        public static FastVec2i operator +(FastVec2i left, FastVec2i right)
        {
            return new FastVec2i(left.X + right.X, left.Y + right.Y);
        }

        public static FastVec2i operator -(FastVec2i left, int right)
        {
            return new FastVec2i(left.X - right, left.Y - right);
        }


        public static FastVec2i operator -(int left, FastVec2i right)
        {
            return new FastVec2i(left - right.X, left - right.Y);
        }

        public static FastVec2i operator +(FastVec2i left, int right)
        {
            return new FastVec2i(left.X + right, left.Y + right);
        }


        public static FastVec2i operator *(FastVec2i left, int right)
        {
            return new FastVec2i(left.X * right, left.Y * right);
        }

        public static FastVec2i operator *(int left, FastVec2i right)
        {
            return new FastVec2i(left * right.X, left * right.Y);
        }

        public static FastVec2i operator *(FastVec2i left, double right)
        {
            return new FastVec2i((int)(left.X * right), (int)(left.Y * right));
        }


        public static FastVec2i operator *(double left, FastVec2i right)
        {
            return new FastVec2i((int)(left * right.X), (int)(left * right.Y));
        }


        public static double operator *(FastVec2i left, FastVec2i right)
        {
            return left.X * right.X + left.Y * right.Y;
        }

        public static FastVec2i operator /(FastVec2i left, int right)
        {
            return new FastVec2i(left.X / right, left.Y / right);
        }

        public static FastVec2i operator /(FastVec2i left, float right)
        {
            return new FastVec2i((int)(left.X / right), (int)(left.Y / right));
        }


        public static bool operator ==(FastVec2i left, FastVec2i right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FastVec2i left, FastVec2i right)
        {
            return !(left.Equals(right));
        }

        #endregion
    }



}
