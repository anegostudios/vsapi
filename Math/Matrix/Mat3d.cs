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

namespace Vintagestory.API.MathTools
{
    public class Mat3d
    {
        /// <summary>
        /// Creates a new identity mat3
        /// </summary>
        /// <returns>new 3x3 matrix</returns>
        public static double[] Create()
        {
            double[] output = new double[9];
            output[0] = 1;
            output[1] = 0;
            output[2] = 0;
            output[3] = 0;
            output[4] = 1;
            output[5] = 0;
            output[6] = 0;
            output[7] = 0;
            output[8] = 1;
            return output;
        }

        /// <summary>
        /// Copies the upper-left 3x3 values into the given mat3.
        /// </summary>
        /// <param name="output">the receiving 3x3 matrix</param>
        /// <param name="a">the source 4x4 matrix</param>
        /// <returns>output</returns>
        public static double[] FromMat4(double[] output, double[] a)
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
            return output;
        }

        /// <summary>
        /// Creates a new mat3 initialized with values from an existing matrix
        /// </summary>
        /// <param name="a">matrix to clone</param>
        /// <returns>new 3x3 matrix</returns>
        public static double[] CloneIt(double[] a)
        {
            double[] output = new double[9];
            output[0] = a[0];
            output[1] = a[1];
            output[2] = a[2];
            output[3] = a[3];
            output[4] = a[4];
            output[5] = a[5];
            output[6] = a[6];
            output[7] = a[7];
            output[8] = a[8];
            return output;
        }

        /// <summary>
        /// Copy the values from one mat3 to another
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the source matrix</param>
        /// <returns>output</returns>
        public static double[] Copy(double[] output, double[] a)
        {
            output[0] = a[0];
            output[1] = a[1];
            output[2] = a[2];
            output[3] = a[3];
            output[4] = a[4];
            output[5] = a[5];
            output[6] = a[6];
            output[7] = a[7];
            output[8] = a[8];
            return output;
        }

        /// <summary>
        /// Set a mat3 to the identity matrix
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <returns>output</returns>
        public static double[] Identity_(double[] output)
        {
            output[0] = 1;
            output[1] = 0;
            output[2] = 0;
            output[3] = 0;
            output[4] = 1;
            output[5] = 0;
            output[6] = 0;
            output[7] = 0;
            output[8] = 1;
            return output;
        }

