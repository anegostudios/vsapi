using System;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using ProtoBuf;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

#nullable disable

namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// A useful data structure when operating with block postions.<br/>
    /// Valuable Hint: Make use of Copy() or the XXXCopy() variants where needed. A common pitfall is writing code like: BlockPos abovePos = pos.Up(); - with this code abovePos and pos will reference to the same object!
    /// </summary>
    [ProtoContract()]
    [JsonObject(MemberSerialization.OptIn)]
    public class BlockPos : IEquatable<BlockPos>, IVec3
    {
        [ProtoMember(1)]
        [JsonProperty]
        public int X;

        [ProtoMember(2)]
        [JsonProperty]
        public int InternalY {
            get { return Y + dimension * DimensionBoundary; }
            set { Y = value % DimensionBoundary; dimension = value / DimensionBoundary; }
        }

        [ProtoMember(3)]
        [JsonProperty]
        public int Z;

        public int Y;
        public int dimension;

        public const int DimensionBoundary = GlobalConstants.DimensionSizeInChunks * GlobalConstants.ChunkSize;


        [Obsolete("Not dimension-aware. Use new BlockPos(dimensionId) where possible")]
        public BlockPos() { }


        public BlockPos(int dim)
        {
            this.dimension = dim;
        }


        /// <summary>
        /// The new BlockPos takes its dimension from the supplied y value, if the y value is higher than the DimensionBoundary (32768 blocks).
        /// This constructor is therefore dimension-aware, so long as the y parameter was originally based on .InternalY, including for example a Vec3d created originally from .InternalY
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public BlockPos(int x, int y, int z)
        {
            this.X = x;
            this.Y = y % DimensionBoundary;
            this.Z = z;
            this.dimension = y / DimensionBoundary;
        }

        public BlockPos(int x, int y, int z, int dim)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.dimension = dim;
        }

        [Obsolete("Not dimension-aware. Use overload with a dimension parameter instead")]
        public BlockPos(Vec3i vec)
        {
            this.X = vec.X;
            this.Y = vec.Y;
            this.Z = vec.Z;
        }

        public BlockPos(Vec3i vec, int dim)
        {
            this.X = vec.X;
            this.Y = vec.Y;
            this.Z = vec.Z;
            this.dimension = dim;
        }

        /// <summary>
        /// Note - for backwards compatibility, this is *not* dimension-aware; explicitly set the dimension in the resulting BlockPos if you need to
        /// </summary>
        public BlockPos(Vec4i vec)
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
            get { return i == 0 ? X : i == 1 ? Y : i == 2 ? Z : dimension; }
            set { if (i == 0) X = value; else if (i == 1) Y = value; else if (i == 2) Z = value; else dimension = value; }
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

        /// <summary>
        /// Not dimension aware (but existing dimension in this BlockPos will be preserved) - use SetAndCorrectDimension() for dimension awareness
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
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

        public BlockPos Set(FastVec3i pos)
        {
            X = pos.X;
            Y = pos.Y;
            Z = pos.Z;
            return this;
        }

        /// <summary>
        /// Dimension aware version of Set() - use this if the Vec3d has the dimension embedded in the Y coordinate (e.g. Y == 65536+ for dimension 2)
        /// </summary>
        /// <param name="origin"></param>
        /// <returns></returns>
        public BlockPos SetAndCorrectDimension(Vec3d origin)
        {
            X = (int)origin.X;
            Y = (int)origin.Y % DimensionBoundary;
            Z = (int)origin.Z;
            dimension = (int)origin.Y / DimensionBoundary;
            return this;
        }

        /// <summary>
        /// Dimension aware version of Set() - use this if there is a dimension embedded in the y coordinate (e.g. y == 65536+ for dimension 2)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public BlockPos SetAndCorrectDimension(int x, int y, int z)
        {
            X = x;
            Y = y % DimensionBoundary;
            Z = z;
            dimension = y / DimensionBoundary;
            return this;
        }




        /// <summary>
        /// Sets XYZ to new values - not dimension aware (but existing dimension will be preserved) - use SetAndCorrectDimension() for dimension awareness
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

        public BlockPos Set(BlockPos blockPos, int dim)
        {
            X = blockPos.X;
            Y = blockPos.Y;
            Z = blockPos.Z;
            dimension = dim;
            return this;
        }

        public BlockPos SetDimension(int dim)
        {
            dimension = dim;
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
            writer.Write(dimension);
        }

        /// <summary>
        /// Convert a block position to coordinates relative to the world spawn position. Note this is dimension unaware
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
            return new BlockPos(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockPos WestCopy(int length = 1)
        {
            return new BlockPos(X - length, Y, Z, dimension);
        }



        /// <summary>
        /// Creates a copy of this blocks position with the z-position adjusted by +<paramref name="length"/>
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockPos SouthCopy(int length = 1)
        {
            return new BlockPos(X, Y, Z + length, dimension);
        }


        /// <summary>
        /// Creates a copy of this blocks position with the x-position adjusted by +<paramref name="length"/>
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockPos EastCopy(int length = 1)
        {
            return new BlockPos(X + length, Y, Z, dimension);
        }

        /// <summary>
        /// Creates a copy of this blocks position with the z-position adjusted by -<paramref name="length"/>
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockPos NorthCopy(int length = 1)
        {
            return new BlockPos(X, Y, Z - length, dimension);
        }

        /// <summary>
        /// Creates a copy of this blocks position with the y-position adjusted by -<paramref name="length"/>
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockPos DownCopy(int length = 1)
        {
            return new BlockPos(X, Y - length, Z, dimension);
        }

        /// <summary>
        /// Creates a copy of this blocks position with the y-position adjusted by +<paramref name="length"/>
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockPos UpCopy(int length = 1)
        {
            return new BlockPos(X, Y + length, Z, dimension);
        }


        /// <summary>
        /// Creates a copy of this blocks position
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual BlockPos Copy()
        {
            return new BlockPos(X, Y, Z, dimension);
        }


        /// <summary>
        /// Creates a copy of this blocks position, obtaining the correct dimension value from the Y value
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual BlockPos CopyAndCorrectDimension()
        {
            return new BlockPos(X, Y % DimensionBoundary, Z, dimension + Y / DimensionBoundary);
        }


        #region Offseting

        /// <summary>
        /// Offsets the position by given xyz
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dz"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        /// <param name="vector"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockPos Add(FastVec3i vector)
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockPos Add(BlockFacing facing, int length = 1)
        {
            var faceNormals = facing.Normali;
            X += faceNormals.X * length;
            Y += faceNormals.Y * length;
            Z += faceNormals.Z * length;
            return this;
        }

        /// <summary>
        /// Offsets the position into the direction of given block face
        /// </summary>
        /// <param name="facing"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockPos Offset(BlockFacing facing)
        {
            var faceNormals = facing.Normali;
            X += faceNormals.X;
            Y += faceNormals.Y;
            Z += faceNormals.Z;
            return this;
        }

        /// <summary>
        /// Creates a copy of this blocks position and offsets it by given xyz
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dz"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockPos AddCopy(float dx, float dy, float dz)
        {
            return new BlockPos((int)(X + dx), (int)(Y + dy), (int)(Z + dz), dimension);
        }

        /// <summary>
        /// Creates a copy of this blocks position and offsets it by given xyz
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dz"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockPos AddCopy(int dx, int dy, int dz)
        {
            return new BlockPos(X + dx, Y + dy, Z + dz, dimension);
        }

        /// <summary>
        /// Creates a copy of this blocks position and offsets it by given xyz
        /// </summary>
        /// <param name="xyz"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockPos AddCopy(int xyz)
        {
            return new BlockPos(X + xyz, Y + xyz, Z + xyz, dimension);
        }


        /// <summary>
        /// Creates a copy of this blocks position and offsets it by given xyz
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockPos AddCopy(Vec3i vector)
        {
            return new BlockPos(X + vector.X, Y + vector.Y, Z + vector.Z, dimension);
        }

        /// <summary>
        /// Creates a copy of this blocks position and offsets it in the direction of given block face
        /// </summary>
        /// <param name="facing"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockPos AddCopy(BlockFacing facing)
        {
            return new BlockPos(X + facing.Normali.X, Y + facing.Normali.Y, Z + facing.Normali.Z, dimension);
        }

        /// <summary>
        /// Creates a copy of this blocks position and offsets it in the direction of given block face
        /// </summary>
        /// <param name="facing"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockPos AddCopy(BlockFacing facing, int length)
        {
            return new BlockPos(X + facing.Normali.X * length, Y + facing.Normali.Y * length, Z + facing.Normali.Z * length, dimension);
        }


        /// <summary>
        /// Substract a position => you'll have the manhatten distance
        /// </summary>
        /// <param name="pos"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockPos Sub(BlockPos pos)
        {
            X -= pos.X;
            Y -= pos.Y;
            Z -= pos.Z;
            return this;
        }

        /// <summary>
        /// Substract a position =&gt; you'll have the manhatten distance
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockPos Sub(int x, int y, int z)
        {
            X -= x;
            Y -= y;
            Z -= z;
            return this;
        }


        /// <summary>
        /// Substract a position => you'll have the manhatten distance.
        /// <br/>If used within a non-zero dimension the resulting BlockPos will be dimensionless as it's a distance or offset between two positions
        /// </summary>
        /// <param name="pos"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockPos SubCopy(BlockPos pos)
        {
            return new BlockPos(X - pos.X, InternalY - pos.InternalY, Z - pos.Z, 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockPos SubCopy(int x, int y, int z)
        {
            return new BlockPos(X - x, Y - y, Z - z, dimension);
        }


        /// <summary>
        /// Creates a copy of this blocks position and divides it by given factor
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BlockPos DivCopy(int factor)
        {
            return new BlockPos(X / factor, Y / factor, Z / factor, dimension);
        }

        /// <summary>
        /// Iterate through NESW offsets from the starting position.  See also BlockFacing.IterateThruFacingOffset
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void IterateHorizontalOffsets(int i)
        {
            // i = 0,1,2,3 corresponds with NESW
            //   i = 0: Z -1, X +0
            //   i = 1: Z +1, X +1
            //   i = 2: Z +1, X -1
            //   i = 3: Z -1, X -1
            Z += ((i + 1) & 2) - 1;       // First bracket is 1,2,3,4  Second bracket is 0,2,2,0  Final is -1,+1,+1,-1
            X += (i % 3 & 1) - (i / 2);   // First bracket is 0,1,0,0  Second bracket is 0,0,1,1  Final is 0,+1,-1,-1
        }
        #endregion

        #region Distance

        /// <summary>
        /// Returns the Euclidean distance to between this and given position. Note if dimensions are different returns maximum value (i.e. infinite)
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public float DistanceTo(BlockPos pos)
        {
            if (pos.dimension != this.dimension) return float.MaxValue;
            double dx = pos.X - X;
            double dy = pos.Y - Y;
            double dz = pos.Z - Z;

            return GameMath.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        /// <summary>
        /// Returns the Euclidean distance to between this and given position. Note this is dimension unaware
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
        /// Returns the squared Euclidean distance to between this and given position. Dimension aware
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public float DistanceSqTo(double x, double y, double z)
        {
            double dx = x - X;
            double dy = y - InternalY;
            double dz = z - Z;

            return (float)(dx * dx + dy * dy + dz * dz);
        }


        /// <summary>
        /// Returns the squared Euclidean distance between the nearer edge of this blockpos (assumed 1 x 0.75 x 1 cube) and given position
        /// The 0.75 offset is because the "heat source" is likely to be above the base position of this block: it's approximate
        /// Note this is dimension unaware
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
        /// Note this is dimension unaware
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
            if (pos.dimension != this.dimension) return int.MaxValue;
            return Math.Abs(X - pos.X) + Math.Abs(Z - pos.Z);
        }


        /// <summary>
        /// Returns the manhatten distance to given position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public int ManhattenDistance(BlockPos pos)
        {
            if (pos.dimension != this.dimension) return int.MaxValue;
            return Math.Abs(X - pos.X) + Math.Abs(Y - pos.Y) + Math.Abs(Z - pos.Z);
        }


        /// <summary>
        /// Returns the manhatten distance to given position
        /// Note this is dimension unaware
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
        /// Note this is dimension unaware
        /// </summary>
        public bool InRangeHorizontally(int x, int z, int range)
        {
            return Math.Abs(X - x) <= range && Math.Abs(Z - z) <= range;
        }


        #endregion

        /// <summary>
        /// Creates a new instance of a Vec3d initialized with this position
        /// Note this is dimension unaware
        /// </summary>
        /// <returns></returns>
        public Vec3d ToVec3d()
        {
            return new Vec3d(X, InternalY, Z);
        }

        /// <summary>
        /// Creates a new instance of a Vec3i initialized with this position
        /// Note this is dimension unaware
        /// </summary>
        /// <returns></returns>
        public Vec3i ToVec3i()
        {
            return new Vec3i(X, InternalY, Z);
        }

        public Vec3f ToVec3f()
        {
            return new Vec3f(X, InternalY, Z);
        }

        public override string ToString()
        {
            return X + ", " + Y + ", " + Z + (dimension > 0 ? " : " + dimension : "");
        }

        public override bool Equals(object obj)
        {
            return (obj is BlockPos pos) && X == pos.X && Y == pos.Y && Z == pos.Z && dimension == pos.dimension;
        }

        public override int GetHashCode()
        {
            return ((17 * 23 + X) * 23 + Y) * 23 + Z + dimension * 269023;
        }

        public bool Equals(BlockPos other)
        {
            return other != null && X == other.X && Y == other.Y && Z == other.Z && dimension == other.dimension;
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
            return new BlockPos(left.X + right.X, left.Y + right.Y, left.Z + right.Z, left.dimension);
        }

        public static BlockPos operator -(BlockPos left, BlockPos right)
        {
            return new BlockPos(left.X - right.X, left.Y - right.Y, left.Z - right.Z, left.dimension);
        }

        public static BlockPos operator +(BlockPos left, int right)
        {
            return new BlockPos(left.X + right, left.Y + right, left.Z + right, left.dimension);
        }

        public static BlockPos operator -(BlockPos left, int right)
        {
            return new BlockPos(left.X - right, left.Y - right, left.Z - right, left.dimension);
        }

        public static BlockPos operator *(BlockPos left, int right)
        {
            return new BlockPos(left.X * right, left.Y * right, left.Z * right, left.dimension);
        }

        public static BlockPos operator *(int left, BlockPos right)
        {
            return new BlockPos(left * right.X, left * right.Y, left * right.Z, right.dimension);
        }

        public static BlockPos operator /(BlockPos left, int right)
        {
            return new BlockPos(left.X / right, left.Y / right, left.Z / right, left.dimension);
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

        [JsonIgnore]
        public Vec3i AsVec3i => new Vec3i((int)X, (int)Y, (int)Z);


        public static void Walk(BlockPos startPos, BlockPos untilPos, Vec3i mapSizeForClamp, Action<int, int, int> onpos)
        {
            int minx = GameMath.Clamp(Math.Min(startPos.X, untilPos.X), 0, mapSizeForClamp.X);
            int miny = GameMath.Clamp(Math.Min(startPos.Y, untilPos.Y), 0, mapSizeForClamp.Y);
            int minz = GameMath.Clamp(Math.Min(startPos.Z, untilPos.Z), 0, mapSizeForClamp.Z);
            int maxx = GameMath.Clamp(Math.Max(startPos.X, untilPos.X), 0, mapSizeForClamp.X);
            int maxy = GameMath.Clamp(Math.Max(startPos.Y, untilPos.Y), 0, mapSizeForClamp.Y);
            int maxz = GameMath.Clamp(Math.Max(startPos.Z, untilPos.Z), 0, mapSizeForClamp.Z);

            for (int x = minx; x < maxx; x++)
            {
                for (int y = miny; y < maxy; y++)
                {
                    for (int z = minz; z < maxz; z++)
                    {
                        onpos(x, y, z);
                    }
                }
            }
        }

        /// <summary>
        /// Like index3d used inside chunks, but the y value can have any value from 0 to worldheight, inside a chunk column
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ToColumnIndex3d()
        {
            const int chunksize = GlobalConstants.ChunkSize;
            int lx = X % chunksize;
            int lz = Z % chunksize;
            return (Y * chunksize + lz) * chunksize + lx;
        }

        public void SetFromColumnIndex3d(int index3d, int cx, int cz)
        {
            const int chunksize = GlobalConstants.ChunkSize;
            X = cx * chunksize + index3d % chunksize;
            Z = cz * chunksize + index3d / chunksize % chunksize;
            Y = index3d / (chunksize * chunksize);
        }

        public int ToSchematicIndex()
        {
            return (Y << 20) + (Z << 10) + X;
        }

        public void SetFromSchematicIndex(int index3d)
        {
            const int mask = (int)BlockSchematic.PosBitMask;
            X = index3d & mask;
            Z = (index3d >> 10) & mask;
            Y = (index3d >> 20) & mask;
        }
    }

    // Exactly like BlockPos except using this class signifies the block should be looked for in the fluids layer; used for server block ticking
    public class FluidBlockPos : BlockPos
    {
        public FluidBlockPos()
        {
        }

        public FluidBlockPos(int x, int y, int z, int dim)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.dimension = dim;
        }

        public override BlockPos Copy()
        {
            return new FluidBlockPos(X, Y, Z, dimension);
        }
    }
}
