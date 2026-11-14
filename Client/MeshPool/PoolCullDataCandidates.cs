using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

#nullable disable

namespace Vintagestory.API.Client
{
    // Stage 1: find the slots that could be visible using only the sphere arrays
    // Flags (hide, occlusion, LOD) are only looked at in stage 2
    // Glossary for slot, stage 1 and stage 2 is in the PoolCullData class comment
    //
    // Two implementations that must produce the same list:
    //   SIMD    tests Lanes slots at a time (Vector<float>) and is used when the hardware supports it
    //   scalar  tests one slot at a time, the fallback and the easiest one to read
    // Start with the scalar one at the end of the file: SIMD does the same for many slots at once
    //
    // Vector width: Vector<T> is 128 or 256 bit by default For 512 bit you need the
    // DOTNET_MaxVectorTBitWidth=512 switch (and AVX-512 to make it fast) No width is hardcoded
    // here: Lanes, the bit mask and the last batch handling all follow Vector<float>.Count
    internal sealed partial class PoolCullData
    {
        // How many slots fit into one vector: 4 (128 bit), 8 (256 bit) or 16 (512 bit)
        // This is a constant for the JIT so it can optimise the loops well
        private static readonly int Lanes = Vector<float>.Count;

        // SIMD is used only when the CPU supports it, otherwise everything goes through the scalar loop
        private static readonly bool UseSimd = Vector.IsHardwareAccelerated;

        /// <summary>All lanes set: the whole batch is outside A mask wider than 32 lanes does not fit into a uint</summary>
        private static readonly uint AllLanesMask = Lanes >= 32 ? uint.MaxValue : (1u << Lanes) - 1;

        /// <summary>For the startup log: how many slots are tested at a time</summary>
        public static int BatchSize => UseSimd ? Lanes : 1;

        // Stage 1 result: indices of the slots that passed The array is created in the constructor
        private readonly int[] candidateIndices;
        private int candidateCount;

        // Frustum planes for this pool, rebuilt every pass since they depend on the camera
        private readonly PoolPlane[] planes = new PoolPlane[6];
        private readonly PlaneVectors[] planeVectors = new PlaneVectors[6];

        // Range check values spread over vectors: computed once per pass, not once per batch
        private RangeVectors rangeVectors;

        // A plane spread over vectors: n is the normal, d is the offset, e is |n| / sqrt(3) for the AABB test
        private struct PlaneVectors
        {
            public Vector<float> nx, ny, nz, d, ex, ey, ez;
        }

        // The same range values as in CullPass but spread over the lanes of a vector
        private struct RangeVectors
        {
            public Vector<float> offsetX, offsetZ, limitA, limitB;
        }


        // ===============================================================
        //  Entry point
        // ===============================================================

        private void FindCandidates(FrustumCulling culler, in CullPass pass)
        {
            culler.GetPoolPlanes(originX, originY, originZ, pass.IncludeFar, planes);

            // The pool is far away or fully behind the camera: skip the whole pool without touching a slot
            if (PoolBoundsOutside(pass))
            {
                candidateCount = 0;
                return;
            }

            // Pick the implementation: SIMD is faster, scalar is the fallback
            if (UseSimd)
            {
                BuildPlaneVectors(pass.PlaneCount);
                if (pass.Range != RangeKind.None) BuildRangeVectors(pass);
                FillCandidatesSimd(pass);
            }
            else
            {
                FillCandidatesScalar(pass);
            }
        }

        /// <summary>
        /// True if the box around the whole pool is out of range or fully behind one of the planes
        /// This is safe because the box is never smaller than the slots inside it
        /// </summary>
        private bool PoolBoundsOutside(in CullPass pass)
        {
            if (!boundsValid) return false;

            // Radial: the range is a circle around the player
            if (pass.Range == RangeKind.Radial)
            {
                // Nearest point of the box to the player: the player position in pool space clamped into the box
                float px = -pass.OffsetX;
                float pz = -pass.OffsetZ;
                float cx = Math.Max(boundMinX, Math.Min(px, boundMaxX));
                float cz = Math.Max(boundMinZ, Math.Min(pz, boundMaxZ));
                float dx = cx - px;
                float dz = cz - pz;
                if (dx * dx + dz * dz > pass.LimitA) return true;
            }
            // Box: the range is a rectangle (shadows) and the box must overlap it on both axes
            else if (pass.Range == RangeKind.Box)
            {
                float lo = boundMinX + pass.OffsetX;
                float hi = boundMaxX + pass.OffsetX;
                if (lo >= pass.LimitA || hi <= -pass.LimitA) return true;

                lo = boundMinZ + pass.OffsetZ;
                hi = boundMaxZ + pass.OffsetZ;
                if (lo >= pass.LimitB || hi <= -pass.LimitB) return true;
            }

            for (int k = 0; k < pass.PlaneCount; k++)
            {
                if (planes[k].IsBoxOutside(boundMinX, boundMinY, boundMinZ, boundMaxX, boundMaxY, boundMaxZ))
                    return true;
            }

            return false;
        }


