using System;
using Vintagestory.API.Datastructures;

#nullable disable

namespace Vintagestory.API.Client
{
    // One culling pass: what happens, in which order and how the result is written
    // Glossary for slot, stage 1 and stage 2 is in the PoolCullData class comment
    internal sealed partial class PoolCullData
    {
        /// <summary>
        /// Collects the visible slots in the layout MultiDrawElements expects:
        /// one group is one draw call and needs a start in the index buffer plus an index count
        /// </summary>
        private struct GroupWriter
        {
            // Where group starts (byte offsets) and their sizes are written
            private readonly int[] starts;
            private readonly int[] sizes;

            public int Count;            // how many groups were added
            public int RenderedIndices;  // total indices that will be drawn

            public GroupWriter(int[] starts, int[] sizes)
            {
                this.starts = starts;
                this.sizes = sizes;
                Count = 0;
                RenderedIndices = 0;
            }

            /// <summary>Adds one group: byte offset and how many indices it has</summary>
            public void Add(int startBytes, int indices)
            {
                // Each group gets two ints (64 bits for the offset) so the start goes to [Count * 2]
                starts[Count * 2] = startBytes;
                sizes[Count] = indices;
                RenderedIndices += indices;
                Count++;
            }
        }


        /// <summary>
        /// Finds the visible slots and writes them to starts and sizes
        /// Returns the number of groups to draw and puts the total index count into renderedIndices
        /// </summary>
        public int Cull(
            FrustumCulling culler,
            EnumFrustumCullMode mode,
            int[] starts,
            int[] sizes,
            out int renderedIndices)
        {
            var groups = new GroupWriter(starts, sizes);

            if (count > 0)
            {
                if (!FrustumCulling.IsCullingMode(mode))
                {
                    // NoCull: only Hide matters Free slots are hidden too so they drop out here
                    for (int i = 0; i < usedSlot; i++)
                    {
                        if (hide[i] == 0) groups.Add(indexStartBytes[i], indexCount[i]);
                    }
                }
                else
                {
                    CullPass pass = culler.CreatePass(mode, originX, originZ);

                    FindCandidates(culler, pass);

                    // Must run even with zero candidates: slots that were visible in the previous pass
                    // and stopped being visible still have to be reset
                    if (mode == EnumFrustumCullMode.CullNormal)
                        ClearFrustumVisibleFlags();

                    FilterCandidates(culler, mode, pass, ref groups);
                }
            }

            renderedIndices = groups.RenderedIndices;
            return groups.Count;
        }


        // ===============================================================
        //  Stage 2: flags of the candidates
        // ===============================================================

        /// <summary>
        /// Keeps only the candidates that are not hidden, not occluded and pass the per mode rule
        /// for the slot (LOD and view distance for CullNormal, LOD level for the far shadow pass)
        /// </summary>
        private void FilterCandidates(
            FrustumCulling culler,
            EnumFrustumCullMode mode,
            in CullPass pass,
            ref GroupWriter groups)
        {
            // Which of the two visibility buffers the renderer reads this frame
            int buffer = ModelDataPoolLocation.VisibleBufIndex;
            bool writeFlags = mode == EnumFrustumCullMode.CullNormal;

            for (int n = 0; n < candidateCount; n++)
            {
                int i = candidateIndices[n];

                if (hide[i] != 0) continue;

                // The chunk culler writes visibility into a shared Bools object that every slot of the chunk sees
                Bools visibleInBuffer = cullVisible[i];
                if (visibleInBuffer == null || !visibleInBuffer[buffer]) continue;

                if (!PassesObjectChecks(culler, mode, lod[i], i, pass)) continue;

                if (writeFlags) SetFrustumVisible(i, true);

                groups.Add(indexStartBytes[i], indexCount[i]);
            }
        }

        /// <summary>
        /// The part of the rule that depends on the slot (its LOD level) The rule itself lives
        /// in FrustumCulling.PassesObjectTest and stage 1 already did the frustum and range test
        /// </summary>
        private bool PassesObjectChecks(FrustumCulling culler, EnumFrustumCullMode mode,
                                        int lodLevel, int i, in CullPass pass)
        {
            // Only CullNormal needs the distance so it is computed only in that mode
            float distanceSq = 0;
            if (mode == EnumFrustumCullMode.CullNormal)
            {
                float dx = centerX[i] + pass.OffsetX;
                float dz = centerZ[i] + pass.OffsetZ;
                distanceSq = dx * dx + dz * dz;
            }

            return culler.PassesObjectTest(mode, lodLevel, distanceSq);
        }


        // ===============================================================
        //  FrustumVisible
        // ===============================================================

        /// <summary>
        /// Resets FrustumVisible on slots that were visible in the previous CullNormal pass
        ///
        /// Why this is needed: the old code wrote FrustumVisible for every mesh on every pass
        /// Stage 1 skips most slots so those slots would never learn they are no longer visible
        /// So we remember the slots that are currently true (the flagged list) and reset only those
        ///
        /// Hidden slots keep their value, same as in the old code
        ///
        /// Known difference: a slot hidden only by occlusion (CullVisible false but not Hide)
        /// is now reset to false too, where the old code left its value alone
        /// Nothing in the game reads FrustumVisible so this is safe
        /// </summary>
        private void ClearFrustumVisibleFlags()
        {
            int keep = 0;

            for (int n = 0; n < flagged.Count; n++)
            {
                int slot = flagged[n];

                // The slot was freed after it was flagged (only the reference is read, not the Bools object)
                if (cullVisible[slot] == null) continue;

                if (hide[slot] == 0)
                    frustumVisible[slot] = 0;
                else
                    flagged[keep++] = slot;   // hidden slots stay in the list (shifted to the front)
            }

            // Cut the tail: only the slots just reset are left there
            flagged.RemoveRange(keep, flagged.Count - keep);
        }
    }
}
