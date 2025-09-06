using System;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

#nullable disable

namespace Vintagestory.API.Common
{
    public enum EnumHighlightSlot
    {
        Selection = 0,
        Brush = 1,
        Spawner = 2,
        LandClaim = 3,
        SelectionStart = 4,
        SelectionEnd = 5,
    }



    /// <summary>
    /// Important interface to access the game world.
    /// </summary>
    public interface IWorldAccessor
    {
        /// <summary>
        /// The current world config
        /// </summary>
        ITreeAttribute Config { get; }

        /// <summary>
        /// The default spawn position as sent by the server (usually the map middle). Does not take player specific spawn point into account
        /// </summary>
        EntityPos DefaultSpawnPosition { get; }

        /// <summary>
        /// Gets the frame profiler utility.
        /// </summary>
        FrameProfilerUtil FrameProfiler { get; }

        /// <summary>
        /// The api interface
        /// </summary>
        ICoreAPI Api { get; }

        IChunkProvider ChunkProvider { get; }

        /// <summary>
        /// The land claiming api interface
        /// </summary>
        ILandClaimAPI Claims { get; }

        /// <summary>
        /// Returns a list all loaded chunk positions in the form of a long index. Code to turn that into x/y/z coords:
        /// Vec3i coords = new Vec3i(
        ///    (int)(chunkIndex3d % ChunkMapSizeX),
        ///    (int)(chunkIndex3d / ((long)ChunkMapSizeX * ChunkMapSizeZ)),
        ///    (int)((chunkIndex3d / ChunkMapSizeX) % ChunkMapSizeZ)
        /// );
        /// Retrieving the list is not a very fast process, not suggested to be called every frame
        /// </summary>
        long[] LoadedChunkIndices { get; }

        /// <summary>
        /// Returns a list all loaded chunk positions in the form of a long index
        /// </summary>
        long[] LoadedMapChunkIndices { get; }

        /// <summary>
        /// The currently configured block light brightness levels
        /// </summary>
        float[] BlockLightLevels { get; }

        /// <summary>
        /// The currently configured sun light brightness levels
        /// </summary>
        float[] SunLightLevels { get; }

        /// <summary>
        /// The currently configured sea level (y-coordinate)
        /// </summary>
        int SeaLevel { get; }

        /// <summary>
        /// The world seed. Accessible on the server and the client
        /// </summary>
        int Seed { get; }

        /// <summary>
        /// A globally unique identifier for this savegame
        /// </summary>
        string SavegameIdentifier { get; }

        /// <summary>
        /// The currently configured max sun light level
        /// </summary>
        int SunBrightness { get; }

        /// <summary>
        /// Whether the current side (client/server) is in entity debug mode
        /// </summary>
        bool EntityDebugMode { get; }

        /// <summary>
        /// Loaded game assets
        /// </summary>
        IAssetManager AssetManager { get; }

        /// <summary>
        /// Logging Utility
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// The current side (client/server)
        /// </summary>
        EnumAppSide Side { get; }

        /// <summary>
        /// Access blocks and other world data from loaded chunks, fault tolerant
        /// </summary>
        IBlockAccessor BlockAccessor { get; }

        /// <summary>
        /// Fault tolerant bulk block access to the worlds block data. Since this is a single bulk block access instance the cached data is shared for everything accessing this method, hence should only be accessed from the main thread and any changed comitted within the same game tick. You can however use the WorldManager api to get your own instance of a bulk block accessor
        /// </summary>
        IBulkBlockAccessor BulkBlockAccessor { get; }

        /// <summary>
        /// Interface to create instance of certain classes
        /// </summary>
        IClassRegistryAPI ClassRegistry { get; }

        /// <summary>
        /// Interface to access the game calendar. On the server side only available after run stage 'LoadGamePre' (before that it is null)
        /// </summary>
        IGameCalendar Calendar { get; }

        /// <summary>
        /// For collision testing in the main thread
        /// </summary>
        CollisionTester CollisionTester { get; }

