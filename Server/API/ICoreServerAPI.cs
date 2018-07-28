using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Server
{
    /// <summary>
    /// The core api implemented by the server. The main interface for accessing the server. Contains all sub components and some miscellaneous methods.
    /// </summary>
    public interface ICoreServerAPI : ICoreAPI
    {
        /// <summary>
        /// API Component for registering to various Events
        /// </summary>
        new IServerEventAPI Event { get;  }

        /// <summary>
        /// API Component for access/modify everything game world related
        /// </summary>
        IWorldManagerAPI WorldManager { get; }

       
        /// <summary>
        /// API Component for accessing server related functionality
        /// </summary>
        IServerAPI Server { get; }

        /// <summary>
        /// API for sending/receiving network packets
        /// </summary>
        IServerNetworkAPI Network { get; }


        /// <summary>
        /// API for accessing anything in the game world
        /// </summary>
        new IServerWorldAccessor World { get; }



        /// <summary>
        /// Sends a chat message only to given player in given groupId
        /// </summary>
        /// <param name="player"></param>
        /// <param name="groupId"></param>
        /// <param name="message"></param>
        /// <param name="chatType"></param>
        /// <param name="data">Custom data to a message to be received by the client</param>
        void SendMessage(IPlayer player, int groupId, string message, EnumChatType chatType, string data = null);


        /// <summary>
        /// Sends a chat message to all online players in given player group
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="message"></param>
        /// <param name="chatType"></param>
        /// <param name="data">Custom data to a message to be received by the client</param>
        void SendMessageToGroup(int groupid, string message, EnumChatType chatType, string data = null);

        /// <summary>
        /// Sends a chat message to all online players in all of their channels
        /// </summary>
        /// <param name="message"></param>
        /// <param name="chatType"></param>
        /// <param name="data">Custom data to a message to be received by the client</param>
        void BroadcastMessageToAllGroups(string message, EnumChatType chatType, string data = null);




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
        void GrantPrivilege(string code);

        /// <summary>
        /// Revokes a privilege that has been previously granted to all players. Does not revoke privileges granted from a group. Does nothing if this privilege hasn't been previously granted.
        /// </summary>
        /// <param name="code"></param>
        void RevokePrivilege(string code);

        /// <summary>
        /// Grant a privilege to an individual connected player. 
        /// </summary>
        /// <param name="playerUID"></param>
        /// <param name="code"></param>
        /// <param name="permanent">Wether to store this privilege permanently. Otherwise only valid for the active server session.</param>
        /// <returns>False if player was not found</returns>
        bool GrantPrivilege(string playerUID, string code, bool permanent = false);

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
        /// Grans a new build area where players may build with given permission level
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="z1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="z2"></param>
        /// <param name="permissionLevel"></param>
        void GrantBuildArea(int x1, int y1, int z1, int x2, int y2, int z2, int permissionLevel);

        /// <summary>
        /// Removes a previously granted area
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="z1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="z2"></param>
        void RevokeBuildArea(int x1, int y1, int z1, int x2, int y2, int z2);


        /// <summary>
        /// Returns the players permission level
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        int GetPlayerPermissionLevel(int player);



        

        /// <summary>
        /// Register a new item type
        /// </summary>
        /// <param name="item"></param>
        void RegisterItem(Item item);

        /// <summary>
        /// Register a new Block. Must happen before server runphase LoadGame. These are sent to the client during connect, so only need to register them on the server side.
        /// </summary>
        /// <param name = "block">BlockType to register. The Server assigns a block id and sets block.blockId</param>
        void RegisterBlock(Block block);

        /// <summary>
        /// Registers a new crafting recipe. These are sent to the client during connect, so only need to register them on the server side.
        /// </summary>
        /// <param name="recipe"></param>
        void RegisterCraftingRecipe(GridRecipe recipe);

        /// <summary>
        /// Registers a new metal alloy. These are sent to the client during connect, so only need to register them on the server side.
        /// </summary>
        /// <param name="alloy"></param>
        void RegisterMetalAlloy(AlloyRecipe alloy);

        /// <summary>
        /// Registers a new clay forming recipe. These are sent to the client during connect, so only need to register them on the server side.
        /// </summary>
        /// <param name="recipe"></param>
        void RegisterClayFormingRecipe(ClayFormingRecipe recipe);

        /// <summary>
        /// Registers a new tree generator
        /// </summary>
        /// <param name="generatorCode"></param>
        /// <param name="gen"></param>
        void RegisterTreeGenerator(AssetLocation generatorCode, ITreeGenerator gen);

        /// <summary>
        /// Registers a new metal smithing recipe. These are sent to the client during connect, so only need to register them on the server side.
        /// </summary>
        /// <param name="recipe"></param>
        void RegisterSmithingRecipe(SmithingRecipe recipe);

        /// <summary>
        /// Registers a new flint knapping recipe. These are sent to the client during connect, so only need to register them on the server side.
        /// </summary>
        /// <param name="recipe"></param>
        void RegisterKnappingRecipe(KnappingRecipe recipe);

        /// <summary>
        /// Registers a chat command. When registered on the client you access the command by prefixing a dot (.), on the server it's a slash (/)
        /// </summary>
        /// <param name="chatcommand"></param>
        /// <returns></returns>
        bool RegisterCommand(ServerChatCommand chatcommand);

        /// <summary>
        /// Registers a chat command. When registered on the client you access the command by prefixing a dot (.), on the server it's a slash (/)
        /// </summary>
        /// <param name="command"></param>
        /// <param name="descriptionMsg"></param>
        /// <param name="syntaxMsg"></param>
        /// <param name="handler"></param>
        /// <param name="requiredPrivilege"></param>
        /// <returns></returns>
        bool RegisterCommand(string command, string descriptionMsg, string syntaxMsg, ServerChatCommandDelegate handler, string requiredPrivilege = null);
        
    }
}