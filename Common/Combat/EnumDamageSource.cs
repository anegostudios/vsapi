
#nullable disable
namespace Vintagestory.API.Common
{
    public enum EnumDamageSource
    {
        /// <summary>
        /// It came from a block in the world.
        /// </summary>
        Block, 

        /// <summary>
        /// It from another player.
        /// </summary>
        Player,

        /// <summary>
        /// It came from falling too far.
        /// </summary>
        Fall,

        /// <summary>
        /// It came from being in water too long.
        /// </summary>
        Drown,

        /// <summary>
        /// It came from respawning.
        /// </summary>
        Revive,

        /// <summary>
        /// It came from the void.
        /// </summary>
        Void,

        /// <summary>
        /// It came from the /kill command.
        /// </summary>
        Suicide,         // /kill command

        /// <summary>
        /// It came from inside.
        /// </summary>
        Internal,

        /// <summary>
        /// It came from another entity.
        /// </summary>
        Entity,

        /// <summary>
        /// It came from an explostion.
        /// </summary>
        Explosion,

        Machine,

        /// <summary>
        /// It came from a source not identified.
        /// </summary>
        Unknown,

        Weather,

        /// <summary>
        /// It came from entity bleeding
        /// </summary>
        Bleed
    }
}
