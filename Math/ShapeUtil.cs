using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable

namespace Vintagestory.API.MathTools
{
    public static class ShapeUtil
    {
        static Vec3f[][] cubicShellNormalizedVectors;

        public static int MaxShells = 38;

        static ShapeUtil()
        {
            cubicShellNormalizedVectors = new Vec3f[MaxShells][];
            int[] ab = new int[2];

            for (int r = 1; r < MaxShells; r++)
            {
                cubicShellNormalizedVectors[r] = new Vec3f[(2 * r + 1) * (2 * r + 1) * 6];
                int j = 0;

                foreach (BlockFacing facing in BlockFacing.ALLFACES)
                {
                    for (ab[0] = -r; ab[0] <= r; ab[0]++)
                    {
                        for (ab[1] = -r; ab[1] <= r; ab[1]++)
                        {
                            Vec3f pos = new Vec3f(facing.Normali.X * r, facing.Normali.Y * r, facing.Normali.Z * r);
                            int l = 0;
                            if (pos.X == 0) pos.X = ab[l++];
                            if (pos.Y == 0) pos.Y = ab[l++];
                            if (l < 2 && pos.Z == 0) pos.Z = ab[l++];

                            cubicShellNormalizedVectors[r][j++] = pos.Normalize();
                        }
                    }
                }
            }

            
        }



        public static Vec3f[] GetCachedCubicShellNormalizedVectors(int radius)
        {
            return cubicShellNormalizedVectors[radius];
        }

        public static Vec3i[] GenCubicShellVectors(int r)
        {
            int[] ab = new int[2];
            Vec3i[] vectors = new Vec3i[(2 * r + 1) * (2 * r + 1) * 6];
            int j = 0;

            foreach (BlockFacing facing in BlockFacing.ALLFACES)
            {
                for (ab[0] = -r; ab[0] <= r; ab[0]++)
                {
                    for (ab[1] = -r; ab[1] <= r; ab[1]++)
                    {
                        Vec3i pos = new Vec3i(facing.Normali.X * r, facing.Normali.Y * r, facing.Normali.Z * r);
                        int l = 0;
                        if (pos.X == 0) pos.X = ab[l++];
                        if (pos.Y == 0) pos.Y = ab[l++];
                        if (l < 2 && pos.Z == 0) pos.Z = ab[l++];

                        vectors[j++] = pos;
                    }
                }
            }

            return vectors;
        }



        /// <summary>
        /// Returns an array of vectors for each point in a square, sorted by manhatten distance to center, exluding the center point
        /// </summary>
        /// <param name="halflength"></param>
        /// <returns></returns>
        public static Vec2i[] GetSquarePointsSortedByMDist(int halflength)
        {
            if (halflength == 0) return Array.Empty<Vec2i>();

            Vec2i[] result = new Vec2i[(2 * halflength + 1) * (2 * halflength + 1) - 1];
            int i = 0;

            for (int x = -halflength; x <= halflength; x++)
            {
                for (int y = -halflength; y <= halflength; y++)
                {
                    if (x == 0 && y == 0) continue;

                    result[i++] = new Vec2i(x, y);
                }
            }

            return result.OrderBy(vec => vec.ManhattenDistance(0, 0)).ToArray();
        }

        /// <summary>
        /// Returns a square outline of given radius (only for odd lengths)
        /// </summary>
        /// <param name="halflength"></param>
        /// <returns></returns>
        public static Vec2i[] GetHollowSquarePoints(int halflength)
        {
            if (halflength == 0) return Array.Empty<Vec2i>();

            int radius = halflength * 2 + 1;

            Vec2i[] result = new Vec2i[radius * 4 - 4];
            int j = 0;
            for (int i = 0; i < radius * 4 - 1; i++)
            {
                int x = (i % radius) - halflength;
                int y = (i % radius) - halflength;

                int quadrant = i / radius;
                switch (quadrant)
                {
                    case 0: y = -halflength; break;
                    case 1: x = halflength; break;
                    case 2: y = halflength; x = -x; break;
                    case 3: x = -halflength; y = -y; break;
                }

                result[j++] = new Vec2i(x, y);

                if ((i + 1) / radius > quadrant) i++;
            }

            return result;
        }


