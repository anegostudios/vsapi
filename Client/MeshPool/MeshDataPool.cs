using System;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// This is a modeldata pool, which can hold 400k vertices, 300k indices but not more than 900 chunks
    /// </summary>
    public class MeshDataPool
    {
        /// <summary>
        /// The maximum parts for this pool.
        /// </summary>
        public int MaxPartsPerPool;

        /// <summary>
        /// The current vertices for this pool.
        /// </summary>
        public int VerticesPoolSize;

        /// <summary>
        /// the amount of indicies for this pool.
        /// </summary>
        public int IndicesPoolSize;

        internal MeshRef modelRef;
        internal int poolId;

        // For defragmentation, sanity checks, frustum culling
        internal List<ModelDataPoolLocation> poolLocations = new List<ModelDataPoolLocation>();

        // For final rendering 

        /// <summary>
        /// The starting byte for each index.
        /// </summary>
        public int[] indicesStartsByte;

        /// <summary>
        /// The size of each index.
        /// </summary>
        public int[] indicesSizes;

        /// <summary>
        /// How many index groups are there.
        /// </summary>
        public int indicesGroupsCount = 0;
        

        // Current position on where the next free vertex/index can be placed
        /// <summary>
        /// the position of the indices.
        /// </summary>
        public int indicesPosition;

        /// <summary>
        /// the position of the vertices.
        /// </summary>
        public int verticesPosition;

        /// <summary>
        /// The current fragmentaton.
        /// </summary>
        public float CurrentFragmentation;

        /// <summary>
        /// How many of the vertices are used.
        /// </summary>
        public int UsedVertices;

        internal Vec3i poolOrigin;

        /// <summary>
        /// How many triangles are rendered.
        /// </summary>
        public int RenderedTriangles;

        /// <summary>
        /// How many triangles are allocated.
        /// </summary>
        public int AllocatedTris;


        private MeshDataPool(int verticesPoolSize, int indicesPoolSize, int maxPartsPerPool) {
            this.MaxPartsPerPool = maxPartsPerPool;
            this.IndicesPoolSize = indicesPoolSize;
            this.VerticesPoolSize = verticesPoolSize;
        }

        /// <summary>
        /// Allocates a new pool for mesh data.
        /// </summary>
        /// <param name="capi">The core client API</param>
        /// <param name="verticesPoolSize">The vertices pool size.</param>
        /// <param name="indicesPoolSize">The index pool size.</param>
        /// <param name="maxPartsPerPool">The maximum parts per pool.</param>
        /// <param name="customFloats">The custom floats of the pool.</param>
        /// <param name="customBytes">The custom bytes of the pool.</param>
        /// <param name="customInts">The custom ints of the pool.</param>
        /// <returns>The resulting mesh data pool.</returns>
        public static MeshDataPool AllocateNewPool(ICoreClientAPI capi, int verticesPoolSize, int indicesPoolSize, int maxPartsPerPool, CustomMeshDataPartFloat customFloats = null, CustomMeshDataPartShort customShorts = null, CustomMeshDataPartByte customBytes = null, CustomMeshDataPartInt customInts = null)
        {
            MeshDataPool pool = new MeshDataPool(verticesPoolSize, indicesPoolSize, maxPartsPerPool);

            if (IntPtr.Size == 8)
            {
                // 64 bit builds require 64 bit memory addresses (should be long[] really, but GL.MultiDrawElements only accepts int[])
                pool.indicesStartsByte = new int[maxPartsPerPool * 2];
            } else
            {
                pool.indicesStartsByte = new int[maxPartsPerPool];
            }
            
            pool.indicesSizes = new int[maxPartsPerPool];

            //MeshData modeldata = new MeshData(verticesPoolSize, indicesPoolSize, false); - wtf?

            // Allocate the right amount of bytes for custom data
            if (customFloats != null)
            {
                customFloats.SetAllocationSize(verticesPoolSize * customFloats.InterleaveStride / 4);
            }
            if (customShorts != null)
            {
                customShorts.SetAllocationSize(verticesPoolSize * customShorts.InterleaveStride / 2);
            }
            if (customBytes != null)
            {
                customBytes.SetAllocationSize(verticesPoolSize * customBytes.InterleaveStride); 
            }
            if (customInts != null)
            {
                customInts.SetAllocationSize(verticesPoolSize * customInts.InterleaveStride / 4);
            }

            pool.modelRef = capi.Render.AllocateEmptyMesh(
                MeshData.XyzSize * verticesPoolSize,
                0,
                MeshData.UvSize * verticesPoolSize,
                MeshData.RgbaSize * verticesPoolSize,
                MeshData.FlagsSize * verticesPoolSize,
                MeshData.IndexSize * indicesPoolSize,
                customFloats,
                customShorts,
                customBytes,
                customInts,
                EnumDrawMode.Triangles,
                false
            );
            

            return pool;
        }

        /// <summary>
        /// Attempts to add the new model.
        /// </summary>
        /// <param name="capi">The core client API</param>
        /// <param name="modeldata">The model to add</param>
        /// <param name="modelOrigin">The origin point of the model.</param>
        /// <param name="frustumCullSphere">The culling sphere.</param>
        /// <returns>The location of the model (and the data) in the pool.</returns>
        public ModelDataPoolLocation TryAdd(ICoreClientAPI capi, MeshData modeldata, Vec3i modelOrigin, Sphere frustumCullSphere)
        {
            if (poolLocations.Count >= MaxPartsPerPool) return null;

            // Can't add a model data to far away from our baseposition, otherwise our floating point positions
            // become too inaccurate
            if (poolOrigin != null && modelOrigin.SquareDistanceTo(poolOrigin) > 5000 * 5000) return null;

            if (CurrentFragmentation > 0.05f)
            {
                ModelDataPoolLocation location = TrySqueezeInbetween(capi, modeldata, modelOrigin, frustumCullSphere);
                if (location != null) return location;
            }

            return TryAppend(capi, modeldata, modelOrigin, frustumCullSphere);
        }


        ModelDataPoolLocation TrySqueezeInbetween(ICoreClientAPI capi, MeshData modeldata, Vec3i modelOrigin, Sphere frustumCullSphere)
        {
            int curVertexPos = 0;
            int curIndexPos = 0;
            
            for (int i = 0; i < poolLocations.Count; i++)
            {
                ModelDataPoolLocation location = poolLocations[i];

                if (location.indicesStart - curIndexPos > modeldata.IndicesCount && location.verticesStart - curVertexPos > modeldata.VerticesCount)
                {
                    return InsertAt(capi, modeldata, modelOrigin, frustumCullSphere, curIndexPos, curVertexPos, i);
                }

                curIndexPos = location.indicesEnd + 1;
                curVertexPos = location.verticesEnd + 1;

            }

            return null;
        }


        ModelDataPoolLocation TryAppend(ICoreClientAPI capi, MeshData modeldata, Vec3i modelOrigin, Sphere frustumCullSphere) {
            if (modeldata.IndicesCount + indicesPosition >= IndicesPoolSize || modeldata.VerticesCount + verticesPosition >= VerticesPoolSize)
            {
                return null;
            }

            ModelDataPoolLocation location = InsertAt(capi, modeldata, modelOrigin, frustumCullSphere, indicesPosition, verticesPosition, -1);

            indicesPosition += modeldata.IndicesCount;
            verticesPosition += modeldata.VerticesCount;

            return location;
        }


        ModelDataPoolLocation InsertAt(ICoreClientAPI capi, MeshData modeldata, Vec3i modelOrigin, Sphere frustumCullSphere, int indexPosition, int vertexPosition, int listPosition)
        { 
            // Shift the indices numbers accordingly
            for (int i = 0; i < modeldata.IndicesCount; i++)
            {
                modeldata.Indices[i] += vertexPosition;
            }

            // Relative offset to our pool origin
            if (poolOrigin != null) {
                int dx = modelOrigin.X - this.poolOrigin.X;
                int dy = modelOrigin.Y - this.poolOrigin.Y;
                int dz = modelOrigin.Z - this.poolOrigin.Z;

                for (int i = 0; i < modeldata.VerticesCount; i++)
                {
                    modeldata.xyz[3 * i] += dx;
                    modeldata.xyz[3 * i + 1] += dy;
                    modeldata.xyz[3 * i + 2] += dz;
                }
            }

            // Shift position to the end of the used buffers memory
            modeldata.XyzOffset = vertexPosition * MeshData.XyzSize;
            modeldata.NormalsOffset = vertexPosition * MeshData.NormalSize;
            modeldata.RgbaOffset = vertexPosition * MeshData.RgbaSize;
            modeldata.Rgba2Offset = vertexPosition * MeshData.RgbaSize;
            modeldata.UvOffset = vertexPosition * MeshData.UvSize;
            modeldata.FlagsOffset = vertexPosition * MeshData.FlagsSize;
            modeldata.IndicesOffset = indexPosition * MeshData.IndexSize;

            if (modeldata.CustomFloats != null)
            {
                modeldata.CustomFloats.BaseOffset = vertexPosition * modeldata.CustomFloats.InterleaveStride;
            }

            if (modeldata.CustomBytes != null)
            {
                modeldata.CustomBytes.BaseOffset = vertexPosition * modeldata.CustomBytes.InterleaveStride;
            }

            if (modeldata.CustomInts != null)
            {
                modeldata.CustomInts.BaseOffset = vertexPosition * modeldata.CustomInts.InterleaveStride;
            }

            // Load into graphics card
            capi.Render.UpdateMesh(modelRef, modeldata);
            
            
            // Assign a location to it
            ModelDataPoolLocation poolLocation = new ModelDataPoolLocation()
            {
                indicesStart = indexPosition,
                indicesEnd = indexPosition + modeldata.IndicesCount,
                verticesStart = vertexPosition,
                verticesEnd = vertexPosition + modeldata.VerticesCount,
                poolId = poolId,
                frustumCullSphere = frustumCullSphere
            };
            
            // Maintain correct ordering
            if (listPosition != -1)
            {
                poolLocations.Insert(listPosition, poolLocation);
            } else
            {
                poolLocations.Add(poolLocation);
            }
            
            CalcFragmentation();

            return poolLocation;
        }

        /// <summary>
        /// Attempts to remove the model from the pool if the model exists.  Will throw an invalid call or an InvalidOperationException if used improperly.
        /// </summary>
        /// <param name="location">The location of the model data.</param>
        public void RemoveLocation(ModelDataPoolLocation location)
        {
            if (location.poolId != poolId)
            {
                throw new Exception("invalid call");
            }

            if (!poolLocations.Remove(location))
            {    
                throw new InvalidOperationException("Tried to remove mesh that does not exist. This shouldn't happen");
            }

            // Last location?
            if (poolLocations.Count == 0)
            {
                indicesPosition = 0;
                verticesPosition = 0;
            } else
            {
                // Location at the end of the buffer?
                if (location.indicesEnd == indicesPosition && location.verticesEnd == verticesPosition)
                {
                    indicesPosition = poolLocations[poolLocations.Count - 1].indicesEnd;
                    verticesPosition = poolLocations[poolLocations.Count - 1].verticesEnd;
                }
            }


            CalcFragmentation();
        }

        /// <summary>
        /// Draw the model.
        /// </summary>
        /// <param name="capi">The core client API</param>
        /// <param name="frustumCuller">The area where models can be viewed from the camera.</param>
        /// <param name="frustumCullMode">The mode of the culling.</param>
        public void Draw(ICoreClientAPI capi, FrustumCulling frustumCuller, EnumFrustumCullMode frustumCullMode)
        {
            FrustumCull(frustumCuller, frustumCullMode);
            capi.Render.RenderMesh(modelRef, indicesStartsByte, indicesSizes, indicesGroupsCount);
        }

        /// <summary>
        /// Cleans up the rendering view of the models.
        /// </summary>
        /// <param name="frustumCuller">The area where models can be viewed from the camera.</param>
        /// <param name="frustumCullMode">The mode of the culling.</param>
        public void FrustumCull(FrustumCulling frustumCuller, EnumFrustumCullMode frustumCullMode)
        {
            indicesGroupsCount = 0;
            //int tmp = 0;
            int multiplier = (IntPtr.Size == 8) ? 2 : 1;

            RenderedTriangles = 0;
            AllocatedTris = 0;

            for (int i = 0; i < poolLocations.Count; i++)
            {
                ModelDataPoolLocation location = poolLocations[i];

                if (location.IsVisible(frustumCullMode, frustumCuller))
                {
                    indicesStartsByte[indicesGroupsCount * multiplier] = location.indicesStart * 4; // Offset in bytes, not ints
                    indicesSizes[indicesGroupsCount] = location.indicesEnd - location.indicesStart;

                    RenderedTriangles += (location.indicesEnd - location.indicesStart) / 3;

                    indicesGroupsCount++;
                }

                //tmp += location.indicesEnd - location.indicesStart;

                AllocatedTris += (location.indicesEnd - location.indicesStart) / 3;
            }
        }

        /// <summary>
        /// Is this an empty pool.
        /// </summary>
        /// <returns>true if the pool is empty.</returns>
        public bool IsEmpty()
        {
            return poolLocations.Count == 0;
        }

        /// <summary>
        /// Disposes of the current mesh pool.
        /// </summary>
        /// <param name="capi">The core client API</param>
        public void Dispose(ICoreClientAPI capi)
        {
            capi.Render.DeleteMesh(modelRef);
        }

        /// <summary>
        /// Calculates the current fragmentation of the mesh.
        /// </summary>
        public void CalcFragmentation()
        {
            int curPos = 0;
            int unusedVertices = 0;
            UsedVertices = 0;

            if (verticesPosition == 0)
            {
                CurrentFragmentation = 0;
                return;
            }

            foreach (ModelDataPoolLocation location in poolLocations)
            {
                UsedVertices += location.verticesEnd - location.verticesStart;
                unusedVertices += Math.Max(0, location.verticesStart - curPos);
                curPos = location.verticesEnd + 1;
            }

            CurrentFragmentation = (float)unusedVertices / verticesPosition;
        }

        /// <summary>
        /// Gets the current fragmentation of the pool.
        /// </summary>
        /// <returns></returns>
        public float GetFragmentation()
        {
            return CurrentFragmentation;
        }
    }


    /// <summary>
    /// Contains all the data for the given model pool.
    /// </summary>
    public class ModelDataPoolLocation
    {
        public static int visibleBufIndex;

        /// <summary>
        /// The ID of the pool model.
        /// </summary>
        public int poolId;

        /// <summary>
        /// Where the indices of the model start.
        /// </summary>
        public int indicesStart;

        /// <summary>
        /// Where the indices of the model end.
        /// </summary>
        public int indicesEnd;

        /// <summary>
        /// Where the vertices start.
        /// </summary>
        public int verticesStart;

        /// <summary>
        /// Where the vertices end.
        /// </summary>
        public int verticesEnd;

        /// <summary>
        /// The culling sphere.
        /// </summary>
        public Sphere frustumCullSphere;

        /// <summary>
        /// Whether this model is visible or not.
        /// </summary>
        public bool FrustumVisible;

        public BoolRef[] CullVisible = new BoolRef[] { new BoolRef() { value = true }, new BoolRef() { value = true } };

        public int LodLevel = 0;

        /// <summary>
        /// Used for models with movements (like a door).
        /// </summary>
        public int TransitionCounter;

        private bool UpdateVisibleFlag(bool inFrustum)
        {
            /*if (FrustumVisible && !inFrustum)
            {
                //TransitionCounter--;
                //if (TransitionCounter <= 0)  - causes lod 0 and lod 1 to render at the same time, making water brighter. Maybe no longer needed due to double precision frustum culling now?
                FrustumVisible = false;
            }

            if (!FrustumVisible && inFrustum)
            {
                TransitionCounter=10;
                FrustumVisible = true;
            }*/

            FrustumVisible = inFrustum;

            return FrustumVisible;
        }


        internal bool IsVisible(EnumFrustumCullMode mode, FrustumCulling culler)
        {
            switch (mode)
            {
                case EnumFrustumCullMode.CullInstant:
                    return CullVisible[visibleBufIndex].value && culler.SphereInFrustum(frustumCullSphere);

                case EnumFrustumCullMode.CullInstantShadowPassNear:
                    return CullVisible[visibleBufIndex].value && culler.SphereInFrustumShadowPass(frustumCullSphere);

                case EnumFrustumCullMode.CullInstantShadowPassFar:
                    return CullVisible[visibleBufIndex].value && culler.SphereInFrustumShadowPass(frustumCullSphere) && LodLevel == 1;

                case EnumFrustumCullMode.CullNormal:
                    return CullVisible[visibleBufIndex].value && UpdateVisibleFlag(culler.SphereInFrustumAndRange(frustumCullSphere, FrustumVisible, LodLevel));

                default:
                    return true;
            }

        }
    }


    public delegate bool VisibleTestDelegate(FrustumCulling culler);

    
}
