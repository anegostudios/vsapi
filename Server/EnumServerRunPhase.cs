using System;

namespace Vintagestory.API.Server
{
    /// <summary>
    /// These are the stages the server goes through during launch
    /// </summary>
    public enum EnumServerRunPhase
    {
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
        /// Mods will be compiled and started, AssetManager will init all origins and cache assets.
        /// </summary>
        [Obsolete("Use AssetsReady")]
        LoadAssets = 3,
        AssetsReady = 3,

        /// <summary>
        /// All configs loaded, game world loaded by server. Mods receive events at this point.
        /// </summary>
        [Obsolete("Use ModsAndConfigReady")]
        LoadGamePre = 4,
        ModsAndConfigReady = 4,

        /// <summary>
        /// All configs loaded, game world loaded by server. All blocks are loaded.
        /// </summary>
        GameReady = 5,

        /// <summary>
        /// All configs loaded, spawn chunks loaded, game world loaded by server. All blocks are loaded.
        /// </summary>
        [Obsolete("Use GameReady")]
        LoadGame = 5,

        /// <summary>
        /// Spawn chunks now loaded.
        /// </summary>
        WorldReady = 6,

        /// <summary>
        /// About to run first game world tick.
        /// </summary>
        RunGame = 7,

        /// <summary>
        /// Shutdown has begun
        /// </summary>
        Shutdown = 8,

        /// <summary>
        /// Shutdown complete
        /// </summary>
        Exit = 9
    }
}
