using System;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    /// <summary>
    /// For handling events on the event bus
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="handling">Set to EnumHandling.Last to stop further propagation of the event</param>
    /// <param name="data"></param>
    public delegate void EventBusListenerDelegate(string eventName, ref EnumHandling handling, IAttribute data);

    public enum EnumChunkDirtyReason
    {
        NewlyCreated,
        NewlyLoaded,
        MarkedDirty
    }

    /// <summary>
    /// For handling dirty chunks
    /// </summary>
    /// <param name="chunkCoord"></param>
    /// <param name="chunk"></param>
    /// <param name="reason"></param>
    public delegate void ChunkDirtyDelegate(Vec3i chunkCoord, IWorldChunk chunk, EnumChunkDirtyReason reason);

    /// <summary>
    /// Triggered immediately when the server loads a chunk column from disk or generates a new one, in the SupplyChunks thread (not the main thread)
    /// </summary>
    /// <param name="mapChunk"></param>
    /// <param name="chunkX"></param>
    /// <param name="chunkZ"></param>
    /// <param name="chunks"></param>
    public delegate void ChunkColumnBeginLoadChunkThread(IServerMapChunk mapChunk, int chunkX, int chunkZ, IWorldChunk[] chunks);

    /// <summary>
    /// Triggered when the server loaded a chunk column from disk or generated a new one
    /// </summary>
    /// <param name="chunkCoord"></param>
    /// <param name="chunks"></param>
    public delegate void ChunkColumnLoadedDelegate(Vec2i chunkCoord, IWorldChunk[] chunks);

    /// <summary>
    /// Triggered just before a chunk column gets unloaded
    /// </summary>
    /// <param name="chunkCoord">chunkX and chunkZ of the column (multiply with chunksize to get position). The Y component is zero</param>
    public delegate void ChunkColumnUnloadDelegate(Vec3i chunkCoord);


    public delegate void EntityMountDelegate(EntityAgent mountingEntity, IMountableSeat mountedSeat);

    /// <summary>
    /// Triggered when a server receives a gait update from a client-authoritative mount
    /// </summary>
    /// <param name="mountEntity"></param>
    /// <param name="gaitCode"></param>
    public delegate void MountGaitReceivedDelegate(Entity mountEntity, string gaitCode);

    /// <summary>
    /// Triggered when the server loaded a map region from disk or generated a new one
    /// </summary>
    /// <param name="mapCoord">regionX and regionZ (multiply with region size to get block position)</param>
    /// <param name="region"></param>
    public delegate void MapRegionLoadedDelegate(Vec2i mapCoord, IMapRegion region);

    /// <summary>
    /// Triggered just before a map region gets unloaded
    /// </summary>
    /// <param name="mapCoord">regionX and regionZ (multiply with region size to get block position)</param>
    /// <param name="region"></param>
    public delegate void MapRegionUnloadDelegate(Vec2i mapCoord, IMapRegion region);



    public delegate bool MatchGridRecipeDelegate(IPlayer player, GridRecipe recipe, ItemSlot[] ingredients, int gridWidth);


    public delegate EnumWorldAccessResponse TestBlockAccessDelegate(IPlayer player, BlockSelection blockSel, EnumBlockAccessFlags accessType, ref string claimant, EnumWorldAccessResponse response);
    public delegate EnumWorldAccessResponse TestBlockAccessClaimDelegate(IPlayer player, BlockSelection blockSel, EnumBlockAccessFlags accessType, ref string claimant, LandClaim claim, EnumWorldAccessResponse response);

    /// <summary>
    /// Events that are available on the server and the client
    /// </summary>
    public interface IEventAPI
    {
        event EntityMountDelegate EntityMounted;
        event EntityMountDelegate EntityUnmounted;

        /// <summary>
        /// Called when a player changed dimension
        /// </summary>
        event PlayerCommonDelegate PlayerDimensionChanged;

        /// <summary>
        /// Triggered when block access is tested, allows you to override the engine response
        /// </summary>
        event TestBlockAccessDelegate OnTestBlockAccess;
        event TestBlockAccessClaimDelegate OnTestBlockAccessClaim;

        /// <summary>
        /// Triggered when a new entity spawned
        /// </summary>
        event EntityDelegate OnEntitySpawn;

        /// <summary>
        /// Triggered when a new entity got loaded (either spawned or loaded from disk)
        /// </summary>
        event EntityDelegate OnEntityLoaded;

        event EntityDeathDelegate OnEntityDeath;

        /// <summary>
        /// Triggered when a new entity despawned
        /// </summary>
        event EntityDespawnDelegate OnEntityDespawn;

        /// <summary>
        /// Called whenever a chunk was marked dirty (as in, its blocks or light values have been modified or it got newly loaded or newly created)
        /// </summary>
        event ChunkDirtyDelegate ChunkDirty;

        /// <summary>
        /// Called whenever the server loaded from disk or newly generated a map region
        /// </summary>
        event MapRegionLoadedDelegate MapRegionLoaded;

        /// <summary>
        /// Called just before a map region is about to get unloaded. On shutdown this method is called for all loaded map regions.
        /// </summary>
        event MapRegionUnloadDelegate MapRegionUnloaded;


        /// <summary>
        /// Called whenever any method calls world.BlockAccessor.GetClimateAt(). Used by the survival mod to modify the rainfall and temperature values to adjust for seasonal and day/night temperature variations. Be sure to also register to OnGetClimateForDate.
        /// </summary>
        event OnGetClimateDelegate OnGetClimate;

        /// <summary>
        /// Called whenever any method calls world.BlockAccessor.GetWindSpeedAt(). Used by the survival mod to set the wind speed
        /// </summary>
        event OnGetWindSpeedDelegate OnGetWindSpeed;

        /// <summary>
        /// Called when a player tries to gridcraft something
        /// </summary>
        event MatchGridRecipeDelegate MatchesGridRecipe;

        /// <summary>
        /// There's 2 global event busses, 1 on the client and 1 on the server. This pushes an event onto the bus.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        void PushEvent(string eventName, IAttribute data = null);


        /// <summary>
        /// Registers a listener on the event bus. This is intended for mods as the game engine itself does not push any events.
        /// </summary>
        /// <param name="OnEvent">The handler for the events</param>
        /// <param name="priority">Set this to a different value if you want to catch an event before/after another mod catches it</param>
        /// <param name="filterByEventName">If set, events only with given eventName are received</param>
        void RegisterEventBusListener(EventBusListenerDelegate OnEvent, double priority = 0.5, string filterByEventName = null);


        /// <summary>
        /// Calls given method after every given interval until unregistered. The engine may call your method slightly later since these event are handled only during fixed interval game ticks.
        /// </summary>
        /// <param name="onGameTick"></param>
        /// <param name="millisecondInterval"></param>
        /// <param name="initialDelayOffsetMs"></param>
        /// <returns>listenerId</returns>
        long RegisterGameTickListener(Action<float> onGameTick, int millisecondInterval, int initialDelayOffsetMs = 0);


        /// <summary>
        /// Calls given method after every given interval until unregistered. The engine may call your method slightly later since these event are handled only during fixed interval game ticks.
        /// This overload includes an ErrorHandler callback, triggered if calling onGameTick throws an exception
        /// </summary>
        /// <param name="onGameTick"></param>
        /// <param name="errorHandler"></param>
        /// <param name="millisecondInterval"></param>
        /// <param name="initialDelayOffsetMs"></param>
        /// <returns>listenerId</returns>
        long RegisterGameTickListener(Action<float> onGameTick, Action<Exception> errorHandler, int millisecondInterval, int initialDelayOffsetMs = 0);


        /// <summary>
        /// Calls given method after every given interval until unregistered. The engine may call your method slightly later since these event are handled only during fixed interval game ticks.
        /// </summary>
        /// <param name="onGameTick"></param>
        /// <param name="pos"></param>
        /// <param name="millisecondInterval"></param>
        /// <param name="initialDelayOffsetMs"></param>
        /// <returns>listenerId</returns>
        long RegisterGameTickListener(Action<IWorldAccessor, BlockPos, float> onGameTick, BlockPos pos, int millisecondInterval, int initialDelayOffsetMs = 0);




        /// <summary>
        /// Calls given method after supplied amount of milliseconds. The engine may call your method slightly later since these event are handled only during fixed interval game ticks.
        /// </summary>
        /// <param name="OnTimePassed"></param>
        /// <param name="millisecondDelay"></param>
        /// <returns>listenerId</returns>
        long RegisterCallback(Action<float> OnTimePassed, int millisecondDelay);

        /// <summary>
        /// Calls given method after supplied amount of milliseconds. The engine may call your method slightly later since these event are handled only during fixed interval game ticks.
        /// This overload can be used to signify callbacks which do no harm if registered while the game is paused (otherwise, registering a callback while paused will produce an error in logs, or an intentional exception in Developer Mode)
        /// </summary>
        /// <param name="OnTimePassed"></param>
        /// <param name="millisecondDelay"></param>
        /// <param name="permittedWhilePaused"></param>
        /// <returns>listenerId</returns>
        long RegisterCallback(Action<float> OnTimePassed, int millisecondDelay, bool permittedWhilePaused);

        /// <summary>
        /// Calls given method after supplied amount of milliseconds, lets you supply a block position to be passed to the method. The engine may call your method slightly later since these event are handled only during fixed interval game ticks.
        /// </summary>
        /// <param name="OnTimePassed"></param>
        /// <param name="pos"></param>
        /// <param name="millisecondDelay"></param>
        /// <returns>listenerId</returns>
        long RegisterCallback(Action<IWorldAccessor, BlockPos, float> OnTimePassed, BlockPos pos, int millisecondDelay);


        /// <summary>
        /// Removes a delayed callback
        /// </summary>
        /// <param name="listenerId"></param>
        void UnregisterCallback(long listenerId);


        /// <summary>
        /// Removes a game tick listener
        /// </summary>
        /// <param name="listenerId"></param>
        void UnregisterGameTickListener(long listenerId);


        /// <summary>
        /// Can be used to execute supplied method a frame later or can be called from a seperate thread to ensure some code is executed in the main thread. Calling this method is thread safe.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="code">Task category identifier for the frame profiler</param>
        void EnqueueMainThreadTask(Action action, string code);


        void TriggerPlayerDimensionChanged(IPlayer player);
        void TriggerEntityDeath(Entity entity, DamageSource damageSourceForDeath);
        bool TriggerMatchesRecipe(IPlayer forPlayer, GridRecipe gridRecipe, ItemSlot[] ingredients, int gridWidth);
        void TriggerEntityMounted(EntityAgent entityAgent, IMountableSeat entityRideableSeat);
        void TriggerEntityUnmounted(EntityAgent entityAgent, IMountableSeat entityRideableSeat);
    }
}
