using System.Runtime.CompilerServices;

namespace Vintagestory.API.MathTools
{
    public static class NewSimplexNoiseLayer
    {
        public const double OldToNewFrequency2D = 0.577350269189626;
        public const double OldToNewFrequency3D = 0.6123724356957945;
        public const float NewToOldMaxValue2D = 0.8659203889141831f;
        public const float NewToOldMaxValue3D = 0.9871465422297019f;
        public const double MaxYSlope_ImprovedXZ = 5.0; // Not accurate, but if it exceeds this then it's very rare.

        private const long PrimeX = 0x5205402B9270C86FL;
        private const long PrimeY = 0x598CD327003817B5L;
        private const long PrimeZ = 0x5BCC226E9FA0BACBL;
        private const long HashMultiplier = 0x53A3F72DEEC546F5L;
        private const long SeedFlip3D = -0x52D547B2E96ED629L;

        private const double Skew2D = 0.366025403784439;
        private const double Unskew2D = -0.21132486540518713;

        private const double Root3Over3 = 0.577350269189626;
        private const double FallbackRotate3 = 2.0 / 3.0;
        private const double Rotate3Orthogonalizer = -0.211324865405187;

        private const int NGrads2DExponent = 7;
        private const int NGrads3DExponent = 8;
        private const int NFrames2DExponent = 6;
        private const int NGrads2D = 1 << NGrads2DExponent;
        private const int NGrads3D = 1 << NGrads3DExponent;
        private const int NFrames2D = 1 << NFrames2DExponent;

        private const double Normalizer2D = 0.05481866495625118;
        private const double Normalizer3D = 0.2781926117527186;
        private const double FrameNormalizer2D = 0.05517902400809695;

        private const float RSquared2D = 2.0f / 3.0f;
        private const float RSquared3D = 3.0f / 4.0f;

        public static float Evaluate(long seed, double x, double y)
        {
            // Get points for A2* lattice
            double s = Skew2D * (x + y);
            double xs = x + s, ys = y + s;

            // Get base points and offsets.
            int xsb = FastFloor(xs), ysb = FastFloor(ys);
            float xi = (float)(xs - xsb), yi = (float)(ys - ysb);

            // Prime pre-multiplication for hash.
            long xsbp = xsb * PrimeX, ysbp = ysb * PrimeY;

            // Unskew.
            float t = (xi + yi) * (float)Unskew2D;
            float dx0 = xi + t, dy0 = yi + t;

            // First vertex.
            float a0 = RSquared2D - dx0 * dx0 - dy0 * dy0;
            long hash0 = HashPrimes(seed, xsbp, ysbp);
            float value = (a0 * a0) * (a0 * a0) * Grad(hash0, dx0, dy0);

            // Second vertex.
            float a1 = (float)(2 * (1 + 2 * Unskew2D) * (1 / Unskew2D + 2)) * t + ((float)(-2 * (1 + 2 * Unskew2D) * (1 + 2 * Unskew2D)) + a0);
            float dx1 = dx0 - (float)(1 + 2 * Unskew2D);
            float dy1 = dy0 - (float)(1 + 2 * Unskew2D);
            long hash1 = HashPrimes(seed, xsbp + PrimeX, ysbp + PrimeY);
            value += (a1 * a1) * (a1 * a1) * Grad(hash1, dx1, dy1);

            // Third and fourth vertices.
            // Nested conditionals were faster than compact bit logic/arithmetic.
            float xmyi = xi - yi;
            if (t < Unskew2D)
            {
                if (xi + xmyi > 1)
                {
                    float dx2 = dx0 - (float)(3 * Unskew2D + 2);
                    float dy2 = dy0 - (float)(3 * Unskew2D + 1);
                    float a2 = RSquared2D - dx2 * dx2 - dy2 * dy2;
                    if (a2 > 0)
                    {
                        long hash2 = HashPrimes(seed, xsbp + (PrimeX << 1), ysbp + PrimeY);
                        value += (a2 * a2) * (a2 * a2) * Grad(hash2, dx2, dy2);
                    }
                }
                else
                {
                    float dx2 = dx0 - (float)Unskew2D;
                    float dy2 = dy0 - (float)(Unskew2D + 1);
                    float a2 = RSquared2D - dx2 * dx2 - dy2 * dy2;
                    if (a2 > 0)
                    {
                        long hash2 = HashPrimes(seed, xsbp, ysbp + PrimeY);
                        value += (a2 * a2) * (a2 * a2) * Grad(hash2, dx2, dy2);
                    }
                }

                if (yi - xmyi > 1)
                {
                    float dx3 = dx0 - (float)(3 * Unskew2D + 1);
                    float dy3 = dy0 - (float)(3 * Unskew2D + 2);
                    float a3 = RSquared2D - dx3 * dx3 - dy3 * dy3;
                    if (a3 > 0)
                    {
                        long hash3 = HashPrimes(seed, xsbp + PrimeX, ysbp + (PrimeY << 1));
                        value += (a3 * a3) * (a3 * a3) * Grad(hash3, dx3, dy3);
                    }
                }
                else
                {
                    float dx3 = dx0 - (float)(Unskew2D + 1);
                    float dy3 = dy0 - (float)Unskew2D;
                    float a3 = RSquared2D - dx3 * dx3 - dy3 * dy3;
                    if (a3 > 0)
                    {
                        long hash3 = HashPrimes(seed, xsbp + PrimeX, ysbp);
                        value += (a3 * a3) * (a3 * a3) * Grad(hash3, dx3, dy3);
                    }
                }
            }
            else
            {
                if (xi + xmyi < 0)
                {
                    float dx2 = dx0 + (float)(1 + Unskew2D);
                    float dy2 = dy0 + (float)Unskew2D;
                    float a2 = RSquared2D - dx2 * dx2 - dy2 * dy2;
                    if (a2 > 0)
                    {
                        long hash2 = HashPrimes(seed, xsbp - PrimeX, ysbp);
                        value += (a2 * a2) * (a2 * a2) * Grad(hash2, dx2, dy2);
                    }
                }
                else
                {
                    float dx2 = dx0 - (float)(Unskew2D + 1);
                    float dy2 = dy0 - (float)Unskew2D;
                    float a2 = RSquared2D - dx2 * dx2 - dy2 * dy2;
                    if (a2 > 0)
                    {
                        long hash2 = HashPrimes(seed, xsbp + PrimeX, ysbp);
                        value += (a2 * a2) * (a2 * a2) * Grad(hash2, dx2, dy2);
                    }
                }

                if (yi < xmyi)
                {
                    float dx3 = dx0 + (float)Unskew2D;
                    float dy3 = dy0 + (float)(Unskew2D + 1);
                    float a3 = RSquared2D - dx3 * dx3 - dy3 * dy3;
                    if (a3 > 0)
                    {
                        long hash3 = HashPrimes(seed, xsbp, ysbp - PrimeY);
                        value += (a3 * a3) * (a3 * a3) * Grad(hash3, dx3, dy3);
                    }
                }
                else
                {
                    float dx3 = dx0 - (float)Unskew2D;
                    float dy3 = dy0 - (float)(Unskew2D + 1);
                    float a3 = RSquared2D - dx3 * dx3 - dy3 * dy3;
                    if (a3 > 0)
                    {
                        long hash3 = HashPrimes(seed, xsbp, ysbp + PrimeY);
                        value += (a3 * a3) * (a3 * a3) * Grad(hash3, dx3, dy3);
                    }
                }
            }

            return value;
        }

