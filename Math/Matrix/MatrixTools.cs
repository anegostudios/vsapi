using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    public class MatrixToolsd
    {
        public static Vec3d Project(Vec3d pos, double[] projection, double[] view, int viewportWidth, int viewportHeight)
        {
            double[] outmat = new double[16];
            Mat4d.Mul(outmat, projection, view);

            double[] outpos = Mat4d.MulWithVec4(outmat, new double[] { pos.X, pos.Y, pos.Z, 1 });

            return new Vec3d(
                (outpos[0] / outpos[3] + 1) * (viewportWidth / 2),
                (outpos[1] / outpos[3] + 1) * (viewportHeight / 2),
                outpos[2]
            );
        }
        

        public static void MatFollowPlayer(double[] m)
        {
            m[12] = 0;
            m[13] = 0;
            m[14] = 0;
        }

        public static void LoadPlayerFacingMatrix(double[] m)
        {
            // http://stackoverflow.com/a/5487981
            // | d 0 0 T.x |
            // | 0 d 0 T.y |
            // | 0 0 d T.z |
            // | 0 0 0   1 |
            double d = GameMath.Sqrt(m[0] * m[0] + m[1] * m[1] + m[2] * m[2]);

            m[0] = d;
            m[1] = 0;
            m[2] = 0;
            m[3] = 0;

            m[4] = 0;
            m[5] = d;
            m[6] = 0;
            m[7] = 0;

            m[8] = 0;
            m[9] = 0;
            m[10] = d;
            m[11] = 0;

            m[12] = m[12];
            m[13] = m[13];
            m[14] = m[14];
            m[15] = 1;

            Mat4d.RotateX(m, m, GameMath.PI);

           // game.GlLoadMatrix(m);
        }


        

        public static void MatFacePlayer(double[] m)
        {
            // http://stackoverflow.com/a/5487981
            // | d 0 0 T.x |
            // | 0 d 0 T.y |
            // | 0 0 d T.z |
            // | 0 0 0   1 |
            double d = GameMath.Sqrt(m[0] * m[0] + m[1] * m[1] + m[2] * m[2]);

            m[0] = d;
            m[1] = 0;
            m[2] = 0;
            m[3] = 0;

            m[4] = 0;
            m[5] = d;
            m[6] = 0;
            m[7] = 0;

            m[8] = 0;
            m[9] = 0;
            m[10] = d;
            m[11] = 0;

            m[12] = m[12];
            m[13] = m[13];
            m[14] = m[14];
            m[15] = 1;

            Mat4d.RotateX(m, m, GameMath.PI);
        }
    }
}
