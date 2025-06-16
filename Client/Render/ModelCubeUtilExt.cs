using System;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{

    public class ModelCubeUtilExt : CubeMeshUtil
    {

        public enum EnumShadeMode
        {
            Off,
            On,
            Gradient
        }

        static int[] gradientNormalMixedFlags = new int[6];

        static ModelCubeUtilExt()
        {
            for (int i = 0; i < 6; i++)
            {
                var vec = new Vec3f(0, 1, 0).Mul(0.33f) + BlockFacing.ALLFACES[i].Normalf.Clone().Mul(0.66f);
                vec.Normalize();
                gradientNormalMixedFlags[i] = VertexFlags.PackNormal(vec);
            }
        }

        public static void AddFace(
            MeshData modeldata, BlockFacing face, Vec3f centerXyz, Vec3f sizeXyz, Vec2f originUv, Vec2f sizeUv, int textureId, int color, EnumShadeMode shade, int[] vertexFlags,
            float brightness = 1f, int uvRotation = 0, byte climateColorMapId = 0, byte seasonColorMapId = 0, short renderPass = -1)
        {
            int coordPos = face.Index * 12; // 4 * 3 xyz's perface
            int uvPos = face.Index * 8;     // 4 * 2 uvs per face
            int lastVertexNumber = modeldata.VerticesCount;

            int col = ColorUtil.ColorMultiply3(color, brightness);

            if (shade == EnumShadeMode.Gradient)
            {
                float half = sizeXyz.Y / 2f;
                int normalUpFlags = BlockFacing.UP.NormalPackedFlags;
                int normalDownFlags = gradientNormalMixedFlags[face.Index];

                // 4 vertices per face
                for (int i = 0; i < 4; i++)
                {
                    float x = centerXyz.X + sizeXyz.X * CubeVertices[coordPos++] / 2;
                    float y = centerXyz.Y + sizeXyz.Y * CubeVertices[coordPos++] / 2;

                    int uvIndex = 2 * ((uvRotation + i) % 4) + uvPos;
                    modeldata.AddWithFlagsVertex(
                        x,
                        y,
                        centerXyz.Z + sizeXyz.Z * CubeVertices[coordPos++] / 2,
                        originUv.X + sizeUv.X * CubeUvCoords[uvIndex],
                        originUv.Y + sizeUv.Y * CubeUvCoords[uvIndex + 1],
                        col,
                        vertexFlags[i] | (y > half ? normalUpFlags : normalDownFlags)
                    );
                }
            }
            else
            {
                // 4 vertices per face
                for (int i = 0; i < 4; i++)
                {
                    int uvIndex = 2 * ((uvRotation + i) % 4) + uvPos;
                    modeldata.AddWithFlagsVertex(
                        centerXyz.X + sizeXyz.X * CubeVertices[coordPos++] / 2,
                        centerXyz.Y + sizeXyz.Y * CubeVertices[coordPos++] / 2,
                        centerXyz.Z + sizeXyz.Z * CubeVertices[coordPos++] / 2,
                        originUv.X + sizeUv.X * CubeUvCoords[uvIndex],
                        originUv.Y + sizeUv.Y * CubeUvCoords[uvIndex + 1],
                        col,
                        vertexFlags[i]
                    );
                }
            }




            // 2 triangles = 6 indices per face
            modeldata.AddIndex(lastVertexNumber + 0);
            modeldata.AddIndex(lastVertexNumber + 1);
            modeldata.AddIndex(lastVertexNumber + 2);
            modeldata.AddIndex(lastVertexNumber + 0);
            modeldata.AddIndex(lastVertexNumber + 2);
            modeldata.AddIndex(lastVertexNumber + 3);

            if (modeldata.XyzFacesCount >= modeldata.XyzFaces.Length)
            {
                Array.Resize(ref modeldata.XyzFaces, modeldata.XyzFaces.Length + 32);
            }
            if (modeldata.TextureIndicesCount >= modeldata.TextureIndices.Length)
            {
                Array.Resize(ref modeldata.TextureIndices, modeldata.TextureIndices.Length + 32);
            }

            modeldata.TextureIndices[modeldata.TextureIndicesCount++] = modeldata.getTextureIndex(textureId);
            modeldata.XyzFaces[modeldata.XyzFacesCount++] = shade != EnumShadeMode.Off ? face.MeshDataIndex : (byte)0;


            if (modeldata.ClimateColorMapIds != null)
            {
                if (modeldata.ColorMapIdsCount >= modeldata.ClimateColorMapIds.Length)
                {
                    Array.Resize(ref modeldata.ClimateColorMapIds, modeldata.ClimateColorMapIds.Length + 32);
                    Array.Resize(ref modeldata.SeasonColorMapIds, modeldata.SeasonColorMapIds.Length + 32);
                }

                modeldata.ClimateColorMapIds[modeldata.ColorMapIdsCount] = climateColorMapId;
                modeldata.SeasonColorMapIds[modeldata.ColorMapIdsCount++] = seasonColorMapId;
            }

            if (modeldata.RenderPassesAndExtraBits != null)
            {
                if (modeldata.RenderPassCount >= modeldata.RenderPassesAndExtraBits.Length)
                {
                    Array.Resize(ref modeldata.RenderPassesAndExtraBits, modeldata.RenderPassesAndExtraBits.Length + 32);
                }

                modeldata.RenderPassesAndExtraBits[modeldata.RenderPassCount++] = renderPass;
            }
        }







        public static void AddFaceSkipTex(MeshData modeldata, BlockFacing face, Vec3f centerXyz, Vec3f sizeXyz, int color, float brightness = 1f)
        {
            int coordPos = face.Index * 12; // 4 * 3 xyz's perface
            int lastVertexNumber = modeldata.VerticesCount;

            // 4 vertices per face
            for (int i = 0; i < 4; i++)
            {
                float[] pos = new float[]
                {
                    centerXyz.X + sizeXyz.X * CubeVertices[coordPos++] / 2,
                    centerXyz.Y + sizeXyz.Y * CubeVertices[coordPos++] / 2,
                    centerXyz.Z + sizeXyz.Z * CubeVertices[coordPos++] / 2
                };

                modeldata.AddVertexSkipTex(
                    pos[0], pos[1], pos[2],
                    ColorUtil.ColorMultiply3(color, brightness)
                );
            }

            // 2 triangles = 6 indices per face
            modeldata.AddIndex(lastVertexNumber + 0);
            modeldata.AddIndex(lastVertexNumber + 1);
            modeldata.AddIndex(lastVertexNumber + 2);
            modeldata.AddIndex(lastVertexNumber + 0);
            modeldata.AddIndex(lastVertexNumber + 2);
            modeldata.AddIndex(lastVertexNumber + 3);
        }
    }


}