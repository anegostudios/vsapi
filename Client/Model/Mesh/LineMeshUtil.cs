using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    //(-1,-1,-1) to (1,1,1)
    public class LineMeshUtil
    {
        public static MeshData GetRectangle(int color = 0)
        {
            MeshData m = new MeshData();
            m.setMode(EnumDrawMode.Lines);
            m.xyz = new float[3 * 4];
            m.Rgba = new byte[4 * 4];
            m.Indices = new int[8];

            AddLineLoop(m,
                new Vec3f(-1, -1, 0),
                new Vec3f(-1, 1, 0),
                new Vec3f(1, 1, 0),
                new Vec3f(1, -1, 0),
                color
            );

            return m;
        }

        public static MeshData GetCube(int color = 0)
        {
            MeshData m = new MeshData();
            m.setMode(EnumDrawMode.Lines);
            m.xyz = new float[3 * 4 * 6];
            m.Rgba = new byte[4 * 4 * 6];
            m.Indices = new int[8 * 6];

            AddLineLoop(m,
                    new Vec3f(-1, -1, -1),
                    new Vec3f(-1, 1, -1),
                    new Vec3f(1, 1, -1),
                    new Vec3f(1, -1, -1),
                    color
                );
            AddLineLoop(m,
                    new Vec3f(-1, -1, -1),
                    new Vec3f(1, -1, -1),
                    new Vec3f(1, -1, 1),
                    new Vec3f(-1, -1, 1),
                    color
                );
            AddLineLoop(m,
                    new Vec3f(-1, -1, -1),
                    new Vec3f(-1, -1, 1),
                    new Vec3f(-1, 1, 1),
                    new Vec3f(-1, 1, -1),
                    color
                );
            AddLineLoop(m,
                    new Vec3f(-1, -1, 1),
                    new Vec3f(1, -1, 1),
                    new Vec3f(1, 1, 1),
                    new Vec3f(-1, 1, 1),
                    color
                );
            AddLineLoop(m,
                    new Vec3f(-1, 1, -1),
                    new Vec3f(-1, 1, 1),
                    new Vec3f(1, 1, 1),
                    new Vec3f(1, 1, -1),
                    color
                );
            AddLineLoop(m,
                    new Vec3f(1, -1, -1),
                    new Vec3f(1, 1, -1),
                    new Vec3f(1, 1, 1),
                    new Vec3f(1, -1, 1),
                    color
                );

            return m;
        }


        public static void AddLine2D(MeshData m, float x1, float y1, float x2, float y2, int color)
        {
            int startVertex = m.GetVerticesCount();

            AddVertex(m, x1, y1, 50, color);
            AddVertex(m, x2, y2, 50, color);

            m.Indices[m.IndicesCount++] = startVertex + 0;
            m.Indices[m.IndicesCount++] = startVertex + 1;
        }

        public static void AddLineLoop(MeshData m, Vec3f p0, Vec3f p1, Vec3f p2, Vec3f p3, int color)
        {
            int startVertex = m.GetVerticesCount();
            AddVertex(m, p0.X, p0.Y, p0.Z, color);
            AddVertex(m, p1.X, p1.Y, p1.Z, color);
            AddVertex(m, p2.X, p2.Y, p2.Z, color);
            AddVertex(m, p3.X, p3.Y, p3.Z, color);

            m.Indices[m.IndicesCount++] = startVertex + 0;
            m.Indices[m.IndicesCount++] = startVertex + 1;
            m.Indices[m.IndicesCount++] = startVertex + 1;
            m.Indices[m.IndicesCount++] = startVertex + 2;
            m.Indices[m.IndicesCount++] = startVertex + 2;
            m.Indices[m.IndicesCount++] = startVertex + 3;
            m.Indices[m.IndicesCount++] = startVertex + 3;
            m.Indices[m.IndicesCount++] = startVertex + 0;
        }

        public static void AddVertex(MeshData model, float x, float y, float z, int color)
        {
            model.xyz[model.XyzCount + 0] = x;
            model.xyz[model.XyzCount + 1] = y;
            model.xyz[model.XyzCount + 2] = z;
            model.Rgba[model.RgbaCount + 0] = (byte)(ColorUtil.ColorR(color));
            model.Rgba[model.RgbaCount + 1] = (byte)(ColorUtil.ColorG(color));
            model.Rgba[model.RgbaCount + 2] = (byte)(ColorUtil.ColorB(color));
            model.Rgba[model.RgbaCount + 3] = (byte)(ColorUtil.ColorA(color));
            model.VerticesCount++;
        }
    }

}
