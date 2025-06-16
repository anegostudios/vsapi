
#nullable disable
namespace Vintagestory.API.Common
{
    /// <summary>
    /// When picking a random seed for this block, what axes should we base it on?
    /// </summary>
    [DocumentAsJson]
    public enum EnumRandomizeAxes
    {
        /// <summary>
        /// Create a random value based on all three axes. 
        /// </summary>
        XYZ,

        /// <summary>
        /// Create a random value based only on the X and Z axes. Allows blocks placed on top of each other to all have the same random properties (e.g. size, rotation, offset).
        /// Commonly used for multiblock plants.
        /// </summary>
        XZ
    }
}