        // ===============================================================
        //  SIMD: Lanes slots at a time
        // ===============================================================

        private void FillCandidatesSimd(in CullPass pass)
        {
            int found = 0;

            // Full batches: every lane is a real slot so the mask needs no clean up
            int fullBatches = usedSlot - usedSlot % Lanes;
            for (int batch = 0; batch < fullBatches; batch += Lanes)
            {
                EmitCandidates(InsideMaskSimd(batch, pass), batch, ref found);
            }

            // The last batch reaches past usedSlot Free slots are rejected by FreeSlotRadius so the extra
            // lanes are usually empty anyway The mask is still applied so that a broken camera matrix
            // (planes with NaN which compare false everywhere) can never report a free slot as visible
            if (fullBatches < usedSlot)
            {
                uint inside = InsideMaskSimd(fullBatches, pass) & ((1u << (usedSlot - fullBatches)) - 1);
                EmitCandidates(inside, fullBatches, ref found);
            }

            candidateCount = found;
        }

        /// <summary>
        /// Turns a batch mask into a list of indices: walk the set bits until the mask becomes zero
        /// </summary>
        private void EmitCandidates(uint inside, int batch, ref int found)
        {
            while (inside != 0)
            {
                candidateIndices[found++] = batch + BitOperations.TrailingZeroCount(inside);
                inside &= inside - 1;   // clear the lowest set bit
            }
        }

