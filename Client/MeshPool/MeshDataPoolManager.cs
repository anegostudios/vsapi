using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Holds a collection of pools, usually for 1 render pass
    /// </summary>
    public class MeshDataPoolManager
    {
        List<MeshDataPool> pools = new List<MeshDataPool>();

        internal FrustumCulling frustumCuller;
        ICoreClientAPI capi;

        MeshDataPoolMasterManager masterPool;
        CustomMeshDataPartFloat customFloats = null;
        CustomMeshDataPartShort customShorts = null;
        CustomMeshDataPartByte customBytes = null;
        CustomMeshDataPartInt customInts = null;

        int defaultVertexPoolSize;
        int defaultIndexPoolSize;
        int maxPartsPerPool;

        Vec3f tmp = new Vec3f();

        /// <summary>
        /// Creates a new Mesh Data Pool
        /// </summary>
        /// <param name="masterPool">The master mesh data pool manager</param>
        /// <param name="frustumCuller">the Frustum Culler for the Pool</param>
        /// <param name="capi">The Client API</param>
        /// <param name="defaultVertexPoolSize">Size allocated for the Vertices.</param>
        /// <param name="defaultIndexPoolSize">Size allocated for the Indices</param>
        /// <param name="maxPartsPerPool">The maximum number of parts for this pool.</param>
        /// <param name="customFloats">Additional float data</param>
        /// <param name="customShorts"></param>
        /// <param name="customBytes">Additional byte data</param>
        /// <param name="customInts">additional int data</param>
        public MeshDataPoolManager(MeshDataPoolMasterManager masterPool, FrustumCulling frustumCuller, ICoreClientAPI capi, int defaultVertexPoolSize, int defaultIndexPoolSize, int maxPartsPerPool, CustomMeshDataPartFloat customFloats = null, CustomMeshDataPartShort customShorts = null, CustomMeshDataPartByte customBytes = null, CustomMeshDataPartInt customInts = null)
        {
            this.masterPool = masterPool;
            this.frustumCuller = frustumCuller;
            this.capi = capi;
            this.customFloats = customFloats;
            this.customBytes = customBytes;
            this.customInts = customInts;
            this.customShorts = customShorts;
            this.defaultIndexPoolSize = defaultIndexPoolSize;
            this.defaultVertexPoolSize = defaultVertexPoolSize;
            this.maxPartsPerPool = maxPartsPerPool;
        }

        /// <summary>
        /// Adds a model to the mesh pool.
        /// </summary>
        /// <param name="modeldata">The model data</param>
        /// <param name="modelOrigin">The origin point of the Model</param>
        /// <param name="dimension"></param>
        /// <param name="frustumCullSphere">The culling sphere.</param>
        /// <returns>The location identifier for the pooled model.</returns>
        public ModelDataPoolLocation AddModel(MeshData modeldata, Vec3i modelOrigin, int dimension, Sphere frustumCullSphere)
        {
            ModelDataPoolLocation location = null;

            for (int i = 0; i < pools.Count; i++)
            {
                location = pools[i].TryAdd(capi, modeldata, modelOrigin, dimension, frustumCullSphere);
                if (location != null) break;
            }

            if (location == null)
            {
                int vertexSize = Math.Max(modeldata.VerticesCount, defaultVertexPoolSize);
                int indexSize = Math.Max(modeldata.IndicesCount, defaultIndexPoolSize);

                if (vertexSize > defaultVertexPoolSize)
                {
                    capi.World.Logger.Warning("Chunk (or some other mesh source at origin: {0}) exceeds default geometric complexity maximum of {1} vertices and {2} indices. You must be loading some very complex objects (#v = {3}, #i = {4}). Adjusted Pool size accordingly.", modelOrigin, defaultVertexPoolSize, defaultIndexPoolSize, modeldata.VerticesCount, modeldata.IndicesCount);
                }

                MeshDataPool pool = MeshDataPool.AllocateNewPool(capi, vertexSize, indexSize, maxPartsPerPool, customFloats, customShorts, customBytes, customInts);
                pool.poolOrigin = modelOrigin.Clone();
                pool.dimensionId = dimension;

                masterPool.AddModelDataPool(pool);
                pools.Add(pool);
                location = pool.TryAdd(capi, modeldata, modelOrigin, dimension, frustumCullSphere);
            }

            if (location == null)
            {
                capi.World.Logger.Fatal("Can't add modeldata (probably a tesselated chunk @{0}) to the model data pool list, blocks will likely be invisible. Potential reasons are the parts per pool were exceeded, or other code reasons, please report this. Default pool size is {1} vertices and {2} indices. Mesh size (#v = {3}, #i = {4}). Try increasing MaxVertexSize and MaxIndexSize.", modelOrigin, defaultVertexPoolSize, defaultIndexPoolSize, modeldata.VerticesCount, modeldata.IndicesCount);
            }

            return location;
        }


        /// <summary>
        /// Renders the chunk models to the GPU.  One of the most important methods in the entire game!
        /// </summary>
        /// <param name="playerpos">The position of the Player</param>
        /// <param name="originUniformName"></param>
        /// <param name="frustumCullMode">The culling mode.  Default is CulHideDelay.</param>
        public void Render(Vec3d playerpos, string originUniformName, EnumFrustumCullMode frustumCullMode = EnumFrustumCullMode.CullNormal)
        {
            int count = pools.Count;
            for (int i = 0; i < count; i++)
            {
                MeshDataPool pool = pools[i];
                if (pool.dimensionId == Dimensions.MiniDimensions)
                {
                    bool revertModelviewMatrix = false;
                    bool revertMVPMatrix = false;
                    bool revertTransparency = false;

                    // Special code for movable chunks
                    if (!capi.World.TryGetMiniDimension(pool.poolOrigin, out IMiniDimension dimension) || dimension.selectionTrackingOriginalPos == null) continue;

                    pool.SetFullyVisible();
                    if (pool.indicesGroupsCount == 0)
                    {
                        continue;
                    }

                    FastVec3d renderOffset = dimension.GetRenderOffset(masterPool.currentDt);

                    var currShader = capi.Render.CurrentActiveShader;
                    if (currShader.HasUniform("modelViewMatrix"))
                    {
                        currShader.UniformMatrix("modelViewMatrix", dimension.GetRenderTransformMatrix(masterPool.currentModelViewMatrix, playerpos));
                        revertModelviewMatrix = true;
                        if (currShader.HasUniform("forcedTransparency"))
                        {
                            currShader.Uniform("forcedTransparency", capi.Settings.Float["previewTransparency"]);
                            revertTransparency = true;
                        }
                    }
                    else if (currShader.HasUniform("mvpMatrix"))
                    {
                        currShader.UniformMatrix("mvpMatrix", dimension.GetRenderTransformMatrix(masterPool.shadowMVPMatrix, playerpos));
                        revertMVPMatrix = true;
                    }

                    currShader.Uniform(originUniformName, tmp.Set(
                        (float)(pool.poolOrigin.X + renderOffset.X - playerpos.X),
                        (float)(pool.poolOrigin.Y + renderOffset.Y - playerpos.Y),
                        (float)(pool.poolOrigin.Z + renderOffset.Z - playerpos.Z)
                    ));

                    try
                    {
                        pool.RenderMesh(capi.Render);
                    }
                    finally
                    {
                        if (revertModelviewMatrix)
                            capi.Render.CurrentActiveShader.UniformMatrix("modelViewMatrix", masterPool.currentModelViewMatrix);
                        if (revertMVPMatrix)
                            capi.Render.CurrentActiveShader.UniformMatrix("mvpMatrix", masterPool.shadowMVPMatrix);
                        if (revertTransparency)
                            capi.Render.CurrentActiveShader.Uniform("forcedTransparency", 0f);
                    }
                }
                else
                {
                    pool.FrustumCull(frustumCuller, frustumCullMode);

                    if (pool.indicesGroupsCount == 0)
                    {
                        continue;
                    }

                    capi.Render.CurrentActiveShader.Uniform(originUniformName, tmp.Set(
                        (float)(pool.poolOrigin.X - playerpos.X),
                        (float)(pool.poolOrigin.Y + pool.dimensionId * BlockPos.DimensionBoundary - playerpos.Y),
                        (float)(pool.poolOrigin.Z - playerpos.Z)
                    ));

                    pool.RenderMesh(capi.Render);
                }
            }
        }

        /// <summary>
        /// Gets the stats of the model.
        /// </summary>
        /// <param name="usedVideoMemory">The amount of memory used by this pool.</param>
        /// <param name="renderedTris">The number of Tris rendered by this pool.</param>
        /// <param name="allocatedTris">The number of tris allocated by this pool.</param>
        public void GetStats(ref long usedVideoMemory, ref long renderedTris, ref long allocatedTris)
        {
            // 1 vertex =  (without SSBOs)
            // - 3 xyz floats  = 12 byte
            // - 2 uv floats   = 8 byte
            // - 4 rgba bytes  = 4 byte
            // - 1 flags int    = 4 byte
            // - 1 custom int (colormapData) = 4 byte

            // Optionally, for liquids
            // - 2 custom floats (flowvector)  = 8 byte    when customdata is there e.g. for liquids
            // - 1 custom int (waterflags or uv2)  = 4 byte    when customdata is there e.g. for liquids or topsoil

            // Total vertex: 32 bytes plus optional custom data
            // Total indices: 6 bytes  (for every 4 vertices, 6 indices of 4 bytes each)

            // So 10mil tris => 30mil indices, 20mil vertices
            // = 114mb indices, 610mb vertices => 724mb video memory   (ignoring custom data)
            //    (Most vertices also have at least some custom data)

            // If we use SSBOs, video memory per vertex (base, no custom data) reduces from 32 bytes to 20 bytes, assuming 4 bytes of CustomInts per vertex for the colormapData
            // and we use 0 bytes for indices  (apart from the single standard index buffer)
            // so overall, 38 bytes per vertex became 20 bytes per vertex, which is quite good.

            long vertexSize = (capi.Render.UseSSBOs ? 16 + 4 - 4 : 12 + 8 + 4 + 4) + (customFloats == null ? 0 : customFloats.InterleaveStride) + (customShorts == null ? 0 : customShorts.InterleaveStride) + (customBytes == null ? 0 : customBytes.InterleaveStride) + (customInts == null ? 0 : customInts.InterleaveStride);
            int indexSize = capi.Render.UseSSBOs ? 0 : 4;

            int maxIndices = 0;
            for (int i = 0; i < pools.Count; i++)
            {
                MeshDataPool pool = pools[i];

                usedVideoMemory += pool.VerticesPoolSize * vertexSize + pool.IndicesPoolSize * indexSize;
                renderedTris += pool.RenderedTriangles;
                allocatedTris += pool.AllocatedTris;

                if (pool.IndicesPoolSize > maxIndices) maxIndices = pool.IndicesPoolSize;
            }

            if (capi.Render.UseSSBOs)
            {
                usedVideoMemory += maxIndices * 4;
            }
        }
    }


}
