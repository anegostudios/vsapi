using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VintagestoryAPI.Common.Collectible.Block
{
    public interface IBlockFlowing
    {
        string Flow { get; set; }
        Vintagestory.API.MathTools.Vec3i FlowNormali { get; set; }
        bool IsLava { get; }
    }
}
