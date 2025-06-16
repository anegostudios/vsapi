
#nullable disable
namespace Vintagestory.API.Server
{
    /// <summary>
    /// How strongly the Server should protect against hacking
    /// </summary>
    public enum EnumProtectionLevel
    {
        /// <summary>
        /// No verification or protection of any kind
        /// </summary>
        Off,
        /// <summary>
        /// Server will: Simulate each players movements and correct the position if the client reports the player at a different positions, verify the players picking range when placing or removing blocks
        /// </summary>
        Basic,
        /// <summary>
        /// Not used yet
        /// </summary>
        Pedantic
    }
}