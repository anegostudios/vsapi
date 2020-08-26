using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Client
{
    public enum EnumFaceCullMode
    {
        /// <summary>
        /// Culls faces if they are opaque faces adjacent to opaque faces
        /// </summary>
        Default = 0,

        /// <summary>
        /// Never culls any faces
        /// </summary>
        NeverCull = 1,

        /// <summary>
        /// Culls all faces that are adjacent to opaque faces and faces adjacent to blocks of the same id
        /// </summary>
        Merge = 2,

        MergeIce = 7,

        /// <summary>
        /// Culls all faces that are adjacent to opaque faces and the bottom, east or south faces adjacent to blocks of the same id
        /// This causes to still leave one single face inbetween instead of 2, eliminating any z-fighting.
        /// </summary>
        Collapse = 3,

        /// <summary>
        /// Same as Merge but checks for equal material
        /// </summary>
        MergeMaterial = 4,

        /// <summary>
        /// Same as Collapse but checks for equal material
        /// </summary>
        CollapseMaterial = 5,

        /// <summary>
        /// Same as CollapseMaterial but also culls faces towards opaque blocks
        /// </summary>
        Liquid = 6,

        MergeSnowLayer = 8
    }
}