        /// <summary>
        /// Just a random number generator. Makes use of ThreadLocal for thread safety.
        /// </summary>
        Random Rand { get; }

        /// <summary>
        /// Amount of milliseconds ellapsed since startup
        /// </summary>
        long ElapsedMilliseconds { get; }

        /// <summary>
        /// List of all loaded blocks and items without placeholders
        /// </summary>
        List<CollectibleObject> Collectibles { get; }

        /// <summary>
        /// List of all loaded blocks. The array index is the block id. Some may be null or placeholders (then block.code is null). Client-side none are null, what was null return as air blocks.
        /// </summary>
        IList<Block> Blocks { get; }

        /// <summary>
        /// List of all loaded items. The array index is the item id. Some may be placeholders (then item.code is null).  Server-side, some may be null; client-side, a check for item == null is not necessary.
        /// </summary>
        IList<Item> Items { get; }

        /// <summary>
        /// List of all loaded entity types.
        /// </summary>
        List<EntityProperties> EntityTypes { get; }

        /// <summary>
        /// List of the codes of all loaded entity types, in the AssetLocation short string format (e.g. "creature" for entities with domain game:, "domain:creature" for entities with other domains)
        /// </summary>
        List<string> EntityTypeCodes { get; }

        /// <summary>
        /// List of all loaded crafting recipes
        /// </summary>
        List<GridRecipe> GridRecipes { get; }

        /// <summary>
        /// Retrieve a previously registered recipe registry
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        RecipeRegistryBase GetRecipeRegistry(string code);

        /// <summary>
        /// The range in blocks within a client will receive regular updates for an entity
        /// </summary>
        int DefaultEntityTrackingRange { get; }

        /// <summary>
        /// Retrieve the item class from given item id
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        Item GetItem(int itemId);

        /// <summary>
        /// Retrieve the block class from given block id
        /// </summary>
        /// <param name="blockId"></param>
        /// <returns></returns>
        Block GetBlock(int blockId);

        /// <summary>
        /// Returns all blocktypes matching given wildcard
        /// </summary>
        /// <param name="wildcard"></param>
        /// <returns></returns>
        Block[] SearchBlocks(AssetLocation wildcard);

        /// <summary>
        /// Returns all item types matching given wildcard
        /// </summary>
        /// <param name="wildcard"></param>
        /// <returns></returns>
        Item[] SearchItems(AssetLocation wildcard);

        /// <summary>
        /// Retrieve the item class from given item code. Will return null if the item does not exist.
        /// </summary>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        Item GetItem(AssetLocation itemCode);

        /// <summary>
        /// Retrieve the block class from given block code. Will return null if the block does not exist. Logs a warning if block does not exist
        /// </summary>
        /// <param name="blockCode"></param>
        /// <returns></returns>
        Block GetBlock(AssetLocation blockCode);

        /// <summary>
        /// Retrieve the entity class from given entity code. Will return null if the entity does not exist.
        /// </summary>
        /// <param name="entityCode"></param>
        /// <returns></returns>
        EntityProperties GetEntityType(AssetLocation entityCode);

        /// <summary>
        /// Spawns a dropped itemstack at given position. Will immediately disappear if stacksize==0
        /// Returns the entity spawned (may be null!)
        /// </summary>
        /// <param name="itemstack"></param>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        Entity SpawnItemEntity(ItemStack itemstack, Vec3d position, Vec3d velocity = null);

        /// <summary>
        /// Spawns a dropped itemstack at given position. Will immediately disappear if stacksize==0
        /// Returns the entity spawned (may be null!)
        /// </summary>
        /// <param name="itemstack"></param>
        /// <param name="pos"></param>
        /// <param name="velocity"></param>
        Entity SpawnItemEntity(ItemStack itemstack, BlockPos pos, Vec3d velocity = null);

