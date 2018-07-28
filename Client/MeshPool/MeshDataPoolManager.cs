using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.MathTools;

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
        CustomMeshDataPartByte customBytes = null;
        CustomMeshDataPartInt customInts = null;

        int defaultVertexPoolSize;
        int defaultIndexPoolSize;
        int maxPartsPerPool;
        
        Vec3f tmp = new Vec3f();


        public MeshDataPoolManager(MeshDataPoolMasterManager masterPool, FrustumCulling frustumCuller, ICoreClientAPI capi, int defaultVertexPoolSize, int defaultIndexPoolSize, int maxPartsPerPool, CustomMeshDataPartFloat customFloats = null, CustomMeshDataPartByte customBytes = null, CustomMeshDataPartInt customInts = null)
        {
            this.masterPool = masterPool;
            this.frustumCuller = frustumCuller;
            this.capi = capi;
            this.customFloats = customFloats;
            this.customBytes = customBytes;
            this.customInts = customInts;
            this.defaultIndexPoolSize = defaultIndexPoolSize;
            this.defaultVertexPoolSize = defaultVertexPoolSize;
            this.maxPartsPerPool = maxPartsPerPool;
        }

        public ModelDataPoolLocation AddModel(MeshData modeldata, Vec3i modelOrigin, Sphere frustumCullSphere)
        {
            ModelDataPoolLocation location = null;

            for (int i = 0; i < pools.Count; i++)
            {
                location = pools[i].TryAdd(capi, modeldata, modelOrigin, frustumCullSphere);
                if (location != null) break;
            }

            if (location == null)
            {
                MeshDataPool pool = MeshDataPool.AllocateNewPool(capi, defaultVertexPoolSize, defaultIndexPoolSize, maxPartsPerPool, customFloats, customBytes, customInts);
                pool.poolOrigin = modelOrigin;

                masterPool.AddModelDataPool(pool);
                pools.Add(pool);
                location = pool.TryAdd(capi, modeldata, modelOrigin, frustumCullSphere);
            }

            if (location == null)
            {
                capi.World.Logger.Fatal("Can't add modeldata (probably a tesselated chunk @{0}) to the model data pool list, it exceeds the size of a single empty pool of {1} vertices and {2} indices. You must be loading some very complex objects (#v = {3}, #i = {4}). Try increasing MaxVertexSize and MaxIndexSize. The whole chunk will be invisible.", modelOrigin, defaultVertexPoolSize, defaultIndexPoolSize, modeldata.VerticesCount, modeldata.IndicesCount);

                //location = new ModelDataPoolLocation() { frustumCullSphere = frustumCullSphere };
                //pools[0].poolLocations.Add(location);
            }

            return location;
        }

        public void Render(Vec3d playerpos, string originUniformName, EnumFrustumCullMode frustumCullMode = EnumFrustumCullMode.CullHideDelay)
        {
            for (int i = 0; i < pools.Count; i++)
            {
                MeshDataPool pool = pools[i];
                pool.FrustumCull(frustumCuller, frustumCullMode);

                capi.Render.CurrentActiveShader.Uniform(originUniformName, tmp.Set(
                    (float)(pool.poolOrigin.X - playerpos.X),
                    (float)(pool.poolOrigin.Y - playerpos.Y),
                    (float)(pool.poolOrigin.Z - playerpos.Z)
                ));

                capi.Render.RenderMesh(pool.modelRef, pool.indicesStartsByte, pool.indicesSizes, pool.indicesGroupsCount);
            }
        }

        public void GetStats(ref long usedVideoMemory, ref long renderedTris, ref long allocatedTris)
        {
            // 1 index = 4 byte
            // 1 vertex = 
            // - 3 xyz floats  = 12 byte 
            // - 2 uv floats   = 8 byte 
            // - 4 rgba bytes  = 4 byte
            // - 4 rgba2 bytes = 4 byte
            // - 1 flags int    = 4 byte
            //[ - 1 normals int = 4 byte]
            // - 2 custom floats (uv2)  = 8 byte    when customdata is there

            // Total vertex: 32 bytes or 40 bytes
            // % size normals: 11% or 9%
            // Total index: 4 bytes

            // So 10mil tris => 30mil indices, 30m*0.75=23 mil vertices
            // = 114mb indices, 789mb vertices => 903mb video memory   (no custom floats)
            //                  88mb normals

            long vertexSize = 12 + 8 + 4 + 4 + 4 + (customFloats == null ? 0 : customFloats.InterleaveStride) + (customBytes == null ? 0 : customBytes.InterleaveStride) + (customInts == null ? 0 : customInts.InterleaveStride);

            for (int i = 0; i < pools.Count; i++)
            {
                MeshDataPool pool = pools[i];

                usedVideoMemory += pool.VerticesPoolSize * vertexSize + pool.IndicesPoolSize * 4;
                renderedTris += pool.RenderedTriangles;
                allocatedTris += pool.AllocatedTris;
            }
        }
    }


}
