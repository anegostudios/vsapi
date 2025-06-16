using System.Collections.Generic;

#nullable disable

namespace Vintagestory.API.Common
{
    public class Entitlement
    {
        public string Code;
        public string Name;
    }

    /// <summary>
    /// Represents a player
    /// </summary>
    public interface IPlayer
    {

        /// <summary>
        /// Returns the players privilege role
        /// </summary>
        /// <returns></returns>
        IPlayerRole Role { get; set; }

        /// <summary>
        /// The players player group memberships
        /// </summary>
        PlayerGroupMembership[] Groups { get; }

        /// <summary>
        /// Load the players group that he is a member of
        /// </summary>
        /// <returns></returns>
        PlayerGroupMembership[] GetGroups();

        /// <summary>
        /// Returns the membership data if player is part of this group, otherwise null
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        PlayerGroupMembership GetGroup(int groupId);

        /// <summary>
        /// List of the users entitlements, vanilla servers will list VIV and/or VS Team member entitlements
        /// </summary>
        List<Entitlement> Entitlements { get; }

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

        bool ImmersiveFpMode { get; }

        /// <summary>
        /// Check if a player has the given privilege
        /// </summary>
        /// <param name = "privilegeCode">The privilege to check</param>
        /// <returns>true if the player has the given privilege, false otherwise</returns>
        bool HasPrivilege(string privilegeCode);



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
