using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    public enum EnumItemDamageSource
    {
        /// <summary>
        /// The item was breaking a block.
        /// </summary>
        BlockBreaking,

        /// <summary>
        /// The item was attacking a creature.
        /// </summary>
        Attacking,

        /// <summary>
        /// the item was thrown into a fire.
        /// </summary>
        Fire
    }
}
