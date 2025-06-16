
#nullable disable
namespace Vintagestory.API.Common
{
    /// <summary>
    /// A players game mode
    /// </summary>
    public enum EnumGameMode
    {
        /// <summary>
        /// Can not place or remove blocks, but can interact with blocks and entities
        /// </summary>
        Guest,
        /// <summary>
        /// May not fly or break blocks immediately
        /// </summary>
        Survival,
        /// <summary>
        /// Can fly, break blocks immediately, etc.
        /// </summary>
        Creative,
        /// <summary>
        /// Can fly but may not interact with the world in any way
        /// </summary>
        Spectator
    }
}