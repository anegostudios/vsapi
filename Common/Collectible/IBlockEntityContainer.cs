using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    public interface IBlockEntityContainer
    {
        /// <summary>
        /// The inventory attached to this block entity container
        /// </summary>
        IInventory Inventory { get; }

        /// <summary>
        /// The class name for the inventory.
        /// </summary>
        string InventoryClassName { get; }
    }
}
