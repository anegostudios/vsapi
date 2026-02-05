using System;
using Vintagestory.API.Client;

#nullable disable

namespace Vintagestory.API.MathTools
{
    public class CuboidfWithId : Cuboidf
    {
        public string Id;

        public CuboidfWithId()
        {
        }
        public CuboidfWithId(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            Set((float)x1, (float)y1, (float)z1, (float)x2, (float)y2, (float)z2);
        }
        public CuboidfWithId(float x1, float y1, float z1, float x2, float y2, float z2)
        {
            Set(x1, y1, z1, x2, y2, z2);
        }

        public CuboidfWithId(Cuboidf cuboid)
        {
            Set(cuboid);
        }

    }

    /// <summary>
    /// Represents a three dimensional axis-aligned cuboid using two 3D coordinates. Used for collision and selection boxes.
    /// </summary>
    [DocumentAsJson]
    public class Cuboidf : ICuboid<float,Cuboidf>
    {
        /// <summary>
        /// Start X Pos
        /// </summary>
        [DocumentAsJson("Optional", "0")]
        public float X1;

        /// <summary>
        /// Start Y Pos
        /// </summary>
        [DocumentAsJson("Optional", "0")]
        public float Y1;

        /// <summary>
        /// Start Z Pos
        /// </summary>
        [DocumentAsJson("Optional", "0")]
        public float Z1;

        /// <summary>
        /// End X Pos
        /// </summary>
        [DocumentAsJson("Optional", "0")]
        public float X2;

        /// <summary>
        /// End Y Pos
        /// </summary>
        [DocumentAsJson("Optional", "0")]
        public float Y2;

        /// <summary>
        /// End Z Pos
        /// </summary>
        [DocumentAsJson("Optional", "0")]
        public float Z2;

        /// <summary>
        /// This is equivalent to width so long as X2 > X1, but could in theory be a negative number if the box has its corners the wrong way around
        /// </summary>
        public float XSize { get { return X2 - X1; } }
        /// <summary>
        /// This is equivalent to height so long as Y2 > Y1, but could in theory be a negative number if the box has its corners the wrong way around
        /// </summary>
        public float YSize { get { return Y2 - Y1; } }
        /// <summary>
        /// This is equivalent to length so long as Z2 > Z1, but could in theory be a negative number if the box has its corners the wrong way around
        /// </summary>
        public float ZSize { get { return Z2 - Z1; } }

        public float Width { get { return MaxX - MinX; } }
        public float Height { get { return MaxY - MinY; } }
        public float Length { get { return MaxZ - MinZ; } }

        public float MinX {  get { return Math.Min(X1, X2); } }
        public float MinY { get { return Math.Min(Y1, Y2); } }
        public float MinZ { get { return Math.Min(Z1, Z2); } }
        public float MaxX { get { return Math.Max(X1, X2); } }
        public float MaxY { get { return Math.Max(Y1, Y2); } }
        public float MaxZ { get { return Math.Max(Z1, Z2); } }

        public float MidX { get { return (X1 + X2)/2; } }
        public float MidY { get { return (Y1 + Y2)/2; } }
        public float MidZ { get { return (Z1 + Z2)/2; } }


        public float this[int index]
        {
            get
            {
                switch (index) {
                    case 0: return X1;
                    case 1: return Y1;
                    case 2: return Z1;
                    case 3: return X2;
                    case 4: return Y2;
                    case 5: return Z2;
                }

                throw new ArgumentException("Out of bounds");
            }

            set
            {
                switch (index)
                {
                    case 0: X1 = value; return;
                    case 1: Y1 = value; return;
                    case 2: Z1 = value; return;
                    case 3: X2 = value; return;
                    case 4: Y2 = value; return;
                    case 5: Z2 = value; return;
                }

                throw new ArgumentException("Out of bounds");
            }
        }

        /// <summary>
        /// True when all values are 0
        /// </summary>
        public bool Empty
        {
            get { return X1 == 0 && Y1 == 0 && Z1 == 0 && X2 == 0 && Y2 == 0 && Z2 == 0; }
        }

