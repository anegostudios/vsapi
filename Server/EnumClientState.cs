
#nullable disable
namespace Vintagestory.API.Server
{
    /// <summary>
    /// The current connection state of a player thats currently connecting to the server
    /// </summary>
    public enum EnumClientState
    {
        Offline = 0,
        Connecting = 1,
        Connected = 2,
        Playing = 3,
        Queued = 4,
    }
}