        public static void VectorEvaluate(long seed, double x, double y, out float valueX, out float valueY)
        {
            // Get points for A2* lattice
            double s = Skew2D * (x + y);
            double xs = x + s, ys = y + s;

            // Get base points and offsets.
            int xsb = FastFloor(xs), ysb = FastFloor(ys);
            float xi = (float)(xs - xsb), yi = (float)(ys - ysb);

            // Prime pre-multiplication for hash.
            long xsbp = xsb * PrimeX, ysbp = ysb * PrimeY;

            // Unskew.
            float t = (xi + yi) * (float)Unskew2D;
            float dx0 = xi + t, dy0 = yi + t;

            // First vertex.
            float a0 = RSquared2D - dx0 * dx0 - dy0 * dy0;
            long hash0 = HashPrimes(seed, xsbp, ysbp);
            FVectorXY value = (a0 * a0) * (a0 * a0) * GradFrame(hash0, dx0, dy0);

            // Second vertex.
            float a1 = (float)(2 * (1 + 2 * Unskew2D) * (1 / Unskew2D + 2)) * t + ((float)(-2 * (1 + 2 * Unskew2D) * (1 + 2 * Unskew2D)) + a0);
            float dx1 = dx0 - (float)(1 + 2 * Unskew2D);
            float dy1 = dy0 - (float)(1 + 2 * Unskew2D);
            long hash1 = HashPrimes(seed, xsbp + PrimeX, ysbp + PrimeY);
            value += (a1 * a1) * (a1 * a1) * GradFrame(hash1, dx1, dy1);

            // Third and fourth vertices.
            // Nested conditionals were faster than compact bit logic/arithmetic.
            float xmyi = xi - yi;
            if (t < Unskew2D)
            {
                if (xi + xmyi > 1)
                {
                    float dx2 = dx0 - (float)(3 * Unskew2D + 2);
                    float dy2 = dy0 - (float)(3 * Unskew2D + 1);
                    float a2 = RSquared2D - dx2 * dx2 - dy2 * dy2;
                    if (a2 > 0)
                    {
                        long hash2 = HashPrimes(seed, xsbp + (PrimeX << 1), ysbp + PrimeY);
                        value += (a2 * a2) * (a2 * a2) * GradFrame(hash2, dx2, dy2);
                    }
                }
                else
                {
                    float dx2 = dx0 - (float)Unskew2D;
                    float dy2 = dy0 - (float)(Unskew2D + 1);
                    float a2 = RSquared2D - dx2 * dx2 - dy2 * dy2;
                    if (a2 > 0)
                    {
                        long hash2 = HashPrimes(seed, xsbp, ysbp + PrimeY);
                        value += (a2 * a2) * (a2 * a2) * GradFrame(hash2, dx2, dy2);
                    }
                }

                if (yi - xmyi > 1)
                {
                    float dx3 = dx0 - (float)(3 * Unskew2D + 1);
                    float dy3 = dy0 - (float)(3 * Unskew2D + 2);
                    float a3 = RSquared2D - dx3 * dx3 - dy3 * dy3;
                    if (a3 > 0)
                    {
                        long hash3 = HashPrimes(seed, xsbp + PrimeX, ysbp + (PrimeY << 1));
                        value += (a3 * a3) * (a3 * a3) * GradFrame(hash3, dx3, dy3);
                    }
                }
                else
                {
                    float dx3 = dx0 - (float)(Unskew2D + 1);
                    float dy3 = dy0 - (float)Unskew2D;
                    float a3 = RSquared2D - dx3 * dx3 - dy3 * dy3;
                    if (a3 > 0)
                    {
                        long hash3 = HashPrimes(seed, xsbp + PrimeX, ysbp);
                        value += (a3 * a3) * (a3 * a3) * GradFrame(hash3, dx3, dy3);
                    }
                }
            }
            else
            {
                if (xi + xmyi < 0)
                {
                    float dx2 = dx0 + (float)(1 + Unskew2D);
                    float dy2 = dy0 + (float)Unskew2D;
                    float a2 = RSquared2D - dx2 * dx2 - dy2 * dy2;
                    if (a2 > 0)
                    {
                        long hash2 = HashPrimes(seed, xsbp - PrimeX, ysbp);
                        value += (a2 * a2) * (a2 * a2) * GradFrame(hash2, dx2, dy2);
                    }
                }
                else
                {
                    float dx2 = dx0 - (float)(Unskew2D + 1);
                    float dy2 = dy0 - (float)Unskew2D;
                    float a2 = RSquared2D - dx2 * dx2 - dy2 * dy2;
                    if (a2 > 0)
                    {
                        long hash2 = HashPrimes(seed, xsbp + PrimeX, ysbp);
                        value += (a2 * a2) * (a2 * a2) * GradFrame(hash2, dx2, dy2);
                    }
                }

                if (yi < xmyi)
                {
                    float dx3 = dx0 + (float)Unskew2D;
                    float dy3 = dy0 + (float)(Unskew2D + 1);
                    float a3 = RSquared2D - dx3 * dx3 - dy3 * dy3;
                    if (a3 > 0)
                    {
                        long hash3 = HashPrimes(seed, xsbp, ysbp - PrimeY);
                        value += (a3 * a3) * (a3 * a3) * GradFrame(hash3, dx3, dy3);
                    }
                }
                else
                {
                    float dx3 = dx0 - (float)Unskew2D;
                    float dy3 = dy0 - (float)(Unskew2D + 1);
                    float a3 = RSquared2D - dx3 * dx3 - dy3 * dy3;
                    if (a3 > 0)
                    {
                        long hash3 = HashPrimes(seed, xsbp, ysbp + PrimeY);
                        value += (a3 * a3) * (a3 * a3) * GradFrame(hash3, dx3, dy3);
                    }
                }
            }

            valueX = value.X;
            valueY = value.Y;
        }

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
            float a0 = RSquared3D - x0 * x0 - y0 * y0 - z0 * z0;
            long hash0 = HashPrimes(seed, xrbp + (xNMask & PrimeX), yrbp + (yNMask & PrimeY), zrbp + (zNMask & PrimeZ));
            float value = (a0 * a0) * (a0 * a0) * Grad(hash0, x0, y0, z0);