        public Vec3f Start
        {
            get { return new Vec3f(MinX, MinY, MinZ); }
        }

        public Vec3f End
        {
            get { return new Vec3f(MaxX, MaxY, MaxZ); }
        }

        public Vec3d Startd
        {
            get { return new Vec3d(MinX, MinY, MinZ); }
        }

        public Vec3d Endd
        {
            get { return new Vec3d(MaxX, MaxY, MaxZ); }
        }

        public Vec3d Center => new Vec3d(MidX, MidY, MidZ);

        public Cuboidf()
        {

        }

        public Cuboidf(float size)
        {
            X1 = -size/2;
            Y1 = -size/2;
            Z1 = -size/2;

            X2 = size / 2;
            Y2 = size / 2;
            Z2 = size / 2;
        }

        public Cuboidf(ISize3 size)
        {
            X1 = -size.WidthAsFloat / 2;
            Y1 = -size.HeightAsFloat / 2;
            Z1 = -size.LengthAsFloat / 2;

            X2 = size.WidthAsFloat / 2;
            Y2 = size.HeightAsFloat / 2;
            Z2 = size.LengthAsFloat / 2;
        }



        public Cuboidf(Vec3f start, Vec3f end)
        {
            this.X1 = start.X;
            this.Y1 = start.Y;
            this.Z1 = start.Z;

            this.X2 = end.X;
            this.Y2 = end.Y;
            this.Z2 = end.Z;
        }

        public Cuboidf(float x1, float y1, float z1, float x2, float y2, float z2)
        {
            Set(x1, y1, z1, x2, y2, z2);
        }

        /// <summary>
        /// Sets the minimum and maximum values of the cuboid
        /// </summary>
        public Cuboidf Set(float x1, float y1, float z1, float x2, float y2, float z2)
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
        public Cuboidf Set(IVec3 min, IVec3 max)
        {
            Set(min.XAsFloat, min.YAsFloat, min.ZAsFloat, max.XAsFloat, max.YAsFloat, max.ZAsFloat);
            return this;
        }

        public void Set(Cuboidf collisionBox)
        {
            this.X1 = collisionBox.X1;
            this.Y1 = collisionBox.Y1;
            this.Z1 = collisionBox.Z1;
            this.X2 = collisionBox.X2;
            this.Y2 = collisionBox.Y2;
            this.Z2 = collisionBox.Z2;
        }

 
        /// <summary>
        /// Adds the given offset to the cuboid
        /// </summary>
        public Cuboidf Translate(float posX, float posY, float posZ)
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
        public Cuboidf Translate(IVec3 vec)
        {
            Translate(vec.XAsFloat, vec.YAsFloat, vec.ZAsFloat);
            return this;
        }

        /// <summary>
        /// Substractes the given offset to the cuboid
        /// </summary>
        public Cuboidf Sub(float posX, float posY, float posZ)
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
        /// Substractes the given offset to the cuboid
        /// </summary>
        public Cuboidf Sub(IVec3 vec)
        {
            Sub(vec.XAsFloat, vec.YAsFloat, vec.ZAsFloat);
            return this;
        }

        /// <summary>
        /// Returns if the given point is inside the cuboid
        /// </summary>
        public bool Contains(double x, double y, double z)
        {
            return x >= X1 && x <= X2 && y >= Y1 && y <= Y2 && z >= Z1 && z <= Z2;
        }

        /// <summary>
        /// Returns if the given point is inside the cuboid
        /// </summary>
        public bool ContainsOrTouches(float x, float y, float z)
        {
            return x >= X1 && x <= X2 && y >= Y1 && y <= Y2 && z >= Z1 && z <= Z2;
        }

        /// <summary>
        /// Returns if the given point is inside the cuboid
        /// </summary>
        public bool ContainsOrTouches(IVec3 vec)
        {
            return ContainsOrTouches(vec.XAsFloat, vec.YAsFloat, vec.ZAsFloat);
        }


