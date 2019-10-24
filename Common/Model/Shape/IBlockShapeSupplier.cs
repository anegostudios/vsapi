using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// Adds a tesselator to your block
    /// WARNING: please make sure whatever functions and fields you use with the OnTesselation event are THREAD SAFE!
    /// </summary>
    [Obsolete("No longer needed. All block entities by default will have the OnTesselation method executed")]
    public interface IBlockShapeSupplier
    {
        
    }
}
