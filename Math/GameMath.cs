using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Vintagestory.API.Datastructures;

#nullable disable

namespace Vintagestory.API.MathTools
{
    public static class Easings
    {
        public static double EaseOutBack(double x)
        {
            double c1 = 1.70158f;
            double c3 = c1 + 1;

            return 1 + c3 * Math.Pow(x - 1, 3) + c1 * Math.Pow(x - 1, 2);
        }
    }



    /// <summary>
    /// A large set of useful game mathematics functions
    /// </summary>
    public static class GameMath
    {
        /// <summary>
        /// 360°
        /// </summary>
        public const float TWOPI = (float)Math.PI * 2;
        /// <summary>
        /// 180°
        /// </summary>
        public const float PI = (float)Math.PI;
        /// <summary>
        /// 90°
        /// </summary>
        public const float PIHALF = (float)Math.PI / 2;

        public const double DEG2RAD_DOUBLE = Math.PI / 180.0;
        public const float DEG2RAD = (float)Math.PI / 180.0f;
        public const float RAD2DEG = 180.0f / (float)Math.PI;
        const uint murmurseed = 144;


        #region Standard Sin/Cos Methods
        /// <summary>
        /// The sine of the angle, expressed as a float.  The angle is in radians
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Sin(float value)
        {
            return (float)Math.Sin(value);
        }

        /// <summary>
        /// The cosine of the angle, expressed as a float.  The angle is in radians
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float Cos(float value)
        {
            return (float)Math.Cos(value);
        }

        public static float Acos(float value)
        {
            return (float)Math.Acos(value);
        }

        public static float Asin(float value)
        {
            return (float)Math.Asin(value);
        }


        public static float Tan(float value)
        {
            return (float)Math.Tan(value);
        }


        public static double Sin(double value)
        {
            return Math.Sin(value);
        }

        public static double Cos(double value)
        {
            return Math.Cos(value);
        }

        public static double Acos(double value)
        {
            return Math.Acos(value);
        }

        public static double Asin(double value)
        {
            return Math.Asin(value);
        }

        public static double Tan(double value)
        {
            return Math.Tan(value);
        }


        // http://www.java-gaming.org/index.php?PHPSESSID=cuplcuatuqd8i9imq70sf7kt41&topic=24191.0

        /// <summary>
        /// Faster Sin at the cost of lower accuracy
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        public static float FastSin(float rad)
        {
            return sinValues[(int)(rad * radToIndex) & SIN_MASK];
        }

        /// <summary>
        /// Faster Cos at the cost of lower accuracy
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        public static float FastCos(float rad)
        {
            return cosValues[(int)(rad * radToIndex) & SIN_MASK];
        }

        /// <summary>
        /// Faster Sin at the cost of lower accuracy
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static float FastSinDeg(float deg)
        {
            return sinValues[(int)(deg * degToIndex) & SIN_MASK];
        }

        /// <summary>
        /// Faster Cos at the cost of lower accuracy
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static float FastCosDeg(float deg)
        {
            return cosValues[(int)(deg * degToIndex) & SIN_MASK];
        }

        private static int SIN_BITS, SIN_MASK, SIN_COUNT;
        private static float radFull, radToIndex;
        private static float degFull, degToIndex;
        private static float[] sinValues, cosValues;

        static GameMath()
        {

            SIN_BITS  = 12;
            SIN_MASK  = ~(-1 << SIN_BITS);
            SIN_COUNT = SIN_MASK + 1;

            radFull    = (float) (Math.PI* 2.0);
            degFull    = (float) (360.0);
            radToIndex = SIN_COUNT / radFull;
            degToIndex = SIN_COUNT / degFull;

            sinValues = new float[SIN_COUNT];
            cosValues = new float[SIN_COUNT];

            for (int i = 0; i<SIN_COUNT; i++)
            {
                sinValues[i] = (float) Math.Sin((i + 0.5f) / SIN_COUNT* radFull);
                cosValues[i] = (float) Math.Cos((i + 0.5f) / SIN_COUNT* radFull);
            }

            // Four cardinal directions (credits: Nate)
            for (int i = 0; i< 360; i += 90)
            {
                sinValues[(int)(i * degToIndex) & SIN_MASK] = (float)Math.Sin(i* Math.PI / 180.0);
                cosValues[(int)(i * degToIndex) & SIN_MASK] = (float)Math.Cos(i* Math.PI / 180.0);
            }
        }
        #endregion

        #region Fast and Slow Sqrts

        public static float Sqrt(float value)
        {
            return (float)Math.Sqrt(value);
        }