        public bool Intersects(Cuboidf other, double offsetx, double offsety, double offsetz)
        {
            // For performance, this is a conditional statement with && conjunction: the conditional will fail early if any is false
            // We test the X pair first, then the Z pair, then the Y pair, as it's quite easy for any given pair to test false
            if (X2 > other.X1 + offsetx &&
                X1 < other.X2 + offsetx &&
                Z2 > other.Z1 + offsetz &&
                Z1 < other.Z2 + offsetz &&
                Y2 > other.Y1 + offsety &&
                Y1 < Math.Round(other.Y2 + offsety, 5) // Fix float/double rounding errors. Only need to fix the vertical because gravity. Thankfully we don't have horizontal gravity.
            ) return true;

            return false;
        }


        /// <summary>
        /// If the given cuboid intersects with this cuboid
        /// </summary>
        public bool IntersectsOrTouches(Cuboidf other)
        {
            // For performance, this is a conditional statement with && conjunction: the conditional will fail early if any is false
            // We test the X pair first, then the Z pair, then the Y pair, as it's quite easy for any given pair to test false
            if (X2 >= other.X1 &&
                X1 <= other.X2 &&
                Y2 >= other.Y1 &&
                Y1 <= other.Y2 &&
                Z2 >= other.Z1 &&
                Z1 <= other.Z2
            ) return true;

            return false;
        }


        public Cuboidf OmniNotDownGrowBy(float size)
        {
            X1 -= size;
            Z1 -= size;
            X2 += size;
            Y2 += size;
            Z2 += size;
            return this;
        }

        public Cuboidf OmniGrowBy(float size)
        {
            X1 -= size;
            Y1 -= size;
            Z1 -= size;
            X2 += size;
            Y2 += size;
            Z2 += size;
            return this;
        }

        public Cuboidf ShrinkBy(float size)
        {
            X1 += size;
            Y1 += size;
            Z1 += size;
            X2 -= size;
            Y2 -= size;
            Z2 -= size;
            return this;
        }


        /// <summary>
        /// Grows the cuboid so that it includes the given block
        /// </summary>
        public Cuboidf GrowToInclude(int x, int y, int z)
        {
            X1 = Math.Min(X1, x);
            Y1 = Math.Min(Y1, y);
            Z1 = Math.Min(Z1, z);
            X2 = Math.Max(X2, x + 1);
            Y2 = Math.Max(Y2, y + 1);
            Z2 = Math.Max(Z2, z + 1);
            return this;
        }

        public Cuboidf ClampTo(Vec3f min, Vec3f max)
        {
            X1 = Math.Max(min.X, X1);
            Y1 = Math.Max(min.Y, Y1);
            Z1 = Math.Max(min.Z, Z1);

            X2 = Math.Min(max.X, X2);
            Y2 = Math.Min(max.Y, Y2);
            Z2 = Math.Min(max.Z, Z2);

            return this;
        }

        /// <summary>
        /// Grows the cuboid so that it includes the given block
        /// </summary>
        public Cuboidf GrowToInclude(IVec3 vec)
        {
            GrowToInclude(vec.XAsInt, vec.XAsInt, vec.ZAsInt);
            return this;
        }

        /// <summary>
        /// Returns the shortest distance between given point and any point inside the cuboid
        /// </summary>
        public double ShortestDistanceFrom(float x, float y, float z)
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
            return ShortestDistanceFrom(vec.XAsFloat, vec.YAsFloat, vec.ZAsFloat);
        }

        /// <summary>
        /// Returns a new x coordinate that's ensured to be outside this cuboid. Used for collision detection.
        /// </summary>
        public double pushOutX(Cuboidf from, float x, ref EnumPushDirection direction)
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
        public double pushOutY(Cuboidf from, float y, ref EnumPushDirection direction)
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
        public double pushOutZ(Cuboidf from, float z, ref EnumPushDirection direction)
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
        public Cuboidf RotatedCopy(float degX, float degY, float degZ, Vec3d origin)
        {
            float radX = degX * GameMath.DEG2RAD;
            float radY = degY * GameMath.DEG2RAD;
            float radZ = degZ * GameMath.DEG2RAD;

            Span<float> matrix = stackalloc float[16];
            Mat4f.NewIdentity(matrix);
            Mat4f.Translate(matrix, (float)origin.X, (float)origin.Y, (float)origin.Z);
            Mat4f.RotateByXYZ(matrix, radX, radY, radZ);
            Mat4f.Translate(matrix, -(float)origin.X, -(float)origin.Y, -(float)origin.Z);

            return TransformedCopy(matrix);
        }