        /// <summary>
        /// Creates a new entity. It's the responsibility of the given Entity to call set its EntityType.
        /// This should be done inside its Initialize method before base.Initialize is called.
        /// </summary>
        /// <param name="entity"></param>
        void SpawnEntity(Entity entity);

        /// <summary>
        /// Like SpawnEntity() but sends out the entity spawn and motion packets to nearby clients immediately. Use this e.g. for more responsive projectiles
        /// </summary>
        /// <param name="entity"></param>
        void SpawnPriorityEntity(Entity entity);

        /// <summary>
        /// Loads a previously created entity into the loadedEntities list. Used when a chunk is loaded.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="fromChunkIndex3d"></param>
        /// <returns></returns>
        bool LoadEntity(Entity entity, long fromChunkIndex3d);

        /// <summary>
        /// Removes an entity from its old chunk and adds it to the chunk with newChunkIndex3d
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="newChunkIndex3d"></param>
        void UpdateEntityChunk(Entity entity, long newChunkIndex3d);

        /// <summary>
        /// Retrieve all entities within given range and given matcher method. If now matcher method is supplied, all entities are returned.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="horRange"></param>
        /// <param name="vertRange"></param>
        /// <param name="matches"></param>
        /// <returns></returns>
        Entity[] GetEntitiesAround(Vec3d position, float horRange, float vertRange, ActionConsumable<Entity> matches = null);

        /// <summary>
        /// Retrieve all entities within a cuboid bound by startPos and endPos. If now matcher method is supplied, all entities are returned.
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="matches"></param>
        /// <returns></returns>
        Entity[] GetEntitiesInsideCuboid(BlockPos startPos, BlockPos endPos, ActionConsumable<Entity> matches = null);

        /// <summary>
        /// Retrieve all players within given range and given matcher method. This method is faster than when using GetEntitiesAround with a matcher for players
        /// </summary>
        /// <param name="position"></param>
        /// <param name="horRange"></param>
        /// <param name="vertRange"></param>
        /// <param name="matches"></param>
        /// <returns></returns>
        IPlayer[] GetPlayersAround(Vec3d position, float horRange, float vertRange, ActionConsumable<IPlayer> matches = null);

        /// <summary>
        /// Retrieve the nearest entity within given range and given matcher method
        /// </summary>
        /// <param name="position"></param>
        /// <param name="horRange"></param>
        /// <param name="vertRange"></param>
        /// <param name="matches"></param>
        /// <returns></returns>
        Entity GetNearestEntity(Vec3d position, float horRange, float vertRange, ActionConsumable<Entity> matches = null);

        /// <summary>
        /// Retrieve an entity by its unique id, returns null if no such entity exists or hasn't been loaded
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        Entity GetEntityById(long entityId);

        /// <summary>
        /// Retrieves the first found entity that intersects any of the supplied collisionboxes offseted by basePos. This is a helper method for you to determine if you can place a block at given position. You can also implement it yourself with intersection testing and GetEntitiesAround()
        /// </summary>
        /// <param name="collisionBoxes"></param>
        /// <param name="basePos"></param>
        /// <param name="matches"></param>
        /// <returns></returns>
        Entity[] GetIntersectingEntities(BlockPos basePos, Cuboidf[] collisionBoxes, ActionConsumable<Entity> matches = null);


        /// <summary>
        /// Find the nearest player to the given position. Thread safe.
        /// </summary>
        /// <param name = "x">x coordinate</param>
        /// <param name = "y">y coordinate</param>
        /// <param name = "z">z coordinate</param>
        /// <returns>ID of the nearest player</returns>
        IPlayer NearestPlayer(double x, double y, double z);

        /// <summary>
        /// Gets a list of all online players. Warning: Also returns currently connecting player whose player data may not have been fully initialized. Check for player.ConnectionState to avoid these.
        /// </summary>
        /// <returns>Array containing the IDs of online players</returns>
        IPlayer[] AllOnlinePlayers { get; }

