using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Datastructures;

namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// Represents a three dimensional axis-aligned cuboid using two 3d coordinates. Used for collision and selection boxes.
    /// </summary>
    public class Cuboidf : ICuboid<float,Cuboidf>
    {
        public float X1;
        public float Y1;
        public float Z1;

        public float X2;
        public float Y2;
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

        public Cuboidf()
        {

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

            float[] matrix = Mat4f.Create();
            Mat4f.RotateX(matrix, matrix, radX);
            Mat4f.RotateY(matrix, matrix, radY);
            Mat4f.RotateZ(matrix, matrix, radZ);

            float[] min = new float[] { X1 - (float)origin.X, Y1 - (float)origin.Y, Z1 - (float)origin.Z, 1 };
            float[] max = new float[] { X2 - (float)origin.X, Y2 - (float)origin.Y, Z2 - (float)origin.Z, 1 };

            min = Mat4f.MulWithVec4(matrix, min);
            max = Mat4f.MulWithVec4(matrix, max);

            float tmp;
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

            Cuboidf cube = new Cuboidf()
            {
                X1 = min[0] + (float)origin.X,
                Y1 = min[1] + (float)origin.Y,
                Z1 = min[2] + (float)origin.Z,
                X2 = max[0] + (float)origin.X,
                Y2 = max[1] + (float)origin.Y,
                Z2 = max[2] + (float)origin.Z
            };

            return cube;
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
            return "[" + X1 + ", " + Y1 + ", " + Z1 + " - " + X2 + ", " + Y2 + ", " + Z2 + "]";
        }

    }
}
