using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public interface IEdibleBlock
    {
        bool IsEdibleFor(BlockPos pos, EntityAgent entity);

    }
}
