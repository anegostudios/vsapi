using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// A model data pool: one large buffer on the GPU that chunk meshes are packed into
    /// It holds up to 400k vertices and 300k indices but never more than 900 chunks
    /// </summary>
    public class MeshDataPool
    {
        /// <summary>Maximum number of parts (meshes) in this pool</summary>
        public int MaxPartsPerPool;

        /// <summary>Size of the pool vertex buffer</summary>
        public int VerticesPoolSize;

        /// <summary>Size of the pool index buffer</summary>
        public int IndicesPoolSize;

        // The mesh on the GPU and the pool id (a part checks the id to see it belongs to this pool)
        internal MeshRef modelRef;
        internal int poolId;

        // Sorted by position in the vertex and index buffers Needed to squeeze meshes into gaps
        // and to compute fragmentation Culling does NOT use this any more, it reads the slot arrays in cullData
        internal List<ModelDataPoolLocation> poolLocations = [];

        // Culling data and flags for each part (Hide, LodLevel, FrustumVisible, CullVisible), one slot per part
        private readonly PoolCullData cullData;

        // For the final rendering

        /// <summary>Start byte of each index group</summary>
        public int[] indicesStartsByte;

        /// <summary>Size of each index group</summary>
        public int[] indicesSizes;

        /// <summary>How many index groups there are (how many draw calls to make)</summary>
        public int indicesGroupsCount = 0;

        // Current position where the next vertex or index can be placed
        /// <summary>Position in the index buffer</summary>
        public int indicesPosition;

        /// <summary>Position in the vertex buffer</summary>
        public int verticesPosition;

        /// <summary>Current buffer fragmentation (share of holes)</summary>
        public float CurrentFragmentation;

        /// <summary>How many vertices are actually used</summary>
        public int UsedVertices;

        // Where the pool data sits in the world Set only while the pool is empty, via SetPoolOrigin
        // null means the world origin (0,0,0)
        private Vec3i poolOriginInternal;

        internal int dimensionId;

        /// <summary>How many triangles were drawn</summary>
        public int RenderedTriangles;

        /// <summary>How many triangles are allocated</summary>
        public int AllocatedTris;


        /// <summary>Read only view of the pool origin Null means the pool uses the world origin (0,0,0)</summary>
        internal Vec3i PoolOrigin => poolOriginInternal;


        /// <summary>
        /// Sets the pool origin Allowed only while the pool is empty: changing the origin with live parts
        /// would silently desync the coordinates stored in cullData Callers must use this method
        /// because the field can no longer be changed directly
        /// </summary>
        internal void SetPoolOrigin(Vec3i newOrigin)
        {
            if (poolLocations.Count > 0)
                throw new InvalidOperationException("MeshDataPool: pool origin can only be set while the pool is empty");

            if (poolOriginInternal == null)
                poolOriginInternal = new Vec3i(newOrigin.X, newOrigin.Y, newOrigin.Z);
            else
                poolOriginInternal.Set(newOrigin.X, newOrigin.Y, newOrigin.Z);
        }


        private MeshDataPool(int verticesPoolSize, int indicesPoolSize, int maxPartsPerPool)
        {
            this.MaxPartsPerPool = maxPartsPerPool;
            this.IndicesPoolSize = indicesPoolSize;
            this.VerticesPoolSize = verticesPoolSize;
            this.cullData = new PoolCullData(maxPartsPerPool);
        }

        /// <summary>
        /// Creates a new pool for mesh data
        /// </summary>
        public static MeshDataPool AllocateNewPool(ICoreClientAPI capi, int verticesPoolSize, int indicesPoolSize, int maxPartsPerPool, CustomMeshDataPartFloat customFloats = null, CustomMeshDataPartShort customShorts = null, CustomMeshDataPartByte customBytes = null, CustomMeshDataPartInt customInts = null)
        {
            MeshDataPool pool = new MeshDataPool(verticesPoolSize, indicesPoolSize, maxPartsPerPool)
            {
                // 64 bit builds need 64 bit memory addresses (a long array would be proper
                // but GL.MultiDrawElements accepts only int arrays)
                indicesStartsByte = new int[maxPartsPerPool * 2],
                indicesSizes = new int[maxPartsPerPool]
            };

            // Allocate the right number of bytes for the custom data
            customFloats?.SetAllocationSize(verticesPoolSize * customFloats.InterleaveStride / 4);
            customShorts?.SetAllocationSize(verticesPoolSize * customShorts.InterleaveStride / 2);
            customBytes?.SetAllocationSize(verticesPoolSize * customBytes.InterleaveStride);
            customInts?.SetAllocationSize(verticesPoolSize * customInts.InterleaveStride / 4);

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
        /// Tries to add a new mesh to the pool Returns null when there is no room
        /// or when the mesh is from another dimension
        /// </summary>
        public ModelDataPoolLocation TryAdd(ICoreClientAPI capi, MeshData modeldata, Vec3i modelOrigin, int dimension, Sphere frustumCullSphere)
        {
            if (poolLocations.Count >= MaxPartsPerPool || dimension != this.dimensionId) return null;

            // The pool origin can only be set while the pool is empty If it is not empty the new mesh
            // must be close enough to the existing origin or the float coordinates relative to the pool
            // would lose precision
            if (poolLocations.Count == 0)
            {
                SetPoolOrigin(modelOrigin);
            }
            else if (modelOrigin.SquareDistanceTo(poolOriginInternal) > 5000 * 5000)
            {
                return null;
            }

            // If the buffer has many holes first try to squeeze the mesh into one of them
            if (CurrentFragmentation > 0.03f)
            {
                ModelDataPoolLocation location = TrySqueezeInbetween(capi, modeldata, modelOrigin, frustumCullSphere);
                if (location != null) return location;
            }

            return TryAppend(capi, modeldata, modelOrigin, frustumCullSphere);
        }


        /// <summary>
        /// Looks for a hole between the meshes already in the pool that fits the new mesh
        /// Walks the list in order and checks the gap before every mesh
        /// </summary>
        ModelDataPoolLocation TrySqueezeInbetween(ICoreClientAPI capi, MeshData modeldata, Vec3i modelOrigin, Sphere frustumCullSphere)
        {
            int curVertexPos = 0;
            int curIndexPos = 0;

            for (int i = 0; i < poolLocations.Count; i++)
            {
                ModelDataPoolLocation location = poolLocations[i];

                if (location.IndicesStart - curIndexPos >= modeldata.IndicesCount && location.VerticesStart - curVertexPos >= modeldata.VerticesCount)
                {
                    return InsertAt(capi, modeldata, modelOrigin, frustumCullSphere, curIndexPos, curVertexPos, i);
                }

                curIndexPos = location.IndicesEnd;
                curVertexPos = location.VerticesEnd;
            }

            return null;
        }


        /// <summary>
        /// Puts the mesh at the end of the used part of the buffers Returns null when it does not fit
        /// </summary>
        ModelDataPoolLocation TryAppend(ICoreClientAPI capi, MeshData modeldata, Vec3i modelOrigin, Sphere frustumCullSphere)
        {
            if (modeldata.IndicesCount + indicesPosition > IndicesPoolSize || modeldata.VerticesCount + verticesPosition > VerticesPoolSize)
            {
                return null;
            }

            ModelDataPoolLocation location = InsertAt(capi, modeldata, modelOrigin, frustumCullSphere, indicesPosition, verticesPosition, -1);

            indicesPosition += modeldata.IndicesCount;
            verticesPosition += modeldata.VerticesCount;
#if DEBUG
            // With SSBOs vertices must land on a 4 vertex boundary (the buffer address must be a multiple of 16 bytes)
            if (verticesPosition % 4 != 0 && capi.Render.UseSSBOs) { throw new Exception("Vertices in pool not aligned on 4-vertex boundary. Try disabling clientsetting: allowSSBOs."); }
#endif

            return location;
        }


        /// <summary>
        /// The shared insert step: shifts indices and vertices to the chosen position, uploads the mesh
        /// to the GPU and creates the ModelDataPoolLocation A listPosition of -1 means append to the list
        /// </summary>
        ModelDataPoolLocation InsertAt(ICoreClientAPI capi, MeshData modeldata, Vec3i modelOrigin, Sphere frustumCullSphere, int indexPosition, int vertexPosition, int listPosition)
        {
            // Shift the index numbers to the new vertex position
            if (vertexPosition > 0)  // not needed when vertexPosition is zero
            {
                for (int i = 0; i < modeldata.IndicesCount; i++)
                {
                    modeldata.Indices[i] += vertexPosition;
                }
            }

            // Move the vertices into coordinates relative to the pool origin
            if (poolOriginInternal != null)
            {
                int dx = modelOrigin.X - poolOriginInternal.X;
                int dy = modelOrigin.Y - poolOriginInternal.Y;
                int dz = modelOrigin.Z - poolOriginInternal.Z;

                for (int i = 0; i < modeldata.VerticesCount; i++)
                {
                    modeldata.xyz[3 * i] += dx;
                    modeldata.xyz[3 * i + 1] += dy;
                    modeldata.xyz[3 * i + 2] += dz;
                }
            }

            // Shift the offsets to the end of the used buffers
            modeldata.XyzOffset = vertexPosition * MeshData.XyzSize;
            modeldata.NormalsOffset = vertexPosition * MeshData.NormalSize;
            modeldata.RgbaOffset = vertexPosition * MeshData.RgbaSize;
            modeldata.Rgba2Offset = vertexPosition * MeshData.RgbaSize;
            modeldata.UvOffset = vertexPosition * MeshData.UvSize;
            modeldata.FlagsOffset = vertexPosition * MeshData.FlagsSize;
            modeldata.IndicesOffset = indexPosition * MeshData.IndexSize;

            modeldata.CustomFloats?.BaseOffset = vertexPosition * modeldata.CustomFloats.InterleaveStride;
            modeldata.CustomShorts?.BaseOffset = vertexPosition * modeldata.CustomShorts.InterleaveStride;
            modeldata.CustomBytes?.BaseOffset = vertexPosition * modeldata.CustomBytes.InterleaveStride;
            modeldata.CustomInts?.BaseOffset = vertexPosition * modeldata.CustomInts.InterleaveStride;

            // Upload to the GPU
            capi.Render.UpdateChunkMesh(modelRef, modeldata);

            // Create the description of the place in the pool
            ModelDataPoolLocation poolLocation = new ModelDataPoolLocation
            {
                IndicesStart = indexPosition,
                IndicesEnd = indexPosition + modeldata.IndicesCount,
                VerticesStart = vertexPosition,
                VerticesEnd = vertexPosition + modeldata.VerticesCount,
                PoolId = poolId,
                FrustumCullSphere = frustumCullSphere,
                // Hand out a slot: from now on Hide, LodLevel, FrustumVisible and CullVisible live in cullData
                // The pool origin is passed together with the sphere so the two cannot get out of sync
                Slots = cullData,
                Slot = cullData.Allocate(poolOriginInternal, frustumCullSphere, indexPosition * 4, modeldata.IndicesCount)
            };

            // Keep the list sorted by buffer position
            if (listPosition != -1)
            {
                poolLocations.Insert(listPosition, poolLocation);
            }
            else
            {
                poolLocations.Add(poolLocation);
            }

            CalcFragmentation();

            return poolLocation;
        }

        /// <summary>
        /// Removes a mesh from the pool Throws when it cannot be removed
        /// (wrong pool or no such mesh in this pool)
        /// </summary>
        public void RemoveLocation(ModelDataPoolLocation location)
        {
            if (location.PoolId != poolId)
            {
                throw new Exception("invalid call");
            }

            int index = poolLocations.IndexOf(location);
            if (index < 0)
            {
                throw new InvalidOperationException("Tried to remove mesh that does not exist. This shouldn't happen");
            }

            poolLocations.RemoveAt(index);

            // Free the slot The location keeps its last flag values so reads and writes through a stale
            // reference stay harmless and can never hit a part that reuses the slot later
            int slot = location.Slot;
            location.Detach();
            cullData.Free(slot);

            // Was this the last part?
            if (poolLocations.Count == 0)
            {
                indicesPosition = 0;
                verticesPosition = 0;
            }
            else
            {
                // Was the part at the very end of the buffer?
                if (location.IndicesEnd == indicesPosition && location.VerticesEnd == verticesPosition)
                {
                    indicesPosition = poolLocations[poolLocations.Count - 1].IndicesEnd;
                    verticesPosition = poolLocations[poolLocations.Count - 1].VerticesEnd;
                }
            }

            CalcFragmentation();
        }

        /// <summary>
        /// Draws the pool: first picks the visible parts, then sends them in one call
        /// </summary>
        public void Draw(ICoreClientAPI capi, FrustumCulling frustumCuller, EnumFrustumCullMode frustumCullMode)
        {
            FrustumCull(frustumCuller, frustumCullMode);
            capi.Render.RenderMesh(modelRef, indicesStartsByte, indicesSizes, indicesGroupsCount);
        }

        /// <summary>
        /// Finds the visible parts and fills the arrays for MultiDrawElements
        /// The rules themselves are implemented in PoolCullData and set by FrustumCulling
        /// </summary>
        public void FrustumCull(FrustumCulling frustumCuller, EnumFrustumCullMode frustumCullMode)
        {
            indicesGroupsCount = cullData.Cull(frustumCuller, frustumCullMode,
                                               indicesStartsByte, indicesSizes, out int renderedIndices);

            RenderedTriangles = renderedIndices / 3;
            AllocatedTris = cullData.TotalIndices / 3;
        }

        /// <summary>
        /// Draws every part that is not hidden without frustum culling Used for mini dimension pools
        /// Everything goes through Cull so Hide, the group layout and the triangle stats share one place
        /// </summary>
        public void SetFullyVisible()
        {
            FrustumCull(null, EnumFrustumCullMode.NoCull);
        }

        /// <summary>Is the pool empty</summary>
        public bool IsEmpty()
        {
            return poolLocations.Count == 0;
        }

        /// <summary>Releases the pool buffers on the GPU</summary>
        public void Dispose(ICoreClientAPI capi)
        {
            capi.Render.DeleteMesh(modelRef);
        }

        /// <summary>
        /// Computes fragmentation: the share of the used vertex buffer taken up by holes between meshes
        /// The more holes there are, the more worthwhile it is to insert new meshes into the gaps
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
                // How many vertices are used and how much space was skipped before this part
                UsedVertices += location.VerticesEnd - location.VerticesStart;
                unusedVertices += Math.Max(0, location.VerticesStart - curPos);
                curPos = location.VerticesEnd;
            }

            CurrentFragmentation = (float)unusedVertices / verticesPosition;
        }

        /// <summary>Current pool fragmentation</summary>
        public float GetFragmentation()
        {
            return CurrentFragmentation;
        }

        //[MethodImpl(MethodImplOptions.NoInlining)]
        /// <summary>Sends only the parts found by the previous FrustumCull to the GPU</summary>
        public void RenderMesh(IRenderAPI render)
        {
            render.RenderMesh(modelRef, indicesStartsByte, indicesSizes, indicesGroupsCount);
        }
    }


    /// <summary>
    /// Description of one pool part (one chunk mesh in one LOD and one render pass)
    ///
    /// The flags Hide, LodLevel, FrustumVisible and CullVisible are properties: while the part is in a pool
    /// they live in the pool slot arrays (PoolCullData) so culling reads them without touching this object
    /// After the part is removed from the pool the last values stay in this object
    ///
    /// Note: this class implements no visibility rules All rules (the frustum test per mode, the LOD and
    /// view distance rule, the shadow pass threshold) live in FrustumCulling and the batched version
    /// lives in PoolCullData There is no separate per object version to keep in sync
    /// </summary>
    public class ModelDataPoolLocation
    {
        /// <summary>Which of the two visibility buffers to read this frame (0 or 1)</summary>
        public static int VisibleBufIndex;

        /// <summary>
        /// ID of the pool that owns this part
        /// </summary>
        public int PoolId;

        /// <summary>
        /// Where the mesh indices start
        /// </summary>
        public int IndicesStart;

        /// <summary>
        /// Where the mesh indices end
        /// </summary>
        public int IndicesEnd;

        /// <summary>
        /// Where the vertices start
        /// </summary>
        public int VerticesStart;

        /// <summary>
        /// Where the vertices end
        /// </summary>
        public int VerticesEnd;

        /// <summary>
        /// The sphere used for culling
        /// </summary>
        public Sphere FrustumCullSphere;

        /// <summary>
        /// For models that move, such as a door
        /// </summary>
        public int TransitionCounter;

        // Where the flags live while the part is in a pool Null before insertion and after removal
        internal PoolCullData Slots;
        internal int Slot;

        // Values used while there is no slot
        private bool hideLocal;
        private bool frustumVisibleLocal;
        private int lodLocal;
        private Bools cullLocal;

        /// <summary>Hidden parts are not drawn</summary>
        public bool Hide
        {
            get => Slots != null ? Slots.GetHide(Slot) : hideLocal;
            set { if (Slots != null) Slots.SetHide(Slot, value); else hideLocal = value; }
        }

        /// <summary>Whether the part was visible in the last CullNormal pass</summary>
        public bool FrustumVisible
        {
            get => Slots != null ? Slots.GetFrustumVisible(Slot) : frustumVisibleLocal;
            set { if (Slots != null) Slots.SetFrustumVisible(Slot, value); else frustumVisibleLocal = value; }
        }

        /// <summary>Mesh detail level: 0 is the most detailed and 3 is the coarsest</summary>
        public int LodLevel
        {
            get => Slots != null ? Slots.GetLod(Slot) : lodLocal;
            set { if (Slots != null) Slots.SetLod(Slot, value); else lodLocal = value; }
        }

        /// <summary>
        /// Visibility set by the chunk culler This is the shared Bools object of the chunk itself
        /// so a change is visible to every slot of the chunk at once
        /// </summary>
        public Bools CullVisible
        {
            get => Slots != null ? Slots.GetCullVisible(Slot) : (cullLocal ??= new Bools(true, true));
            set { if (Slots != null) Slots.SetCullVisible(Slot, value); else cullLocal = value; }
        }

        /// <summary>
        /// Detaches the part from its slot after copying the current flags into the local fields
        /// Called before the slot is freed
        /// </summary>
        internal void Detach()
        {
            if (Slots == null) return;

            hideLocal = Slots.GetHide(Slot);
            frustumVisibleLocal = Slots.GetFrustumVisible(Slot);
            lodLocal = Slots.GetLod(Slot);
            cullLocal = Slots.GetCullVisible(Slot) ?? new Bools(true, true);
            Slots = null;
        }
    }


    /// <summary>Visibility test delegate for external code</summary>
    public delegate bool VisibleTestDelegate(FrustumCulling culler);
}
