using System;
using System.Runtime.CompilerServices;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// A data structure that can be used to upload mesh information onto the graphics card
    /// Please note, all arrays are used as a buffer. They do not tightly fit the data but are always sized as a multiple of 2 from the initial size.
    /// </summary>
    public class MeshData
    {
        public int[] TextureIds = new int[0];

        /// <summary>
        /// The x/y/z coordinates buffer. This should hold VerticesCount*3 values.
        /// </summary>
        public float[] xyz;

        /// <summary>
        /// The render flags buffer. This should hold VerticesCount*1 values.
        /// </summary>
        public int[] Flags;

        /// <summary>
        /// True if the flags array contains any wind mode
        /// </summary>
        public bool HasAnyWindModeSet;

        /// <summary>
        /// The normals buffer. This should hold VerticesCount*1 values. Currently unused by the engine.
        /// GL_INT_2_10_10_10_REV Format
        /// x: bits 0-9    (10 bit signed int)
        /// y: bits 10-19  (10 bit signed int)
        /// z: bits 20-29  (10 bit signed int) 
        /// w: bits 30-31
        /// </summary>
        public int[] Normals;

        /// <summary>
        /// The uv buffer for texture coordinates. This should hold VerticesCount*2 values.
        /// </summary>
        public float[] Uv;

        /// <summary>
        /// The vertex color buffer. This should hold VerticesCount*4 values.
        /// </summary>
        public byte[] Rgba;

        /// <summary>
        /// The indices buffer. This should hold IndicesCount values.
        /// </summary>
        public int[] Indices;

        /// <summary>
        /// Texture index per face, references to and index in the TextureIds array
        /// </summary>
        public byte[] TextureIndices;


        /// <summary>
        /// Custom floats buffer. Can be used to upload arbitrary amounts of float values onto the graphics card
        /// </summary>
        public CustomMeshDataPartFloat CustomFloats;

        /// <summary>
        /// Custom ints buffer. Can be used to upload arbitrary amounts of int values onto the graphics card
        /// </summary>
        public CustomMeshDataPartInt CustomInts = null;

        /// <summary>
        /// Custom shorts buffer. Can be used to upload arbitrary amounts of short values onto the graphics card
        /// </summary>
        public CustomMeshDataPartShort CustomShorts = null;

        /// <summary>
        /// Custom bytes buffer. Can be used to upload arbitrary amounts of byte values onto the graphics card
        /// </summary>
        public CustomMeshDataPartByte CustomBytes;

        /// <summary>
        /// When using instanced rendering, set this flag to have the xyz values instanced.
        /// </summary>
        public bool XyzInstanced = false;
        /// <summary>
        /// When using instanced rendering, set this flag to have the uv values instanced.
        /// </summary>
        public bool UvInstanced = false;
        /// <summary>
        /// When using instanced rendering, set this flag to have the rgba values instanced.
        /// </summary>
        public bool RgbaInstanced = false;
        /// <summary>
        /// When using instanced rendering, set this flag to have the rgba2 values instanced.
        /// </summary>
        public bool Rgba2Instanced = false;
        /// <summary>
        /// When using instanced rendering, set this flag to have the indices instanced.
        /// </summary>
        public bool IndicesInstanced = false;
        /// <summary>
        /// When using instanced rendering, set this flag to have the flags instanced.
        /// </summary>
        public bool FlagsInstanced = false;


        /// <summary>
        /// xyz vbo usage hints for the graphics card. Recommended to be set to false when this section of data changes often.
        /// </summary>
        public bool XyzStatic = true;
        /// <summary>
        /// uv vbo usage hints for the graphics card. Recommended to be set to false when this section of data changes often.
        /// </summary>
        public bool UvStatic = true;
        /// <summary>
        /// rgab vbo usage hints for the graphics card. Recommended to be set to false when this section of data changes often.
        /// </summary>
        public bool RgbaStatic = true;
        /// <summary>
        /// rgba2 vbo usage hints for the graphics card. Recommended to be set to false when this section of data changes often.
        /// </summary>
        public bool Rgba2Static = true;
        /// <summary>
        /// indices vbo usage hints for the graphics card. Recommended to be set to false when this section of data changes often.
        /// </summary>
        public bool IndicesStatic = true;
        /// <summary>
        /// flags vbo usage hints for the graphics card. Recommended to be set to false when this section of data changes often.
        /// </summary>
        public bool FlagsStatic = true;


        /// <summary>
        /// For offseting the data in the VBO. This field is used when updating a mesh.
        /// </summary>
        public int XyzOffset = 0;
        /// <summary>
        /// For offseting the data in the VBO. This field is used when updating a mesh.
        /// </summary>
        public int UvOffset = 0;
        /// <summary>
        /// For offseting the data in the VBO. This field is used when updating a mesh.
        /// </summary>
        public int RgbaOffset = 0;
        /// <summary>
        /// For offseting the data in the VBO. This field is used when updating a mesh.
        /// </summary>
        public int Rgba2Offset = 0;
        /// <summary>
        /// For offseting the data in the VBO. This field is used when updating a mesh.
        /// </summary>
        public int FlagsOffset = 0;
        /// <summary>
        /// For offseting the data in the VBO. This field is used when updating a mesh.
        /// </summary>
        public int NormalsOffset = 0;
        /// <summary>
        /// For offseting the data in the VBO. This field is used when updating a mesh.
        /// </summary>
        public int IndicesOffset = 0;


        /// <summary>
        /// The meshes draw mode
        /// </summary>
        public EnumDrawMode mode;

        /// <summary>
        /// Amount of currently assigned normals
        /// </summary>
        public int NormalsCount;

        /// <summary>
        /// Amount of currently assigned vertices
        /// </summary>
        public int VerticesCount;

        /// <summary>
        /// Amount of currently assigned indices
        /// </summary>
        public int IndicesCount;

        /// <summary>
        /// Vertex buffer size
        /// </summary>
        public int VerticesMax;

        /// <summary>
        /// Index buffer size
        /// </summary>
        public int IndicesMax;


        /// <summary>
        /// BlockShapeTesselator xyz faces. Required by TerrainChunkTesselator to determine vertex lightness. Should hold VerticesCount / 4 values. Set to 0 for no face, set to 1..8 for faces 0..7
        /// </summary>
        public byte[] XyzFaces;

        /// <summary>
        /// Amount of assigned xyz face values
        /// </summary>
        public int XyzFacesCount;

        public int TextureIndicesCount;


        public int IndicesPerFace = 6;
        public int VerticesPerFace = 4;

        /// <summary>
        /// BlockShapeTesselator climate colormap ids. Required by TerrainChunkTesselator to determine whether to color a vertex by a color map or not. Should hold VerticesCount / 4 values. Set to 0 for no color mapping, set 1..n for color map 0..n-1
        /// </summary>
        public byte[] ClimateColorMapIds;
        /// <summary>
        /// BlockShapeTesselator season colormap ids. Required by TerrainChunkTesselator to determine whether to color a vertex by a color map or not. Should hold VerticesCount / 4 values. Set to 0 for no color mapping, set 1..n for color map 0..n-1
        /// </summary>
        public byte[] SeasonColorMapIds;


        [Obsolete("Use RenderPassesAndExtraBits instead")]
        public short[] RenderPasses => RenderPassesAndExtraBits;

        /// <summary>
        /// BlockShapeTesselator renderpass. Required by TerrainChunkTesselator to determine in which mesh data pool each quad should land in. Should hold VerticesCount / 4 values.<br/>
        /// Lower 10 bits = render pass<br/>
        /// Upper 6 bits = extra bits for tesselators<br/>
        ///    Bit 10: DisableRandomDrawOffset
        /// </summary>
        public short[] RenderPassesAndExtraBits;

        /// <summary>
        /// Amount of assigned tint values
        /// </summary>
        public int ColorMapIdsCount;

        /// <summary>
        /// Amount of assigned render pass values
        /// </summary>
        public int RenderPassCount;

        /// <summary>
        /// Gets the number of verticies in the the mesh.
        /// </summary>
        /// <returns>The number of verticies in this mesh.</returns>
        /// <remarks>..Shouldn't this be a property?</remarks>
        public int GetVerticesCount() { return VerticesCount; }

        /// <summary>
        /// Sets the number of verticies in this mesh.
        /// </summary>
        /// <param name="value">The number of verticies in this mesh</param>
        /// <remarks>..Shouldn't this be a property?</remarks>
        public void SetVerticesCount(int value) { VerticesCount = value; }

        /// <summary>
        /// Gets the number of Indicices in this mesh.
        /// </summary>
        /// <returns>The number of indicies in the mesh.</returns>
        /// <remarks>..Shouldn't this be a property?</remarks>
        public int GetIndicesCount() { return IndicesCount; }

        /// <summary>
        /// Sets the number of indices in this mesh.
        /// </summary>
        /// <param name="value">The number of indices in this mesh.</param>
        public void SetIndicesCount(int value) { IndicesCount = value; }

        /// <summary>
        /// The size of the position values.
        /// </summary>
        public const int XyzSize = sizeof(float) * 3;

        /// <summary>
        /// The size of the normals.
        /// </summary>
        public const int NormalSize = sizeof(int);

        /// <summary>
        /// The size of the color.
        /// </summary>
        public const int RgbaSize = sizeof(byte) * 4;

        /// <summary>
        /// The size of the Uv.
        /// </summary>
        public const int UvSize = sizeof(float) * 2;

        /// <summary>
        /// the size of the index.
        /// </summary>
        public const int IndexSize = sizeof(int) * 1;

        /// <summary>
        /// the size of the flags.
        /// </summary>
        public const int FlagsSize = sizeof(int);

        /// <summary>
        /// returns VerticesCount * 3
        /// </summary>
        public int XyzCount
        {
            get { return VerticesCount * 3; }
        }

        /// <summary>
        /// returns VerticesCount * 4
        /// </summary>
        public int RgbaCount
        {
            get { return VerticesCount * 4; }
        }

        /// <summary>
        /// returns VerticesCount * 4
        /// </summary>
        public int Rgba2Count
        {
            get { return VerticesCount * 4; }
        }

        /// <summary>
        /// returns VerticesCount
        /// </summary>
        public int FlagsCount
        {
            get { return VerticesCount; }
        }

        /// <summary>
        /// returns VerticesCount * 2
        /// </summary>
        public int UvCount
        {
            get { return VerticesCount * 2; }
        }


        public float[] GetXyz() { return xyz; }
        public void SetXyz(float[] p) { xyz = p; }
        public byte[] GetRgba() { return Rgba; }
        public void SetRgba(byte[] p) { Rgba = p; }
        public float[] GetUv() { return Uv; }
        public void SetUv(float[] p) { Uv = p; }
        public int[] GetIndices() { return Indices; }
        public void SetIndices(int[] p) { Indices = p; }
        public EnumDrawMode GetMode() { return mode; }
        public void SetMode(EnumDrawMode p) { mode = p; }


        /// <summary>
        /// Offset the mesh by given values
        /// </summary>
        /// <param name="offset"></param>
        public MeshData Translate(Vec3f offset)
        {
            Translate(offset.X, offset.Y, offset.Z);
            return this;
        }

        /// <summary>
        /// Offset the mesh by given values
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public MeshData Translate(float x, float y, float z)
        {
            for (int i = 0; i < VerticesCount; i++)
            {
                xyz[i * 3] += x;
                xyz[i * 3 + 1] += y;
                xyz[i * 3 + 2] += z;
            }
            return this;
        }

        /// <summary>
        /// Rotate the mesh by given angles around given origin
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="radX"></param>
        /// <param name="radY"></param>
        /// <param name="radZ"></param>
        public MeshData Rotate(Vec3f origin, float radX, float radY, float radZ)
        {
            float[] matrix = Mat4f.Create();
            Mat4f.RotateX(matrix, matrix, radX);
            Mat4f.RotateY(matrix, matrix, radY);
            Mat4f.RotateZ(matrix, matrix, radZ);

            return MatrixTransform(matrix, new float[4], origin);
        }


        /// <summary>
        /// Scale the mesh by given values around given origin
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        /// <param name="scaleZ"></param>
        public MeshData Scale(Vec3f origin, float scaleX, float scaleY, float scaleZ)
        {
            float[] matrix = Mat4f.Create();
            Mat4f.Scale(matrix, matrix, new float[] { scaleX, scaleY, scaleZ });

            for (int i = 0; i < VerticesCount; i++)
            {
                Mat4f.MulWithVec3_Position_WithOrigin(matrix, xyz, xyz, i * 3, origin);
            }
            return this;
        }

        /// <summary>
        /// Apply given transformation on the mesh
        /// </summary>
        /// <param name="transform"></param>        
        public MeshData ModelTransform(ModelTransform transform)
        {
            float[] matrix = Mat4f.Create();

            float dx = transform.Translation.X + transform.Origin.X;
            float dy = transform.Translation.Y + transform.Origin.Y;
            float dz = transform.Translation.Z + transform.Origin.Z;
            Mat4f.Translate(matrix, matrix, dx, dy, dz);

            Mat4f.RotateX(matrix, matrix, transform.Rotation.X * GameMath.DEG2RAD);
            Mat4f.RotateY(matrix, matrix, transform.Rotation.Y * GameMath.DEG2RAD);
            Mat4f.RotateZ(matrix, matrix, transform.Rotation.Z * GameMath.DEG2RAD);

            Mat4f.Scale(matrix, matrix, transform.ScaleXYZ.X, transform.ScaleXYZ.Y, transform.ScaleXYZ.Z);

            Mat4f.Translate(matrix, matrix, -transform.Origin.X, -transform.Origin.Y, -transform.Origin.Z);

            MatrixTransform(matrix);

            return this;
        }

        /// <summary>
        /// Apply given transformation on the mesh
        /// </summary>
        /// <param name="matrix"></param>
        public MeshData MatrixTransform(float[] matrix)
        {
            return MatrixTransform(matrix, new float[4]);
        }

        /// <summary>
        /// Apply given transformation on the mesh - specifying two temporary vectors to work in (these can then be re-used for performance reasons)
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="vec">a re-usable float[4], values unimportant</param>
        /// <param name="origin">origin point</param>
        public MeshData MatrixTransform(float[] matrix, float[] vec, Vec3f origin = null)
        {
            if (origin == null)
            {
                for (int i = 0; i < VerticesCount; i++)
                {
                    Mat4f.MulWithVec3_Position(matrix, xyz, xyz, i * 3);
                }
            }
            else
            {
                for (int i = 0; i < VerticesCount; i++)
                {
                    Mat4f.MulWithVec3_Position_WithOrigin(matrix, xyz, xyz, i * 3, origin);
                }
            }

            if (Normals != null)
            {
                for (int i = 0; i < VerticesCount; i++)
                {
                    NormalUtil.FromPackedNormal(Normals[i], ref vec);
                    Mat4f.MulWithVec4(matrix, vec, vec);
                    Normals[i] = NormalUtil.PackNormal(vec);
                }
            }

            if (XyzFaces != null)
            {
                for (int i = 0; i < XyzFaces.Length; i++)
                {
                    byte meshFaceIndex = XyzFaces[i];
                    if (meshFaceIndex == 0) continue;

                    Vec3f normalfv = BlockFacing.ALLFACES[meshFaceIndex - 1].Normalf;
                    XyzFaces[i] = Mat4f.MulWithVec3_BlockFacing(matrix, normalfv).MeshDataIndex;
                }
            }

            if (Flags != null)
            {
                for (int i = 0; i < Flags.Length; i++)
                {
                    VertexFlags.UnpackNormal(Flags[i], vec);

                    Mat4f.MulWithVec3(matrix, vec, vec);

                    float len = GameMath.RootSumOfSquares(vec[0], vec[1], vec[2]);

                    Flags[i] = (Flags[i] & ~VertexFlags.NormalBitMask) | (VertexFlags.PackNormal(vec[0] / len, vec[1] / len, vec[2] / len));
                }
            }

            return this;
        }


        /// <summary>
        /// Apply given transformation on the mesh
        /// </summary>
        /// <param name="matrix"></param>
        public MeshData MatrixTransform(double[] matrix)
        {
            // For performance, before proceeding with a full-scale matrix operation on the whole mesh, we test whether the matrix is a translation-only matrix, meaning a matrix with no scaling or rotation.
            // (This can also include an identity matrix as a special case, as an identity matrix also has no scaling and no rotation: the "translation" in that case is (0,0,0))
            if (Mat4d.IsTranslationOnly(matrix))
            {
                Translate((float)matrix[12], (float)matrix[13], (float)matrix[14]);    // matrix[12] is dX, matrix[13] is dY,  matrix[14] is dZ
                return this;
            }



            // http://www.opengl-tutorial.org/beginners-tutorials/tutorial-3-matrices/

            double[] inVec = new double[] { 0, 0, 0, 0 };
            double[] outVec;

            for (int i = 0; i < VerticesCount; i++)
            {
                // Keep this code - it's more readable than below inlined method. It's worth inlining because this method is called during Shape tesselation, which has *a lot* of shapes to load
                //double[] pos = new double[] { 0, 0, 0, 1 };
                /*pos[0] = xyz[i * 3];
                pos[1] = xyz[i * 3 + 1];
                pos[2] = xyz[i * 3 + 2];

                pos = Mat4d.MulWithVec4(matrix, pos);

                xyz[i * 3] = (float)pos[0];
                xyz[i * 3 + 1] = (float)pos[1];
                xyz[i * 3 + 2] = (float)pos[2];*/

                // Inlined version of above code
                float x = xyz[i * 3];
                float y = xyz[i * 3 + 1];
                float z = xyz[i * 3 + 2];
                xyz[i * 3 + 0] = (float)(matrix[4 * 0 + 0] * x + matrix[4 * 1 + 0] * y + matrix[4 * 2 + 0] * z + matrix[4 * 3 + 0]);
                xyz[i * 3 + 1] = (float)(matrix[4 * 0 + 1] * x + matrix[4 * 1 + 1] * y + matrix[4 * 2 + 1] * z + matrix[4 * 3 + 1]);
                xyz[i * 3 + 2] = (float)(matrix[4 * 0 + 2] * x + matrix[4 * 1 + 2] * y + matrix[4 * 2 + 2] * z + matrix[4 * 3 + 2]);


                if (Normals != null)
                {
                    NormalUtil.FromPackedNormal(Normals[i], ref inVec);
                    outVec = Mat4d.MulWithVec4(matrix, inVec);
                    Normals[i] = NormalUtil.PackNormal(outVec);
                }
            }

            if (XyzFaces != null)
            {
                for (int i = 0; i < XyzFaces.Length; i++)
                {
                    byte meshFaceIndex = XyzFaces[i];
                    if (meshFaceIndex == 0) continue;

                    Vec3d normald = BlockFacing.ALLFACES[meshFaceIndex - 1].Normald;

                    // Inlined version of above code
                    double ox = matrix[4 * 0 + 0] * normald.X + matrix[4 * 1 + 0] * normald.Y + matrix[4 * 2 + 0] * normald.Z;
                    double oy = matrix[4 * 0 + 1] * normald.X + matrix[4 * 1 + 1] * normald.Y + matrix[4 * 2 + 1] * normald.Z;
                    double oz = matrix[4 * 0 + 2] * normald.X + matrix[4 * 1 + 2] * normald.Y + matrix[4 * 2 + 2] * normald.Z;

                    BlockFacing rotatedFacing = BlockFacing.FromVector(ox, oy, oz);

                    XyzFaces[i] = rotatedFacing.MeshDataIndex;
                }
            }

            if (Flags != null)
            {
                for (int i = 0; i < Flags.Length; i++)
                {
                    VertexFlags.UnpackNormal(Flags[i], inVec);

                    // Inlined version of above code
                    double ox = matrix[4 * 0 + 0] * inVec[0] + matrix[4 * 1 + 0] * inVec[1] + matrix[4 * 2 + 0] * inVec[2];
                    double oy = matrix[4 * 0 + 1] * inVec[0] + matrix[4 * 1 + 1] * inVec[1] + matrix[4 * 2 + 1] * inVec[2];
                    double oz = matrix[4 * 0 + 2] * inVec[0] + matrix[4 * 1 + 2] * inVec[1] + matrix[4 * 2 + 2] * inVec[2];

                    Flags[i] = (Flags[i] & ~VertexFlags.NormalBitMask) | (VertexFlags.PackNormal(ox, oy, oz));
                }
            }

            return this;
        }

        /// <summary>
        /// Creates a new mesh data instance with no components initialized.
        /// </summary>
        public MeshData(bool initialiseArrays = true)
        {
            if (initialiseArrays)
            {
                XyzFaces = new byte[0];
                ClimateColorMapIds = new byte[0];
                SeasonColorMapIds = new byte[0];
                RenderPassesAndExtraBits = new short[0];
            }
        }

        /// <summary>
        /// Creates a new mesh data instance with given components, but you can also freely nullify or set individual components after initialization
        /// Any component that is null is ignored by UploadModel/UpdateModel
        /// </summary>
        /// <param name="capacityVertices"></param>
        /// <param name="capacityIndices"></param>
        /// <param name="withUv"></param>
        /// <param name="withNormals"></param>
        /// <param name="withRgba"></param>
        /// <param name="withFlags"></param>
        public MeshData(int capacityVertices, int capacityIndices, bool withNormals = false, bool withUv = true, bool withRgba = true, bool withFlags = true)
        {
            XyzFaces = new byte[0];
            ClimateColorMapIds = new byte[0];
            SeasonColorMapIds = new byte[0];
            RenderPassesAndExtraBits = new short[0];
            xyz = new float[capacityVertices * 3];

            if (withNormals)
            {
                Normals = new int[capacityVertices];
            }

            if (withUv)
            {
                Uv = new float[capacityVertices * 2];
                TextureIndices = new byte[capacityVertices / VerticesPerFace];
            }
            if (withRgba)
            {
                Rgba = new byte[capacityVertices * 4];
            }
            if (withFlags)
            {
                Flags = new int[capacityVertices];
            }

            Indices = new int[capacityIndices];
            IndicesMax = capacityIndices;
            VerticesMax = capacityVertices;
        }

        /// <summary>
        /// Sets up the tints array for holding tint info
        /// </summary>
        /// <returns></returns>
        public MeshData WithColorMaps()
        {
            SeasonColorMapIds = new byte[VerticesMax / 4];
            ClimateColorMapIds = new byte[VerticesMax / 4];
            return this;
        }


        /// <summary>
        /// Sets up the xyzfaces array for holding xyzfaces info
        /// </summary>
        /// <returns></returns>
        public MeshData WithXyzFaces()
        {
            XyzFaces = new byte[VerticesMax / 4];
            return this;
        }

        /// <summary>
        /// Sets up the renderPasses array for holding render pass info
        /// </summary>
        /// <returns></returns>
        public MeshData WithRenderpasses()
        {
            RenderPassesAndExtraBits = new short[VerticesMax / 4];
            return this;
        }


        /// <summary>
        /// Sets up the renderPasses array for holding render pass info
        /// </summary>
        /// <returns></returns>
        public MeshData WithNormals()
        {
            Normals = new int[VerticesMax];
            return this;
        }


        /// <summary>
        /// Add supplied mesh data to this mesh. If a given dataset is not set, it is not copied from the sourceMesh. Automatically adjusts the indices for you.
        /// Is filtered to only add mesh data for given render pass.
        /// A negative render pass value defaults to EnumChunkRenderPass.Opaque
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filterByRenderPass"></param>
        public void AddMeshData(MeshData data, EnumChunkRenderPass filterByRenderPass)
        {
            int renderPassInt = (int)filterByRenderPass;

            AddMeshData(data, (i) =>
            {
                return data.RenderPassesAndExtraBits[i] != renderPassInt && (data.RenderPassesAndExtraBits[i] != -1 || filterByRenderPass != EnumChunkRenderPass.Opaque);
            });
        }

        public delegate bool MeshDataFilterDelegate(int faceIndex);
        public void AddMeshData(MeshData data, MeshDataFilterDelegate dele = null)
        {
            int di = 0;
            int vertexNum;

            int verticesPerFace = this.VerticesPerFace;
            int indicesPerFace = this.IndicesPerFace;

            for (int i = 0; i < data.VerticesCount / verticesPerFace; i++)
            {
                if (dele?.Invoke(i) == false)
                {
                    di += indicesPerFace;
                    continue;
                }

                int lastelement = VerticesCount;

                if (Uv != null)
                {
                    AddTextureId(data.TextureIds[data.TextureIndices[i]]);
                }

                // Face Vertices
                for (int k = 0; k < verticesPerFace; k++)
                {
                    vertexNum = i * verticesPerFace + k;

                    if (VerticesCount >= VerticesMax)
                    {
                        GrowVertexBuffer();
                        GrowNormalsBuffer();
                    }


                    xyz[XyzCount + 0] = data.xyz[vertexNum * 3 + 0];
                    xyz[XyzCount + 1] = data.xyz[vertexNum * 3 + 1];
                    xyz[XyzCount + 2] = data.xyz[vertexNum * 3 + 2];


                    if (Normals != null)
                    {
                        Normals[VerticesCount] = data.Normals[vertexNum];
                    }


                    if (Uv != null)
                    {
                        Uv[UvCount + 0] = data.Uv[vertexNum * 2 + 0];
                        Uv[UvCount + 1] = data.Uv[vertexNum * 2 + 1];
                    }

                    if (Rgba != null)
                    {
                        Rgba[RgbaCount + 0] = data.Rgba[vertexNum * 4 + 0];
                        Rgba[RgbaCount + 1] = data.Rgba[vertexNum * 4 + 1];
                        Rgba[RgbaCount + 2] = data.Rgba[vertexNum * 4 + 2];
                        Rgba[RgbaCount + 3] = data.Rgba[vertexNum * 4 + 3];
                    }

                    if (Flags != null)
                    {
                        Flags[FlagsCount] = data.Flags[vertexNum];
                    }

                    if (CustomInts != null && data.CustomInts != null)
                    {
                        int valsPerVertex = data.CustomInts.InterleaveStride == 0 ? data.CustomInts.InterleaveSizes[0] : data.CustomInts.InterleaveStride;

                        for (int j = 0; j < valsPerVertex; j++)
                        {
                            CustomInts.Add(data.CustomInts.Values[vertexNum / valsPerVertex + j]);
                        }
                    }

                    if (CustomFloats != null && data.CustomFloats != null)
                    {
                        int valsPerVertex = data.CustomFloats.InterleaveStride == 0 ? data.CustomFloats.InterleaveSizes[0] : data.CustomFloats.InterleaveStride;

                        for (int j = 0; j < valsPerVertex; j++)
                        {
                            CustomFloats.Add(data.CustomFloats.Values[vertexNum / valsPerVertex + j]);
                        }
                    }

                    if (CustomShorts != null && data.CustomShorts != null)
                    {
                        int valsPerVertex = data.CustomShorts.InterleaveStride == 0 ? data.CustomShorts.InterleaveSizes[0] : data.CustomShorts.InterleaveStride;

                        for (int j = 0; j < valsPerVertex; j++)
                        {
                            CustomShorts.Add(data.CustomShorts.Values[vertexNum / valsPerVertex + j]);
                        }
                    }

                    if (CustomBytes != null && data.CustomBytes != null)
                    {
                        int valsPerVertex = data.CustomBytes.InterleaveStride == 0 ? data.CustomBytes.InterleaveSizes[0] : data.CustomBytes.InterleaveStride;

                        for (int j = 0; j < valsPerVertex; j++)
                        {
                            CustomBytes.Add(data.CustomBytes.Values[vertexNum / valsPerVertex + j]);
                        }
                    }

                    VerticesCount++;

                }


                // 6 indices
                for (int k = 0; k < indicesPerFace; k++)
                {
                    int indexNum = i * indicesPerFace + k;
                    AddIndex(lastelement - (i - di / indicesPerFace) * verticesPerFace + data.Indices[indexNum] - (2 * di) / 3);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte getTextureIndex(int textureId)
        {
            for (byte i = 0; i < TextureIds.Length; i++)
            {
                if (TextureIds[i] == textureId) return i;
            }
            TextureIds = TextureIds.Append(textureId);
            return (byte)(TextureIds.Length - 1);
        }

        /// <summary>
        /// Add supplied mesh data to this mesh. If a given dataset is not set, it is not copied from the sourceMesh. Automatically adjusts the indices for you.
        /// </summary>
        /// <param name="sourceMesh"></param>
        public void AddMeshData(MeshData sourceMesh)
        {
            for (int i = 0; i < sourceMesh.VerticesCount; i++)
            {
                if (VerticesCount >= VerticesMax)
                {
                    GrowVertexBuffer();
                    GrowNormalsBuffer();
                }

                xyz[XyzCount + 0] = sourceMesh.xyz[i * 3 + 0];
                xyz[XyzCount + 1] = sourceMesh.xyz[i * 3 + 1];
                xyz[XyzCount + 2] = sourceMesh.xyz[i * 3 + 2];

                if (Normals != null)
                {
                    Normals[VerticesCount] = sourceMesh.Normals[i];
                }

                if (Uv != null)
                {
                    Uv[UvCount + 0] = sourceMesh.Uv[i * 2 + 0];
                    Uv[UvCount + 1] = sourceMesh.Uv[i * 2 + 1];
                }

                if (Rgba != null)
                {
                    Rgba[RgbaCount + 0] = sourceMesh.Rgba[i * 4 + 0];
                    Rgba[RgbaCount + 1] = sourceMesh.Rgba[i * 4 + 1];
                    Rgba[RgbaCount + 2] = sourceMesh.Rgba[i * 4 + 2];
                    Rgba[RgbaCount + 3] = sourceMesh.Rgba[i * 4 + 3];
                }

                if (Flags != null && sourceMesh.Flags != null)
                {
                    Flags[VerticesCount] = sourceMesh.Flags[i];
                }

                VerticesCount++;
            }

            addMeshDataEtc(sourceMesh);
        }


        public void AddMeshData(MeshData sourceMesh, float xOffset, float yOffset, float zOffset)
        {
            for (int i = 0; i < sourceMesh.VerticesCount; i++)
            {
                if (VerticesCount >= VerticesMax)
                {
                    GrowVertexBuffer();
                    GrowNormalsBuffer();
                }

                xyz[XyzCount + 0] = sourceMesh.xyz[i * 3 + 0] + xOffset;
                xyz[XyzCount + 1] = sourceMesh.xyz[i * 3 + 1] + yOffset;
                xyz[XyzCount + 2] = sourceMesh.xyz[i * 3 + 2] + zOffset;

                if (Normals != null)
                {
                    Normals[VerticesCount] = sourceMesh.Normals[i];
                }

                if (Uv != null)
                {
                    Uv[UvCount + 0] = sourceMesh.Uv[i * 2 + 0];
                    Uv[UvCount + 1] = sourceMesh.Uv[i * 2 + 1];
                }

                if (Rgba != null)
                {
                    Rgba[RgbaCount + 0] = sourceMesh.Rgba[i * 4 + 0];
                    Rgba[RgbaCount + 1] = sourceMesh.Rgba[i * 4 + 1];
                    Rgba[RgbaCount + 2] = sourceMesh.Rgba[i * 4 + 2];
                    Rgba[RgbaCount + 3] = sourceMesh.Rgba[i * 4 + 3];
                }

                if (Flags != null && sourceMesh.Flags != null)
                {
                    Flags[VerticesCount] = sourceMesh.Flags[i];
                }

                VerticesCount++;
            }

            addMeshDataEtc(sourceMesh);
        }


        private void addMeshDataEtc(MeshData sourceMesh)
        {
            for (int i = 0; i < sourceMesh.XyzFacesCount; i++)
            {
                AddXyzFace(sourceMesh.XyzFaces[i]);
            }

            for (int i = 0; i < sourceMesh.TextureIndicesCount; i++)
            {
                AddTextureId(sourceMesh.TextureIds[sourceMesh.TextureIndices[i]]);
            }

            int start = IndicesCount > 0 ? (mode == EnumDrawMode.Triangles ? Indices[IndicesCount - 1] + 1 : Indices[IndicesCount - 2] + 1) : 0;

            for (int i = 0; i < sourceMesh.IndicesCount; i++)
            {
                AddIndex(start + sourceMesh.Indices[i]);
            }

            for (int i = 0; i < sourceMesh.ColorMapIdsCount; i++)
            {
                AddColorMapIndex(sourceMesh.ClimateColorMapIds[i], sourceMesh.SeasonColorMapIds[i]);
            }

            for (int i = 0; i < sourceMesh.RenderPassCount; i++)
            {
                AddRenderPass(sourceMesh.RenderPassesAndExtraBits[i]);
            }

            if (CustomInts != null && sourceMesh.CustomInts != null)
            {
                for (int i = 0; i < sourceMesh.CustomInts.Count; i++)
                {
                    CustomInts.Add(sourceMesh.CustomInts.Values[i]);
                }
            }

            if (CustomFloats != null && sourceMesh.CustomFloats != null)
            {
                for (int i = 0; i < sourceMesh.CustomFloats.Count; i++)
                {
                    CustomFloats.Add(sourceMesh.CustomFloats.Values[i]);
                }
            }

            if (CustomShorts != null && sourceMesh.CustomShorts != null)
            {
                for (int i = 0; i < sourceMesh.CustomShorts.Count; i++)
                {
                    CustomShorts.Add(sourceMesh.CustomShorts.Values[i]);
                }
            }

            if (CustomBytes != null && sourceMesh.CustomBytes != null)
            {
                for (int i = 0; i < sourceMesh.CustomBytes.Count; i++)
                {
                    CustomBytes.Add(sourceMesh.CustomBytes.Values[i]);
                }
            }
        }




        /// <summary>
        /// Removes the last index in the indices array
        /// </summary>
        public void RemoveIndex()
        {
            if (IndicesCount > 0) IndicesCount--;
        }

        /// <summary>
        /// Removes the last vertex in the vertices array
        /// </summary>
        public void RemoveVertex()
        {
            if (VerticesCount > 0) VerticesCount--;
        }

        /// <summary>
        /// Removes the last "count" vertices from the vertex array
        /// </summary>
        /// <param name="count"></param>
        public void RemoveVertices(int count)
        {
            VerticesCount = Math.Max(0, VerticesCount - count);
        }



        /// <summary>
        /// Adds a new vertex to the mesh. Grows the vertex buffer if necessary.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="color"></param>
        public void AddVertexSkipTex(float x, float y, float z, int color = ColorUtil.WhiteArgb)
        {
            int count = VerticesCount;
            if (count >= VerticesMax)
            {
                GrowVertexBuffer();
            }

            float[] xyz = this.xyz;

            int xyzCount = count * 3;
            xyz[xyzCount + 0] = x;
            xyz[xyzCount + 1] = y;
            xyz[xyzCount + 2] = z;

            // Write int color into byte array
            unsafe
            {
                fixed (byte* rgbaByte = Rgba)
                {
                    int* rgbaInt = (int*)rgbaByte;
                    rgbaInt[count] = color;
                }
            }

            VerticesCount = count + 1;
        }



        /// <summary>
        /// Adds a new vertex to the mesh. Grows the vertex buffer if necessary.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="u"></param>
        /// <param name="v"></param>
        public void AddVertex(float x, float y, float z, float u, float v)
        {
            int count = VerticesCount;
            if (count >= VerticesMax)
            {
                GrowVertexBuffer();
            }

            float[] xyz = this.xyz;
            float[] Uv = this.Uv;

            int xyzCount = count * 3;
            xyz[xyzCount + 0] = x;
            xyz[xyzCount + 1] = y;
            xyz[xyzCount + 2] = z;

            int uvCount = count * 2;
            Uv[uvCount + 0] = u;
            Uv[uvCount + 1] = v;

            VerticesCount = count + 1;
        }

        /// <summary>
        /// Adds a new vertex to the mesh. Grows the vertex buffer if necessary.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="color"></param>
        public void AddVertex(float x, float y, float z, float u, float v, int color)
        {
            AddWithFlagsVertex(x, y, z, u, v, color, 0);
        }


        /// <summary>
        /// Adds a new vertex to the mesh. Grows the vertex buffer if necessary.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="color"></param>
        public void AddVertex(float x, float y, float z, float u, float v, byte[] color)
        {
            int count = VerticesCount;
            if (count >= VerticesMax)
            {
                GrowVertexBuffer();
            }

            float[] xyz = this.xyz;
            float[] Uv = this.Uv;
            byte[] Rgba = this.Rgba;

            int xyzCount = count * 3;
            xyz[xyzCount + 0] = x;
            xyz[xyzCount + 1] = y;
            xyz[xyzCount + 2] = z;

            int uvCount = count * 2;
            Uv[uvCount + 0] = u;
            Uv[uvCount + 1] = v;

            int rgbaCount = count * 4;
            Rgba[rgbaCount + 0] = color[0];
            Rgba[rgbaCount + 1] = color[1];
            Rgba[rgbaCount + 2] = color[2];
            Rgba[rgbaCount + 3] = color[3];

            VerticesCount = count + 1;
        }



        /// <summary>
        /// Adds a new vertex to the mesh. Grows the vertex buffer if necessary.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="color"></param>
        /// <param name="flags"></param>
        public void AddWithFlagsVertex(float x, float y, float z, float u, float v, int color, int flags)
        {
            int count = VerticesCount;
            if (count >= VerticesMax)
            {
                GrowVertexBuffer();
            }

            float[] xyz = this.xyz;
            float[] Uv = this.Uv;

            int xyzCount = count * 3;
            xyz[xyzCount + 0] = x;
            xyz[xyzCount + 1] = y;
            xyz[xyzCount + 2] = z;

            int uvCount = count * 2;
            Uv[uvCount + 0] = u;
            Uv[uvCount + 1] = v;

            if (this.Flags != null)
            {
                this.Flags[count] = flags;
            }


            // Write int color into byte array
            unsafe
            {
                fixed (byte* rgbaByte = Rgba)
                {
                    int* rgbaInt = (int*)rgbaByte;
                    rgbaInt[count] = color;
                }
            }


            VerticesCount = count + 1;
        }


        /// <summary>
        /// Adds a new vertex to the mesh. Grows the vertex buffer if necessary.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="color"></param>
        /// <param name="flags"></param>
        public void AddVertexWithFlags(float x, float y, float z, float u, float v, int color, int flags)
        {
            int count = VerticesCount;
            if (count >= VerticesMax)
            {
                GrowVertexBuffer();
            }

            float[] xyz = this.xyz;
            float[] Uv = this.Uv;

            int xyzCount = count * 3;
            xyz[xyzCount + 0] = x;
            xyz[xyzCount + 1] = y;
            xyz[xyzCount + 2] = z;

            int uvCount = count * 2;
            Uv[uvCount + 0] = u;
            Uv[uvCount + 1] = v;

            if (this.Flags != null)
            {
                this.Flags[count] = flags;
            }

            // Write int color into byte array
            unsafe
            {
                fixed (byte* rgbaByte = Rgba)
                {
                    int* rgbaInt = (int*)rgbaByte;
                    rgbaInt[count] = color;
                }
            }

            VerticesCount = count + 1;
        }


        public void AddVertexWithFlagsSkipTexture(float x, float y, float z, float u, float v, int color, int flags)
        {
            int count = VerticesCount;
            if (count >= VerticesMax)
            {
                GrowVertexBuffer();
            }

            float[] xyz = this.xyz;
            float[] Uv = this.Uv;

            int xyzCount = count * 3;
            xyz[xyzCount + 0] = x;
            xyz[xyzCount + 1] = y;
            xyz[xyzCount + 2] = z;

            int uvCount = count * 2;
            Uv[uvCount + 0] = u;
            Uv[uvCount + 1] = v;

            if (this.Flags != null)
            {
                this.Flags[count] = flags;
            }

            // Write int color into byte array
            unsafe
            {
                fixed (byte* rgbaByte = Rgba)
                {
                    int* rgbaInt = (int*)rgbaByte;
                    rgbaInt[count] = color;
                }
            }

            VerticesCount = count + 1;
        }


        /// <summary>
        /// Applies a vertex flag to an existing MeshData (uses binary OR)
        /// </summary>
        public void SetVertexFlags(int flag)
        {
            if (this.Flags != null)
            {
                int count = VerticesCount;
                for (int i = 0; i < count; i++)
                {
                    this.Flags[i] |= flag;
                }
            }
        }


        /// <summary>
        /// Adds a new normal to the mesh. Grows the normal buffer if necessary.
        /// </summary>
        /// <param name="normalizedX"></param>
        /// <param name="normalizedY"></param>
        /// <param name="normalizedZ"></param>
        public void AddNormal(float normalizedX, float normalizedY, float normalizedZ)
        {
            if (NormalsCount >= Normals.Length) GrowNormalsBuffer();

            Normals[NormalsCount++] = NormalUtil.PackNormal(normalizedX, normalizedY, normalizedZ);
        }

        /// <summary>
        /// Adds a new normal to the mesh. Grows the normal buffer if necessary.
        /// </summary>
        /// <param name="facing"></param>
        public void AddNormal(BlockFacing facing)
        {
            if (NormalsCount >= Normals.Length) GrowNormalsBuffer();

            Normals[NormalsCount++] = facing.NormalPacked;
        }

        public void AddColorMapIndex(byte climateMapIndex, byte seasonMapIndex)
        {
            if (ColorMapIdsCount >= SeasonColorMapIds.Length)
            {
                Array.Resize(ref SeasonColorMapIds, SeasonColorMapIds.Length + 32);
                Array.Resize(ref ClimateColorMapIds, ClimateColorMapIds.Length + 32);
            }

            ClimateColorMapIds[ColorMapIdsCount] = climateMapIndex;
            SeasonColorMapIds[ColorMapIdsCount++] = seasonMapIndex;
        }

        public void AddRenderPass(short renderPass)
        {
            if (RenderPassCount >= RenderPassesAndExtraBits.Length)
            {
                Array.Resize(ref RenderPassesAndExtraBits, RenderPassesAndExtraBits.Length + 32);
            }

            RenderPassesAndExtraBits[RenderPassCount++] = renderPass;
        }


        public void AddXyzFace(byte faceIndex)
        {
            if (XyzFacesCount >= XyzFaces.Length)
            {
                Array.Resize(ref XyzFaces, XyzFaces.Length + 32);
            }

            XyzFaces[XyzFacesCount++] = faceIndex;
        }

        public void AddTextureId(int textureId)
        {
            if (TextureIndicesCount >= TextureIndices.Length)
            {
                Array.Resize(ref TextureIndices, TextureIndices.Length + 32);
            }

            TextureIndices[TextureIndicesCount++] = getTextureIndex(textureId);
        }

        public void AddIndex(int index)
        {
            if (IndicesCount >= IndicesMax)
            {
                GrowIndexBuffer();
            }

            Indices[IndicesCount++] = index;
        }

        public void AddIndices(int i1, int i2, int i3, int i4, int i5, int i6)
        {
            int count = IndicesCount;
            if (count + 6 > IndicesMax)
            {
                GrowIndexBuffer(6);
            }
            int[] currentIndices = this.Indices;

            currentIndices[count++] = i1;
            currentIndices[count++] = i2;
            currentIndices[count++] = i3;
            currentIndices[count++] = i4;
            currentIndices[count++] = i5;
            currentIndices[count++] = i6;
            IndicesCount = count;
        }

        public void AddIndices(int[] indices)
        {
            int length = indices.Length;
            int count = IndicesCount;
            if (count + length > IndicesMax)
            {
                GrowIndexBuffer(length);
            }
            int[] currentIndices = this.Indices;

            for (int i = 0; i < length; i++)
            {
                currentIndices[count++] = indices[i];
            }
            IndicesCount = count;
        }

        public void GrowIndexBuffer()
        {
            int i = IndicesCount;
            int[] largerIndices = new int[IndicesMax = i * 2];  //there was previously a potential bug if this was ever called with IndicesCount < IndicesMax (it never was!)

            int[] currentIndices = this.Indices;
            while (--i >= 0)
            {
                largerIndices[i] = currentIndices[i];
            }
            Indices = largerIndices;
        }


        public void GrowIndexBuffer(int byAtLeastQuantity)
        {
            int newSize = Math.Max(IndicesCount * 2, IndicesCount + byAtLeastQuantity);
            int[] largerIndices = new int[IndicesMax = newSize];

            int[] currentIndices = this.Indices;
            int i = IndicesCount;
            while (--i >= 0)
            {
                largerIndices[i] = currentIndices[i];
            }
            Indices = largerIndices;
        }



        public void GrowNormalsBuffer()
        {
            if (Normals != null)
            {
                int i = Normals.Length;
                int[] largerNormals = new int[i * 2];
                int[] currentNormals = this.Normals;
                while (--i >= 0)
                {
                    largerNormals[i] = currentNormals[i];
                }
                Normals = largerNormals;
            }
        }

        /// <summary>
        /// Doubles the size of the xyz, uv, rgba, rgba2 and flags arrays
        /// </summary>
        public void GrowVertexBuffer()
        {
            if (xyz != null)
            {
                int xyzCount = XyzCount;
                float[] largerXyz = new float[xyzCount * 2];
                float[] currentXyz = xyz;
                int i = currentXyz.Length;
                while (--i >= 0)
                {
                    largerXyz[i] = currentXyz[i];
                }
                xyz = largerXyz;
            }

            if (Uv != null)
            {
                int uvCount = UvCount;
                float[] largerUv = new float[uvCount * 2];
                float[] currentUv = Uv;
                int i = currentUv.Length;
                while (--i >= 0)
                {
                    largerUv[i] = currentUv[i];
                }
                Uv = largerUv;
            }

            if (Rgba != null)
            {
                int rgbaCount = RgbaCount;
                byte[] largerRgba = new byte[rgbaCount * 2];
                byte[] currentRgba = Rgba;
                int i = currentRgba.Length;
                while (--i >= 0)
                {
                    largerRgba[i] = currentRgba[i];
                }
                Rgba = largerRgba;
            }

            if (Flags != null)
            {
                int flagsCount = FlagsCount;
                int[] largerFlags = new int[flagsCount * 2];
                int[] currentFlags = Flags;
                int i = currentFlags.Length;
                while (--i >= 0)
                {
                    largerFlags[i] = currentFlags[i];
                }
                Flags = largerFlags;
            }

            VerticesMax = VerticesMax * 2;
        }


        /// <summary>
        /// Resizes all buffers to tightly fit the data. Recommended to run this method for long-term in-memory storage of meshdata for meshes that won't get any new vertices added
        /// </summary>
        public void CompactBuffers()
        {
            if (xyz != null)
            {
                int cnt = XyzCount;
                float[] tightXyz = new float[cnt + 1];
                Array.Copy(xyz, 0, tightXyz, 0, cnt);
                xyz = tightXyz;
            }

            if (Uv != null)
            {
                int cnt = UvCount;
                float[] tightUv = new float[cnt + 1];
                Array.Copy(Uv, 0, tightUv, 0, cnt);
                Uv = tightUv;
            }

            if (Rgba != null)
            {
                int cnt = RgbaCount;
                byte[] tightRgba = new byte[cnt + 1];
                Array.Copy(Rgba, 0, tightRgba, 0, cnt);
                Rgba = tightRgba;
            }

            if (Flags != null)
            {
                int cnt = FlagsCount;
                int[] tightFlags = new int[cnt + 1];
                Array.Copy(Flags, 0, tightFlags, 0, cnt);
                Flags = tightFlags;
            }

            VerticesMax = VerticesCount;
        }

        /// <summary>
        /// Creates a compact, deep copy of the mesh
        /// </summary>
        /// <returns></returns>
        public MeshData Clone()
        {
            MeshData dest = new MeshData(false);
            dest.VerticesPerFace = VerticesPerFace;
            dest.IndicesPerFace = IndicesPerFace;

            unchecked
            {
                dest.SetVerticesCount(VerticesCount);
                dest.xyz = xyz.FastCopy(XyzCount);

                if (Normals != null)
                {
                    dest.Normals = Normals.FastCopy(NormalsCount);
                }

                if (XyzFaces != null)
                {
                    dest.XyzFaces = XyzFaces.FastCopy(XyzFacesCount);
                    dest.XyzFacesCount = XyzFacesCount;
                }

                if (TextureIndices != null)
                {
                    dest.TextureIndices = TextureIndices.FastCopy(TextureIndicesCount);
                    dest.TextureIndicesCount = TextureIndicesCount;
                    dest.TextureIds = (int[])TextureIds.Clone();
                }

                if (ClimateColorMapIds != null)
                {
                    dest.ClimateColorMapIds = ClimateColorMapIds.FastCopy(ColorMapIdsCount);
                    dest.ColorMapIdsCount = ColorMapIdsCount;
                }

                if (SeasonColorMapIds != null)
                {
                    dest.SeasonColorMapIds = SeasonColorMapIds.FastCopy(ColorMapIdsCount);
                    dest.ColorMapIdsCount = ColorMapIdsCount;
                }

                if (RenderPassesAndExtraBits != null)
                {
                    dest.RenderPassesAndExtraBits = RenderPassesAndExtraBits.FastCopy(RenderPassCount);
                    dest.RenderPassCount = RenderPassCount;
                }

                if (Uv != null)
                {
                    dest.Uv = Uv.FastCopy(UvCount);
                }

                if (Rgba != null)
                {
                    dest.Rgba = Rgba.FastCopy(RgbaCount);
                }

                if (Flags != null)
                {
                    dest.Flags = Flags.FastCopy(FlagsCount);
                }

                dest.Indices = Indices.FastCopy(IndicesCount);
                dest.SetIndicesCount(GetIndicesCount());

                if (CustomFloats != null)
                {
                    dest.CustomFloats = CustomFloats.Clone();
                }

                if (CustomShorts != null)
                {
                    dest.CustomShorts = CustomShorts.Clone();
                }

                if (CustomBytes != null)
                {
                    dest.CustomBytes = CustomBytes.Clone();
                }

                if (CustomInts != null)
                {
                    dest.CustomInts = CustomInts.Clone();
                }

                // This is wrong, because we don't copy unused elements
                //dest.VerticesMax = VerticesMax;
                //dest.IndicesMax = IndicesMax;

                dest.VerticesMax = XyzCount / 3;
                dest.IndicesMax = dest.Indices.Length;
            }

            return dest;
        }



        /// <summary>
        /// Creates an empty copy of the mesh
        /// </summary>
        /// <returns></returns>
        public MeshData EmptyClone()
        {
            MeshData dest = new MeshData(false);
            dest.VerticesPerFace = VerticesPerFace;
            dest.IndicesPerFace = IndicesPerFace;

            unchecked
            {
                dest.xyz = new float[XyzCount];

                if (Normals != null)
                {
                    dest.Normals = new int[Normals.Length];
                }

                if (XyzFaces != null)
                {
                    dest.XyzFaces = new byte[XyzFaces.Length];
                }

                if (TextureIndices != null)
                {
                    dest.TextureIndices = new byte[TextureIndices.Length];
                }

                if (ClimateColorMapIds != null)
                {
                    dest.ClimateColorMapIds = new byte[ClimateColorMapIds.Length];
                }

                if (SeasonColorMapIds != null)
                {
                    dest.SeasonColorMapIds = new byte[SeasonColorMapIds.Length];
                }

                if (RenderPassesAndExtraBits != null)
                {
                    dest.RenderPassesAndExtraBits = new short[RenderPassesAndExtraBits.Length];
                }

                if (Uv != null)
                {
                    dest.Uv = new float[UvCount];
                }

                if (Rgba != null)
                {
                    dest.Rgba = new byte[RgbaCount];
                }

                if (Flags != null)
                {
                    dest.Flags = new int[FlagsCount];
                }

                dest.Indices = new int[GetIndicesCount()];

                if (CustomFloats != null)
                {
                    dest.CustomFloats = CustomFloats.EmptyClone();
                }

                if (CustomShorts != null)
                {
                    dest.CustomShorts = CustomShorts.EmptyClone();
                }

                if (CustomBytes != null)
                {
                    dest.CustomBytes = CustomBytes.EmptyClone();
                }

                if (CustomInts != null)
                {
                    dest.CustomInts = CustomInts.EmptyClone();
                }

                dest.VerticesMax = XyzCount / 3;
                dest.IndicesMax = dest.Indices.Length;
            }

            return dest;
        }



        /// <summary>
        /// Sets the counts of all data to 0
        /// </summary>
        public MeshData Clear()
        {
            IndicesCount = 0;
            VerticesCount = 0;
            ColorMapIdsCount = 0;
            RenderPassCount = 0;
            XyzFacesCount = 0;
            NormalsCount = 0;
            TextureIndicesCount = 0;
            if (CustomBytes != null) CustomBytes.Count = 0;
            if (CustomFloats != null) CustomFloats.Count = 0;
            if (CustomShorts != null) CustomShorts.Count = 0;
            if (CustomInts != null) CustomInts.Count = 0;
            return this;
        }

        public int SizeInBytes()
        {
            return
                (xyz == null ? 0 : xyz.Length * 4) +
                (Indices == null ? 0 : Indices.Length * 4) +
                (Rgba == null ? 0 : Rgba.Length) +
                (ClimateColorMapIds == null ? 0 : ClimateColorMapIds.Length * 1) +
                (SeasonColorMapIds == null ? 0 : SeasonColorMapIds.Length * 1) +
                (XyzFaces == null ? 0 : XyzFaces.Length * 1) +
                (RenderPassesAndExtraBits == null ? 0 : RenderPassesAndExtraBits.Length * 2) +
                (Normals == null ? 0 : Normals.Length * 4) +
                (Flags == null ? 0 : Flags.Length * 4) +
                (Uv == null ? 0 : Uv.Length * 4) +
                (CustomBytes?.Values == null ? 0 : CustomBytes.Values.Length) +
                (CustomFloats?.Values == null ? 0 : CustomFloats.Values.Length * 4) +
                (CustomShorts?.Values == null ? 0 : CustomShorts.Values.Length * 2) +
                (CustomInts?.Values == null ? 0 : CustomInts.Values.Length * 4)
            ;
        }


        /// <summary>
        /// Returns a copy of this mesh with the uvs set to the specified TextureAtlasPosition
        /// </summary>
        public MeshData WithTexPos(TextureAtlasPosition texPos)
        {
            MeshData meshClone = this.Clone();
            meshClone.SetTexPos(texPos);
            return meshClone;
        }

        /// <summary>
        /// Sets the uvs of this mesh to the specified TextureAtlasPosition, assuming the initial UVs range from 0..1, as they will be scaled by the texPos
        /// </summary>
        public void SetTexPos(TextureAtlasPosition texPos)
        {
            float wdt = texPos.x2 - texPos.x1;
            float hgt = texPos.y2 - texPos.y1;

            for (int i = 0; i < this.Uv.Length; i++)
            {
                this.Uv[i] = i % 2 == 0 ? (this.Uv[i] * wdt) + texPos.x1 : (this.Uv[i] * hgt) + texPos.y1;
            }

            var texIndex = getTextureIndex(texPos.atlasTextureId);
            for (int i = 0; i < this.TextureIndices.Length; i++)
            {
                this.TextureIndices[i] = texIndex;
            }
        }

        public MeshData[] SplitByTextureId()
        {
            var meshes = new MeshData[TextureIds.Length];
            for (int i = 0; i < meshes.Length; i++)
            {
                var mesh = meshes[i] = EmptyClone();
                
                mesh.AddMeshData(this, (faceindex) => TextureIndices[faceindex]==i);
                mesh.CompactBuffers();
            }

            return meshes;
        }
    }

}
