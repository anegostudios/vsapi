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

#nullable disable

namespace Vintagestory.API.MathTools
{
    /// <summary>
    /// 4x4 Matrix Math
    /// </summary>
    public class Mat4f
    {
        /// <summary>
        /// Creates a new identity mat4
        /// 0 4 8  12
        /// 1 5 9  13
        /// 2 6 10 14
        /// 3 7 11 15
        /// </summary>
        /// <returns>{mat4} a new 4x4 matrix</returns>
        public static float[] Create()
        {
            float[] output = new float[16];  // a new array is automatically initialised to the default float value of 0f
            output[0] = 1f;
            output[5] = 1f;
            output[10] = 1f;
            output[15] = 1f;
            return output;
        }


        /// <summary>
        /// Creates a new mat4 initialized with values from an existing matrix
        /// </summary>
        /// <param name="a">a matrix to clone</param>
        /// <returns>{mat4} a new 4x4 matrix</returns>
        public static float[] CloneIt(float[] a)
        {
            float[] output = new float[16];
            output[0] = a[0];
            output[1] = a[1];
            output[2] = a[2];
            output[3] = a[3];
            output[4] = a[4];
            output[5] = a[5];
            output[6] = a[6];
            output[7] = a[7];
            output[8] = a[8];
            output[9] = a[9];
            output[10] = a[10];
            output[11] = a[11];
            output[12] = a[12];
            output[13] = a[13];
            output[14] = a[14];
            output[15] = a[15];
            return output;
        }

        /// <summary>
        /// Copy the values from one mat4 to another
        /// </summary>
        /// <param name="output">{mat4} out the receiving matrix</param>
        /// <param name="a">{mat4} a the source matrix</param>
        /// <returns>{mat4} out</returns>
        public static float[] Copy(float[] output, float[] a)
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
            output[9] = a[9];
            output[10] = a[10];
            output[11] = a[11];
            output[12] = a[12];
            output[13] = a[13];
            output[14] = a[14];
            output[15] = a[15];
            return output;
        }

        /// <summary>
        /// Set a mat4 to the identity matrix
        /// </summary>
        /// <param name="output">{mat4} out the receiving matrix</param>
        /// <returns>{mat4} out</returns>
        public static float[] Identity(float[] output)
        {
            output[0] = 1;
            output[1] = 0;
            output[2] = 0;
            output[3] = 0;
            output[4] = 0;
            output[5] = 1;
            output[6] = 0;
            output[7] = 0;
            output[8] = 0;
            output[9] = 0;
            output[10] = 1;
            output[11] = 0;
            output[12] = 0;
            output[13] = 0;
            output[14] = 0;
            output[15] = 1;
            return output;
        }


        /// <summary>
        /// Set a mat4 to the identity matrix with a scale applied
        /// </summary>
        /// <param name="output">{mat4} out the receiving matrix</param>
        /// <param name="scale"></param>
        /// <returns>{mat4} out</returns>
        public static float[] Identity_Scaled(float[] output, float scale)
        {
            output[0] = scale;
            output[1] = 0;
            output[2] = 0;
            output[3] = 0;
            output[4] = 0;
            output[5] = scale;
            output[6] = 0;
            output[7] = 0;
            output[8] = 0;
            output[9] = 0;
            output[10] = scale;
            output[11] = 0;
            output[12] = 0;
            output[13] = 0;
            output[14] = 0;
            output[15] = 1;
            return output;
        }


        /// <summary>
        /// Transpose the values of a mat4
        /// </summary>
        /// <param name="output">{mat4} out the receiving matrix</param>
        /// <param name="a">{mat4} a the source matrix</param>
        /// <returns>{mat4} out</returns>
        public static float[] Transpose(float[] output, float[] a)
        {
            // If we are transposing ourselves we can skip a few steps but have to cache some values
            if (output == a)
            {
                float a01 = a[1]; float a02 = a[2]; float a03 = a[3];
                float a12 = a[6]; float a13 = a[7];
                float a23 = a[11];

                output[1] = a[4];
                output[2] = a[8];
                output[3] = a[12];
                output[4] = a01;
                output[6] = a[9];
                output[7] = a[13];
                output[8] = a02;
                output[9] = a12;
                output[11] = a[14];
                output[12] = a03;
                output[13] = a13;
                output[14] = a23;
            }
            else
            {
                output[0] = a[0];
                output[1] = a[4];
                output[2] = a[8];
                output[3] = a[12];
                output[4] = a[1];
                output[5] = a[5];
                output[6] = a[9];
                output[7] = a[13];
                output[8] = a[2];
                output[9] = a[6];
                output[10] = a[10];
                output[11] = a[14];
                output[12] = a[3];
                output[13] = a[7];
                output[14] = a[11];
                output[15] = a[15];
            }

            return output;
        }

        /// <summary>
        /// Inverts a mat4
        /// </summary>
        /// <param name="output">{mat4} out the receiving matrix</param>
        /// <param name="a">{mat4} a the source matrix</param>
        /// <returns>{mat4} out</returns>
        public static float[] Invert(float[] output, float[] a)
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

            if (det == 0)
            {
                return null;
            }
            float one = 1;
            det = one / det;

            output[0] = (a11 * b11 - a12 * b10 + a13 * b09) * det;
            output[1] = (a02 * b10 - a01 * b11 - a03 * b09) * det;
            output[2] = (a31 * b05 - a32 * b04 + a33 * b03) * det;
            output[3] = (a22 * b04 - a21 * b05 - a23 * b03) * det;
            output[4] = (a12 * b08 - a10 * b11 - a13 * b07) * det;
            output[5] = (a00 * b11 - a02 * b08 + a03 * b07) * det;
            output[6] = (a32 * b02 - a30 * b05 - a33 * b01) * det;
            output[7] = (a20 * b05 - a22 * b02 + a23 * b01) * det;
            output[8] = (a10 * b10 - a11 * b08 + a13 * b06) * det;
            output[9] = (a01 * b08 - a00 * b10 - a03 * b06) * det;
            output[10] = (a30 * b04 - a31 * b02 + a33 * b00) * det;
            output[11] = (a21 * b02 - a20 * b04 - a23 * b00) * det;
            output[12] = (a11 * b07 - a10 * b09 - a12 * b06) * det;
            output[13] = (a00 * b09 - a01 * b07 + a02 * b06) * det;
            output[14] = (a31 * b01 - a30 * b03 - a32 * b00) * det;
            output[15] = (a20 * b03 - a21 * b01 + a22 * b00) * det;