        public Cuboidf RotatedCopyRad(float radX, float radY, float radZ, Vec3d origin)
        {
            Span<float> matrix = stackalloc float[16];
            Mat4f.NewIdentity(matrix);
            Mat4f.Translate(matrix, (float)origin.X, (float)origin.Y, (float)origin.Z);
            Mat4f.RotateByXYZ(matrix, radX, radY, radZ);
            Mat4f.Translate(matrix, -(float)origin.X, -(float)origin.Y, -(float)origin.Z);
            return TransformedCopy(matrix);
        }

        public Cuboidf RotatedCopyRad(float radX, float radY, float radZ)
        {
            Span<float> matrix = stackalloc float[16];
            Mat4f.NewIdentity(matrix);
            Mat4f.RotateByXYZ(matrix, radX, radY, radZ);
            return TransformedCopy(matrix);
        }


        public Cuboidf TransformedCopy(Span<float> matrix)
        {
            Span<float> dcoord1 = stackalloc float[4]; Mat4f.MulWithVec4(matrix, dcoord1, X1, Y1, Z1, 1);
            Span<float> dcoord2 = stackalloc float[4]; Mat4f.MulWithVec4(matrix, dcoord2, X1, Y1, Z2, 1);
            Span<float> dcoord3 = stackalloc float[4]; Mat4f.MulWithVec4(matrix, dcoord3, X2, Y1, Z2, 1);
            Span<float> dcoord4 = stackalloc float[4]; Mat4f.MulWithVec4(matrix, dcoord4, X2, Y1, Z1, 1);

            Span<float> ucoord1 = stackalloc float[4]; Mat4f.MulWithVec4(matrix, ucoord1, X1, Y2, Z1, 1);
            Span<float> ucoord2 = stackalloc float[4]; Mat4f.MulWithVec4(matrix, ucoord2, X1, Y2, Z2, 1);
            Span<float> ucoord3 = stackalloc float[4]; Mat4f.MulWithVec4(matrix, ucoord3, X2, Y2, Z2, 1);
            Span<float> ucoord4 = stackalloc float[4]; Mat4f.MulWithVec4(matrix, ucoord4, X2, Y2, Z1, 1);

            Cuboidf cube = new Cuboidf()
            {
                X1 = GameMath.Min(dcoord1[0], dcoord2[0], dcoord3[0], dcoord4[0], ucoord1[0], ucoord2[0], ucoord3[0], ucoord4[0]),
                Y1 = GameMath.Min(dcoord1[1], dcoord2[1], dcoord3[1], dcoord4[1], ucoord1[1], ucoord2[1], ucoord3[1], ucoord4[1]),
                Z1 = GameMath.Min(dcoord1[2], dcoord2[2], dcoord3[2], dcoord4[2], ucoord1[2], ucoord2[2], ucoord3[2], ucoord4[2]),
                X2 = GameMath.Max(dcoord1[0], dcoord2[0], dcoord3[0], dcoord4[0], ucoord1[0], ucoord2[0], ucoord3[0], ucoord4[0]),
                Y2 = GameMath.Max(dcoord1[1], dcoord2[1], dcoord3[1], dcoord4[1], ucoord1[1], ucoord2[1], ucoord3[1], ucoord4[1]),
                Z2 = GameMath.Max(dcoord1[2], dcoord2[2], dcoord3[2], dcoord4[2], ucoord1[2], ucoord2[2], ucoord3[2], ucoord4[2])
            };

            return cube;
        }

        /// <summary>
        /// Performs a 3-dimensional rotation on the cuboid and returns a new axis-aligned cuboid resulting from this rotation. Not sure it it makes any sense to use this for other rotations than 90 degree intervals.
        /// </summary>
        public Cuboidf TransformedCopy(float[] matrix)
        {
            return TransformedCopy((Span<float>)matrix);
        }

