using System.Runtime.CompilerServices;

namespace Vintagestory.API.MathTools
{
    public static class NewSimplexNoiseLayer
    {
        public const double OldToNewFrequency = 0.6123724356957945;
        public const double MaxYSlope_ImprovedXZ = 5.0; // Not accurate, but if it exceeds this then it's very rare.

        private const long PrimeX = 0x5205402B9270C86FL;
        private const long PrimeY = 0x598CD327003817B5L;
        private const long PrimeZ = 0x5BCC226E9FA0BACBL;
        private const long HashMultiplier = 0x53A3F72DEEC546F5L;
        private const long SeedFlip3D = -0x52D547B2E96ED629L;

        private const double Root3Over3 = 0.577350269189626;
        private const double FallbackRotate3 = 2.0 / 3.0;
        private const double Rotate3Orthogonalizer = -0.211324865405187;

        private const int NGrads3DExponent = 8;
        private const int NGrads3D = 1 << NGrads3DExponent;

        private const double Normalizer3D = 0.2781926117527186;

        public static float Evaluate_ImprovedXZ(long seed, double x, double y, double z)
        {
            // Re-orient the BCC lattice without skewing, so Y points up the main lattice diagonal,
            // and the planes formed by XZ are moved far out of alignment with the cube faces.
            // Orthonormal rotation. Not a skew transform.
            double xz = x + z;
            double s2 = xz * Rotate3Orthogonalizer;
            double yy = y * Root3Over3;
            double xr = x + s2 + yy;
            double zr = z + s2 + yy;
            double yr = xz * -Root3Over3 + yy;

            return Noise3_UnrotatedBase(seed, xr, yr, zr);
        }

