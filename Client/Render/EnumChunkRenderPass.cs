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
        /// Backfaced culled, no alpha testing, alpha discard
        /// </summary>
        Opaque = 0,
        /// <summary>
        /// Backfaced not culled, no alpha blended but alpha discard
        /// </summary>
        OpaqueNoCull = 1,
        /// <summary>
        /// Backfaced not culled, alpha blended and alpha discard
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
