using System;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;

namespace Vintagestory.API.Common
{
    public enum EnumHighlightSlot
    {
        Selection = 0,
        Brush = 1,
        Spawner = 2,
        LandClaim = 3
    }



    /// <summary>
    /// Important interface to access the game world.
    /// </summary>
    public interface IWorldAccessor
    {
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

        /// <summary>
        /// The land claiming api interface
        /// </summary>
        ILandClaimAPI Claims { get; }

        /// <summary>
        /// Returns a list all loaded chunk positions in the form of a long index. Code to turn that into x/y/z coords:
        /// Vec3i coords = new Vec3i(
        ///    (int)(chunkIndex3d % ChunkMapSizeX),
        ///    (int)(chunkIndex3d / (ChunkMapSizeX * ChunkMapSizeZ)),
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
        /// Relaxed block access to the worlds block data
        /// </summary>
        IBlockAccessor BlockAccessor { get; }

        /// <summary>
        /// Relaxed bulk block access to the worlds block data. Since this is a single bulk block access instance the cached data is shared for everything accessing this method, hence should only be accessed from the main thread and any changed comitted within the same game tick. You can however use the WorldManager api to get your own instance of a bulk block accessor
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
        /// List of all loaded blocks. Some may be null or placeholders (then block.code is null).
        /// </summary>
        List<Block> Blocks { get; }

        /// <summary>
        /// List of all loaded items. Some may be null or placeholders (then item.code is null)
        /// </summary>
        List<Item> Items { get; }

        /// <summary>
        /// List of all loaded entity types. 
        /// </summary>
        List<EntityProperties> EntityTypes { get; }

        /// <summary>
        /// List of all loaded crafting recipes
        /// </summary>
        List<GridRecipe> GridRecipes { get; }

        /// <summary>
        /// List of all loaded metal alloys
        /// </summary>
        List<AlloyRecipe> Alloys { get; }

        /// <summary>
        /// List of all loaded cooking recipes
        /// </summary>
        List<CookingRecipe> CookingRecipes { get; }

        /// <summary>
        /// List of all loaded smithing recipes
        /// </summary>
        List<SmithingRecipe> SmithingRecipes { get; }

        /// <summary>
        /// List of all loaded knapping recipes
        /// </summary>
        List<KnappingRecipe> KnappingRecipes { get; }

        /// <summary>
        /// List of all loaded clay forming recipes
        /// </summary>
        List<ClayFormingRecipe> ClayFormingRecipes { get; }

        /// <summary>
        /// List of all loaded barrel recipes
        /// </summary>
        List<BarrelRecipe> BarrelRecipes { get; }



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
        /// <param name="codeBeginsWith"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        Block[] SearchBlocks(AssetLocation wildcard);

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
        /// The current weather for each weather pattern. A value of 0 denotes in active, a value of 1 denotes strongly active
        /// </summary>
        //float[] GetWeatherAtPosition(BlockPos pos);

        /// <summary>
        /// Spawns a dropped itemstack at given position. Will immediately disappear if stacksize==0
        /// </summary>
        /// <param name="itemstack"></param>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        void SpawnItemEntity(ItemStack itemstack, Vec3d position, Vec3d velocity = null);

        /// <summary>
        /// Creates a new entity. It's the responsibility of the given Entity to call set it's EntityType.
        /// This should be done inside it's Initialize method before base.Initialize is called.
        /// </summary>
        /// <param name="entity"></param>
        void SpawnEntity(Entity entity);

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
        /// Retrieves the first found entity that intersects any of the supplied collisionboxes offseted by basePos. This is a helper method for you to determine if you can place a block at given position. You can also implement it yourself with intersection testing and GetEntitiesAround()
        /// </summary>
        /// <param name="collisionBoxes"></param>
        /// <param name="basePos"></param>
        /// <param name="matches"></param>
        /// <returns></returns>
        Entity[] GetIntersectingEntities(BlockPos basePos, Cuboidf[] collisionBoxes, ActionConsumable<Entity> matches = null);

        /// <summary>
        /// Find the nearest player to the given position
        /// </summary>
        /// <param name = "x">x coordinate</param>
        /// <param name = "y">y coordinate</param>
        /// <param name = "z">z coordinate</param>
        /// <returns>ID of the nearest player</returns>
        IPlayer NearestPlayer(double x, double y, double z);

        /// <summary>
        /// Gets a list of all online players. 
        /// </summary>
        /// <returns>Array containing the IDs of online players</returns>
        IPlayer[] AllOnlinePlayers { get; }

        /// <summary>
        /// Gets a list of all players that connected to this server at least once. When called client side you will receive the same as AllOnlinePlayers
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
        /// <param name="dualCallByPlayer">If this call is made on client and on server, set this the causing playerUID to prevent double spawning. Essentially dualCall will spawn the particles on the client, and send it to all other players except source client</param>
        void SpawnParticles(float quantity, int color, Vec3d minPos, Vec3d maxPos, Vec3f minVelocity, Vec3f maxVelocity, float lifeLength, float gravityEffect, float scale = 1f, EnumParticleModel model = EnumParticleModel.Quad, IPlayer dualCallByPlayer = null);

