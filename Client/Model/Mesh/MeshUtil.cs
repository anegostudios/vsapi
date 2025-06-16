using Vintagestory.API.Client.Tesselation;
using Vintagestory.API.Common;

#nullable disable

namespace Vintagestory.API.Client
{
    public static class MeshUtil
    {
        /// <summary>
        /// Sets given flag if vertex y > WaveFlagMinY, otherwise it clears all wind mode bits
        /// </summary>
        /// <param name="sourceMesh"></param>
        /// <param name="waveFlagMinY"></param>
        /// <param name="flag">Default is EnumWindBitModeMask.NormalWind</param>
        public static void SetWindFlag(this MeshData sourceMesh, float waveFlagMinY = 9/16f, int flag = EnumWindBitModeMask.NormalWind)
        {
            int verticesCount = sourceMesh.VerticesCount;
            var sourceMeshXyz = sourceMesh.xyz;
            var sourceMeshFlags = sourceMesh.Flags;
            for (int i = 0; i < verticesCount; i++)
            {
                float y = sourceMeshXyz[i * 3 + 1];

                if (y > waveFlagMinY)
                {
                    sourceMeshFlags[i] |= flag;
                }
                else
                {
                    sourceMeshFlags[i] &= VertexFlags.ClearWindModeBitsMask;
                }
            }
        }

        public static void ClearWindFlags(this MeshData sourceMesh)
        {
            int verticesCount = sourceMesh.VerticesCount;
            var sourceMeshFlags = sourceMesh.Flags;
            for (int i = 0; i < verticesCount; i++)
            {
                sourceMeshFlags[i] &= VertexFlags.ClearWindModeBitsMask;
            }
        }

        public static void ToggleWindModeSetWindData(this MeshData sourceMesh, int leavesNoShearTileSide, bool enableWind, int groundOffsetTop)
        {
            int clearFlags = VertexFlags.ClearWindBitsMask;
            int verticesCount = sourceMesh.VerticesCount;
            var sourceMeshFlags = sourceMesh.Flags;

            if (!enableWind)
            {
                // Shorter return path, and no need to test off in every iteration of the loop in the other code path
                for (int vertexNum = 0; vertexNum < verticesCount; vertexNum++)
                {
                    sourceMeshFlags[vertexNum] &= clearFlags;
                }
                return;
            }

            var sourceMeshXyz = sourceMesh.xyz;
            // We add the ground offset to the winddatabits, but not if a vertex is on a side of the block flagged in leavesNoShearTileSide (because against a solid block) - in that case, ground offset will remain zero
            for (int vertexNum = 0; vertexNum < verticesCount; vertexNum++)
            {
                // This math calculates y as either -1 (for vertices near the bottom of the block) or 0 (for vertices near the top of the block)
                int y = (int)(sourceMesh.xyz[vertexNum * 3 + 1] - 1.5f) >> 1;
                // In more detail: the calculation (int)(x - 1.5f) will be either -2, -1 or 0 - this works reliably unless a vertex is positioned greater than +40/16 or less than -24/16 in which case this code may produce surprising waves (but no leaf block vertex is anywhere close to these limits, even if rotated: a basic leaf model is the widest I know of, some vertices are at coordinates 24/16 or -8/16)
                // The arithmetic right bit shift converts -2 to -1, while also preserving -1 -> -1 and leaving a value of 0 unchanged: a useful performance trick to avoid conditional jumps.

                // It is unusual for leavesNoShearTileSide to be non-zero, but if it is then we need to check whether our vertex is on one of the no-shear sides
                if (leavesNoShearTileSide != 0)
                {
                    // We make a sidesToCheckMask based on figuring out whether this x,y,z vertex is nearer Up or Down face, North or South face, and East or West face
                    // sidesToCheckMask is a bitmask with bits corresponding to TileSideFlagsEnum, i.e. 32 is Down, 16 is Up, 8 is West, etc.
                    // leavesNoShearTileSide was built in the same way. Binary & these two together and if the result is not 0, then we have a vertex on a no-shear tile side, in which case we want groundoffset 0

                    int x = (int)(sourceMeshXyz[vertexNum * 3 + 0] - 1.5f) >> 1;
                    int z = (int)(sourceMeshXyz[vertexNum * 3 + 2] - 1.5f) >> 1;
                    int sidesToCheckMask = 1 << TileSideEnum.Up - y | 4 + z * 3 | 2 - x * 6;     // This math evaluates to 32 or 16 (for y = -1 or 0)  +  1 or 4 (for z = -1 or 0)  +   8 or 2 (for x = -1 or 0)   In other words, bit flags in bit positions corresponding to TileSideFlagsEnum
                                                                                                 // Every vertex has three flags set, because all vertices are on the "outside" of a leaves block - yes, a leaves block is not a standard cube and it has probably been rotated, but this is a good enough approximation to whether this vertex is close to the solid neighbour or not
                    if ((leavesNoShearTileSide & sidesToCheckMask) != 0)    // If this vertex is on a side matching leavesNoShearTileSide, then the binary & result will be non-zero
                    {
                        // No wind shear on this side: ground offset set to 0 in the WindData
                        VertexFlags.ReplaceWindData(ref sourceMeshFlags[vertexNum], 0);
                        continue;
                    }
                }

                // Normally, apply a groundOffset based on the top of the block, -1 for vertices on the bottom of the block
                int groundOffset = groundOffsetTop == 8 ? 7 : groundOffsetTop + y;
                VertexFlags.ReplaceWindData(ref sourceMeshFlags[vertexNum], groundOffset);
            }
        }


    }
}
