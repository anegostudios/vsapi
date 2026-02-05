
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
        /// Server will: verify the players picking range when placing, removing or interacting with blocks
        /// </summary>
        Basic,
        /// <summary>
        /// Not used yet
        /// </summary>
        Pedantic
    }
}
