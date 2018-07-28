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
    public class Vec2i : IEquatable<Vec2i>
    {
        public int X;
        public int Y;

        public Vec2i()
        {

        }

        public Vec2i(int x, int y)
        {
            this.X = x;
            this.Y = y;
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
    }



}