        public static float Evaluate_FallbackOrientation(long seed, double x, double y, double z)
        {
            // Re-orient the BCC lattice via rotation to produce a familiar look.
            // Orthonormal rotation. Not a skew transform.
            double r = FallbackRotate3 * (x + y + z);
            double xr = x - r, yr = y - r, zr = z - r;

            return Noise3_UnrotatedBase(seed, xr, yr, zr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Noise3_UnrotatedBase(long seed, double xr, double yr, double zr)
        {

            // Get base points and offsets.
            int xrb = FastFloor(xr), yrb = FastFloor(yr), zrb = FastFloor(zr);
            float xi = (float)(xr - xrb), yi = (float)(yr - yrb), zi = (float)(zr - zrb);

            // Prime pre-multiplication for hash. Also flip seed for second lattice copy.
            long xrbp = xrb * PrimeX, yrbp = yrb * PrimeY, zrbp = zrb * PrimeZ;
            long seed2 = seed ^ SeedFlip3D;

            // -1 if positive, 0 if negative.
            int xNMask = (int)(-0.5f - xi), yNMask = (int)(-0.5f - yi), zNMask = (int)(-0.5f - zi);

            // First vertex.
            float x0 = xi + xNMask;
            float y0 = yi + yNMask;
            float z0 = zi + zNMask;
            float a0 = 0.75f - x0 * x0 - y0 * y0 - z0 * z0;
            long hash0 = HashPrimes(seed, xrbp + (xNMask & PrimeX), yrbp + (yNMask & PrimeY), zrbp + (zNMask & PrimeZ));
            float value = (a0 * a0) * (a0 * a0) * Grad(hash0, x0, y0, z0);

            // Second vertex.
            float x1 = xi - 0.5f;
            float y1 = yi - 0.5f;
            float z1 = zi - 0.5f;
            float a1 = 0.75f - x1 * x1 - y1 * y1 - z1 * z1;
            value += (a1 * a1) * (a1 * a1) * Grad(
                HashPrimes(seed2, xrbp + PrimeX, yrbp + PrimeY, zrbp + PrimeZ),
                x1, y1, z1);

            // Shortcuts for building the remaining falloffs.
            // Derived by subtracting the polynomials with the offsets plugged in.
            float xAFlipMask0 = ((xNMask | 1) << 1) * x1;
            float yAFlipMask0 = ((yNMask | 1) << 1) * y1;
            float zAFlipMask0 = ((zNMask | 1) << 1) * z1;
            float xAFlipMask1 = (-2 - (xNMask << 2)) * x1 - 1.0f;
            float yAFlipMask1 = (-2 - (yNMask << 2)) * y1 - 1.0f;
            float zAFlipMask1 = (-2 - (zNMask << 2)) * z1 - 1.0f;

            bool skip5 = false;
            float a2 = xAFlipMask0 + a0;
            if (a2 > 0)
            {
                value += (a2 * a2) * (a2 * a2) * Grad(
                    HashPrimes(seed, xrbp + (~xNMask & PrimeX), yrbp + (yNMask & PrimeY), zrbp + (zNMask & PrimeZ)),
                    x0 - (xNMask | 1),
                    y0,
                    z0
                );
            }
            else
            {
                float a3 = yAFlipMask0 + zAFlipMask0 + a0;
                if (a3 > 0)
                {
                    value += (a3 * a3) * (a3 * a3) * Grad(
                        HashPrimes(seed, xrbp + (xNMask & PrimeX), yrbp + (~yNMask & PrimeY), zrbp + (~zNMask & PrimeZ)),
                        x0,
                        y0 - (yNMask | 1),
                        z0 - (zNMask | 1)
                    );
                }

                float a4 = xAFlipMask1 + a1;
                if (a4 > 0)
                {
                    value += (a4 * a4) * (a4 * a4) * Grad(
                        HashPrimes(seed2, xrbp + (xNMask & unchecked(PrimeX * 2)), yrbp + PrimeY, zrbp + PrimeZ),
                        (xNMask | 1) + x1,
                        y1,
                        z1
                    );
                    skip5 = true;
                }
            }

            bool skip9 = false;
            float a6 = yAFlipMask0 + a0;
            if (a6 > 0)
            {
                value += (a6 * a6) * (a6 * a6) * Grad(
                    HashPrimes(seed, xrbp + (xNMask & PrimeX), yrbp + (~yNMask & PrimeY), zrbp + (zNMask & PrimeZ)),
                    x0,
                    y0 - (yNMask | 1),
                    z0
                );
            }
            else
            {
                float a7 = xAFlipMask0 + zAFlipMask0 + a0;
                if (a7 > 0)
                {
                    value += (a7 * a7) * (a7 * a7) * Grad(
                        HashPrimes(seed, xrbp + (~xNMask & PrimeX), yrbp + (yNMask & PrimeY), zrbp + (~zNMask & PrimeZ)),
                        x0 - (xNMask | 1),
                        y0,
                        z0 - (zNMask | 1)
                    );
                }

                float a8 = yAFlipMask1 + a1;
                if (a8 > 0)
                {
                    value += (a8 * a8) * (a8 * a8) * Grad(
                        HashPrimes(seed2, xrbp + PrimeX, yrbp + (yNMask & (PrimeY << 1)), zrbp + PrimeZ),
                        x1,
                        (yNMask | 1) + y1,
                        z1
                    );
                    skip9 = true;
                }
            }

            bool skipD = false;
            float aA = zAFlipMask0 + a0;
            if (aA > 0)
            {
                value += (aA * aA) * (aA * aA) * Grad(
                    HashPrimes(seed, xrbp + (xNMask & PrimeX), yrbp + (yNMask & PrimeY), zrbp + (~zNMask & PrimeZ)),
                    x0,
                    y0,
                    z0 - (zNMask | 1)
                );
            }
            else
            {
                float aB = xAFlipMask0 + yAFlipMask0 + a0;
                if (aB > 0)
                {
                    value += (aB * aB) * (aB * aB) * Grad(
                        HashPrimes(seed, xrbp + (~xNMask & PrimeX), yrbp + (~yNMask & PrimeY), zrbp + (zNMask & PrimeZ)),
                        x0 - (xNMask | 1),
                        y0 - (yNMask | 1),
                        z0
                    );
                }

                float aC = zAFlipMask1 + a1;
                if (aC > 0)
                {
                    value += (aC * aC) * (aC * aC) * Grad(
                        HashPrimes(seed2, xrbp + PrimeX, yrbp + PrimeY, zrbp + (zNMask & (PrimeZ << 1))),
                        x1,
                        y1,
                        (zNMask | 1) + z1
                    );
                    skipD = true;
                }
            }

            if (!skip5)
            {
                float a5 = yAFlipMask1 + zAFlipMask1 + a1;
                if (a5 > 0)
                {
                    value += (a5 * a5) * (a5 * a5) * Grad(
                        HashPrimes(seed2, xrbp + PrimeX, yrbp + (yNMask & (PrimeY << 1)), zrbp + (zNMask & (PrimeZ << 1))),
                        x1,
                        (yNMask | 1) + y1,
                        (zNMask | 1) + z1
                    );
                }
            }

            if (!skip9)
            {
                float a9 = xAFlipMask1 + zAFlipMask1 + a1;
                if (a9 > 0)
                {
                    value += (a9 * a9) * (a9 * a9) * Grad(
                        HashPrimes(seed2, xrbp + (xNMask & unchecked(PrimeX * 2)), yrbp + PrimeY, zrbp + (zNMask & (PrimeZ << 1))),
                        (xNMask | 1) + x1,
                        y1,
                        (zNMask | 1) + z1
                    );
                }
            }

            if (!skipD)
            {
                float aD = xAFlipMask1 + yAFlipMask1 + a1;
                if (aD > 0)
                {
                    value += (aD * aD) * (aD * aD) * Grad(
                        HashPrimes(seed2, xrbp + (xNMask & (PrimeX << 1)), yrbp + (yNMask & (PrimeY << 1)), zrbp + PrimeZ),
                        (xNMask | 1) + x1,
                        (yNMask | 1) + y1,
                        z1
                    );
                }
            }

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FastFloor(double x)
        {
            int xi = (int)x;
            return x < xi ? xi - 1 : xi;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long HashPrimes(long seed, long xsvp, long ysvp, long zsvp)
        {
            return seed ^ xsvp ^ ysvp ^ zsvp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Grad(long hash, float dx, float dy, float dz)
        {
            hash *= HashMultiplier;
            hash ^= hash >> (64 - NGrads3DExponent + 2);
            int gi = (int)hash & ((NGrads3D - 1) << 2);
            return Gradients3D[gi | 0] * dx + Gradients3D[gi | 1] * dy + Gradients3D[gi | 2] * dz;
        }

        private static readonly float[] Gradients3D;
        static NewSimplexNoiseLayer()
        {
            Gradients3D = new float[NGrads3D * 4];
            float[] grad3 = {
                 2.22474487139f,       2.22474487139f,      -1.0f,                 0.0f,
                 2.22474487139f,       2.22474487139f,       1.0f,                 0.0f,
                 3.0862664687972017f,  1.1721513422464978f,  0.0f,                 0.0f,
                 1.1721513422464978f,  3.0862664687972017f,  0.0f,                 0.0f,
                -2.22474487139f,       2.22474487139f,      -1.0f,                 0.0f,
                -2.22474487139f,       2.22474487139f,       1.0f,                 0.0f,
                -1.1721513422464978f,  3.0862664687972017f,  0.0f,                 0.0f,
                -3.0862664687972017f,  1.1721513422464978f,  0.0f,                 0.0f,
                -1.0f,                -2.22474487139f,      -2.22474487139f,       0.0f,
                 1.0f,                -2.22474487139f,      -2.22474487139f,       0.0f,
                 0.0f,                -3.0862664687972017f, -1.1721513422464978f,  0.0f,
                 0.0f,                -1.1721513422464978f, -3.0862664687972017f,  0.0f,
                -1.0f,                -2.22474487139f,       2.22474487139f,       0.0f,
                 1.0f,                -2.22474487139f,       2.22474487139f,       0.0f,
                 0.0f,                -1.1721513422464978f,  3.0862664687972017f,  0.0f,
                 0.0f,                -3.0862664687972017f,  1.1721513422464978f,  0.0f,
                //--------------------------------------------------------------------//
                -2.22474487139f,      -2.22474487139f,      -1.0f,                 0.0f,
                -2.22474487139f,      -2.22474487139f,       1.0f,                 0.0f,
                -3.0862664687972017f, -1.1721513422464978f,  0.0f,                 0.0f,
                -1.1721513422464978f, -3.0862664687972017f,  0.0f,                 0.0f,
                -2.22474487139f,      -1.0f,                -2.22474487139f,       0.0f,
                -2.22474487139f,       1.0f,                -2.22474487139f,       0.0f,
                -1.1721513422464978f,  0.0f,                -3.0862664687972017f,  0.0f,
                -3.0862664687972017f,  0.0f,                -1.1721513422464978f,  0.0f,
                -2.22474487139f,      -1.0f,                 2.22474487139f,       0.0f,
                -2.22474487139f,       1.0f,                 2.22474487139f,       0.0f,
                -3.0862664687972017f,  0.0f,                 1.1721513422464978f,  0.0f,
                -1.1721513422464978f,  0.0f,                 3.0862664687972017f,  0.0f,
                -1.0f,                 2.22474487139f,      -2.22474487139f,       0.0f,
                 1.0f,                 2.22474487139f,      -2.22474487139f,       0.0f,
                 0.0f,                 1.1721513422464978f, -3.0862664687972017f,  0.0f,
                 0.0f,                 3.0862664687972017f, -1.1721513422464978f,  0.0f,
                -1.0f,                 2.22474487139f,       2.22474487139f,       0.0f,
                 1.0f,                 2.22474487139f,       2.22474487139f,       0.0f,
                 0.0f,                 3.0862664687972017f,  1.1721513422464978f,  0.0f,
                 0.0f,                 1.1721513422464978f,  3.0862664687972017f,  0.0f,
                 2.22474487139f,      -2.22474487139f,      -1.0f,                 0.0f,
                 2.22474487139f,      -2.22474487139f,       1.0f,                 0.0f,
                 1.1721513422464978f, -3.0862664687972017f,  0.0f,                 0.0f,
                 3.0862664687972017f, -1.1721513422464978f,  0.0f,                 0.0f,
                 2.22474487139f,      -1.0f,                -2.22474487139f,       0.0f,
                 2.22474487139f,       1.0f,                -2.22474487139f,       0.0f,
                 3.0862664687972017f,  0.0f,                -1.1721513422464978f,  0.0f,
                 1.1721513422464978f,  0.0f,                -3.0862664687972017f,  0.0f,
                 2.22474487139f,      -1.0f,                 2.22474487139f,       0.0f,
                 2.22474487139f,       1.0f,                 2.22474487139f,       0.0f,
                 1.1721513422464978f,  0.0f,                 3.0862664687972017f,  0.0f,
                 3.0862664687972017f,  0.0f,                 1.1721513422464978f,  0.0f,
            };
            for (int i = 0; i < grad3.Length; i++)
            {
                grad3[i] = (float)(grad3[i] / Normalizer3D);
            }
            for (int i = 0, j = 0; i < Gradients3D.Length; i++, j++)
            {
                if (j == grad3.Length) j = 0;
                Gradients3D[i] = grad3[j];
            }
        }
    }

}
