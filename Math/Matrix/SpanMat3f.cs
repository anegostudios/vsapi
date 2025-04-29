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
    public partial class Mat3f
    {
        /// <summary>
        /// Copies the upper-left 3x3 values into the given mat3.
        /// </summary>
        /// <param name="output">the receiving 3x3 matrix</param>
        /// <param name="a">the source 4x4 matrix</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromMat4(Span<float> output, ReadOnlySpan<float> a)
        {
            output[0] = a[0];
            output[1] = a[1];
            output[2] = a[2];
            output[3] = a[4];
            output[4] = a[5];
            output[5] = a[6];
            output[6] = a[8];
            output[7] = a[9];
            output[8] = a[10];
        }

        /// <summary>
        /// Set a mat3 to the identity matrix
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Identity(Span<float> output)
        {
            output[0] = 1f;
            output[1] = 0f;
            output[2] = 0f;
            output[3] = 0f;
            output[4] = 1f;
            output[5] = 0f;
            output[6] = 0f;
            output[7] = 0f;
            output[8] = 1f;
        }

        /// <summary>
        /// Transpose the values of a mat3
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the source matrix</param>
        public static void Transpose(Span<float> output, ReadOnlySpan<float> a)
        {
            // If we are transposing ourselves we can skip a few steps but have to cache some values
            if (output == a)
            {
                float a01 = a[1];
                float a02 = a[2];
                float a12 = a[5];
                output[1] = a[3];
                output[2] = a[6];
                output[3] = a01;
                output[5] = a[7];
                output[6] = a02;
                output[7] = a12;
            }
            else
            {
                output[0] = a[0];
                output[1] = a[3];
                output[2] = a[6];
                output[3] = a[1];
                output[4] = a[4];
                output[5] = a[7];
                output[6] = a[2];
                output[7] = a[5];
                output[8] = a[8];
            }
        }

        /// <summary>
        /// Inverts a mat3
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the source matrix</param>
        /// <returns><see langword="true"/> if the operation was successful</returns>
        public static bool Invert(Span<float> output, ReadOnlySpan<float> a)
        {
            float a00 = a[0]; float a01 = a[1]; float a02 = a[2];
            float a10 = a[3]; float a11 = a[4]; float a12 = a[5];
            float a20 = a[6]; float a21 = a[7]; float a22 = a[8];

            float b01 = a22 * a11 - a12 * a21;
            float b11 = -a22 * a10 + a12 * a20;
            float b21 = a21 * a10 - a11 * a20;

            // Calculate the determinant
            float det = a00 * b01 + a01 * b11 + a02 * b21;

            if (det == 0f)
            {
                return false;
            }
            float one = 1f;
            det = one / det;

            output[0] = b01 * det;
            output[1] = (-a22 * a01 + a02 * a21) * det;
            output[2] = (a12 * a01 - a02 * a11) * det;
            output[3] = b11 * det;
            output[4] = (a22 * a00 - a02 * a20) * det;
            output[5] = (-a12 * a00 + a02 * a10) * det;
            output[6] = b21 * det;
            output[7] = (-a21 * a00 + a01 * a20) * det;
            output[8] = (a11 * a00 - a01 * a10) * det;
            return true;
        }

        /// <summary>
        /// Calculates the adjugate of a mat3
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the source matrix</param>
        public static void Adjoint(Span<float> output, ReadOnlySpan<float> a)
        {
            float a00 = a[0]; float a01 = a[1]; float a02 = a[2];
            float a10 = a[3]; float a11 = a[4]; float a12 = a[5];
            float a20 = a[6]; float a21 = a[7]; float a22 = a[8];

            output[0] = (a11 * a22 - a12 * a21);
            output[1] = (a02 * a21 - a01 * a22);
            output[2] = (a01 * a12 - a02 * a11);
            output[3] = (a12 * a20 - a10 * a22);
            output[4] = (a00 * a22 - a02 * a20);
            output[5] = (a02 * a10 - a00 * a12);
            output[6] = (a10 * a21 - a11 * a20);
            output[7] = (a01 * a20 - a00 * a21);
            output[8] = (a00 * a11 - a01 * a10);
        }

        /// <summary>
        /// Calculates the determinant of a mat3
        /// </summary>
        /// <param name="a">the source matrix</param>
        public static float Determinant(ReadOnlySpan<float> a)
        {
            float a00 = a[0]; float a01 = a[1]; float a02 = a[2];
            float a10 = a[3]; float a11 = a[4]; float a12 = a[5];
            float a20 = a[6]; float a21 = a[7]; float a22 = a[8];

            return a00 * (a22 * a11 - a12 * a21) + a01 * (-a22 * a10 + a12 * a20) + a02 * (a21 * a10 - a11 * a20);
        }

        /// <summary>
        /// Multiplies two mat3's
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the first operand</param>
        /// <param name="b">the second operand</param>
        public static void Multiply(Span<float> output, ReadOnlySpan<float> a, ReadOnlySpan<float> b)
        {
            float a00 = a[0]; float a01 = a[1]; float a02 = a[2];
            float a10 = a[3]; float a11 = a[4]; float a12 = a[5];
            float a20 = a[6]; float a21 = a[7]; float a22 = a[8];

            float b00 = b[0]; float b01 = b[1]; float b02 = b[2];
            float b10 = b[3]; float b11 = b[4]; float b12 = b[5];
            float b20 = b[6]; float b21 = b[7]; float b22 = b[8];

            output[0] = b00 * a00 + b01 * a10 + b02 * a20;
            output[1] = b00 * a01 + b01 * a11 + b02 * a21;
            output[2] = b00 * a02 + b01 * a12 + b02 * a22;

            output[3] = b10 * a00 + b11 * a10 + b12 * a20;
            output[4] = b10 * a01 + b11 * a11 + b12 * a21;
            output[5] = b10 * a02 + b11 * a12 + b12 * a22;

            output[6] = b20 * a00 + b21 * a10 + b22 * a20;
            output[7] = b20 * a01 + b21 * a11 + b22 * a21;
            output[8] = b20 * a02 + b21 * a12 + b22 * a22;
        }

        /// <summary>
        /// Alias for <see cref="Multiply(Span{float}, ReadOnlySpan{float}, ReadOnlySpan{float})"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Mul(Span<float> output, ReadOnlySpan<float> a, ReadOnlySpan<float> b)
        {
            Multiply(output, a, b);
        }

        /// <summary>
        /// Translate a mat3 by the given vector
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the matrix to translate</param>
        /// <param name="v">vector to translate by</param>
        public static void Translate(Span<float> output, ReadOnlySpan<float> a, ReadOnlySpan<float> v)
        {
            float a00 = a[0]; float a01 = a[1]; float a02 = a[2];
            float a10 = a[3]; float a11 = a[4]; float a12 = a[5];
            float a20 = a[6]; float a21 = a[7]; float a22 = a[8];
            float x = v[0]; float y = v[1];

            output[0] = a00;
            output[1] = a01;
            output[2] = a02;

            output[3] = a10;
            output[4] = a11;
            output[5] = a12;

            output[6] = x * a00 + y * a10 + a20;
            output[7] = x * a01 + y * a11 + a21;
            output[8] = x * a02 + y * a12 + a22;
        }

        /// <summary>
        /// Rotates a mat3 by the given angle
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the matrix to rotate</param>
        /// <param name="rad">the angle to rotate the matrix by</param>
        public static void Rotate(Span<float> output, ReadOnlySpan<float> a, float rad)
        {
            float a00 = a[0]; float a01 = a[1]; float a02 = a[2];
            float a10 = a[3]; float a11 = a[4]; float a12 = a[5];
            float a20 = a[6]; float a21 = a[7]; float a22 = a[8];

            float s = GameMath.Sin(rad);
            float c = GameMath.Cos(rad);

            output[0] = c * a00 + s * a10;
            output[1] = c * a01 + s * a11;
            output[2] = c * a02 + s * a12;

            output[3] = c * a10 - s * a00;
            output[4] = c * a11 - s * a01;
            output[5] = c * a12 - s * a02;

            output[6] = a20;
            output[7] = a21;
            output[8] = a22;
        }

        /// <summary>
        /// Scales the mat3 by the dimensions in the given vec2
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the matrix to rotate</param>
        /// <param name="v">the vec2 to scale the matrix by</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Scale(Span<float> output, ReadOnlySpan<float> a, ReadOnlySpan<float> v)
        {
            float x = v[0]; float y = v[1];

            output[0] = x * a[0];
            output[1] = x * a[1];
            output[2] = x * a[2];

            output[3] = y * a[3];
            output[4] = y * a[4];
            output[5] = y * a[5];

            output[6] = a[6];
            output[7] = a[7];
            output[8] = a[8];
        }

        /// <summary>
        /// Copies the values from a mat2d into a mat3
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the matrix to copy</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromMat2d(Span<float> output, ReadOnlySpan<float> a)
        {
            output[0] = a[0];
            output[1] = a[1];
            output[2] = 0f;

            output[3] = a[2];
            output[4] = a[3];
            output[5] = 0f;

            output[6] = a[4];
            output[7] = a[5];
            output[8] = 1f;
        }

        /// <summary>
        /// Calculates a 3x3 matrix from the given quaternion
        /// </summary>
        /// <param name="output">mat3 receiving operation result</param>
        /// <param name="q">Quaternion to create matrix from</param>
        public static void FromQuat(Span<float> output, ReadOnlySpan<float> q)
        {
            float x = q[0]; float y = q[1]; float z = q[2]; float w = q[3];
            float x2 = x + x;
            float y2 = y + y;
            float z2 = z + z;

            float xx = x * x2;
            float xy = x * y2;
            float xz = x * z2;
            float yy = y * y2;
            float yz = y * z2;
            float zz = z * z2;
            float wx = w * x2;
            float wy = w * y2;
            float wz = w * z2;

            output[0] = 1f - (yy + zz);
            output[3] = xy + wz;
            output[6] = xz - wy;

            output[1] = xy - wz;
            output[4] = 1f - (xx + zz);
            output[7] = yz + wx;

            output[2] = xz + wy;
            output[5] = yz - wx;
            output[8] = 1f - (xx + yy);
        }

        /// <summary>
        /// Calculates a 3x3 normal matrix (transpose inverse) from the 4x4 matrix
        /// </summary>
        /// <param name="output">mat3 receiving operation result</param>
        /// <param name="a">Mat4 to derive the normal matrix from</param>
        /// <returns><see langword="true"/> if the operation was successful</returns>
        public static bool NormalFromMat4(Span<float> output, ReadOnlySpan<float> a)
        {
            float a00 = a[0]; float a01 = a[1]; float a02 = a[2]; float a03 = a[3];
            float a10 = a[4]; float a11 = a[5]; float a12 = a[6]; float a13 = a[7];
            float a20 = a[8]; float a21 = a[9]; float a22 = a[10]; float a23 = a[11];
            float a30 = a[12]; float a31 = a[13]; float a32 = a[14]; float a33 = a[15];

            float b00 = a00 * a11 - a01 * a10;
            float b01 = a00 * a12 - a02 * a10;
            float b02 = a00 * a13 - a03 * a10;
            float b03 = a01 * a12 - a02 * a11;
            float b04 = a01 * a13 - a03 * a11;
            float b05 = a02 * a13 - a03 * a12;
            float b06 = a20 * a31 - a21 * a30;
            float b07 = a20 * a32 - a22 * a30;
            float b08 = a20 * a33 - a23 * a30;
            float b09 = a21 * a32 - a22 * a31;
            float b10 = a21 * a33 - a23 * a31;
            float b11 = a22 * a33 - a23 * a32;

            // Calculate the determinant
            float det = b00 * b11 - b01 * b10 + b02 * b09 + b03 * b08 - b04 * b07 + b05 * b06;

            if (det == 0f)
            {
                return false;
            }
            float one = 1f;
            det = one / det;

            output[0] = (a11 * b11 - a12 * b10 + a13 * b09) * det;
            output[1] = (a12 * b08 - a10 * b11 - a13 * b07) * det;
            output[2] = (a10 * b10 - a11 * b08 + a13 * b06) * det;

            output[3] = (a02 * b10 - a01 * b11 - a03 * b09) * det;
            output[4] = (a00 * b11 - a02 * b08 + a03 * b07) * det;
            output[5] = (a01 * b08 - a00 * b10 - a03 * b06) * det;

            output[6] = (a31 * b05 - a32 * b04 + a33 * b03) * det;
            output[7] = (a32 * b02 - a30 * b05 - a33 * b01) * det;
            output[8] = (a30 * b04 - a31 * b02 + a33 * b00) * det;

            return true;
        }
    }
}
