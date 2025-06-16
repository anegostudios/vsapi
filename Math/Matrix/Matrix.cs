using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    public class Matrixf
    {
        public float[] Values;

        public double[] ValuesAsDouble {
            get
            {
                double[] values = new double[16];
                for (int i = 0; i < 16; i++) values[i] = Values[i];
                return values;
            }
        }

        public Matrixf()
        {
            Values = Mat4f.Create();
        }

        public Matrixf(float[] values)
        {
            Values = Mat4f.Create();
            Set(values);
        }

        public static Matrixf Create()
        {
            return new Matrixf();
        }

        public Matrixf Identity()
        {
            Mat4f.Identity(Values);
            return this;
        }

        public Matrixf Set(float[] values)
        {
            Values[0] = values[0];
            Values[1] = values[1];
            Values[2] = values[2];
            Values[3] = values[3];
            Values[4] = values[4];
            Values[5] = values[5];
            Values[6] = values[6];
            Values[7] = values[7];
            Values[8] = values[8];
            Values[9] = values[9];
            Values[10] = values[10];
            Values[11] = values[11];
            Values[12] = values[12];
            Values[13] = values[13];
            Values[14] = values[14];
            Values[15] = values[15];
            return this;
        }

        public Matrixf Set(double[] values)
        {
            Values[0] = (float)values[0];
            Values[1] = (float)values[1];
            Values[2] = (float)values[2];
            Values[3] = (float)values[3];
            Values[4] = (float)values[4];
            Values[5] = (float)values[5];
            Values[6] = (float)values[6];
            Values[7] = (float)values[7];
            Values[8] = (float)values[8];
            Values[9] = (float)values[9];
            Values[10] = (float)values[10];
            Values[11] = (float)values[11];
            Values[12] = (float)values[12];
            Values[13] = (float)values[13];
            Values[14] = (float)values[14];
            Values[15] = (float)values[15];
            return this;
        }

        public Matrixf Translate(double x, double y, double z)
        {
            Mat4f.Translate(Values, Values, (float)x, (float)y, (float)z);
            return this;
        }

        public Matrixf Translate(Vec3f vec)
        {
            Mat4f.Translate(Values, Values, vec.X, vec.Y, vec.Z);
            return this;
        }


        public Matrixf Translate(float x, float y, float z)
        {
            Mat4f.Translate(Values, Values, x, y, z);
            return this;
        }

        public Matrixf Scale(float x, float y, float z)
        {
            Mat4f.Scale(Values, Values, x, y, z);
            return this;
        }

        public Matrixf RotateDeg(Vec3f degrees)
        {
            Mat4f.RotateX(Values, Values, degrees.X * GameMath.DEG2RAD);
            Mat4f.RotateY(Values, Values, degrees.Y * GameMath.DEG2RAD);
            Mat4f.RotateZ(Values, Values, degrees.Z * GameMath.DEG2RAD);
            return this;
        }


        public Matrixf Rotate(Vec3f radians)
        {
            Mat4f.RotateX(Values, Values, radians.X);
            Mat4f.RotateY(Values, Values, radians.Y);
            Mat4f.RotateZ(Values, Values, radians.Z);
            return this;
        }

        public Matrixf Rotate(float radX, float radY, float radZ)
        {
            Mat4f.RotateX(Values, Values, radX);
            Mat4f.RotateY(Values, Values, radY);
            Mat4f.RotateZ(Values, Values, radZ);
            return this;
        }

        public Matrixf RotateX(float radX)
        {
            Mat4f.RotateX(Values, Values, radX);
            return this;
        }

        public Matrixf RotateY(float radY)
        {
            Mat4f.RotateY(Values, Values, radY);
            return this;
        }

        public Matrixf RotateZ(float radZ)
        {
            Mat4f.RotateZ(Values, Values, radZ);
            return this;
        }





        public Matrixf RotateXDeg(float degX)
        {
            Mat4f.RotateX(Values, Values, degX * GameMath.DEG2RAD);
            return this;
        }

        public Matrixf RotateYDeg(float degY)
        {
            Mat4f.RotateY(Values, Values, degY * GameMath.DEG2RAD);
            return this;
        }

        public Matrixf RotateZDeg(float degZ)
        {
            Mat4f.RotateZ(Values, Values, degZ * GameMath.DEG2RAD);
            return this;
        }


        /// <summary>
        /// Vectors with w==0 are called vectors and with w==1 are called points
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public Vec4f TransformVector(Vec4f vec)
        {
            Vec4f outval = new Vec4f();
            Mat4f.MulWithVec4(Values, vec, outval);
            return outval;
        }

        /// <summary>
        /// Vectors with w==0 are called vectors and with w==1 are called points
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public Vec4d TransformVector(Vec4d vec)
        {
            Vec4d outval = new Vec4d();
            Mat4f.MulWithVec4(Values, vec, outval);
            return outval;
        }


        public Matrixf Mul(float[] matrix)
        {
            Mat4f.Mul(Values, Values, matrix);
            return this;
        }

        public Matrixf Mul(Matrixf matrix)
        {
            Mat4f.Mul(Values, Values, matrix.Values);
            return this;
        }

        public Matrixf ReverseMul(float[] matrix)
        {
            Mat4f.Mul(Values, matrix, Values);
            return this;
        }

        public Matrixf FollowPlayer()
        {
            Values[12] = 0;
            Values[13] = 0;
            Values[14] = 0;
            return this;
        }

        public Matrixf FollowPlayerXZ()
        {
            Values[12] = 0;
            Values[14] = 0;
            return this;
        }

        public Matrixf Invert()
        {
            Mat4f.Invert(Values, Values);
            return this;
        }

        public Matrixf Clone()
        {
            return new Matrixf()
            {
                Values = (float[])Values.Clone()
            };
        }

    }
}
