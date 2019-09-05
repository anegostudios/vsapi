using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Server
{
    public interface IPermissionManager
    {
        /// <summary>
        /// Set given role for given player. Role must exist in the serverconfig.json
        /// </summary>
        /// <param name="player"></param>
        /// <param name="role"></param>
        void SetRole(IServerPlayer player, IPlayerRole role);

        /// <summary>
        /// Set given role for given player. Role must exist in the serverconfig.json
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
        void RegisterPrivilege(string code, string shortdescription);

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
