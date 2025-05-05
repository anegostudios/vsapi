//glMatrix license:
//Copyright (c) 2013, Brandon Jones, Colin MacKenzie IV. All rights reserved.

//Redistribution and use in source and binary forms, with or without modification,
//are permitted provided that the following conditions are met:

//  * Redistributions of source code must retain the above copyright notice, this
//    list of conditions and the following disclaimer.
//  * Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.

//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
//ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
//ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Runtime.CompilerServices;

namespace Vintagestory.API.MathTools
{
    public partial class Quaterniond
    {
        /// <summary>
        /// Sets a quaternion to represent the shortest rotation from one
        /// vector to another.
        ///
        /// Both vectors are assumed to be unit length.
        /// </summary>
        /// <param name="output">the receiving quaternion.</param>
        /// <param name="a">the initial vector</param>
        /// <param name="b">the destination vector</param>
        public static void RotationTo(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            Span<double> tmpvec3 = stackalloc double[3];
            Span<double> xUnitVec3 = stackalloc double[3] { 1.0, 0.0, 0.0 };
            Span<double> yUnitVec3 = stackalloc double[3] { 0.0, 1.0, 0.0 };

            double dot = Vec3Utilsd.Dot(a, b);

            double nines = 999999.0 / 1000000.0; // 0.999999

            double epsilon = 1.0 / 1000000.0; // 0.000001

            if (dot < -nines)
            {
                Vec3Utilsd.Cross(tmpvec3, xUnitVec3, a);
                if (Vec3Utilsd.Length(tmpvec3) < epsilon)
                    Vec3Utilsd.Cross(tmpvec3, yUnitVec3, a);
                Vec3Utilsd.Normalize(tmpvec3, tmpvec3);
                SetAxisAngle(output, tmpvec3, GameMath.PI);
                return;
            }
            else if (dot > nines)
            {
                output[0] = 0.0;
                output[1] = 0.0;
                output[2] = 0.0;
                output[3] = 1.0;
                return;
            }
            else
            {
                Vec3Utilsd.Cross(tmpvec3, a, b);
                output[0] = tmpvec3[0];
                output[1] = tmpvec3[1];
                output[2] = tmpvec3[2];
                output[3] = 1.0 + dot;
                Normalize(output, output);
            }
        }

        /// <summary>
        /// Sets the specified quaternion with values corresponding to the given
        /// axes. Each axis is a vec3 and is expected to be unit length and
        /// perpendicular to all other specified axes.
        /// </summary>
        /// <param name="view">the vector representing the viewing direction</param>
        /// <param name="right">the vector representing the local "right" direction</param>
        /// <param name="up">the vector representing the local "up" direction</param>
        public static void SetAxes(Span<double> output, ReadOnlySpan<double> view, ReadOnlySpan<double> right, ReadOnlySpan<double> up)
        {
            Span<double> matr = stackalloc double[9];

            matr[0] = right[0];
            matr[3] = right[1];
            matr[6] = right[2];

            matr[1] = up[0];
            matr[4] = up[1];
            matr[7] = up[2];

            matr[2] = view[0];
            matr[5] = view[1];
            matr[8] = view[2];
            FromMat3(output, matr);
            Normalize(output, output);
        }

        /// <summary>
        /// Set the components of a quat to the given values
        /// </summary>
        /// <param name="output">the receiving quaternion</param>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        /// <param name="z">Z component</param>
        /// <param name="w">W component</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set(Span<double> output, double x, double y, double z, double w)
        {
            QVec4d.Set(output, x, y, z, w);
        }

        /// <summary>
        /// Set a quat to the identity quaternion
        /// </summary>
        /// <param name="output">the receiving quaternion</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Identity(Span<double> output)
        {
            output[0] = 0.0;
            output[1] = 0.0;
            output[2] = 0.0;
            output[3] = 1.0;
        }

        /// <summary>
        /// Sets a quat from the given angle and rotation axis,
        /// then returns it.
        /// </summary>
        /// <param name="output">the receiving quaternion</param>
        /// <param name="axis">the axis around which to rotate</param>
        /// <param name="rad">the angle in radians</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetAxisAngle(Span<double> output, ReadOnlySpan<double> axis, double rad)
        {
            rad /= 2.0;
            double s = GameMath.Sin(rad);
            output[0] = s * axis[0];
            output[1] = s * axis[1];
            output[2] = s * axis[2];
            output[3] = GameMath.Cos(rad);
        }

