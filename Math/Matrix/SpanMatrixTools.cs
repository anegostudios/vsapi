using System;
using System.Runtime.CompilerServices;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    public partial class MatrixToolsd
    {
        public static void Project(Span<double> output, ReadOnlySpan<double> pos,
            ReadOnlySpan<double> projection, ReadOnlySpan<double> view,
            int viewportWidth, int viewportHeight)
        {
            Span<double> outmat = stackalloc double[16];
            Mat4d.Mul(outmat, projection, view);

            ReadOnlySpan<double> pos4 = stackalloc double[4] { pos[0], pos[1], pos[2], 1 };
            Span<double> outpos = stackalloc double[4];
            Mat4d.MulVec4(outpos, outmat, pos4);

            output[0] = (outpos[0] / outpos[3] + 1) * (viewportWidth / 2);
            output[1] = (outpos[1] / outpos[3] + 1) * (viewportHeight / 2);
            output[2] = outpos[2];
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MatFollowPlayer(Span<double> m)
        {
            m[12] = 0;
            m[13] = 0;
            m[14] = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LoadPlayerFacingMatrix(Span<double> m)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void MatFacePlayer(Span<double> m)
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
