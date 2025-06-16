
#nullable disable
namespace Vintagestory.API.Common
{

    /// <summary>
    /// Some world-specific information about a connected player. If you want modify any value, also broadcast the playerdata to all connected clients.
    /// This is the object that stored and loaded with the save game
    /// </summary>
    public interface IWorldPlayerData
    {
        /// <summary>
        /// The players unique identifier
        /// </summary>
        string PlayerUID { get; }

        /// <summary>
        /// The player entity this player is currently controlling
        /// </summary>
        EntityPlayer EntityPlayer { get; }

        /// <summary>
        /// The controls that moves around the EntityPlayer
        /// </summary>
        EntityControls EntityControls { get; }

        /// <summary>
        /// The players viewing distance in blocks that is allowed by the server
        /// </summary>
        int LastApprovedViewDistance { get; set; }

        /// <summary>
        /// The players desired viewing distance in blocks
        /// </summary>
        int DesiredViewDistance { get; set; }

        /// <summary>
        /// The players current game mode. Will return Spectator mode while the player is connecting.
        /// </summary>
        EnumGameMode CurrentGameMode { get; set; }


        /// <summary>
        /// Whether the player can freely fly around
        /// </summary>
        bool FreeMove { get; set; }

        /// <summary>
        /// Whether the player is forcefully kept on vertical or horizontal plane during freemove
        /// </summary>
        EnumFreeMovAxisLock FreeMovePlaneLock { get; set; }

        /// <summary>
        /// Affected by collision boxes or not
        /// </summary>
        bool NoClip { get; set; }

        /// <summary>
        /// The players movement speed
        /// </summary>
        float MoveSpeedMultiplier { get; set; }

        /// <summary>
        /// Range of selectable blox
        /// </summary>
        float PickingRange { get; set; }

        /// <summary>
        /// Block selection mode
        /// </summary>
        bool AreaSelectionMode { get; set; }


        int Deaths { get; }

        /// <summary>
        /// Allows setting of arbitrary, permanantly stored moddata attached to this player. Not synced to client.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        void SetModdata(string key, byte[] data);

        /// <summary>
        /// Removes the permanently stored mod data
        /// </summary>
        /// <param name="key"></param>
        void RemoveModdata(string key);

        /// <summary>
        /// Retrieve arbitrary, permantly stored mod data
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        byte[] GetModdata(string key);

        void SetModData<T>(string key, T data);
        T GetModData<T>(string key, T defaultValue = default(T));
    }
}
