using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Everything frustum culling needs to know about the meshes of one MeshDataPool Not one object
    /// per mesh but a set of parallel arrays (structure of arrays) which is faster for cache and SIMD
    ///
    /// TERMS
    ///   slot   One mesh in this pool: one chunk, in one LOD level, in one render pass
    ///          A slot is just an index into the arrays below It never changes while the mesh is in the pool
    ///   stage 1  Finds the slots that could be visible using only the sphere arrays (fast, vectorized)
    ///   stage 2  Looks at the flags (hide, occluded, LOD) of the slots found in stage 1 only
    ///
    /// FILES (one class split by topic)
    ///   PoolCullData.cs             this file: storage, slot allocation, flag access
    ///   PoolCullDataCulling.cs      what happens in one culling pass (Cull, stage 2, FrustumVisible)
    ///   PoolCullDataCandidates.cs   stage 1: SIMD and scalar implementations, whole pool rejection
    ///
    /// WHERE THE RULES LIVE
    /// This class is mechanism only The visibility rules (what each mode means, the LOD and view
    /// distance rule, the plane equations) live in FrustumCulling: CreatePass, GetPoolPlanes,
    /// PassesObjectTest and IsWithinLodRange Change a rule there and this class follows
    ///
    /// RULES PER MODE (for orientation)
    ///   NoCull or unknown            !Hide
    ///   CullInstant                  !Hide, CullVisible, inside all 6 planes
    ///   CullInstantShadowPassNear    !Hide, CullVisible, inside the shadow range, inside all 6 planes
    ///   CullInstantShadowPassFar     same as above and LodLevel is at least 1
    ///   CullNormal                   !Hide, CullVisible, inside 5 planes (no far plane), LOD rule
    /// </summary>
    internal sealed partial class PoolCullData
    {
        // Array length is rounded up to a multiple of 64 and 64 is a multiple of any vector width
        // so loading the last vector batch never reads past the end of an array
        private const int CapacityStep = 64;

        // Radius given to a free slot It is large and negative so the plane test (n.c + D + e.r below 0)
        // rejects the slot for every plane This way stage 1 needs no separate check for a slot being used
        private const float FreeSlotRadius = -1e30f;


        // ===============================================================
        //  Slot data: one entry per slot, all arrays have the same length
        // ===============================================================

        // Bounding sphere of the mesh Stored relative to the pool origin because absolute world
        // coordinates lose precision as float far from the world origin
        private readonly float[] centerX, centerY, centerZ;
        private readonly float[] radiusX, radiusY, radiusZ;

        // Where the mesh indices live in the pool buffer, which is exactly what gets drawn
        private readonly int[] indexStartBytes;
        private readonly int[] indexCount;

        // State flags Read and written through the methods at the end of this file
        private readonly byte[] hide;            // 1 means hidden, free slots are hidden too
        private readonly byte[] lod;             // LOD level 0 to 3
        private readonly byte[] frustumVisible;  // result of the last CullNormal pass, see ClearFrustumVisibleFlags

        // A reference to the chunk own Bools object, not a copy: the chunk culler changes chunk
        // visibility through that object and every slot of the chunk sees the change at once
        // null means a free slot
        private readonly Bools[] cullVisible;

        private readonly int maxParts;           // the pool never holds more slots than this
        private readonly int capacity;           // array length: maxParts rounded up to CapacityStep

        private readonly int[] freeSlots;        // stack of freed slots, used before the arrays grow
        private int freeCount;
        private int usedSlot;                   // iteration bound: only slots below this index can be in use
        private int count;                       // slots in use

        // Slots whose frustumVisible is 1 Lets us reset only those without scanning the whole pool
        private readonly List<int> flagged = [];

        private double originX, originY, originZ;   // pool origin, see Allocate

        // Box around all spheres added since the pool was last empty It only grows (also when slots
        // are freed) so it can be larger than needed but never smaller That is enough for a pool outside check
        private float boundMinX, boundMinY, boundMinZ;
        private float boundMaxX, boundMaxY, boundMaxZ;
        private bool boundsValid;

        /// <summary>Sum of indices of all slots (for the AllocatedTris statistic)</summary>
        public int TotalIndices { get; private set; }

        public int Count => count;


        public PoolCullData(int maxParts)
        {
            this.maxParts = Math.Max(maxParts, 1);
            capacity = (this.maxParts + CapacityStep - 1) / CapacityStep * CapacityStep;

            centerX = new float[capacity];
            centerY = new float[capacity];
            centerZ = new float[capacity];
            radiusX = new float[capacity];
            radiusY = new float[capacity];
            radiusZ = new float[capacity];
            indexStartBytes = new int[capacity];
            indexCount = new int[capacity];
            hide = new byte[capacity];
            lod = new byte[capacity];
            frustumVisible = new byte[capacity];
            cullVisible = new Bools[capacity];
            freeSlots = new int[capacity];
            candidateIndices = new int[capacity];   

            // Every slot starts out free
            Array.Fill(radiusX, FreeSlotRadius);
            Array.Fill(radiusY, FreeSlotRadius);
            Array.Fill(radiusZ, FreeSlotRadius);
            Array.Fill(hide, (byte)1);
        }


        // ===============================================================
        //  Slot allocation
        // ===============================================================

        /// <summary>
        /// Takes a slot for a new mesh and fills it with the defaults Returns the slot number
        ///
        /// The pool origin is passed in together with the sphere so the two cannot get out of sync
        /// It can only change while the pool is empty (the pool moves the origin only then) otherwise
        /// the spheres stored earlier would be relative to a different origin
        /// </summary>
        internal int Allocate(Vec3i poolOrigin, Sphere sphere, int startBytes, int indices)
        {
            if (count >= maxParts)
                throw new InvalidOperationException("PoolCullData: more slots than MaxPartsPerPool");

            double ox = poolOrigin?.X ?? 0;
            double oy = poolOrigin?.Y ?? 0;
            double oz = poolOrigin?.Z ?? 0;

            if (count > 0 && (originX != ox || originY != oy || originZ != oz))
                throw new InvalidOperationException("PoolCullData: pool origin changed while the pool is not empty");

            originX = ox;
            originY = oy;
            originZ = oz;

            // Take a freed slot or grow the range A popped slot can be an orphan left above usedSlot
            // by the compaction in Free() and the check below extends the range again so an occupied
            // slot is never above the iteration bound used by stage 1
            //
            // Invariants that make the index safe:
            //   I1  all slots at index >= usedSlot are free (cullVisible[slot] == null)
            //   I2  freeSlots may hold entries at index >= usedSlot (orphans from Free compaction)
            //       and Allocate extends usedSlot when such an entry is popped
            //   I3  every entry of freeSlots is below maxParts which is at most capacity
            //   I4  count is below maxParts (checked above) so the range never grows past maxParts
            int slot = freeCount > 0 ? freeSlots[--freeCount] : usedSlot++;
            if (slot >= usedSlot) usedSlot = slot + 1;   // an orphan was popped so extend the range again
            System.Diagnostics.Debug.Assert(slot >= 0 && slot < capacity, "PoolCullData: slot index out of range");

            float cx = (float)(sphere.x - originX);
            float cy = (float)(sphere.y - originY);
            float cz = (float)(sphere.z - originZ);
            float rx = (float)sphere.radius;
            float ry = (float)sphere.radiusY;
            float rz = (float)sphere.radiusZ;

            centerX[slot] = cx;
            centerY[slot] = cy;
            centerZ[slot] = cz;
            radiusX[slot] = rx;
            radiusY[slot] = ry;
            radiusZ[slot] = rz;
            indexStartBytes[slot] = startBytes;
            indexCount[slot] = indices;

            // New mesh defaults (the caller sets lod and cullVisible right after insertion)
            hide[slot] = 0;
            lod[slot] = 0;
            frustumVisible[slot] = 0;
            cullVisible[slot] = new Bools(true, true);

            ExpandBounds(cx, cy, cz, rx, ry, rz);
            count++;
            TotalIndices += indices;
            return slot;
        }

        /// <summary>
        /// Frees a slot Call ModelDataPoolLocation.Detach() first so the mesh object keeps its last
        /// flag values and after this call the slot may go to a different mesh
        ///
        /// It also compacts the iteration bound: if the freed slot (or any other free slot below it)
        /// sits at the top of the range, usedSlot shrinks Reusing such a slot later extends the range
        /// again automatically (see Allocate) so the bound is always a safe upper limit for stage 1
        /// </summary>
        internal void Free(int slot)
        {
            TotalIndices -= indexCount[slot];

            radiusX[slot] = FreeSlotRadius;
            radiusY[slot] = FreeSlotRadius;
            radiusZ[slot] = FreeSlotRadius;
            indexCount[slot] = 0;
            hide[slot] = 1;
            lod[slot] = 0;
            frustumVisible[slot] = 0;
            cullVisible[slot] = null;

            count--;
            if (count == 0)
            {
                // An empty pool starts from scratch, same as in the old pool code
                usedSlot = 0;
                freeCount = 0;
                boundsValid = false;
                flagged.Clear();
            }
            else
            {
                freeSlots[freeCount++] = slot;

                // Compaction: cullVisible is null for every freed slot so the loop lowers the bound below
                // every free slot at the top of the range The slots it passes can stay in freeSlots above
                // the new bound (orphans) and Allocate extends usedSlot when they are reused
                // Amortized O(1): every decrement of usedSlot is paid for by a slot that stage 1 stops
                // iterating until it is used again
                while (usedSlot > 0 && cullVisible[usedSlot - 1] == null) usedSlot--;
            }
        }

        /// <summary>Grows the box around the pool so that it contains this sphere</summary>
        private void ExpandBounds(float cx, float cy, float cz, float rx, float ry, float rz)
        {
            if (!boundsValid)
            {
                boundMinX = cx - rx; boundMaxX = cx + rx;
                boundMinY = cy - ry; boundMaxY = cy + ry;
                boundMinZ = cz - rz; boundMaxZ = cz + rz;
                boundsValid = true;
                return;
            }

            if (cx - rx < boundMinX) boundMinX = cx - rx;
            if (cx + rx > boundMaxX) boundMaxX = cx + rx;
            if (cy - ry < boundMinY) boundMinY = cy - ry;
            if (cy + ry > boundMaxY) boundMaxY = cy + ry;
            if (cz - rz < boundMinZ) boundMinZ = cz - rz;
            if (cz + rz > boundMaxZ) boundMaxZ = cz + rz;
        }


        // ===============================================================
        //  Flag access (used by ModelDataPoolLocation)
        // ===============================================================

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool GetHide(int slot) => hide[slot] != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetHide(int slot, bool value) => hide[slot] = value ? (byte)1 : (byte)0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int GetLod(int slot) => lod[slot];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetLod(int slot, int value) => lod[slot] = (byte)value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Bools GetCullVisible(int slot) => cullVisible[slot];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void SetCullVisible(int slot, Bools value) => cullVisible[slot] = value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool GetFrustumVisible(int slot) => frustumVisible[slot] != 0;

        /// <summary>
        /// Setting true also adds the slot to the flagged list That list is the only way
        /// ClearFrustumVisibleFlags finds the slots to reset so every write of true must go through here
        /// </summary>
        internal void SetFrustumVisible(int slot, bool value)
        {
            if (value)
            {
                if (frustumVisible[slot] == 0)
                {
                    frustumVisible[slot] = 1;
                    flagged.Add(slot);
                }
            }
            else
            {
                frustumVisible[slot] = 0;
            }
        }
    }
}
