using System;
using System.IO;

namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// Represents a vector of 3 floats. Go bug Tyron of you need more utility methods in this class.
    /// </summary>
    public struct FastVec3f
    {
        /// <summary>
        /// The X-Component of the vector
        /// </summary>
        public float X;
        /// <summary>
        /// The Y-Component of the vector
        /// </summary>
        public float Y;
        /// <summary>
        /// The Z-Component of the vector
        /// </summary>
        public float Z;


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
        /// Create a new vector with given coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public FastVec3f(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// Create a new vector with given coordinates
        /// </summary>
        /// <param name="vec"></param>
        public FastVec3f(Vec4f vec)
        {
            this.X = vec.X;
            this.Y = vec.Y;
            this.Z = vec.Z;
        }

        /// <summary>
        /// Create a new vector with given coordinates
        /// </summary>
        /// <param name="values"></param>
        public FastVec3f(float[] values)
        {
            this.X = values[0];
            this.Y = values[1];
            this.Z = values[2];
        }

        public FastVec3f(Vec3i vec3i)
        {
            this.X = vec3i.X;
            this.Y = vec3i.Y;
            this.Z = vec3i.Z;
        }

        /// <summary>
        /// Returns the n-th coordinate
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public float this[int index]
        {
            get { return ((2 - index) / 2) * X + (index % 2) * Y + (index / 2) * Z; }   // branch-free code to result in X if index is 0, Y if index is 1, Z if index is 2
            set { if (index == 0) X = value; else if (index == 1) Y = value; else Z = value; }
        }


        /// <summary>
        /// Returns the length of this vector
        /// </summary>
        /// <returns></returns>
        public float Length()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public void Negate()
        {
            this.X = -X;
            this.Y = -Y;
            this.Z = -Z;
        }



        /// <summary>
        /// Returns the dot product with given vector
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public float Dot(Vec3f a)
        {
            return X * a.X + Y * a.Y + Z * a.Z;
        }

        /// <summary>
        /// Returns the dot product with given vector
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public float Dot(Vec3d a)
        {
            return (float)(X * a.X + Y * a.Y + Z * a.Z);
        }

        /// <summary>
        /// Returns the dot product with given vector
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public double Dot(float[] pos)
        {
            return X * pos[0] + Y * pos[1] + Z * pos[2];
        }

        /// <summary>
        /// Returns the dot product with given vector
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public double Dot(double[] pos)
        {
            return (float)(X * pos[0] + Y * pos[1] + Z * pos[2]);
        }

        public double[] ToDoubleArray()
        {
            return new double[] { X, Y, Z };
        }

        /// <summary>
        /// Adds given x/y/z coordinates to the vector
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public FastVec3f Add(float x, float y, float z)
        {
            this.X += x;
            this.Y += y;
            this.Z += z;
            return this;
        }

        /// <summary>
        /// Multiplies each coordinate with given multiplier
        /// </summary>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        public FastVec3f Mul(float multiplier)
        {
            this.X *= multiplier;
            this.Y *= multiplier;
            this.Z *= multiplier;
            return this;
        }

        /// <summary>
        /// Creates a copy of the vetor
        /// </summary>
        /// <returns></returns>
        public FastVec3f Clone()
        {
            return (FastVec3f)MemberwiseClone();
        }

        /// <summary>
        /// Turns the vector into a unit vector with length 1, but only if length is non-zero
        /// </summary>
        /// <returns></returns>
        public FastVec3f Normalize()
        {
            float length = Length();

            if (length > 0)
            {
                X /= length;
                Y /= length;
                Z /= length;
            }

            return this;
        }

        /// <summary>
        /// Calculates the distance the two endpoints
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public float Distance(FastVec3f vec)
        {
            return (float)Math.Sqrt(
                (X - vec.X) * (X - vec.X) +
                (Y - vec.Y) * (Y - vec.Y) +
                (Z - vec.Z) * (Z - vec.Z)
            );
        }


        /// <summary>
        /// Calculates the square distance the two endpoints
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public double DistanceSq(double x, double y, double z)
        {
            return 
                (X - x) * (X - x) +
                (Y - y) * (Y - y) +
                (Z - z) * (Z - z)
            ;
        }


        /// <summary>
        /// Calculates the distance the two endpoints
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public float Distance(Vec3d vec)
        {
            return (float)Math.Sqrt(
                (X - vec.X) * (X - vec.X) +
                (Y - vec.Y) * (Y - vec.Y) +
                (Z - vec.Z) * (Z - vec.Z)
            );
        }

        /// <summary>
        /// Adds given coordinates to a new vectors and returns it. The original calling vector remains unchanged
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public FastVec3f AddCopy(float x, float y, float z)
        {
            return new FastVec3f(X + x, Y + y, Z + z);
        }

        /// <summary>
        /// Adds both vectors into a new vector. Both source vectors remain unchanged.
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public FastVec3f AddCopy(FastVec3f vec)
        {
            return new FastVec3f(X + vec.X, Y + vec.Y, Z + vec.Z);
        }


        /// <summary>
        /// Substracts val from each coordinate if the coordinate if positive, otherwise it is added. If 0, the value is unchanged. The value must be a positive number
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public void ReduceBy(float val)
        {
            X = X > 0f ? Math.Max(0f, X - val) : Math.Min(0f, X + val);
            Y = Y > 0f ? Math.Max(0f, Y - val) : Math.Min(0f, Y + val);
            Z = Z > 0f ? Math.Max(0f, Z - val) : Math.Min(0f, Z + val);
        }

        /// <summary>
        /// Creates a new vectors that is the normalized version of this vector. 
        /// </summary>
        /// <returns></returns>
        public FastVec3f NormalizedCopy()
        {
            float length = Length();
            return new FastVec3f(
                  X / length,
                  Y / length,
                  Z / length
            );
        }

        /// <summary>
        /// Creates a new double precision vector with the same coordinates
        /// </summary>
        /// <returns></returns>
        public Vec3d ToVec3d()
        {
            return new Vec3d(X, Y, Z);
        }

        /// <summary>
        /// Sets the vector to this coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public FastVec3f Set(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            return this;
        }

        /// <summary>
        /// Sets the vector to the coordinates of given vector
        /// </summary>
        /// <param name="vec"></param>
        public FastVec3f Set(Vec3d vec)
        {
            this.X = (float)vec.X;
            this.Y = (float)vec.Y;
            this.Z = (float)vec.Z;
            return this;
        }

        public FastVec3f Set(float[] vec)
        {
            this.X = vec[0];
            this.Y = vec[1];
            this.Z = vec[2];
            return this;
        }

        /// <summary>
        /// Sets the vector to the coordinates of given vector
        /// </summary>
        /// <param name="vec"></param>
        public void Set(FastVec3f vec)
        {
            this.X = (float)vec.X;
            this.Y = (float)vec.Y;
            this.Z = (float)vec.Z;
        }

        /// <summary>
        /// Simple string represenation of the x/y/z components
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "x=" + X + ", y=" + Y + ", z=" + Z;
        }


        public void Write(BinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
        }

        public static FastVec3f CreateFromBytes(BinaryReader reader)
        {
            return new FastVec3f(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }
    }
}
