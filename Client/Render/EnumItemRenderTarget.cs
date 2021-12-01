using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// The render taget for an item stack
    /// </summary>
    public enum EnumItemRenderTarget
    {
        /// <summary>
        /// Rendered in a UI, usually the inventory
        /// </summary>
        Gui,
        /// <summary>
        /// Rendered in the players hand, first person mode
        /// </summary>
        HandFp,
        /// <summary>
        /// Rendered in the players hand, third person mode
        /// </summary>
        HandTp,
        /// <summary>
        /// Rendered when dropped on the ground
        /// </summary>
        Ground
    }
}