        /// <summary>
        /// Gets a list of all players that connected to this server at least once while the server was running. When called client side you will receive the same as AllOnlinePlayers
        /// </summary>
        /// <returns>Array containing the IDs of online players</returns>
        IPlayer[] AllPlayers { get; }



        /// <summary>
        /// Retrieves the worldplayer data object of given player. When called server side the player does not need to be connected.
        /// </summary>
        /// <param name="playerUid"></param>
        /// <returns></returns>
        IPlayer PlayerByUid(string playerUid);


        /// <summary>
        /// Plays given sound at given position.
        /// </summary>
        /// <param name="location">The sound path, without sounds/ prefix or the .ogg ending</param>
        /// <param name="posx"></param>
        /// <param name="posy"></param>
        /// <param name="posz"></param>
        /// <param name="dualCallByPlayer">If this call is made on client and on server, set this the causing playerUID to prevent double playing. Essentially dualCall will play the sound on the client, and send it to all other players except source client</param>
        /// <param name="randomizePitch"></param>
        /// <param name="range">The range at which the gain will be attenuated to 1% of the supplied volume</param>
        /// <param name="volume"></param>
        void PlaySoundAt(AssetLocation location, double posx, double posy, double posz, IPlayer dualCallByPlayer = null, bool randomizePitch = true, float range = 32, float volume = 1f);

        /// <summary>
        /// Plays given sound at given position - dimension aware. Plays at the center of the BlockPos
        /// </summary>
        /// <param name="location">The sound path, without sounds/ prefix or the .ogg ending</param>
        /// <param name="pos"></param>
        /// <param name="yOffsetFromCenter">How much above or below the central Y position of the block to play</param>
        /// <param name="dualCallByPlayer">If this call is made on client and on server, set this the causing playerUID to prevent double playing. Essentially dualCall will play the sound on the client, and send it to all other players except source client</param>
        /// <param name="randomizePitch"></param>
        /// <param name="range">The range at which the gain will be attenuated to 1% of the supplied volume</param>
        /// <param name="volume"></param>
        void PlaySoundAt(AssetLocation location, BlockPos pos, double yOffsetFromCenter, IPlayer dualCallByPlayer = null, bool randomizePitch = true, float range = 32, float volume = 1f);

        /// <summary>
        /// Plays given sound at given position.
        /// </summary>
        /// <param name="location">The sound path, without sounds/ prefix or the .ogg ending</param>
        /// <param name="atEntity"></param>
        /// <param name="dualCallByPlayer">If this call is made on client and on server, set this the causing playerUID to prevent double playing. Essentially dualCall will play the sound on the client, and send it to all other players except source client</param>
        /// <param name="randomizePitch"></param>
        /// <param name="range">The range at which the gain will be attenuated to 1% of the supplied volume</param>
        /// <param name="volume"></param>
        void PlaySoundAt(AssetLocation location, Entity atEntity, IPlayer dualCallByPlayer = null, bool randomizePitch = true, float range = 32, float volume = 1f);

        /// <summary>
        /// Plays given sound at given position.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="atEntity"></param>
        /// <param name="dualCallByPlayer"></param>
        /// <param name="pitch"></param>
        /// <param name="range"></param>
        /// <param name="volume"></param>
        void PlaySoundAt(AssetLocation location, Entity atEntity, IPlayer dualCallByPlayer, float pitch, float range = 32, float volume = 1f);

        /// <summary>
        /// Plays given sound at given position.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="posx"></param>
        /// <param name="posy"></param>
        /// <param name="posz"></param>
        /// <param name="dualCallByPlayer"></param>
        /// <param name="pitch"></param>
        /// <param name="range"></param>
        /// <param name="volume"></param>
        void PlaySoundAt(AssetLocation location, double posx, double posy, double posz, IPlayer dualCallByPlayer, float pitch, float range = 32, float volume = 1f);

