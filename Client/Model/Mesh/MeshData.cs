using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// A bit fat data structure that can be used to upload mesh information onto the graphics card
    /// Please note, all arrays are used as a buffer. They do not tightly fit the data but are always sized as a multiple of 2 from the initial size.
    /// </summary>
    public class MeshData
    {
        /// <summary>
        /// The x/y/z coordinates buffer. This should hold VerticesCount*3 values.
        /// </summary>
        public float[] xyz;

        /// <summary>
        /// The render flags buffer. This should hold VerticesCount*1 values.
        /// </summary>
        public int[] Flags;

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
        /// The second vertex color buffer. This should hold VerticesCount*4 values.
        /// </summary>
        //public byte[] Rgba2;

        /// <summary>
        /// The indices buffer. This should hold IndicesCount values.
        /// </summary>
        public int[] Indices;
        

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
        public byte[] XyzFaces = new byte[0];

        /// <summary>
        /// Amount of assigned xyz face values
        /// </summary>
        public int XyzFacesCount;


        public int IndicesPerFace = 6;
        public int VerticesPerFace = 4;

        /// <summary>
        /// BlockShapeTesselator climate colormap ids. Required by TerrainChunkTesselator to determine whether to color a vertex by a color map or not. Should hold VerticesCount / 4 values. Set to 0 for no color mapping, set 1..n for color map 0..n-1
        /// </summary>
        public byte[] ClimateColorMapIds = new byte[0];
        /// <summary>
        /// BlockShapeTesselator season colormap ids. Required by TerrainChunkTesselator to determine whether to color a vertex by a color map or not. Should hold VerticesCount / 4 values. Set to 0 for no color mapping, set 1..n for color map 0..n-1
        /// </summary>
        public byte[] SeasonColorMapIds = new byte[0];

        /// <summary>
        /// BlockShapeTesselator renderpass. Required by TerrainChunkTesselator to determine in which mesh data pool each quad should land in. Should hold VerticesCount / 4 values.
        /// </summary>
        public short[] RenderPasses = new short[0];

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

            float[] normal = new float[4];
            float[] pos = new float[] { 0, 0, 0, 1 };

            for (int i = 0; i < VerticesCount; i++)
            {
                pos[0] = xyz[i * 3] - origin.X;
                pos[1] = xyz[i * 3 + 1] - origin.Y;
                pos[2] = xyz[i * 3 + 2] - origin.Z;

                pos = Mat4f.MulWithVec4(matrix, pos);

                xyz[i * 3] = pos[0] + origin.X;
                xyz[i * 3 + 1] = pos[1] + origin.Y;
                xyz[i * 3 + 2] = pos[2] + origin.Z;

                if (Normals != null)
                {
                    NormalUtil.FromPackedNormal(Normals[i], ref normal);
                    normal = Mat4f.MulWithVec4(matrix, normal);
                    Normals[i] = NormalUtil.PackNormal(normal);
                }
            }


            if (XyzFaces != null)
            {
                float[] normalf = new float[4];
                for (int i = 0; i < XyzFaces.Length; i++)
                {
                    byte meshFaceIndex = XyzFaces[i];
                    if (meshFaceIndex == 0) continue;

                    Vec3f normalfv = BlockFacing.ALLFACES[meshFaceIndex - 1].Normalf;
                    normalf[0] = normalfv.X;
                    normalf[1] = normalfv.Y;
                    normalf[2] = normalfv.Z;
                    normalf = Mat4f.MulWithVec4(matrix, normalf);

                    XyzFaces[i] = BlockFacing.FromVector(normalf[0], normalf[1], normalf[2]).MeshDataIndex;
                }
            }

            if (Flags != null)
            {
                for (int i = 0; i < Flags.Length; i++)
                {
                    VertexFlags.PackedIntToNormal(Flags[i] >> 15, normal);
                    normal[3] = 0;
                    normal = Mat4f.MulWithVec4(matrix, normal);

                    Flags[i] = (Flags[i] & ~VertexFlags.NormalBitMask) | (VertexFlags.NormalToPackedInt(normal[0], normal[1], normal[2]) << 15);
                }
            }

            return this;
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

            float[] pos = new float[] { 0, 0, 0, 1 };

            for (int i = 0; i < VerticesCount; i++)
            {
                pos[0] = xyz[i * 3] - origin.X;
                pos[1] = xyz[i * 3 + 1] - origin.Y;
                pos[2] = xyz[i * 3 + 2] - origin.Z;

                pos = Mat4f.MulWithVec4(matrix, pos);

                xyz[i * 3] = pos[0] + origin.X;
                xyz[i * 3 + 1] = pos[1] + origin.Y;
                xyz[i * 3 + 2] = pos[2] + origin.Z;
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

            Mat4f.Translate(matrix, matrix, transform.Translation.X, transform.Translation.Y, transform.Translation.Z);

            Mat4f.Translate(matrix, matrix, transform.Origin.X, transform.Origin.Y, transform.Origin.Z);

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
            float[] pos = new float[] { 0, 0, 0, 1 };
            float[] normal = new float[4];
            float[] outpos = new float[4];

            for (int i = 0; i < VerticesCount; i++)
            {
                pos[0] = xyz[i * 3];
                pos[1] = xyz[i * 3 + 1];
                pos[2] = xyz[i * 3 + 2];

                outpos[0] = outpos[1] = outpos[2] = 0;
                Mat4f.MulWithVec4(matrix, pos, outpos);

                xyz[i * 3] = outpos[0];
                xyz[i * 3 + 1] = outpos[1];
                xyz[i * 3 + 2] = outpos[2];
            }

            if (Normals != null)
            {
                for (int i = 0; i < VerticesCount; i++)
                {
                    NormalUtil.FromPackedNormal(Normals[i], ref normal);
                    normal = Mat4f.MulWithVec4(matrix, normal);
                    Normals[i] = NormalUtil.PackNormal(normal);
                }
            }

            if (XyzFaces != null)
            {
                float[] normalf = new float[4];
                for (int i = 0; i < XyzFaces.Length; i++)
                {
                    byte meshFaceIndex = XyzFaces[i];
                    if (meshFaceIndex == 0) continue;

                    Vec3f normalfv = BlockFacing.ALLFACES[meshFaceIndex - 1].Normalf;
                    normalf[0] = normalfv.X;
                    normalf[1] = normalfv.Y;
                    normalf[2] = normalfv.Z;

                    outpos[0] = outpos[1] = outpos[2] = 0;
                    Mat4f.MulWithVec4(matrix, normalf, outpos);

                    XyzFaces[i] = BlockFacing.FromVector(outpos[0], outpos[1], outpos[2]).MeshDataIndex;
                }
            }

            if (Flags != null)
            {
                for (int i = 0; i < Flags.Length; i++)
                {
                    VertexFlags.PackedIntToNormal(Flags[i] >> 15, normal);
                    normal[3] = 0; // This is a direction, not a position

                    outpos[0] = outpos[1] = outpos[2] = 0;
                    Mat4f.MulWithVec4(matrix, normal, outpos);

                    float len = GameMath.Sqrt(outpos[0] * outpos[0] + outpos[1] * outpos[1] + outpos[2] * outpos[2]);

                    Flags[i] = (Flags[i] & ~VertexFlags.NormalBitMask) | (VertexFlags.NormalToPackedInt(outpos[0] / len, outpos[1] / len, outpos[2] / len) << 15);
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
            // http://www.opengl-tutorial.org/beginners-tutorials/tutorial-3-matrices/
            double[] pos = new double[] { 0, 0, 0, 1 };
            double[] inVec = new double[] { 0, 0, 0, 0 };
            double[] outVec;

            for (int i = 0; i < VerticesCount; i++)
            {
                pos[0] = xyz[i * 3];
                pos[1] = xyz[i * 3 + 1];
                pos[2] = xyz[i * 3 + 2];

                pos = Mat4d.MulWithVec4(matrix, pos);

                xyz[i * 3] = (float)pos[0];
                xyz[i * 3 + 1] = (float)pos[1];
                xyz[i * 3 + 2] = (float)pos[2];

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

                    Vec3f normalf = BlockFacing.ALLFACES[meshFaceIndex - 1].Normalf;
                    inVec[0] = normalf.X;
                    inVec[1] = normalf.Y;
                    inVec[2] = normalf.Z;

                    outVec = Mat4d.MulWithVec4(matrix, inVec);
                    BlockFacing rotatedFacing = BlockFacing.FromVector(outVec[0], outVec[1], outVec[2]);
                    
                    XyzFaces[i] = rotatedFacing.MeshDataIndex;
                }
            }

            if (Flags != null)
            {
                for (int i = 0; i < Flags.Length; i++)
                {
                    VertexFlags.PackedIntToNormal(Flags[i] >> 15, inVec);
                    outVec = Mat4d.MulWithVec4(matrix, inVec);

                    Flags[i] = (Flags[i] & ~VertexFlags.NormalBitMask) | (VertexFlags.NormalToPackedInt(outVec[0], outVec[1], outVec[2]) << 15);
                }
            }

            return this;
        }

        /// <summary>
        /// Creates a new mesh data instance with no components initialized.
        /// </summary>
        public MeshData()
        {

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
        /// <param name="withRgba2"></param>
        /// <param name="withFlags"></param>
        public MeshData(int capacityVertices, int capacityIndices, bool withNormals = false, bool withUv = true, bool withRgba = true, bool withFlags = true)
        {
            xyz = new float[capacityVertices * 3];

            if (withNormals)
            {
                Normals = new int[capacityVertices];
            }

            if (withUv)
            {
                Uv = new float[capacityVertices * 2];
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
            RenderPasses = new short[VerticesMax / 4];
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
            int di = 0;
            int vertexNum = 0;

            for (int i = 0; i < data.VerticesCount / 4; i++)
            {
                if (data.RenderPasses[i] != renderPassInt && (data.RenderPasses[i] !=-1 || filterByRenderPass != EnumChunkRenderPass.Opaque))
                {
                    di += 6;
                    continue;
                }

                int lastelement = VerticesCount;

                // 4 vertices
                for (int k = 0; k < 4; k++)
                {
                    vertexNum = i*4 + k;

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

                    /*if (Rgba2 != null)
                    {
                        Rgba2[RgbaCount + 0] = data.Rgba2[vertexNum * 4 + 0];
                        Rgba2[RgbaCount + 1] = data.Rgba2[vertexNum * 4 + 1];
                        Rgba2[RgbaCount + 2] = data.Rgba2[vertexNum * 4 + 2];
                        Rgba2[RgbaCount + 3] = data.Rgba2[vertexNum * 4 + 3];
                    }*/

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
                for (int k = 0; k < 6; k++)
                {
                    int indexNum = i * 6 + k;
                    AddIndex(lastelement - (i - di / 6) * 4 + data.Indices[indexNum] - (2 * di) / 3);
                }
            }
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

                /*if (Rgba2 != null && sourceMesh.Rgba2 != null)
                {
                    Rgba2[RgbaCount + 0] = sourceMesh.Rgba2[i * 4 + 0];
                    Rgba2[RgbaCount + 1] = sourceMesh.Rgba2[i * 4 + 1];
                    Rgba2[RgbaCount + 2] = sourceMesh.Rgba2[i * 4 + 2];
                    Rgba2[RgbaCount + 3] = sourceMesh.Rgba2[i * 4 + 3];
                }*/

                if (Flags != null && sourceMesh.Flags != null)
                {
                    Flags[VerticesCount] = sourceMesh.Flags[i];
                }


                VerticesCount++;
            }
        
            int start = IndicesCount > 0 ? (mode == EnumDrawMode.Triangles ? Indices[IndicesCount - 1] + 1 : Indices[IndicesCount - 2] + 1) : 0;

            for (int i = 0; i < sourceMesh.IndicesCount; i++)
            {
                AddIndex(start + sourceMesh.Indices[i]);
            }

            for (int i = 0; i < sourceMesh.XyzFacesCount; i++)
            {
                AddXyzFace(sourceMesh.XyzFaces[i]);
            }

            for (int i = 0; i < sourceMesh.ColorMapIdsCount; i++)
            {
                AddColorMapIndex(sourceMesh.ClimateColorMapIds[i], sourceMesh.SeasonColorMapIds[i]);
            }

            for (int i = 0; i < sourceMesh.RenderPassCount; i++)
            {
                AddRenderPass(sourceMesh.RenderPasses[i]);
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


        public void AddMeshData(MeshData data, int xOff, int yOff, int zOff, int lightMultiply, int lightMultiply2)
        {
            int lastelement = VerticesCount;

            for (int i = 0; i < data.VerticesCount; i++)
            {
                if (VerticesCount >= VerticesMax)
                {
                    GrowVertexBuffer();
                }

                xyz[XyzCount + 0] = data.xyz[i * 3 + 0] + xOff;
                xyz[XyzCount + 1] = data.xyz[i * 3 + 1] + yOff;
                xyz[XyzCount + 2] = data.xyz[i * 3 + 2] + zOff;

                if (Normals != null)
                {
                    Normals[VerticesCount] = data.Normals[i];
                }


                if (Uv != null)
                {
                    Uv[UvCount + 0] = data.Uv[i * 2 + 0];
                    Uv[UvCount + 1] = data.Uv[i * 2 + 1];
                }

                if (Rgba != null)
                {
                    Rgba[RgbaCount + 0] = (byte)((data.Rgba[i * 4 + 0] * (lightMultiply & 0xff)) / 255);
                    Rgba[RgbaCount + 1] = (byte)((data.Rgba[i * 4 + 1] * ((lightMultiply >> 8) & 0xff)) / 255);
                    Rgba[RgbaCount + 2] = (byte)((data.Rgba[i * 4 + 2] * ((lightMultiply >> 16) & 0xff)) / 255);
                    Rgba[RgbaCount + 3] = (byte)((data.Rgba[i * 4 + 3] * ((lightMultiply >> 24) & 0xff)) / 255);
                }

                /*if (Rgba2 != null)
                {
                    Rgba2[RgbaCount + 0] = (byte)((data.Rgba[i * 4 + 0] * (lightMultiply2 & 0xff)) / 255);
                    Rgba2[RgbaCount + 1] = (byte)((data.Rgba[i * 4 + 1] * ((lightMultiply2 >> 8) & 0xff)) / 255);
                    Rgba2[RgbaCount + 2] = (byte)((data.Rgba[i * 4 + 2] * ((lightMultiply2 >> 16) & 0xff)) / 255);
                    Rgba2[RgbaCount + 3] = (byte)((data.Rgba[i * 4 + 3] * ((lightMultiply2 >> 24) & 0xff)) / 255);
                }*/

                if (Flags != null)
                {
                    Flags[FlagsCount] = data.Flags[i];
                }

                if (CustomInts != null && data.CustomInts != null)
                {
                    for (int j = 0; j < data.CustomInts.Count; j++)
                    {
                        CustomInts.Add(data.CustomInts.Values[j]);
                    }
                }

                if (CustomFloats != null && data.CustomFloats != null)
                {
                    for (int j = 0; j < data.CustomFloats.Count; j++)
                    {
                        CustomFloats.Add(data.CustomFloats.Values[j]);
                    }
                }

                if (CustomShorts != null && data.CustomShorts != null)
                {
                    for (int j = 0; j < data.CustomShorts.Count; j++)
                    {
                        CustomShorts.Add(data.CustomShorts.Values[j]);
                    }
                }

                if (CustomBytes != null && data.CustomBytes != null)
                {
                    for (int j = 0; j < data.CustomBytes.Count; j++)
                    {
                        CustomBytes.Add(data.CustomBytes.Values[j]);
                    }
                }

                VerticesCount++;
            }


            for (int i = 0; i < data.IndicesCount; i++)
            {
                AddIndex(lastelement + data.Indices[i]);
            }

            for (int i = 0; i < data.XyzFacesCount; i++)
            {
                AddXyzFace(data.XyzFaces[i]);
            }

            for (int i = 0; i < data.ColorMapIdsCount; i++)
            {
                AddColorMapIndex(data.ClimateColorMapIds[i], data.SeasonColorMapIds[i]);
            }
        }

        /// <summary>
        /// Removes the last index in the indices array
        /// </summary>
        public void RemoveIndex()
        {
            if (VerticesCount > 0) VerticesCount--;
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
        public void AddVertex(float x, float y, float z, int color)
        {
            if (VerticesCount >= VerticesMax)
            {
                GrowVertexBuffer();
            }

            xyz[XyzCount + 0] = x;
            xyz[XyzCount + 1] = y;
            xyz[XyzCount + 2] = z;
            

            // Write int color into byte array
            unsafe
            {
                fixed (byte* rgbaByte = Rgba)
                {
                    int* rgbaInt = (int*)rgbaByte;
                    rgbaInt[RgbaCount / 4] = color;
                }
            }

            VerticesCount++;
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
        /// <param name="flags"></param>
        public void AddWithFlagsVertex(float x, float y, float z, float u, float v, int color, int flags)
        {
            if (VerticesCount >= VerticesMax)
            {
                GrowVertexBuffer();
            }

            xyz[XyzCount + 0] = x;
            xyz[XyzCount + 1] = y;
            xyz[XyzCount + 2] = z;
            

            Uv[UvCount + 0] = u;
            Uv[UvCount + 1] = v;

            if (this.Rgba != null)
            {
                // Write int color into byte array
                unsafe
                {
                    fixed (byte* rgbaByte = Rgba)
                    {
                        int* rgbaInt = (int*)rgbaByte;
                        rgbaInt[RgbaCount / 4] = color;
                    }
                }
            }

            if (this.Flags != null)
            {
                this.Flags[FlagsCount] = flags;
            }

            VerticesCount++;
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
        /// <param name="color2"></param>
        /// <param name="flags"></param>
        public void AddVertexWithFlags(float x, float y, float z, float u, float v, int color, int flags)
        {
            if (VerticesCount >= VerticesMax)
            {
                GrowVertexBuffer();
            }

            xyz[XyzCount + 0] = x;
            xyz[XyzCount + 1] = y;
            xyz[XyzCount + 2] = z;

            Uv[UvCount + 0] = u;
            Uv[UvCount + 1] = v;

            if (this.Flags != null)
            {
                this.Flags[FlagsCount] = flags;
            }


            // Write int color into byte array
            unsafe
            {
                fixed (byte* rgbaByte = Rgba)
                {
                    int* rgbaInt = (int*)rgbaByte;
                    rgbaInt[RgbaCount / 4] = color;
                }
            }


            // Write int color into byte array
            /*unsafe
            {
                fixed (byte* rgbaByte2 = Rgba2)
                {
                    int* rgbaInt2 = (int*)rgbaByte2;
                    rgbaInt2[Rgba2Count / 4] = color2;
                }
            }*/

            VerticesCount++;
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
            if (VerticesCount >= VerticesMax)
            {
                GrowVertexBuffer();
            }

            xyz[XyzCount + 0] = x;
            xyz[XyzCount + 1] = y;
            xyz[XyzCount + 2] = z;
            

            Uv[UvCount + 0] = u;
            Uv[UvCount + 1] = v;

            VerticesCount++;
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
            if (VerticesCount >= VerticesMax)
            {
                GrowVertexBuffer();
            }

            xyz[XyzCount + 0] = x;
            xyz[XyzCount + 1] = y;
            xyz[XyzCount + 2] = z;

            Uv[UvCount + 0] = u;
            Uv[UvCount + 1] = v;

            Rgba[RgbaCount + 0] = color[0];
            Rgba[RgbaCount + 1] = color[1];
            Rgba[RgbaCount + 2] = color[2];
            Rgba[RgbaCount + 3] = color[3];

            VerticesCount++;
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
        /// <param name="color2"></param>
        /*public void AddVertex(float x, float y, float z, float u, float v, byte[] color, byte[] color2)
        {
            if (VerticesCount >= VerticesMax)
            {
                GrowVertexBuffer();
            }

            xyz[XyzCount + 0] = x;
            xyz[XyzCount + 1] = y;
            xyz[XyzCount + 2] = z;
            

            Uv[UvCount + 0] = u;
            Uv[UvCount + 1] = v;

            Rgba[RgbaCount + 0] = color[0];
            Rgba[RgbaCount + 1] = color[1];
            Rgba[RgbaCount + 2] = color[2];
            Rgba[RgbaCount + 3] = color[3];

            Rgba2[Rgba2Count + 0] = color2[0];
            Rgba2[Rgba2Count + 1] = color2[1];
            Rgba2[Rgba2Count + 2] = color2[2];
            Rgba2[Rgba2Count + 3] = color2[3];

            Flags[FlagsCount] = 0;

            VerticesCount++;
        }*/


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

            Normals[NormalsCount++] = NormalUtil.PackNormal(facing.Normalf.X, facing.Normalf.Y, facing.Normalf.Z);
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
            if (RenderPassCount >= RenderPasses.Length)
            {
                Array.Resize(ref RenderPasses, RenderPasses.Length + 32);
            }

            RenderPasses[RenderPassCount++] = renderPass;
        }


        public void AddXyzFace(byte faceIndex)
        {
            if (XyzFacesCount >= XyzFaces.Length)
            {
                Array.Resize(ref XyzFaces, XyzFaces.Length + 32);
            }

            XyzFaces[XyzFacesCount++] = faceIndex;
        }

        public void AddIndex(int index)
        {
            if (IndicesCount >= IndicesMax)
            {
                GrowIndexBuffer();
            }

            Indices[IndicesCount++] = index;
        }

        public void AddIndices(int[] indices)
        {
            for (int i = 0; i < indices.Length; i++)
            {
                if (IndicesCount >= IndicesMax)
                {
                    GrowIndexBuffer();
                }

                this.Indices[IndicesCount++] = indices[i];
            }
        }

        public void GrowIndexBuffer()
        {
            int[] largerIndices = new int[IndicesCount * 2];
            for (int i = 0; i < IndicesCount; i++)
            {
                largerIndices[i] = Indices[i];
            }

            IndicesMax = IndicesMax * 2;

            Indices = largerIndices;
        }


        public void GrowIndexBuffer(int byAtLeastQuantity)
        {
            int newSize = Math.Max(IndicesCount * 2, IndicesCount + byAtLeastQuantity);
            int[] largerIndices = new int[newSize];
            for (int i = 0; i < Indices.Length; i++)
            {
                largerIndices[i] = Indices[i];
            }

            IndicesMax = newSize;

            Indices = largerIndices;
        }



        public void GrowNormalsBuffer()
        {
            if (Normals != null)
            {
                int[] largerNormals = new int[Normals.Length * 2];
                for (int i = 0; i < Normals.Length; i++)
                {
                    largerNormals[i] = Normals[i];
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
                for (int i = 0; i < xyz.Length; i++)
                {
                    largerXyz[i] = xyz[i];
                }
                xyz = largerXyz;
            }

           

            if (Uv != null)
            {
                int uvCount = UvCount;
                float[] largerUv = new float[uvCount * 2];
                for (int i = 0; i < Uv.Length; i++)
                {
                    largerUv[i] = Uv[i];
                }
                Uv = largerUv;
            }

            if (Rgba != null)
            {
                int rgbaCount = RgbaCount;
                byte[] largerRgba = new byte[rgbaCount * 2];
                for (int i = 0; i < Rgba.Length; i++)
                {
                    largerRgba[i] = Rgba[i];
                }
                Rgba = largerRgba;
            }

            /*if (Rgba2 != null)
            {
                int rgba2Count = Rgba2Count;
                byte[] largerRgba2 = new byte[rgba2Count * 2];
                for (int i = 0; i < Rgba2.Length; i++)
                {
                    largerRgba2[i] = Rgba2[i];
                }
                Rgba2 = largerRgba2;
            }*/

            if (Flags != null)
            {
                int flagsCount = FlagsCount;
                int[] largerFlags = new int[flagsCount * 2];
                for (int i = 0; i < Flags.Length; i++)
                {
                    largerFlags[i] = Flags[i];
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
                for (int i = 0; i < cnt; i++)
                {
                    tightXyz[i] = xyz[i];
                }
                xyz = tightXyz;
            }

            if (Uv != null)
            {
                int cnt = UvCount;
                float[] tightUv = new float[cnt + 1];
                for (int i = 0; i < cnt; i++)
                {
                    tightUv[i] = Uv[i];
                }
                Uv = tightUv;
            }

            if (Rgba != null)
            {
                int cnt = RgbaCount;
                byte[] tightRgba = new byte[cnt + 1];
                for (int i = 0; i < cnt; i++)
                {
                    tightRgba[i] = Rgba[i];
                }
                Rgba = tightRgba;
            }

            /*if (Rgba2 != null)
            {
                int cnt = Rgba2Count;
                byte[] tightRgba2 = new byte[cnt + 1];
                for (int i = 0; i < cnt; i++)
                {
                    tightRgba2[i] = Rgba2[i];
                }
                Rgba2 = tightRgba2;
            }*/

            if (Flags != null)
            {
                int cnt = FlagsCount;
                int[] tightFlags = new int[cnt + 1];
                for (int i = 0; i < cnt; i++)
                {
                    tightFlags[i] = Flags[i];
                }
                Flags = tightFlags;
            }

            VerticesMax = VerticesCount;
        }

        /// <summary>
        /// Creates a deep copy of the mesh
        /// </summary>
        /// <returns></returns>
        public MeshData Clone()
        {
            MeshData dest = new MeshData();
            unchecked
            {
                dest.xyz = new float[XyzCount];
                for (int i = 0; i < XyzCount; i++)
                {
                    dest.xyz[i] = xyz[i];
                }

                if (Normals != null)
                {
                    dest.Normals = new int[Normals.Length];
                    for (int i = 0; i < Normals.Length; i++)
                    {
                        dest.Normals[i] = Normals[i];
                    }
                }

                if (XyzFaces != null)
                {
                    dest.XyzFaces = new byte[XyzFaces.Length];
                    for (int i = 0; i < XyzFaces.Length; i++)
                    {
                        dest.XyzFaces[i] = XyzFaces[i];
                    }
                    dest.XyzFacesCount = XyzFacesCount;

                }


                if (ClimateColorMapIds != null)
                {
                    dest.ClimateColorMapIds = new byte[ClimateColorMapIds.Length];
                    for (int i = 0; i < ClimateColorMapIds.Length; i++)
                    {
                        dest.ClimateColorMapIds[i] = ClimateColorMapIds[i];
                    }
                    dest.ColorMapIdsCount = ColorMapIdsCount;
                }

                if (SeasonColorMapIds != null)
                {
                    dest.SeasonColorMapIds = new byte[SeasonColorMapIds.Length];
                    for (int i = 0; i < SeasonColorMapIds.Length; i++)
                    {
                        dest.SeasonColorMapIds[i] = SeasonColorMapIds[i];
                    }
                    dest.ColorMapIdsCount = ColorMapIdsCount;
                }

                if (RenderPasses != null)
                {
                    dest.RenderPasses = new short[RenderPasses.Length];
                    for (int i = 0; i < RenderPasses.Length; i++)
                    {
                        dest.RenderPasses[i] = RenderPasses[i];
                    }
                    dest.RenderPassCount = RenderPassCount;
                }


                if (Uv != null)
                {
                    dest.Uv = new float[UvCount];
                    for (int i = 0; i < UvCount; i++)
                    {
                        dest.Uv[i] = Uv[i];
                    }
                }

                if (Rgba != null)
                {
                    dest.Rgba = new byte[RgbaCount];
                    for (int i = 0; i < RgbaCount; i++)
                    {
                        dest.Rgba[i] = Rgba[i];
                    }
                }
                

                /*
                {
                    dest.Rgba2 = new byte[Rgba2Count];
                    for (int i = 0; i < Rgba2Count; i++)
                    {
                        dest.Rgba2[i] = Rgba2[i];
                    }
                }*/

                if (Flags != null)
                {
                    dest.Flags = new int[FlagsCount];
                    for (int i = 0; i < FlagsCount; i++)
                    {
                        dest.Flags[i] = Flags[i];
                    }
                }


                dest.Indices = new int[GetIndicesCount()];
                for (int i = 0; i < GetIndicesCount(); i++)
                {
                    dest.Indices[i] = Indices[i];
                }
                dest.SetVerticesCount(GetVerticesCount());
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

                dest.VerticesMax = VerticesMax;
                dest.IndicesMax = IndicesMax;
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
                //(Rgba2 == null ? 0 : Rgba2.Length) +
                (ClimateColorMapIds == null ? 0 : ClimateColorMapIds.Length * 1) +
                (SeasonColorMapIds == null ? 0 : SeasonColorMapIds.Length * 1) +
                (XyzFaces == null ? 0 : XyzFaces.Length * 1) +
                (RenderPasses == null ? 0 : RenderPasses.Length * 2) +
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
        /// Sets the uvs of this mesh to the specified TextureAtlasPosition
        /// </summary>
        public void SetTexPos(TextureAtlasPosition texPos)
        {
            for (int i = 0; i < this.Uv.Length; i++)
            {
                float x = texPos.x2 - texPos.x1;
                float y = texPos.y2 - texPos.y1;

                this.Uv[i] = i % 2 == 0 ? (this.Uv[i] * x) + texPos.x1 : (this.Uv[i] * y) + texPos.y1;
            }
        }
    }
    
}
