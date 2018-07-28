using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    public class GrindingProperties
    {
        /// <summary>
        /// If set, the block/item is grindable in a quern and this is the resulting itemstack once the grinding time is over.
        /// </summary>
        public JsonItemStack GrindedStack;

        public GrindingProperties Clone()
        {
            return new GrindingProperties()
            {
                GrindedStack = this.GrindedStack.Clone()
            };
        }
    }
}