        void PlaySoundAt(AssetLocation location, double posx, double posy, double posz, IPlayer dualCallByPlayer, EnumSoundType soundType, float pitch, float range = 32, float volume = 1f);

        /// <summary>
        /// Plays given sound at given player position.
        /// </summary>
        /// <param name="location">The sound path, without sounds/ prefix or the .ogg ending</param>
        /// <param name="atPlayer"></param>
        /// <param name="dualCallByPlayer">If this call is made on client and on server, set this the causing playerUID to prevent double playing. Essentially dualCall will play the sound on the client, and send it to all other players except source client</param>
        /// <param name="randomizePitch"></param>
        /// <param name="range">The range at which the gain will be attenuated to 1% of the supplied volume</param>
        /// <param name="volume"></param>
        void PlaySoundAt(AssetLocation location, IPlayer atPlayer, IPlayer dualCallByPlayer = null, bool randomizePitch = true, float range = 32, float volume = 1f);

        /// <summary>
        /// Plays given sound only for given player. Useful when called server side, for the client side there is no difference over using PlaySoundAt or PlaySoundFor
        /// </summary>
        /// <param name="location">The sound path, without sounds/ prefix or the .ogg ending</param>
        /// <param name="forPlayer"></param>
        /// <param name="randomizePitch"></param>
        /// <param name="range">The range at which the gain will be attenuated to 1% of the supplied volume</param>
        /// <param name="volume"></param>
        void PlaySoundFor(AssetLocation location, IPlayer forPlayer, bool randomizePitch = true, float range = 32, float volume = 1f);
        void PlaySoundFor(AssetLocation location, IPlayer forPlayer, float pitch, float range = 32, float volume = 1f);


        /// <summary>
        /// Spawn a bunch of particles
        /// </summary>
        /// <param name="quantity"></param>
        /// <param name="color"></param>
        /// <param name="minPos"></param>
        /// <param name="maxPos"></param>
        /// <param name="minVelocity"></param>
        /// <param name="maxVelocity"></param>
        /// <param name="lifeLength"></param>
        /// <param name="gravityEffect"></param>
        /// <param name="scale"></param>
        /// <param name="model"></param>
        /// <param name="dualCallByPlayer">If this call is made on client and on server, set this to the causing playerUID to prevent double spawning. Essentially dualCall will spawn the particles on the client, and send it to all other players except source client</param>
        void SpawnParticles(float quantity, int color, Vec3d minPos, Vec3d maxPos, Vec3f minVelocity, Vec3f maxVelocity, float lifeLength, float gravityEffect, float scale = 1f, EnumParticleModel model = EnumParticleModel.Quad, IPlayer dualCallByPlayer = null);

        /// <summary>
        /// Spawn a bunch of particles
        /// </summary>
        /// <param name="particlePropertiesProvider"></param>
        /// <param name="dualCallByPlayer">If this call is made on client and on server, set this to the causing playerUID to prevent double spawning. Essentially dualCall will spawn the particles on the client, and send it to all other players except source client</param>
        void SpawnParticles(IParticlePropertiesProvider particlePropertiesProvider, IPlayer dualCallByPlayer = null);


        /// <summary>
        /// Spawn a bunch of particles colored by the block at given position
        /// </summary>
        /// <param name="blockPos">The position of the block to take the color from</param>
        /// <param name="pos">The position where the particles should spawn</param>
        /// <param name="radius"></param>
        /// <param name="quantity"></param>
        /// <param name="scale"></param>
        /// <param name="dualCallByPlayer">If this call is made on client and on server, set this to the causing playerUID to prevent double spawning. Essentially dualCall will spawn the particles on the client, and send it to all other players except source client</param>
        /// <param name="velocity"></param>
        void SpawnCubeParticles(BlockPos blockPos, Vec3d pos, float radius, int quantity, float scale = 1f, IPlayer dualCallByPlayer = null, Vec3f velocity = null);


