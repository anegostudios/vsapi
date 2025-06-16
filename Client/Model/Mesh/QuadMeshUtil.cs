
#nullable disable
namespace Vintagestory.API.Client
{
    /// <summary>
    /// Utility class for simple quad meshes
    /// </summary>
    public class QuadMeshUtil
    {
        static int[] quadVertices = {
            // Front face
            -1, -1,  0,
             1, -1,  0,
             1,  1,  0,
            -1,  1,  0
        };

        static int[] quadTextureCoords = {
            0, 0,
            1, 0,
            1, 1,
            0, 1
        };

        static int[] quadVertexIndices = {
            0, 1, 2,    0, 2, 3
        };

        /// <summary>
        /// Returns a single vertical quad mesh of with vertices going from -1/-1 to 1/1
        /// With UV, without RGBA
        /// </summary>
        /// <returns></returns>
        public static MeshData GetQuad()
        {
            MeshData m = new MeshData();
            float[] xyz = new float[3 * 4];
            for (int i = 0; i < 3 * 4; i++)
            {
                xyz[i] = quadVertices[i];
            }
            m.SetXyz(xyz);
            float[] uv = new float[2 * 4];
            for (int i = 0; i < uv.Length; i++)
            {
                uv[i] = quadTextureCoords[i];
            }
            m.SetUv(uv);
            m.SetVerticesCount(4);
            m.SetIndices(quadVertexIndices);
            m.SetIndicesCount(3 * 2);
            return m;
        }





        /// <summary>
        /// Quad without rgba, with uv
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="dw"></param>
        /// <param name="dh"></param>
        /// <returns></returns>
        public static MeshData GetCustomQuadModelData(float x, float y, float z, float dw, float dh)
        {
            MeshData m = new MeshData(4, 6, false, true, false, false);

            for (int i = 0; i < 4; i++)
            {
                m.AddVertex(
                    x + (quadVertices[i * 3] > 0 ? dw : 0),
                    y + (quadVertices[i * 3 + 1] > 0 ? dh : 0),
                    z,
                    quadTextureCoords[i * 2],
                    quadTextureCoords[i * 2 + 1]
                );
            }

            for (int i = 0; i < 6; i++)
            {
                m.AddIndex(quadVertexIndices[i]);
            }

            return m;
        }


        /// <summary>
        /// Returns a single vertical  quad mesh at given position, size and color
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static MeshData GetCustomQuad(
            float x, float y, float z, float width, float height,
            byte r, byte g, byte b, byte a)
        {
            MeshData m = new MeshData();
            float[] xyz = new float[3 * 4];

            xyz[0] = x;
            xyz[1] = y;
            xyz[2] = z;

            xyz[3] = x + width;
            xyz[4] = y;
            xyz[5] = z;

            xyz[6] = x + width;
            xyz[7] = y + height;
            xyz[8] = z;

            xyz[9] = x;
            xyz[10] = y + height;
            xyz[11] = z;

            m.SetXyz(xyz);

            float[] uv = new float[2 * 4];
            for (int i = 0; i < uv.Length; i+=2)
            {
                uv[i] = quadTextureCoords[i] * width;
                uv[i+1] = quadTextureCoords[i+1] * height;
            }
            m.SetUv(uv);

            byte[] rgba = new byte[4 * 4];
            for (int i = 0; i < 4; i++)
            {
                rgba[i * 4 + 0] = r;
                rgba[i * 4 + 1] = g;
                rgba[i * 4 + 2] = b;
                rgba[i * 4 + 3] = a;
            }

            m.SetRgba(rgba);

            m.SetVerticesCount(4);
            m.SetIndices(quadVertexIndices);
            m.SetIndicesCount(3 * 2);
            return m;
        }



        /// <summary>
        /// Returns a single horziontal quad mesh with given params
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="width"></param>
        /// <param name="length"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static MeshData GetCustomQuadHorizontal(float x, float y, float z, float width, float length, byte r, byte g, byte b, byte a)
        {
            MeshData m = new MeshData();
            float[] xyz = new float[3 * 4];

            xyz[0] = x;
            xyz[1] = y;
            xyz[2] = z;

            xyz[3] = x + width;
            xyz[4] = y;
            xyz[5] = z;

            xyz[6] = x + width;
            xyz[7] = y;
            xyz[8] = z + length;

            xyz[9] = x;
            xyz[10] = y;
            xyz[11] = z + length;

            m.SetXyz(xyz);

            float[] uv = new float[2 * 4];
            for (int i = 0; i < uv.Length; i+=2)
            {
                uv[i] = quadTextureCoords[i] * width;
                uv[i+1] = quadTextureCoords[i+1] * length;
            }
            m.SetUv(uv);

            byte[] rgba = new byte[4 * 4];
            for (int i = 0; i < 4; i++)
            {
                rgba[i * 4 + 0] = r;
                rgba[i * 4 + 1] = g;
                rgba[i * 4 + 2] = b;
                rgba[i * 4 + 3] = a;
            }

            m.SetRgba(rgba);

            m.SetVerticesCount(4);
            m.SetIndices(quadVertexIndices);
            m.SetIndicesCount(3 * 2);
            return m;
        }

        /// <summary>
        /// Returns a custom quad mesh with the given params.
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="u2"></param>
        /// <param name="v2"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dw"></param>
        /// <param name="dh"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static MeshData GetCustomQuadModelData(
            float u, float v, float u2, float v2,
            float dx, float dy, float dw, float dh,
            byte r, byte g, byte b, byte a)
        {
            MeshData m = new MeshData();
            float[] xyz = new float[3 * 4];

            xyz[0] = dx;
            xyz[1] = dy;
            xyz[2] = 0;

            xyz[3] = dx + dw;
            xyz[4] = dy;
            xyz[5] = 0;

            xyz[6] = dx + dw;
            xyz[7] = dy + dh;
            xyz[8] = 0;

            xyz[9] = dx;
            xyz[10] = dy + dh;
            xyz[11] = 0;

            m.SetXyz(xyz);

            float[] uv = new float[2 * 4];

            uv[0] = u;
            uv[1] = v;

            uv[2] = u2;
            uv[3] = v;

            uv[4] = u2;
            uv[5] = v2;

            uv[6] = u;
            uv[7] = v2;

            m.SetUv(uv);

            byte[] rgba = new byte[4 * 4];
            for (int i = 0; i < 4; i++)
            {
                rgba[i * 4 + 0] = r;
                rgba[i * 4 + 1] = g;
                rgba[i * 4 + 2] = b;
                rgba[i * 4 + 3] = a;
            }

            m.SetRgba(rgba);

            m.SetVerticesCount(4);
            m.SetIndices(quadVertexIndices);
            m.SetIndicesCount(3 * 2);
            return m;
        }
    }

}
