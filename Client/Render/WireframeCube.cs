using Vintagestory.API.Client;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.Client.NoObf
{
    public class WireframeCube
    {
        MeshRef modelRef;
        Matrixf mvMat = new Matrixf();

        /// <summary>
        /// Creates a cube mesh with edge points 0/0/0 and 1/1/1
        /// </summary>
        /// <param name="capi"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static WireframeCube CreateUnitCube(ICoreClientAPI capi, int color = 128 << 24)
        {
            var cube = new WireframeCube();
            MeshData data = LineMeshUtil.GetCube(color);

            data.Scale(new Vec3f(), 0.5f, 0.5f, 0.5f);
            data.Translate(0.5f, 0.5f, 0.5f);

            data.Flags = new int[data.VerticesCount];
            for (int i = 0; i < data.Flags.Length; i++) data.Flags[i] = 1 << 8;

            cube.modelRef = capi.Render.UploadMesh(data);
            return cube;
        }

        /// <summary>
        /// Creates a cube mesh with edge points -1/-1/-1 and 1/1/1
        /// </summary>
        /// <param name="capi"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static WireframeCube CreateCenterOriginCube(ICoreClientAPI capi, int color = 128 << 24)
        {
            var cube = new WireframeCube();
            MeshData data = LineMeshUtil.GetCube(color);
            data.Flags = new int[data.VerticesCount];
            for (int i = 0; i < data.Flags.Length; i++) data.Flags[i] = 1 << 8;

            cube.modelRef = capi.Render.UploadMesh(data);
            return cube;
        }


        public WireframeCube() { }

        public void Render(ICoreClientAPI capi, double posx, double posy, double posz, float scalex, float scaley, float scalez, float lineWidth = 1.6f, Vec4f color = null)
        {
            var eplr = capi.World.Player.Entity;

            mvMat
                .Identity()
                .Set(capi.Render.CameraMatrixOrigin)
                .Translate(posx - eplr.CameraPos.X, posy - eplr.CameraPos.Y, posz - eplr.CameraPos.Z)
                .Scale(scalex, scaley, scalez)
            ;

            Render(capi, mvMat, lineWidth, color);
        }

        public void Render(ICoreClientAPI capi, Matrixf mat, float lineWidth = 1.6f, Vec4f color = null)
        {
            var prog = capi.Shader.GetProgram((int)EnumShaderProgram.Wireframe);
            prog.Use();

            capi.Render.LineWidth = lineWidth;
            //game.Platform.SmoothLines(true);
            capi.Render.GLEnableDepthTest();
            capi.Render.GLDepthMask(false);
            capi.Render.GlToggleBlend(true);


            prog.Uniform("origin", new Vec3f(0, 0, 0));
            prog.UniformMatrix("projectionMatrix", capi.Render.CurrentProjectionMatrix);
            prog.UniformMatrix("modelViewMatrix", mat.Values);
            prog.Uniform("colorIn", color ?? ColorUtil.WhiteArgbVec);

            capi.Render.RenderMesh(modelRef);

            prog.Stop();

            if (lineWidth != 1.6f)
            {
                capi.Render.LineWidth = 1.6f;
            }

            if (RuntimeEnv.OS != OS.Mac)
            {
                capi.Render.GLDepthMask(true);
            }
        }

        public void Dispose()
        {
            modelRef?.Dispose();
        }
    }
}
