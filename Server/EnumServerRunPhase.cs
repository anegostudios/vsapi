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
        Start,

        /// <summary>
        /// Server only stuff initialized, Serversystems instantatied, now initializing ServerSystems then Mods. Serversystem receive events at this point.
        /// </summary>
        Initialization,

        /// <summary>
        /// Everything initialized, now loading config
        /// </summary>
        Configuration,

        /// <summary>
        /// Mods will be compiled, AssetManager will init all origins and cache assets.
        /// </summary>
        LoadAssets,

        /// <summary>
        /// All configs loaded, game world loaded by server. Mods receive events at this point.
        /// </summary>
        LoadGamePre,

        /// <summary>
        /// All configs loaded, game world loaded by server. All blocks are loaded.
        /// </summary>
        LoadGame,

        /// <summary>
        /// About to run first game world tick. Spawnchunks get loaded.
        /// </summary>
        RunGame,

        /// <summary>
        /// Shutdown has begun
        /// </summary>
        Shutdown,

        /// <summary>
        /// Shutdown complete
        /// </summary>
        Exit 
    }
}