        /// <summary>
        /// Transpose the values of a mat3
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the source matrix</param>
        /// <returns>output</returns>
        public static double[] Transpose(double[] output, double[] a)
        {
            // If we are transposing ourselves we can skip a few steps but have to cache some values
            if (output == a)
            {
                double a01 = a[1];
                double a02 = a[2];
                double a12 = a[5];
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

            return output;
        }

        /// <summary>
        /// Inverts a mat3
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the source matrix</param>
        /// <returns>output</returns>
        public static double[] Invert(double[] output, double[] a)
        {
            double a00 = a[0]; double a01 = a[1]; double a02 = a[2];
            double a10 = a[3]; double a11 = a[4]; double a12 = a[5];
            double a20 = a[6]; double a21 = a[7]; double a22 = a[8];

            double b01 = a22 * a11 - a12 * a21;
            double b11 = -a22 * a10 + a12 * a20;
            double b21 = a21 * a10 - a11 * a20;

            // Calculate the determinant
            double det = a00 * b01 + a01 * b11 + a02 * b21;

            if (det == 0)
            {
                return null;
            }
            double one = 1;
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
            return output;
        }

        /// <summary>
        /// Calculates the adjugate of a mat3
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the source matrix</param>
        /// <returns>output</returns>
        public static double[] Adjoint(double[] output, double[] a)
        {
            double a00 = a[0]; double a01 = a[1]; double a02 = a[2];
            double a10 = a[3]; double a11 = a[4]; double a12 = a[5];
            double a20 = a[6]; double a21 = a[7]; double a22 = a[8];

            output[0] = (a11 * a22 - a12 * a21);
            output[1] = (a02 * a21 - a01 * a22);
            output[2] = (a01 * a12 - a02 * a11);
            output[3] = (a12 * a20 - a10 * a22);
            output[4] = (a00 * a22 - a02 * a20);
            output[5] = (a02 * a10 - a00 * a12);
            output[6] = (a10 * a21 - a11 * a20);
            output[7] = (a01 * a20 - a00 * a21);
            output[8] = (a00 * a11 - a01 * a10);
            return output;
        }

        /// <summary>
        /// Calculates the determinant of a mat3
        /// </summary>
        /// <param name="a">the source matrix</param>
        /// <returns>determinant of a</returns>
        public static double Determinant(double[] a)
        {
            double a00 = a[0]; double a01 = a[1]; double a02 = a[2];
            double a10 = a[3]; double a11 = a[4]; double a12 = a[5];
            double a20 = a[6]; double a21 = a[7]; double a22 = a[8];

            return a00 * (a22 * a11 - a12 * a21) + a01 * (-a22 * a10 + a12 * a20) + a02 * (a21 * a10 - a11 * a20);
        }

        /// <summary>
        /// Multiplies two mat3's
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the first operand</param>
        /// <param name="b">the second operand</param>
        /// <returns>output</returns>
        public static double[] Multiply(double[] output, double[] a, double[] b)
        {
            double a00 = a[0]; double a01 = a[1]; double a02 = a[2];
            double a10 = a[3]; double a11 = a[4]; double a12 = a[5];
            double a20 = a[6]; double a21 = a[7]; double a22 = a[8];

            double b00 = b[0]; double b01 = b[1]; double b02 = b[2];
            double b10 = b[3]; double b11 = b[4]; double b12 = b[5];
            double b20 = b[6]; double b21 = b[7]; double b22 = b[8];

            output[0] = b00 * a00 + b01 * a10 + b02 * a20;
            output[1] = b00 * a01 + b01 * a11 + b02 * a21;
            output[2] = b00 * a02 + b01 * a12 + b02 * a22;

            output[3] = b10 * a00 + b11 * a10 + b12 * a20;
            output[4] = b10 * a01 + b11 * a11 + b12 * a21;
            output[5] = b10 * a02 + b11 * a12 + b12 * a22;

            output[6] = b20 * a00 + b21 * a10 + b22 * a20;
            output[7] = b20 * a01 + b21 * a11 + b22 * a21;
            output[8] = b20 * a02 + b21 * a12 + b22 * a22;
            return output;
        }

        /// <summary>
        /// Alias for <see cref="Multiply(double[], double[], double[])"/>
        /// </summary>
        public static double[] Mul(double[] output, double[] a, double[] b)
        {
            return Multiply(output, a, b);
        }

        /// <summary>
        /// Translate a mat3 by the given vector
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the matrix to translate</param>
        /// <param name="v">vector to translate by</param>
        /// <returns>output</returns>
        public static double[] Translate(double[] output, double[] a, double[] v)
        {
            double a00 = a[0]; double a01 = a[1]; double a02 = a[2];
            double a10 = a[3]; double a11 = a[4]; double a12 = a[5];
            double a20 = a[6]; double a21 = a[7]; double a22 = a[8];
            double x = v[0]; double y = v[1];

            output[0] = a00;
            output[1] = a01;
            output[2] = a02;

            output[3] = a10;
            output[4] = a11;
            output[5] = a12;

            output[6] = x * a00 + y * a10 + a20;
            output[7] = x * a01 + y * a11 + a21;
            output[8] = x * a02 + y * a12 + a22;
            return output;
        }

        /// <summary>
        /// Rotates a mat3 by the given angle
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the matrix to rotate</param>
        /// <param name="rad">the angle to rotate the matrix by</param>
        /// <returns>output</returns>
        public static double[] Rotate(double[] output, double[] a, double rad)
        {
            double a00 = a[0]; double a01 = a[1]; double a02 = a[2];
            double a10 = a[3]; double a11 = a[4]; double a12 = a[5];
            double a20 = a[6]; double a21 = a[7]; double a22 = a[8];

            double s = GameMath.Sin(rad);
            double c = GameMath.Cos(rad);

            output[0] = c * a00 + s * a10;
            output[1] = c * a01 + s * a11;
            output[2] = c * a02 + s * a12;

            output[3] = c * a10 - s * a00;
            output[4] = c * a11 - s * a01;
            output[5] = c * a12 - s * a02;

            output[6] = a20;
            output[7] = a21;
            output[8] = a22;
            return output;
        }

        /// <summary>
        /// Scales the mat3 by the dimensions in the given vec2
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the matrix to rotate</param>
        /// <param name="v">the vec2 to scale the matrix by</param>
        /// <returns>output</returns>
        public static double[] Scale(double[] output, double[] a, double[] v)
        {
            double x = v[0]; double y = v[1];

            output[0] = x * a[0];
            output[1] = x * a[1];
            output[2] = x * a[2];

            output[3] = y * a[3];
            output[4] = y * a[4];
            output[5] = y * a[5];

            output[6] = a[6];
            output[7] = a[7];
            output[8] = a[8];
            return output;
        }

        /// <summary>
        /// Copies the values from a mat2d into a mat3
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the matrix to copy</param>
        /// <returns>output</returns>
        public static double[] FromMat2d(double[] output, double[] a)
        {
            output[0] = a[0];
            output[1] = a[1];
            output[2] = 0;

            output[3] = a[2];
            output[4] = a[3];
            output[5] = 0;

            output[6] = a[4];
            output[7] = a[5];
            output[8] = 1;
            return output;
        }

        /// <summary>
        /// Calculates a 3x3 matrix from the given quaternion
        /// </summary>
        /// <param name="output">mat3 receiving operation result</param>
        /// <param name="q">Quaternion to create matrix from</param>
        /// <returns>output</returns>
        public static double[] FromQuat(double[] output, double[] q)
        {
            double x = q[0]; double y = q[1]; double z = q[2]; double w = q[3];
            double x2 = x + x;
            double y2 = y + y;
            double z2 = z + z;

            double xx = x * x2;
            double xy = x * y2;
            double xz = x * z2;
            double yy = y * y2;
            double yz = y * z2;
            double zz = z * z2;
            double wx = w * x2;
            double wy = w * y2;
            double wz = w * z2;

            output[0] = 1 - (yy + zz);
            output[3] = xy + wz;
            output[6] = xz - wy;

            output[1] = xy - wz;
            output[4] = 1 - (xx + zz);
            output[7] = yz + wx;

            output[2] = xz + wy;
            output[5] = yz - wx;
            output[8] = 1 - (xx + yy);

            return output;
        }

        /// <summary>
        /// Calculates a 3x3 normal matrix (transpose inverse) from the 4x4 matrix
        /// </summary>
        /// <param name="output">mat3 receiving operation result</param>
        /// <param name="a">Mat4 to derive the normal matrix from</param>
        /// <returns>output</returns>
        public static double[] NormalFromMat4(double[] output, double[] a)
        {
            double a00 = a[0]; double a01 = a[1]; double a02 = a[2]; double a03 = a[3];
            double a10 = a[4]; double a11 = a[5]; double a12 = a[6]; double a13 = a[7];
            double a20 = a[8]; double a21 = a[9]; double a22 = a[10]; double a23 = a[11];
            double a30 = a[12]; double a31 = a[13]; double a32 = a[14]; double a33 = a[15];

            double b00 = a00 * a11 - a01 * a10;
            double b01 = a00 * a12 - a02 * a10;
            double b02 = a00 * a13 - a03 * a10;
            double b03 = a01 * a12 - a02 * a11;
            double b04 = a01 * a13 - a03 * a11;
            double b05 = a02 * a13 - a03 * a12;
            double b06 = a20 * a31 - a21 * a30;
            double b07 = a20 * a32 - a22 * a30;
            double b08 = a20 * a33 - a23 * a30;
            double b09 = a21 * a32 - a22 * a31;
            double b10 = a21 * a33 - a23 * a31;
            double b11 = a22 * a33 - a23 * a32;

            // Calculate the determinant
            double det = b00 * b11 - b01 * b10 + b02 * b09 + b03 * b08 - b04 * b07 + b05 * b06;

            if (det == 0)
            {
                return null;
            }
            double one = 1;
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

            return output;
        }
    }
}
