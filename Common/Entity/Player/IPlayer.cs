using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{

    /// <summary>
    /// Represents a player
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// The block the player is currently aiming at
        /// </summary>
        BlockSelection CurrentBlockSelection { get; }

        /// <summary>
        /// The entity the player is currently aiming at
        /// </summary>
        EntitySelection CurrentEntitySelection { get; }


        /// <summary>
        /// Get the players character name. The character name can be changed every 60 days in the account manager, so don't consider the players name as a unique identifier for a player. Use PlayerUID instead
        /// </summary>
        string PlayerName { get; }

        /// <summary>
        /// Returns the players identifier that is unique across all registered players and will never change. Use this to uniquely identify a player for all eternity. Shorthand for WorldData.PlayerUID
        /// </summary>
        string PlayerUID { get; }

        /// <summary>
        /// The players current client id, 0 if not connected. This is the number thats assigned by the server for any connecting player. You probably don't need this number.
        /// </summary>
        int ClientId { get; }
        
        /// <summary>
        /// The entity the player currently controls
        /// </summary>
        EntityPlayer Entity { get; }

        /// <summary>
        /// Some world-specific information about the player. This object is stored with the save game.
        /// If you modify it server side, be sure to call player.BroadcastPlayerData() to send it to affected clients.
        /// </summary>
        IWorldPlayerData WorldData { get; }

        /// <summary>
        /// Returns the given players inventory manager that let's you do various interesting things with the players inventory.
        /// </summary>
        IPlayerInventoryManager InventoryManager { get; }


        /// <summary>
        /// The list of privileges the player currently has access to (by role or direct assignment)
        /// This list is available for the playing player on the client, but not for other players.
        /// </summary>
        string[] Privileges { get; }



        /*/// <summary>
        /// Sends a redirection request to the specified client. The target server has to be public!
        /// </summary>
        /// <param name = "player"></param>
        /// <param name = "host">The host of the target server</param>
        /// <param name = "name">Name of the server the player gets displayed when connecting to it</param>
        void SendPlayerRedirect(int player, string host, string name);
        */

    }
}
