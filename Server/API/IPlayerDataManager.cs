using System;
using System.Collections.Generic;
using Vintagestory.API.Common;

#nullable disable

namespace Vintagestory.API.Server
{
    public enum EnumServerResponse
    {
        Good,
        Bad,
        Offline
    }


    public interface IPlayerDataManager
    {
        /// <summary>
        /// Returns a copy of the player data dictionary loaded by the server. Thats the contents of Playerdata/playerdata.json
        /// </summary>
        Dictionary<string, IServerPlayerData> PlayerDataByUid { get; }

        /// <summary>
        /// Retrieve a players offline, world-agnostic data by player uid
        /// </summary>
        /// <param name="playerUid"></param>
        /// <returns></returns>
        IServerPlayerData GetPlayerDataByUid(string playerUid);
        /// <summary>
        /// Retrieve a players offline, world-agnostic data by his last known name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IServerPlayerData GetPlayerDataByLastKnownName(string name);

        /// <summary>
        /// Resolves a player name to a player uid, independent on whether this player is online, offline or never even joined the server. This is done by contacting the auth server, so please use this method sparingly.
        /// </summary>
        /// <param name="playername"></param>
        /// <param name="onPlayerReceived"></param>
        void ResolvePlayerName(string playername, Action<EnumServerResponse, string> onPlayerReceived);

        /// <summary>
        /// Resolves a player uid to a player name, independent on whether this player is online, offline or never even joined the server. This is done by contacting the auth server, so please use this method sparingly.
        /// </summary>
        /// <param name="playeruid"></param>
        /// <param name="onPlayerReceived"></param>
        
        void ResolvePlayerUid(string playeruid, Action<EnumServerResponse, string> onPlayerReceived);
    }

    public interface IGroupManager
    {
        Dictionary<int, PlayerGroup> PlayerGroupsById { get; }

        PlayerGroup GetPlayerGroupByName(string name);

        void AddPlayerGroup(PlayerGroup group);

        void RemovePlayerGroup(PlayerGroup group);
    }

    public interface IPermissionManager
    {
        /// <summary>
        /// Retrieve a role by its role code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        IPlayerRole GetRole(string code);

        /// <summary>
        /// Set given role for given player. Role must exist in the serverconfig.json. For a list of roles, read sapi.Config.Roles
        /// </summary>
        /// <param name="player"></param>
        /// <param name="role"></param>
        void SetRole(IServerPlayer player, IPlayerRole role);

        /// <summary>
        /// Set given role for given player. Role must exist in the serverconfig.json. For a list of roles, read sapi.Config.Roles
        /// </summary>
        /// <param name="player"></param>
        /// <param name="roleCode"></param>
        void SetRole(IServerPlayer player, string roleCode);
        

        /// <summary>
        /// Registers a user privilege with the server. Is only active for the current server session and lost during a server restart/shutdown, so register it during server startup.
        /// New privileges are auto-granted to admins and the server console.
        /// </summary>
        /// <param name = "code">Privilege to register</param>
        /// <param name = "shortdescription">Short description</param>
        /// <param name = "adminAutoGrant">By default, super users are automatically granted all privileges. Set this value to false to change that</param>
        void RegisterPrivilege(string code, string shortdescription, bool adminAutoGrant = true);

        /// <summary>
        /// Grants privilege to all players connected or yet to connect. This setting is only active for the current server session and lost during a server restart/shutdown.
        /// </summary>
        /// <param name="code"></param>
        void GrantTemporaryPrivilege(string code);

        /// <summary>
        /// Revokes a privilege that has been previously granted to all players. Does not revoke privileges granted from a group. Does nothing if this privilege hasn't been previously granted.
        /// </summary>
        /// <param name="code"></param>
        void DropTemporaryPrivilege(string code);


        /// <summary>
        /// Grant a privilege to an individual connected player. 
        /// </summary>
        /// <param name="playerUID"></param>
        /// <param name="code"></param>
        /// <param name="permanent">Wether to store this privilege permanently. Otherwise only valid for the active server session.</param>
        /// <returns>False if player was not found</returns>
        bool GrantPrivilege(string playerUID, string code, bool permanent = false);

        /// <summary>
        /// Actively denies a privilege from a player, overrides privileges granted by a role. Does not however override non permanent privileges
        /// </summary>
        /// <param name="playerUID"></param>
        /// <param name="code"></param>
        /// <returns>False if player was not found</returns>
        bool DenyPrivilege(string playerUID, string code);

        /// <summary>
        /// Removes a previously set privilege denial, if any was set.
        /// </summary>
        /// <param name="playerUID"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        bool RemovePrivilegeDenial(string playerUID, string code);

        /// <summary>
        /// Revokes a privilege that has been previously granted to this player. Does not revoke privileges granted from a group. Does nothing if the player does not have given privilege.
        /// </summary>
        /// <param name="playerUID"></param>
        /// <param name="code"></param>
        /// <param name="permanent">If true it removes a previously granted permanent privilege. If false it removes a previously granted temporary privilege.</param>
        /// <returns>False if player was not found</returns>
        bool RevokePrivilege(string playerUID, string code, bool permanent = false);

        /// <summary>
        /// Add given privilege to given group, granting everyone in this group access to this privilege. This setting is only active for the current server session and lost during a server restart/shutdown.
        /// </summary>
        /// <param name="groupCode"></param>
        /// <param name="privilegeCode"></param>
        /// <returns></returns>
        bool AddPrivilegeToGroup(string groupCode, string privilegeCode);

        /// <summary>
        /// Revokes given privilege to given group, revoking everyones access to this privilege inside this group
        /// </summary>
        /// <param name="groupCode"></param>
        /// <param name="privilegeCode"></param>
        /// <returns></returns>
        bool RemovePrivilegeFromGroup(string groupCode, string privilegeCode);


        /// <summary>
        /// Returns the players permission level
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        int GetPlayerPermissionLevel(int player);



    }
}
