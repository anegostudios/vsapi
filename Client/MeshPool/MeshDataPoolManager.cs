using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Holds a set of pools, usually for one render pass
    /// A new mesh is first offered to the existing pools and if none of them takes it, a new pool is created
    /// </summary>
    public class MeshDataPoolManager
    {
        // Pools of this manager They are searched when adding and they are drawn in Render
        List<MeshDataPool> pools = [];

        internal FrustumCulling frustumCuller;
        ICoreClientAPI capi;

        MeshDataPoolMasterManager masterPool;

        // Extra data attached to every vertex (colors, water and so on)
        CustomMeshDataPartFloat customFloats = null;
        CustomMeshDataPartShort customShorts = null;
        CustomMeshDataPartByte customBytes = null;
        CustomMeshDataPartInt customInts = null;

        int defaultVertexPoolSize;
        int defaultIndexPoolSize;
        int maxPartsPerPool;

        Vec3f tmp = new();   // temp vector for the pool origin uniform

        /// <summary>
        /// Creates a new pool manager
        /// </summary>
        /// <param name="masterPool">The master pool manager these pools belong to</param>
        /// <param name="frustumCuller">The frustum culler for the pool</param>
        /// <param name="capi">The client API</param>
        /// <param name="defaultVertexPoolSize">Default vertex buffer size</param>
        /// <param name="defaultIndexPoolSize">Default index buffer size</param>
        /// <param name="maxPartsPerPool">Maximum number of parts in one pool</param>
        /// <param name="customFloats">Extra float data</param>
        /// <param name="customShorts">Extra short data</param>
        /// <param name="customBytes">Extra byte data</param>
        /// <param name="customInts">Extra int data</param>
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
        /// Adds a model to the pool It first tries every existing pool and if none has room, a new pool is created
        /// </summary>
        /// <param name="modeldata">The model data</param>
        /// <param name="modelOrigin">Model origin in the world</param>
        /// <param name="dimension">Dimension (normal world or mini dimension)</param>
        /// <param name="frustumCullSphere">The sphere used for culling</param>
        /// <returns>A handle to the place of the model in the pool</returns>
        public ModelDataPoolLocation AddModel(MeshData modeldata, Vec3i modelOrigin, int dimension, Sphere frustumCullSphere)
        {
            ModelDataPoolLocation location = null;

            // Look for a pool that has room for the model
            for (int i = 0; i < pools.Count; i++)
            {
                location = pools[i].TryAdd(capi, modeldata, modelOrigin, dimension, frustumCullSphere);
                if (location != null) break;
            }

            // None accepted it so create a new pool sized for this model
            if (location == null)
            {
                int vertexSize = Math.Max(modeldata.VerticesCount, defaultVertexPoolSize);
                int indexSize = Math.Max(modeldata.IndicesCount, defaultIndexPoolSize);

                if (vertexSize > defaultVertexPoolSize)
                {
                    capi.World.Logger.Warning("Chunk (or some other mesh source at origin: {0}) exceeds default geometric complexity maximum of {1} vertices and {2} indices. You must be loading some very complex objects (#v = {3}, #i = {4}). Adjusted Pool size accordingly.", modelOrigin, defaultVertexPoolSize, defaultIndexPoolSize, modeldata.VerticesCount, modeldata.IndicesCount);
                }

                MeshDataPool pool = MeshDataPool.AllocateNewPool(capi, vertexSize, indexSize, maxPartsPerPool, customFloats, customShorts, customBytes, customInts);

                pool.dimensionId = dimension;

                masterPool.AddModelDataPool(pool);
                pools.Add(pool);
                location = pool.TryAdd(capi, modeldata, modelOrigin, dimension, frustumCullSphere);
            }

            // It did not fit even into the new pool so this is an error and goes to the log
            if (location == null)
            {
                capi.World.Logger.Fatal("Can't add modeldata (probably a tesselated chunk @{0}) to the model data pool list, blocks will likely be invisible. Potential reasons are the parts per pool were exceeded, or other code reasons, please report this. Default pool size is {1} vertices and {2} indices. Mesh size (#v = {3}, #i = {4}). Try increasing MaxVertexSize and MaxIndexSize.", modelOrigin, defaultVertexPoolSize, defaultIndexPoolSize, modeldata.VerticesCount, modeldata.IndicesCount);
            }

            return location;
        }


        /// <summary>
        /// Draws the chunk models on the GPU One of the most important methods in the whole game
        /// </summary>
        /// <param name="playerpos">Player position</param>
        /// <param name="originUniformName">Name of the uniform that receives the pool origin</param>
        /// <param name="frustumCullMode">Culling mode, CullNormal by default</param>
        public void Render(Vec3d playerpos, string originUniformName, EnumFrustumCullMode frustumCullMode = EnumFrustumCullMode.CullNormal)
        {
            int count = pools.Count;
            for (int i = 0; i < count; i++)
            {
                MeshDataPool pool = pools[i];

                // Mini dimensions (moving world pieces) go through a separate path
                if (pool.dimensionId == Dimensions.MiniDimensions)
                {
                    RenderMiniDimensionPool(pool, playerpos, originUniformName);
                    continue;
                }
                else
                {
                    // Pick the visible parts using the rules of the selected mode
                    pool.FrustumCull(frustumCuller, frustumCullMode);

                    // Nothing is visible so the whole pool can be skipped
                    if (pool.indicesGroupsCount == 0)
                    {
                        continue;
                    }

                    // Tell the shader where the pool sits relative to the player
                    capi.Render.CurrentActiveShader.Uniform(originUniformName, tmp.Set(
                        (float)(pool.PoolOrigin.X - playerpos.X),
                        (float)(pool.PoolOrigin.Y + pool.dimensionId * BlockPos.DimensionBoundary - playerpos.Y),
                        (float)(pool.PoolOrigin.Z - playerpos.Z)
                    ));

                    pool.RenderMesh(capi.Render);
                }
            }
        }

        /// <summary>
        /// Draws a mini dimension pool (moving chunks) Unlike regular pools it skips frustum culling,
        /// draws everything through SetFullyVisible and pushes its own transform and origin into the shader
        /// </summary>
        private void RenderMiniDimensionPool(MeshDataPool pool, Vec3d playerpos, string originUniformName)
        {
            // Special handling for moving chunks
            if (!capi.World.TryGetMiniDimension(pool.PoolOrigin, out IMiniDimension dimension) || dimension.selectionTrackingOriginalPos == null) return;

            // No culling: draw everything that is not hidden
            pool.SetFullyVisible();
            if (pool.indicesGroupsCount == 0)
            {
                return;
            }

            // Offset by which the mini dimension moved this frame
            FastVec3d renderOffset = dimension.GetRenderOffset(masterPool.currentDt);

            // Remember what was changed so it can be restored
            bool revertModelviewMatrix = false;
            bool revertMVPMatrix = false;
            bool revertTransparency = false;

            var currShader = capi.Render.CurrentActiveShader;
            if (currShader.HasUniform("modelViewMatrix"))
            {
                // Normal pass: replace the model view matrix with the mini dimension matrix
                currShader.UniformMatrix("modelViewMatrix", dimension.GetRenderTransformMatrix(masterPool.currentModelViewMatrix, playerpos));
                revertModelviewMatrix = true;
                if (currShader.HasUniform("forcedTransparency"))
                {
                    // Preview, for example while moving a block, is drawn semi transparent
                    currShader.Uniform("forcedTransparency", capi.Settings.Float["previewTransparency"]);
                    revertTransparency = true;
                }
            }
            else if (currShader.HasUniform("mvpMatrix"))
            {
                // The shadow pass uses a different matrix
                currShader.UniformMatrix("mvpMatrix", dimension.GetRenderTransformMatrix(masterPool.shadowMVPMatrix, playerpos));
                revertMVPMatrix = true;
            }

            currShader.Uniform(originUniformName, tmp.Set(
                (float)(pool.PoolOrigin.X + renderOffset.X - playerpos.X),
                (float)(pool.PoolOrigin.Y + renderOffset.Y - playerpos.Y),
                (float)(pool.PoolOrigin.Z + renderOffset.Z - playerpos.Z)
            ));

            try
            {
                pool.RenderMesh(capi.Render);
            }
            finally
            {
                // Restore matrices and transparency even when drawing threw
                if (revertModelviewMatrix)
                    capi.Render.CurrentActiveShader.UniformMatrix("modelViewMatrix", masterPool.currentModelViewMatrix);
                if (revertMVPMatrix)
                    capi.Render.CurrentActiveShader.UniformMatrix("mvpMatrix", masterPool.shadowMVPMatrix);
                if (revertTransparency)
                    capi.Render.CurrentActiveShader.Uniform("forcedTransparency", 0f);
            }
        }

        /// <summary>
        /// Collects stats over the pools: how much video memory is used, how many triangles
        /// were drawn and how many are allocated
        /// </summary>
        /// <param name="usedVideoMemory">Used video memory in bytes (added to the passed value)</param>
        /// <param name="renderedTris">Drawn triangles (added)</param>
        /// <param name="allocatedTris">Allocated triangles (added)</param>
        public void GetStats(ref long usedVideoMemory, ref long renderedTris, ref long allocatedTris)
        {
            // 1 vertex without SSBOs is:
            //   3 xyz floats  = 12 bytes
            //   2 uv floats   = 8 bytes
            //   4 rgba bytes  = 4 bytes
            //   1 flags int   = 4 bytes
            //   1 custom int (colormapData) = 4 bytes
            //
            // Optionally for liquids:
            //   2 custom floats (flowvector) = 8 bytes when custom data is present, for example liquids
            //   1 custom int (waterflags or uv2) = 4 bytes when custom data is present, for example liquids or topsoil
            //
            // Total vertex: 32 bytes plus optional custom data
            // Total indices: 6 bytes (for every 4 vertices, 6 indices of 4 bytes each)
            //
            // So 10 million triangles means 30 million indices and 20 million vertices
            // which is 114 MB of indices and 610 MB of vertices for 724 MB of video memory (ignoring custom data)
            // and most vertices also have at least some custom data
            //
            // With SSBOs video memory per vertex (base, no custom data) drops from 32 to 20 bytes if we count
            // 4 bytes of CustomInts per vertex for colormapData and indices take 0 bytes apart from the single
            // shared index buffer so overall 38 bytes per vertex became 20 bytes per vertex which is quite good

            // Size of one vertex in bytes: base part plus optional custom data when present
            long vertexSize = (capi.Render.UseSSBOs ? 16 + 4 - 4 : 12 + 8 + 4 + 4) + (customFloats == null ? 0 : customFloats.InterleaveStride) + (customShorts == null ? 0 : customShorts.InterleaveStride) + (customBytes == null ? 0 : customBytes.InterleaveStride) + (customInts == null ? 0 : customInts.InterleaveStride);
            int indexSize = capi.Render.UseSSBOs ? 0 : 4;   // with SSBOs the indices live in one shared buffer

            int maxIndices = 0;
            for (int i = 0; i < pools.Count; i++)
            {
                MeshDataPool pool = pools[i];

                usedVideoMemory += pool.VerticesPoolSize * vertexSize + pool.IndicesPoolSize * indexSize;
                renderedTris += pool.RenderedTriangles;
                allocatedTris += pool.AllocatedTris;

                // For the shared index buffer take the size of the largest pool
                if (pool.IndicesPoolSize > maxIndices) maxIndices = pool.IndicesPoolSize;
            }

            // With SSBOs one more shared index buffer is needed (4 bytes per index)
            if (capi.Render.UseSSBOs)
            {
                usedVideoMemory += maxIndices * 4;
            }
        }
    }


}
