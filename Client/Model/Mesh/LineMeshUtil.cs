using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    //(-1,-1,-1) to (1,1,1)
    public class LineMeshUtil
    {
        /// <summary>
        /// Gets the current rectangle for the line.
        /// </summary>
        /// <param name="color">the converted base color.</param>
        /// <returns>The mesh data for the rectangle..</returns>
        public static MeshData GetRectangle(int color = 0)
        {
            MeshData m = new MeshData();
            m.SetMode(EnumDrawMode.Lines);
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

        /// <summary>
        /// Gets the cube of this line.
        /// </summary>
        /// <param name="color">The converted base color.</param>
        /// <returns>The mesh data for the cube.</returns>
        public static MeshData GetCube(int color = 0)
        {
            MeshData m = new MeshData();
            m.SetMode(EnumDrawMode.Lines);
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

        /// <summary>
        /// Adds a 2D line to the mesh data.
        /// </summary>
        /// <param name="m">The current mesh data.</param>
        /// <param name="x1">X position of the first point.</param>
        /// <param name="y1">Y position of the first point.</param>
        /// <param name="x2">X position of the second point.</param>
        /// <param name="y2">Y position of the second point.</param>
        /// <param name="color">The converted base color.</param>
        public static void AddLine2D(MeshData m, float x1, float y1, float x2, float y2, int color)
        {
            int startVertex = m.GetVerticesCount();

            AddVertex(m, x1, y1, 50, color);
            AddVertex(m, x2, y2, 50, color);

            m.Indices[m.IndicesCount++] = startVertex + 0;
            m.Indices[m.IndicesCount++] = startVertex + 1;
        }

        /// <summary>
        /// Adds a collection of lines to the given mesh.
        /// </summary>
        /// <param name="m">The current mesh data.</param>
        /// <param name="p0">The first point.</param>
        /// <param name="p1">The second point.</param>
        /// <param name="p2">The third point.</param>
        /// <param name="p3">The fourth point.</param>
        /// <param name="color">The color of the resulting lines.</param>
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

        /// <summary>
        /// Adds a vertex to the mesh data.
        /// </summary>
        /// <param name="model">The mesh data.</param>
        /// <param name="x">X position of the vertex.</param>
        /// <param name="y">Y position of the vertex.</param>
        /// <param name="z">Z position of the vertex.</param>
        /// <param name="color">The color of the vertex.</param>
        public static void AddVertex(MeshData model, float x, float y, float z, int color)
        {
            model.xyz[model.XyzCount + 0] = x;
            model.xyz[model.XyzCount + 1] = y;
            model.xyz[model.XyzCount + 2] = z;
            model.Rgba[model.RgbaCount + 0] = ColorUtil.ColorR(color);
            model.Rgba[model.RgbaCount + 1] = ColorUtil.ColorG(color);
            model.Rgba[model.RgbaCount + 2] = ColorUtil.ColorB(color);
            model.Rgba[model.RgbaCount + 3] = ColorUtil.ColorA(color);
            model.VerticesCount++;
        }
    }

}
