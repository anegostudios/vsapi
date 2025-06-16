
#nullable disable
namespace Vintagestory.API.Common
{
    public enum EnumWorldAccessResponse
    {
        /// <summary>
        /// Access ok or was called client side
        /// </summary>
        Granted = 0,
        /// <summary>
        /// Players in spectator mode may not place blocks
        /// </summary>
        InSpectatorMode = 1,
        /// <summary>
        /// Player tries to place/break blocks but is in guest mode
        /// </summary>
        InGuestMode = 2,
        /// <summary>
        /// Dead players may not place blocks
        /// </summary>
        PlayerDead = 3,
        /// <summary>
        /// This player was not granted the block build or use privilege
        /// </summary>
        NoPrivilege = 4,
        /// <summary>
        /// Player does not have the build/use blocks every privilege and the position is claimed by another player
        /// </summary>
        LandClaimed = 5,
        /// <summary>
        /// A mod denied use/placement
        /// </summary>
        DeniedByMod = 6
    }
}
