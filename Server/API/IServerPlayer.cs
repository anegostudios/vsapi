using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

#nullable disable

namespace Vintagestory.API.Server
{
    /// <summary>
    /// Represents a player on the server side that joined the server at least once. May not be online at this point in time.
    /// </summary>
    public interface IServerPlayer : IPlayer
    {
        event OnEntityAction InWorldAction;

        int ItemCollectMode { get; set; }

        /// <summary>
        /// The "radius" of chunks that the player already received. If set to 0, the server will recheck all nearby chunks if they have been sent or not and send them when necessary
        /// </summary>
        int CurrentChunkSentRadius { get; set; }

        /// <summary>
        /// Retrieves the current connection state of the client
        /// </summary>
        EnumClientState ConnectionState { get; }

        /// <summary>
        /// Get the IP for the given player ID. Returns null if not connected, or when called on client side.
        /// </summary>
        string IpAddress { get; }

        /// <summary>
        /// The language this player is currently using
        /// </summary>
        string LanguageCode { get; }

        /// <summary>
        /// Returns the players ping time in seconds. Returns NaN if not connected or when on client side.
        /// </summary>
        float Ping { get; }

        /// <summary>
        /// The players configuration that is world independent
        /// </summary>
        IServerPlayerData ServerData { get; }


        /// <summary>
        /// Notifies all clients of given players playerdata. Useful when you modified any of the WorldData. Does nothing if this player is not connected. Also sends the player data to the player himself
        /// </summary>
        void BroadcastPlayerData(bool sendInventory = false);


        /// <summary>
        /// Disconnects (kicks) this player from the server. Does nothing if this player is not connected.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Disconnects (kicks) a player from the server with given reason. Does nothing if this player is not connected.
        /// </summary>
        /// <param name = "message">Message displayed to the player</param>
        void Disconnect(string message);

        /// <summary>
        /// Shows a vibrating red text in the players screen. If message is null the client will try to find a language entry using supplied code prefixed with 'ingameerror-' (which is recommended so that the errors are translated to the users local language)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <param name="langparams"> If message is null, these are the arguments passed into the Language translation tool</param>
        void SendIngameError(string code, string message = null, params object[] langparams);


        /// <summary>
        /// Sends a chat message to this player to given groupId. You can use GlobalConstants.GeneralChatGroup as groupId to send it to the players general chat.
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="message"></param>
        /// <param name="chatType"></param>
        /// <param name="data">Optional parameter that can be used to pass on unformated data. Just like stdin/stdout/stderr, you could see this as a separate "programming communication channel"</param>
        void SendMessage(int groupId, string message, EnumChatType chatType, string data = null);

        /// <summary>
        /// Sends a chat message (notification type) to this player, localised to the player's own language independent from the server language
        /// <br/>The message will be string formatted - similar to Lang.Get() - with the specified optional args
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void SendLocalisedMessage(int groupId, string message, params object[] args);

        /// <summary>
        /// Sets the players privilege role. For a list of roles, read sapi.Config.Roles
        /// </summary>
        /// <param name="roleCode"></param>
        void SetRole(string roleCode);

        /// <summary>
        /// Sets a player specific spawn position
        /// </summary>
        /// <param name="pos"></param>
        void SetSpawnPosition(PlayerSpawnPos pos);

        /// <summary>
        /// Removes the player specific spawn position, which means it will default to the role or global default spawn position
        /// </summary>
        void ClearSpawnPosition();


        /// <summary>
        /// Returns the default spawn position.
        /// This method will return the custom spawnpoint if one has been permanently set.
        /// If no custom spawnpoint is present this method will return the global default spawnpoint.
        /// Returns null when called on client side.
        /// </summary>
        /// <param name="consumeSpawnUse">If true, and this spawn point is use limited, will consume one use of it</param>
        /// <returns></returns>
        FuzzyEntityPos GetSpawnPosition(bool consumeSpawnUse);

        void SetModData<T>(string key, T data);
        T GetModData<T>(string key, T defaultValue = default(T));


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
    }
}
