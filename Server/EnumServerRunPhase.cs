using System;

#nullable disable

namespace Vintagestory.API.Server
{
    /// <summary>
    /// These are the stages the server goes through during launch
    /// </summary>
    public enum EnumServerRunPhase
    {
        /// <summary>
        /// Server is listening to sockets but nothing has been launched yet
        /// </summary>
        Standby = -1,

        /// <summary>
        /// Before anything has been initialized (you cannot receive any events at this point)
        /// </summary>
        Start = 0,

        /// <summary>
        /// Server only stuff initialized, Serversystems instantatied, now initializing ServerSystems then Mods. Serversystem receive events at this point.
        /// </summary>
        Initialization = 1,

        /// <summary>
        /// Everything initialized, now loading config
        /// </summary>
        Configuration = 2,

        /// <summary>
        /// Mods will be compiled and started, AssetManager will init all origins and cache assets. Mods are initialized at this point (PreStart/Start method)
        /// </summary>
        [Obsolete("Use AssetsReady")]
        LoadAssets = 3,
        AssetsReady = 3,

        /// <summary>
        /// Final touches for loaded assets. Mods receive AssetsFinalize() call
        /// </summary>
        AssetsFinalize = 4,

        /// <summary>
        /// All configs loaded, game world loaded by server. Mods receive StartClient/ServerSide and events at this point. 
        /// </summary>
        [Obsolete("Use ModsAndConfigReady")]
        LoadGamePre = 5,
        ModsAndConfigReady = 5,

        /// <summary>
        /// All configs loaded, game world loaded by server. All blocks are loaded.
        /// </summary>
        GameReady = 6,

        /// <summary>
        /// All configs loaded, spawn chunks loaded, game world loaded by server. All blocks are loaded.
        /// </summary>
        [Obsolete("Use GameReady")]
        LoadGame = 6,

        /// <summary>
        /// Notifies mods that the world is ready, after all other loading steps. Following this, worldgen will start and spawn chunks loaded (no separate RunPhase in 1.19)
        /// </summary>
        WorldReady = 7,

        /// <summary>
        /// About to run first game world tick.
        /// </summary>
        RunGame = 8,

        /// <summary>
        /// Shutdown has begun
        /// </summary>
        Shutdown = 9,

        /// <summary>
        /// Shutdown complete
        /// </summary>
        Exit = 10
    }
}
