using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// Represents a vector of 2 ints. Go bug Tyron if you need more utility methods in this class.
    /// </summary>
    public readonly struct Vec2iImmutable : IEquatable<Vec2i>, IEquatable<Vec2iImmutable>
    {
        private readonly int x;
        private readonly int y;

        public int X { get => x; }
        public int Y { get => y; }

        public static Vec2iImmutable Zero => new Vec2iImmutable(0, 0);

        public Vec2iImmutable(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vec2iImmutable(Vec3d pos)
        {
            this.x = (int)pos.X;
            this.y = (int)pos.Z;
        }

        public bool Equals(Vec2i other)
        {
            return other != null && x == other.X && y == other.Y;
        }

        public bool Equals(Vec2iImmutable other)
        {
            return x == other.x && y == other.y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vec2iImmutable vec)
            {
                return x == vec.x && y == vec.y;
            }
            else if (obj is Vec2i other)
            {
                return other != null && x == other.X && y == other.Y;
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + x.GetHashCode();
            hash = hash * 23 + y.GetHashCode();
            return hash;
        }

        public int this[int index]
        {
            get { return index == 0 ? x : y; }
        }

        public int ManhattenDistance(Vec2iImmutable point)
        {
            return Math.Abs(x - point.X) + Math.Abs(y - point.Y);
        }

        public int ManhattenDistance(Vec2i point)
        {
            return Math.Abs(x - point.X) + Math.Abs(y - point.Y);
        }

        public int ManhattenDistance(int x, int y)
        {
            return Math.Abs(this.x - x) + Math.Abs(this.y - y);
        }

        public override string ToString()
        {
            return x + " / " + y;
        }

        public Vec2iImmutable Add(int dx, int dy)
        {
            return new Vec2iImmutable(x + dx, y + dy);
        }


        /// <summary>
        /// 27 lowest bits for X Coordinate, then 27 bits for Z coordinate
        /// </summary>
        /// <returns></returns>
        public ulong ToChunkIndex()
        {
            return ((ulong)y << 27) | (uint)x;
        }
    }
}
