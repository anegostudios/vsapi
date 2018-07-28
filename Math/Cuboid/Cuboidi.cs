using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.MathTools
{
    public class Cuboidi : ICuboid<int,Cuboidi>
    {
        public int X1;
        public int Y1;
        public int Z1;

        public int X2;
        public int Y2;
        public int Z2;

        public Vec3i Start
        {
            get { return new Vec3i(X1, Y1, Z1); }
        }

        public Vec3i End
        {
            get { return new Vec3i(X2, Y2, Z2); }
        }
        

        public Cuboidi()
        {

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
        public Cuboidi Add(int posX, int posY, int posZ)
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
        public Cuboidi Add(IVec3 vec)
        {
            Add(vec.XAsInt, vec.YAsInt, vec.ZAsInt);
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
        /// Substractes the given offset to the cuboid
        /// </summary>
        public Cuboidi Sub(IVec3 vec)
        {
            Sub(vec.XAsInt, vec.YAsInt, vec.ZAsInt);
            return this;
        }


        /// <summary>
        /// Returns if the given point is inside the cuboid
        /// </summary>
        public bool Contains(int x, int y, int z)
        {
            return x >= X1 && x < X2 && y >= Y1 && y < Y2 && z >= Z1 && z < Z2;
        }

        /// <summary>
        /// Returns if the given point is inside the cuboid
        /// </summary>
        public bool ContainsOrTouches(int x, int y, int z)
        {
            return x >= X1 && x <= X2 && y >= Y1 && y <= Y2 && z >= Z1 && z <= Z2;
        }

        /// <summary>
        /// Returns if the given point is inside the cuboid
        /// </summary>
        public bool ContainsOrTouches(IVec3 vec)
        {
            return ContainsOrTouches(vec.XAsInt, vec.YAsInt, vec.ZAsInt);
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

            double[] pos = new double[] { 0, 0, 0, 1 };

            //Vec3d origin = new Vec3d(0.5, 0.5, 0.5);

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
        public bool IntersectsWith(Cuboidi with)
        {
            return with.X2 > this.X1 && with.X1 < this.X2 ? (with.Y2 > this.Y1 && with.Y1 < this.Y2 ? with.Z2 > this.Z1 && with.Z1 < this.Z2 : false) : false;
        }

        /// <summary>
        /// Creates a copy of the cuboid
        /// </summary>
        public Cuboidi Clone()
        {
            Cuboidi cloned = (Cuboidi)MemberwiseClone();
            return cloned;
        }

        public bool Equals(Cuboidi other)
        {
            return other.X1 == X1 && other.Y1 == Y1 && other.Z1 == Z1 && other.X2 == X2 && other.Y2 == Y2 && other.Z2 == Z2;
        }
    }
}
