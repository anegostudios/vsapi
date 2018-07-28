using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace Vintagestory.API.Common
{
    public class DummySlot : ItemSlot
    {
        public DummySlot(ItemStack stack) : base(null)
        {
            this.itemstack = stack;
        }
    }
}
