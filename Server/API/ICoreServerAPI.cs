using System;
using Vintagestory.API.Common;

#nullable disable

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
        /// Everything related to roles and privileges
        /// </summary>
        IPermissionManager Permissions { get; }

        /// <summary>
        /// Everything related to player groups
        /// </summary>
        IGroupManager Groups { get; }

        /// <summary>
        /// World-agnostic player data. You can query this information even when the player is offline
        /// </summary>
        IPlayerDataManager PlayerData { get; }

        /// <summary>
        /// API for sending/receiving network packets
        /// </summary>
        new IServerNetworkAPI Network { get; }


        /// <summary>
        /// API for accessing anything in the game world
        /// </summary>
        new IServerWorldAccessor World { get; }



        /// <summary>
        /// Shows a vibrating red text in the players screen. If text is null the client will try to find a language entry using supplied code prefixed with 'ingameerror-' (which is recommended so that the errors are translated to the users local language)
        /// </summary>
        /// <param name="player"></param>
        /// <param name="errorCode"></param>
        /// <param name="text"></param>
        /// <param name="langparams">If text is null, these are the arguments passed into the Language translation tool</param>
        void SendIngameError(IServerPlayer player, string errorCode, string text = null, params object[] langparams);



        /// <summary>
        /// Shows a discovery text on the players screen. If text is null the client will try to find a language entry using supplied code prefixed with 'ingamediscovery-' (which is recommended so that the errors are translated to the users local language)
        /// </summary>
        /// <param name="player"></param>
        /// <param name="discoveryCode"></param>
        /// <param name="text"></param>
        /// <param name="langparams">If text is null, these are the arguments passed into the Language translation tool</param>
        void SendIngameDiscovery(IServerPlayer player, string discoveryCode, string text = null, params object[] langparams);


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
        /// Injects a message or command into the server console input processing system. This lets you run commands or chat as Admin.
        /// </summary>
        /// <param name="message"></param>
        void InjectConsole(string message);

        /// <summary>
        /// Calls a command as if given player called it
        /// </summary>
        /// <param name="player"></param>
        /// <param name="message"></param>
        [Obsolete("Use ChatCommand subapi instead")]
        void HandleCommand(IServerPlayer player, string message);



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
        /// Registers a new tree generator
        /// </summary>
        /// <param name="generatorCode"></param>
        /// <param name="gen"></param>
        void RegisterTreeGenerator(AssetLocation generatorCode, ITreeGenerator gen);

        /// <summary>
        /// Registers a new tree generator
        /// </summary>
        /// <param name="generatorCode"></param>
        /// <param name="genhandler"></param>
        void RegisterTreeGenerator(AssetLocation generatorCode, GrowTreeDelegate genhandler);


        /// <summary>
        /// Registers a chat command. When registered on the client you access the command by prefixing a dot (.), on the server it's a slash (/)
        /// </summary>
        /// <param name="chatcommand"></param>
        /// <returns></returns>
        [Obsolete("Use ChatCommand subapi instead")]
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
        [Obsolete("Use ChatCommand subapi instead")]
        bool RegisterCommand(string command, string descriptionMsg, string syntaxMsg, ServerChatCommandDelegate handler, string requiredPrivilege = null);

        /// <summary>
        /// For internal use: used to remap block and item Ids, as soon as assets are loaded from disk, before recipes etc. are loaded or anything else which may occur in modsystem AssetsLoaded() methods
        /// </summary>
        void TriggerOnAssetsFirstLoaded();
    }
}