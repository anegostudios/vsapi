using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client.Tesselation;
using Vintagestory.API.Common;

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
            for (int i = 0; i < verticesCount; i++)
            {
                float y = sourceMesh.xyz[i * 3 + 1];

                if (y > waveFlagMinY)
                {
                    sourceMesh.Flags[i] |= flag;
                }
                else
                {
                    sourceMesh.Flags[i] &= VertexFlags.ClearWindModeBitsMask;
                }
            }
        }

        public static void ClearWindFlags(this MeshData sourceMesh)
        {
            int verticesCount = sourceMesh.VerticesCount;
            for (int i = 0; i < verticesCount; i++)
            {
                sourceMesh.Flags[i] &= VertexFlags.ClearWindModeBitsMask;
            }
        }

        public static void ToggleWindModeSetWindData(this MeshData sourceMesh, int leavesNoShearTileSide, bool enableWind, int groundOffsetTop)
        {
            int clearFlags = VertexFlags.ClearWindBitsMask;
            int verticesCount = sourceMesh.VerticesCount;

            if (!enableWind)
            {
                // Shorter return path, and no need to test off in every iteration of the loop in the other code path
                for (int vertexNum = 0; vertexNum < verticesCount; vertexNum++)
                {
                    sourceMesh.Flags[vertexNum] &= clearFlags;
                }
                return;
            }

            // We add the ground offset to the winddatabits, but not if this side of the block is flagged in leavesNoShearTileSide (because against a solid block) - in that case, ground offset will remain zero
            for (int vertexNum = 0; vertexNum < verticesCount; vertexNum++)
            {
                int flag = sourceMesh.Flags[vertexNum] &= VertexFlags.ClearWindDataBitsMask;

                float fx = sourceMesh.xyz[vertexNum * 3 + 0];
                float fz = sourceMesh.xyz[vertexNum * 3 + 2];

                // The calculation (int)(x - 1.5f) will be either -2, -1 or 0 - this works reliably unless a vertex is positioned greater than +40/16 or less than -24/16 in which case this code may produce surprising waves (but no leaf block vertex is anywhere close to these limits, even if rotated: a basic leaf model is the widest I know of, some vertices are at coordinates 24/16 or -8/16)
                // The arithmetic right bit shift converts -2 to -1, while also preserving -1 -> -1 and leaving a value of 0 unchanged: a useful performance trick to avoid conditional jumps.

                int x = (int)(fx - 1.5f) >> 1;
                int y = (int)(sourceMesh.xyz[vertexNum * 3 + 1] - 1.5f) >> 1;
                int z = (int)(fz - 1.5f) >> 1;

                int sidesToCheckMask = 1 << TileSideEnum.Up - y | 4 + z * 3 | 2 - x * 6;     // evaluates to 32 or 16 (for y = -1 or 0)  +  1 or 4 (for z = -1 or 0)  +   8 or 2 (for x = -1 or 0)   In other words, bit flags in bit positions corresponding to TileSideFlagsEnum
                                                                                             // Every vertex has three flags set, because all vertices are on the "outside" of a leaves block - yes, a leaves block is not a standard cube and it has probably been rotated, but this is a good enough approximation to whether this vertex is close to the solid neighbour or not
                if ((leavesNoShearTileSide & sidesToCheckMask) == 0)
                {
                    flag |= (groundOffsetTop == 8 ? 7 : groundOffsetTop + y) << VertexFlags.WindDataBitsPos;
                }

                sourceMesh.Flags[vertexNum] = flag;
            }
        }


    }
}
