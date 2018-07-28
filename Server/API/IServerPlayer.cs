using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;

namespace Vintagestory.API.Server
{
    /// <summary>
    /// Represents a player on the server side that joined the server at least once. May not be online at this point in time.
    /// </summary>
    public interface IServerPlayer : IPlayer
    {
        /// <summary>
        /// Retrieves the current connection state of the client
        /// </summary>
        EnumClientState ConnectionState { get; }

        /// <summary>
        /// Get the IP for the given player ID. Returns null if not connected, or when called on client side.
        /// </summary>
        string IpAddress { get; }


        /// <summary>
        /// Returns the players ping time in seconds. Returns NaN if not connected or when on client side.
        /// </summary>
        float Ping { get; }

        /// <summary>
        /// The players configuration that is world independent
        /// </summary>
        IServerPlayerData ServerData { get; }

        /// <summary>
        /// Returns the players privilege group
        /// </summary>
        /// <returns></returns>
        IPlayerRole Role { get; }

        /// <summary>
        /// Notifies all clients of given players playerdata. Useful when you modified any of the WorldData. Does nothing if this player is not connected.
        /// </summary>
        void BroadcastPlayerData();


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
        /// Sends a chat message to this player to given groupId. You can use GlobalConstants.GeneralChatGroup as groupId to send it to the players general chat.
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="message"></param>
        /// <param name="chatType"></param>
        /// <param name="data">Optional parameter that can be used to pass on unformated data. Just like stdin/stdout/stderr, you could see this as a separate "programming communication channel"</param>
        void SendMessage(int groupId, string message, EnumChatType chatType, string data = null);


        /// <summary>
        /// Check if a player has the given privilege
        /// </summary>
        /// <param name = "privilegeCode">The privilege to check</param>
        /// <returns>true if the player has the given privilege, false otherwise</returns>
        bool HasPrivilege(string privilegeCode);


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
        EntityPos SpawnPosition { get; }


        /// <summary>
        /// Tells the server send a position packet to the client
        /// </summary>
        void SendPositionToClient();
    }
}