            // Second vertex.
            float x1 = xi - 0.5f;
            float y1 = yi - 0.5f;
            float z1 = zi - 0.5f;
            float a1 = RSquared3D - x1 * x1 - y1 * y1 - z1 * z1;
            long hash1 = HashPrimes(seed2, xrbp + PrimeX, yrbp + PrimeY, zrbp + PrimeZ);
            value += (a1 * a1) * (a1 * a1) * Grad(hash1, x1, y1, z1);

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
                float x2 = x0 - (xNMask | 1);
                float y2 = y0;
                float z2 = z0;
                long hash2 = HashPrimes(seed, xrbp + (~xNMask & PrimeX), yrbp + (yNMask & PrimeY), zrbp + (zNMask & PrimeZ));
                value += (a2 * a2) * (a2 * a2) * Grad(hash2, x2, y2, z2);
            }
            else
            {
                float a3 = yAFlipMask0 + zAFlipMask0 + a0;
                if (a3 > 0)
                {
                    float x3 = x0;
                    float y3 = y0 - (yNMask | 1);
                    float z3 = z0 - (zNMask | 1);
                    long hash3 = HashPrimes(seed, xrbp + (xNMask & PrimeX), yrbp + (~yNMask & PrimeY), zrbp + (~zNMask & PrimeZ));
                    value += (a3 * a3) * (a3 * a3) * Grad(hash3, x3, y3, z3);
                }

                float a4 = xAFlipMask1 + a1;
                if (a4 > 0)
                {
                    float x4 = (xNMask | 1) + x1;
                    float y4 = y1;
                    float z4 = z1;
                    long hash4 = HashPrimes(seed2, xrbp + (xNMask & unchecked(PrimeX * 2)), yrbp + PrimeY, zrbp + PrimeZ);
                    value += (a4 * a4) * (a4 * a4) * Grad(hash4, x4, y4, z4);
                    skip5 = true;
                }
            }

