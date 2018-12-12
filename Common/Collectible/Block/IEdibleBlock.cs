using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Determines whether or not the block can be edited.
    /// </summary>
    public interface IEdibleBlock
    {
        bool IsEdibleFor(BlockPos pos, EntityAgent entity);

    }
}
