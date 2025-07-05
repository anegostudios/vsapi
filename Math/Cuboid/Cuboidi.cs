using ProtoBuf;
using System;

#nullable disable

namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// A 3D cuboid, created out of integers.
    /// </summary>
    [DocumentAsJson]
    [ProtoContract]
    public class Cuboidi : ICuboid<int,Cuboidi>, IEquatable<Cuboidi>
    {
        [ProtoMember(1)]
        [DocumentAsJson]
        public int X1;
        
        [ProtoMember(2)]
        [DocumentAsJson]
        public int Y1;

        [ProtoMember(3)]
        [DocumentAsJson]
        public int Z1;

        [ProtoMember(4)]
        [DocumentAsJson]
        public int X2;

        [ProtoMember(5)]
        [DocumentAsJson]
        public int Y2;

        [ProtoMember(6)]
        [DocumentAsJson]
        public int Z2;

        public int[] Coordinates => new int[] { X1, Y1, Z1, X2, Y2, Z2 };

        public int MinX => Math.Min(X1, X2); 
        public int MinY => Math.Min(Y1, Y2); 
        public int MinZ => Math.Min(Z1, Z2); 


        public int MaxX => Math.Max(X1, X2); 
        public int MaxY => Math.Max(Y1, Y2); 
        public int MaxZ => Math.Max(Z1, Z2);

        public int SizeX => MaxX - MinX; 
        public int SizeY => MaxY - MinY;

        public int SizeZ => MaxZ - MinZ;

        public int SizeXYZ => SizeX * SizeY * SizeZ;
        public int SizeXZ => SizeX * SizeZ;

        public Vec3i Start => new Vec3i(X1, Y1, Z1);
        public Vec3i End => new Vec3i(X2, Y2, Z2);
        public Vec3i Center => new Vec3i((X1 + X2) / 2, (Y1 + Y2) / 2, (Z1 + Z2) / 2);

        public int CenterX => (X1 + X2) / 2;
        public int CenterY => (Y1 + Y2) / 2;
        public int CenterZ => (Z1 + Z2) / 2;
        public int Volume => SizeX * SizeY * SizeZ;


        public Cuboidi() { }

        public Cuboidi(int[] coordinates)
        {
            Set(coordinates[0], coordinates[1], coordinates[2], coordinates[3], coordinates[4], coordinates[5]);
        }

        public Cuboidi(int x1, int y1, int z1, int x2, int y2, int z2)
        {
            Set(x1, y1, z1, x2, y2, z2);
        }

        public Cuboidi(BlockPos startPos, BlockPos endPos)
        {
            Set(startPos.X, startPos.Y, startPos.Z, endPos.X, endPos.Y, endPos.Z);
        }

        public Cuboidi(Vec3i startPos, Vec3i endPos)
        {
            Set(startPos.X, startPos.Y, startPos.Z, endPos.X, endPos.Y, endPos.Z);
        }

        public Cuboidi(BlockPos startPos, int size)
        {
            Set(startPos.X, startPos.Y, startPos.Z, startPos.X + size, startPos.Y + size, startPos.Z + size);
        }

        /// <summary>
        /// Sets the minimum and maximum values of the cuboid
        /// </summary>
        public Cuboidi Set(int x1, int y1, int z1, int x2, int y2, int z2)
        {
            this.X1 = x1;
            this.Y1 = y1;
            this.Z1 = z1;
            this.X2 = x2;
            this.Y2 = y2;
            this.Z2 = z2;
            return this;
        }

        /// <summary>
        /// Sets the minimum and maximum values of the cuboid
        /// </summary>
        public Cuboidi Set(IVec3 min, IVec3 max)
        {
            Set(min.XAsInt, min.YAsInt, min.ZAsInt, max.XAsInt, max.YAsInt, max.ZAsInt);
            return this;
        }

        /// <summary>
        /// Adds the given offset to the cuboid
        /// </summary>
        public Cuboidi Translate(int posX, int posY, int posZ)
        {
            this.X1 += posX;
            this.Y1 += posY;
            this.Z1 += posZ;
            this.X2 += posX;
            this.Y2 += posY;
            this.Z2 += posZ;
            return this;
        }

        /// <summary>
        /// Adds the given offset to the cuboid
        /// </summary>
        public Cuboidi Translate(IVec3 vec)
        {
            Translate(vec.XAsInt, vec.YAsInt, vec.ZAsInt);
            return this;
        }

        /// <summary>
        /// Substractes the given offset to the cuboid
        /// </summary>
        public Cuboidi Sub(int posX, int posY, int posZ)
        {
            this.X1 -= posX;
            this.Y1 -= posY;
            this.Z1 -= posZ;
            this.X2 -= posX;
            this.Y2 -= posY;
            this.Z2 -= posZ;
            return this;
        }

        /// <summary>
        /// Divides the given value to the cuboid
        /// </summary>
        public Cuboidi Div(int value)
        {
            this.X1 /= value;
            this.Y1 /= value;
            this.Z1 /= value;
            this.X2 /= value;
            this.Y2 /= value;
            this.Z2 /= value;
            return this;
        }

        /// <summary>
        /// Substractes the given offset to the cuboid
        /// </summary>
        public Cuboidi Sub(IVec3 vec)
        {
            Sub(vec.XAsInt, vec.YAsInt, vec.ZAsInt);
            return this;
        }

        public bool Contains(Vec3d pos)
        {
            return pos.X >= MinX && pos.X < MaxX && pos.Y >= MinY && pos.Y < MaxY && pos.Z >= MinZ && pos.Z < MaxZ;
        }

        public bool Contains(IVec3 pos)
        {
            return pos.XAsInt >= MinX && pos.XAsInt < MaxX && pos.YAsInt >= MinY && pos.YAsInt < MaxY && pos.ZAsInt >= MinZ && pos.ZAsInt < MaxZ;
        }

        /// <summary>
        /// Returns if the given point is inside the cuboid
        /// </summary>
        public bool Contains(int x, int y, int z)
        {
            return x >= MinX && x < MaxX && y >= MinY && y < MaxY && z >= MinZ && z < MaxZ;
        }

        /// <summary>
        /// Returns if the given point is inside the cuboid
        /// </summary>
        public bool Contains(int x, int z)
        {
            return x >= MinX && x < MaxX && z >= MinZ && z < MaxZ;
        }

        /// <summary>
        /// Returns if the given point is inside the cuboid
        /// </summary>
        public bool Contains(BlockPos pos)
        {
            return pos.X >= MinX && pos.X < MaxX && pos.InternalY >= MinY && pos.InternalY < MaxY && pos.Z >= MinZ && pos.Z < MaxZ;
        }

        /// <summary>
        /// Returns if the given point is inside the cuboid
        /// </summary>
        public bool ContainsOrTouches(int x, int y, int z)
        {
            return x >= MinX && x <= MaxX && y >= MinY && y <= MaxY && z >= MinZ && z <= MaxZ;
        }

        public bool ContainsOrTouches(Cuboidi cuboid)
        {
            return ContainsOrTouches(cuboid.MinX, cuboid.MinY, cuboid.MinZ) && ContainsOrTouches(cuboid.MaxX, cuboid.MaxY, cuboid.MaxZ);
        }


        /// <summary>
        /// Returns if the given point is inside the cuboid
        /// </summary>
        public bool ContainsOrTouches(BlockPos pos)
        {
            return pos.X >= MinX && pos.X <= MaxX && pos.InternalY >= MinY && pos.InternalY <= MaxY && pos.Z >= MinZ && pos.Z <= MaxZ;
        }

        /// <summary>
        /// Returns if the given point is inside the cuboid
        /// </summary>
        public bool ContainsOrTouches(IVec3 vec)
        {
            return ContainsOrTouches(vec.XAsInt, vec.YAsInt, vec.ZAsInt);
        }

        /// <summary>
        /// Returns if the given entityPos is inside the cuboid
        /// </summary>
        public bool ContainsOrTouches(Common.Entities.EntityPos pos)
        {
            return pos.X >= MinX && pos.X <= MaxX && pos.Y >= MinY && pos.Y <= MaxY && pos.Z >= MinZ && pos.Z <= MaxZ;
        }

        /// <summary>
        /// Grows the cuboid so that it includes the given block
        /// </summary>
        public Cuboidi GrowToInclude(int x, int y, int z)
        {
            X1 = Math.Min(X1, x);
            Y1 = Math.Min(Y1, y);
            Z1 = Math.Min(Z1, z);
            X2 = Math.Max(X2, x);
            Y2 = Math.Max(Y2, y);
            Z2 = Math.Max(Z2, z);
            return this;
        }

        /// <summary>
        /// Grows the cuboid so that it includes the given block
        /// </summary>
        public Cuboidi GrowToInclude(IVec3 vec)
        {
            GrowToInclude(vec.XAsInt, vec.YAsInt, vec.ZAsInt);
            return this;
        }

        public Cuboidi GrowBy(int dx, int dy, int dz)
        {
            X1 -= dx;
            X2 += dx;
            Y1 -= dy;
            Y2 += dy;
            Z1 -= dz;
            Z2 += dz;
            return this;
        }

        /// <summary>
        /// Returns the shortest distance between given point and any point inside the cuboid
        /// </summary>
        public double ShortestDistanceFrom(int x, int y, int z)
        {
            double cx = GameMath.Clamp(x, X1, X2);
            double cy = GameMath.Clamp(y, Y1, Y2);
            double cz = GameMath.Clamp(z, Z1, Z2);

            return Math.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy) + (z - cz) * (z - cz));
        }

        /// <summary>
        /// Returns the shortest distance between given point and any point inside the cuboid
        /// </summary>
        public double ShortestDistanceFrom(IVec3 vec)
        {
            return ShortestDistanceFrom(vec.XAsInt, vec.YAsInt, vec.ZAsInt);
        }


        /// <summary>
        /// Returns the shortest distance to any point between this and given cuboid
        /// </summary>
        public double ShortestDistanceFrom(Cuboidi cuboid)
        {
            double cx = cuboid.X1 - GameMath.Clamp(cuboid.X1, X1, X2);
            double cy = cuboid.Y1 - GameMath.Clamp(cuboid.Y1, Y1, Y2);
            double cz = cuboid.Z1 - GameMath.Clamp(cuboid.Z1, Z1, Z2);

            double dx = cuboid.X2 - GameMath.Clamp(cuboid.X2, X1, X2);
            double dy = cuboid.Y2 - GameMath.Clamp(cuboid.Y2, Y1, Y2);
            double dz = cuboid.Z2 - GameMath.Clamp(cuboid.Z2, Z1, Z2);

            return Math.Sqrt(
                Math.Min(cx * cx, dx * dx) + Math.Min(cy * cy, dy * dy) + Math.Min(cz * cz, dz * dz)
            );
        }



        /// <summary>
        /// Returns a new x coordinate that's ensured to be outside this cuboid. Used for collision detection.
        /// </summary>
        public double pushOutX(Cuboidi from, int x, ref EnumPushDirection direction)
        {
            direction = EnumPushDirection.None;
            if (from.Y2 > Y1 && from.Y1 < Y2 && from.Z2 > Z1 && from.Z1 < Z2)
            {
                if (x > 0.0D && from.X2 <= X1 && X1 - from.X2 < x)
                {
                    direction = EnumPushDirection.Positive;
                    x = X1 - from.X2;
                }
                else if (x < 0.0D && from.X1 >= X2 && X2 - from.X1 > x)
                {
                    direction = EnumPushDirection.Negative;
                    x = X2 - from.X1;
                }
            }

            return x;
        }

        /// <summary>
        /// Returns a new y coordinate that's ensured to be outside this cuboid. Used for collision detection.
        /// </summary>
        public double pushOutY(Cuboidi from, int y, ref EnumPushDirection direction)
        {
            direction = EnumPushDirection.None;
            if (from.X2 > X1 && from.X1 < X2 && from.Z2 > Z1 && from.Z1 < Z2)
            {
                if (y > 0.0D && from.Y2 <= Y1 && Y1 - from.Y2 < y)
                {
                    direction = EnumPushDirection.Positive;
                    y = Y1 - from.Y2;
                }
                else if (y < 0.0D && from.Y1 >= Y2 && Y2 - from.Y1 > y)
                {
                    direction = EnumPushDirection.Negative;
                    y = Y2 - from.Y1;
                }
            }

            return y;
        }

        /// <summary>
        /// Returns a new z coordinate that's ensured to be outside this cuboid. Used for collision detection.
        /// </summary>
        public double pushOutZ(Cuboidi from, int z, ref EnumPushDirection direction)
        {
            direction = EnumPushDirection.None;
            if (from.X2 > X1 && from.X1 < X2 && from.Y2 > Y1 && from.Y1 < Y2)
            {
                if (z > 0.0D && from.Z2 <= Z1 && Z1 - from.Z2 < z)
                {
                    direction = EnumPushDirection.Positive;
                    z = Z1 - from.Z2;
                }
                else if (z < 0.0D && from.Z1 >= Z2 && Z2 - from.Z1 > z)
                {
                    direction = EnumPushDirection.Negative;
                    z = Z2 - from.Z1;
                }
            }

            return z;
        }

        /// <summary>
        /// Performs a 3-dimensional rotation on the cuboid and returns a new axis-aligned cuboid resulting from this rotation. Not sure it it makes any sense to use this for other rotations than 90 degree intervals.
        /// </summary>
        public Cuboidi RotatedCopy(int degX, int degY, int degZ, Vec3d origin)
        {
            double radX = degX * GameMath.DEG2RAD;
            double radY = degY * GameMath.DEG2RAD;
            double radZ = degZ * GameMath.DEG2RAD;

            double[] matrix = Mat4d.Create();
            Mat4d.RotateX(matrix, matrix, radX);
            Mat4d.RotateY(matrix, matrix, radY);
            Mat4d.RotateZ(matrix, matrix, radZ);

            double[] min = new double[] { X1 - origin.X, Y1 - origin.Y, Z1 - origin.Z, 1 };
            double[] max = new double[] { X2 - origin.X, Y2 - origin.Y, Z2 - origin.Z, 1 };

            min = Mat4d.MulWithVec4(matrix, min);
            max = Mat4d.MulWithVec4(matrix, max);

            double tmp;
            if (max[0] < min[0])
            {
                tmp = max[0];
                max[0] = min[0];
                min[0] = tmp;
            }
            if (max[1] < min[1])
            {
                tmp = max[1];
                max[1] = min[1];
                min[1] = tmp;
            }
            if (max[2] < min[2])
            {
                tmp = max[2];
                max[2] = min[2];
                min[2] = tmp;
            }

            return new Cuboidi(
                (int)(Math.Round(min[0]) + origin.X), 
                (int)(Math.Round(min[1]) + origin.Y), 
                (int)(Math.Round(min[2]) + origin.Z), 
                (int)(Math.Round(max[0]) + origin.X), 
                (int)(Math.Round(max[1]) + origin.Y), 
                (int)(Math.Round(max[2] + origin.Z))
            );
        }

        /// <summary>
        /// Performs a 3-dimensional rotation on the cuboid and returns a new axis-aligned cuboid resulting from this rotation. Not sure it it makes any sense to use this for other rotations than 90 degree intervals.
        /// </summary>
        public Cuboidi RotatedCopy(IVec3 vec, Vec3d origin)
        {
            return RotatedCopy(vec.XAsInt, vec.YAsInt, vec.ZAsInt, origin);
        }

        /// <summary>
        /// Returns a new cuboid offseted by given position
        /// </summary>
        public Cuboidi OffsetCopy(int x, int y, int z)
        {
            return new Cuboidi(X1 + x, Y1 + y, Z1 + z, X2 + x, Y2 + y, Z2 + z);
        }

        /// <summary>
        /// Returns a new cuboid offseted by given position
        /// </summary>
        public Cuboidi OffsetCopy(IVec3 vec)
        {
            return OffsetCopy(vec.XAsInt, vec.YAsInt, vec.ZAsInt);
        }

        /// <summary>
        /// If the given cuboid intersects with this cubiod
        /// </summary>
        public bool Intersects(Cuboidi with)
        {
            return with.MaxX > this.MinX && with.MinX < this.MaxX ? (with.MaxY > this.MinY && with.MinY < this.MaxY ? with.MaxZ > this.MinZ && with.MinZ < this.MaxZ : false) : false;
        }

        /// <summary>
        /// Ignores the y-axis
        /// </summary>
        /// <param name="with"></param>
        /// <returns></returns>
        public bool Intersects(HorRectanglei with)
        {
            return with.MaxX > this.MinX && with.MinX < this.MaxX ? (with.MaxZ > this.MinZ && with.MinZ < this.MaxZ) : false;
        }


        /// <summary>
        /// If the given cuboid intersects  with or is adjacent to this cubiod
        /// </summary>
        public bool IntersectsOrTouches(Cuboidi with)
        {
            return with.MaxX >= this.MinX && with.MinX <= this.MaxX ? (with.MaxY >= this.MinY && with.MinY <= this.MaxY ? with.MaxZ >= this.MinZ && with.MinZ <= this.MaxZ : false) : false;
        }


        /// <summary>
        /// Creates a copy of the cuboid
        /// </summary>
        public Cuboidi Clone()
        {
            Cuboidi cloned = new Cuboidi(Start, End);
            return cloned;
        }

        public bool Equals(Cuboidi other)
        {
            return other.X1 == X1 && other.Y1 == Y1 && other.Z1 == Z1 && other.X2 == X2 && other.Y2 == Y2 && other.Z2 == Z2;
        }

        public override bool Equals(object obj)
        {
            return (obj is Cuboidi cub) && Equals(cub);
        }

        /// <summary>
        /// Returns true if supplied cuboid is directly adjacent to this one
        /// </summary>
        /// <param name="cuboidi"></param>
        /// <returns></returns>
        internal bool IsAdjacent(Cuboidi cuboidi)
        {
            bool intersect = Intersects(cuboidi);
            bool intersectOrTouch = IntersectsOrTouches(cuboidi);
            return !intersect && intersectOrTouch;
        }


        public override string ToString()
        {
            return string.Format("X1={0},Y1={1},Z1={2},X2={3},Y2={4},Z2={5}", X1, Y1, Z1, X2, Y2, Z2);
        }

        public override int GetHashCode()
        {
            int hashCode = 927660019;
            hashCode = hashCode * -1521134295 + X1.GetHashCode();
            hashCode = hashCode * -1521134295 + Y1.GetHashCode();
            hashCode = hashCode * -1521134295 + Z1.GetHashCode();
            hashCode = hashCode * -1521134295 + X2.GetHashCode();
            hashCode = hashCode * -1521134295 + Y2.GetHashCode();
            hashCode = hashCode * -1521134295 + Z2.GetHashCode();
            return hashCode;
        }

    }
}
