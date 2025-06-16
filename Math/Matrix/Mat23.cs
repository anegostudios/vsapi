
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
    /// 2x3 Matrix
    /// * A mat2d contains six elements defined as:
    /// * <pre>
    /// * [a, b,
    /// *  c, d,
    /// *  tx,ty]
    /// * </pre>
    /// * This is a short form for the 3x3 matrix:
    /// * <pre>
    /// * [a, b, 0
    /// *  c, d, 0
    /// *  tx,ty,1]
    /// * </pre>
    /// * The last column is ignored so the array is shorter and operations are faster.
    /// </summary>
    public class Mat23
    {
        /// <summary>
        /// Creates a new identity mat2d
        /// Returns a new 2x3 matrix
        /// </summary>
        /// <returns></returns>
        public static float[] Create()
        {
            float[] output = new float[6];
            output[0] = 1;
            output[1] = 0;
            output[2] = 0;
            output[3] = 1;
            output[4] = 0;
            output[5] = 0;
            return output;
        }

        /// <summary>
        /// Creates a new mat2d initialized with values from an existing matrix
        /// Returns a new 2x3 matrix
        /// </summary>
        /// <param name="a">matrix to clone</param>
        /// <returns></returns>
        public static float[] CloneIt(float[] a)
        {
            float[] output = new float[6];
            output[0] = a[0];
            output[1] = a[1];
            output[2] = a[2];
            output[3] = a[3];
            output[4] = a[4];
            output[5] = a[5];
            return output;
        }

        /// <summary>
        /// Copy the values from one mat2d to another
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
            output[4] = a[4];
            output[5] = a[5];
            return output;
        }

        /// <summary>
        /// Set a mat2d to the identity matrix
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
            output[4] = 0;
            output[5] = 0;
            return output;
        }

        /// <summary>
        /// Inverts a mat2d
        /// Returns output
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the source matrix</param>
        /// <returns></returns>
        public static float[] Invert(float[] output, float[] a)
        {
            float aa = a[0]; float ab = a[1]; float ac = a[2]; float ad = a[3];
            float atx = a[4]; float aty = a[5];

            float det = aa * ad - ab * ac;
            if (det == 0)
            {
                return null;
            }
            float one = 1;
            det = one / det;

            output[0] = ad * det;
            output[1] = -ab * det;
            output[2] = -ac * det;
            output[3] = aa * det;
            output[4] = (ac * aty - ad * atx) * det;
            output[5] = (ab * atx - aa * aty) * det;
            return output;
        }

        /// <summary>
        /// Calculates the determinant of a mat2d
        /// Returns determinant of a
        /// </summary>
        /// <param name="a">the source matrix</param>
        /// <returns></returns>
        public static float Determinant(float[] a)
        {
            return a[0] * a[3] - a[1] * a[2];
        }


        /// <summary>
        /// Multiplies two mat2d's
        /// Returns output
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the first operand</param>
        /// <param name="b">the second operand</param>
        /// <returns></returns>
        public static float[] Multiply(float[] output, float[] a, float[] b)
        {
            float aa = a[0]; float ab = a[1]; float ac = a[2]; float ad = a[3];
            float atx = a[4]; float aty = a[5];
            float ba = b[0]; float bb = b[1]; float bc = b[2]; float bd = b[3];
            float btx = b[4]; float bty = b[5];

            output[0] = aa * ba + ab * bc;
            output[1] = aa * bb + ab * bd;
            output[2] = ac * ba + ad * bc;
            output[3] = ac * bb + ad * bd;
            output[4] = ba * atx + bc * aty + btx;
            output[5] = bb * atx + bd * aty + bty;
            return output;
        }

        /// <summary>
        /// Alias for {@link mat2d.multiply} @function
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
        /// Rotates a mat2d by the given angle
        /// Returns output
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the matrix to rotate</param>
        /// <param name="rad">the angle to rotate the matrix by</param>
        /// <returns></returns>
        public static float[] Rotate(float[] output, float[] a, float rad)
        {
            float aa = a[0];
            float ab = a[1];
            float ac = a[2];
            float ad = a[3];
            float atx = a[4];
            float aty = a[5];
            float st = GameMath.Sin(rad);
            float ct = GameMath.Cos(rad);

            output[0] = aa * ct + ab * st;
            output[1] = -aa * st + ab * ct;
            output[2] = ac * ct + ad * st;
            output[3] = -ac * st + ct * ad;
            output[4] = ct * atx + st * aty;
            output[5] = ct * aty - st * atx;
            return output;
        }

        /// <summary>
        /// Scales the mat2d by the dimensions in the given vec2
        /// Returns output
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the matrix to translate</param>
        /// <param name="v">the vec2 to scale the matrix by</param>
        /// <returns></returns>
        public static float[] Scale(float[] output, float[] a, float[] v)
        {
            float vx = v[0]; float vy = v[1];
            output[0] = a[0] * vx;
            output[1] = a[1] * vy;
            output[2] = a[2] * vx;
            output[3] = a[3] * vy;
            output[4] = a[4] * vx;
            output[5] = a[5] * vy;
            return output;
        }

        /// <summary>
        /// Translates the mat2d by the dimensions in the given vec2
        /// Returns output
        /// </summary>
        /// <param name="output">the receiving matrix</param>
        /// <param name="a">the matrix to translate</param>
        /// <param name="v">the vec2 to translate the matrix by</param>
        /// <returns></returns>
        public static float[] Translate(float[] output, float[] a, float[] v)
        {
            output[0] = a[0];
            output[1] = a[1];
            output[2] = a[2];
            output[3] = a[3];
            output[4] = a[4] + v[0];
            output[5] = a[5] + v[1];
            return output;
        }
        
    }
}
