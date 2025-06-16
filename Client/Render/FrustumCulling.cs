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


using System;
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
        private const float SQRT3 = 1.7320508f;

        /// <summary>
        /// Creates a Plane with normalised (length 1.0) normal vector
        /// </summary>
        public Plane(double x, double y, double z, double d)
        {
            double normaliser = Math.Sqrt(x * x + y * y + z * z);
            normalX = x / normaliser;
            normalY = y / normaliser;
            normalZ = z / normaliser;
            D = d / normaliser;
        }

        public double distanceOfPoint(double x, double y, double z)
        {
            return normalX * x + normalY * y + normalZ * z + D;
        }

        public bool AABBisOutside(Sphere sphere)
        {
            // The 8 corners of the AABB can be found from the Sphere object, by adding or subtracting halfCubeSize from the centre position

            // Optimised algorithm: First, figure out which of the 8 corners of the AABB we are going to test against this plane - if even the "lowest" corner (the corner in the direction most opposed to the plane normal direction) is outside (above) the plane then the whole AABB must be outside the plane
            // X axis 
            int sign = normalX > 0 ? 1 : -1;
            double testX = (double)sphere.x + sign * sphere.radius / SQRT3;   // sphere.radius is 16 if the sphere represents a chunk and chunksize is 32

            sign = normalY > 0 ? 1 : -1;
            double testY = (double)sphere.y + sign * sphere.radiusY / SQRT3;

            sign = normalZ > 0 ? 1 : -1;
            double testZ = (double)sphere.z + sign * sphere.radiusZ / SQRT3;

            // Now see if that test corner is "outside" the plane
            return testX * normalX + testY * normalY + testZ * normalZ + D < 0;
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
            if (frustum[0].AABBisOutside(sphere)) return false;
            if (frustum[1].AABBisOutside(sphere)) return false;
            if (frustum[2].AABBisOutside(sphere)) return false;
            if (frustum[3].AABBisOutside(sphere)) return false;
            if (frustum[4].AABBisOutside(sphere)) return false;
            if (frustum[5].AABBisOutside(sphere)) return false;

            return true;
        }


        public bool InFrustumShadowPass(Sphere sphere)
        {
            double dist = Math.Abs(playerPos.X - sphere.x);
            if (dist >= shadowRangeX) return false;
            dist = Math.Abs(playerPos.Z - sphere.z);
            if (dist >= shadowRangeZ) return false;

            if (frustum[0].AABBisOutside(sphere)) return false;
            if (frustum[1].AABBisOutside(sphere)) return false;
            if (frustum[2].AABBisOutside(sphere)) return false;
            if (frustum[3].AABBisOutside(sphere)) return false;
            if (frustum[4].AABBisOutside(sphere)) return false;
            if (frustum[5].AABBisOutside(sphere)) return false;
            return true;// && lodLevel == 1) || (distx < shadowRangeX * lodBias + 24 && distz < shadowRangeZ * lodBias + 24);
        }


        public bool InFrustumAndRange(Sphere sphere, bool nowVisible, int lodLevel = 0)
        {
            if (frustum[0].AABBisOutside(sphere)) return false;
            if (frustum[1].AABBisOutside(sphere)) return false;
            if (frustum[2].AABBisOutside(sphere)) return false;
            if (frustum[3].AABBisOutside(sphere)) return false;
            if (frustum[4].AABBisOutside(sphere)) return false;
            // we do not test the Far plane as we have a range check instead  (and testing actually shows chunks are never beyond the Far plane anyhow)


            // Lod level 3: implements Lod2: this is the mesh drawn at long distance  (may be empty)
            // Lod level 2: implements Lod2: this is the mesh drawn at short and medium view distance
            // Lod level 1: drawn at all view distance
            // Lod level 0: only high detail stuff

            double distance = playerPos.HorDistanceSqTo(sphere.x, sphere.z);

            switch (lodLevel)
            {
                case 0:
                    return lod0BiasSq > 0 && distance < lod0BiasSq + 32 * 32;
                case 1:
                    return distance < ViewDistanceSq;
                case 2:
                    return distance <= lod2BiasSq;
                case 3:
                    return distance > lod2BiasSq && distance < ViewDistanceSq;
                default:
                    return false;
            }
        }

        public void CalcFrustumEquations(BlockPos playerPos, double[] projectionMatrix, double[] cameraMatrix)
        {
            this.playerPos = playerPos;

            double[] matFrustum = Mat4d.Create();

            Mat4d.Multiply(matFrustum, projectionMatrix, cameraMatrix);

            CalcFrustumEquations(matFrustum);
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