        // This algo returns a point outline or shell of 
        // - A square until radius 9
        // - An octagon with only side a growing beyond radius 9
        public static Vec2i[] GetOctagonPoints(int x, int y, int r)
        {
            if (r == 0)
            {
                return new Vec2i[] { new Vec2i(x, y) };
            }

            List<Vec2i> points = new List<Vec2i>();

            int th = 9; // Math.Max(9, r / 2); - can't, it will skip some chunks then

            int S = 2 * r;
            int a = Math.Min(S, th);
            int b = (int)Math.Ceiling(Math.Max(0, S - th) / 2.0);

            int a2 = (a / 2);

            for (var i = 0; i < a; i++)
            {
                points.Add(new Vec2i(x + i - a2, y - r));
                points.Add(new Vec2i(x - i + a2, y + r));
                points.Add(new Vec2i(x - r, y - i + a2));
                points.Add(new Vec2i(x + r, y + i - a2));
            }

            for (var i = 0; i < b; i++)
            {
                points.Add(new Vec2i(x + a2 + i, y - r + i));
                points.Add(new Vec2i(x - r + i, y + a2 + i));

                points.Add(new Vec2i(x - r + i, y - a2 - i));
                points.Add(new Vec2i(x + a2 + i, y + r - i));
            }

            return points.ToArray<Vec2i>();
        }


        // This algo returns a point outline or shell of 
        // - A square until radius 9
        // - An octagon with only side a growing beyond radius 9
        public static void LoadOctagonIndices(ICollection<long> list, int x, int y, int r, int mapSizeX)
        {
            if (r == 0)
            {
                list.Add(MapUtil.Index2dL(x, y, mapSizeX));
                return;
            }


            int S = 2 * r;
            int a = Math.Min(S, 9);
            int b = (int)(Math.Max(0, S - 9) / Math.Sqrt(2));

            int a2 = (a / 2);

            for (var i = 0; i < a; i++)
            {
                list.Add(MapUtil.Index2dL(x + i - a2, y - r, mapSizeX));
                list.Add(MapUtil.Index2dL(x - i + a2, y + r, mapSizeX));
                list.Add(MapUtil.Index2dL(x - r, y - i + a2, mapSizeX));
                list.Add(MapUtil.Index2dL(x + r, y + i - a2, mapSizeX));
            }

            for (var i = 0; i < b; i++)
            {
                list.Add(MapUtil.Index2dL(x + a2 + i, y - r + i, mapSizeX));
                list.Add(MapUtil.Index2dL(x - r + i, y + a2 + i, mapSizeX));

                list.Add(MapUtil.Index2dL(x - r + i, y - a2 - i, mapSizeX));
                list.Add(MapUtil.Index2dL(x + a2 + i, y + r - i, mapSizeX));
            }
        }




        // http://members.chello.at/~easyfilter/bresenham.html        
        public static Vec2i[] GetPointsOfCircle(int xm, int ym, int r)
        {
            List<Vec2i> points = new List<Vec2i>();

            int x = -r, y = 0, err = 2 - 2 * r; /* II. Quadrant */
            do
            {
                points.Add(new Vec2i(xm - x, ym + y)); /*   I. Quadrant */
                points.Add(new Vec2i(xm - y, ym - x)); /*  II. Quadrant */
                points.Add(new Vec2i(xm + x, ym - y)); /* III. Quadrant */
                points.Add(new Vec2i(xm + y, ym + x)); /*  IV. Quadrant */
                r = err;
                if (r <= y) err += ++y * 2 + 1;           /* e_xy+e_y < 0 */
                if (r > x || err > y) err += ++x * 2 + 1; /* e_xy+e_x > 0 or no 2nd y-step */
            } while (x < 0);

            return points.ToArray<Vec2i>();
        }
    }
}
