
#nullable disable
namespace Vintagestory.API.Common
{
    public enum EnumDespawnReason
    {
        /// <summary>
        /// Despawned because it died.
        /// </summary>
        Death,

        /// <summary>
        /// Completely burned up
        /// </summary>
        Combusted,

        /// <summary>
        /// Despawned because the player (or players) moved out of ranged.
        /// </summary>
        OutOfRange,

        /// <summary>
        /// A player picked up this item and is removed from the world. (ItemEntity -> Item)
        /// </summary>
        PickedUp,

        /// <summary>
        /// The region was unloaded.
        /// </summary>
        Unload,

        /// <summary>
        /// The last player disconnected from the game.
        /// </summary>
        Disconnect,

        /// <summary>
        /// The entity expired.  
        /// </summary>
        Expire,

        /// <summary>
        /// The entity was removed.
        /// </summary>
        Removed
    }
}