            bool skip9 = false;
            float a6 = yAFlipMask0 + a0;
            if (a6 > 0)
            {
                float x6 = x0;
                float y6 = y0 - (yNMask | 1);
                float z6 = z0;
                long hash6 = HashPrimes(seed, xrbp + (xNMask & PrimeX), yrbp + (~yNMask & PrimeY), zrbp + (zNMask & PrimeZ));
                value += (a6 * a6) * (a6 * a6) * Grad(hash6, x6, y6, z6);
            }
            else
            {
                float a7 = xAFlipMask0 + zAFlipMask0 + a0;
                if (a7 > 0)
                {
                    float x7 = x0 - (xNMask | 1);
                    float y7 = y0;
                    float z7 = z0 - (zNMask | 1);
                    long hash7 = HashPrimes(seed, xrbp + (~xNMask & PrimeX), yrbp + (yNMask & PrimeY), zrbp + (~zNMask & PrimeZ));
                    value += (a7 * a7) * (a7 * a7) * Grad(hash7, x7, y7, z7);
                }

                float a8 = yAFlipMask1 + a1;
                if (a8 > 0)
                {
                    float x8 = x1;
                    float y8 = (yNMask | 1) + y1;
                    float z8 = z1;
                    long hash8 = HashPrimes(seed2, xrbp + PrimeX, yrbp + (yNMask & (PrimeY << 1)), zrbp + PrimeZ);
                    value += (a8 * a8) * (a8 * a8) * Grad(hash8, x8, y8, z8);
                    skip9 = true;
                }
            }

            bool skipD = false;
            float aA = zAFlipMask0 + a0;
            if (aA > 0)
            {
                float xA = x0;
                float yA = y0;
                float zA = z0 - (zNMask | 1);
                long hashA = HashPrimes(seed, xrbp + (xNMask & PrimeX), yrbp + (yNMask & PrimeY), zrbp + (~zNMask & PrimeZ));
                value += (aA * aA) * (aA * aA) * Grad(hashA, xA, yA, zA);
            }
            else
            {
                float aB = xAFlipMask0 + yAFlipMask0 + a0;
                if (aB > 0)
                {
                    float xB = x0 - (xNMask | 1);
                    float yB = y0 - (yNMask | 1);
                    float zB = z0;
                    long hashB = HashPrimes(seed, xrbp + (~xNMask & PrimeX), yrbp + (~yNMask & PrimeY), zrbp + (zNMask & PrimeZ));
                    value += (aB * aB) * (aB * aB) * Grad(hashB, xB, yB, zB);
                }

                float aC = zAFlipMask1 + a1;
                if (aC > 0)
                {
                    float xC = x1;
                    float yC = y1;
                    float zC = (zNMask | 1) + z1;
                    long hashC = HashPrimes(seed2, xrbp + PrimeX, yrbp + PrimeY, zrbp + (zNMask & (PrimeZ << 1)));
                    value += (aC * aC) * (aC * aC) * Grad(hashC, xC, yC, zC);
                    skipD = true;
                }
            }

            if (!skip5)
            {
                float a5 = yAFlipMask1 + zAFlipMask1 + a1;
                if (a5 > 0)
                {
                    float x5 = x1;
                    float y5 = (yNMask | 1) + y1;
                    float z5 = (zNMask | 1) + z1;
                    long hash5 = HashPrimes(seed2, xrbp + PrimeX, yrbp + (yNMask & (PrimeY << 1)), zrbp + (zNMask & (PrimeZ << 1)));
                    value += (a5 * a5) * (a5 * a5) * Grad(hash5, x5, y5, z5);
                }
            }

            if (!skip9)
            {
                float a9 = xAFlipMask1 + zAFlipMask1 + a1;
                if (a9 > 0)
                {
                    float x9 = (xNMask | 1) + x1;
                    float y9 = y1;
                    float z9 = (zNMask | 1) + z1;
                    long hash9 = HashPrimes(seed2, xrbp + (xNMask & unchecked(PrimeX * 2)), yrbp + PrimeY, zrbp + (zNMask & (PrimeZ << 1)));
                    value += (a9 * a9) * (a9 * a9) * Grad(hash9, x9, y9, z9);
                }
            }

