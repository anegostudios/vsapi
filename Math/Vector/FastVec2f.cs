using System;

#nullable disable

namespace Vintagestory.API.MathTools
{
    public struct FastVec2f
    {
        const float tinyEpsilon = 1e-20f;
        public static readonly FastVec2f Zero = new FastVec2f(0, 0);

        public float X;
        public float Y;

        public float A => X;
        public float B => Y;
        /// <summary>Identical to .Y - optionally, you can use .Z instead for code readability if the vector represents X,Z values</summary>
        public float Z => Y;

        public FastVec2f()
        {

        }

        public FastVec2f(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public FastVec2f(Size2i size)
        {
            this.X = size.Width;
            this.Y = size.Height;
        }




        public override string ToString()
        {
            return "X=" + X + ", Y=" + Y;
        }

        public float Length()
        {
            return GameMath.Sqrt(X * X + Y * Y);
        }

        public float LengthSq()
        {
            return (X * X + Y * Y);
        }

        public float DistanceTo(float x, float y)
        {
            float dx = X - x;
            float dy = Y - y;
            return GameMath.Sqrt(dx * dx + dy * dy);
        }

        public float DistanceTo(FastVec2f p)
        {
            float dx = X - p.X;
            float dy = Y - p.Y;
            return GameMath.Sqrt(dx * dx + dy * dy);
        }


        public static float Distance(float x1, float y1, float x2, float y2)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;

            return GameMath.Sqrt(dx * dx + dy * dy);
        }


        public static float Dot(FastVec2f a, FastVec2f b)
        {
            return a.X * b.X + a.Y * b.Y;
        }


        public FastVec2f Normalize()
        {
            float ls = LengthSq();
            if (ls < tinyEpsilon) return new FastVec2f(1, 0);
            return this / GameMath.Sqrt(ls);
        }


        public FastVec2f LeftNormal()
        {
            return new FastVec2f(-Y, X);
        }

        public FastVec2f Rotate(float angleRad)
        {
            float c = GameMath.Cos(angleRad);
            float s = GameMath.Sin(angleRad);
            return new FastVec2f(c * X - s * Y, s * X + c * Y);
        }


        public override bool Equals(object obj)
        {
            return obj is FastVec2f f &&
                   X == f.X &&
                   Y == f.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        #region Operators
        public static FastVec2f operator -(FastVec2f left, FastVec2f right)
        {
            return new FastVec2f(left.X - right.X, left.Y - right.Y);
        }

        public static FastVec2f operator +(FastVec2f left, FastVec2f right)
        {
            return new FastVec2f(left.X + right.X, left.Y + right.Y);
        }


        public static FastVec2f operator -(FastVec2f left, float right)
        {
            return new FastVec2f(left.X - right, left.Y - right);
        }

        public static FastVec2f operator -(float left, FastVec2f right)
        {
            return new FastVec2f(left - right.X, left - right.Y);
        }

        public static FastVec2f operator +(FastVec2f left, float right)
        {
            return new FastVec2f(left.X + right, left.Y + right);
        }


        public static FastVec2f operator *(FastVec2f left, float right)
        {
            return new FastVec2f(left.X * right, left.Y * right);
        }

        public static FastVec2f operator *(float left, FastVec2f right)
        {
            return new FastVec2f(left * right.X, left * right.Y);
        }

        public static FastVec2f operator *(FastVec2f left, double right)
        {
            return new FastVec2f(left.X * (float)right, left.Y * (float)right);
        }

        public static FastVec2f operator *(double left, FastVec2f right)
        {
            return new FastVec2f((float)left * right.X, (float)left * right.Y);
        }


        public static double operator *(FastVec2f left, FastVec2f right)
        {
            return left.X * right.X + left.Y * right.Y;
        }

        public static FastVec2f operator /(FastVec2f left, float right)
        {
            return new FastVec2f(left.X / right, left.Y / right);
        }


        public static FastVec2f operator +(FastVec2f left, Vec2i right)
        {
            return new FastVec2f(left.X + right.X, left.Y + right.Y);
        }

        public static bool operator ==(FastVec2f left, FastVec2f right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FastVec2f left, FastVec2f right)
        {
            return !(left == right);
        }

        #endregion
    }



}
