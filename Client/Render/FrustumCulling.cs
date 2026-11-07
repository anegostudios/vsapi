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
// Optimization: SIMD AABB check via Vector<float> (256 and 512 bit)

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
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

        // Absolute values of normal components divided by sqrt(3). Calculated once when the plane is created,
        // so the AABB check then runs without branches and without Math.Abs on each call.
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
        /// The box is outside if its corner closest to the inner side of the plane is still behind the plane:
        /// n.c + |n|.h + D &lt; 0, where h = radius / sqrt3 is half the box side on each axis.
        /// This is also the scalar path when SIMD is not used (no vector acceleration or Vector&lt;float&gt; is already 128 bit).
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

        // Whether to use the SIMD path. Vector acceleration is required and at least 6 lanes so that all six planes
        // fit into one Vector<float>: this is 256 bit (8 lanes) and wider (512 bit gives 16 lanes).
        // On 128 bit (4 lanes, including ARM64) the scalar path is used: measurements showed that a separate
        // 128 bit branch with two plane sets is almost not faster than the scalar one.
        // Both conditions are JIT constants, so after compilation only the needed branch remains.
        private static bool UseSimd
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Vector.IsHardwareAccelerated && Vector<float>.Count >= 6;
        }

        // Data for SIMD. Rebuilt once per frame in CalcFrustumEquations.
        //
        // The sphere center is taken relative to origin (the player position) BEFORE conversion to float,
        // and D is shifted by the same amount. This preserves precision
        // far from the world zero point, where absolute coordinates in float already lose precision.
        private double originX, originY, originZ;

        // Data for six planes in one vector set. Lane i stores plane i,
        // the other lanes (6 and above: two at 256 bit, ten at 512 bit) are padding.
        // Padding lanes have zero normal and positive D, so they never produce "outside".
        private struct PlaneSet
        {
            public Vector<float> nx, ny, nz, d, ex, ey, ez;
        }

        private PlaneSet simdPlanes;

        // Added to the Far plane lane (lane 5) when Far is not checked (includeFar == false).
        // The lane becomes a huge positive number and cannot produce "outside".
        // This replaces the 0x1F mask: instead of extracting bits one LessThanAny is enough.
        // The other lanes are zero. It does not depend on the frame, so it is calculated once when the class is loaded.
        private static readonly Vector<float> FarOffset = CreateFarOffset();

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
            // The cheap distance check goes first: it rejects most chunks
            // before any plane math.
            // The conversion to float is intentional to match BlockPos.HorDistanceSqTo
            // (otherwise the LOD thresholds would shift).
            // dx and dz are calculated once and reused in the plane check.
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

            // Far is not checked here (includeFar = false): its role has already been performed by the distance check above.
            return !AabbOutside(sphere, false, dx, dz);
        }

        // Checks the box against the view pyramid planes.
        // dx and dz are the sphere center relative to origin on the X and Z axes.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool AabbOutside(Sphere s, bool includeFar, double dx, double dz)
        {
            if (UseSimd)
            {
                // The sphere values are broadcast to all lanes. Conversion to float happens after subtracting origin
                var vx = new Vector<float>((float)dx);
                var vy = new Vector<float>((float)(s.y - originY));
                var vz = new Vector<float>((float)dz);
                var vrx = new Vector<float>((float)s.radius);
                var vry = new Vector<float>((float)s.radiusY);
                var vrz = new Vector<float>((float)s.radiusZ);

                // All six planes are evaluated with one vector, the result is checked with one comparison
                Vector<float> acc = Eval(in simdPlanes, vx, vy, vz, vrx, vry, vrz);

                // If Far is not needed, its lane is shifted to a large positive value and cannot trigger.
                if (!includeFar)
                    acc += FarOffset;

                return Vector.LessThanAny(acc, Vector<float>.Zero);
            }

            // SIMD is not used: scalar check with early exit, the loop is manually unrolled
            if (frustum[0].AABBisOutside(s)) return true;
            if (frustum[1].AABBisOutside(s)) return true;
            if (frustum[2].AABBisOutside(s)) return true;
            if (frustum[3].AABBisOutside(s)) return true;
            if (frustum[4].AABBisOutside(s)) return true;
            if (includeFar && frustum[5].AABBisOutside(s)) return true;

            return false;
        }

        // Plane value: d + n.c + e.r.
        // Calculated as three independent chains that are added at the end.
        // This shortens the dependency chain from 6 to 4, and the processor can execute the chains in parallel.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector<float> Eval(
            in PlaneSet p,
            Vector<float> sx, Vector<float> sy, Vector<float> sz,
            Vector<float> rx, Vector<float> ry, Vector<float> rz)
        {
            Vector<float> a = sy * p.ny + (sx * p.nx + p.d);
            Vector<float> b = sz * p.nz + rx * p.ex;
            Vector<float> c = rz * p.ez + ry * p.ey;
            return (b + c) + a;
        }


        public void CalcFrustumEquations(BlockPos playerPos, double[] projectionMatrix, double[] cameraMatrix)
        {
            this.playerPos = playerPos;

            // origin is also needed by the distance checks, so it is set before any early exit
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

            double[] matFrustum = Mat4d.Create();

            Mat4d.Multiply(matFrustum, projectionMatrix, cameraMatrix);

            CalcFrustumEquations(matFrustum);

            BuildSimdPlanes();
        }

        /// <summary>
        /// Packs six planes into SIMD registers. Runs once per frame.
        /// Lane i stores plane i, lanes 6 and above are padding (zero normal and D = 1, they never produce "outside").
        /// D is shifted by origin: n.c + D equals n.(c - o) + (D + n.o). The shift in double is exact,
        /// and the remaining values are small enough to store as float.
        /// </summary>
        private void BuildSimdPlanes()
        {
            // Without the SIMD path this data is not needed, the scalar path is used
            if (!UseSimd)
                return;

            int n = Vector<float>.Count;
            Span<float> nx = stackalloc float[n];
            Span<float> ny = stackalloc float[n];
            Span<float> nz = stackalloc float[n];
            Span<float> d = stackalloc float[n];
            Span<float> ex = stackalloc float[n];
            Span<float> ey = stackalloc float[n];
            Span<float> ez = stackalloc float[n];

            // Each lane is set explicitly in both branches, so it does not matter whether stackalloc zeroes memory
            for (int i = 0; i < n; i++)
            {
                if (i < 6)
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
                else
                {
                    nx[i] = ny[i] = nz[i] = 0f;
                    ex[i] = ey[i] = ez[i] = 0f;
                    d[i] = 1f;
                }
            }

            simdPlanes = new PlaneSet
            {
                nx = new Vector<float>(nx),
                ny = new Vector<float>(ny),
                nz = new Vector<float>(nz),
                d = new Vector<float>(d),
                ex = new Vector<float>(ex),
                ey = new Vector<float>(ey),
                ez = new Vector<float>(ez),
            };
        }

        // Vector with the offset for the Far plane: 1e30f in lane 5, zeros in the others.
        // The UseSimd check is required: the static initializer runs when the class is loaded on any hardware,
        // and with 4 lanes there is no index 5 in the vector, which would cause IndexOutOfRangeException (TypeInitializationException).
        private static Vector<float> CreateFarOffset()
        {
            if (!UseSimd)
                return Vector<float>.Zero;

            Span<float> off = stackalloc float[Vector<float>.Count];
            off.Clear();
            off[5] = 1e30f;
            return new Vector<float>(off);
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
