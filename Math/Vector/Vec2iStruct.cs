using ProtoBuf;
using System;

#nullable disable

namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// Represents a vector of 2 ints. Go bug Tyron if you need more utility methods in this class.
    /// </summary>
    [ProtoContract]
    public struct Vec2iStruct : IEquatable<Vec2iStruct>
    {
        [ProtoMember(1)]
        public int X;
        [ProtoMember(2)]
        public int Y;

        /// <summary>
        /// Will always return a number in the range 0..1023, no matter what the value of X and Y
        /// </summary>
        public int ToInChunkIndex { get { return (Y & 31) * 32 + (X & 31); } }

        public Vec2iStruct()
        {

        }

        public Vec2iStruct(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vec2iStruct(Vec3d pos)
        {
            this.X = (int)pos.X;
            this.Y = (int)pos.Z;
        }

        public bool Equals(Vec2iStruct other)
        {
            return X == other.X && Y == other.Y;
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

        [Obsolete("Use the correctly-spelled ManhattanDistance instead")]
        public int ManhattenDistance(Vec2i point)
        {
            return ManhattanDistance(point);
        }

        public int ManhattanDistance(Vec2i point)
        {
            return Math.Abs(X - point.X) + Math.Abs(Y - point.Y);
        }

        [Obsolete("Use the correctly-spelled ManhattanDistance instead")]
        public int ManhattenDistance(int x, int y)
        {
            return ManhattanDistance(x, y);
        }

        public int ManhattanDistance(int x, int y)
        {
            return Math.Abs(X - x) + Math.Abs(Y - y);
        }

        public Vec2iStruct Set(int x, int y)
        {
            this.X = x;
            this.Y = y;
            return this;
        }

        public Vec2iStruct Set(Vec2i vec)
        {
            this.X = vec.X;
            this.Y = vec.Y;
            return this;
        }

        public override string ToString()
        {
            return X + " / " + Y;
        }

        public Vec2iStruct Add(int dx, int dy)
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
        public static Vec2iStruct operator -(Vec2iStruct left, Vec2iStruct right)
        {
            return new Vec2iStruct(left.X - right.X, left.Y - right.Y);
        }

        public static Vec2iStruct operator +(Vec2iStruct left, Vec2iStruct right)
        {
            return new Vec2iStruct(left.X + right.X, left.Y + right.Y);
        }

        public static Vec2iStruct operator -(Vec2iStruct left, int right)
        {
            return new Vec2iStruct(left.X - right, left.Y - right);
        }


        public static Vec2iStruct operator -(int left, Vec2iStruct right)
        {
            return new Vec2iStruct(left - right.X, left - right.Y);
        }

        public static Vec2iStruct operator +(Vec2iStruct left, int right)
        {
            return new Vec2iStruct(left.X + right, left.Y + right);
        }


        public static Vec2iStruct operator *(Vec2iStruct left, int right)
        {
            return new Vec2iStruct(left.X * right, left.Y * right);
        }

        public static Vec2iStruct operator *(int left, Vec2iStruct right)
        {
            return new Vec2iStruct(left * right.X, left * right.Y);
        }

        public static Vec2iStruct operator *(Vec2iStruct left, double right)
        {
            return new Vec2iStruct((int)(left.X * right), (int)(left.Y * right));
        }


        public static Vec2iStruct operator *(double left, Vec2iStruct right)
        {
            return new Vec2iStruct((int)(left * right.X), (int)(left * right.Y));
        }


        public static double operator *(Vec2iStruct left, Vec2iStruct right)
        {
            return left.X * right.X + left.Y * right.Y;
        }

        public static Vec2iStruct operator /(Vec2iStruct left, int right)
        {
            return new Vec2iStruct(left.X / right, left.Y / right);
        }

        public static Vec2iStruct operator /(Vec2iStruct left, float right)
        {
            return new Vec2iStruct((int)(left.X / right), (int)(left.Y / right));
        }


        public static bool operator ==(Vec2iStruct left, Vec2iStruct right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vec2iStruct left, Vec2iStruct right)
        {
            return !(left == right);
        }

        #endregion
    }



}