        /// <summary>
        /// Spawn a bunch of particles colored by given itemstack
        /// </summary>
        /// <param name="pos">The position where the particles should spawn</param>
        /// <param name="item"></param>
        /// <param name="radius"></param>
        /// <param name="quantity"></param>
        /// <param name="scale"></param>
        /// <param name="dualCallByPlayer">If this call is made on client and on server, set this to the causing playerUID to prevent double spawning. Essentially dualCall will spawn the particles on the client, and send it to all other players except source client</param>
        /// <param name="velocity"></param>
        void SpawnCubeParticles(Vec3d pos, ItemStack item, float radius, int quantity, float scale = 1f, IPlayer dualCallByPlayer = null, Vec3f velocity = null);




        /// <summary>
        /// Shoots out a virtual ray at between given positions and stops when the ray hits a block or entity selection box. The block/entity it struck first is then returned by reference.
        /// </summary>
        /// <param name="fromPos"></param>
        /// <param name="toPos"></param>
        /// <param name="blockSelection"></param>
        /// <param name="entitySelection"></param>
        /// <param name="bfilter">Can be used to ignore certain blocks. Return false to ignore</param>
        /// <param name="efilter">Can be used to ignore certain entities. Return false to ignore</param>
        void RayTraceForSelection(Vec3d fromPos, Vec3d toPos, ref BlockSelection blockSelection, ref EntitySelection entitySelection, BlockFilter bfilter = null, EntityFilter efilter = null);


        /// <summary>
        /// Shoots out a virtual ray at between given positions and stops when the ray hits a block or entity intersection box supplied by given supplier. The block/entity it struck first is then returned by reference.
        /// </summary>
        /// <param name="supplier"></param>
        /// <param name="fromPos"></param>
        /// <param name="toPos"></param>
        /// <param name="blockSelection"></param>
        /// <param name="entitySelection"></param>
        /// <param name="bfilter">Can be used to ignore certain blocks. Return false to ignore</param>
        /// <param name="efilter">Can be used to ignore certain entities. Return false to ignore</param>
        void RayTraceForSelection(IWorldIntersectionSupplier supplier, Vec3d fromPos, Vec3d toPos, ref BlockSelection blockSelection, ref EntitySelection entitySelection, BlockFilter bfilter = null, EntityFilter efilter = null);


        /// <summary>
        /// Shoots out a virtual ray at given position and angle and stops when the ray hits a block or entity selection box. The block/entity it struck first is then returned by reference.
        /// </summary>
        /// <param name="fromPos"></param>
        /// <param name="pitch"></param>
        /// <param name="yaw"></param>
        /// <param name="range"></param>
        /// <param name="blockSelection"></param>
        /// <param name="entitySelection"></param>
        /// <param name="bfilter">Can be used to ignore certain blocks. Return false to ignore</param>
        /// <param name="efilter">Can be used to ignore certain entities. Return false to ignore</param>
        void RayTraceForSelection(Vec3d fromPos, float pitch, float yaw, float range, ref BlockSelection blockSelection, ref EntitySelection entitySelection, BlockFilter bfilter = null, EntityFilter efilter = null);

        /// <summary>
        /// Shoots out a given ray and stops when the ray hits a block or entity selection box. The block/entity it struck first is then returned by reference.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="blockSelection"></param>
        /// <param name="entitySelection"></param>
        /// <param name="filter"></param>
        /// <param name="efilter">Can be used to ignore certain entities. Return false to ignore</param>
        void RayTraceForSelection(Ray ray, ref BlockSelection blockSelection, ref EntitySelection entitySelection, BlockFilter filter = null, EntityFilter efilter = null);


        /// <summary>
        /// Calls given method after every given interval until unregistered. The engine may call your method slightly later since these event are handled only during fixed interval game ticks.
        /// </summary>
        /// <param name="onGameTick"></param>
        /// <param name="millisecondInterval"></param>
        /// <param name="initialDelayOffsetMs"></param>
        /// <returns>listenerId</returns>
        long RegisterGameTickListener(Action<float> onGameTick, int millisecondInterval, int initialDelayOffsetMs = 0);