            return output;
        }

        /// <summary>
        /// Calculates the adjugate of a mat4   
        /// </summary>
        /// <param name="output">{mat4} out the receiving matrix</param>
        /// <param name="a">{mat4} a the source matrix</param>
        /// <returns>{mat4} out</returns>
        public static float[] Adjoint(float[] output, float[] a)
        {
            float a00 = a[0]; float a01 = a[1]; float a02 = a[2]; float a03 = a[3];
            float a10 = a[4]; float a11 = a[5]; float a12 = a[6]; float a13 = a[7];
            float a20 = a[8]; float a21 = a[9]; float a22 = a[10]; float a23 = a[11];
            float a30 = a[12]; float a31 = a[13]; float a32 = a[14]; float a33 = a[15];

            output[0] = (a11 * (a22 * a33 - a23 * a32) - a21 * (a12 * a33 - a13 * a32) + a31 * (a12 * a23 - a13 * a22));
            output[1] = -(a01 * (a22 * a33 - a23 * a32) - a21 * (a02 * a33 - a03 * a32) + a31 * (a02 * a23 - a03 * a22));
            output[2] = (a01 * (a12 * a33 - a13 * a32) - a11 * (a02 * a33 - a03 * a32) + a31 * (a02 * a13 - a03 * a12));
            output[3] = -(a01 * (a12 * a23 - a13 * a22) - a11 * (a02 * a23 - a03 * a22) + a21 * (a02 * a13 - a03 * a12));
            output[4] = -(a10 * (a22 * a33 - a23 * a32) - a20 * (a12 * a33 - a13 * a32) + a30 * (a12 * a23 - a13 * a22));
            output[5] = (a00 * (a22 * a33 - a23 * a32) - a20 * (a02 * a33 - a03 * a32) + a30 * (a02 * a23 - a03 * a22));
            output[6] = -(a00 * (a12 * a33 - a13 * a32) - a10 * (a02 * a33 - a03 * a32) + a30 * (a02 * a13 - a03 * a12));
            output[7] = (a00 * (a12 * a23 - a13 * a22) - a10 * (a02 * a23 - a03 * a22) + a20 * (a02 * a13 - a03 * a12));
            output[8] = (a10 * (a21 * a33 - a23 * a31) - a20 * (a11 * a33 - a13 * a31) + a30 * (a11 * a23 - a13 * a21));
            output[9] = -(a00 * (a21 * a33 - a23 * a31) - a20 * (a01 * a33 - a03 * a31) + a30 * (a01 * a23 - a03 * a21));
            output[10] = (a00 * (a11 * a33 - a13 * a31) - a10 * (a01 * a33 - a03 * a31) + a30 * (a01 * a13 - a03 * a11));
            output[11] = -(a00 * (a11 * a23 - a13 * a21) - a10 * (a01 * a23 - a03 * a21) + a20 * (a01 * a13 - a03 * a11));
            output[12] = -(a10 * (a21 * a32 - a22 * a31) - a20 * (a11 * a32 - a12 * a31) + a30 * (a11 * a22 - a12 * a21));
            output[13] = (a00 * (a21 * a32 - a22 * a31) - a20 * (a01 * a32 - a02 * a31) + a30 * (a01 * a22 - a02 * a21));
            output[14] = -(a00 * (a11 * a32 - a12 * a31) - a10 * (a01 * a32 - a02 * a31) + a30 * (a01 * a12 - a02 * a11));
            output[15] = (a00 * (a11 * a22 - a12 * a21) - a10 * (a01 * a22 - a02 * a21) + a20 * (a01 * a12 - a02 * a11));
            return output;
        }

        /// <summary>
        /// Calculates the determinant of a mat4
        /// </summary>
        /// <param name="a">{mat4} a the source matrix</param>
        /// <returns>{Number} determinant of a</returns>
        public static float Determinant(float[] a)
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
            return b00 * b11 - b01 * b10 + b02 * b09 + b03 * b08 - b04 * b07 + b05 * b06;
        }

        /// <summary>
        /// Multiplies two mat4's
        /// 
        /// </summary>
        /// <param name="output">{mat4} out the receiving matrix</param>
        /// <param name="a">{mat4} a the first operand</param>
        /// <param name="b">{mat4} b the second operand</param>
        /// <returns>{mat4} out</returns>
        public static float[] Multiply(float[] output, float[] a, float[] b)
        {
            float a00 = a[0]; float a01 = a[1]; float a02 = a[2]; float a03 = a[3];
            float a10 = a[4]; float a11 = a[5]; float a12 = a[6]; float a13 = a[7];
            float a20 = a[8]; float a21 = a[9]; float a22 = a[10]; float a23 = a[11];
            float a30 = a[12]; float a31 = a[13]; float a32 = a[14]; float a33 = a[15];

            // Cache only the current line of the second matrix
            float b0 = b[0]; float b1 = b[1]; float b2 = b[2]; float b3 = b[3];
            output[0] = b0 * a00 + b1 * a10 + b2 * a20 + b3 * a30;
            output[1] = b0 * a01 + b1 * a11 + b2 * a21 + b3 * a31;
            output[2] = b0 * a02 + b1 * a12 + b2 * a22 + b3 * a32;
            output[3] = b0 * a03 + b1 * a13 + b2 * a23 + b3 * a33;

            b0 = b[4]; b1 = b[5]; b2 = b[6]; b3 = b[7];
            output[4] = b0 * a00 + b1 * a10 + b2 * a20 + b3 * a30;
            output[5] = b0 * a01 + b1 * a11 + b2 * a21 + b3 * a31;
            output[6] = b0 * a02 + b1 * a12 + b2 * a22 + b3 * a32;
            output[7] = b0 * a03 + b1 * a13 + b2 * a23 + b3 * a33;

            b0 = b[8]; b1 = b[9]; b2 = b[10]; b3 = b[11];
            output[8] = b0 * a00 + b1 * a10 + b2 * a20 + b3 * a30;
            output[9] = b0 * a01 + b1 * a11 + b2 * a21 + b3 * a31;
            output[10] = b0 * a02 + b1 * a12 + b2 * a22 + b3 * a32;
            output[11] = b0 * a03 + b1 * a13 + b2 * a23 + b3 * a33;

            b0 = b[12]; b1 = b[13]; b2 = b[14]; b3 = b[15];
            output[12] = b0 * a00 + b1 * a10 + b2 * a20 + b3 * a30;
            output[13] = b0 * a01 + b1 * a11 + b2 * a21 + b3 * a31;
            output[14] = b0 * a02 + b1 * a12 + b2 * a22 + b3 * a32;
            output[15] = b0 * a03 + b1 * a13 + b2 * a23 + b3 * a33;
            return output;
        }

        /// <summary>
        /// mat4.multiply
        /// </summary>
        /// <param name="output"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float[] Mul(float[] output, float[] a, float[] b)
        {
            return Multiply(output, a, b);
        }


        /// <summary>
        /// Translate a mat4 by the given vector
        /// </summary>
        /// <param name="output">{mat4} out the receiving matrix</param>
        /// <param name="input">{mat4} a the matrix to translate</param>
        /// <param name="x">{vec3} v vector to translate by</param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns>{mat4} out</returns>
        public static float[] Translate(float[] output, float[] input, float x, float y, float z)
        {
            if (input == output)
            {
                output[12] = input[0] * x + input[4] * y + input[8] * z + input[12];
                output[13] = input[1] * x + input[5] * y + input[9] * z + input[13];
                output[14] = input[2] * x + input[6] * y + input[10] * z + input[14];
                output[15] = input[3] * x + input[7] * y + input[11] * z + input[15];
            }
            else
            {
                float a00; float a01; float a02; float a03;
                float a10; float a11; float a12; float a13;
                float a20; float a21; float a22; float a23;

                a00 = input[0]; a01 = input[1]; a02 = input[2]; a03 = input[3];
                a10 = input[4]; a11 = input[5]; a12 = input[6]; a13 = input[7];
                a20 = input[8]; a21 = input[9]; a22 = input[10]; a23 = input[11];

                output[0] = a00; output[1] = a01; output[2] = a02; output[3] = a03;
                output[4] = a10; output[5] = a11; output[6] = a12; output[7] = a13;
                output[8] = a20; output[9] = a21; output[10] = a22; output[11] = a23;

                output[12] = a00 * x + a10 * y + a20 * z + input[12];
                output[13] = a01 * x + a11 * y + a21 * z + input[13];
                output[14] = a02 * x + a12 * y + a22 * z + input[14];
                output[15] = a03 * x + a13 * y + a23 * z + input[15];
            }

            return output;
        }

        /// <summary>
        /// Translate a mat4 by the given vector
        /// </summary>
        /// <param name="output">{mat4} out the receiving matrix</param>
        /// <param name="input">{mat4} a the matrix to translate</param>
        /// <param name="translate">{vec3} v vector to translate by</param>
        /// <returns>{mat4} out</returns>
        public static float[] Translate(float[] output, float[] input, float[] translate)
        {
            float x = translate[0]; float y = translate[1]; float z = translate[2];
            if (input == output)
            {
                output[12] = input[0] * x + input[4] * y + input[8] * z + input[12];
                output[13] = input[1] * x + input[5] * y + input[9] * z + input[13];
                output[14] = input[2] * x + input[6] * y + input[10] * z + input[14];
                output[15] = input[3] * x + input[7] * y + input[11] * z + input[15];
            }
            else
            {
                float a00; float a01; float a02; float a03;
                float a10; float a11; float a12; float a13;
                float a20; float a21; float a22; float a23;

                a00 = input[0]; a01 = input[1]; a02 = input[2]; a03 = input[3];
                a10 = input[4]; a11 = input[5]; a12 = input[6]; a13 = input[7];
                a20 = input[8]; a21 = input[9]; a22 = input[10]; a23 = input[11];

                output[0] = a00; output[1] = a01; output[2] = a02; output[3] = a03;
                output[4] = a10; output[5] = a11; output[6] = a12; output[7] = a13;
                output[8] = a20; output[9] = a21; output[10] = a22; output[11] = a23;

                output[12] = a00 * x + a10 * y + a20 * z + input[12];
                output[13] = a01 * x + a11 * y + a21 * z + input[13];
                output[14] = a02 * x + a12 * y + a22 * z + input[14];
                output[15] = a03 * x + a13 * y + a23 * z + input[15];
            }

            return output;
        }

        /// <summary>
        /// Scales the mat4 by the dimensions in the given vec3
        /// </summary>
        /// <param name="output">{mat4} out the receiving matrix</param>
        /// <param name="a">{mat4} a the matrix to scale</param>
        /// <param name="v">{vec3} v the vec3 to scale the matrix by</param>
        /// <returns>{mat4} out</returns>
        public static float[] Scale(float[] output, float[] a, float[] v)
        {
            float x = v[0]; float y = v[1]; float z = v[2];

            output[0] = a[0] * x;
            output[1] = a[1] * x;
            output[2] = a[2] * x;
            output[3] = a[3] * x;
            output[4] = a[4] * y;
            output[5] = a[5] * y;
            output[6] = a[6] * y;
            output[7] = a[7] * y;
            output[8] = a[8] * z;
            output[9] = a[9] * z;
            output[10] = a[10] * z;
            output[11] = a[11] * z;
            output[12] = a[12];
            output[13] = a[13];
            output[14] = a[14];
            output[15] = a[15];
            return output;
        }

        public static void SimpleScaleMatrix(Span<float> matrix, float x, float y, float z)
        {
            matrix.Clear();
            matrix[0] = x;
            matrix[5] = y;
            matrix[10] = z;
            matrix[15] = 1f;
        }


        /// <summary>
        /// Scales the mat4 by the dimensions in the given vec3
        /// </summary>
        /// <param name="output">{mat4} out the receiving matrix</param>
        /// <param name="a">{mat4} a the matrix to scale</param>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        /// <param name="zScale"></param>
        /// <returns>{mat4} out</returns>
        public static float[] Scale(float[] output, float[] a, float xScale, float yScale, float zScale)
        {
            output[0] = a[0] * xScale;
            output[1] = a[1] * xScale;
            output[2] = a[2] * xScale;
            output[3] = a[3] * xScale;
            output[4] = a[4] * yScale;
            output[5] = a[5] * yScale;
            output[6] = a[6] * yScale;
            output[7] = a[7] * yScale;
            output[8] = a[8] * zScale;
            output[9] = a[9] * zScale;
            output[10] = a[10] * zScale;
            output[11] = a[11] * zScale;
            output[12] = a[12];
            output[13] = a[13];
            output[14] = a[14];
            output[15] = a[15];
            return output;
        }

        /// <summary>
        /// Rotates a mat4 by the given angle
        /// </summary>
        /// <param name="output">{mat4} out the receiving matrix</param>
        /// <param name="a">{mat4} a the matrix to rotate</param>
        /// <param name="rad">{Number} rad the angle to rotate the matrix by</param>
        /// <param name="axis">{vec3} axis the axis to rotate around</param>
        /// <returns>{mat4} out</returns>
        public static float[] Rotate(float[] output, float[] a, float rad, float[] axis)
        {
            float x = axis[0]; float y = axis[1]; float z = axis[2];
            float len = GameMath.Sqrt(x * x + y * y + z * z);
            float s; float c; float t;
            float a00; float a01; float a02; float a03;
            float a10; float a11; float a12; float a13;
            float a20; float a21; float a22; float a23;
            float b00; float b01; float b02;
            float b10; float b11; float b12;
            float b20; float b21; float b22;

            if (GlMatrixMathf.Abs(len) < GlMatrixMathf.GLMAT_EPSILON()) { return null; }

            len = 1 / len;
            x *= len;
            y *= len;
            z *= len;

            s = GameMath.Sin(rad);
            c = GameMath.Cos(rad);
            t = 1 - c;

            a00 = a[0]; a01 = a[1]; a02 = a[2]; a03 = a[3];
            a10 = a[4]; a11 = a[5]; a12 = a[6]; a13 = a[7];
            a20 = a[8]; a21 = a[9]; a22 = a[10]; a23 = a[11];

            // Construct the elements of the rotation matrix
            b00 = x * x * t + c; b01 = y * x * t + z * s; b02 = z * x * t - y * s;
            b10 = x * y * t - z * s; b11 = y * y * t + c; b12 = z * y * t + x * s;
            b20 = x * z * t + y * s; b21 = y * z * t - x * s; b22 = z * z * t + c;

            // Perform rotation-specific matrix multiplication
            output[0] = a00 * b00 + a10 * b01 + a20 * b02;
            output[1] = a01 * b00 + a11 * b01 + a21 * b02;
            output[2] = a02 * b00 + a12 * b01 + a22 * b02;
            output[3] = a03 * b00 + a13 * b01 + a23 * b02;
            output[4] = a00 * b10 + a10 * b11 + a20 * b12;
            output[5] = a01 * b10 + a11 * b11 + a21 * b12;
            output[6] = a02 * b10 + a12 * b11 + a22 * b12;
            output[7] = a03 * b10 + a13 * b11 + a23 * b12;
            output[8] = a00 * b20 + a10 * b21 + a20 * b22;
            output[9] = a01 * b20 + a11 * b21 + a21 * b22;
            output[10] = a02 * b20 + a12 * b21 + a22 * b22;
            output[11] = a03 * b20 + a13 * b21 + a23 * b22;

            if (a != output)
            {
                // If the source and destination differ, copy the unchanged last row
                output[12] = a[12];
                output[13] = a[13];
                output[14] = a[14];
                output[15] = a[15];
            }
            return output;
        }

        /// <summary>
        /// Rotates a matrix by the given angle around the X axis
        /// </summary>
        /// <param name="output">{mat4} out the receiving matrix</param>
        /// <param name="a">{mat4} a the matrix to rotate</param>
        /// <param name="rad">{Number} rad the angle to rotate the matrix by</param>
        /// <returns>{mat4} out</returns>
        public static float[] RotateX(float[] output, float[] a, float rad)
        {
            float s = GameMath.Sin(rad);
            float c = GameMath.Cos(rad);
            float a10 = a[4];
            float a11 = a[5];
            float a12 = a[6];
            float a13 = a[7];
            float a20 = a[8];
            float a21 = a[9];
            float a22 = a[10];
            float a23 = a[11];

            if (a != output)
            {
                // If the source and destination differ, copy the unchanged rows
                output[0] = a[0];
                output[1] = a[1];
                output[2] = a[2];
                output[3] = a[3];
                output[12] = a[12];
                output[13] = a[13];
                output[14] = a[14];
                output[15] = a[15];
            }

            // Perform axis-specific matrix multiplication
            output[4] = a10 * c + a20 * s;
            output[5] = a11 * c + a21 * s;
            output[6] = a12 * c + a22 * s;
            output[7] = a13 * c + a23 * s;
            output[8] = a20 * c - a10 * s;
            output[9] = a21 * c - a11 * s;
            output[10] = a22 * c - a12 * s;
            output[11] = a23 * c - a13 * s;
            return output;
        }

        /// <summary>
        /// Rotates a matrix by the given angle around the Y axis
        /// </summary>
        /// <param name="output">{mat4} out the receiving matrix</param>
        /// <param name="a">{mat4} a the matrix to rotate</param>
        /// <param name="rad">{Number} rad the angle to rotate the matrix by</param>
        /// <returns>{mat4} out</returns>
        public static float[] RotateY(float[] output, float[] a, float rad)
        {
            float s = GameMath.Sin(rad);
            float c = GameMath.Cos(rad);
            float a00 = a[0];
            float a01 = a[1];
            float a02 = a[2];
            float a03 = a[3];
            float a20 = a[8];
            float a21 = a[9];
            float a22 = a[10];
            float a23 = a[11];

            if (a != output)
            {
                // If the source and destination differ, copy the unchanged rows
                output[4] = a[4];
                output[5] = a[5];
                output[6] = a[6];
                output[7] = a[7];
                output[12] = a[12];
                output[13] = a[13];
                output[14] = a[14];
                output[15] = a[15];
            }

            // Perform axis-specific matrix multiplication
            output[0] = a00 * c - a20 * s;
            output[1] = a01 * c - a21 * s;
            output[2] = a02 * c - a22 * s;
            output[3] = a03 * c - a23 * s;
            output[8] = a00 * s + a20 * c;
            output[9] = a01 * s + a21 * c;
            output[10] = a02 * s + a22 * c;
            output[11] = a03 * s + a23 * c;
            return output;
        }

        /// <summary>
        /// Rotates a matrix by the given angle around the Z axis
        /// </summary>
        /// <param name="output">{mat4} out the receiving matrix</param>
        /// <param name="a">{mat4} a the matrix to rotate</param>
        /// <param name="rad">{Number} rad the angle to rotate the matrix by</param>
        /// <returns>{mat4} out</returns>
        public static float[] RotateZ(float[] output, float[] a, float rad)
        {
            float s = GameMath.Sin(rad);
            float c = GameMath.Cos(rad);
            float a00 = a[0];
            float a01 = a[1];
            float a02 = a[2];
            float a03 = a[3];
            float a10 = a[4];
            float a11 = a[5];
            float a12 = a[6];
            float a13 = a[7];

            if (a != output)
            {
                // If the source and destination differ, copy the unchanged last row
                output[8] = a[8];
                output[9] = a[9];
                output[10] = a[10];
                output[11] = a[11];
                output[12] = a[12];
                output[13] = a[13];
                output[14] = a[14];
                output[15] = a[15];
            }

            // Perform axis-specific matrix multiplication
            output[0] = a00 * c + a10 * s;
            output[1] = a01 * c + a11 * s;
            output[2] = a02 * c + a12 * s;
            output[3] = a03 * c + a13 * s;
            output[4] = a10 * c - a00 * s;
            output[5] = a11 * c - a01 * s;
            output[6] = a12 * c - a02 * s;
            output[7] = a13 * c - a03 * s;
            return output;
        }


        /// <summary>
        /// Provides a composite rotation matrix, equivalent to RotateX followed by RotateY followed by RotateZ
        /// <br/>Here we work on a Span (which may be on the stack) for higher performance
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="radX"></param>
        /// <param name="radY"></param>
        /// <param name="radZ"></param>
        public static void RotateXYZ(Span<float> matrix, float radX, float radY, float radZ)
        {
            float sx = GameMath.Sin(radX);
            float cx = GameMath.Cos(radX);
            float sy = GameMath.Sin(radY);
            float cy = GameMath.Cos(radY);
            float sz = GameMath.Sin(radZ);
            float cz = GameMath.Cos(radZ);
            float a01 = sx * sy;
            float a02 = - cx * sy;
            matrix[0] = cy * cz;
            matrix[1] = a01 * cz + cx * sz;
            matrix[2] = a02 * cz + sx * sz;
            matrix[3] = 0f;
            matrix[4] = -cy * sz;
            matrix[5] = cx * cz - a01 * sz;
            matrix[6] = sx * cz - a02 * sz;
            matrix[7] = 0f;
            matrix[8] = sy;
            matrix[9] = - sx * cy;
            matrix[10] = cx * cy;
            matrix[11] = 0f;
            matrix[12] = 0f;
            matrix[13] = 0f;
            matrix[14] = 0f;
            matrix[15] = 1f;
        }

        /// <summary>
        /// Creates a matrix from a quaternion rotation and vector translation
        /// This is equivalent to (but much faster than):
        ///     mat4.identity(dest);
        ///     mat4.translate(dest, vec);
        ///     var quatMat = mat4.create();
        ///     quat4.toMat4(quat, quatMat);
        ///     mat4.multiply(dest, quatMat);
        /// </summary>
        /// <param name="output">{mat4} out mat4 receiving operation result</param>
        /// <param name="q">{quat4} q Rotation quaternion</param>
        /// <param name="v">{vec3} v Translation vector</param>
        /// <returns>{mat4} out</returns>
        public static float[] FromRotationTranslation(float[] output, float[] q, float[] v)
        {
            // Quaternion math
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

            output[0] = 1 - (yy + zz);
            output[1] = xy + wz;
            output[2] = xz - wy;
            output[3] = 0;
            output[4] = xy - wz;
            output[5] = 1 - (xx + zz);
            output[6] = yz + wx;
            output[7] = 0;
            output[8] = xz + wy;
            output[9] = yz - wx;
            output[10] = 1 - (xx + yy);
            output[11] = 0;
            output[12] = v[0];
            output[13] = v[1];
            output[14] = v[2];
            output[15] = 1;

            return output;
        }

        /// <summary>
        /// Calculates a 4x4 matrix from the given quaternion
        /// </summary>
        /// <param name="output">{mat4} out mat4 receiving operation result</param>
        /// <param name="q">{quat} q Quaternion to create matrix from</param>
        /// <returns>{mat4} out</returns>
        public static float[] FromQuat(float[] output, float[] q)
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

            output[0] = 1 - (yy + zz);
            output[1] = xy + wz;
            output[2] = xz - wy;
            output[3] = 0;

            output[4] = xy - wz;
            output[5] = 1 - (xx + zz);
            output[6] = yz + wx;
            output[7] = 0;

            output[8] = xz + wy;
            output[9] = yz - wx;
            output[10] = 1 - (xx + yy);
            output[11] = 0;

            output[12] = 0;
            output[13] = 0;
            output[14] = 0;
            output[15] = 1;

            return output;
        }

        /// <summary>
        /// Generates a frustum matrix with the given bounds
        /// </summary>
        /// <param name="output">{mat4} out mat4 frustum matrix will be written into</param>
        /// <param name="left">{Number} left Left bound of the frustum</param>
        /// <param name="right">{Number} right Right bound of the frustum</param>
        /// <param name="bottom">{Number} bottom Bottom bound of the frustum</param>
        /// <param name="top">{Number} top Top bound of the frustum</param>
        /// <param name="near">{Number} near Near bound of the frustum</param>
        /// <param name="far">{Number} far Far bound of the frustum</param>
        /// <returns>{mat4} out</returns>
        public static float[] Frustum(float[] output, float left, float right, float bottom, float top, float near, float far)
        {
            float rl = 1 / (right - left);
            float tb = 1 / (top - bottom);
            float nf = 1 / (near - far);
            output[0] = (near * 2) * rl;
            output[1] = 0;
            output[2] = 0;
            output[3] = 0;
            output[4] = 0;
            output[5] = (near * 2) * tb;
            output[6] = 0;
            output[7] = 0;
            output[8] = (right + left) * rl;
            output[9] = (top + bottom) * tb;
            output[10] = (far + near) * nf;
            output[11] = -1;
            output[12] = 0;
            output[13] = 0;
            output[14] = (far * near * 2) * nf;
            output[15] = 0;
            return output;
        }

        /// <summary>
        /// Generates a perspective projection matrix with the given bounds
        /// </summary>
        /// <param name="output">{mat4} out mat4 frustum matrix will be written into</param>
        /// <param name="fovy">{number} fovy Vertical field of view in radians</param>
        /// <param name="aspect">{number} aspect Aspect ratio. typically viewport width/height</param>
        /// <param name="near">{number} near Near bound of the frustum</param>
        /// <param name="far">{number} far Far bound of the frustum</param>
        /// <returns>{mat4} out</returns>
        public static float[] Perspective(float[] output, float fovy, float aspect, float near, float far)
        {
            float one = 1;
            float f = one / GameMath.Tan(fovy / 2);
            float nf = 1 / (near - far);
            output[0] = f / aspect;
            output[1] = 0;
            output[2] = 0;
            output[3] = 0;
            output[4] = 0;
            output[5] = f;
            output[6] = 0;
            output[7] = 0;
            output[8] = 0;
            output[9] = 0;
            output[10] = (far + near) * nf;
            output[11] = -1;
            output[12] = 0;
            output[13] = 0;
            output[14] = (2 * far * near) * nf;
            output[15] = 0;
            return output;
        }

        /// <summary>
        /// Generates a orthogonal projection matrix with the given bounds
        /// </summary>
        /// <param name="output">{mat4} out mat4 frustum matrix will be written into</param>
        /// <param name="left">{number} left Left bound of the frustum</param>
        /// <param name="right">{number} right Right bound of the frustum</param>
        /// <param name="bottom">{number} bottom Bottom bound of the frustum</param>
        /// <param name="top">{number} top Top bound of the frustum</param>
        /// <param name="near">{number} near Near bound of the frustum</param>
        /// <param name="far">{number} far Far bound of the frustum</param>
        /// <returns>{mat4} out</returns>
        public static float[] Ortho(float[] output, float left, float right, float bottom, float top, float near, float far)
        {
            float lr = 1 / (left - right);
            float bt = 1 / (bottom - top);
            float nf = 1 / (near - far);
            output[0] = -2 * lr;
            output[1] = 0;
            output[2] = 0;
            output[3] = 0;
            output[4] = 0;
            output[5] = -2 * bt;
            output[6] = 0;
            output[7] = 0;
            output[8] = 0;
            output[9] = 0;
            output[10] = 2 * nf;
            output[11] = 0;
            output[12] = (left + right) * lr;
            output[13] = (top + bottom) * bt;
            output[14] = (far + near) * nf;
            output[15] = 1;
            return output;
        }

        /// <summary>
        /// Generates a look-at matrix with the given eye position, focal point, and up axis
        /// </summary>
        /// <param name="output">{mat4} out mat4 frustum matrix will be written into</param>
        /// <param name="eye">{vec3} eye Position of the viewer</param>
        /// <param name="center">{vec3} center Point the viewer is looking at</param>
        /// <param name="up">{vec3} up vec3 pointing up</param>
        /// <returns>{mat4} out</returns>
        public static float[] LookAt(float[] output, float[] eye, float[] center, float[] up)
        {
            float x0; float x1; float x2; float y0; float y1; float y2; float z0; float z1; float z2; float len;
            float eyex = eye[0];
            float eyey = eye[1];
            float eyez = eye[2];
            float upx = up[0];
            float upy = up[1];
            float upz = up[2];
            float centerx = center[0];
            float centery = center[1];
            float centerz = center[2];

            if (GlMatrixMathf.Abs(eyex - centerx) < GlMatrixMathf.GLMAT_EPSILON() &&
                GlMatrixMathf.Abs(eyey - centery) < GlMatrixMathf.GLMAT_EPSILON() &&
                GlMatrixMathf.Abs(eyez - centerz) < GlMatrixMathf.GLMAT_EPSILON())
            {
                return Mat4f.Identity(output);
            }

            z0 = eyex - centerx;
            z1 = eyey - centery;
            z2 = eyez - centerz;

            len = 1 / GameMath.Sqrt(z0 * z0 + z1 * z1 + z2 * z2);
            z0 *= len;
            z1 *= len;
            z2 *= len;

            x0 = upy * z2 - upz * z1;
            x1 = upz * z0 - upx * z2;
            x2 = upx * z1 - upy * z0;
            len = GameMath.Sqrt(x0 * x0 + x1 * x1 + x2 * x2);
            if (len == 0)
            {
                x0 = 0;
                x1 = 0;
                x2 = 0;
            }
            else
            {
                len = 1 / len;
                x0 *= len;
                x1 *= len;
                x2 *= len;
            }

            y0 = z1 * x2 - z2 * x1;
            y1 = z2 * x0 - z0 * x2;
            y2 = z0 * x1 - z1 * x0;

            len = GameMath.Sqrt(y0 * y0 + y1 * y1 + y2 * y2);
            if (len == 0)
            {
                y0 = 0;
                y1 = 0;
                y2 = 0;
            }
            else
            {
                len = 1 / len;
                y0 *= len;
                y1 *= len;
                y2 *= len;
            }

            output[0] = x0;
            output[1] = y0;
            output[2] = z0;
            output[3] = 0;
            output[4] = x1;
            output[5] = y1;
            output[6] = z1;
            output[7] = 0;
            output[8] = x2;
            output[9] = y2;
            output[10] = z2;
            output[11] = 0;
            output[12] = -(x0 * eyex + x1 * eyey + x2 * eyez);
            output[13] = -(y0 * eyex + y1 * eyey + y2 * eyez);
            output[14] = -(z0 * eyex + z1 * eyey + z2 * eyez);
            output[15] = 1;

            return output;
        }

        /// <summary>
        /// Multiply the matrix with a vec4. Reference: http://mathinsight.org/matrix_vector_multiplication
        /// Returns a new vec4 vector
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="vec4"></param>
        /// <returns></returns>
        public static float[] MulWithVec4(float[] matrix, float[] vec4)
        {
            float[] output = new float[] { 0, 0, 0, 0 };

            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    output[row] += matrix[4 * col + row] * vec4[col];
                }
            }

            return output;
        }


        public static float[] MulWithVec4(float[] matrix, float v1, float v2, float v3, float v4)
        {
            float[] output = new float[] { 0, 0, 0, 0 };

            for (int row = 0; row < 4; row++)
            {
                output[row] += matrix[4 * 0 + row] * v1;
                output[row] += matrix[4 * 1 + row] * v2;
                output[row] += matrix[4 * 2 + row] * v3;
                output[row] += matrix[4 * 3 + row] * v4;
            }

            return output;
        }


        public static void MulWithVec4(float[] matrix, float[] vec, float[] output)
        {
            MulWithVec4((Span<float>)matrix, vec, output);
        }

        public static void MulWithVec4(Span<float> matrix, float[] vec, float[] output)
        {
            float vx = vec[0];
            float vy = vec[1];
            float vz = vec[2];
            float va = vec[3];
            output[0] = matrix[0] * vx + matrix[4] * vy + matrix[8] * vz + matrix[12] * va;
            output[1] = matrix[1] * vx + matrix[5] * vy + matrix[9] * vz + matrix[13] * va;
            output[2] = matrix[2] * vx + matrix[6] * vy + matrix[10] * vz + matrix[14] * va;
            output[3] = matrix[3] * vx + matrix[7] * vy + matrix[11] * vz + matrix[15] * va;
        }


        /// <summary>
        /// Used for vec3 representing a direction or normal - as a vec4 this would have the 4th element set to 0, so that applying a matrix transform with a translation would have *no* effect
        /// </summary>
        public static void MulWithVec3(float[] matrix, float[] vec, float[] output)
        {
            MulWithVec3((Span<float>)matrix, vec, output);
        }

        /// <summary>
        /// Used for vec3 representing a direction or normal - as a vec4 this would have the 4th element set to 0, so that applying a matrix transform with a translation would have *no* effect
        /// </summary>
        public static void MulWithVec3(Span<float> matrix, float[] vec, float[] output)
        {
            float x = vec[0];
            float y = vec[1];
            float z = vec[2];
            output[0] = matrix[0] * x + matrix[4] * y + matrix[8] * z;
            output[1] = matrix[1] * x + matrix[5] * y + matrix[9] * z;
            output[2] = matrix[2] * x + matrix[6] * y + matrix[10] * z;
        }

        /// <summary>
        /// Used for vec3 representing an x,y,z position - as a vec4 this would have the 4th element set to 1, so that applying a matrix transform with a translation would have an effect
        /// The offset is used to index within the original and output arrays - e.g. in MeshData.xyz
        /// </summary>
        public static void MulWithVec3_Position(float[] matrix, float[] vec, float[] output, int offset)
        {
            MulWithVec3_Position((Span<float>)matrix, vec, output, offset);
        }

        public static void MulWithVec3_Position(Span<float> matrix, float[] vec, float[] output, int offset)
        {
            float x = vec[offset + 0];
            float y = vec[offset + 1];
            float z = vec[offset + 2];
            output[offset + 0] = matrix[0] * x + matrix[4] * y + matrix[8] * z + matrix[12];
            output[offset + 1] = matrix[1] * x + matrix[5] * y + matrix[9] * z + matrix[13];
            output[offset + 2] = matrix[2] * x + matrix[6] * y + matrix[10] * z + matrix[14];
        }


        public static void MulWithVec3_Position_AndScale(float[] matrix, float[] vec, float[] output, int offset, float scaleFactor)
        {
            float x = (vec[offset + 0] - 0.5f) * scaleFactor + 0.5f;
            float y = vec[offset + 1] * scaleFactor;
            float z = (vec[offset + 2] - 0.5f) *scaleFactor + 0.5f;
            output[offset + 0] = matrix[0] * x + matrix[4] * y + matrix[8] * z + matrix[12];
            output[offset + 1] = matrix[1] * x + matrix[5] * y + matrix[9] * z + matrix[13];
            output[offset + 2] = matrix[2] * x + matrix[6] * y + matrix[10] * z + matrix[14];
        }


        public static void MulWithVec3_Position_AndScaleXY(float[] matrix, float[] vec, float[] output, int offset, float scaleFactor)
        {
            float x = (vec[offset + 0] - 0.5f) * scaleFactor + 0.5f;
            float y = vec[offset + 1];
            float z = (vec[offset + 2] - 0.5f) * scaleFactor + 0.5f;
            output[offset + 0] = matrix[0] * x + matrix[4] * y + matrix[8] * z + matrix[12];
            output[offset + 1] = matrix[1] * x + matrix[5] * y + matrix[9] * z + matrix[13];
            output[offset + 2] = matrix[2] * x + matrix[6] * y + matrix[10] * z + matrix[14];
        }

        /// <summary>
        /// Used for vec3 representing an x,y,z position - as a vec4 this would have the 4th element set to 1, so that applying a matrix transform with a translation would have an effect
        /// The offset is used to index within the original and output arrays - e.g. in MeshData.xyz
        /// The origin is the origin for the rotation
        /// </summary>
        public static void MulWithVec3_Position_WithOrigin(float[] matrix, float[] vec, float[] output, int offset, Vec3f origin)
        {
            MulWithVec3_Position_WithOrigin((Span<float>)matrix, vec, output, offset, origin);
        }

        public static void MulWithVec3_Position_WithOrigin(Span<float> matrix, float[] vec, float[] output, int offset, Vec3f origin)
        {
            float vx = vec[offset + 0] - origin.X;
            float vy = vec[offset + 1] - origin.Y;
            float vz = vec[offset + 2] - origin.Z;
            output[offset + 0] = origin.X + matrix[0] * vx + matrix[4] * vy + matrix[8] * vz + matrix[12];
            output[offset + 1] = origin.Y + matrix[1] * vx + matrix[5] * vy + matrix[9] * vz + matrix[13];
            output[offset + 2] = origin.Z + matrix[2] * vx + matrix[6] * vy + matrix[10] * vz + matrix[14];
        }


        /// <summary>
        /// Used for vec3 representing an x,y,z position - as a vec4 this would have the 4th element set to 1, so that applying a matrix transform with a translation would have an effect
        /// </summary>
        public static void MulWithVec3_Position(float[] matrix, float x, float y, float z, Vec3f output)
        {
            output.X = matrix[0] * x + matrix[4] * y + matrix[8] * z + matrix[12];
            output.Y = matrix[1] * x + matrix[5] * y + matrix[9] * z + matrix[13];
            output.Z = matrix[2] * x + matrix[6] * y + matrix[10] * z + matrix[14];
        }


        /// <summary>
        /// Used for Vec3f representing a direction or normal - as a vec4 this would have the 4th element set to 0, so that applying a matrix transform with a translation would have *no* effect
        /// </summary>
        public static void MulWithVec3(float[] matrix, Vec3f vec, Vec3f output)
        {
            output.X = matrix[0] * vec.X + matrix[4] * vec.Y + matrix[8] * vec.Z;
            output.Y = matrix[1] * vec.X + matrix[5] * vec.Y + matrix[9] * vec.Z;
            output.Z = matrix[2] * vec.X + matrix[6] * vec.Y + matrix[10] * vec.Z;
        }


        /// <summary>
        /// Used for x,y,z representing a direction or normal - as a vec4 this would have the 4th element set to 0, so that applying a matrix transform with a translation would have *no* effect
        /// </summary>
        public static FastVec3f MulWithVec3(float[] matrix, float x, float y, float z)
        {
            float xOut = matrix[0] * x + matrix[4] * y + matrix[8] * z;
            float yOut = matrix[1] * x + matrix[5] * y + matrix[9] * z;
            float zOut = matrix[2] * x + matrix[6] * y + matrix[10] * z;
            return new FastVec3f(xOut, yOut, zOut);
        }


        public static BlockFacing MulWithVec3_BlockFacing(float[] matrix, Vec3f vec)
        {
            return MulWithVec3_BlockFacing((Span<float>) matrix, vec);
        }

        public static BlockFacing MulWithVec3_BlockFacing(Span<float> matrix, Vec3f vec)
        {
            float x = matrix[0] * vec.X + matrix[4] * vec.Y + matrix[8] * vec.Z;
            float y = matrix[1] * vec.X + matrix[5] * vec.Y + matrix[9] * vec.Z;
            float z = matrix[2] * vec.X + matrix[6] * vec.Y + matrix[10] * vec.Z;
            return BlockFacing.FromVector(x, y, z);
        }


        public static double[] MulWithVec4(float[] matrix, double[] vec4)
        {
            double[] output = new double[] { 0, 0, 0, 0 };

            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    output[row] += matrix[4 * col + row] * vec4[col];
                }
            }

            return output;
        }

        /// <summary>
        /// Multiply the matrix with a vec4. Reference: http://mathinsight.org/matrix_vector_multiplication
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="vec4"></param>
        /// <param name="outVal"></param>
        /// <returns></returns>
        public static void MulWithVec4(float[] matrix, float[] vec4, Vec4f outVal)
        {
            outVal.Set(0, 0, 0, 0);

            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    outVal[row] += matrix[4 * col + row] * vec4[col];
                }
            }
        }



        /// <summary>
        /// Multiply the matrix with a vec4. Reference: http://mathinsight.org/matrix_vector_multiplication
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="inVal"></param>
        /// <param name="outVal"></param>
        public static void MulWithVec4(float[] matrix, Vec4d inVal, Vec4d outVal)
        {
            outVal.Set(0, 0, 0, 0);

            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    outVal[row] += matrix[4 * col + row] * inVal[col];
                }
            }
        }


        /// <summary>
        /// Multiply the matrix with a vec4. Reference: http://mathinsight.org/matrix_vector_multiplication
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="inVal"></param>
        /// <param name="outVal"></param>
        /// <returns></returns>
        public static void MulWithVec4(float[] matrix, Vec4f inVal, Vec4f outVal)
        {
            outVal.Set(0, 0, 0, 0);

            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    outVal[row] += matrix[4 * col + row] * inVal[col];
                }
            }
        }

        public static void ExtractEulerAngles(float[] m, ref float thetaX, ref float thetaY, ref float thetaZ)
        {
            float sinY = m[8];
            if (Math.Abs(sinY) == 1f)
            {
                //    thetaZ = sinY * (float)Math.Atan2(-m[6], m[5]);
                //    thetaY = -sinY * GameMath.PIHALF;
                //    thetaX = 0;
                //}
                //else
                //{
                //    thetaX = (float)Math.Atan2(m[9], m[10]);
                //    thetaY = GameMath.Asin(-sinY);
                //    thetaZ = (float)Math.Atan2(m[4], m[0]);
                thetaX = sinY * (float)Math.Atan2(m[1], m[5]);
                thetaY = sinY * GameMath.PIHALF;
                thetaZ = 0;
            }
            else
            {
                thetaX = (float)Math.Atan2(-m[9], m[10]);
                thetaY = GameMath.Asin(sinY);
                thetaZ = (float)Math.Atan2(-m[4], m[0]);
            }
        }

        class GlMatrixMathf
        {
            public static float Abs(float len)
            {
                if (len < 0)
                {
                    return -len;
                }
                else
                {
                    return len;
                }
            }

            public static float GLMAT_EPSILON()
            {
                float one = 1;
                return one / 1000000;
            }
        }
    }
}