        /// <summary>
        /// Adds two quat's
        /// </summary>
        /// <param name="output">the receiving quaternion</param>
        /// <param name="a">the first operand</param>
        /// <param name="b">the second operand</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            QVec4d.Add(output, a, b);
        }

        /// <summary>
        /// Multiplies two quat's
        /// </summary>
        /// <param name="output">the receiving quaternion</param>
        /// <param name="a">the first operand</param>
        /// <param name="b">the second operand</param>
        public static void Multiply(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            double ax = a[0]; double ay = a[1]; double az = a[2]; double aw = a[3];
            double bx = b[0]; double by = b[1]; double bz = b[2]; double bw = b[3];

            output[0] = ax * bw + aw * bx + ay * bz - az * by;
            output[1] = ay * bw + aw * by + az * bx - ax * bz;
            output[2] = az * bw + aw * bz + ax * by - ay * bx;
            output[3] = aw * bw - ax * bx - ay * by - az * bz;
        }

        /// <summary>
        /// Alias for <see cref="Multiply(Span{double}, ReadOnlySpan{double}, ReadOnlySpan{double})"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Mul(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            Multiply(output, a, b);
        }

        /// <summary>
        /// Scales a quat by a scalar number
        /// </summary>
        /// <param name="output">the receiving vector</param>
        /// <param name="a">the vector to scale</param>
        /// <param name="b">amount to scale the vector by</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Scale(Span<double> output, ReadOnlySpan<double> a, double b)
        {
            QVec4d.Scale(output, a, b);
        }

        /// <summary>
        /// Rotates a quaternion by the given angle aboutput the X axis
        /// </summary>
        /// <param name="output">quat receiving operation result</param>
        /// <param name="a">quat to rotate</param>
        /// <param name="rad">angle (in radians) to rotate</param>
        public static void RotateX(Span<double> output, ReadOnlySpan<double> a, double rad)
        {
            rad /= 2.0;

            double ax = a[0]; double ay = a[1]; double az = a[2]; double aw = a[3];
            double bx = GameMath.Sin(rad); double bw = GameMath.Cos(rad);

            output[0] = ax * bw + aw * bx;
            output[1] = ay * bw + az * bx;
            output[2] = az * bw - ay * bx;
            output[3] = aw * bw - ax * bx;
        }

        /// <summary>
        /// Rotates a quaternion by the given angle aboutput the Y axis
        /// </summary>
        /// <param name="output">quat receiving operation result</param>
        /// <param name="a">quat to rotate</param>
        /// <param name="rad">angle (in radians) to rotate</param>
        public static void RotateY(Span<double> output, ReadOnlySpan<double> a, double rad)
        {
            rad /= 2.0;

            double ax = a[0]; double ay = a[1]; double az = a[2]; double aw = a[3];
            double by = GameMath.Sin(rad); double bw = GameMath.Cos(rad);

            output[0] = ax * bw - az * by;
            output[1] = ay * bw + aw * by;
            output[2] = az * bw + ax * by;
            output[3] = aw * bw - ay * by;
        }

        /// <summary>
        /// Rotates a quaternion by the given angle aboutput the Z axis
        /// </summary>
        /// <param name="output">quat receiving operation result</param>
        /// <param name="a">quat to rotate</param>
        /// <param name="rad">angle (in radians) to rotate</param>
        public static void RotateZ(Span<double> output, ReadOnlySpan<double> a, double rad)
        {
            rad /= 2.0;

            double ax = a[0]; double ay = a[1]; double az = a[2]; double aw = a[3];
            double bz = GameMath.Sin(rad); double bw = GameMath.Cos(rad);

            output[0] = ax * bw + ay * bz;
            output[1] = ay * bw - ax * bz;
            output[2] = az * bw + aw * bz;
            output[3] = aw * bw - az * bz;
        }

        /// <summary>
        /// Calculates the W component of a quat from the X, Y, and Z components.
        /// Assumes that quaternion is 1 unit in length.
        /// Any existing W component will be ignored.
        /// </summary>
        /// <param name="output">the receiving quaternion</param>
        /// <param name="a">quat to calculate W component of</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CalculateW(Span<double> output, ReadOnlySpan<double> a)
        {
            double x = a[0]; double y = a[1]; double z = a[2];

            output[0] = x;
            output[1] = y;
            output[2] = z;
            output[3] = -GameMath.Sqrt(Math.Abs(1.0 - x * x - y * y - z * z));
        }

        /// <summary>
        /// Calculates the dot product of two quat's
        /// </summary>
        /// <param name="a">the first operand</param>
        /// <param name="b">the second operand</param>
        /// <returns>dot product of a and b</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Dot(ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            return QVec4d.Dot(a, b);
        }

        public static void ToEulerAngles(Span<double> output, ReadOnlySpan<double> quat)
        {
            // roll (x-axis rotation)
            double sinr_cosp = 2.0 * (quat[3] * quat[0] + quat[1] * quat[2]);
            double cosr_cosp = 1.0 - 2.0 * (quat[0] * quat[0] + quat[1] * quat[1]);
            output[2] = Math.Atan2(sinr_cosp, cosr_cosp);

            // pitch (y-axis rotation)
            double sinp = 2.0 * (quat[3] * quat[1] - quat[2] * quat[0]);
            if (Math.Abs(sinp) >= 1.0)
                output[1] = Math.PI / 2 * Math.Sign(sinp); // use 90 degrees if out of range
            else
                output[1] = Math.Asin(sinp);

            // yaw (z-axis rotation)
            double siny_cosp = 2.0 * (quat[3] * quat[2] + quat[0] * quat[1]);
            double cosy_cosp = 1.0 - 2.0 * (quat[1] * quat[1] + quat[2] * quat[2]);
            output[0] = Math.Atan2(siny_cosp, cosy_cosp);
        }

        /// <summary>
        /// Performs a linear interpolation between two quat's
        /// </summary>
        /// <param name="output">the receiving quaternion</param>
        /// <param name="a">the first operand</param>
        /// <param name="b">the second operand</param>
        /// <param name="t">interpolation amount between the two inputs</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Lerp(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b, double t)
        {
            QVec4d.Lerp(output, a, b, t);
        }

        /// <summary>
        /// Performs a spherical linear interpolation between two quat
        /// </summary>
        /// <param name="output">the receiving quaternion</param>
        /// <param name="a">the first operand</param>
        /// <param name="b">the second operand</param>
        /// <param name="t">interpolation amount between the two inputs</param>
        public static void Slerp(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b, double t)
        {
            //    // benchmarks:
            //    //    http://jsperf.com/quaternion-slerp-implementations

            double ax = a[0]; double ay = a[1]; double az = a[2]; double aw = a[3];
            double bx = b[0]; double by = b[1]; double bz = b[2]; double bw = b[3];

            double omega; double cosom; double sinom; double scale0; double scale1;

            // calc cosine
            cosom = ax * bx + ay * by + az * bz + aw * bw;
            // adjust signs (if necessary)
            if (cosom < 0.0)
            {
                cosom = -cosom;
                bx = -bx;
                by = -by;
                bz = -bz;
                bw = -bw;
            }
            double one = 1.0;
            double epsilon = one / 1000000;
            // calculate coefficients
            if ((one - cosom) > epsilon)
            {
                // standard case (slerp)
                omega = GameMath.Acos(cosom);
                sinom = GameMath.Sin(omega);
                scale0 = GameMath.Sin((one - t) * omega) / sinom;
                scale1 = GameMath.Sin(t * omega) / sinom;
            }
            else
            {
                // "from" and "to" quaternions are very close
                //  ... so we can do a linear interpolation
                scale0 = one - t;
                scale1 = t;
            }
            // calculate final values
            output[0] = scale0 * ax + scale1 * bx;
            output[1] = scale0 * ay + scale1 * by;
            output[2] = scale0 * az + scale1 * bz;
            output[3] = scale0 * aw + scale1 * bw;
        }

        /// <summary>
        /// Calculates the inverse of a quat
        /// </summary>
        /// <param name="output">the receiving quaternion</param>
        /// <param name="a">quat to calculate inverse of</param>
        public static void Invert(Span<double> output, ReadOnlySpan<double> a)
        {
            double a0 = a[0]; double a1 = a[1]; double a2 = a[2]; double a3 = a[3];
            double dot = a0 * a0 + a1 * a1 + a2 * a2 + a3 * a3;
            double one = 1.0;
            double invDot = (dot != 0.0) ? one / dot : 0.0;

            // TODO: Would be faster to return [0,0,0,0] immediately if dot == 0

            output[0] = -a0 * invDot;
            output[1] = -a1 * invDot;
            output[2] = -a2 * invDot;
            output[3] = a3 * invDot;
        }

        /// <summary>
        /// Calculates the conjugate of a quat
        /// If the quaternion is normalized, this function is faster than quat.inverse and produces the same result.
        /// </summary>
        /// <param name="output">the receiving quaternion</param>
        /// <param name="a">quat to calculate conjugate of</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Conjugate(Span<double> output, ReadOnlySpan<double> a)
        {
            output[0] = -a[0];
            output[1] = -a[1];
            output[2] = -a[2];
            output[3] = a[3];
        }

        /// <summary>
        /// Calculates the length of a quat
        /// </summary>
        /// <param name="a">vector to calculate length of</param>
        /// <returns>length of a</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Length(ReadOnlySpan<double> a)
        {
            return QVec4d.Length(a);
        }

        /// <summary>
        /// Alias for <see cref="Length(ReadOnlySpan{double})"/>
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Len(ReadOnlySpan<double> a)
        {
            return Length(a);
        }

        /// <summary>
        /// Calculates the squared length of a quat
        /// </summary>
        /// <param name="a">vector to calculate squared length of</param>
        /// <returns>squared length of a</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SquaredLength(ReadOnlySpan<double> a)
        {
            return QVec4d.SquaredLength(a);
        }

        /// <summary>
        /// Alias for <see cref="SquaredLength(ReadOnlySpan{double})"/>
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SqrLen(ReadOnlySpan<double> a)
        {
            return SquaredLength(a);
        }

        /// <summary>
        /// Normalize a quat
        /// </summary>
        /// <param name="output">the receiving quaternion</param>
        /// <param name="a">quaternion to normalize</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Normalize(Span<double> output, ReadOnlySpan<double> a)
        {
            QVec4d.Normalize(output, a);
        }

        /// <summary>
        /// Creates a quaternion from the given 3x3 rotation matrix.
        ///
        /// NOTE: The resultant quaternion is not normalized, so you should be sure
        /// to renormalize the quaternion yourself where necessary.
        /// </summary>
        /// <param name="output">the receiving quaternion</param>
        /// <param name="m">rotation matrix</param>
        public static void FromMat3(Span<double> output, ReadOnlySpan<double> m)
        {
            // Algorithm in Ken Shoemake's article in 1987 SIGGRAPH course notes
            // article "Quaternion Calculus and Fast Animation".
            double fTrace = m[0] + m[4] + m[8];
            double fRoot;

            double zero = 0;
            double one = 1;
            double half = one / 2;
            if (fTrace > zero)
            {
                // |w| > 1/2, may as well choose w > 1/2
                fRoot = GameMath.Sqrt(fTrace + one);  // 2w
                output[3] = half * fRoot;
                fRoot = half / fRoot;  // 1/(4w)
                output[0] = (m[7] - m[5]) * fRoot;
                output[1] = (m[2] - m[6]) * fRoot;
                output[2] = (m[3] - m[1]) * fRoot;
            }
            else
            {
                // |w| <= 1/2
                int i = 0;
                if (m[4] > m[0])
                    i = 1;
                if (m[8] > m[i * 3 + i])
                    i = 2;
                int j = (i + 1) % 3;
                int k = (i + 2) % 3;

                fRoot = GameMath.Sqrt(m[i * 3 + i] - m[j * 3 + j] - m[k * 3 + k] + one);
                output[i] = half * fRoot;
                fRoot = half / fRoot;
                output[3] = (m[k * 3 + j] - m[j * 3 + k]) * fRoot;
                output[j] = (m[j * 3 + i] + m[i * 3 + j]) * fRoot;
                output[k] = (m[k * 3 + i] + m[i * 3 + k]) * fRoot;
            }
        }
    }

    partial class QVec4d
    {
        /// <summary>
        /// Set the components of a QVec4d to the given values
        /// </summary>
        /// <param name="output">the receiving vector</param>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        /// <param name="z">Z component</param>
        /// <param name="w">W component</param>
        public static void Set(Span<double> output, double x, double y, double z, double w)
        {
            output[0] = x;
            output[1] = y;
            output[2] = z;
            output[3] = w;
        }

        /// <summary>
        /// Adds two QVec4d's
        /// </summary>
        /// <param name="output">the receiving vector</param>
        /// <param name="a">the first operand</param>
        /// <param name="b">the second operand</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            output[0] = a[0] + b[0];
            output[1] = a[1] + b[1];
            output[2] = a[2] + b[2];
            output[3] = a[3] + b[3];
        }

        /// <summary>
        /// Subtracts vector b from vector a
        /// </summary>
        /// <param name="output">the receiving vector</param>
        /// <param name="a">the first operand</param>
        /// <param name="b">the second operand</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Subtract(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            output[0] = a[0] - b[0];
            output[1] = a[1] - b[1];
            output[2] = a[2] - b[2];
            output[3] = a[3] - b[3];
        }

        /// <summary>
        /// Alias for <see cref="Subtract(Span{double}, ReadOnlySpan{double}, ReadOnlySpan{double})"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sub(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            Subtract(output, a, b);
        }

        /// <summary>
        /// Multiplies two QVec4d's
        /// </summary>
        /// <param name="output">the receiving vector</param>
        /// <param name="a">the first operand</param>
        /// <param name="b">the second operand</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            output[0] = a[0] * b[0];
            output[1] = a[1] * b[1];
            output[2] = a[2] * b[2];
            output[3] = a[3] * b[3];
        }

        /// <summary>
        /// Alias for <see cref="Multiply(Span{double}, ReadOnlySpan{double}, ReadOnlySpan{double})"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Mul(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            Multiply(output, a, b);
        }

        /// <summary>
        /// Divides two QVec4d's
        /// </summary>
        /// <param name="output">the receiving vector</param>
        /// <param name="a">the first operand</param>
        /// <param name="b">the second operand</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Divide(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            output[0] = a[0] / b[0];
            output[1] = a[1] / b[1];
            output[2] = a[2] / b[2];
            output[3] = a[3] / b[3];
        }

        /// <summary>
        /// Alias for <see cref="Divide(Span{double}, ReadOnlySpan{double}, ReadOnlySpan{double})"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Div(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            Divide(output, a, b);
        }

        /// <summary>
        /// Returns the minimum of two QVec4d's
        /// </summary>
        /// <param name="output">the receiving vector</param>
        /// <param name="a">the first operand</param>
        /// <param name="b">the second operand</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Min(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            output[0] = Math.Min(a[0], b[0]);
            output[1] = Math.Min(a[1], b[1]);
            output[2] = Math.Min(a[2], b[2]);
            output[3] = Math.Min(a[3], b[3]);
        }

        /// <summary>
        /// Returns the maximum of two QVec4d's
        /// </summary>
        /// <param name="output">the receiving vector</param>
        /// <param name="a">the first operand</param>
        /// <param name="b">the second operand</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Max(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            output[0] = Math.Max(a[0], b[0]);
            output[1] = Math.Max(a[1], b[1]);
            output[2] = Math.Max(a[2], b[2]);
            output[3] = Math.Max(a[3], b[3]);
        }

        /// <summary>
        /// Scales a QVec4d by a scalar number
        /// </summary>
        /// <param name="output">the receiving vector</param>
        /// <param name="a">the vector to scale</param>
        /// <param name="b">amount to scale the vector by</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Scale(Span<double> output, ReadOnlySpan<double> a, double b)
        {
            output[0] = a[0] * b;
            output[1] = a[1] * b;
            output[2] = a[2] * b;
            output[3] = a[3] * b;
        }

        /// <summary>
        /// Adds two QVec4d's after scaling the second operand by a scalar value
        /// </summary>
        /// <param name="output">the receiving vector</param>
        /// <param name="a">the first operand</param>
        /// <param name="b">the second operand</param>
        /// <param name="scale">the amount to scale b by before adding</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ScaleAndAdd(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b, double scale)
        {
            output[0] = a[0] + (b[0] * scale);
            output[1] = a[1] + (b[1] * scale);
            output[2] = a[2] + (b[2] * scale);
            output[3] = a[3] + (b[3] * scale);
        }

        /// <summary>
        /// Calculates the euclidian distance between two QVec4d's
        /// </summary>
        /// <param name="a">the first operand</param>
        /// <param name="b">the second operand</param>
        /// <returns>distance between a and b</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Distance(ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            double x = b[0] - a[0];
            double y = b[1] - a[1];
            double z = b[2] - a[2];
            double w = b[3] - a[3];
            return GameMath.Sqrt(x * x + y * y + z * z + w * w);
        }

        /// <summary>
        /// Alias for <see cref="Distance(ReadOnlySpan{double}, ReadOnlySpan{double})"/>
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Dist(ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            return Distance(a, b);
        }

        /// <summary>
        /// Calculates the squared euclidian distance between two QVec4d's
        /// </summary>
        /// <param name="a">the first operand</param>
        /// <param name="b">the second operand</param>
        /// <returns>squared distance between a and b</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SquaredDistance(ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            double x = b[0] - a[0];
            double y = b[1] - a[1];
            double z = b[2] - a[2];
            double w = b[3] - a[3];
            return x * x + y * y + z * z + w * w;
        }

        /// <summary>
        /// Alias for <see cref="SquaredDistance(ReadOnlySpan{double}, ReadOnlySpan{double})"/>
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SqrDist(ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            return SquaredDistance(a, b);
        }
        /// <summary>
        /// Calculates the length of a QVec4d
        /// </summary>
        /// <param name="a">vector to calculate length of</param>
        /// <returns>length of a</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Length(ReadOnlySpan<double> a)
        {
            double x = a[0];
            double y = a[1];
            double z = a[2];
            double w = a[3];
            return GameMath.Sqrt(x * x + y * y + z * z + w * w);
        }

        /// <summary>
        /// Alias for <see cref="Length(ReadOnlySpan{double})"/>
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Len(ReadOnlySpan<double> a)
        {
            return Length(a);
        }

        /// <summary>
        /// Calculates the squared length of a QVec4d
        /// </summary>
        /// <param name="a">vector to calculate squared length of</param>
        /// <returns>squared length of a</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SquaredLength(ReadOnlySpan<double> a)
        {
            double x = a[0];
            double y = a[1];
            double z = a[2];
            double w = a[3];
            return x * x + y * y + z * z + w * w;
        }

        /// <summary>
        /// Alias for <see cref="SquaredLength(ReadOnlySpan{double})"/>
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SqrLen(ReadOnlySpan<double> a)
        {
            return SquaredLength(a);
        }

        /// <summary>
        /// Negates the components of a QVec4d
        /// </summary>
        /// <param name="output">the receiving vector</param>
        /// <param name="a">vector to negate</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Negate(Span<double> output, ReadOnlySpan<double> a)
        {
            output[0] = -a[0];
            output[1] = -a[1];
            output[2] = -a[2];
            output[3] = -a[3];
        }

        /// <summary>
        /// Normalize a QVec4d
        /// </summary>
        /// <param name="output">the receiving vector</param>
        /// <param name="a">vector to normalize</param>
        public static void Normalize(Span<double> output, ReadOnlySpan<double> a)
        {
            double x = a[0];
            double y = a[1];
            double z = a[2];
            double w = a[3];
            double len = x * x + y * y + z * z + w * w;
            if (len > 0.0)
            {
                double one = 1.0;
                len = one / GameMath.Sqrt(len);
                output[0] = x * len;
                output[1] = y * len;
                output[2] = z * len;
                output[3] = w * len;
            }
        }

        /// <summary>
        /// Calculates the dot product of two QVec4d's
        /// </summary>
        /// <param name="a">the first operand</param>
        /// <param name="b">the second operand</param>
        /// <returns>dot product of a and b</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Dot(ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            return a[0] * b[0] + a[1] * b[1] + a[2] * b[2] + a[3] * b[3];
        }

        /// <summary>
        /// Performs a linear interpolation between two QVec4d's
        /// </summary>
        /// <param name="output">the receiving vector</param>
        /// <param name="a">the first operand</param>
        /// <param name="b">the second operand</param>
        /// <param name="t">interpolation amount between the two inputs</param>
        public static void Lerp(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b, double t)
        {
            double ax = a[0];
            double ay = a[1];
            double az = a[2];
            double aw = a[3];
            output[0] = ax + t * (b[0] - ax);
            output[1] = ay + t * (b[1] - ay);
            output[2] = az + t * (b[2] - az);
            output[3] = aw + t * (b[3] - aw);
        }


        /// <summary>
        /// Transforms the QVec4d with a mat4.
        /// </summary>
        /// <param name="output">the receiving vector</param>
        /// <param name="a">the vector to transform</param>
        /// <param name="m">matrix to transform with</param>
        public static void TransformMat4(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> m)
        {
            double x = a[0]; double y = a[1]; double z = a[2]; double w = a[3];
            output[0] = m[0] * x + m[4] * y + m[8] * z + m[12] * w;
            output[1] = m[1] * x + m[5] * y + m[9] * z + m[13] * w;
            output[2] = m[2] * x + m[6] * y + m[10] * z + m[14] * w;
            output[3] = m[3] * x + m[7] * y + m[11] * z + m[15] * w;
        }

        /// <summary>
        /// Transforms the QVec4d with a quat
        /// </summary>
        /// <param name="output">the receiving vector</param>
        /// <param name="a">the vector to transform</param>
        /// <param name="q">quaternion to transform with</param>
        public static void transformQuat(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> q)
        {
            double x = a[0]; double y = a[1]; double z = a[2];
            double qx = q[0]; double qy = q[1]; double qz = q[2]; double qw = q[3];

            // calculate quat * vec
            double ix = qw * x + qy * z - qz * y;
            double iy = qw * y + qz * x - qx * z;
            double iz = qw * z + qx * y - qy * x;
            double iw = -qx * x - qy * y - qz * z;

            // calculate result * inverse quat
            output[0] = ix * qw + iw * -qx + iy * -qz - iz * -qy;
            output[1] = iy * qw + iw * -qy + iz * -qx - ix * -qz;
            output[2] = iz * qw + iw * -qz + ix * -qy - iy * -qx;
        }
    }

    /// <summary>
    /// Don't use this class unless you need it to interoperate with Mat4d
    /// </summary>
    public partial class Vec3Utilsd
    {
        /// <summary>
        /// Set the components of a vec3 to the given values
        /// </summary>
        /// <param name="output"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set(Span<double> output, double x, double y, double z)
        {
            output[0] = x;
            output[1] = y;
            output[2] = z;
        }

        /// <summary>
        /// Adds two vec3's
        /// </summary>
        /// <param name="output">the receiving vector</param>
        /// <param name="a">the first operand</param>
        /// <param name="b">the second operand</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            output[0] = a[0] + b[0];
            output[1] = a[1] + b[1];
            output[2] = a[2] + b[2];
        }

        /// <summary>
        /// Subtracts vector b from vector a
        /// </summary>
        /// <param name="output"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Substract(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            output[0] = a[0] - b[0];
            output[1] = a[1] - b[1];
            output[2] = a[2] - b[2];
        }


        /// <summary>
        /// Multiplies two vec3's
        /// </summary>
        /// <param name="output"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Multiply(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            output[0] = a[0] * b[0];
            output[1] = a[1] * b[1];
            output[2] = a[2] * b[2];
        }

        /// <summary>
        /// Alias for <see cref="Multiply(Span{double}, ReadOnlySpan{double}, ReadOnlySpan{double})"/>
        /// </summary>
        /// <param name="output"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Mul(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            Multiply(output, a, b);
        }

        /// <summary>
        /// Divides two vec3's
        /// </summary>
        /// <param name="output"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Divide(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            output[0] = a[0] / b[0];
            output[1] = a[1] / b[1];
            output[2] = a[2] / b[2];
        }

        /// <summary>
        /// Returns the minimum of two vec3's
        /// </summary>
        /// <param name="output"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Min(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            output[0] = Math.Min(a[0], b[0]);
            output[1] = Math.Min(a[1], b[1]);
            output[2] = Math.Min(a[2], b[2]);
        }

        /// <summary>
        /// Returns the maximum of two vec3's
        /// </summary>
        /// <param name="output"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Max(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            output[0] = Math.Max(a[0], b[0]);
            output[1] = Math.Max(a[1], b[1]);
            output[2] = Math.Max(a[2], b[2]);
        }

        /// <summary>
        /// Scales a vec3 by a scalar number
        /// </summary>
        /// <param name="output"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Scale(Span<double> output, ReadOnlySpan<double> a, double b)
        {
            output[0] = a[0] * b;
            output[1] = a[1] * b;
            output[2] = a[2] * b;
        }

        /// <summary>
        /// Adds two vec3's after scaling the second operand by a scalar value
        /// </summary>
        /// <param name="output"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="scale"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ScaleAndAdd(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b, double scale)
        {
            output[0] = a[0] + (b[0] * scale);
            output[1] = a[1] + (b[1] * scale);
            output[2] = a[2] + (b[2] * scale);
        }

        /// <summary>
        /// Calculates the euclidian distance between two vec3's. Returns {Number} distance between a and b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Distance(ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            double x = b[0] - a[0];
            double y = b[1] - a[1];
            double z = b[2] - a[2];
            return GameMath.Sqrt(x * x + y * y + z * z);
        }

        /// <summary>
        /// Calculates the squared euclidian distance between two vec3's. Returns {Number} squared distance between a and b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SquaredDistance(ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            double x = b[0] - a[0];
            double y = b[1] - a[1];
            double z = b[2] - a[2];
            return x * x + y * y + z * z;
        }

        /// <summary>
        /// Calculates the length of a vec3
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Length(ReadOnlySpan<double> a)
        {
            double x = a[0];
            double y = a[1];
            double z = a[2];
            return GameMath.Sqrt(x * x + y * y + z * z);
        }

        /// <summary>
        /// Calculates the squared length of a vec3. Returns {Number} squared length of a
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SquaredLength(ReadOnlySpan<double> a)
        {
            double x = a[0];
            double y = a[1];
            double z = a[2];
            return x * x + y * y + z * z;
        }



        /// <summary>
        /// Negates the components of a vec3
        /// </summary>
        /// <param name="output"></param>
        /// <param name="a"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Negate(Span<double> output, ReadOnlySpan<double> a)
        {
            output[0] = -a[0];
            output[1] = -a[1];
            output[2] = -a[2];
        }

        /// <summary>
        /// Normalize a vec3
        /// </summary>
        /// <param name="output"></param>
        /// <param name="a"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Normalize(Span<double> output, ReadOnlySpan<double> a)
        {
            double x = a[0];
            double y = a[1];
            double z = a[2];
            double len = x * x + y * y + z * z;
            if (len > 0.0)
            {
                double one = 1.0;
                len = one / GameMath.Sqrt(len);
                output[0] = a[0] * len;
                output[1] = a[1] * len;
                output[2] = a[2] * len;
            }
        }

        /// <summary>
        /// Calculates the dot product of two vec3's. Returns {Number} dot product of a and b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Dot(ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            return a[0] * b[0] + a[1] * b[1] + a[2] * b[2];
        }

        /// <summary>
        /// Computes the cross product of two vec3's. Returns {vec3} out
        /// </summary>
        /// <param name="output"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Cross(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b)
        {
            double ax = a[0];
            double ay = a[1];
            double az = a[2];
            double bx = b[0];
            double by = b[1];
            double bz = b[2];

            output[0] = ay * bz - az * by;
            output[1] = az * bx - ax * bz;
            output[2] = ax * by - ay * bx;
        }

        /// <summary>
        /// Performs a linear interpolation between two vec3's
        /// </summary>
        /// <param name="output"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Lerp(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> b, double t)
        {
            double ax = a[0];
            double ay = a[1];
            double az = a[2];
            output[0] = ax + t * (b[0] - ax);
            output[1] = ay + t * (b[1] - ay);
            output[2] = az + t * (b[2] - az);
        }


        /// <summary>
        /// Transforms the vec3 with a mat4. 4th vector component is implicitly '1'. Returns {vec3} out
        /// </summary>
        /// <param name="output"></param>
        /// <param name="a"></param>
        /// <param name="m"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TransformMat4(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> m)
        {
            double x = a[0];
            double y = a[1];
            double z = a[2];
            output[0] = m[0] * x + m[4] * y + m[8] * z + m[12];
            output[1] = m[1] * x + m[5] * y + m[9] * z + m[13];
            output[2] = m[2] * x + m[6] * y + m[10] * z + m[14];
        }

        /// <summary>
        /// Transforms the vec3 with a mat3. Returns {vec3} out
        /// </summary>
        /// <param name="output"></param>
        /// <param name="a"></param>
        /// <param name="m"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TransformMat3(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> m)
        {
            double x = a[0];
            double y = a[1];
            double z = a[2];
            output[0] = x * m[0] + y * m[3] + z * m[6];
            output[1] = x * m[1] + y * m[4] + z * m[7];
            output[2] = x * m[2] + y * m[5] + z * m[8];
        }

        /// <summary>
        /// Transforms the vec3 with a quat
        /// </summary>
        /// <param name="output"></param>
        /// <param name="a"></param>
        /// <param name="q"></param>
        public static void TransformQuat(Span<double> output, ReadOnlySpan<double> a, ReadOnlySpan<double> q)
        {
            double x = a[0];
            double y = a[1];
            double z = a[2];

            double qx = q[0];
            double qy = q[1];
            double qz = q[2];
            double qw = q[3];

            // calculate quat * vec
            double ix = qw * x + qy * z - qz * y;
            double iy = qw * y + qz * x - qx * z;
            double iz = qw * z + qx * y - qy * x;
            double iw = -qx * x - qy * y - qz * z;

            // calculate result * inverse quat
            output[0] = ix * qw + iw * -qx + iy * -qz - iz * -qy;
            output[1] = iy * qw + iw * -qy + iz * -qx - ix * -qz;
            output[2] = iz * qw + iw * -qz + ix * -qy - iy * -qx;
        }
    }
}