        /// <summary>
        /// Spawn a bunch of particles
        /// </summary>
        /// <param name="particlePropertiesProvider"></param>
        /// <param name="dualCallByPlayer"></param>
        void SpawnParticles(IParticlePropertiesProvider particlePropertiesProvider, IPlayer dualCallByPlayer = null);


        /// <summary>
        /// Spawn a bunch of particles colored by the block at given position
        /// </summary>
        /// <param name="blockPos">The position of the block to take the color from</param>
        /// <param name="pos">The position where the particles should spawn</param>
        /// <param name="item"></param>
        /// <param name="radius"></param>
        /// <param name="quantity"></param>
        /// <param name="scale"></param>
        /// <param name="dualCallByPlayer"></param>
        void SpawnCubeParticles(BlockPos blockPos, Vec3d pos, float radius, int quantity, float scale = 1f, IPlayer dualCallByPlayer = null);


        /// <summary>
        /// Spawn a bunch of particles colored by given itemstack
        /// </summary>
        /// <param name="pos">The position where the particles should spawn</param>
        /// <param name="item"></param>
        /// <param name="radius"></param>
        /// <param name="quantity"></param>
        /// <param name="scale"></param>
        /// <param name="dualCallByPlayer"></param>
        void SpawnCubeParticles(Vec3d pos, ItemStack item, float radius, int quantity, float scale = 1f, IPlayer dualCallByPlayer = null);




        /// <summary>
        /// Shoots out a virtual ray at between given positions and stops when the ray hits a block or entity selection box. The block/entity it struck first is then returned by reference.
        /// </summary>
        /// <param name="fromPos"></param>
        /// <param name="toPos"></param>
        /// <param name="blockSelection"></param>
        /// <param name="entitySelection"></param>
        /// <param name="filter">Can be used to ignore certain blocks</param>
        void RayTraceForSelection(Vec3d fromPos, Vec3d toPos, ref BlockSelection blockSelection, ref EntitySelection entitySelection, BlockFilter filter = null);
        

        /// <summary>
        /// Shoots out a virtual ray at between given positions and stops when the ray hits a block or entity intersection box supplied by given supplier. The block/entity it struck first is then returned by reference.
        /// </summary>
        /// <param name="supplier"></param>
        /// <param name="fromPos"></param>
        /// <param name="toPos"></param>
        /// <param name="blockSelection"></param>
        /// <param name="entitySelection"></param>
        /// <param name="filter">Can be used to ignore certain blocks</param>
        void RayTraceForSelection(IWorldIntersectionSupplier supplier, Vec3d fromPos, Vec3d toPos, ref BlockSelection blockSelection, ref EntitySelection entitySelection, BlockFilter filter = null);


        /// <summary>
        /// Shoots out a virtual ray at given position and angle and stops when the ray hits a block or entity selection box. The block/entity it struck first is then returned by reference.
        /// </summary>
        /// <param name="fromPos"></param>
        /// <param name="pitch"></param>
        /// <param name="yaw"></param>
        /// <param name="range"></param>
        /// <param name="blockSelection"></param>
        /// <param name="entitySelection"></param>
        /// <param name="filter">Can be used to ignore certain blocks. Return false to ignore</param>
        void RayTraceForSelection(Vec3d fromPos, float pitch, float yaw, float range, ref BlockSelection blockSelection, ref EntitySelection entitySelection, BlockFilter filter = null);

        /// <summary>
        /// Shoots out a given ray and stops when the ray hits a block or entity selection box. The block/entity it struck first is then returned by reference.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="blockSelection"></param>
        /// <param name="entitySelection"></param>
        /// <param name="filter">Can be used to ignore certain blocks. Return false to ignore</param>
        void RayTraceForSelection(Ray ray, ref BlockSelection blockSelection, ref EntitySelection entitySelection, BlockFilter filter = null);



        /// <summary>
        /// Calls given method after every given interval until unregistered. The engine may call your method slightly later since these event are handled only during fixed interval game ticks.
        /// </summary>
        /// <param name="OnGameTick"></param>
        /// <param name="millisecondInterval"></param>
        /// <returns>listenerId</returns>
        long RegisterGameTickListener(Action<float> OnGameTick, int millisecondInterval);

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
        /// <param name="mode"></param>
        /// <param name="shape">When arbitrary, the blocks list represents the blocks to be highlighted. When Cube the blocks list should contain 2 positions for start and end</param>
        void HighlightBlocks(IPlayer player, int highlightSlotId, List<BlockPos> blocks, List<int> colors, EnumHighlightBlocksMode mode = EnumHighlightBlocksMode.Absolute, EnumHighlightShape shape = EnumHighlightShape.Arbitrary);

        /// <summary>
        /// Sends given player a list of block positions that should be highlighted (using a default color)
        /// </summary>
        /// <param name="player"></param>
        /// <param name="highlightSlotId">for multiple highlights use a different number</param>
        /// <param name="blocks"></param>
        /// <param name="mode"></param>
        /// <param name="shape">When arbitrary, the blocks list represents the blocks to be highlighted. When Cube the blocks list should contain 2 positions for start and end</param>
        void HighlightBlocks(IPlayer player, int highlightSlotId, List<BlockPos> blocks, EnumHighlightBlocksMode mode = EnumHighlightBlocksMode.Absolute, EnumHighlightShape shape = EnumHighlightShape.Arbitrary);

    }
}