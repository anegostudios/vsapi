using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// The various render passes available for rendering blocks
    /// </summary>
    public enum EnumChunkRenderPass
    {
        /// <summary>
        /// Default
        /// </summary>
        Opaque = 0,
        /// <summary>
        /// No backface culling
        /// </summary>
        OpaqueNoCull = 1,
        /// <summary>
        /// No backface culling and alpha blending on
        /// </summary>
        BlendNoCull = 2,
        /// <summary>
        /// Uses a special rendering system called Weighted Blended Order Independent Transparency for half transparent blocks
        /// </summary>
        Transparent = 3,
        /// <summary>
        /// Same as OpaqueNoCull but with a few quirks
        /// </summary>
        Liquid = 4,
        /// <summary>
        /// Special render pass for top soil only in order to have climated tinted grass half transparently overlaid over an opaque block
        /// </summary>
        TopSoil = 5,
        /// <summary>
        /// Special render pass for meta blocks
        /// </summary>
        Meta = 6
    }
}
