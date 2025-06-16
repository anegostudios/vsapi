using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    class DecorSelectionBox : Cuboidf
    {
        /// <summary>
        /// The offset to the BlockPos, to find the block which produced this selection box.  Normally null.  Used by Decor system
        /// (Could be developed in future for other blocks with selection boxes outside their own 1x1x1 block space e.g. opened gate?)
        /// </summary>
        public Vec3i PosAdjust;

        public DecorSelectionBox(float x1, float y1, float z1, float x2, float y2, float z2) : base(x1, y1, z1, x2, y2, z2)
        {
        }
    }
}
