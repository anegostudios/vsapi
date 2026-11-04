//The original sphere-based culling was from Mark Morley's tutorial on frustum culling.
//http://www.crownandcutlass.com/features/technicaldetails/frustum.html

/*Original copyright notice on that source:
 * 
 * "This page and its contents are Copyright 2000 by Mark Morley
 * Unless otherwise noted, you may use any and all code examples provided herein in any way you want.
 * All other content, including but not limited to text and images, may not be reproduced without consent.
 * This file was last edited on Wednesday, 24-Jan-2001 13:24:38 PST"
*/

// AABB update by radfast, February 2023
// Performance update: distance test first, precomputed plane extents, SIMD AABB test

using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    public enum EnumFrustumCullMode
    {
        NoCull = 0,
        CullNormal = 1,
        CullInstant = 2,
        CullInstantShadowPassNear = 3,
        CullInstantShadowPassFar = 4
    }

    public struct Plane
    {
        public double normalX;
        public double normalY;
        public double normalZ;
        public double D;

        private const double INV_SQRT3 = 0.57735026918962576;

        // Absolute normal components divided by sqrt(3). Computed once, so the AABB test has no branches.
        internal double ex;
        internal double ey;
        internal double ez;

        /// <summary>
        /// Creates a Plane with normalised (length 1.0) normal vector
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

        public double distanceOfPoint(double x, double y, double z)
        {
            return normalX * x + normalY * y + normalZ * z + D;
        }

        /// <summary>
        /// The box is outside if its corner nearest to the inside is still behind the plane:
        /// n.c + |n|.h + D &lt; 0, where h = radius / sqrt3 is the half size on each axis.
        /// Also serves as the scalar fallback when no vector hardware is available.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AABBisOutside(Sphere sphere)
        {
            return normalX * sphere.x + normalY * sphere.y + normalZ * sphere.z + D
                 + ex * sphere.radius + ey * sphere.radiusY + ez * sphere.radiusZ < 0;
        }
    }


    public class FrustumCulling
    {
        public int ViewDistanceSq;
        internal BlockPos playerPos;
        public float lod0BiasSq;
        /// <summary>If distance squared is above this fraction of ViewDistanceSq, switch to LOD2.  Default value corresponds to distances beyond around 67% of the player's view distance.   At default, approximately 55% of the total rendered chunks will use LOD2.</summary>
        public double lod2BiasSq = 0.45;
        public double shadowRangeX;
        public double shadowRangeZ;

        /// <summary>
        /// Index order: Near 0, Left 1, Right 2, Top 3, Bottom 4, Far 5
        /// </summary>
        private Plane[] frustum = new Plane[6];

        // SIMD data, rebuilt once per frame in CalcFrustumEquations.
        // Sphere centres are taken relative to the origin below before the float cast,
        // and D is shifted to match, so precision holds far from world zero.
        private double originX, originY, originZ;

        // Vector256 layout: 6 planes in 8 lanes, lanes 6 and 7 are padding
        private Vector256<float> w_nx, w_ny, w_nz, w_d, w_ex, w_ey, w_ez;

        // Vector128 layout: planes 0 to 3 in set a, planes 4 and 5 in set b, lanes 2 and 3 are padding
        private Vector128<float> a_nx, a_ny, a_nz, a_d, a_ex, a_ey, a_ez;
        private Vector128<float> b_nx, b_ny, b_nz, b_d, b_ex, b_ey, b_ez;

        public void UpdateViewDistance(int newValue)
        {
            ViewDistanceSq = newValue * newValue + 20 * 20;
        }

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


        public bool InFrustum(Sphere sphere)
        {
            return !AabbOutside(sphere, true, sphere.x - originX, sphere.z - originZ);
        }


        public bool InFrustumShadowPass(Sphere sphere)
        {
            double dx = sphere.x - originX;
            double dz = sphere.z - originZ;
            if (Math.Abs(dx) >= shadowRangeX || Math.Abs(dz) >= shadowRangeZ) return false;

            return !AabbOutside(sphere, true, dx, dz);
        }


        public bool InFrustumAndRange(Sphere sphere, bool nowVisible, int lodLevel = 0)
        {
            // The cheap distance test runs first and rejects most chunks before any plane maths.
            // The float cast matches BlockPos.HorDistanceSqTo.
            double dx = sphere.x - originX;
            double dz = sphere.z - originZ;
            double distance = (float)(dx * dx + dz * dz);

            // Lod level 3: implements Lod2: this is the mesh drawn at long distance  (may be empty)
            // Lod level 2: implements Lod2: this is the mesh drawn at short and medium view distance
            // Lod level 1: drawn at all view distance
            // Lod level 0: only high detail stuff
            switch (lodLevel)
            {
                case 0:
                    if (!(lod0BiasSq > 0 && distance < lod0BiasSq + 32 * 32)) return false;
                    break;
                case 1:
                    if (!(distance < ViewDistanceSq)) return false;
                    break;
                case 2:
                    if (!(distance <= lod2BiasSq)) return false;
                    break;
                case 3:
                    if (!(distance > lod2BiasSq && distance < ViewDistanceSq)) return false;
                    break;
                default:
                    return false;
            }

            // The Far plane is not tested here because the range check above replaces it
            return !AabbOutside(sphere, false, dx, dz);
        }

        // Tests the box against the frustum planes. The hardware checks are constants for the JIT,
        // so only one of the three paths remains in the compiled code.
        // dx and dz are the sphere centre relative to the origin on the X and Z axes.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool AabbOutside(Sphere s, bool includeFar, double dx, double dz)
        {
            if (Vector256.IsHardwareAccelerated)
            {
                // All planes in one vector, one mask extraction
                Vector256<float> acc = Eval(
                    Vector256.Create((float)dx), Vector256.Create((float)(s.y - originY)), Vector256.Create((float)dz),
                    Vector256.Create((float)s.radius), Vector256.Create((float)s.radiusY), Vector256.Create((float)s.radiusZ),
                    w_nx, w_ny, w_nz, w_d, w_ex, w_ey, w_ez);

                uint bits = Vector256.LessThan(acc, Vector256<float>.Zero).ExtractMostSignificantBits();
                return (bits & (includeFar ? 0x3Fu : 0x1Fu)) != 0;
            }

            if (Vector128.IsHardwareAccelerated)
            {
                // Four planes first, the remaining two only if the first set did not reject
                var vx = Vector128.Create((float)dx);
                var vy = Vector128.Create((float)(s.y - originY));
                var vz = Vector128.Create((float)dz);
                var vrx = Vector128.Create((float)s.radius);
                var vry = Vector128.Create((float)s.radiusY);
                var vrz = Vector128.Create((float)s.radiusZ);

                Vector128<float> acc = Eval(vx, vy, vz, vrx, vry, vrz, a_nx, a_ny, a_nz, a_d, a_ex, a_ey, a_ez);
                if ((Vector128.LessThan(acc, Vector128<float>.Zero).ExtractMostSignificantBits() & 0xFu) != 0) return true;

                acc = Eval(vx, vy, vz, vrx, vry, vrz, b_nx, b_ny, b_nz, b_d, b_ex, b_ey, b_ez);
                return (Vector128.LessThan(acc, Vector128<float>.Zero).ExtractMostSignificantBits() & (includeFar ? 0x3u : 0x1u)) != 0;
            }

            // No vector hardware: scalar test with early exit
            int count = includeFar ? 6 : 5;
            for (int i = 0; i < count; i++)
            {
                if (frustum[i].AABBisOutside(s)) return true;
            }
            return false;
        }

        // Plane value d + n.c + e.r, built from three independent chains that are added at the end.
        // This shortens the dependency chain from 6 operations to 4.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector256<float> Eval(
            Vector256<float> sx, Vector256<float> sy, Vector256<float> sz,
            Vector256<float> rx, Vector256<float> ry, Vector256<float> rz,
            Vector256<float> nx, Vector256<float> ny, Vector256<float> nz,
            Vector256<float> d, Vector256<float> ex, Vector256<float> ey, Vector256<float> ez)
        {
            Vector256<float> a = Mad(sy, ny, Mad(sx, nx, d));
            Vector256<float> b = Mad(sz, nz, rx * ex);
            Vector256<float> c = Mad(rz, ez, ry * ey);
            return (b + c) + a;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector128<float> Eval(
            Vector128<float> sx, Vector128<float> sy, Vector128<float> sz,
            Vector128<float> rx, Vector128<float> ry, Vector128<float> rz,
            Vector128<float> nx, Vector128<float> ny, Vector128<float> nz,
            Vector128<float> d, Vector128<float> ex, Vector128<float> ey, Vector128<float> ez)
        {
            Vector128<float> a = Mad(sy, ny, Mad(sx, nx, d));
            Vector128<float> b = Mad(sz, nz, rx * ex);
            Vector128<float> c = Mad(rz, ez, ry * ey);
            return (b + c) + a;
        }

        // Multiply and add, fused when the CPU supports FMA
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector256<float> Mad(Vector256<float> a, Vector256<float> b, Vector256<float> c)
        {
            if (Fma.IsSupported) return Fma.MultiplyAdd(a, b, c);
            return a * b + c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector128<float> Mad(Vector128<float> a, Vector128<float> b, Vector128<float> c)
        {
            if (Fma.IsSupported) return Fma.MultiplyAdd(a, b, c);
            return a * b + c;
        }

        public void CalcFrustumEquations(BlockPos playerPos, double[] projectionMatrix, double[] cameraMatrix)
        {
            this.playerPos = playerPos;

            double[] matFrustum = Mat4d.Create();

            Mat4d.Multiply(matFrustum, projectionMatrix, cameraMatrix);

            CalcFrustumEquations(matFrustum);

            BuildSimdPlanes();
        }

        /// <summary>
        /// Stores the origin and packs the planes into SIMD registers. Runs once per frame.
        /// n.c + D equals n.(c - o) + (D + n.o), so shifting by the origin o is exact in double,
        /// and the remaining values are small enough to store as float.
        /// </summary>
        private void BuildSimdPlanes()
        {
            // The origin is also used by the distance checks, so it is set before any early return
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

            if (!Vector128.IsHardwareAccelerated) return;

            Span<float> nx = stackalloc float[8];
            Span<float> ny = stackalloc float[8];
            Span<float> nz = stackalloc float[8];
            Span<float> d = stackalloc float[8];
            Span<float> ex = stackalloc float[8];
            Span<float> ey = stackalloc float[8];
            Span<float> ez = stackalloc float[8];
            nx.Clear(); ny.Clear(); nz.Clear(); d.Clear(); ex.Clear(); ey.Clear(); ez.Clear();

            for (int i = 0; i < 6; i++)
            {
                Plane p = frustum[i];
                nx[i] = (float)p.normalX;
                ny[i] = (float)p.normalY;
                nz[i] = (float)p.normalZ;
                d[i] = (float)(p.D + p.normalX * originX + p.normalY * originY + p.normalZ * originZ);
                ex[i] = (float)p.ex;
                ey[i] = (float)p.ey;
                ez[i] = (float)p.ez;
            }

            // Padding lanes have a zero normal and positive D, so they never report "outside"
            d[6] = 1f;
            d[7] = 1f;

            if (Vector256.IsHardwareAccelerated)
            {
                w_nx = Vector256.Create((ReadOnlySpan<float>)nx);
                w_ny = Vector256.Create((ReadOnlySpan<float>)ny);
                w_nz = Vector256.Create((ReadOnlySpan<float>)nz);
                w_d = Vector256.Create((ReadOnlySpan<float>)d);
                w_ex = Vector256.Create((ReadOnlySpan<float>)ex);
                w_ey = Vector256.Create((ReadOnlySpan<float>)ey);
                w_ez = Vector256.Create((ReadOnlySpan<float>)ez);
            }
            else
            {
                a_nx = Vector128.Create((ReadOnlySpan<float>)nx.Slice(0, 4));
                a_ny = Vector128.Create((ReadOnlySpan<float>)ny.Slice(0, 4));
                a_nz = Vector128.Create((ReadOnlySpan<float>)nz.Slice(0, 4));
                a_d = Vector128.Create((ReadOnlySpan<float>)d.Slice(0, 4));
                a_ex = Vector128.Create((ReadOnlySpan<float>)ex.Slice(0, 4));
                a_ey = Vector128.Create((ReadOnlySpan<float>)ey.Slice(0, 4));
                a_ez = Vector128.Create((ReadOnlySpan<float>)ez.Slice(0, 4));

                b_nx = Vector128.Create((ReadOnlySpan<float>)nx.Slice(4, 4));
                b_ny = Vector128.Create((ReadOnlySpan<float>)ny.Slice(4, 4));
                b_nz = Vector128.Create((ReadOnlySpan<float>)nz.Slice(4, 4));
                b_d = Vector128.Create((ReadOnlySpan<float>)d.Slice(4, 4));
                b_ex = Vector128.Create((ReadOnlySpan<float>)ex.Slice(4, 4));
                b_ey = Vector128.Create((ReadOnlySpan<float>)ey.Slice(4, 4));
                b_ez = Vector128.Create((ReadOnlySpan<float>)ez.Slice(4, 4));
            }
        }

        /// <summary>
        /// Calculating the frustum planes.
        /// </summary>
        /// <remarks>
        /// From the current OpenGL modelview and projection matrices,
        /// calculate the frustum plane equations (Ax+By+Cz+D=0, normal=(A,B,C))
        /// The equations can then be used to see on which side points are.
        /// </remarks>
        private void CalcFrustumEquations(double[] matrix)
        {
            unchecked
            {
                double x, y, z, d;

                // Extract the numbers for the RIGHT plane
                x = matrix[3] - matrix[0];
                y = matrix[7] - matrix[4];
                z = matrix[11] - matrix[8];
                d = matrix[15] - matrix[12];
                frustum[2] = new Plane(x, y, z, d);

                // Extract the numbers for the LEFT plane
                x = matrix[3] + matrix[0];
                y = matrix[7] + matrix[4];
                z = matrix[11] + matrix[8];
                d = matrix[15] + matrix[12];
                frustum[1] = new Plane(x, y, z, d);

                // Extract the BOTTOM plane
                x = matrix[3] + matrix[1];
                y = matrix[7] + matrix[5];
                z = matrix[11] + matrix[9];
                d = matrix[15] + matrix[13];
                frustum[4] = new Plane(x, y, z, d);

                // Extract the TOP plane
                x = matrix[3] - matrix[1];
                y = matrix[7] - matrix[5];
                z = matrix[11] - matrix[9];
                d = matrix[15] - matrix[13];
                frustum[3] = new Plane(x, y, z, d);

                // Extract the FAR plane
                x = matrix[3] - matrix[2];
                y = matrix[7] - matrix[6];
                z = matrix[11] - matrix[10];
                d = matrix[15] - matrix[14];
                frustum[5] = new Plane(x, y, z, d);

                // Extract the NEAR plane
                x = matrix[3] + matrix[2];
                y = matrix[7] + matrix[6];
                z = matrix[11] + matrix[10];
                d = matrix[15] + matrix[14];
                frustum[0] = new Plane(x, y, z, d);
            }
        }

    }

    public class EnumLodPool
    {
        public const int NearbyDetail = 0;
        public const int Everywhere = 1;
        public const int EverywhereExceptFar = 2;
        public const int FarDistanceOnly = 3;
    }
}