        public static float Sqrt(double value)
        {
            return (float)Math.Sqrt(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float RootSumOfSquares(float a, float b, float c)
        {
            return (float)Math.Sqrt(a * a + b * b + c * c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SumOfSquares(double a, double b, double c)
        {
            return a * a + b * b + c * c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Square(double a)
        {
            return a * a;
        }

        #endregion

        #region Clamping

        /// <summary>
        /// Force val to be inside a certain range
        /// </summary>
        /// <param name="val"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float val, float min, float max)
        {
            return val < min ? min : val > max ? max : val;
        }

        /// <summary>
        /// Force val to be inside a certain range
        /// </summary>
        /// <param name="val"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(int val, int min, int max)
        {
            return val < min ? min : val > max ? max : val;
        }

        /// <summary>
        /// Force val to be inside a certain range
        /// </summary>
        /// <param name="val"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Clamp(byte val, byte min, byte max)
        {
            return val < min ? min : val > max ? max : val;
        }

        /// <summary>
        /// Force val to be inside a certain range
        /// </summary>
        /// <param name="val"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp(double val, double min, double max)
        {
            return val < min ? min : val > max ? max : val;
        }

        /// <summary>
        /// Force val to be outside a certain range
        /// </summary>
        /// <param name="val"></param>
        /// <param name="atLeastNeg"></param>
        /// <param name="atLeastPos"></param>
        /// <returns></returns>
        public static int InverseClamp(int val, int atLeastNeg, int atLeastPos)
        {
            return val < atLeastPos ? atLeastPos : val > atLeastNeg ? atLeastNeg : val;
        }

        #endregion

        #region Modulo
        /// <summary>
        /// C#'s %-Operation is actually not modulo but remainder, so this is the actual modulo function that ensures positive numbers as return value
        /// </summary>
        /// <param name="k"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int Mod(int k, int n)
        {
            return ((k %= n) < 0) ? k + n : k;
        }

        public static uint Mod(uint k, uint n)
        {
            return ((k %= n) < 0) ? k + n : k;
        }

        /// <summary>
        /// C#'s %-Operation is actually not modulo but remainder, so this is the actual modulo function that ensures positive numbers as return value
        /// </summary>
        /// <param name="k"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static float Mod(float k, float n)
        {
            return ((k %= n) < 0) ? k + n : k;
        }

        /// <summary>
        /// C#'s %-Operation is actually not modulo but remainder, so this is the actual modulo function that ensures positive numbers as return value
        /// </summary>
        /// <param name="k"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double Mod(double k, double n)
        {
            return ((k %= n) < 0) ? k + n : k;
        }

        #endregion

        #region Interpolation

        /// <summary>
        /// Treats given value as a statistical average. Example: 2.1 will turn into 2 90% of the times and into 3 10% of times.
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int RoundRandom(Random rand, float value)
        {
            return (int)value + ((rand.NextDouble() < (value - (int)value)) ? 1 : 0);
        }

        /// <summary>
        /// Treats given value as a statistical average. Example: 2.1 will turn into 2 90% of the times and into 3 10% of times.
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int RoundRandom(IRandom rand, float value)
        {
            return (int)value + ((rand.NextDouble() < (value - (int)value)) ? 1 : 0);
        }




        /// <summary>
        /// Returns the shortest distance between 2 angles
        /// See also https://stackoverflow.com/a/14498790/1873041
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static float AngleDegDistance(float start, float end)
        {
            return ((((end - start) % 360) + 540) % 360) - 180;
        }

        /// <summary>
        /// Returns the shortest distance between 2 angles
        /// See also <seealso href="https://stackoverflow.com/a/14498790/1873041">https://stackoverflow.com/a/14498790/1873041</seealso>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static float AngleRadDistance(float start, float end)
        {
            return ((((end - start) % TWOPI) + TWOPI + PI) % TWOPI) - PI;
        }

        /// <summary>
        /// For angles in radians, normalise to the range 0 to 2 * PI and also, if barely close to a right angle, set it to a right angle (fixes loss of precision after multiple rotation operations etc.)
        /// </summary>
        /// <param name="angleRad"></param>
        /// <returns></returns>
        public static float NormaliseAngleRad(float angleRad)
        {
            float epsilon = 0.0000032f;
            float angle = GameMath.Mod(angleRad, GameMath.TWOPI);
            if ((angle + epsilon) % GameMath.PIHALF <= epsilon * 2)    // If within epsilon of a right angle, set it to a right angle
            {
                int quadrant = (int)((angle + epsilon) / GameMath.PIHALF) % 4;
                angle = quadrant * GameMath.PIHALF;
            }
            return angle;
        }

        /// <summary>
        /// Returns the smallest number, ignoring the sign of either value. Examples:<br/>
        /// Smallest(1, 3) returns 1
        /// Smallest(-20, 3) returns 3
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double Smallest(double a, double b)
        {
            double ap = Math.Abs(a);
            double bp = Math.Abs(b);
            if (ap < bp)
            {
                return a;
            }
            return b;
        }


        /// <summary>
        /// Returns the smallest number, ignoring the sign of either value
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double Largest(double a, double b)
        {
            double ap = Math.Abs(a);
            double bp = Math.Abs(b);
            if (ap > bp)
            {
                return a;
            }
            return b;
        }


        /// <summary>
        /// Returns the shortest distance between 2 values that are cyclical (e.g. angles, daytime hours, etc.)
        /// See also <seealso href="https://stackoverflow.com/a/14498790/1873041">https://stackoverflow.com/a/14498790/1873041</seealso>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public static float CyclicValueDistance(float start, float end, float period)
        {
            return ((((end - start) % period) + period * 1.5f) % period) - period/2;
        }

        /// <summary>
        /// Returns the shortest distance between 2 values that are cyclical (e.g. angles, daytime hours, etc.)
        /// See also <seealso href="https://stackoverflow.com/a/14498790/1873041">https://stackoverflow.com/a/14498790/1873041</seealso>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public static double CyclicValueDistance(double start, double end, double period)
        {
            return ((((end - start) % period) + period * 1.5f) % period) - period / 2;
        }


        /// <summary>
        /// Generates a gaussian blur kernel to be used when blurring something
        /// </summary>
        /// <param name="sigma"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static double[,] GenGaussKernel(double sigma = 1, int size = 5)
        {
            double[,] kernel = new double[size, size];
            double mean = size / 2.0;
            double sum = 0.0; // For accumulating the kernel values

            for (int x = 0; x < size; ++x)
            {
                for (int y = 0; y < size; ++y)
                {
                    kernel[x, y] = Math.Exp(-0.5 * (Math.Pow((x - mean) / sigma, 2.0) + Math.Pow((y - mean) / sigma, 2.0)))
                                     / (2 * Math.PI * sigma * sigma);

                    // Accumulate the kernel values
                    sum += kernel[x, y];
                }
            }

            // Normalize the kernel
            for (int x = 0; x< size; ++x)
            {
                for (int y = 0; y < size; ++y)
                {
                    kernel[x, y] /= sum;
                }
            }

            return kernel;
        }


        /// <summary>
        /// Does linear interpolation on a 2d map for each of the 4 bytes individually (e.g. RGBA color). It's basically a bilinear zoom of an image like you know it from common image editing software. Only intended for square images.
        /// The resulting map will be without any paddding (also requires at least 1 padding at bottom and left side)
        /// </summary>
        /// <param name="map"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public static int[] BiLerpColorMap(IntDataMap2D map, int zoom)
        {
            int innerSize = map.InnerSize;
            int outSize = innerSize * zoom;
            int[] result = new int[outSize * outSize];

            int x, z;
            int pad = map.TopLeftPadding;

            for (int inX = 0; inX < innerSize; inX++)
            {
                for (int inZ = 0; inZ < innerSize; inZ++)
                {
                    int leftTop = map.Data[(inZ+pad) * map.Size + inX + pad];
                    int rightTop = map.Data[(inZ+pad) * map.Size + inX + 1 + pad];
                    int leftBottom = map.Data[(inZ + 1 + pad) * map.Size + inX + pad];
                    int rightBottom = map.Data[(inZ + 1 + pad) * map.Size + inX + 1 + pad];

                    for (int dz = 0; dz < zoom; dz++)
                    {
                        z = inZ * zoom + dz;

                        for (int dx = 0; dx < zoom; dx++)
                        {
                            x = inX * zoom + dx;

                            result[z * outSize + x] = BiLerpRgbColor(
                                (float)dx / zoom,
                                (float)dz / zoom,
                                leftTop,
                                rightTop,
                                leftBottom,
                                rightBottom
                            );
                        }
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Linear Interpolates one selected bytes of the 4 ints
        /// </summary>
        /// <param name="lx"></param>
        /// <param name="ly"></param>
        /// <param name="byteIndex">0, 1, 2 or 3</param>
        /// <param name="leftTop"></param>
        /// <param name="rightTop"></param>
        /// <param name="leftBottom"></param>
        /// <param name="rightBottom"></param>
        /// <returns></returns>
        public static byte BiLerpByte(float lx, float ly, int byteIndex, int leftTop, int rightTop, int leftBottom, int rightBottom)
        {
            byte top = LerpByte(lx, (byte)(leftTop >> (byteIndex * 8)), (byte)(rightTop >> (byteIndex * 8)));
            byte bottom = LerpByte(lx, (byte)(leftBottom >> (byteIndex * 8)), (byte)(rightBottom >> (byteIndex * 8)));

            return LerpByte(ly, top, bottom);
        }

        /// <summary>
        /// Linear Interpolates one selected bytes of the 4 ints
        /// </summary>
        /// <param name="lx"></param>
        /// <param name="ly"></param>
        /// <param name="byteIndex">0, 1, 2 or 3</param>
        /// <param name="leftTop"></param>
        /// <param name="rightTop"></param>
        /// <param name="leftBottom"></param>
        /// <param name="rightBottom"></param>
        /// <returns></returns>
        public static byte BiSerpByte(float lx, float ly, int byteIndex, int leftTop, int rightTop, int leftBottom, int rightBottom)
        {
            return BiLerpByte(SmoothStep(lx), SmoothStep(ly), byteIndex, leftTop, rightTop, leftBottom, rightBottom);
        }



        /// <summary>
        /// Linear Interpolates the bytes of the int individually (i.e. interpolates RGB values individually)
        /// </summary>
        /// <param name="lx"></param>
        /// <param name="ly"></param>
        /// <param name="leftTop"></param>
        /// <param name="rightTop"></param>
        /// <param name="leftBottom"></param>
        /// <param name="rightBottom"></param>
        /// <returns></returns>
        public static int BiLerpRgbaColor(float lx, float ly, int leftTop, int rightTop, int leftBottom, int rightBottom)
        {
            return BiLerpRgbColor(lx, ly, leftTop, rightTop, leftBottom, rightBottom) +
                (BiLerpAndMask(leftTop >> 4, rightTop >> 4, leftBottom >> 4, rightBottom >> 4, lx, ly, 0xff00000) << 4)    // We can't use mask 0xff000000 as we would like to because that is a negative number
                ;
        }


        /// <summary>
        /// Linear Interpolates the lower 3 bytes of the int individually (i.e. interpolates RGB values individually)
        /// </summary>
        /// <param name="lx"></param>
        /// <param name="ly"></param>
        /// <param name="leftTop"></param>
        /// <param name="rightTop"></param>
        /// <param name="leftBottom"></param>
        /// <param name="rightBottom"></param>
        /// <returns></returns>
        public static int BiLerpRgbColor(float lx, float ly, int leftTop, int rightTop, int leftBottom, int rightBottom)
        {
            return
                BiLerpAndMask(leftTop, rightTop, leftBottom, rightBottom, lx, ly, 0xff) +
                BiLerpAndMask(leftTop, rightTop, leftBottom, rightBottom, lx, ly, 0xff00) +
                BiLerpAndMask(leftTop, rightTop, leftBottom, rightBottom, lx, ly, 0xff0000)
                ;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int BiLerpAndMask(int leftTop, int rightTop, int leftBottom, int rightBottom, float lx, float ly, int mask)
        {
            return (int)Lerp(Lerp(leftTop & mask, rightTop & mask, lx), Lerp(leftBottom & mask, rightBottom & mask, lx), ly) & mask;
        }

        /// <summary>
        /// Smoothstep Interpolates the lower 3 bytes of the int individually (i.e. interpolates RGB values individually)
        /// </summary>
        /// <param name="lx"></param>
        /// <param name="ly"></param>
        /// <param name="leftTop"></param>
        /// <param name="rightTop"></param>
        /// <param name="leftBottom"></param>
        /// <param name="rightBottom"></param>
        /// <returns></returns>
        public static int BiSerpRgbColor(float lx, float ly, int leftTop, int rightTop, int leftBottom, int rightBottom)
        {
            return BiLerpRgbColor(SmoothStep(lx), SmoothStep(ly), leftTop, rightTop, leftBottom, rightBottom);
        }

        /// <summary>
        /// Linear Interpolates the lower 3 bytes of the int individually (i.e. interpolates RGB values individually)
        /// </summary>
        /// <param name="lx"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LerpRgbColor(float lx, int left, int right)
        {
            return
                (int)((1 - lx) * (left & 0xff) + lx * (right & 0xff)) +
                ((int)((1 - lx) * (left & 0xff00) + lx * (right & 0xff00)) & 0xff00) +
                ((int)((1 - lx) * (left & 0xff0000) + lx * (right & 0xff0000)) & 0xff0000)
                ;
        }


        /// <summary>
        /// Linear Interpolates the 4 bytes of the int individually (i.e. interpolates RGB values individually)
        /// </summary>
        /// <param name="lx"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LerpRgbaColor(float lx, int left, int right)
        {
            return
                (int)((1 - lx) * (left & 0xff) + lx * (right & 0xff)) +
                ((int)((1 - lx) * (left & 0xff00) + lx * (right & 0xff00)) & 0xff00) +
                ((int)((1 - lx) * (left & 0xff0000) + lx * (right & 0xff0000)) & 0xff0000) +
                ((int)((1 - lx) * ((left >> 24) & 0xff) + lx * ((right >> 24) & 0xff)) << 24)
                ;
        }

        /// <summary>
        /// Linear Interpolates a single byte
        /// </summary>
        /// <param name="lx"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte LerpByte(float lx, byte left, byte right)
        {
            return (byte)((1 - lx) * left + lx * right);
        }


        /// <summary>
        /// Basic Bilinear Lerp
        /// </summary>
        /// <param name="topleft"></param>
        /// <param name="topright"></param>
        /// <param name="botleft"></param>
        /// <param name="botright"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static float BiLerp(float topleft, float topright, float botleft, float botright, float x, float z)
        {
            float top = topleft + (topright - topleft) * x;
            float bot = botleft + (botright - botleft) * x;
            return top + (bot - top) * z;
        }


        /// <summary>
        /// Basic Bilinear Lerp
        /// </summary>
        /// <param name="topleft"></param>
        /// <param name="topright"></param>
        /// <param name="botleft"></param>
        /// <param name="botright"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static double BiLerp(double topleft, double topright, double botleft, double botright, double x, double z)
        {
            double top = topleft + (topright - topleft) * x;
            double bot = botleft + (botright - botleft) * x;
            return top + (bot - top) * z;
        }

        /// <summary>
        /// Same as <see cref="Lerp(float, float, float)"/>
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Mix(float v0, float v1, float t)
        {
            return v0 + (v1 - v0) * t;
        }



        /// <summary>
        /// Same as <see cref="Lerp(float, float, float)"/>
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Mix(int v0, int v1, float t)
        {
            return (int)(v0 + (v1 - v0) * t);
        }


        /// <summary>
        /// Basic Lerp
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float v0, float v1, float t)
        {
            return v0 + (v1 - v0) * t;
        }

        /// <summary>
        /// Basic Lerp
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Lerp(double v0, double v1, double t)
        {
            return v0 + (v1 - v0) * t;
        }

        /// <summary>
        /// Smooth Interpolation using inlined Smoothstep
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Serp(float v0, float v1, float t)
        {
            return v0 + (v1 - v0) * t * t * (3.0f - 2.0f * t);
        }



        /// <summary>
        /// Unlike the other implementation here, which uses the default "uniform"
        /// treatment of t, this computation is used to calculate the same values but
        /// introduces the ability to "parameterize" the t values used in the
        /// calculation. This is based on Figure 3 from
        /// http://www.cemyuksel.com/research/catmullrom_param/catmullrom.pdf
        /// </summary>
        /// <param name="t">the actual interpolation ratio from 0 to 1 representing the position between p1 and p2 to interpolate the value.</param>
        /// <param name="p">An array of double values of length 4, where interpolation occurs from p1 to p2.</param>
        /// <param name="time">An array of time measures of length 4, corresponding to each p value.</param>
        /// <returns></returns>
        public static double CPCatmullRomSplineLerp(double t, double[] p, double[] time)
        {
            double L01 = p[0] * (time[1] - t) / (time[1] - time[0]) + p[1] * (t - time[0]) / (time[1] - time[0]);
            double L12 = p[1] * (time[2] - t) / (time[2] - time[1]) + p[2] * (t - time[1]) / (time[2] - time[1]);
            double L23 = p[2] * (time[3] - t) / (time[3] - time[2]) + p[3] * (t - time[2]) / (time[3] - time[2]);
            double L012 = L01 * (time[2] - t) / (time[2] - time[0]) + L12 * (t - time[0]) / (time[2] - time[0]);
            double L123 = L12 * (time[3] - t) / (time[3] - time[1]) + L23 * (t - time[1]) / (time[3] - time[1]);
            double C12 = L012 * (time[2] - t) / (time[2] - time[1]) + L123 * (t - time[1]) / (time[2] - time[1]);
            return C12;
        }


        #endregion

        #region Stepping methods

        /// <summary>
        /// Better Lerp but more CPU intensive, see also https://en.wikipedia.org/wiki/Smoothstep
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SmoothStep(float x) { return x * x * (3.0f - 2.0f * x); }

        /// <summary>
        /// Better Lerp but more CPU intensive, see also https://en.wikipedia.org/wiki/Smoothstep
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SmoothStep(double x) { return x * x * (3.0f - 2.0f * x); }

        /// <summary>
        /// Better Lerp but more CPU intensive, see also https://en.wikipedia.org/wiki/Smoothstep
        /// </summary>
        /// <param name="edge0"></param>
        /// <param name="edge1"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Smootherstep(float edge0, float edge1, float x)
        {
            // Scale, and clamp x to 0..1 range
            x = Clamp((x - edge0) / (edge1 - edge0), 0.0f, 1.0f);
            // Evaluate polynomial
            return x * x * x * (x * (x * 6 - 15) + 10);
        }

        /// <summary>
        /// Better Lerp but more CPU intensive, see also https://en.wikipedia.org/wiki/Smoothstep
        /// </summary>
        /// <param name="edge0"></param>
        /// <param name="edge1"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Smootherstep(double edge0, double edge1, double x)
        {
            // Scale, and clamp x to 0..1 range
            x = Clamp((x - edge0) / (edge1 - edge0), 0.0, 1.0);
            // Evaluate polynomial
            return x * x * x * (x * (x * 6 - 15) + 10);
        }


        /// <summary>
        /// Better Lerp but more CPU intensive, see also https://en.wikipedia.org/wiki/Smoothstep. x must be in range of 0..1
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Smootherstep(double x)
        {
            // Evaluate polynomial
            return x * x * x * (x * (x * 6 - 15) + 10);
        }


        /// <summary>
        /// Returns a value between 0..1. Returns 0 if val is smaller than left or greater than right. For val == (left+right)/2 the return value is 1. Every other value is a linear interpolation based on the distance to the middle value. Ascii art representation:
        ///
        ///1  |      /\
        ///   |     /  \
        ///0.5|    /    \
        ///   |   /      \
        ///   |  /        \
        ///0  __/__________\______
        ///  left          right
        /// </summary>
        /// <param name="val"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static float TriangleStep(int val, int left, int right)
        {
            float mid = (left + right) / 2;
            float range = (right - left) / 2;

            return Math.Max(0, 1 - Math.Abs(val - mid) / range);
        }

        /// <summary>
        /// Returns a value between 0..1. Returns 0 if val is smaller than left or greater than right. For val == (left+right)/2 the return value is 1. Every other value is a linear interpolation based on the distance to the middle value. Ascii art representation:
        ///
        ///1  |      /\
        ///   |     /  \
        ///0.5|    /    \
        ///   |   /      \
        ///   |  /        \
        ///0  __/__________\______
        ///  left          right
        /// </summary>
        /// <param name="val"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static float TriangleStep(float val, float left, float right)
        {
            float mid = (left + right) / 2;
            float range = (right - left) / 2;

            return Math.Max(0, 1 - Math.Abs(val - mid) / range);
        }


        /// <summary>
        /// Same as TriangleStep but skipping the step to calc mid and range.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="mid"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float TriangleStepFast(int val, int mid, int range)
        {
            return Math.Max(0, 1 - Math.Abs(val - mid) / range);
        }





        #endregion

        #region MinMax

        public static double Max(double a, double b)
        {
            return Math.Max(a, b);
        }

        public static double Max(params double[] values)
        {
            double max = values[0];
            for (int i = 0; i < values.Length; i++)
            {
                max = Math.Max(max, values[i]);
            }
            return max;
        }

        public static float Max(float a, float b)
        {
            return Math.Max(a, b);
        }

        public static float Max(params float[] values)
        {
            float max = values[0];
            for (int i = 0; i < values.Length; i++)
            {
                max = Math.Max(max, values[i]);
            }
            return max;
        }

        public static int Max(int a, int b)
        {
            return Math.Max(a, b);
        }

        public static int Max(params int[] values)
        {
            int max = values[0];
            for (int i = 0; i < values.Length; i++)
            {
                max = Math.Max(max, values[i]);
            }
            return max;
        }

        public static int Min(int a, int b)
        {
            return Math.Min(a, b);
        }

        public static int Min(params int[] values)
        {
            int min = values[0];
            for (int i = 0; i < values.Length; i++)
            {
                min = Math.Min(min, values[i]);
            }
            return min;
        }

        public static float Min(float a, float b)
        {
            return Math.Min(a, b);
        }

        public static float Min(params float[] values)
        {
            float min = values[0];
            for (int i = 0; i < values.Length; i++)
            {
                min = Math.Min(min, values[i]);
            }
            return min;
        }

        public static float SmoothMin(float a, float b, float smoothingFactor)
        {
            float falloff = Math.Max(smoothingFactor - Math.Abs(a - b), 0.0f) / smoothingFactor;
            return Math.Min(a, b) - falloff * falloff * smoothingFactor * 0.25f;
        }

        public static float SmoothMax(float a, float b, float smoothingFactor)
        {
            float falloff = Math.Max(smoothingFactor - Math.Abs(a - b), 0.0f) / smoothingFactor;
            return Math.Max(a, b) + falloff * falloff * smoothingFactor * 0.25f;
        }

        public static double SmoothMin(double a, double b, double smoothingFactor)
        {
            double falloff = Math.Max(smoothingFactor - Math.Abs(a - b), 0.0) / smoothingFactor;
            return Math.Min(a, b) - falloff * falloff * smoothingFactor * 0.25;
        }

        public static double SmoothMax(double a, double b, double smoothingFactor)
        {
            double falloff = Math.Max(smoothingFactor - Math.Abs(a - b), 0.0) / smoothingFactor;
            return Math.Max(a, b) + falloff * falloff * smoothingFactor * 0.25;
        }

        #endregion

        #region Hashing

        public static uint Crc32(string input)
        {
            return Crc32(Encoding.UTF8.GetBytes(input));
        }

        public static uint Crc32(byte[] input)
        {
            return Crc32Algorithm.Compute(input);
        }

        /// <summary>
        /// Pretty much taken directly from the string.GetHashCode() implementation, but on these methods the documentation states: "You should never persist or use a hash code outside the application domain in which it was created, [...]."
        /// Hence, this is one basic 32bit bit implementation that can be used in a platform independent, persistent way.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int DotNetStringHash(string text)
        {
            unsafe
            {
                fixed (char* src = text)
                {
                    int hash1 = (5381<<16) + 5381;
                    int hash2 = hash1;

                    // 32 bit machines.
                    int* pint = (int *)src;
                    int len = text.Length;
                    while (len > 2)
                    {
                        hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
                        hash2 = ((hash2 << 5) + hash2 + (hash2 >> 27)) ^ pint[1];
                        pint += 2;
                        len  -= 4;
                    }

                    if (len > 0)
                    {
                        hash1 = ((hash1 << 5) + hash1 + (hash1 >> 27)) ^ pint[0];
                    }

                    return hash1 + (hash2 * 1566083941);
                }
            }
        }

        /// <summary>
        /// See also https://msdn.microsoft.com/en-us/library/system.security.cryptography.md5%28v=vs.110%29.aspx
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Md5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
        }

        /// <summary>
        /// A single iteration of Bob Jenkins' One-At-A-Time hashing algorithm - optimised version for the vector hashes
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int oaatHashMany(int x)
        {
            for (int i = OaatIterations; i > 0; i--)
            {
                x += (x << 10);
                x ^= (x >> 6);
                x += (x << 3);
                x ^= (x >> 11);
                x += (x << 15);
            }
            return x;
        }

        /// <summary>
        /// A single iteration of Bob Jenkins' One-At-A-Time hashing algorithm.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int oaatHashMany(int x, int count)
        {
            for (int i = count; i > 0; i--)
            {
                x += (x << 10);
                x ^= (x >> 6);
                x += (x << 3);
                x ^= (x >> 11);
                x += (x << 15);
            }
            return x;
        }

        /// <summary>
        /// Bob Jenkins' One-At-A-Time hashing algorithm
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint oaatHashUMany(uint x)
        {
            for (int i = OaatIterations; i > 0; i--)
            {
                x += (x << 10);
                x ^= (x >> 6);
                x += (x << 3);
                x ^= (x >> 11);
                x += (x << 15);
            }
            return x;
        }

        static readonly int OaatIterations = 3;

        /// <summary>
        /// Bob Jenkins' One-At-A-Time hashing algorithm. Fast, but not very random.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int oaatHash(Vec2i v) { return oaatHashMany(v.X ^ oaatHashMany(v.Y)); }

        /// <summary>
        /// Bob Jenkins' One-At-A-Time hashing algorithm. Fast, but not very random.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int oaatHash(int x, int y) { return oaatHashMany(x ^ oaatHashMany(y)); }

        /// <summary>
        /// Bob Jenkins' One-At-A-Time hashing algorithm. Fast, but not very random.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int oaatHash(Vec3i v) { return oaatHashMany(v.X) ^ oaatHashMany(v.Y) ^ oaatHashMany(v.Z); }

        /// <summary>
        /// Bob Jenkins' One-At-A-Time hashing algorithm. Fast, but not very random.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int oaatHash(int x, int y, int z) { return oaatHashMany(x) ^ oaatHashMany(y) ^ oaatHashMany(z); }

        /// <summary>
        /// Bob Jenkins' One-At-A-Time hashing algorithm. Fast, but not very random.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint oaatHashU(int x, int y, int z) { return oaatHashUMany((uint)x) ^ oaatHashUMany((uint)y) ^ oaatHashUMany((uint)z); }

        /// <summary>
        /// Bob Jenkins' One-At-A-Time hashing algorithm. Fast, but not very random.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int oaatHash(Vec4i v) { return oaatHashMany(v.X) ^ oaatHashMany(v.Y) ^ oaatHashMany(v.Z) ^ oaatHashMany(v.W); }

        /// <summary>
        /// A really bad, but very fast hashing method.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static float PrettyBadHash(int x, int y)
        {
            return (float)Mod((x * 12.9898 + y * 78.233) * 43758.5453, 1);
        }

        /// <summary>
        /// A not so fast, but higher quality than oaatHash(). See also https://en.wikipedia.org/wiki/MurmurHash. Includes a modulo operation.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="mod"></param>
        /// <returns></returns>
        public static int MurmurHash3Mod(int x, int y, int z, int mod)
        {
            return Mod(MurmurHash3(x, y, z), mod);
        }



        /// <summary>
        /// A not so fast, but higher quality than oaatHash(). See also https://en.wikipedia.org/wiki/MurmurHash
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static int MurmurHash3(int x, int y, int z)
        {
            const uint c1 = 0xcc9e2d51;
            const uint c2 = 0x1b873593;

            uint h1 = murmurseed;

            /* bitmagic hash */
            h1 ^= rotl32((uint)x * c1, 15) * c2;
            h1 = rotl32(h1, 13) * 5 + 0xe6546b64;

            /* bitmagic hash */
            h1 ^= rotl32((uint)y * c1, 15) * c2;
            h1 = rotl32(h1, 13) * 5 + 0xe6546b64;

            /* bitmagic hash */
            h1 ^= rotl32((uint)z * c1, 15) * c2;
            h1 = rotl32(h1, 13) * 5 + 0xe6546b64;

            // finalization, magic chants to wrap it all up
            unchecked //ignore overflow
            {
                return (int)fmix(h1 ^ 3);
            }
        }

        private static uint rotl32(uint x, int r)
        {
            return x << r | x >> 32 - r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint fmix(uint h)
        {
            h = (h ^ h >> 16) * 0x85ebca6b;
            h = (h ^ h >> 13) * 0xc2b2ae35;
            return h ^ h >> 16;
        }




        /// <summary>
        /// Quasirandom sequence by Martin Roberts (http://extremelearning.com.au/unreasonable-effectiveness-of-quasirandom-sequences/)
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double R2Sequence1D(int n)
        {
            double g = 1.6180339887498948482;
            double a1 = 1.0 / g;
            return (0.5 + a1 * n) % 1;
        }

        /// <summary>
        /// Quasirandom sequence by Martin Roberts (http://extremelearning.com.au/unreasonable-effectiveness-of-quasirandom-sequences/)
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Vec2d R2Sequence2D(int n)
        {
            double g = 1.32471795724474602596;
            double a1 = 1.0 / g;
            double a2 = 1.0 / (g * g);
            return new Vec2d((0.5 + a1 * n) % 1, (0.5 + a2 * n) % 1);
        }

        /// <summary>
        /// Quasirandom sequence by Martin Roberts (http://extremelearning.com.au/unreasonable-effectiveness-of-quasirandom-sequences/)
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static Vec3d R2Sequence3D(int n)
        {
            double g = 1.22074408460575947536;
            double a1 = 1.0 / g;
            double a2 = 1.0 / (g * g);
            double a3 = 1.0 / (g * g * g);
            return new Vec3d((0.5 + a1 * n) % 1, (0.5 + a2 * n) % 1, (0.5 + a3 * n) % 1);
        }





        #endregion

        /// <summary>
        /// Assigns the value of x1 to x2 and vice versa
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        public static void FlipVal(ref int x1, ref int x2)
        {
            int tmp = x1;
            x2 = x1;
            x1 = tmp;
        }

        /// <summary>
        /// Assigns the value of x1 to x2 and vice versa
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        public static void FlipVal(ref double x1, ref double x2)
        {
            double tmp = x1;
            x2 = x1;
            x1 = tmp;
        }

        /// <summary>
        /// Performs a Fisher-Yates shuffle in linear time or O(n)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rand"></param>
        /// <param name="array"></param>
        public static void Shuffle<T>(Random rand, T[] array)
        {
            int n = array.Length;        // The number of items left to shuffle (loop invariant).
            while (n > 1)
            {
                int k = rand.Next(n);  // 0 <= k < n.
                n--;                   // n is now the last pertinent index;
                T temp = array[n];     // swap array[n] with array[k] (does nothing if k == n).
                array[n] = array[k];
                array[k] = temp;
            }
        }


        /// <summary>
        /// Performs a Fisher-Yates shuffle in linear time or O(n)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rand"></param>
        /// <param name="array"></param>
        public static void Shuffle<T>(Random rand, List<T> array)
        {
            int n = array.Count;        // The number of items left to shuffle (loop invariant).
            while (n > 1)
            {
                int k = rand.Next(n);  // 0 <= k < n.
                n--;                   // n is now the last pertinent index;
                T temp = array[n];     // swap array[n] with array[k] (does nothing if k == n).
                array[n] = array[k];
                array[k] = temp;
            }
        }



        /// <summary>
        /// Performs a Fisher-Yates shuffle in linear time or O(n)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rand"></param>
        /// <param name="array"></param>
        public static void Shuffle<T>(LCGRandom rand, List<T> array)
        {
            int n = array.Count;        // The number of items left to shuffle (loop invariant).
            while (n > 1)
            {
                int k = rand.NextInt(n);  // 0 <= k < n.
                n--;                   // n is now the last pertinent index;
                T temp = array[n];     // swap array[n] with array[k] (does nothing if k == n).
                array[n] = array[k];
                array[k] = temp;
            }
        }



        /// <summary>
        /// Plot a 3d line, see also http://members.chello.at/~easyfilter/bresenham.html
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="z0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="z1"></param>
        /// <param name="onPlot"></param>
        public static void BresenHamPlotLine3d(int x0, int y0, int z0, int x1, int y1, int z1, PlotDelegate3D onPlot)
        {
            int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int dz = Math.Abs(z1 - z0), sz = z0 < z1 ? 1 : -1;
            int dm = GameMath.Max(dx, dy, dz), i = dm; /* maximum difference */
            x1 = y1 = z1 = dm / 2; /* error offset */

            for (; ; )
            {  /* loop */
                onPlot(x0, y0, z0);
                if (i-- == 0) break;
                x1 -= dx; if (x1 < 0) { x1 += dm; x0 += sx; }
                y1 -= dy; if (y1 < 0) { y1 += dm; y0 += sy; }
                z1 -= dz; if (z1 < 0) { z1 += dm; z0 += sz; }
            }
        }


        /// <summary>
        /// Plot a 3d line, see also http://members.chello.at/~easyfilter/bresenham.html
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="z0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="z1"></param>
        /// <param name="onPlot"></param>
        public static void BresenHamPlotLine3d(int x0, int y0, int z0, int x1, int y1, int z1, PlotDelegate3DBlockPos onPlot)
        {
            int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int dz = Math.Abs(z1 - z0), sz = z0 < z1 ? 1 : -1;
            int dm = GameMath.Max(dx, dy, dz), i = dm; /* maximum difference */
            x1 = y1 = z1 = dm / 2; /* error offset */

            BlockPos pos = new BlockPos();

            for (; ; )
            {  /* loop */
                pos.Set(x0, y0, z0);
                onPlot(pos);
                if (i-- == 0) break;
                x1 -= dx; if (x1 < 0) { x1 += dm; x0 += sx; }
                y1 -= dy; if (y1 < 0) { y1 += dm; y0 += sy; }
                z1 -= dz; if (z1 < 0) { z1 += dm; z0 += sz; }
            }
        }

        /// <summary>
        /// Plot a 2d line, see also http://members.chello.at/~easyfilter/bresenham.html
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="onPlot"></param>
        public static void BresenHamPlotLine2d(int x0, int y0, int x1, int y1, PlotDelegate2D onPlot)
        {
            int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = -Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = dx + dy, e2; /* error value e_xy */

            for (; ; )
            {  /* loop */
                onPlot(x0, y0);
                if (x0 == x1 && y0 == y1) break;
                e2 = 2 * err;
                if (e2 >= dy) { err += dy; x0 += sx; } /* e_xy+e_x > 0 */
                if (e2 <= dx) { err += dx; y0 += sy; } /* e_xy+e_y < 0 */
            }
        }

        public static Vec3f ToEulerAngles(Vec4f q)
        {
            Vec3d vec = ToEulerAngles(new Vec4d(q.X, q.Y, q.Z, q.W));
            return new Vec3f((float)vec.X, (float)vec.Y, (float)vec.Z);
        }

        public static Vec3d ToEulerAngles(Vec4d q)
        {
            Vec3d angles = new Vec3d();

            // roll (x-axis rotation)
            double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
            double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
            angles.X = Math.Atan2(sinr_cosp, cosr_cosp);
            // pitch (y-axis rotation)
            double sinp = 2 * (q.W * q.Y - q.Z * q.X);

            angles.Y = Math.Asin(sinp);

            // yaw (z-axis rotation)
            double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
            double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
            angles.Z = Math.Atan2(siny_cosp, cosy_cosp);

            return angles;
        }

        public static int IntFromBools(int[] intBools)
        {
            int result = 0;
            int i = intBools.Length;
            while (i-- != 0)
            {
                if (intBools[i] != 0) result += 1 << i;
            }
            return result;
        }

        public static int IntFromBools(bool[] bools)
        {
            int result = 0;
            int i = bools.Length;
            while (i-- != 0)
            {
                if (bools[i]) result += 1 << i;
            }
            return result;
        }

        public static void BoolsFromInt(bool[] bools, int v)
        {
            int i = bools.Length;
            while (i-- != 0)
            {
                bools[i] = (v & (1 << i)) != 0;
            }
        }

        /// <summary>
        /// Map a value from one range to another
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fromMin"></param>
        /// <param name="fromMax"></param>
        /// <param name="toMin"></param>
        /// <param name="toMax"></param>
        /// <returns></returns>
        public static T Map<T>(T value, T fromMin, T fromMax, T toMin, T toMax) where T : INumber<T>
        {
            return (value - fromMin) * (toMax - toMin) / (fromMax - fromMin) + toMin;
        }
    }


    public delegate void PlotDelegate3D(int x, int y, int z);
    public delegate void PlotDelegate3DBlockPos(BlockPos pos);
    public delegate void PlotDelegate2D(int x, int z);

    [StructLayout(LayoutKind.Explicit)]
    internal struct FloatIntUnion
    {
        [FieldOffset(0)]
        public float f;

        [FieldOffset(0)]
        public int tmp;
    }


    [StructLayout(LayoutKind.Explicit)]
    internal struct DoubleLongUnion
    {
        [FieldOffset(0)]
        public double f;

        [FieldOffset(0)]
        public long tmp;
    }

}