        /// <summary>
        /// Performs a 3-dimensional rotation on the cuboid and returns a new axis-aligned cuboid resulting from this rotation. Not sure it it makes any sense to use this for other rotations than 90 degree intervals.
        /// </summary>
        public Cuboidf RotatedCopy(IVec3 vec, Vec3d origin)
        {
            return RotatedCopy(vec.XAsFloat, vec.YAsFloat, vec.ZAsFloat, origin);
        }

        /// <summary>
        /// Returns a new double precision cuboid offseted by given position
        /// </summary>
        public Cuboidf OffsetCopy(float x, float y, float z)
        {
            return new Cuboidf(X1 + x, Y1 + y, Z1 + z, X2 + x, Y2 + y, Z2 + z);
        }

        /// <summary>
        /// Returns a new cuboid offseted by given position
        /// </summary>
        public Cuboidf OffsetCopy(IVec3 vec)
        {
            return OffsetCopy(vec.XAsFloat, vec.YAsFloat, vec.ZAsFloat);
        }

        /// <summary>
        /// Returns a new cuboid offseted by given position
        /// </summary>
        public Cuboidd OffsetCopyDouble(double x, double y, double z)
        {
            return new Cuboidd(X1 + x, Y1 + y, Z1 + z, X2 + x, Y2 + y, Z2 + z);
        }

        /// <summary>
        /// Returns a new cuboid offseted by given position
        /// </summary>
        public Cuboidd OffsetCopyDouble(IVec3 vec)
        {
            return OffsetCopyDouble(vec.XAsDouble, vec.YAsDouble, vec.ZAsDouble);
        }

        /// <summary>
        /// Expands this in the given direction by amount d
        /// </summary>
        public void Expand(BlockFacing face, float d)
        {
            switch (face.Index)
            {
                case BlockFacing.indexNORTH: Z1 -= d; break;
                case BlockFacing.indexEAST: X2 += d; break;
                case BlockFacing.indexSOUTH: Z2 += d; break;
                case BlockFacing.indexWEST: X1 -= d; break;
                case BlockFacing.indexUP: Y2 += d; break;
                case BlockFacing.indexDOWN: Y1 -= d; break;
            }
        }


        /// <summary>
        /// Creates a copy of the cuboid
        /// </summary>
        public Cuboidf Clone()
        {
            Cuboidf cloned = (Cuboidf)MemberwiseClone();
            return cloned;
        }

        public Cuboidd ToDouble()
        {
            return new Cuboidd(X1, Y1, Z1, X2, Y2, Z2);
        }

        /// <summary>
        /// Returns a new cuboid with default size 1 width/height/length
        /// </summary>
        /// <returns></returns>
        public static Cuboidf Default()
        {
            return new Cuboidf(0f, 0f, 0f, 1f, 1f, 1f);
        }

        
        /// <summary>
        /// Makes sure the collisionbox coords are multiples of 0.0001
        /// </summary>
        public void RoundToFracsOfOne10thousand()
        {
            X1 = RoundToOne10thousand(X1);
            Y1 = RoundToOne10thousand(Y1);
            Z1 = RoundToOne10thousand(Z1);
            X2 = RoundToOne10thousand(X2);
            Y2 = RoundToOne10thousand(Y2);
            Z2 = RoundToOne10thousand(Z2);
        }

        static float RoundToOne10thousand(float val)
        {
            return (int) (val * 10000f + 0.5f) / 10000f;
        }

        public bool Equals(Cuboidf other)
        {
            return other.X1 == X1 && other.Y1 == Y1 && other.Z1 == Z1 && other.X2 == X2 && other.Y2 == Y2 && other.Z2 == Z2;
        }

        public Cuboidi ConvertToCuboidi()
        {
            return new Cuboidi(
                (int)X1, (int)Y1, (int)Z1, (int)X2, (int)Y2, (int)Z2
            );
        }

        public override string ToString()
        {
            return "[" + X1 + ", " + Y1 + ", " + Z1 + " => " + X2 + ", " + Y2 + ", " + Z2 + "]";
        }

    }
}
