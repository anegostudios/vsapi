using System;

#nullable disable

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
        [Obsolete("Use HandTp instead")]
        HandFp,
        /// <summary>
        /// Rendered in the players hand, third person mode
        /// </summary>
        HandTp,
        /// <summary>
        /// Rendered in the players off hand, third person mode
        /// </summary>
        HandTpOff,
        /// <summary>
        /// Rendered when dropped on the ground
        /// </summary>
        Ground
    }
}