            if (!skipD)
            {
                float aD = xAFlipMask1 + yAFlipMask1 + a1;
                if (aD > 0)
                {
                    float xD = (xNMask | 1) + x1;
                    float yD = (yNMask | 1) + y1;
                    float zD = z1;
                    long hashD = HashPrimes(seed2, xrbp + (xNMask & (PrimeX << 1)), yrbp + (yNMask & (PrimeY << 1)), zrbp + PrimeZ);
                    value += (aD * aD) * (aD * aD) * Grad(hashD, xD, yD, zD);
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
        private static long HashPrimes(long seed, long xsvp, long ysvp)
        {
            return seed ^ xsvp ^ ysvp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long HashPrimes(long seed, long xsvp, long ysvp, long zsvp)
        {
            return seed ^ xsvp ^ ysvp ^ zsvp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Grad(long hash, float dx, float dy)
        {
            hash *= HashMultiplier;
            hash ^= hash >> (64 - NGrads2DExponent - 1);
            int gi = (int)hash & ((NGrads2D - 1) << 1);
            return Gradients2D[gi | 0] * dx + Gradients2D[gi | 1] * dy;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static FVectorXY GradFrame(long hash, float dx, float dy)
        {
            hash *= HashMultiplier;
            hash ^= hash >> (64 - NFrames2DExponent - 3);

            int gi = (int)hash & ((NFrames2D - 1) << 3);
            int flipChannel = (int)hash & 2;
            int flipSign0 = (int)(hash << 2) & 4;
            int flipSign1 = (int)hash & 4;

            // Two noise gradients, 90 degrees to each other.
            // As long as these are constrained perpendicular like this,
            // and as long as the noise uses radially-symmetric vertex falloff curves (it does),
            // then this is sufficient to make the warping isotropic!
            // We don't need to sample extra vectors to rotate the output frame.
            float rampA = FrameGradients2D[gi | (0 ^ flipChannel) | flipSign0] * dx + FrameGradients2D[gi | (1 ^ flipChannel) | flipSign0] * dy;
            float rampB = FrameGradients2D[gi | (2 ^ flipChannel) | flipSign1] * dx + FrameGradients2D[gi | (3 ^ flipChannel) | flipSign1] * dy;

            return new FVectorXY(rampA, rampB);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Grad(long hash, float dx, float dy, float dz)
        {
            hash *= HashMultiplier;
            hash ^= hash >> (64 - NGrads3DExponent - 2);
            int gi = (int)hash & ((NGrads3D - 1) << 2);
            return Gradients3D[gi | 0] * dx + Gradients3D[gi | 1] * dy + Gradients3D[gi | 2] * dz;
        }

        private static readonly float[] Gradients2D;
        private static readonly float[] Gradients3D;
        private static readonly float[] FrameGradients2D;
        static NewSimplexNoiseLayer()
        {
            double[] grad2 = {
                 0.38268343236509,   0.923879532511287,
                 0.923879532511287,  0.38268343236509,
                 0.923879532511287, -0.38268343236509,
                 0.38268343236509,  -0.923879532511287,
                -0.38268343236509,  -0.923879532511287,
                -0.923879532511287, -0.38268343236509,
                -0.923879532511287,  0.38268343236509,
                -0.38268343236509,   0.923879532511287,
                //-------------------------------------//
                 0.130526192220052,  0.99144486137381,
                 0.608761429008721,  0.793353340291235,
                 0.793353340291235,  0.608761429008721,
                 0.99144486137381,   0.130526192220051,
                 0.99144486137381,  -0.130526192220051,
                 0.793353340291235, -0.60876142900872,
                 0.608761429008721, -0.793353340291235,
                 0.130526192220052, -0.99144486137381,
                -0.130526192220052, -0.99144486137381,
                -0.608761429008721, -0.793353340291235,
                -0.793353340291235, -0.608761429008721,
                -0.99144486137381,  -0.130526192220052,
                -0.99144486137381,   0.130526192220051,
                -0.793353340291235,  0.608761429008721,
                -0.608761429008721,  0.793353340291235,
                -0.130526192220052,  0.99144486137381,
            };
            Gradients2D = new float[NGrads2D * 2];
            for (int i = 0; i < grad2.Length; i++)
            {
                grad2[i] /= Normalizer2D;
            }
            for (int i = 0, j = 0; i < Gradients2D.Length; i++, j++)
            {
                if (j == grad2.Length) j = 0;
                Gradients2D[i] = (float)grad2[j];
            }

            double[] grad3 = {
                 2.22474487139,       2.22474487139,      -1.0,                 0.0,
                 2.22474487139,       2.22474487139,       1.0,                 0.0,
                 3.0862664687972017,  1.1721513422464978,  0.0,                 0.0,
                 1.1721513422464978,  3.0862664687972017,  0.0,                 0.0,
                -2.22474487139,       2.22474487139,      -1.0,                 0.0,
                -2.22474487139,       2.22474487139,       1.0,                 0.0,
                -1.1721513422464978,  3.0862664687972017,  0.0,                 0.0,
                -3.0862664687972017,  1.1721513422464978,  0.0,                 0.0,
                -1.0,                -2.22474487139,      -2.22474487139,       0.0,
                 1.0,                -2.22474487139,      -2.22474487139,       0.0,
                 0.0,                -3.0862664687972017, -1.1721513422464978,  0.0,
                 0.0,                -1.1721513422464978, -3.0862664687972017,  0.0,
                -1.0,                -2.22474487139,       2.22474487139,       0.0,
                 1.0,                -2.22474487139,       2.22474487139,       0.0,
                 0.0,                -1.1721513422464978,  3.0862664687972017,  0.0,
                 0.0,                -3.0862664687972017,  1.1721513422464978,  0.0,
                //--------------------------------------------------------------------//
                -2.22474487139,      -2.22474487139,      -1.0,                 0.0,
                -2.22474487139,      -2.22474487139,       1.0,                 0.0,
                -3.0862664687972017, -1.1721513422464978,  0.0,                 0.0,
                -1.1721513422464978, -3.0862664687972017,  0.0,                 0.0,
                -2.22474487139,      -1.0,                -2.22474487139,       0.0,
                -2.22474487139,       1.0,                -2.22474487139,       0.0,
                -1.1721513422464978,  0.0,                -3.0862664687972017,  0.0,
                -3.0862664687972017,  0.0,                -1.1721513422464978,  0.0,
                -2.22474487139,      -1.0,                 2.22474487139,       0.0,
                -2.22474487139,       1.0,                 2.22474487139,       0.0,
                -3.0862664687972017,  0.0,                 1.1721513422464978,  0.0,
                -1.1721513422464978,  0.0,                 3.0862664687972017,  0.0,
                -1.0,                 2.22474487139,      -2.22474487139,       0.0,
                 1.0,                 2.22474487139,      -2.22474487139,       0.0,
                 0.0,                 1.1721513422464978, -3.0862664687972017,  0.0,
                 0.0,                 3.0862664687972017, -1.1721513422464978,  0.0,
                -1.0,                 2.22474487139,       2.22474487139,       0.0,
                 1.0,                 2.22474487139,       2.22474487139,       0.0,
                 0.0,                 3.0862664687972017,  1.1721513422464978,  0.0,
                 0.0,                 1.1721513422464978,  3.0862664687972017,  0.0,
                 2.22474487139,      -2.22474487139,      -1.0,                 0.0,
                 2.22474487139,      -2.22474487139,       1.0,                 0.0,
                 1.1721513422464978, -3.0862664687972017,  0.0,                 0.0,
                 3.0862664687972017, -1.1721513422464978,  0.0,                 0.0,
                 2.22474487139,      -1.0,                -2.22474487139,       0.0,
                 2.22474487139,       1.0,                -2.22474487139,       0.0,
                 3.0862664687972017,  0.0,                -1.1721513422464978,  0.0,
                 1.1721513422464978,  0.0,                -3.0862664687972017,  0.0,
                 2.22474487139,      -1.0,                 2.22474487139,       0.0,
                 2.22474487139,       1.0,                 2.22474487139,       0.0,
                 1.1721513422464978,  0.0,                 3.0862664687972017,  0.0,
                 3.0862664687972017,  0.0,                 1.1721513422464978,  0.0,
            };
            Gradients3D = new float[NGrads3D * 4];
            for (int i = 0; i < grad3.Length; i++)
            {
                grad3[i] /= Normalizer3D;
            }
            for (int i = 0, j = 0; i < Gradients3D.Length; i++, j++)
            {
                if (j == grad3.Length) j = 0;
                Gradients3D[i] = (float)grad3[j];
            }

            double[] frameGrads2D = {
                0.012271538285719925,   0.9999247018391445,     0.9999247018391445,     -0.012271538285719925,  -0.012271538285719925, -0.9999247018391445,     -0.9999247018391445,    0.012271538285719925,
                0.03680722294135883,    0.9993223845883495,     0.9993223845883495,     -0.03680722294135883,   -0.03680722294135883,  -0.9993223845883495,     -0.9993223845883495,    0.03680722294135883,
                0.06132073630220858,    0.9981181129001492,     0.9981181129001492,     -0.06132073630220858,   -0.06132073630220858,  -0.9981181129001492,     -0.9981181129001492,    0.06132073630220858,
                0.0857973123444399,     0.996312612182778,      0.996312612182778,      -0.0857973123444399,    -0.0857973123444399,   -0.996312612182778,      -0.996312612182778,     0.0857973123444399,
                0.11022220729388306,    0.9939069700023561,     0.9939069700023561,     -0.11022220729388306,   -0.11022220729388306,  -0.9939069700023561,     -0.9939069700023561,    0.11022220729388306,
                0.13458070850712617,    0.99090263542778,       0.99090263542778,       -0.13458070850712617,   -0.13458070850712617,  -0.99090263542778,       -0.99090263542778,      0.13458070850712617,
                0.15885814333386145,    0.9873014181578584,     0.9873014181578584,     -0.15885814333386145,   -0.15885814333386145,  -0.9873014181578584,     -0.9873014181578584,    0.15885814333386145,
                0.18303988795514095,    0.9831054874312163,     0.9831054874312163,     -0.18303988795514095,   -0.18303988795514095,  -0.9831054874312163,     -0.9831054874312163,    0.18303988795514095,
                0.20711137619221856,    0.9783173707196277,     0.9783173707196277,     -0.20711137619221856,   -0.20711137619221856,  -0.9783173707196277,     -0.9783173707196277,    0.20711137619221856,
                0.2310581082806711,     0.9729399522055602,     0.9729399522055602,     -0.2310581082806711,    -0.2310581082806711,   -0.9729399522055602,     -0.9729399522055602,    0.2310581082806711,
                0.25486565960451457,    0.9669764710448521,     0.9669764710448521,     -0.25486565960451457,   -0.25486565960451457,  -0.9669764710448521,     -0.9669764710448521,    0.25486565960451457,
                0.27851968938505306,    0.9604305194155658,     0.9604305194155658,     -0.27851968938505306,   -0.27851968938505306,  -0.9604305194155658,     -0.9604305194155658,    0.27851968938505306,
                0.3020059493192281,     0.9533060403541939,     0.9533060403541939,     -0.3020059493192281,    -0.3020059493192281,   -0.9533060403541939,     -0.9533060403541939,    0.3020059493192281,
                0.3253102921622629,     0.9456073253805213,     0.9456073253805213,     -0.3253102921622629,    -0.3253102921622629,   -0.9456073253805213,     -0.9456073253805213,    0.3253102921622629,
                0.34841868024943456,    0.937339011912575,      0.937339011912575,      -0.34841868024943456,   -0.34841868024943456,  -0.937339011912575,      -0.937339011912575,     0.34841868024943456,
                0.37131719395183754,    0.9285060804732156,     0.9285060804732156,     -0.37131719395183754,   -0.37131719395183754,  -0.9285060804732156,     -0.9285060804732156,    0.37131719395183754,
                0.3939920400610481,     0.9191138516900578,     0.9191138516900578,     -0.3939920400610481,    -0.3939920400610481,   -0.9191138516900578,     -0.9191138516900578,    0.3939920400610481,
                0.41642956009763715,    0.9091679830905224,     0.9091679830905224,     -0.41642956009763715,   -0.41642956009763715,  -0.9091679830905224,     -0.9091679830905224,    0.41642956009763715,
                0.43861623853852766,    0.8986744656939538,     0.8986744656939538,     -0.43861623853852766,   -0.43861623853852766,  -0.8986744656939538,     -0.8986744656939538,    0.43861623853852766,
                0.46053871095824,       0.8876396204028539,     0.8876396204028539,     -0.46053871095824,      -0.46053871095824,     -0.8876396204028539,     -0.8876396204028539,    0.46053871095824,
                0.4821837720791227,     0.8760700941954066,     0.8760700941954066,     -0.4821837720791227,    -0.4821837720791227,   -0.8760700941954066,     -0.8760700941954066,    0.4821837720791227,
                0.5035383837257176,     0.8639728561215867,     0.8639728561215867,     -0.5035383837257176,    -0.5035383837257176,   -0.8639728561215867,     -0.8639728561215867,    0.5035383837257176,
                0.524589682678469,      0.8513551931052652,     0.8513551931052652,     -0.524589682678469,     -0.524589682678469,    -0.8513551931052652,     -0.8513551931052652,    0.524589682678469,
                0.5453249884220465,     0.8382247055548381,     0.8382247055548381,     -0.5453249884220465,    -0.5453249884220465,   -0.8382247055548381,     -0.8382247055548381,    0.5453249884220465,
                0.5657318107836131,     0.8245893027850253,     0.8245893027850253,     -0.5657318107836131,    -0.5657318107836131,   -0.8245893027850253,     -0.8245893027850253,    0.5657318107836131,
                0.5857978574564389,     0.8104571982525948,     0.8104571982525948,     -0.5857978574564389,    -0.5857978574564389,   -0.8104571982525948,     -0.8104571982525948,    0.5857978574564389,
                0.6055110414043255,     0.7958369046088836,     0.7958369046088836,     -0.6055110414043255,    -0.6055110414043255,   -0.7958369046088836,     -0.7958369046088836,    0.6055110414043255,
                0.6248594881423863,     0.7807372285720945,     0.7807372285720945,     -0.6248594881423863,    -0.6248594881423863,   -0.7807372285720945,     -0.7807372285720945,    0.6248594881423863,
                0.6438315428897914,     0.765167265622459,      0.765167265622459,      -0.6438315428897914,    -0.6438315428897914,   -0.765167265622459,      -0.765167265622459,     0.6438315428897914,
                0.6624157775901718,     0.7491363945234594,     0.7491363945234594,     -0.6624157775901718,    -0.6624157775901718,   -0.7491363945234594,     -0.7491363945234594,    0.6624157775901718,
                0.680600997795453,      0.7326542716724128,     0.7326542716724128,     -0.680600997795453,     -0.680600997795453,    -0.7326542716724128,     -0.7326542716724128,    0.680600997795453,
                0.6983762494089728,     0.7157308252838187,     0.7157308252838187,     -0.6983762494089728,    -0.6983762494089728,   -0.7157308252838187,     -0.7157308252838187,    0.6983762494089728,
                0.7157308252838186,     0.6983762494089729,     0.6983762494089729,     -0.7157308252838186,    -0.7157308252838186,   -0.6983762494089729,     -0.6983762494089729,    0.7157308252838186,
                0.7326542716724128,     0.680600997795453,      0.680600997795453,      -0.7326542716724128,    -0.7326542716724128,   -0.680600997795453,      -0.680600997795453,     0.7326542716724128,
                0.7491363945234593,     0.6624157775901718,     0.6624157775901718,     -0.7491363945234593,    -0.7491363945234593,   -0.6624157775901718,     -0.6624157775901718,    0.7491363945234593,
                0.765167265622459,      0.6438315428897915,     0.6438315428897915,     -0.765167265622459,     -0.765167265622459,    -0.6438315428897915,     -0.6438315428897915,    0.765167265622459,
                0.7807372285720945,     0.6248594881423865,     0.6248594881423865,     -0.7807372285720945,    -0.7807372285720945,   -0.6248594881423865,     -0.6248594881423865,    0.7807372285720945,
                0.7958369046088835,     0.6055110414043255,     0.6055110414043255,     -0.7958369046088835,    -0.7958369046088835,   -0.6055110414043255,     -0.6055110414043255,    0.7958369046088835,
                0.8104571982525948,     0.5857978574564389,     0.5857978574564389,     -0.8104571982525948,    -0.8104571982525948,   -0.5857978574564389,     -0.5857978574564389,    0.8104571982525948,
                0.8245893027850253,     0.5657318107836132,     0.5657318107836132,     -0.8245893027850253,    -0.8245893027850253,   -0.5657318107836132,     -0.5657318107836132,    0.8245893027850253,
                0.838224705554838,      0.5453249884220465,     0.5453249884220465,     -0.838224705554838,     -0.838224705554838,    -0.5453249884220465,     -0.5453249884220465,    0.838224705554838,
                0.8513551931052652,     0.5245896826784688,     0.5245896826784688,     -0.8513551931052652,    -0.8513551931052652,   -0.5245896826784688,     -0.5245896826784688,    0.8513551931052652,
                0.8639728561215867,     0.5035383837257176,     0.5035383837257176,     -0.8639728561215867,    -0.8639728561215867,   -0.5035383837257176,     -0.5035383837257176,    0.8639728561215867,
                0.8760700941954066,     0.48218377207912283,    0.48218377207912283,    -0.8760700941954066,    -0.8760700941954066,   -0.48218377207912283,    -0.48218377207912283,   0.8760700941954066,
                0.8876396204028539,     0.46053871095824,       0.46053871095824,       -0.8876396204028539,    -0.8876396204028539,   -0.46053871095824,       -0.46053871095824,      0.8876396204028539,
                0.8986744656939538,     0.4386162385385277,     0.4386162385385277,     -0.8986744656939538,    -0.8986744656939538,   -0.4386162385385277,     -0.4386162385385277,    0.8986744656939538,
                0.9091679830905223,     0.4164295600976373,     0.4164295600976373,     -0.9091679830905223,    -0.9091679830905223,   -0.4164295600976373,     -0.4164295600976373,    0.9091679830905223,
                0.9191138516900578,     0.3939920400610481,     0.3939920400610481,     -0.9191138516900578,    -0.9191138516900578,   -0.3939920400610481,     -0.3939920400610481,    0.9191138516900578,
                0.9285060804732156,     0.3713171939518376,     0.3713171939518376,     -0.9285060804732156,    -0.9285060804732156,   -0.3713171939518376,     -0.3713171939518376,    0.9285060804732156,
                0.937339011912575,      0.3484186802494345,     0.3484186802494345,     -0.937339011912575,     -0.937339011912575,    -0.3484186802494345,     -0.3484186802494345,    0.937339011912575,
                0.9456073253805213,     0.325310292162263,      0.325310292162263,      -0.9456073253805213,    -0.9456073253805213,   -0.325310292162263,      -0.325310292162263,     0.9456073253805213,
                0.9533060403541938,     0.3020059493192282,     0.3020059493192282,     -0.9533060403541938,    -0.9533060403541938,   -0.3020059493192282,     -0.3020059493192282,    0.9533060403541938,
                0.9604305194155658,     0.27851968938505306,    0.27851968938505306,    -0.9604305194155658,    -0.9604305194155658,   -0.27851968938505306,    -0.27851968938505306,   0.9604305194155658,
                0.9669764710448521,     0.2548656596045146,     0.2548656596045146,     -0.9669764710448521,    -0.9669764710448521,   -0.2548656596045146,     -0.2548656596045146,    0.9669764710448521,
                0.9729399522055601,     0.23105810828067128,    0.23105810828067128,    -0.9729399522055601,    -0.9729399522055601,   -0.23105810828067128,    -0.23105810828067128,   0.9729399522055601,
                0.9783173707196277,     0.20711137619221856,    0.20711137619221856,    -0.9783173707196277,    -0.9783173707196277,   -0.20711137619221856,    -0.20711137619221856,   0.9783173707196277,
                0.9831054874312163,     0.18303988795514106,    0.18303988795514106,    -0.9831054874312163,    -0.9831054874312163,   -0.18303988795514106,    -0.18303988795514106,   0.9831054874312163,
                0.9873014181578584,     0.1588581433338614,     0.1588581433338614,     -0.9873014181578584,    -0.9873014181578584,   -0.1588581433338614,     -0.1588581433338614,    0.9873014181578584,
                0.99090263542778,       0.13458070850712622,    0.13458070850712622,    -0.99090263542778,      -0.99090263542778,     -0.13458070850712622,    -0.13458070850712622,   0.99090263542778,
                0.9939069700023561,     0.11022220729388318,    0.11022220729388318,    -0.9939069700023561,    -0.9939069700023561,   -0.11022220729388318,    -0.11022220729388318,   0.9939069700023561,
                0.996312612182778,      0.08579731234443988,    0.08579731234443988,    -0.996312612182778,     -0.996312612182778,    -0.08579731234443988,    -0.08579731234443988,   0.996312612182778,
                0.9981181129001492,     0.06132073630220865,    0.06132073630220865,    -0.9981181129001492,    -0.9981181129001492,   -0.06132073630220865,    -0.06132073630220865,   0.9981181129001492,
                0.9993223845883495,     0.03680722294135899,    0.03680722294135899,    -0.9993223845883495,    -0.9993223845883495,   -0.03680722294135899,    -0.03680722294135899,   0.9993223845883495,
                0.9999247018391445,     0.012271538285719944,   0.012271538285719944,   -0.9999247018391445,    -0.9999247018391445,   -0.012271538285719944,   -0.012271538285719944,  0.9999247018391445,
            };
            FrameGradients2D = new float[frameGrads2D.Length];
            for (int i = 0; i < frameGrads2D.Length; i++)
            {
                FrameGradients2D[i] = (float)(frameGrads2D[i] / FrameNormalizer2D);
            }
        }

        struct FVectorXY
        {
            public float X, Y;
            public FVectorXY(float x, float y)
            {
                X = x;
                Y = y;
            }
            public static FVectorXY operator +(FVectorXY a, FVectorXY b) => new FVectorXY(a.X + b.X, a.Y + b.Y);
            public static FVectorXY operator *(FVectorXY a, float b) => new FVectorXY(a.X * b, a.Y * b);
            public static FVectorXY operator *(float b, FVectorXY a) => new FVectorXY(a.X * b, a.Y * b);
        }
    }

}
