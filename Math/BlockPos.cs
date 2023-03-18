using System;
using System.IO;
using ProtoBuf;
using Vintagestory.API.Common;

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

        public BlockPos(Vec3i vec)
        {
            this.X = vec.X;
            this.Y = vec.Y;
            this.Z = vec.Z;
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

        public BlockPos Set(float x, float y, float z)
        {
            X = (int)x;
            Y = (int)y;
            Z = (int)z;
            return this;
        }

        public BlockPos Set(BlockPos blockPos)
        {
            X = blockPos.X;
            Y = blockPos.Y;
            Z = blockPos.Z;
            return this;
        }

        /// <summary>
        /// Sets this BlockPos to the x,y,z values given, and returns a boolean stating if the existing values were already equal to x,y,z
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns>Returns true if the BlockPos already held these exact x, y, z values (the .Set operation has not changed anything)<br/>Returns false if the .Set operation caused a change to the BlockPos</returns>
        public bool SetAndEquals(int x, int y, int z)
        {
            if (X == x && Z == z && Y == y) return true;
            X = x;
            Y = y;
            Z = z;
            return false;
        }

        public void ToBytes(BinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
        }

        /// <summary>
        /// Convert a block position to coordinates relative to the world spawn position
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        public Vec3i ToLocalPosition(ICoreAPI api)
        {
            return new Vec3i(X - api.World.DefaultSpawnPosition.XInt, Y, Z - api.World.DefaultSpawnPosition.ZInt);
        }

        public BlockPos West()
        {
            X -= 1;
            return this;
        }

        public static BlockPos CreateFromBytes(BinaryReader reader)
        {
            return new BlockPos(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
        }

        public BlockPos North()
        {
            Z -= 1;
            return this;
        }

        public BlockPos East()
        {
            X += 1;
            return this;
        }

        public BlockPos South()
        {
            Z += 1;
            return this;
        }

        /// <summary>
        /// Returns the direction moved from the other blockPos, to get to this BlockPos
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public BlockFacing FacingFrom(BlockPos other)
        {
            int dx = other.X - X;
            int dy = other.Y - Y;
            int dz = other.Z - Z;

            if (dx * dx >= dz * dz)
            {
                if (dx * dx >= dy * dy) return dx > 0 ? BlockFacing.WEST : BlockFacing.EAST;
            }
            else
            {
                if (dz * dz >= dy * dy) return dz > 0 ? BlockFacing.NORTH : BlockFacing.SOUTH;
            }
            return dy > 0 ? BlockFacing.DOWN : BlockFacing.UP;
        }

        /// <summary>
        /// Creates a copy of this blocks position with the x-position adjusted by -<paramref name="length"/>
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public BlockPos WestCopy(int length = 1)
        {
            return new BlockPos(X - length, Y, Z);
        }



        /// <summary>
        /// Creates a copy of this blocks position with the z-position adjusted by +<paramref name="length"/>
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public BlockPos SouthCopy(int length = 1)
        {
            return new BlockPos(X, Y, Z + length);
        }


        /// <summary>
        /// Creates a copy of this blocks position with the x-position adjusted by +<paramref name="length"/>
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public BlockPos EastCopy(int length = 1)
        {
            return new BlockPos(X + length, Y, Z);
        }

        /// <summary>
        /// Creates a copy of this blocks position with the z-position adjusted by -<paramref name="length"/>
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public BlockPos NorthCopy(int length = 1)
        {
            return new BlockPos(X, Y, Z - length);
        }

        /// <summary>
        /// Creates a copy of this blocks position with the y-position adjusted by -<paramref name="length"/>
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public BlockPos DownCopy(int length = 1)
        {
            return new BlockPos(X, Y - length, Z);
        }

        /// <summary>
        /// Creates a copy of this blocks position with the y-position adjusted by +<paramref name="length"/>
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public BlockPos UpCopy(int length = 1)
        {
            return new BlockPos(X, Y + length, Z);
        }


        /// <summary>
        /// Creates a copy of this blocks position
        /// </summary>
        /// <returns></returns>
        public virtual BlockPos Copy()
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
        /// Creates a copy of this blocks position and offsets it by given xyz
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
        /// Creates a copy of this blocks position and offsets it by given xyz
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
        /// Creates a copy of this blocks position and offsets it by given xyz
        /// </summary>
        /// <param name="xyz"></param>
        /// <returns></returns>
        public BlockPos AddCopy(int xyz)
        {
            return new BlockPos(X + xyz, Y + xyz, Z + xyz);
        }


        /// <summary>
        /// Creates a copy of this blocks position and offsets it by given xyz
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public BlockPos AddCopy(Vec3i vector)
        {
            return new BlockPos(X + vector.X, Y + vector.Y, Z + vector.Z);
        }

        /// <summary>
        /// Creates a copy of this blocks position and offsets it in the direction of given block face
        /// </summary>
        /// <param name="facing"></param>
        /// <returns></returns>
        public BlockPos AddCopy(BlockFacing facing)
        {
            return new BlockPos(X + facing.Normali.X, Y + facing.Normali.Y, Z + facing.Normali.Z);
        }

        /// <summary>
        /// Creates a copy of this blocks position and offsets it in the direction of given block face
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
        /// Creates a copy of this blocks position and divides it by given factor
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
        /// Returns the squared Euclidean distance between the nearer edge of this blockpos (assumed 1 x 0.75 x 1 cube) and given position
        /// The 0.75 offset is because the "heat source" is likely to be above the base position of this block: it's approximate
        /// </summary>
        public double DistanceSqToNearerEdge(double x, double y, double z)
        {
            double dx = x - X;
            double dy = y - Y - 0.75;
            double dz = z - Z;
            if (dx > 0) dx = dx <= 1 ? 0 : dx - 1;
            if (dz > 0) dz = dz <= 1 ? 0 : dz - 1;

            return dx * dx + dy * dy + dz * dz;
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

        /// <summary>
        /// Returns true if the specified x,z is within a box the specified range around this position
        /// </summary>
        public bool InRangeHorizontally(int x, int z, int range)
        {
            return Math.Abs(X - x) <= range && Math.Abs(Z - z) <= range;
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
            return (obj is BlockPos pos) && X == pos.X && Y == pos.Y && Z == pos.Z;
        }

        public override int GetHashCode()
        {
            return ((17 * 23 + X) * 23 + Y) * 23 + Z;
        }
        

        public bool Equals(BlockPos other)
        {
            return other != null && X == other.X && Y == other.Y && Z == other.Z;
        }

        public bool Equals(int x, int y, int z)
        {
            return X == x && Y == y && Z == z;
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

    // Exactly like BlockPos except using this class signifies the block should be looked for in the fluids layer; used for server block ticking
    public class FluidBlockPos : BlockPos
    {
        public FluidBlockPos()
        {
        }

        public FluidBlockPos(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public FluidBlockPos(Vec3i vec)
        {
            this.X = vec.X;
            this.Y = vec.Y;
            this.Z = vec.Z;
        }

        public override BlockPos Copy()
        {
            return new FluidBlockPos(X, Y, Z);
        }
    }
}
