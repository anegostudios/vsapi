using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// A useful data structure when operating with block postions.<br/>
    /// Valuable Hint: Make use of Copy() or the XXXCopy() variants where needed. A common pitfall is writing code like: BlockPos abovePos = pos.Up(); - with this code abovePos and pos will reference to the same object!
    /// </summary>
    [ProtoContract()]
    public class BlockPos : IEquatable<BlockPos>, IVec3
    {
        [ProtoMember(1)]
        public int X;
        [ProtoMember(2)]
        public int Y;
        [ProtoMember(3)]
        public int Z;

        public BlockPos()
        {


        }
        public BlockPos(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// 0 = x, 1 = y, 2 = z
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public int this[int i]
        {
            get { return i == 0 ? X : i == 1 ? Y : Z; }
            set { if (i == 0) X = value; else if (i == 1) Y = value; else Z = value; }
        }

        /// <summary>
        /// Move the position vertically up
        /// </summary>
        /// <param name="dy"></param>
        /// <returns></returns>
        public BlockPos Up(int dy = 1)
        {
            Y += dy;
            return this;
        }

        /// <summary>
        /// Move the position vertically down
        /// </summary>
        /// <param name="dy"></param>
        /// <returns></returns>
        public BlockPos Down(int dy = 1)
        {
            Y -= dy;
            return this;
        }

        public BlockPos Set(Vec3d origin)
        {
            X = (int)origin.X;
            Y = (int)origin.Y;
            Z = (int)origin.Z;
            return this;
        }

        public BlockPos Set(Vec3i pos)
        {
            X = pos.X;
            Y = pos.Y;
            Z = pos.Z;
            return this;
        }



        /// <summary>
        /// Sets XYZ to new vlaues
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public BlockPos Set(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
            return this;
        }

        public BlockPos Set(BlockPos blockPos)
        {
            X = blockPos.X;
            Y = blockPos.Y;
            Z = blockPos.Z;
            return this;
        }

        public void ToBytes(BinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
        }

        public BlockPos West()
        {
            Z -= 1;
            return this;
        }

        public static BlockPos CreateFromBytes(BinaryReader reader)
        {
            return new BlockPos(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
        }

        public BlockPos North()
        {
            X += 1;
            return this;
        }

        public BlockPos East()
        {
            Z += 1;
            return this;
        }

        public BlockPos South()
        {
            X -= 1;
            return this;
        }

        /// <summary>
        /// Creates a copy of this block positions and moves the position by the given length to the west
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public BlockPos WestCopy(int length = 1)
        {
            return new BlockPos(X, Y, Z - length);
        }

      

        /// <summary>
        /// Creates a copy of this block positions and moves the position by the given length to the south
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public BlockPos SouthCopy(int length = 1)
        {
            return new BlockPos(X - length, Y, Z);
        }


        /// <summary>
        /// Creates a copy of this block positions and moves the position by the given length to the east
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public BlockPos EastCopy(int length = 1)
        {
            return new BlockPos(X, Y, Z + length);
        }

        /// <summary>
        /// Creates a copy of this block positions and moves the position by the given length to the north
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public BlockPos NorthCopy(int length = 1)
        {
            return new BlockPos(X + length, Y, Z);
        }

        /// <summary>
        /// Creates a copy of this block positions and moves the position by the given length down
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public BlockPos DownCopy(int length = 1)
        {
            return new BlockPos(X, Y - length, Z);
        }

        /// <summary>
        /// Creates a copy of this block positions and moves the position by the given length up
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public BlockPos UpCopy(int length = 1)
        {
            return new BlockPos(X, Y + length, Z);
        }


        /// <summary>
        /// Creates a copy of this block positions
        /// </summary>
        /// <returns></returns>
        public BlockPos Copy()
        {
            return new BlockPos(X, Y, Z);
        }


        #region Offseting

        /// <summary>
        /// Offsets the position by given xyz
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dz"></param>
        /// <returns></returns>
        public BlockPos Add(float dx, float dy, float dz)
        {
            X += (int)dx;
            Y += (int)dy;
            Z += (int)dz;
            return this;
        }


        /// <summary>
        /// Offsets the position by given xyz
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dz"></param>
        /// <returns></returns>
        public BlockPos Add(int dx, int dy, int dz)
        {
            X += dx;
            Y += dy;
            Z += dz;
            return this;
        }

        /// <summary>
        /// Offsets the position by given xyz vector
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public BlockPos Add(Vec3i vector)
        {
            X += vector.X;
            Y += vector.Y;
            Z += vector.Z;
            return this;
        }


        /// <summary>
        /// Offsets the position by given xyz vector
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public BlockPos Add(BlockPos pos)
        {
            X += pos.X;
            Y += pos.Y;
            Z += pos.Z;
            return this;
        }


        /// <summary>
        /// Offsets the position into the direction of given block face
        /// </summary>
        /// <param name="facing"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public BlockPos Add(BlockFacing facing, int length = 1)
        {
            X += facing.Normali.X * length;
            Y += facing.Normali.Y * length;
            Z += facing.Normali.Z * length;
            return this;
        }

        /// <summary>
        /// Offsets the position into the direction of given block face
        /// </summary>
        /// <param name="facing"></param>
        /// <returns></returns>
        public BlockPos Offset(BlockFacing facing)
        {
            X += facing.Normali.X;
            Y += facing.Normali.Y;
            Z += facing.Normali.Z;
            return this;
        }

        /// <summary>
        /// Creates a copy of this block positions and offsets it by given xyz
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dz"></param>
        /// <returns></returns>
        public BlockPos AddCopy(float dx, float dy, float dz)
        {
            return new BlockPos((int)(X + dx), (int)(Y + dy), (int)(Z + dz));
        }

        /// <summary>
        /// Creates a copy of this block positions and offsets it by given xyz
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dz"></param>
        /// <returns></returns>
        public BlockPos AddCopy(int dx, int dy, int dz)
        {
            return new BlockPos(X + dx, Y + dy, Z + dz);
        }

        /// <summary>
        /// Creates a copy of this block positions and offsets it by given xyz
        /// </summary>
        /// <param name="xyz"></param>
        /// <returns></returns>
        public BlockPos AddCopy(int xyz)
        {
            return new BlockPos(X + xyz, Y + xyz, Z + xyz);
        }


        /// <summary>
        /// Creates a copy of this block positions and offsets it by given xyz
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public BlockPos AddCopy(Vec3i vector)
        {
            return new BlockPos(X + vector.X, Y + vector.Y, Z + vector.Z);
        }

        /// <summary>
        /// Creates a copy of this block positions and offsets it in the direction of given block face
        /// </summary>
        /// <param name="facing"></param>
        /// <returns></returns>
        public BlockPos AddCopy(BlockFacing facing)
        {
            return new BlockPos(X + facing.Normali.X, Y + facing.Normali.Y, Z + facing.Normali.Z);
        }

        /// <summary>
        /// Creates a copy of this block positions and offsets it in the direction of given block face
        /// </summary>
        /// <param name="facing"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public BlockPos AddCopy(BlockFacing facing, int length)
        {
            return new BlockPos(X + facing.Normali.X * length, Y + facing.Normali.Y * length, Z + facing.Normali.Z * length);
        }


        /// <summary>
        /// Substract a position => you'll have the manhatten distance 
        /// </summary>
        /// <param name="pos"></param>
        public BlockPos Sub(BlockPos pos)
        {
            X -= pos.X;
            Y -= pos.Y;
            Z -= pos.Z;
            return this;
        }

        /// <summary>
        /// Substract a position => you'll have the manhatten distance 
        /// </summary>
        /// <param name="pos"></param>
        public BlockPos Sub(int x, int y, int z)
        {
            X -= x;
            Y -= y;
            Z -= z;
            return this;
        }


        /// <summary>
        /// Substract a position => you'll have the manhatten distance 
        /// </summary>
        /// <param name="pos"></param>
        public BlockPos SubCopy(BlockPos pos)
        {
            return new BlockPos(X - pos.X, Y - pos.Y, Z - pos.Z);
        }


        /// <summary>
        /// Creates a copy of this block positions and divides it by given factor
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public BlockPos DivCopy(int factor)
        {
            return new BlockPos(X / factor, Y / factor, Z / factor);
        }

        #endregion

        #region Distance

        /// <summary>
        /// Returns the Euclidean distance to between this and given position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public float DistanceTo(BlockPos pos)
        {
            double dx = pos.X - X;
            double dy = pos.Y - Y;
            double dz = pos.Z - Z;

            return GameMath.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        /// <summary>
        /// Returns the Euclidean distance to between this and given position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public float DistanceTo(double x, double y, double z)
        {
            double dx = x - X;
            double dy = y - Y;
            double dz = z - Z;

            return GameMath.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        /// <summary>
        /// Returns the squared Euclidean distance to between this and given position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public float DistanceSqTo(double x, double y, double z)
        {
            double dx = x - X;
            double dy = y - Y;
            double dz = z - Z;

            return (float)(dx * dx + dy * dy + dz * dz);
        }

        /// <summary>
        /// Returns the squared Euclidean horizontal distance to between this and given position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public float HorDistanceSqTo(double x, double z)
        {
            double dx = x - X;
            double dz = z - Z;

            return (float)(dx * dx + dz * dz);
        }

        /// <summary>
        /// Returns the manhatten distance to given position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public int HorizontalManhattenDistance(BlockPos pos)
        {
            return Math.Abs(X - pos.X) + Math.Abs(Z - pos.Z);
        }


        /// <summary>
        /// Returns the manhatten distance to given position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public int ManhattenDistance(BlockPos pos)
        {
            return Math.Abs(X - pos.X) + Math.Abs(Y - pos.Y) + Math.Abs(Z - pos.Z);
        }


        /// <summary>
        /// Returns the manhatten distance to given position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public int ManhattenDistance(int x, int y, int z)
        {
            return Math.Abs(X - x) + Math.Abs(Y - y) + Math.Abs(Z - z);
        }

        #endregion

        /// <summary>
        /// Creates a new instance of a Vec3d initialized with this position
        /// </summary>
        /// <returns></returns>
        public Vec3d ToVec3d()
        {
            return new Vec3d(X, Y, Z);
        }

        /// <summary>
        /// Creates a new instance of a Vec3i initialized with this position
        /// </summary>
        /// <returns></returns>
        public Vec3i ToVec3i()
        {
            return new Vec3i(X, Y, Z);
        }

        public Vec3f ToVec3f()
        {
            return new Vec3f(X, Y, Z);
        }



        public override string ToString()
        {
            return X + ", " + Y + ", " + Z;
        }

        public override bool Equals(object obj)
        {
            BlockPos pos;
            if (obj is BlockPos)
            {
                pos = (BlockPos)obj;
            } else
            {
                return false;
            }

            return X == pos.X && Y == pos.Y && Z == pos.Z;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + X.GetHashCode();
            hash = hash * 23 + Y.GetHashCode();
            hash = hash * 23 + Z.GetHashCode();
            return hash;
        }

        public bool Equals(BlockPos other)
        {
            return other != null && X == other.X && Y == other.Y && Z == other.Z;
        }

        public int GetHashCode(BlockPos obj)
        {
            int hash = 17;
            hash = hash * 23 + obj.X.GetHashCode();
            hash = hash * 23 + obj.Y.GetHashCode();
            hash = hash * 23 + obj.Z.GetHashCode();
            return hash;
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

        #region Operators
        public static BlockPos operator +(BlockPos left, BlockPos right)
        {
            return new BlockPos(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public static BlockPos operator -(BlockPos left, BlockPos right)
        {
            return new BlockPos(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public static BlockPos operator +(BlockPos left, int right)
        {
            return new BlockPos(left.X + right, left.Y + right, left.Z + right);
        }

        public static BlockPos operator -(BlockPos left, int right)
        {
            return new BlockPos(left.X - right, left.Y - right, left.Z - right);
        }

        public static BlockPos operator *(BlockPos left, int right)
        {
            return new BlockPos(left.X * right, left.Y * right, left.Z * right);
        }

        public static BlockPos operator *(int left, BlockPos right)
        {
            return new BlockPos(left * right.X, left * right.Y, left * right.Z);
        }

        public static BlockPos operator /(BlockPos left, int right)
        {
            return new BlockPos(left.X / right, left.Y / right, left.Z / right);
        }

        public static bool operator ==(BlockPos left, BlockPos right)
        {
            if (object.ReferenceEquals(left, null))
            {
                return object.ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(BlockPos left, BlockPos right)
        {
            return !(left == right);
        }

        #endregion

    }
}
