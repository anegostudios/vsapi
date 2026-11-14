//The original sphere based culling was from Mark Morley tutorial on frustum culling
//http://www.crownandcutlass.com/features/technicaldetails/frustum.html

/*Original copyright notice on that source:
 * 
 * "This page and its contents are Copyright 2000 by Mark Morley
 * Unless otherwise noted, you may use any and all code examples provided herein in any way you want.
 * All other content, including but not limited to text and images, may not be reproduced without consent.
 * This file was last edited on Wednesday, 24-Jan-2001 13:24:38 PST"
*/

// AABB update by radfast, February 2023
// Optimization: SIMD AABB check via Vector<float> (256 and 512 bit)

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Frustum culling mode
    /// </summary>
    public enum EnumFrustumCullMode
    {
        NoCull = 0,                      // no culling at all, draw everything not hidden
        CullNormal = 1,                  // normal world pass: LOD and view distance
        CullInstant = 2,                 // one off draw: frustum only
        CullInstantShadowPassNear = 3,   // near shadows: frustum and shadow area only
        CullInstantShadowPassFar = 4     // far shadows: like near but LOD 1 and coarser only
    }

    /// <summary>
    /// A plane of the form n.p + D = 0 A point in front of the plane gives a value above zero
    /// and a point behind it gives a value below zero
    /// </summary>
    public struct Plane
    {
        public double normalX;
        public double normalY;
        public double normalZ;
        public double D;

        // 1 / sqrt(3) Needed to turn a sphere radius into half a box side
        private const double INV_SQRT3 = 0.57735026918962576;

        // Absolute normal components divided by sqrt(3) They are computed once when the plane is
        // created so the AABB test runs without branches and without Math.Abs on every call
        internal double ex;
        internal double ey;
        internal double ez;

        /// <summary>
        /// Creates a plane with a normalized normal of length 1
        /// </summary>
        public Plane(double x, double y, double z, double d)
        {
            double inv = 1.0 / Math.Sqrt(x * x + y * y + z * z);
            normalX = x * inv;
            normalY = y * inv;
            normalZ = z * inv;
            D = d * inv;

            ex = Math.Abs(normalX) * INV_SQRT3;
            ey = Math.Abs(normalY) * INV_SQRT3;
            ez = Math.Abs(normalZ) * INV_SQRT3;
        }

        /// <summary>Signed distance from a point to the plane, negative means the point is behind it</summary>
        public double distanceOfPoint(double x, double y, double z)
        {
            return normalX * x + normalY * y + normalZ * z + D;
        }

        /// <summary>
        /// The box is outside when its corner closest to the inner side of the plane is still behind it:
        /// n.c + |n|.h + D below 0 where h is the radius divided by sqrt(3), half the box side on each axis
        /// This is also the scalar path when SIMD is not used (no vector support or Vector&lt;float&gt; is 128 bit)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AABBisOutside(Sphere sphere)
        {
            return normalX * sphere.x + normalY * sphere.y + normalZ * sphere.z + D
                 + ex * sphere.radius + ey * sphere.radiusY + ez * sphere.radiusZ < 0;
        }
    }


    /// <summary>
    /// A frustum plane in pool space where the pool origin is the coordinate origin
    /// Built by <see cref="FrustumCulling.GetPoolPlanes"/> and used by PoolCullData
    /// </summary>
    internal struct PoolPlane
    {
        public float nx, ny, nz, d;   // normal and offset
        public float ex, ey, ez;      // |normal| / sqrt(3) for the AABB test


        /// <summary>
        /// The box is fully behind the plane Instead of one corner we pick the corner closest to the inner
        /// side of the plane by taking the maximum or minimum per axis depending on the normal sign
        /// </summary>
        public bool IsBoxOutside(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
        {
            float cx = nx > 0 ? maxX : minX;
            float cy = ny > 0 ? maxY : minY;
            float cz = nz > 0 ? maxZ : minZ;
            return nx * cx + ny * cy + nz * cz + d < 0;
        }
    }


    /// <summary>
    /// Kind of cheap range check done before the plane tests
    /// </summary>
    internal enum RangeKind
    {
        None = 0,     // CullInstant: planes only
        Radial = 1,   // CullNormal: circle around the player (view distance and LOD)
        Box = 2       // Shadow modes: rectangle shadowRangeX and shadowRangeZ
    }


    /// <summary>
    /// Settings of one culling pass computed from the mode and the culler state
    /// This is the rule object used by the batched implementation in PoolCullData
    /// </summary>
    internal readonly struct CullPass
    {
        public readonly RangeKind Range;
        public readonly float LimitA;    // Radial: max squared distance, Box: shadowRangeX
        public readonly float LimitB;    // Box: shadowRangeZ
        public readonly float OffsetX;   // pool origin relative to the player
        public readonly float OffsetZ;

        public CullPass(RangeKind range, float limitA, float limitB, float offsetX, float offsetZ)
        {
            Range = range; LimitA = limitA; LimitB = limitB; OffsetX = offsetX; OffsetZ = offsetZ;
        }

        // With Radial the far plane is not needed because the distance check already plays that role
        // which is the same as in InFrustumAndRange
        public bool IncludeFar => Range != RangeKind.Radial;
        public int PlaneCount => IncludeFar ? 6 : 5;
    }


    /// <summary>
    /// The frustum culler keeps the planes of the current view pyramid and tests whether an object is
    /// inside them All visibility rules live here and are used by both the single object checks
    /// (InFrustum and friends) and the batched version in PoolCullData
    /// </summary>
    public class FrustumCulling
    {
        public int ViewDistanceSq;
        internal BlockPos playerPos;
        public float lod0BiasSq;
        /// <summary>If the squared distance is above this share of ViewDistanceSq we switch to LOD2  The default corresponds to roughly 67 percent of the player view distance  With the default about 55 percent of all drawn chunks use LOD2</summary>
        public double lod2BiasSq = 0.45;
        public double shadowRangeX;
        public double shadowRangeZ;

        // Plane order for the tests: the ones that reject the most go first
        // The frustum indices are Near 0, Left 1, Right 2, Top 3, Bottom 4, Far 5
        private static readonly int[] PoolPlaneOrder = [1, 2, 0, 3, 4, 5];

        /// <summary>
        /// Index order: Near 0, Left 1, Right 2, Top 3, Bottom 4, Far 5
        /// </summary>
        private Plane[] frustum = new Plane[6];


        // Rebuilt once per frame in CalcFrustumEquations
        // The sphere center is taken relative to origin (the player position) BEFORE the conversion to
        // float and D is shifted by the same amount This keeps precision far from the world zero point
        // where absolute coordinates as float already lose precision
        private double originX, originY, originZ;

        /// <summary>Updates the view distance A small margin of 20 blocks is added to it</summary>
        public void UpdateViewDistance(int newValue)
        {
            ViewDistanceSq = newValue * newValue + 20 * 20;
        }

        /// <summary>
        /// Tests a sphere against all six planes with the center as the point and the radius as tolerance
        /// Returns true when the sphere is at least partly inside the frustum
        /// </summary>
        public bool SphereInFrustum(double x, double y, double z, double radius)
        {
            if (frustum[0].distanceOfPoint(x, y, z) <= -radius) return false;
            if (frustum[1].distanceOfPoint(x, y, z) <= -radius) return false;
            if (frustum[2].distanceOfPoint(x, y, z) <= -radius) return false;
            if (frustum[3].distanceOfPoint(x, y, z) <= -radius) return false;
            if (frustum[4].distanceOfPoint(x, y, z) <= -radius) return false;
            if (frustum[5].distanceOfPoint(x, y, z) <= -radius) return false;

            return true;
        }


        /// <summary>The sphere is inside the frustum, ignoring range and LOD</summary>
        public bool InFrustum(Sphere sphere)
        {
            return !AabbOutside(sphere, true);
        }


        /// <summary>The object is inside the area the shadows are built from</summary>
        public bool InFrustumShadowPass(Sphere sphere)
        {
            double dx = sphere.x - originX;
            double dz = sphere.z - originZ;

            // Quick check: nothing outside the shadow rectangle is drawn
            if (Math.Abs(dx) >= shadowRangeX || Math.Abs(dz) >= shadowRangeZ) return false;

            return !AabbOutside(sphere, true);
        }


        /// <summary>
        /// Full check for the normal pass: range and LOD first, then the planes
        /// </summary>
        public bool InFrustumAndRange(Sphere sphere, int lodLevel = 0)
        {
            // The cheap distance check comes first because it rejects most chunks
            // before any plane math
            // The conversion to float is deliberate to match BlockPos.HorDistanceSqTo
            // otherwise the LOD thresholds would shift
            double dx = sphere.x - originX;
            double dz = sphere.z - originZ;
            float distanceSq = (float)(dx * dx + dz * dz);

            if (!PassesObjectTest(EnumFrustumCullMode.CullNormal, lodLevel, distanceSq)) return false;

            // The far plane is not checked here (includeFar is false) because its role was already played by the distance check above
            return !AabbOutside(sphere, false);
        }


        /// <summary>
        /// The only place that defines the LOD and view distance rule used by both the single check
        /// in IsVisible through InFrustumAndRange and the batched check in PoolCullData
        ///
        /// LOD 3: implements Lod2, the mesh drawn at long distance (may be empty)
        /// LOD 2: implements Lod2, the mesh drawn at short and medium distance
        /// LOD 1: drawn at any distance
        /// LOD 0: only high detail objects
        /// </summary>
        internal bool IsWithinLodRange(int lodLevel, float distanceSq)
        {
            switch (lodLevel)
            {
                case 0: return lod0BiasSq > 0 && distanceSq < lod0BiasSq + 32 * 32;
                case 1: return distanceSq < ViewDistanceSq;
                case 2: return distanceSq <= lod2BiasSq;
                case 3: return distanceSq > lod2BiasSq && distanceSq < ViewDistanceSq;
                default: return false;
            }
        }


        /// <summary>
        /// The extra per object rule that depends on the mode and is applied after the frustum test
        /// This is the only place it is defined:
        ///   CullNormal               the LOD and view distance rule (IsWithinLodRange)
        ///   CullInstantShadowPassFar the LOD level is at least 1
        /// All other modes have no extra check
        ///
        /// distanceSq is the squared horizontal distance relative to the pool origin and is only used for
        /// CullNormal Pass 0 for the other modes
        ///
        /// Used by both InFrustumAndRange in the single path and PoolCullData.PassesObjectChecks in the
        /// batched path so the rule cannot drift between them
        /// </summary>
        internal bool PassesObjectTest(EnumFrustumCullMode mode, int lodLevel, float distanceSq)
        {
            switch (mode)
            {
                case EnumFrustumCullMode.CullNormal:
                    return IsWithinLodRange(lodLevel, distanceSq);

                case EnumFrustumCullMode.CullInstantShadowPassFar:
                    return lodLevel >= 1;

                default:
                    return true;
            }
        }


        // Tests the box against the view pyramid planes
        // dx and dz are the sphere center relative to origin on the X and Z axes
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool AabbOutside(Sphere s, bool includeFar)
        {
            // SIMD is not used here: a scalar check with early exit and the loop unrolled by hand
            if (frustum[0].AABBisOutside(s)) return true;
            if (frustum[1].AABBisOutside(s)) return true;
            if (frustum[2].AABBisOutside(s)) return true;
            if (frustum[3].AABBisOutside(s)) return true;
            if (frustum[4].AABBisOutside(s)) return true;
            if (includeFar && frustum[5].AABBisOutside(s)) return true;

            return false;
        }


        /// <summary>
        /// Rebuilds the frustum planes from the projection and camera matrices
        /// </summary>
        public void CalcFrustumEquations(BlockPos playerPos, double[] projectionMatrix, double[] cameraMatrix)
        {
            this.playerPos = playerPos;

            // origin is also needed by the distance checks so it is set before any early exit
            if (playerPos != null)
            {
                originX = playerPos.X;
                originY = playerPos.Y;
                originZ = playerPos.Z;
            }
            else
            {
                originX = originY = originZ = 0;
            }

            // Multiply projection by camera to get the final matrix the planes are extracted from
            double[] matFrustum = Mat4d.Create();

            Mat4d.Multiply(matFrustum, projectionMatrix, cameraMatrix);

            CalcFrustumEquations(matFrustum);
        }


        /// <summary>
        /// Which modes cull parts at all, as opposed to NoCull or an unknown mode
        /// Shared with PoolCullData so the rule is not duplicated
        /// </summary>
        internal static bool IsCullingMode(EnumFrustumCullMode mode)
        {
            return mode == EnumFrustumCullMode.CullNormal
                || mode == EnumFrustumCullMode.CullInstant
                || mode == EnumFrustumCullMode.CullInstantShadowPassNear
                || mode == EnumFrustumCullMode.CullInstantShadowPassFar;
        }


        /// <summary>
        /// Builds the CullPass for the given mode and pool origin The pool origin is only used for the
        /// cheap range offsets while the plane geometry with D shifted into pool space is built later
        /// in GetPoolPlanes
        /// </summary>
        internal CullPass CreatePass(EnumFrustumCullMode mode, double poolOriginX, double poolOriginZ)
        {
            GetPoolOffset(poolOriginX, poolOriginZ, out float offsetX, out float offsetZ);

            switch (mode)
            {
                case EnumFrustumCullMode.CullNormal:
                    return new CullPass(RangeKind.Radial, MaxRangeSquared(), 0, offsetX, offsetZ);

                case EnumFrustumCullMode.CullInstantShadowPassNear:
                case EnumFrustumCullMode.CullInstantShadowPassFar:
                    return new CullPass(RangeKind.Box, (float)shadowRangeX, (float)shadowRangeZ, offsetX, offsetZ);

                default:   // CullInstant
                    return new CullPass(RangeKind.None, 0, 0, offsetX, offsetZ);
            }
        }


        /// <summary>
        /// The largest squared distance at which a part can be visible on any LOD
        /// A small margin guarantees the float pre check never rejects what the exact test would accept
        /// </summary>
        private float MaxRangeSquared()
        {
            double lod0Range = lod0BiasSq + 32 * 32;
            double max = Math.Max(Math.Max(ViewDistanceSq, lod0Range), lod2BiasSq);
            return (float)(max * 1.001 + 1);
        }


        /// <summary>
        /// Planes in pool space (sphere center minus pool origin) The D shift is computed in double so
        /// precision does not suffer far from the world zero Returns the number of written planes,
        /// 6 or 5 without Far, same as in InFrustumAndRange
        /// </summary>
        internal int GetPoolPlanes(double poolX, double poolY, double poolZ, bool includeFar, PoolPlane[] result)
        {
            int count = includeFar ? 6 : 5;

            for (int k = 0; k < count; k++)
            {
                Plane p = frustum[PoolPlaneOrder[k]];
                result[k] = new PoolPlane
                {
                    nx = (float)p.normalX,
                    ny = (float)p.normalY,
                    nz = (float)p.normalZ,
                    // Shift the plane into pool space by plugging in the pool origin
                    d = (float)(p.D + p.normalX * poolX + p.normalY * poolY + p.normalZ * poolZ),
                    ex = (float)p.ex,
                    ey = (float)p.ey,
                    ez = (float)p.ez
                };
            }
            return count;
        }

        /// <summary>Pool origin position relative to the player (for the range, LOD and shadow range checks)</summary>
        internal void GetPoolOffset(double poolX, double poolZ, out float offsetX, out float offsetZ)
        {
            offsetX = (float)(poolX - originX);
            offsetZ = (float)(poolZ - originZ);
        }


        /// <summary>
        /// Extracts the frustum planes from the final matrix which is projection times camera
        /// </summary>
        /// <remarks>
        /// Every plane is a combination of two matrix rows: adding them gives one side of the pyramid
        /// and subtracting them gives the opposite side The plane equation is Ax+By+Cz+D=0 with the
        /// normal (A,B,C) and the sign of the value tells which side of the plane a point is on
        /// </remarks>
        private void CalcFrustumEquations(double[] matrix)
        {
            unchecked
            {
                double x, y, z, d;

                // Right plane
                x = matrix[3] - matrix[0];
                y = matrix[7] - matrix[4];
                z = matrix[11] - matrix[8];
                d = matrix[15] - matrix[12];
                frustum[2] = new Plane(x, y, z, d);

                // Left plane
                x = matrix[3] + matrix[0];
                y = matrix[7] + matrix[4];
                z = matrix[11] + matrix[8];
                d = matrix[15] + matrix[12];
                frustum[1] = new Plane(x, y, z, d);

                // Bottom plane
                x = matrix[3] + matrix[1];
                y = matrix[7] + matrix[5];
                z = matrix[11] + matrix[9];
                d = matrix[15] + matrix[13];
                frustum[4] = new Plane(x, y, z, d);

                // Top plane
                x = matrix[3] - matrix[1];
                y = matrix[7] - matrix[5];
                z = matrix[11] - matrix[9];
                d = matrix[15] - matrix[13];
                frustum[3] = new Plane(x, y, z, d);

                // Far plane
                x = matrix[3] - matrix[2];
                y = matrix[7] - matrix[6];
                z = matrix[11] - matrix[10];
                d = matrix[15] - matrix[14];
                frustum[5] = new Plane(x, y, z, d);

                // Near plane
                x = matrix[3] + matrix[2];
                y = matrix[7] + matrix[6];
                z = matrix[11] + matrix[10];
                d = matrix[15] + matrix[14];
                frustum[0] = new Plane(x, y, z, d);
            }
        }
    }

    /// <summary>
    /// LOD level numbers used for the pool (values of LodLevel)
    /// </summary>
    public class EnumLodPool
    {
        public const int NearbyDetail = 0;         // only close to the player
        public const int Everywhere = 1;           // at any distance
        public const int EverywhereExceptFar = 2;   // everywhere except very distant chunks
        public const int FarDistanceOnly = 3;      // only at long distances
    }
}
