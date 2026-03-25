
#nullable disable
namespace Vintagestory.API.Server
{
    /// <summary>
    /// The current connection state of a player thats currently connecting to the server
    /// </summary>
    public enum EnumClientState
    {
        Offline = 0,
        /// <summary>
        /// Is a newly opened connection, subject to verfication
        /// </summary>
        Connecting = 1,
        /// <summary>
        /// Has passed the MaxClients Test
        /// </summary>
        Admitted = 2,
        /// <summary>
        /// The players client sent us a packet that it loaded
        /// </summary>
        Connected = 3,
        /// <summary>
        /// The players client sent us a packet that this player is now actually playing (after character selection)
        /// </summary>
        Playing = 4,
        /// <summary>
        /// Is in the connection queue without initialized player data
        /// </summary>
        Queued = 5
    }

    public static class EnumClientStateExtensions
    {
        public static bool ConnectedOrPlaying(this EnumClientState state)
        {
            return state == EnumClientState.Connected || state == EnumClientState.Playing;
        }

        public static bool IsAdmitted(this EnumClientState state)
        {
            return state == EnumClientState.Admitted || state == EnumClientState.Connected || state == EnumClientState.Playing;
        }
    }
}
