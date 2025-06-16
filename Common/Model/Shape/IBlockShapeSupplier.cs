using System;

#nullable disable

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
