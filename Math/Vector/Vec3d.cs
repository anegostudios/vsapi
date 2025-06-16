using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using Vintagestory.API.Client;

#nullable disable

namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// Represents a vector of 3 doubles. Go bug Tyron of you need more utility methods in this class.
    /// </summary>

    [ProtoContract]
    public class Vec3d : IVec3, IEquatable<Vec3d>
    {
        [ProtoMember(1)]
        public double X;
        [ProtoMember(2)]
        public double Y;
        [ProtoMember(3)]
        public double Z;

        [JsonIgnore]
        public BlockPos AsBlockPos { get { return new BlockPos((int)X, (int)Y, (int)Z); } }

        [JsonIgnore]
        public int XInt => (int)X;
        [JsonIgnore]
        public int YInt => (int)Y;
        [JsonIgnore]
        public int ZInt => (int)Z;

        /// <summary>
        /// Create a new instance with x/y/z set to 0
        /// </summary>
        public static Vec3d Zero { get { return new Vec3d(); } }


        public Vec3d()
        {

        }

        public Vec3d(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Create a new vector with given coordinates
        /// </summary>
        /// <param name="vec"></param>
        public Vec3d(Vec4d vec)
        {
            this.X = vec.X;
            this.Y = vec.Y;
            this.Z = vec.Z;
        }

        public Vec3d(Common.Entities.EntityPos vec)
        {
            this.X = vec.X;
            this.Y = vec.Y;
            this.Z = vec.Z;
        }

        /// <summary>
        /// Returns the n-th coordinate
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double this[int index]
        {
            get { return index == 0 ? X : (index == 1 ? Y : Z); }
            set { if (index == 0) X = value; else if (index == 1) Y = value; else Z = value; }
        }



        public double Dot(Vec3d a)
        {
            return X * a.X + Y * a.Y + Z * a.Z;
        }


        public Vec3d Cross(Vec3d vec)
        {
            Vec3d outv = new Vec3d();
            outv.X = Y * vec.Z - Z * vec.Y;
            outv.Y = Z * vec.X - X * vec.Z;
            outv.Z = X * vec.Y - Y * vec.X;
            return outv;
        }

        public void Cross(Vec3d a, Vec3d b)
        {
            X = a.Y * b.Z - a.Z * b.Y;
            Y = a.Z * b.X - a.X * b.Z;
            Z = a.X * b.Y - a.Y * b.X;
        }

        public void Cross(Vec3d a, Vec4d b)
        {
            X = a.Y * b.Z - a.Z * b.Y;
            Y = a.Z * b.X - a.X * b.Z;
            Z = a.X * b.Y - a.Y * b.X;
        }

        public void Negate()
        {
            this.X = -X;
            this.Y = -Y;
            this.Z = -Z;
        }

        public void Cross(Vec4d a, Vec4d b)
        {
            X = a.Y * b.Z - a.Z * b.Y;
            Y = a.Z * b.X - a.X * b.Z;
            Z = a.X * b.Y - a.Y * b.X;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec3d Add(Vec3d a)
        {
            this.X += a.X;
            this.Y += a.Y;
            this.Z += a.Z;
            return this;
        }
        public Vec3d Add(BlockPos a)
        {
            this.X += a.X;
            this.Y += a.InternalY;
            this.Z += a.Z;
            return this;
        }

        public Vec3d Add(Vec3f a)
        {
            this.X += a.X;
            this.Y += a.Y;
            this.Z += a.Z;
            return this;
        }

        public Vec3d AddCopy(Vec3f a)
        {
            return new Vec3d(X + a.X, Y + a.Y, Z + a.Z);
        }

        public Vec3d AddCopy(Vec3d a)
        {
            return new Vec3d(X + a.X, Y + a.Y, Z + a.Z);
        }

        public Vec3d AddCopy(float x, float y, float z)
        {
            return new Vec3d(X + x, Y + y, Z + z);
        }

        public Vec3d AddCopy(double x, double y, double z)
        {
            return new Vec3d(X + x, Y + y, Z + z);
        }

        public Vec3d AddCopy(BlockFacing facing)
        {
            return new Vec3d(X + facing.Normalf.X, Y + facing.Normalf.Y, Z + facing.Normalf.Z);
        }

        public Vec3d AddCopy(BlockPos pos)
        {
            return new Vec3d(X + pos.X, Y + pos.InternalY, Z + pos.Z);
        }

        public Vec3d Mul(double val)
        {
            X *= val;
            Y *= val;
            Z *= val;
            return this;
        }

        public Vec3d Mul(double x, double y, double z)
        {
            X *= x;
            Y *= y;
            Z *= z;
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vec3d Add(double x, double y, double z)
        {
            X += x;
            Y += y;
            Z += z;
            return this;
        }

        public Vec3d Sub(double x, double y, double z)
        {
            X -= x;
            Y -= y;
            Z -= z;
            return this;
        }

        public Vec3d SubCopy(double x, double y, double z)
        {
            return new Vec3d(X - x, Y - y, Z - z);
        }

        public Vec3d SubCopy(Vec3d sub)
        {
            return new Vec3d(X - sub.X, Y - sub.Y, Z - sub.Z);
        }


        public float[] ToFloatArray()
        {
            return new float[] { (float)X, (float)Y, (float)Z };
        }

        public double[] ToDoubleArray()
        {
            return new double[] { X, Y, Z };
        }

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public double HorLength()
        {
            return Math.Sqrt(X * X + Z * Z);
        }

        public double LengthSq()
        {
            return X * X + Y * Y + Z * Z;
        }

        public Vec3d Normalize()
        {
            double length = Length();
            if (length > 0)
            {
                X /= length;
                Y /= length;
                Z /= length;
            }

            return this;
        }

        public Vec3d Clone()
        {
            return new Vec3d(X, Y, Z);
        }

        public Vec3d Sub(Vec3d vec)
        {
            X -= vec.X;
            Y -= vec.Y;
            Z -= vec.Z;
            return this;
        }

        public Vec3d Add(double value)
        {
            X += value;
            Y += value;
            Z += value;
            return this;
        }

        public static Vec3d Add(Vec3d a, Vec3d b)
        {
            return new Vec3d(
                a.X + b.X,
                a.Y + b.Y,
                a.Z + b.Z
            );
        }

        public static Vec3d Sub(Vec3d a, Vec3d b)
        {
            return new Vec3d(
                a.X - b.X,
                a.Y - b.Y,
                a.Z - b.Z
            );
        }

        public Vec3d Sub(BlockPos pos)
        {
            X -= pos.X;
            Y -= pos.InternalY;
            Z -= pos.Z;
            return this;
        }

        /// <summary>
        /// Note: adjusts the calling Vec3d, does not make a copy
        /// </summary>
        public Vec3d Scale(double f)
        {
            X *= f;
            Y *= f;
            Z *= f;
            return this;
        }

        /// <summary>
        /// Note: adjusts the calling Vec3d, does not make a copy
        /// </summary>
        public Vec3d Offset(Vec3d b)
        {
            X += b.X;
            Y += b.Y;
            Z += b.Z;
            return this;
        }

        /// <summary>
        /// Note: adjusts the calling Vec3d, does not make a copy
        /// </summary>
        public Vec3d Offset(double x, double y, double z)
        {
            X += x;
            Y += y;
            Z += z;
            return this;
        }

        public Vec3d OffsetCopy(float x, float y, float z)
        {
            return new Vec3d(
                X + x,
                Y + y,
                Z + z
            );
        }

        public Vec3d OffsetCopy(double x, double y, double z)
        {
            return new Vec3d(
                X + x,
                Y + y,
                Z + z
            );
        }

        public Vec3d Set(Vec3i pos)
        {
            this.X = pos.X;
            this.Y = pos.Y;
            this.Z = pos.Z;
            return this;
        }

        public Vec3d Set(Vec3f pos)
        {
            this.X = pos.X;
            this.Y = pos.Y;
            this.Z = pos.Z;
            return this;
        }

        public Vec3d Set(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            return this;
        }

        public Vec3d Set(Vec3d pos)
        {
            this.X = pos.X;
            this.Y = pos.Y;
            this.Z = pos.Z;
            return this;
        }

        public Vec3d Set(BlockPos pos)
        {
            this.X = pos.X;
            this.Y = pos.Y;
            this.Z = pos.Z;
            return this;
        }

        public Vec3d Set(Common.Entities.EntityPos pos)
        {
            this.X = pos.X;
            this.Y = pos.Y;
            this.Z = pos.Z;
            return this;
        }

        /// <summary>
        /// Include dimension info. We don't always want this, but sometimes we do
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Vec3d SetWithDimension(Common.Entities.EntityPos pos)
        {
            this.X = pos.X;
            this.Y = pos.InternalY;
            this.Z = pos.Z;
            return this;
        }

        public float SquareDistanceTo(float x, float y, float z)
        {
            double dx = X - x;
            double dy = Y - y;
            double dz = Z - z;

            return (float)(dx * dx + dy * dy + dz * dz);
        }

        public float SquareDistanceTo(double x, double y, double z)
        {
            double dx = X - x;
            double dy = Y - y;
            double dz = Z - z;

            return (float)(dx * dx + dy * dy + dz * dz);
        }

        public float SquareDistanceTo(Vec3d pos)
        {
            double dx = X - pos.X;
            double dy = Y - pos.Y;
            double dz = Z - pos.Z;

            return (float)(dx * dx + dy * dy + dz * dz);
        }

        public float SquareDistanceTo(Common.Entities.EntityPos pos)
        {
            double dx = X - pos.X;
            double dy = Y - pos.Y;
            double dz = Z - pos.Z;

            return (float)(dx * dx + dy * dy + dz * dz);
        }

        public float DistanceTo(Vec3d pos)
        {
            double dx = X - pos.X;
            double dy = Y - pos.Y;
            double dz = Z - pos.Z;

            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public float DistanceTo(double x, double y, double z)
        {
            double dx = X - x;
            double dy = Y - y;
            double dz = Z - z;

            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public float HorizontalSquareDistanceTo(Vec3d pos)
        {
            double dx = X - pos.X;
            double dz = Z - pos.Z;

            return (float)(dx * dx + dz * dz);
        }

        public float HorizontalSquareDistanceTo(double x, double z)
        {
            double dx = X - x;
            double dz = Z - z;

            return (float)(dx * dx + dz * dz);
        }

        #region Operators
        public static Vec3d operator -(Vec3d left, Vec3d right)
        {
            return new Vec3d(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public static Vec3d operator +(Vec3d left, Vec3d right)
        {
            return new Vec3d(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public static Vec3d operator +(Vec3d left, Vec3i right)
        {
            return new Vec3d(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public static Vec3d operator -(Vec3d left, float right)
        {
            return new Vec3d(left.X - right, left.Y - right, left.Z - right);
        }


        public static Vec3d operator -(float left, Vec3d right)
        {
            return new Vec3d(left - right.X, left - right.Y, left - right.Z);
        }

        public static Vec3d operator +(Vec3d left, float right)
        {
            return new Vec3d(left.X + right, left.Y + right, left.Z + right);
        }


        public static Vec3d operator *(Vec3d left, float right)
        {
            return new Vec3d(left.X * right, left.Y * right, left.Z * right);
        }

        public static Vec3d operator *(float left, Vec3d right)
        {
            return new Vec3d(left * right.X, left * right.Y, left * right.Z);
        }

        public static Vec3d operator *(Vec3d left, double right)
        {
            return new Vec3d(left.X * right, left.Y * right, left.Z * right);
        }


        public static Vec3d operator *(double left, Vec3d right)
        {
            return new Vec3d(left * right.X, left * right.Y, left * right.Z);
        }


        public static double operator *(Vec3d left, Vec3d right)
        {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
        }

        public static Vec3d operator /(Vec3d left, float right)
        {
            return new Vec3d(left.X / right, left.Y / right, left.Z / right);
        }

        public static Vec3d operator /(Vec3d left, double right)
        {
            return new Vec3d(left.X / right, left.Y / right, left.Z / right);
        }


        public static bool operator ==(Vec3d left, Vec3d right)
        {
            if (object.ReferenceEquals(left, null))
            {
                return object.ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(Vec3d left, Vec3d right)
        {
            return !(left == right);
        }

        #endregion

        public Vec3f ToVec3f()
        {
            return new Vec3f((float)X, (float)Y, (float)Z);
        }

        
        public override string ToString()
        {
            return "x=" + X + ", y=" + Y + ", z=" + Z;
        }


        public void ToBytes(BinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
        }

        public static Vec3d CreateFromBytes(BinaryReader reader)
        {
            return new Vec3d(reader.ReadDouble(), reader.ReadDouble(), reader.ReadDouble());
        }


        public Vec3d RotatedCopy(float yaw)
        {
            Matrixf mat = new Matrixf();
            mat.RotateY(yaw);
            return mat.TransformVector(new Vec4d(X, Y, Z, 0)).XYZ;
        }

        public Vec3d AheadCopy(double offset, float Pitch, float Yaw)
        {
            float cosPitch = GameMath.Cos(Pitch);
            float sinPitch = GameMath.Sin(Pitch);

            float cosYaw = GameMath.Cos(Yaw);
            float sinYaw = GameMath.Sin(Yaw);

            return new Vec3d(X - cosPitch * sinYaw * offset, Y + sinPitch * offset, Z - cosPitch * cosYaw * offset);
        }

        public Vec3d AheadCopy(double offset, double Pitch, double Yaw)
        {
            double cosPitch = Math.Cos(Pitch);
            double sinPitch = Math.Sin(Pitch);

            double cosYaw = Math.Cos(Yaw);
            double sinYaw = Math.Sin(Yaw);

            return new Vec3d(X - cosPitch * sinYaw * offset, Y + sinPitch * offset, Z - cosPitch * cosYaw * offset);
        }


        public Vec3d Ahead(double offset, float Pitch, float Yaw)
        {
            float cosPitch = GameMath.Cos(Pitch);
            float sinPitch = GameMath.Sin(Pitch);

            float cosYaw = GameMath.Cos(Yaw);
            float sinYaw = GameMath.Sin(Yaw);

            X -= cosPitch * sinYaw * offset;
            Y += sinPitch * offset;
            Z -= cosPitch * cosYaw * offset;

            return this;
        }


        public Vec3d Ahead(double offset, double Pitch, double Yaw)
        {
            double cosPitch = Math.Cos(Pitch);
            double sinPitch = Math.Sin(Pitch);

            double cosYaw = Math.Cos(Yaw);
            double sinYaw = Math.Sin(Yaw);

            X -= cosPitch * sinYaw * offset;
            Y += sinPitch * offset;
            Z -= cosPitch * cosYaw * offset;

            return this;
        }

        public bool Equals(Vec3d other, double epsilon)
        {
            return
                Math.Abs(this.X - other.X) < epsilon &&
                Math.Abs(this.Y - other.Y) < epsilon &&
                Math.Abs(this.Z - other.Z) < epsilon
            ;
        }

        public bool Equals(Vec3d other)
        {
            return other != null && X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vec3d other)
            {
                return other != null && X == other.X && Y == other.Y && Z == other.Z;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ((17 * 23 + X.GetHashCode()) * 23 + Y.GetHashCode()) * 23 + Z.GetHashCode();
        }

        int IVec3.XAsInt { get { return (int)X; } }

        int IVec3.YAsInt { get { return (int)Y; } }

        int IVec3.ZAsInt { get { return (int)Z; } }

        double IVec3.XAsDouble { get { return X; } }

        double IVec3.YAsDouble { get { return Y; } }

        double IVec3.ZAsDouble { get { return Z; } }

        float IVec3.XAsFloat { get { return (float)X; } }

        float IVec3.YAsFloat { get { return (float)Y; } }

        float IVec3.ZAsFloat { get { return (float)Z; } }

        public Vec3i AsVec3i => new Vec3i((int)X, (int)Y, (int)Z);
    }
}
