
#nullable disable
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
    /// <summary>
    /// 2x2 Matrix
    /// </summary>
    public class Mat22
    {
        /// <summary>
        /// Creates a new identity mat2
        /// Returns a new 2x2 matrix
        /// </summary>
        /// <returns></returns>
        public static float[] Create()
        {
            float[] output = new float[4];
            output[0] = 1;
            output[1] = 0;
            output[2] = 0;
            output[3] = 1;
            return output;
        }

        /// <summary>
        /// Creates a new mat2 initialized with values from an existing matrix
        /// Returns a new 2x2 matrix
        /// </summary>
        /// <param name="a">matrix to clone</param>
        /// <returns></returns>
        public static float[] CloneIt(float[] a)
        {
            float[] output = new float[4];
            output[0] = a[0];
            output[1] = a[1];
            output[2] = a[2];
            output[3] = a[3];
            return output;
        }

        /// <summary>
        /// Copy the values from one mat2 to another
        /// Returns output
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the source matrix</param>
        /// <returns></returns>
        public static float[] Copy(float[] output, float[] a)
        {
            output[0] = a[0];
            output[1] = a[1];
            output[2] = a[2];
            output[3] = a[3];
            return output;
        }

        /// <summary>
        /// Set a mat2 to the identity matrix
        /// Returns output
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <returns></returns>
        public static float[] Identity_(float[] output)
        {
            output[0] = 1;
            output[1] = 0;
            output[2] = 0;
            output[3] = 1;
            return output;
        }

        /// <summary>
        /// Transpose the values of a mat2
        /// Returns output
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the source matrix</param>
        /// <returns></returns>
        public static float[] Transpose(float[] output, float[] a)
        {
            output[0] = a[0];
            output[1] = a[2];
            output[2] = a[1];
            output[3] = a[3];

            return output;
        }

        /// <summary>
        /// Inverts a mat2
        /// Returns output
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the source matrix</param>
        /// <returns></returns>
        public static float[] Invert(float[] output, float[] a)
        {
            float a0 = a[0]; float a1 = a[1]; float a2 = a[2]; float a3 = a[3];

            // Calculate the determinant
            float det = a0 * a3 - a2 * a1;

            if (det == 0)
            {
                return null;
            }
            float one = 1;
            det = one / det;

            output[0] = a3 * det;
            output[1] = -a1 * det;
            output[2] = -a2 * det;
            output[3] = a0 * det;

            return output;
        }

        /// <summary>
        /// Calculates the adjugate of a mat2
        /// Returns output
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the source matrix</param>
        /// <returns></returns>
        public static float[] Adjoint(float[] output, float[] a)
        {
            // Caching this value is nessecary if output == a
            float a0 = a[0];
            output[0] = a[3];
            output[1] = -a[1];
            output[2] = -a[2];
            output[3] = a0;

            return output;
        }

        /// <summary>
        /// Calculates the determinant of a mat2
        /// Returns determinant of a
        /// </summary>
        /// <param name="a">the source matrix</param>
        /// <returns></returns>
        public static float Determinant(float[] a)
        {
            return a[0] * a[3] - a[2] * a[1];
        }

        /// <summary>
        /// Multiplies two mat2's
        /// Returns output
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the first operand</param>
        /// <param name="b">the second operand</param>
        /// <returns></returns>
        public static float[] Multiply(float[] output, float[] a, float[] b)
        {
            float a0 = a[0]; float a1 = a[1]; float a2 = a[2]; float a3 = a[3];
            float b0 = b[0]; float b1 = b[1]; float b2 = b[2]; float b3 = b[3];
            output[0] = a0 * b0 + a1 * b2;
            output[1] = a0 * b1 + a1 * b3;
            output[2] = a2 * b0 + a3 * b2;
            output[3] = a2 * b1 + a3 * b3;
            return output;
        }

        /// <summary>
        /// Alias for {@link mat2.multiply}
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
        /// Rotates a mat2 by the given angle
        /// Returns output
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the matrix to rotate</param>
        /// <param name="rad">the angle to rotate the matrix by</param>
        /// <returns></returns>
        public static float[] Rotate(float[] output, float[] a, float rad)
        {
            float a0 = a[0]; float a1 = a[1]; float a2 = a[2]; float a3 = a[3];
            float s = GameMath.Sin(rad);
            float c = GameMath.Cos(rad);
            output[0] = a0 * c + a1 * s;
            output[1] = a0 * -s + a1 * c;
            output[2] = a2 * c + a3 * s;
            output[3] = a2 * -s + a3 * c;
            return output;
        }

        /// <summary>
        /// Scales the mat2 by the dimensions in the given vec2
        /// Returns output
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the matrix to rotate</param>
        /// <param name="v">the vec2 to scale the matrix by</param>
        /// <returns></returns>
        public static float[] Scale(float[] output, float[] a, float[] v)
        {
            float a0 = a[0]; float a1 = a[1]; float a2 = a[2]; float a3 = a[3];
            float v0 = v[0]; float v1 = v[1];
            output[0] = a0 * v0;
            output[1] = a1 * v1;
            output[2] = a2 * v0;
            output[3] = a3 * v1;
            return output;
        }


    }
}
