
#nullable disable
namespace Vintagestory.API.Common
{
    /// <summary>
    /// Types for how an item can damage it's durability.
    /// </summary>
    [DocumentAsJson]
    public enum EnumItemDamageSource
    {
        /// <summary>
        /// The item was breaking a block.
        /// </summary>
        BlockBreaking,

        /// <summary>
        /// The item was attacking a creature.
        /// </summary>
        Attacking,

        /// <summary>
        /// Unused. The item was thrown into a fire.
        /// </summary>
        Fire
    }
}