        /// <summary>Tests one batch Returns a bit mask: bit k set means slot (batch + k) is inside</summary>
        private uint InsideMaskSimd(int batch, in CullPass pass)
        {
            var cx = LoadVector(centerX, batch);
            var cz = LoadVector(centerZ, batch);

            // Range first: it needs only x and z and rejects the most
            uint outside = 0;
            if (pass.Range != RangeKind.None)
            {
                outside = OutsideRangeBits(in rangeVectors, pass.Range, cx, cz);
                if (outside == AllLanesMask) return 0;
            }

            var cy = LoadVector(centerY, batch);
            var rx = LoadVector(radiusX, batch);
            var ry = LoadVector(radiusY, batch);
            var rz = LoadVector(radiusZ, batch);

            // Left, Right and Near reject the most so they are tested before the early exit
            outside |= OutsidePlaneBits(in planeVectors[0], cx, cy, cz, rx, ry, rz);
            outside |= OutsidePlaneBits(in planeVectors[1], cx, cy, cz, rx, ry, rz);
            outside |= OutsidePlaneBits(in planeVectors[2], cx, cy, cz, rx, ry, rz);

            if (outside == AllLanesMask) return 0;

            outside |= OutsidePlaneBits(in planeVectors[3], cx, cy, cz, rx, ry, rz);
            outside |= OutsidePlaneBits(in planeVectors[4], cx, cy, cz, rx, ry, rz);

            if (pass.PlaneCount == 6)
                outside |= OutsidePlaneBits(in planeVectors[5], cx, cy, cz, rx, ry, rz);

            // Keep the bits of the lanes that are still 0 in outside
            return ~outside & AllLanesMask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector<float> LoadVector(float[] array, int offset)
        {
            // Bounds are safe: the array length is a multiple of CapacityStep which is a multiple of any vector width
            return Vector.LoadUnsafe(ref MemoryMarshal.GetArrayDataReference(array), (nuint)offset);
        }

        /// <summary>
        /// Turns all bits set for a rejected lane into one bit per lane
        ///
        /// Vector has no generic ExtractMostSignificantBits (only the Vector2/3/4 overloads) so the width
        /// is picked here Vector&lt;T&gt;.Count is a constant for the JIT so only one branch survives compilation
        /// The last case is for a width that is none of the three (scalable vectors, ARM64 SVE):
        /// there is no movemask instruction for them so the mask is built lane by lane
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint Bits(Vector<int> outside)
        {
            // 512 bit returns a ulong and only the low Vector<float>.Count bits can ever be set
            if (Vector<int>.Count == Vector512<int>.Count) return unchecked((uint)Vector512.ExtractMostSignificantBits(outside.AsVector512()));
            if (Vector<int>.Count == Vector256<int>.Count) return Vector256.ExtractMostSignificantBits(outside.AsVector256());
            if (Vector<int>.Count == Vector128<int>.Count) return Vector128.ExtractMostSignificantBits(outside.AsVector128());

            // No movemask: build the mask by hand, one lane at a time
            uint inside = 0;
            for (int lane = 0; lane < Vector<int>.Count; lane++)
            {
                if (outside[lane] == 0) inside |= 1u << lane;
            }
            return inside;
        }

        /// <summary>Per lane: the bit is set when the slot is out of range Same test as IsOutsideRangeScalar</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint OutsideRangeBits(in RangeVectors r, RangeKind kind, Vector<float> cx, Vector<float> cz)
        {
            Vector<float> dx = cx + r.offsetX;
            Vector<float> dz = cz + r.offsetZ;

            if (kind == RangeKind.Radial)
            {
                // Circle: the range limit is compared against the squared distance
                return Bits(Vector.GreaterThan(dx * dx + dz * dz, r.limitA));
            }

            // Rectangle: the slot is outside when it exceeds the limit on at least one axis
            return Bits(Vector.GreaterThanOrEqual(Vector.Abs(dx), r.limitA)
                      | Vector.GreaterThanOrEqual(Vector.Abs(dz), r.limitB));
        }

        /// <summary>
        /// Per lane: the bit is set when the slot box is fully behind the plane Same test as the plane loop
        /// in FillCandidatesScalar and with the same order of additions so both give identical results
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint OutsidePlaneBits(
            in PlaneVectors p,
            Vector<float> cx, Vector<float> cy, Vector<float> cz,
            Vector<float> rx, Vector<float> ry, Vector<float> rz)
        {
            // Distance from the slot center to the plane plus the radius along the normal:
            // a negative value means the slot box is fully behind the plane
            Vector<float> distance = (p.nx * cx + p.ny * cy + p.nz * cz + p.d)
                                   + (p.ex * rx + p.ey * ry + p.ez * rz);

            return Bits(Vector.LessThan(distance, Vector<float>.Zero));
        }

        /// <summary>Spreads each plane value over all lanes of a vector once per pass instead of once per batch</summary>
        private void BuildPlaneVectors(int planeCount)
        {
            for (int k = 0; k < planeCount; k++)
            {
                ref readonly PoolPlane p = ref planes[k];
                planeVectors[k] = new PlaneVectors
                {
                    nx = new Vector<float>(p.nx),
                    ny = new Vector<float>(p.ny),
                    nz = new Vector<float>(p.nz),
                    d = new Vector<float>(p.d),
                    ex = new Vector<float>(p.ex),
                    ey = new Vector<float>(p.ey),
                    ez = new Vector<float>(p.ez),
                };
            }
        }

        /// <summary>Spreads the range check values over vectors once per pass instead of once per batch</summary>
        private void BuildRangeVectors(in CullPass pass)
        {
            rangeVectors = new RangeVectors
            {
                offsetX = new Vector<float>(pass.OffsetX),
                offsetZ = new Vector<float>(pass.OffsetZ),
                limitA = new Vector<float>(pass.LimitA),
                limitB = new Vector<float>(pass.LimitB),
            };
        }



        // ===============================================================
        //  Scalar: one slot at a time The fallback and the reference for the SIMD code above
        // ===============================================================

        private void FillCandidatesScalar(in CullPass pass)
        {
            // Local copies of the array references so the loop does not reload the fields
            float[] cxs = centerX, cys = centerY, czs = centerZ;
            float[] rxs = radiusX, rys = radiusY, rzs = radiusZ;
            PoolPlane[] pl = planes;

            int planeCount = pass.PlaneCount;
            int found = 0;

            for (int i = 0; i < usedSlot; i++)
            {
                // Range first: it is cheap and rejects most slots
                float cx = cxs[i], cz = czs[i];
                if (pass.Range != RangeKind.None && IsOutsideRangeScalar(cx, cz, pass))
                    continue;

                float cy = cys[i];
                float rx = rxs[i], ry = rys[i], rz = rzs[i];

                bool inside = true;
                for (int k = 0; k < planeCount; k++)
                {
                    ref readonly PoolPlane p = ref pl[k];

                    // Same order of additions as in OutsidePlaneBits (SIMD)
                    float distance = (p.nx * cx + p.ny * cy + p.nz * cz + p.d)
                                     + (p.ex * rx + p.ey * ry + p.ez * rz);

                    if (distance < 0)
                    {
                        inside = false;
                        break;
                    }
                }

                if (inside)
                    candidateIndices[found++] = i;
            }

            candidateCount = found;
        }

        /// <summary>Range check for one slot: a circle around the player or the shadow rectangle</summary>
        private static bool IsOutsideRangeScalar(float x, float z, in CullPass pass)
        {
            float dx = x + pass.OffsetX;
            float dz = z + pass.OffsetZ;

            if (pass.Range == RangeKind.Radial)
                return dx * dx + dz * dz > pass.LimitA;

            return Math.Abs(dx) >= pass.LimitA || Math.Abs(dz) >= pass.LimitB;
        }
    }
}
