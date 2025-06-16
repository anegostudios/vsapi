using System;

#nullable disable

namespace Vintagestory.API.MathTools
{
    public class Vec4f
    {
        public float X;
        public float Y;
        public float Z;
        public float W;


        /// <summary>
        /// Synonum for X
        /// </summary>
        public float R { get { return X; } set { X = value; } }
        /// <summary>
        /// Synonum for Y
        /// </summary>
        public float G { get { return Y; } set { Y = value; } }
        /// <summary>
        /// Synonum for Z
        /// </summary>
        public float B { get { return Z; } set { Z = value; } }
        /// <summary>
        /// Synonum for W
        /// </summary>
        public float A { get { return W; } set { W = value; } }


        public Vec3f XYZ { get { return new Vec3f(X, Y, Z); } }

        public Vec4f()
        {

        }

        public Vec4f(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        /// <summary>
        /// Returns the n-th coordinate
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float this[int index]
        {
            get { return index == 0 ? X : (index == 1 ? Y : (index == 2 ? Z : W)); }
            set { if (index == 0) X = value; else if (index == 1) Y = value; else if (index == 2) Z = value; else W = value; }
        }

        public Vec4f Set(float[] vec)
        {
            this.X = vec[0];
            this.Y = vec[1];
            this.Z = vec[2];
            this.W = vec[3];
            return this;
        }

        public Vec4f Set(Vec4f vec)
        {
            this.X = vec.X;
            this.Y = vec.Y;
            this.Z = vec.Z;
            this.W = vec.W;
            return this;
        }

        public Vec4f Mul(Vec4f vec)
        {
            this.X *= vec.X;
            this.Y *= vec.Y;
            this.Z *= vec.Z;
            this.W *= vec.W;
            return this;
        }

        public Vec4f Set(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
            return this;
        }

        public Vec4f Clone()
        {
            return new Vec4f(X, Y, Z, W);
        }


        /// <summary>
        /// Turns the vector into a unit vector with length 1, but only if length is non-zero
        /// </summary>
        /// <returns></returns>
        public Vec4f NormalizeXYZ()
        {
            float length = LengthXYZ();

            if (length > 0)
            {
                X /= length;
                Y /= length;
                Z /= length;
            }

            return this;
        }



        /// <summary>
        /// Returns the length of this vector
        /// </summary>
        /// <returns></returns>
        public float LengthXYZ()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }


        #region Operators
        public static Vec4f operator -(Vec4f left, Vec4f right)
        {
            return new Vec4f(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.W - right.W);
        }

        public static Vec4f operator +(Vec4f left, Vec4f right)
        {
            return new Vec4f(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);
        }

        public static Vec4f operator +(Vec4f left, Vec4i right)
        {
            return new Vec4f(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.W + right.W);
        }

        public static Vec4f operator -(Vec4f left, float right)
        {
            return new Vec4f(left.X - right, left.Y - right, left.Z - right, left.W - right);
        }


        public static Vec4f operator -(float left, Vec4f right)
        {
            return new Vec4f(left - right.X, left - right.Y, left - right.Z, left - right.W);
        }

        public static Vec4f operator +(Vec4f left, float right)
        {
            return new Vec4f(left.X + right, left.Y + right, left.Z + right, left.W + right);
        }


        public static Vec4f operator *(Vec4f left, float right)
        {
            return new Vec4f(left.X * right, left.Y * right, left.Z * right, left.W * right);
        }

        public static Vec4f operator *(float left, Vec4f right)
        {
            return new Vec4f(left * right.X, left * right.Y, left * right.Z, left * right.W);
        }

        public static double operator *(Vec4f left, Vec4f right)
        {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z + left.W * right.W;
        }

        public static Vec4f operator /(Vec4f left, float right)
        {
            return new Vec4f(left.X / right, left.Y / right, left.Z / right, left.W / right);
        }

        #endregion

    }
}
