using System;
using Vintagestory.API.Common.Entities;

#nullable disable

namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// Represents a three dimensional axis-aligned cuboid using two 3d coordinates. Used for collision and selection withes.
    /// </summary>
    public class Cuboidd : ICuboid<double,Cuboidd>
    {
        const double epsilon = 0.000016;   // 1 millionth of a block multiplied by 16.  See RemoveRoundingErrors().  Needed because a rounding error of this kind of size is seen in practice when EntityBox is offset by EntityPos and EntityPos is in the region of 500,000, due to limits on the precision of double arithmetic on large numbers.  The error leads to clipping of the player into blocks, when testing collisions, because with the rounding error the player is seen as "already" inside the block: for example player z-pos is 500000.3, player half-width is 0.3, entityBox.Z1 is 499999.999999988 with the rounding error.  Actually the error in that example is 1/90th of epsilon at positions around 500,000 but choosing this epsilon value of 0.000001 allows some safety even for map edge positions in larger world sizes up to 32Mx32M blocks with no problem. 64M worlds could maybe have minor clipping issues far away from the map centre, untested.

        public double X1;
        public double Y1;
        public double Z1;

        public double X2;
        public double Y2;
        public double Z2;

        /// <summary>
        /// MaxX-MinX
        /// </summary>
        public double Width => MaxX - MinX;
        /// <summary>
        /// MaxY-MinY
        /// </summary>
        public double Height => MaxY - MinY;
        /// <summary>
        /// MaxZ-MinZ
        /// </summary>
        public double Length => MaxZ - MinZ;

        public double MinX { get { return Math.Min(X1, X2); } }
        public double MinY { get { return Math.Min(Y1, Y2); } }
        public double MinZ { get { return Math.Min(Z1, Z2); } }
        public double MaxX { get { return Math.Max(X1, X2); } }
        public double MaxY { get { return Math.Max(Y1, Y2); } }
        public double MaxZ { get { return Math.Max(Z1, Z2); } }


        public Vec3d Start
        {
            get { return new Vec3d(MinX, MinY, MinZ); }
        }

        public Vec3d End
        {
            get { return new Vec3d(MaxX, MaxY, MaxZ); }
        }

        public Cuboidd()
        {

        }

        public Cuboidd(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            Set(x1, y1, z1, x2, y2, z2);
        }

        public Cuboidd(Vec3d start, Vec3d end)
        {
            this.X1 = start.X;
            this.Y1 = start.Y;
            this.Z1 = start.Z;

            this.X2 = end.X;
            this.Y2 = end.Y;
            this.Z2 = end.Z;
        }

        /// <summary>
        /// Sets the minimum and maximum values of the cuboid
        /// </summary>
        public Cuboidd Set(double x1, double y1, double z1, double x2, double y2, double z2)
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
        public Cuboidd Set(IVec3 min, IVec3 max)
        {
            Set(min.XAsDouble, min.YAsDouble, min.ZAsDouble, max.XAsDouble, max.YAsDouble, max.ZAsDouble);
            return this;
        }

        /// <summary>
        /// Sets the minimum and maximum values of the cuboid
        /// </summary>
        public Cuboidd Set(Cuboidf selectionBox)
        {
            this.X1 = selectionBox.X1;
            this.Y1 = selectionBox.Y1;
            this.Z1 = selectionBox.Z1;
            this.X2 = selectionBox.X2;
            this.Y2 = selectionBox.Y2;
            this.Z2 = selectionBox.Z2;
            return this;
        }


        public void Set(Cuboidd other)
        {
            this.X1 = other.X1;
            this.Y1 = other.Y1;
            this.Z1 = other.Z1;
            this.X2 = other.X2;
            this.Y2 = other.Y2;
            this.Z2 = other.Z2;
        }

        /// <summary>
        /// Sets the cuboid to the selectionBox, translated by vec
        /// </summary>
        public Cuboidd SetAndTranslate(Cuboidf selectionBox, Vec3d vec)
        {
            this.X1 = selectionBox.X1 + vec.X;
            this.Y1 = selectionBox.Y1 + vec.Y;
            this.Z1 = selectionBox.Z1 + vec.Z;
            this.X2 = selectionBox.X2 + vec.X;
            this.Y2 = selectionBox.Y2 + vec.Y;
            this.Z2 = selectionBox.Z2 + vec.Z;
            return this;
        }

        /// <summary>
        /// Sets the cuboid to the selectionBox, translated by (dX, dY, dZ)
        /// </summary>
        public Cuboidd SetAndTranslate(Cuboidf selectionBox, double dX, double dY, double dZ)
        {
            this.X1 = selectionBox.X1 + dX;
            this.Y1 = selectionBox.Y1 + dY;
            this.Z1 = selectionBox.Z1 + dZ;
            this.X2 = selectionBox.X2 + dX;
            this.Y2 = selectionBox.Y2 + dY;
            this.Z2 = selectionBox.Z2 + dZ;
            return this;
        }

        public void RemoveRoundingErrors()
        {
            double x1Test = X1 * 16;   // Prevents clipping for whole blocks and other common block hitbox sizes based on 16 voxels; multiplication and later division by a power of 2 will not cause loss of precision in a double
            double z1Test = Z1 * 16;
            double x2Test = X2 * 16;
            double z2Test = Z2 * 16;
            if (Math.Ceiling(x1Test) - x1Test < epsilon) X1 = Math.Ceiling(x1Test) / 16;   // Push out X1 from being just inside a block's collision box (e.g. at xxx.99999) due to precision errors
            if (Math.Ceiling(z1Test) - z1Test < epsilon) Z1 = Math.Ceiling(z1Test) / 16;
            if (x2Test - Math.Floor(x2Test) < epsilon) X2 = Math.Floor(x2Test) / 16;
            if (z2Test - Math.Floor(z2Test) < epsilon) Z2 = Math.Floor(z2Test) / 16;
        }


        /// <summary>
        /// Adds the given offset to the cuboid
        /// </summary>
        public Cuboidd Translate(IVec3 vec)
        {
            Translate(vec.XAsDouble, vec.YAsDouble, vec.ZAsDouble);
            return this;
        }

        /// <summary>
        /// Adds the given offset to the cuboid
        /// </summary>
        public Cuboidd Translate(double posX, double posY, double posZ)
        {
            this.X1 += posX;
            this.Y1 += posY;
            this.Z1 += posZ;
            this.X2 += posX;
            this.Y2 += posY;
            this.Z2 += posZ;
            return this;
        }

        public Cuboidd GrowBy(double dx, double dy, double dz)
        {
            this.X1 -= dx;
            this.Y1 -= dy;
            this.Z1 -= dz;
            this.X2 += dx;
            this.Y2 += dy;
            this.Z2 += dz;
            return this;
        }
        
        /// <summary>
        /// Returns if the given point is inside the cuboid
        /// </summary>
        public bool ContainsOrTouches(double x, double y, double z)
        {
            return x >= X1 && x <= X2 && y >= Y1 && y <= Y2 && z >= Z1 && z <= Z2;
        }

        /// <summary>
        /// Returns if the given point is inside the cuboid
        /// </summary>
        public bool Contains(double x, double y, double z)
        {
            return x > X1 && x < X2 && y > Y1 && y < Y2 && z > Z1 && z < Z2;
        }

        /// <summary>
        /// Returns if the given point is inside the cuboid
        /// </summary>
        public bool ContainsOrTouches(IVec3 vec)
        {
            return ContainsOrTouches(vec.XAsDouble, vec.YAsDouble, vec.ZAsDouble);
        }

        /// <summary>
        /// Grows the cuboid so that it includes the given block
        /// </summary>
        public Cuboidd GrowToInclude(int x, int y, int z)
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
        public Cuboidd GrowToInclude(IVec3 vec)
        {
            GrowToInclude(vec.XAsInt, vec.YAsInt, vec.ZAsInt);
            return this;
        }

        /// <summary>
        /// Returns the shortest distance between given point and any point inside the cuboid
        /// </summary>
        public double ShortestDistanceFrom(double x, double y, double z)
        {
            double cx = x - GameMath.Clamp(x, X1, X2);
            double cy = y - GameMath.Clamp(y, Y1, Y2);
            double cz = z - GameMath.Clamp(z, Z1, Z2);

            return Math.Sqrt(cx*cx + cy*cy + cz*cz);
        }

        
        public Cuboidi ToCuboidi()
        {
            return new Cuboidi((int)X1, (int)Y1, (int)Z1, (int)X2, (int)Y2, (int)Z2);
        }


        /// <summary>
        /// Returns the shortest distance between given point and any point inside the cuboid
        /// </summary>
        public double ShortestVerticalDistanceFrom(double y)
        {
            return y - GameMath.Clamp(y, Y1, Y2);
        }




        /// <summary>
        /// Returns the shortest vertical distance to any point between this and given cuboid
        /// </summary>
        public double ShortestVerticalDistanceFrom(Cuboidd cuboid)
        {
            double cy = cuboid.Y1 - GameMath.Clamp(cuboid.Y1, Y1, Y2);
            double dy = cuboid.Y2 - GameMath.Clamp(cuboid.Y2, Y1, Y2);
            return Math.Min(cy, dy);
        }




        /// <summary>
        /// Returns the shortest distance to any point between this and given cuboid
        /// </summary>
        public double ShortestVerticalDistanceFrom(Cuboidf cuboid, EntityPos offset)
        {
            double oY1 = offset.Y + cuboid.Y1;
            double oY2 = offset.Y + cuboid.Y2;

            double cy = oY1 - GameMath.Clamp(oY1, Y1, Y2);

            // Test if inside
            if (oY1 <= Y1 && oY2 >= Y2) cy = 0;

            double dy = oY2 - GameMath.Clamp(oY2, Y1, Y2);

            return Math.Min(cy, dy);
        }




        /// <summary>
        /// Returns the shortest distance to any point between this and given cuboid
        /// </summary>
        public double ShortestDistanceFrom(Cuboidd cuboid)
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
        /// Returns the shortest distance to any point between this and given cuboid
        /// </summary>
        public double ShortestDistanceFrom(Cuboidf cuboid, BlockPos offset)
        {
            double oX1 = offset.X + cuboid.X1;
            double oY1 = offset.Y + cuboid.Y1;
            double oZ1 = offset.Z + cuboid.Z1;

            double oX2 = offset.X + cuboid.X2;
            double oY2 = offset.Y + cuboid.Y2;
            double oZ2 = offset.Z + cuboid.Z2;


            double cx = oX1 - GameMath.Clamp(oX1, X1, X2);
            double cy = oY1 - GameMath.Clamp(oY1, Y1, Y2);
            double cz = oZ1 - GameMath.Clamp(oZ1, Z1, Z2);

            // Test if inside
            if (oX1 <= X1 && oX2 >= X2) cx = 0;
            if (oY1 <= Y1 && oY2 >= Y2) cy = 0;
            if (oZ1 <= Z1 && oZ2 >= Z2) cz = 0;

            double dx = oX2 - GameMath.Clamp(oX2, X1, X2);
            double dy = oY2 - GameMath.Clamp(oY2, Y1, Y2);
            double dz = oZ2 - GameMath.Clamp(oZ2, Z1, Z2);

            return Math.Sqrt(
                Math.Min(cx * cx, dx * dx) + Math.Min(cy * cy, dy * dy) + Math.Min(cz * cz, dz * dz)
            );
        }


        /// <summary>
        /// Returns the shortest horizontal distance to any point between this and given cuboid
        /// </summary>
        public double ShortestHorizontalDistanceFrom(Cuboidf cuboid, BlockPos offset)
        {
            double cx = offset.X + cuboid.X1 - GameMath.Clamp(offset.X + cuboid.X1, X1, X2);
            double cz = offset.Z + cuboid.Z1 - GameMath.Clamp(offset.Z + cuboid.Z1, Z1, Z2);

            double dx = offset.X + cuboid.X2 - GameMath.Clamp(offset.X + cuboid.X2, X1, X2);
            double dz = offset.Z + cuboid.Z2 - GameMath.Clamp(offset.Z + cuboid.Z2, Z1, Z2);

            return Math.Sqrt(
                Math.Min(cx * cx, dx * dx) + Math.Min(cz * cz, dz * dz)
            );
        }

        /// <summary>
        /// Returns the shortest horizontal distance to any point between this and given coordinate
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public double ShortestHorizontalDistanceFrom(double x, double z)
        {
            double cx = x - GameMath.Clamp(x, X1, X2);
            double cz = z - GameMath.Clamp(z, Z1, Z2);

            return Math.Sqrt(cx * cx + cz * cz);
        }





        /// <summary>
        /// Returns the shortest distance between given point and any point inside the cuboid
        /// </summary>
        public double ShortestDistanceFrom(IVec3 vec)
        {
            return ShortestDistanceFrom(vec.XAsDouble, vec.YAsDouble, vec.ZAsDouble);
        }

        /// <summary>
        /// Returns a new x coordinate that's ensured to be outside this cuboid. Used for collision detection.
        /// </summary>
        public double pushOutX(Cuboidd from, double motx, ref EnumPushDirection direction)
        {
            direction = EnumPushDirection.None;
            if (from.Z2 > Z1 && from.Z1 < Z2 && from.Y2 > Y1 && from.Y1 < Y2)
            {
                if (motx > 0.0D && from.X2 <= X1 && X1 - from.X2 < motx)
                {
                    direction = EnumPushDirection.Positive;
                    motx = X1 - from.X2;
                }
                else if (motx < 0.0D && from.X1 >= X2 && X2 - from.X1 > motx)
                {
                    direction = EnumPushDirection.Negative;
                    motx = X2 - from.X1;
                }
            }

            return motx;
        }

        /// <summary>
        /// Returns a new y coordinate that's ensured to be outside this cuboid. Used for collision detection.
        /// </summary>
        public double pushOutY(Cuboidd from, double moty, ref EnumPushDirection direction)
        {
            direction = EnumPushDirection.None;
            if (from.X2 > X1 && from.X1 < X2 && from.Z2 > Z1 && from.Z1 < Z2)
            {
                if (moty > 0.0D && from.Y2 <= Y1 && Y1 - from.Y2 < moty)
                {
                    direction = EnumPushDirection.Positive;
                    moty = Y1 - from.Y2;
                }
                else if (moty < 0.0D && from.Y1 >= Y2 && Y2 - from.Y1 > moty)
                {
                    direction = EnumPushDirection.Negative;
                    moty = Y2 - from.Y1;
                }
            }

            return moty;
        }

        /// <summary>
        /// Returns a new z coordinate that's ensured to be outside this cuboid. Used for collision detection.
        /// </summary>
        public double pushOutZ(Cuboidd from, double motz, ref EnumPushDirection direction)
        {
            direction = EnumPushDirection.None;
            if (from.X2 > X1 && from.X1 < X2 && from.Y2 > Y1 && from.Y1 < Y2)
            {
                if (motz > 0.0D && from.Z2 <= Z1 && Z1 - from.Z2 < motz)
                {
                    direction = EnumPushDirection.Positive;
                    motz = Z1 - from.Z2;
                }
                else if (motz < 0.0D && from.Z1 >= Z2 && Z2 - from.Z1 > motz)
                {
                    direction = EnumPushDirection.Negative;
                    motz = Z2 - from.Z1;
                }
            }

            return motz;
        }

        /// <summary>
        /// Performs a 3-dimensional rotation on the cuboid and returns a new axis-aligned cuboid resulting from this rotation. Not sure it it makes any sense to use this for other rotations than 90 degree intervals.
        /// </summary>
        public Cuboidd RotatedCopy(double degX, double degY, double degZ, Vec3d origin)
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

            return new Cuboidd(min[0] + origin.X, min[1] + origin.Y, min[2] + origin.Z, max[0] + origin.X, max[1] + origin.Y, max[2] + origin.Z);
        }

        /// <summary>
        /// Performs a 3-dimensional rotation on the cuboid and returns a new axis-aligned cuboid resulting from this rotation. Not sure it makes any sense to use this for other rotations than 90 degree intervals.
        /// </summary>
        public Cuboidd RotatedCopy(IVec3 vec, Vec3d origin)
        {
            return RotatedCopy(vec.XAsDouble, vec.YAsDouble, vec.ZAsDouble, origin);
        }

        public Cuboidd Offset(double dx, double dy, double dz)
        {
            X1 += dx;
            Y1 += dy;
            Z1 += dz;
            X2 += dx;
            Y2 += dy;
            Z2 += dz;
            return this;
        }

        /// <summary>
        /// Returns a new cuboid offseted by given position
        /// </summary>
        public Cuboidd OffsetCopy(double x, double y, double z)
        {
            return new Cuboidd(X1 + x, Y1 + y, Z1 + z, X2 + x, Y2 + y, Z2 + z);
        }

        /// <summary>
        /// Returns a new cuboid offseted by given position
        /// </summary>
        public Cuboidd OffsetCopy(IVec3 vec)
        {
            return OffsetCopy(vec.XAsDouble, vec.YAsDouble, vec.ZAsDouble);
        }


        /// <summary>
        /// If the given cuboid intersects with this cuboid
        /// </summary>
        public bool Intersects(Cuboidd other)
        {
            // For performance, this is a conditional statement with && conjunction: the conditional will fail early if any is false
            if (X2 > other.X1 &&
                X1 < other.X2 &&
                Y2 > other.Y1 &&
                Y1 < other.Y2 &&
                Z2 > other.Z1 &&
                Z1 < other.Z2
            ) return true;

            return false;
        }

        /// <summary>
        /// If the given cuboid intersects with this cuboid
        /// </summary>
        public bool Intersects(Cuboidf other)
        {
            // For performance, this is a conditional statement with && conjunction: the conditional will fail early if any is false
            if (X2 > other.X1 &&
                X1 < other.X2 &&
                Y2 > other.Y1 &&
                Y1 < other.Y2 &&
                Z2 > other.Z1 &&
                Z1 < other.Z2
            ) return true;

            return false;
        }

        /// <summary>
        /// If the given cuboid intersects with this cuboid
        /// </summary>
        public bool Intersects(Cuboidf other, Vec3d offset)
        {
            // For performance, this is a conditional statement with && conjunction: the conditional will fail early if any is false
            // We test the X pair first, then the Z pair, then the Y pair, as it's quite easy for any given pair to test false
            if (X2 > other.X1 + offset.X &&
                X1 < other.X2 + offset.X &&
                Z2 > other.Z1 + offset.Z &&
                Z1 < other.Z2 + offset.Z &&
                Y2 > other.Y1 + offset.Y &&
                Y1 < Math.Round(other.Y2 + offset.Y, 5) // Fix float/double rounding errors. Only need to fix the vertical because gravity. Thankfully we don't have horizontal gravity.
            ) return true;

            return false;
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
        public bool IntersectsOrTouches(Cuboidd other)
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


        /// <summary>
        /// If the given cuboid intersects with this cuboid
        /// </summary>
        public bool IntersectsOrTouches(Cuboidf other, Vec3d offset)
        {
            // For performance, this is a conditional statement with && conjunction: the conditional will fail early if any is false
            // We test the X pair first, then the Z pair, then the Y pair, as it's quite easy for any given pair to test false
            if (X2 >= other.X1 + offset.X &&
                X1 <= other.X2 + offset.X &&
                Z2 >= other.Z1 + offset.Z &&
                Z1 <= other.Z2 + offset.Z &&
                Y2 >= other.Y1 + offset.Y &&
                Y1 <= Math.Round(other.Y2 + offset.Y, 5) // Fix float/double rounding errors. Only need to fix the vertical because gravity. Thankfully we don't have horizontal gravity.
            ) return true;

            return false;
        }

        /// <summary>
        /// If the given cuboid intersects with this cuboid
        /// </summary>
        public bool IntersectsOrTouches(Cuboidf other, double offsetX, double offsetY, double offsetZ)
        {
            bool isOutSide =
                X2 < other.X1 + offsetX ||
                X1 > other.X2 + offsetX ||
                Y2 < other.Y1 + offsetY ||
                Y1 > other.Y2 + offsetY ||
                Z2 < other.Z1 + offsetZ ||
                Z1 > other.Z2 + offsetZ
            ;

            return !isOutSide;
        }


        public Cuboidf ToFloat()
        {
            return new Cuboidf((float)X1, (float)Y1, (float)Z1, (float)X2, (float)Y2, (float)Z2);
        }

        /// <summary>
        /// Creates a copy of the cuboid
        /// </summary>
        public Cuboidd Clone()
        {
            Cuboidd cloned = (Cuboidd)MemberwiseClone();
            return cloned;
        }

        public bool Equals(Cuboidd other)
        {
            return other.X1 == X1 && other.Y1 == Y1 && other.Z1 == Z1 && other.X2 == X2 && other.Y2 == Y2 && other.Z2 == Z2;
        }

        
    }
}
