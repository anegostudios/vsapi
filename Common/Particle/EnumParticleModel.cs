
#nullable disable
namespace Vintagestory.API.Common
{
    /// <summary>
    /// Types of model for a particle.
    /// </summary>
    [DocumentAsJson]
    public enum EnumParticleModel
    {
        /// <summary>
        /// A 2D quad.
        /// </summary>
        Quad = 0,

        /// <summary>
        /// A 3D cube.
        /// </summary>
        Cube = 1
    }
}