        /// <summary>
        /// Removes a game tick listener
        /// </summary>
        /// <param name="listenerId"></param>
        void UnregisterGameTickListener(long listenerId);

        /// <summary>
        /// Calls given method after supplied amount of milliseconds. The engine may call your method slightly later since these event are handled only during fixed interval game ticks.
        /// </summary>
        /// <param name="OnTimePassed"></param>
        /// <param name="millisecondDelay"></param>
        /// <returns>listenerId</returns>
        long RegisterCallback(Action<float> OnTimePassed, int millisecondDelay);

        /// <summary>
        /// Calls given method after supplied amount of milliseconds. The engine may call your method slightly later since these event are handled only during fixed interval game ticks.
        /// Ignores any subsequent registers for the same blockpos while a callback is still in the queue. Used e.g. for liquid physics to prevent unnecessary multiple updates
        /// </summary>
        /// <param name="OnGameTick"></param>
        /// <param name="pos"></param>
        /// <param name="millisecondInterval"></param>
        /// <returns>listenerId</returns>
        long RegisterCallbackUnique(Action<IWorldAccessor, BlockPos, float> OnGameTick, BlockPos pos, int millisecondInterval);


        /// <summary>
        /// Calls given method after supplied amount of milliseconds, lets you supply a block position to be passed to the method. The engine may call your method slightly later since these event are handled only during fixed interval game ticks.
        /// </summary>
        /// <param name="OnTimePassed"></param>
        /// <param name="pos"></param>
        /// <param name="millisecondDelay"></param>
        /// <returns>listenerId</returns>
        long RegisterCallback(Action<IWorldAccessor, BlockPos, float> OnTimePassed, BlockPos pos, int millisecondDelay);

        /// <summary>
        /// Returns true if given client has a privilege. Always returns true on the client.
        /// </summary>
        /// <param name="clientid"></param>
        /// <param name="privilege"></param>
        /// <returns></returns>
        bool PlayerHasPrivilege(int clientid, string privilege);

        /// <summary>
        /// Removes a delayed callback
        /// </summary>
        /// <param name="listenerId"></param>
        void UnregisterCallback(long listenerId);


        /// <summary>
        /// Utility for testing intersections. Only access from main thread.
        /// </summary>
        AABBIntersectionTest InteresectionTester { get; }




        /// <summary>
        /// Sends given player a list of block positions that should be highlighted
        /// </summary>
        /// <param name="player"></param>
        /// <param name="highlightSlotId">for multiple highlights use a different number</param>
        /// <param name="blocks"></param>
        /// <param name="colors"></param>
        /// <param name="mode"></param>
        /// <param name="shape"></param>
        /// <param name="scale"></param>
        void HighlightBlocks(IPlayer player, int highlightSlotId, List<BlockPos> blocks, List<int> colors, EnumHighlightBlocksMode mode = EnumHighlightBlocksMode.Absolute, EnumHighlightShape shape = EnumHighlightShape.Arbitrary, float scale = 1f);

        /// <summary>
        /// Sends given player a list of block positions that should be highlighted (using a default color)
        /// </summary>
        /// <param name="player"></param>
        /// <param name="highlightSlotId">for multiple highlights use a different number</param>
        /// <param name="blocks"></param>
        /// <param name="mode"></param>
        /// <param name="shape">When arbitrary, the blocks list represents the blocks to be highlighted. When Cube the blocks list should contain 2 positions for start and end</param>
        void HighlightBlocks(IPlayer player, int highlightSlotId, List<BlockPos> blocks, EnumHighlightBlocksMode mode = EnumHighlightBlocksMode.Absolute, EnumHighlightShape shape = EnumHighlightShape.Arbitrary);



