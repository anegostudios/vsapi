using ProtoBuf;
using System;
using System.Collections.Generic;

namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// Represents a vector of 3 ints. Go bug Tyron if you need more utility methods in this class.
    /// </summary>
    [ProtoContract]
    public class Vec3i : IEquatable<Vec3i>, IVec3
    {
        [ProtoMember(1)]
        public int X;
        [ProtoMember(2)]
        public int Y;
        [ProtoMember(3)]
        public int Z;

        /// <summary>
        /// List of offset of all direct and indirect neighbours of coordinate 0/0/0
        /// </summary>
        public static readonly Vec3i[] DirectAndIndirectNeighbours;


        static Vec3i()
        {
            List<Vec3i> allNeighbours = new List<Vec3i>();
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dz = -1; dz <= 1; dz++)
                    {
                        if (dx == 0 && dy == 0 && dz == 0) continue;

                        allNeighbours.Add(new Vec3i(dx, dy, dz));
                    }
                }
            }

            DirectAndIndirectNeighbours = allNeighbours.ToArray();
        }

        public BlockPos AsBlockPos
        {
            get { return new BlockPos(X, Y, Z); }
        }

        public Vec3i()
        {

        }

        public Vec3i(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vec3i(BlockPos pos)
        {
            X = pos.X;
            Y = pos.Y;
            Z = pos.Z;
        }

        public Vec3i Add(int x, int y, int z)
        {
            X += x;
            Y += y;
            Z += z;
            return this;
        }

        public Vec3i AddCopy(int x, int y, int z)
        {
            return new Vec3i(X + x, Y + y, Z + z);
        }

        public Vec3i Add(int x, int y, int z, Vec3i intoVec)
        {
            intoVec.X = X + x;
            intoVec.Y = Y + y;
            intoVec.Z = Z + z;
            return this;
        }

        public Vec3i Add(BlockFacing towardsFace)
        {
            X += towardsFace.Normali.X;
            Y += towardsFace.Normali.Y;
            Z += towardsFace.Normali.Z;
            return this;
        }

        public int ManhattenDistanceTo(Vec3i vec)
        {
            return Math.Abs(X - vec.X) + Math.Abs(Y - vec.Y) + Math.Abs(Z - vec.Z);
        }

        public long SquareDistanceTo(Vec3i vec)
        {
            long dx = X - vec.X;
            long dy = Y - vec.Y;
            long dz = Z - vec.Z;

            return dx * dx + dy * dy + dz * dz;
        }


        public long SquareDistanceTo(int x, int y, int z)
        {
            long dx = X - x;
            long dy = Y - y;
            long dz = Z - z;

            return dx * dx + dy * dy + dz * dz;
        }

        public double DistanceTo(Vec3i vec)
        {
            long dx = X - vec.X;
            long dy = Y - vec.Y;
            long dz = Z - vec.Z;

            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }


        /// <summary>
        /// Substracts val from each coordinate if the coordinate if positive, otherwise it is added. If 0, the value is unchanged. The value must be a positive number
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public void Reduce(int val = 1)
        {
            X = X > 0 ? Math.Max(0, X - val) : Math.Min(0, X + val);
            Y = Y > 0 ? Math.Max(0, Y - val) : Math.Min(0, Y + val);
            Z = Z > 0 ? Math.Max(0, Z - val) : Math.Min(0, Z + val);
        }

        public void ReduceX(int val = 1)
        {
            X = X > 0 ? Math.Max(0, X - val) : Math.Min(0, X + val);
        }

        public void ReduceY(int val = 1)
        {
            Y = Y > 0 ? Math.Max(0, Y - val) : Math.Min(0, Y + val);
        }

        public void ReduceZ(int val = 1)
        {
            Z = Z > 0 ? Math.Max(0, Z - val) : Math.Min(0, Z + val);
        }


        public static Vec3i operator *(Vec3i left, int right)
        {
            return new Vec3i(left.X * right, left.Y * right, left.Z * right);
        }

        public static Vec3i operator *(int left, Vec3i right)
        {
            return new Vec3i(left * right.X, left * right.Y, left * right.Z);
        }

        public static Vec3i operator /(Vec3i left, int right)
        {
            return new Vec3i(left.X / right, left.Y / right, left.Z / right);
        }

        public Vec3i Set(int positionX, int positionY, int positionZ)
        {
            this.X = positionX;
            this.Y = positionY;
            this.Z = positionZ;
            return this;
        }

        public Vec3i Set(Vec3i fromPos)
        {
            this.X = fromPos.X;
            this.Y = fromPos.Y;
            this.Z = fromPos.Z;
            return this;
        }

        internal void Offset(BlockFacing face)
        {
            this.X += face.Normali.X;
            this.Y += face.Normali.Y;
            this.Z += face.Normali.Z;
        }



        public Vec3i Clone()
        {
            return (Vec3i)MemberwiseClone();
        }

        public override bool Equals(object obj)
        {
            Vec3i pos;
            if (obj is Vec3i)
            {
                pos = (Vec3i)obj;
            }
            else
            {
                return false;
            }

            return X == pos.X && Y == pos.Y && Z == pos.Z;
        }

        public bool Equals(Vec3i other)
        {
            return other != null && X == other.X && Y == other.Y && Z == other.Z;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + X.GetHashCode();
            hash = hash * 23 + Y.GetHashCode();
            hash = hash * 23 + Z.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return "X=" + X + ",Y=" + Y + ",Z=" + Z;
        }

        internal float[] ToFloats()
        {
            return new float[]
            {
                X, Y, Z
            };
        }
        
        /// <summary>
        /// 27 lowest bits for X Coordinate, then 27 bits for Z coordinate and the highest 10 bits for Y coordinate
        /// </summary>
        /// <returns></returns>
        public ulong ToChunkIndex()
        {
            return ((ulong)Y << 54) | ((ulong)Z << 27) | (uint)X;
        }

        static ulong chunkMask = 134217728 - 1;   // 2^27 - 1

        
        public static Vec3i FromChunkIndex(ulong index)
        {
            return new Vec3i(
                (int)(index & chunkMask),
                (int)(index >> 54),
                (int)((index >> 27) & chunkMask)
            );
        }

        public static ulong ToChunkIndex(int x, int y, int z)
        {
            return ((ulong)y << 54) | ((ulong)z << 27) | (uint)x;
        }

        int IVec3.XAsInt { get { return X; } }

        int IVec3.YAsInt { get { return Y; } }

        int IVec3.ZAsInt { get { return Z; } }

        double IVec3.XAsDouble { get { return X; } }

        double IVec3.YAsDouble { get { return Y; } }
        
        double IVec3.ZAsDouble { get { return Z; } }

        float IVec3.XAsFloat { get { return X; } }

        float IVec3.YAsFloat { get { return Y; } }

        float IVec3.ZAsFloat { get { return Z; } }

        public static Vec3i Zero => new Vec3i(0, 0, 0);

        public Vec3i AddCopy(BlockFacing facing)
        {
            return new Vec3i(X + facing.Normali.X, Y + facing.Normali.Y, Z + facing.Normali.Z);
        }

        public BlockPos ToBlockPos()
        {
            return new BlockPos()
            {
                X = this.X,
                Y = this.Y,
                Z = this.Z
            };
        }

        public bool Equals(int x, int y, int z)
        {
            return this.X == x && this.Y == y && this.Z == z;
        }

    }
}
