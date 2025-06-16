
#nullable disable
namespace Vintagestory.API.Common
{
    public enum EnumDeathCause
    {
        /// <summary>
        /// The death was caused by falling too far.
        /// </summary>
        FallDamage,

        /// <summary>
        /// The death was caused by damage from a block.
        /// </summary>
        BlockDamage,

        /// <summary>
        /// The death was caused by loss of getting air when underwater.
        /// </summary>
        Drowning,

        /// <summary>
        /// The death was caused by a strong concussive force to the forehead.
        /// </summary>
        Explosion,

        /// <summary>
        /// The death was caused by an injury the player sustained.
        /// </summary>
        Injury,

        /// <summary>
        /// We don't know what killed ya.
        /// </summary>
        Unknown
    }
}