        /// <summary>
        /// Retrieve a customized interface to access blocks in the loaded game world.
        /// </summary>
        /// <param name="synchronize">Whether or not a call to Setblock should send the update also to all connected clients</param>
        /// <param name="relight">Whether or not to relight the chunk after a call to SetBlock and the light values changed by that</param>
        /// <param name="strict">Log an error message if GetBlock/SetBlock was called to an unloaded chunk</param>
        /// <param name="debug">If strict, crashes the server if a unloaded chunk was crashed, prints an exception and exports a png image of the current loaded chunks</param>
        /// <returns></returns>
        IBlockAccessor GetBlockAccessor(bool synchronize, bool relight, bool strict, bool debug = false);


        /// <summary>
        /// Retrieve a customized interface to access blocks in the loaded game world. Does not to relight/sync on a SetBlock until Commit() is called. On commit all touched blocks are relit/synced at once. This method should be used when setting many blocks (e.g. tree generation, explosion, etc.).
        /// </summary>
        /// <param name="synchronize">Whether or not a call to Setblock should send the update also to all connected clients</param>
        /// <param name="relight">Whether or not to relight the chunk after the a call to SetBlock and the light values changed by that</param>
        /// <param name="debug"></param>
        /// <returns></returns>
        IBulkBlockAccessor GetBlockAccessorBulkUpdate(bool synchronize, bool relight, bool debug = false);

        /// <summary>
        /// Retrieve a customized interface to access blocks in the loaded game world. Does not relight. On commit all touched blocks are updated at once. This method is currently used for the snow accumulation system
        /// </summary>
        /// <param name="synchronize"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        IBulkBlockAccessor GetBlockAccessorBulkMinimalUpdate(bool synchronize, bool debug = false);

        /// <summary>
        /// Retrieve a special Bulk blockaccessor which can have the chunks it accesses directly provided to it from a loading mapchunk. On commit all touched blocks are updated at once. This method is currently used for the snow accumulation system
        /// </summary>
        IBulkBlockAccessor GetBlockAccessorMapChunkLoading(bool synchronize, bool debug = false);

        /// <summary>
        /// Same as GetBlockAccessorBulkUpdate, additionally, each Commit() stores the previous state and you can perform undo/redo operations on these.
        /// </summary>
        /// <param name="synchronize">Whether or not a call to Setblock should send the update also to all connected clients</param>
        /// <param name="relight">Whether or not to relight the chunk after a call to SetBlock and the light values changed by that</param>
        /// <param name="debug"></param>
        /// <returns></returns>
        IBlockAccessorRevertable GetBlockAccessorRevertable(bool synchronize, bool relight, bool debug = false);

        /// <summary>
        /// Same as GetBlockAccessor but you have to call PrefetchBlocks() before using GetBlock(). It pre-loads all blocks in given area resulting in faster GetBlock() access
        /// </summary>
        /// <param name="synchronize">Whether or not a call to Setblock should send the update also to all connected clients</param>
        /// <param name="relight">Whether or not to relight the chunk after a call to SetBlock and the light values changed by that</param>
        /// <returns></returns>
        IBlockAccessorPrefetch GetBlockAccessorPrefetch(bool synchronize, bool relight);

        /// <summary>
        /// Same as the normal block accessor but remembers the previous chunk that was accessed. This can give you a 10-50% performance boosts when you scan many blocks in tight loops
        /// DONT FORGET: Call .Begin() before getting/setting in a tight loop. Not calling it can cause the game to crash
        /// </summary>
        /// <param name="synchronize"></param>
        /// <param name="relight"></param>
        /// <returns></returns>
        ICachingBlockAccessor GetCachingBlockAccessor(bool synchronize, bool relight);


        /// <summary>
        /// This block accessor is *read only* and does not use lock() or chunk.Unpack() in order to make it very fast. This comes at the cost of sometimes reading invalid data (block id = 0) when the chunk is packed or being packed.
        /// </summary>
        /// <returns></returns>
        IBlockAccessor GetLockFreeBlockAccessor();
    }
}
