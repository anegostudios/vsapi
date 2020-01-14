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
        public JsonItemStack GroundStack;

        [Obsolete("Use GroundStack instead")]
        public JsonItemStack GrindedStack { 
            get
            {
                return this.GroundStack;
            }
            set
            {
                GroundStack = value;
            }
        }

        /// <summary>
        /// Makes a deep copy of the properties.
        /// </summary>
        /// <returns></returns>
        public GrindingProperties Clone()
        {
            return new GrindingProperties()
            {
                GroundStack = this.GroundStack.Clone()
            };
        }
    }
}
