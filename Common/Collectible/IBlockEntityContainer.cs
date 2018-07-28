using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    public interface IBlockEntityContainer
    {
        IInventory Inventory { get; }
        string InventoryClassName { get; }
    }
}
