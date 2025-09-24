using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Server
{
    /// <summary>
    /// Contains methods to hook into various server processes
    /// </summary>
    public interface IServerEventAPI : IEventAPI
    {
        /// <summary>
        /// Returns the list of currently registered map chunk generator handlers for given world type. Returns an array of handler lists. Each element in the array represents all the handlers for one worldgenpass (see EnumWorldGenPass)
        /// When world type is null, all handlers are returned
        /// </summary>
        /// <param name="worldType">"standard" for the vanilla world generator</param>
        /// <returns></returns>
        IWorldGenHandler GetRegisteredWorldGenHandlers(string worldType);


        bool TriggerTrySpawnEntity(IBlockAccessor blockAccessor, ref EntityProperties properties, Vec3d position, long herdId);

        /// <summary>
        /// If you require neighbour chunk data during world generation, you have to register to this event to receive access to the chunk generator thread. This method is only called once during server startup.
        /// </summary>
        /// <param name="handler"></param>
        void GetWorldgenBlockAccessor(WorldGenThreadDelegate handler);

        /// <summary>
        /// Triggered before the first chunk, map chunk or map region is generated, given that the passed on world type has been selected. Called right after the save game has been loaded.
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="forWorldType"></param>
        void InitWorldGenerator(Action handler, string forWorldType);

        /// <summary>
        /// Event that is triggered whenever a new column of chunks is being generated. It is always called before the ChunkGenerator event
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="forWorldType">For which world types to use this generator</param>
        void MapChunkGeneration(MapChunkGeneratorDelegate handler, string forWorldType);

        /// <summary>
        /// Event that is triggered whenever a new 16x16 section of column of chunks is being generated. It is always called before the ChunkGenerator and before the MapChunkGeneration event
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="forWorldType">For which world types to use this generator</param>
        void MapRegionGeneration(MapRegionGeneratorDelegate handler, string forWorldType);

        /// <summary>
        /// Vintagestory uses this method to generate the basic terrain (base terrain + rock strata + caves) in full columns. Only called once in pass EnumWorldGenPass.TerrainNoise. Register to this event if you need acces to a whole chunk column during inital generation.
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="pass"></param>
        /// <param name="forWorldType">For which world types to use this generator</param>
        void ChunkColumnGeneration(ChunkColumnGenerationDelegate handler, EnumWorldGenPass pass, string forWorldType);

        /// <summary>
        /// Registers a method to be called by certain special worldgen triggers, for example Resonance Archives entrance staircase
        /// </summary>
        void WorldgenHook(WorldGenHookDelegate handler, string forWorldType, string hook);
        /// <summary>
        /// Trigger the special worldgen hook, with the name "hook", if it exists
        /// </summary>
        void TriggerWorldgenHook(string hook, IBlockAccessor blockAccessor, BlockPos pos, string param);

        /// <summary>
        /// Called when just loaded (or generated) a full chunkcolumn
        /// </summary>
        event ChunkColumnBeginLoadChunkThread BeginChunkColumnLoadChunkThread;

        /// <summary>
        /// Called whenever the server loaded from disk or fully generated a chunkcolumn
        /// </summary>
        event ChunkColumnLoadedDelegate ChunkColumnLoaded;

        /// <summary>
        /// Called just before a chunk column is about to get unloaded. On shutdown this method is called for all loaded chunks, so this method can get called tens of thousands of times there, beware
        /// </summary>
        event ChunkColumnUnloadDelegate ChunkColumnUnloaded;




        /// <summary>
        /// Registers a handler to be called every time a player uses a block. The methods return value determines if the player may place/break this block.
        /// </summary>
        event CanUseDelegate CanUseBlock;

        /// <summary>
        /// Called when the server attempts to spawn given entity. Return false to deny spawning.
        /// </summary>
        event TrySpawnEntityDelegate OnTrySpawnEntity;

        /// <summary>
        /// Called when a player interacts with an entity
        /// </summary>
        event OnInteractDelegate OnPlayerInteractEntity;

        /// <summary>
        /// Called when a new player joins
        /// </summary>
        event PlayerDelegate PlayerCreate;

        /// <summary>
        /// Called when a player got respawned
        /// </summary>
        event PlayerDelegate PlayerRespawn;

        /// <summary>
        /// Called when a player joins
        /// </summary>
        event PlayerDelegate PlayerJoin;

        /// <summary>
        /// Called when a player joins and his client is now fully loaded and ready to play
        /// </summary>
        event PlayerDelegate PlayerNowPlaying;

        /// <summary>
        /// Called when a player intentionally leaves
        /// </summary>
        event PlayerDelegate PlayerLeave;

        /// <summary>
        /// Called whenever a player disconnects (timeout, leave, disconnect, kick, etc.).
        /// </summary>
        event PlayerDelegate PlayerDisconnect;

        /// <summary>
        /// Called when a player wrote a chat message
        /// </summary>
        event PlayerChatDelegate PlayerChat;

        /// <summary>
        /// Called when a player died
        /// </summary>
        event PlayerDeathDelegate PlayerDeath;

        /// <summary>
        /// Whenever a player switched his game mode or has it switched for him
        /// </summary>
        event PlayerDelegate PlayerSwitchGameMode;

        /// <summary>
        /// Fired before a player changes their active slot (such as selected hotbar slot).
        /// Allows for the event to be cancelled depending on the return value.
        /// </summary>
        event API.Common.Func<IServerPlayer, ActiveSlotChangeEventArgs, EnumHandling> BeforeActiveSlotChanged;

        /// <summary>
        /// Fired after a player changes their active slot (such as selected hotbar slot).
        /// </summary>
        event Action<IServerPlayer, ActiveSlotChangeEventArgs> AfterActiveSlotChanged;

        /// <summary>
        /// Triggered after assets have been loaded and parsed and registered, but before they are declared to be ready - e.g. you can add more behaviors here, or make other code-based changes to properties read from JSONs
        /// <br/>Note: modsystems should register for this in a Start() method not StartServerSide(): the AssetsFinalizer event is fired before StartServerSide() is reached
        /// </summary>
        [Obsolete("Override Method Modsystem.AssetsFinalize instead")]
        event Action AssetsFinalizers;

        /// <summary>
        /// Triggered after the game world data has been loaded. At this point all blocks are loaded and the Map size is known.
        /// <br/><br/>In 1.17+ do NOT use this server event to add or update behaviors or attributes or other fixed properties of any block, item or entity, in code (additional to what is read from JSON).
        /// Instead, code which needs to do that should be registered for event sapi.Event.AssetsFinalizers.  See VSSurvivalMod system BlockReinforcement.cs for an example.
        /// </summary>
        event Action SaveGameLoaded;

        /// <summary>
        /// Triggered after a savegame has been created - i.e. when a new world was created
        /// </summary>
        event Action SaveGameCreated;

        /// <summary>
        /// Triggered when starting up worldgen during server startup (as the final stage of the WorldReady EnumServerRunPhase)
        /// </summary>
        event Action WorldgenStartup;

        /// <summary>
        /// Triggered when a new multithreaded physics thread starts (for example, use this to initialise any ThreadStatic element which must be initialised per-thread)
        /// </summary>
        event Action PhysicsThreadStart;

        /// <summary>
        /// Triggered before the game world data is being saved to disk
        /// </summary>
        event Action GameWorldSave;

        /// <summary>
        /// Called when something wants to pause the server, e.g. the autosave system. This method will be called every 50ms until all delegates return Ready state. Timeout is 60 seconds.
        /// </summary>
        event SuspendServerDelegate ServerSuspend;

        /// <summary>
        /// Called when something wants to resume execution of the server, e.g. the autosave system
        /// </summary>
        event ResumeServerDelegate ServerResume;

        /// <summary>
        /// Triggered whenever the server enters a new run phase. Since mods are only loaded during run phase "LoadGamePre" registering to any earlier event will get triggered.
        /// </summary>
        /// <param name="runPhase"></param>
        /// <param name="handler"></param>
        void ServerRunPhase(EnumServerRunPhase runPhase, Action handler);

        /// <summary>
        /// Registers a method to be called every given interval
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="interval"></param>
        void Timer(Action handler, double interval);
        object TriggerInitWorldGen();

        /// <summary>
        /// Registers a method to be called every time a player places a block
        /// </summary>
        event BlockPlacedDelegate DidPlaceBlock;

        /// <summary>
        /// Registers a handler to be called every time a player places a block. The methods return value determines if the player may place/break this block. When returning false the client will be notified and the action reverted
        /// </summary>
        event CanPlaceOrBreakDelegate CanPlaceOrBreakBlock;

        /// <summary>
        /// Called when a block should got broken now (that has been broken by a player). Set handling to PreventDefault to handle the block breaking yourself. Otherwise the engine will break the block (= either call heldItemstack.Collectible.OnBlockBrokenWith when player holds something in his hands or block.OnBlockBroken).
        /// </summary>
        event BlockBreakDelegate BreakBlock;

        /// <summary>
        /// Registers a method to be called every time a player deletes a block. Called after the block was already broken
        /// </summary>
        event BlockBrokenDelegate DidBreakBlock;

        /// <summary>
        /// Registers a method to be called every time a player uses a block
        /// </summary>
        event BlockUsedDelegate DidUseBlock;

        /// <summary>
        /// Triggers an immediate ClientAwarenessEvent for the specified player
        /// </summary>
        /// <param name="player"></param>
        void PlayerChunkTransition(IServerPlayer player);

        /// <summary>
        /// Registers a method to be called every time the server receives the gait from a client-authoritative mount entity
        /// </summary>
        event MountGaitReceivedDelegate MountGaitReceived;
        void TriggerMountGaitReceived(Entity mountEntity, string gaitCode);
    }
}
