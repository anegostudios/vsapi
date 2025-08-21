using System;
using System.Collections.Generic;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{
    public class BlockUpdate
    {
        public bool ExchangeOnly;
        public BlockPos Pos;
        /// <summary>
        /// Contains either liquid layer of solid layer block
        /// </summary>
        public int OldBlockId;
        /// <summary>
        /// Contains liquid layer of block
        /// </summary>
        public int OldFluidBlockId;
        /// <summary>
        /// If this value is negative, it indicates no change to the block (neither air block nor anything else) because only the fluid is being updated
        /// </summary>
        public int NewSolidBlockId = -1;
        /// <summary>
        /// If this value is negative, it indicates no change to the fluids layer block (neither air block nor anything else) because only the solid block is being updated
        /// </summary>
        public int NewFluidBlockId = -1;
        public ItemStack ByStack;

        public byte[] OldBlockEntityData;
        public byte[] NewBlockEntityData;
        public List<DecorUpdate> Decors;
        public List<DecorUpdate> OldDecors;
    }

    public struct DecorUpdate
    {
        public int faceAndSubposition;
        public int decorId;
    }

    public class EntityUpdate
    {
        /// <summary>
        /// If set this entity was spawned or Moved (position needs to be set too)
        /// </summary>
        public long EntityId = -1;

        /// <summary>
        /// If set this entity needs to be spawned
        /// </summary>
        public EntityProperties EntityProperties;

        /// <summary>
        /// If set the entity was moved
        /// </summary>
        public EntityPos OldPosition;
        public EntityPos NewPosition;
    }

    /// A climate condition at a given position
    public class ClimateCondition
    {
        /// <summary>
        /// Between -20 and +40 degrees
        /// </summary>
        public float Temperature;

        /// <summary>
        /// If you read the now values, you can still get the world gen rain fall from this value. Between 0..1
        /// </summary>
        public float WorldgenRainfall;

        /// <summary>
        /// If you read the now values, you can still get the world gen temp from this value
        /// </summary>
        public float WorldGenTemperature;

        /// <summary>
        /// Nomalized value between 0..1. Static value determined on world generation
        /// </summary>
        public float GeologicActivity;

        /// <summary>
        /// Nomalized value between 0..1. When loading the now values, this is set to the current precipitation value, otherwise to "yearly averages" or the values generated during worldgen
        /// </summary>
        public float Rainfall;

        public float RainCloudOverlay;

        /// <summary>
        /// Nomalized value between 0..1
        /// </summary>
        public float Fertility;

        /// <summary>
        /// Nomalized value between 0..1
        /// </summary>
        public float ForestDensity;
        /// <summary>
        /// Nomalized value between 0..1
        /// </summary>
        public float ShrubDensity;

        public void SetLerped(ClimateCondition left, ClimateCondition right, float w)
        {
            Temperature = left.Temperature * (1 - w) + right.Temperature * w;
            Rainfall = left.Rainfall * (1 - w) + right.Rainfall * w;
            Fertility = left.Fertility * (1 - w) + right.Fertility * w;
            ForestDensity = left.ForestDensity * (1 - w) + right.ForestDensity * w;
            ShrubDensity = left.ShrubDensity * (1 - w) + right.ShrubDensity * w;
        }


    }

    /// <summary>
    /// Used in blockAccessor.GetLightLevel() to determine what kind of light level you want
    /// </summary>
    [DocumentAsJson]
    public enum EnumLightLevelType
    {
        /// <summary>
        /// Will get you just the block light
        /// </summary>
        OnlyBlockLight,
        /// <summary>
        /// Will get you just the sun light unaffected by the day/night cycle
        /// </summary>
        OnlySunLight,
        /// <summary>
        /// Will get you max(sunlight, onlyblocklight)
        /// </summary>
        MaxLight,
        /// <summary>
        /// Will get you max(sunlight * sunbrightness, blocklight)
        /// </summary>
        MaxTimeOfDayLight,
        /// <summary>
        /// Will get you sunlight * sunbrightness
        /// </summary>
        TimeOfDaySunLight,
        /// <summary>
        /// Will get you sunbrightness
        /// </summary>
        Sunbrightness
    }


    public interface ICachingBlockAccessor : IBlockAccessor
    {
        /// <summary>
        /// True if the most recent GetBlock or SetBlock had a laoded chunk
        /// </summary>
        bool LastChunkLoaded { get; }
        void Begin();
        void Dispose();
    }

    /// <summary>
    /// Provides read/write access to the blocks of a world
    /// </summary>
    public interface IBlockAccessor
    {
        /// <summary>
        /// Width, Length and Height of a chunk
        /// </summary>
        [Obsolete("Use GlobalConstants.ChunkSize instead.  Fetching a property in inner-loop code is needlessly inefficient!")]
        int ChunkSize { get; }

        /// <summary>
        /// Width and Length of a region in blocks
        /// </summary>
        int RegionSize { get; }

        /// <summary>
        /// X Size of the world in blocks
        /// </summary>
        int MapSizeX { get; }

        /// <summary>
        /// Y Size of the world in blocks
        /// </summary>
        int MapSizeY { get; }

        /// <summary>
        /// Z Size of the world in blocks
        /// </summary>
        int MapSizeZ { get; }


        int RegionMapSizeX { get; }
        int RegionMapSizeY { get; }
        int RegionMapSizeZ { get; }

        /// <summary>
        /// Whether to update the snow accum map on a SetBlock()
        /// </summary>
        bool UpdateSnowAccumMap { get; set; }

        /// <summary>
        /// Size of the world in blocks
        /// </summary>
        Vec3i MapSize { get; }


        /// <summary>
        /// Retrieve chunk at given chunk position (= divide block position by chunk size)
        /// <br/>For dimension awareness, chunkY would need to be based on BlockPos.InternalY / chunksize or else explicitly include the dimensionId multiplied by GlobalConstants.DimensionSizeInChunks
        /// </summary>
        /// <param name="chunkX"></param>
        /// <param name="chunkY"></param>
        /// <param name="chunkZ"></param>
        /// <returns></returns>
        IWorldChunk GetChunk(int chunkX, int chunkY, int chunkZ);

        /// <summary>
        /// Retrieve chunk at given chunk position, returns null if chunk is not loaded
        /// </summary>
        /// <param name="chunkIndex3D"></param>
        /// <returns></returns>
        IWorldChunk GetChunk(long chunkIndex3D);

        /// <summary>
        /// Retrieves a map region at given region position, returns null if region is not loaded
        /// </summary>
        /// <param name="regionX"></param>
        /// <param name="regionZ"></param>
        /// <returns></returns>
        IMapRegion GetMapRegion(int regionX, int regionZ);



        /// <summary>
        /// Retrieve chunk at given block position, returns null if chunk is not loaded
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        IWorldChunk GetChunkAtBlockPos(BlockPos pos);


        /// <summary>
        /// Get the block id of the block at the given world coordinate
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        int GetBlockId(BlockPos pos);

        /// <summary>
        /// Get the block type of the block at the given world coordinate, dimension aware. Will never return null. For air blocks or invalid coordinates you'll get a block instance with block code "air" and id 0
        /// Same as <see cref="GetBlock(BlockPos, int)"/> with BlockLayersAccess.Default as layer
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        Block GetBlock(BlockPos pos);

        /// <summary>
        /// Direct raw coordinate access to blocks. PLEASE NOTE: The caller has to ensure dimension awareness (i.e. pos.InternalY when using BlockPos)<br/>
        /// Gets the block type of the block at the given world coordinate.
        /// </summary>
        /// <param name="x">x coordinate</param>
        /// <param name="y">y coordinate</param>
        /// <param name="z">z coordinate</param>
        /// <param name="layer">The block layer to retrieve from. See also <see cref="BlockLayersAccess"/></param>
        /// <returns>Never null. For unpopulated locations or invalid coordinates you'll get a block instance with block code "air" and id 0</returns>
        Block GetBlockRaw(int x, int y, int z, int layer = BlockLayersAccess.Default);

        /// <summary>
        /// Get block type at given world coordinate, dimension aware. Will never return null. For airblocks or invalid coordinates you'll get a block instance with block code "air" and id 0
        /// <br/>Reads the block from the specified layer(s), see BlockLayersAccess documentation for details.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name = "layer">blocks layer e.g. solid, fluid etc.</param>
        /// <returns></returns>
        Block GetBlock(BlockPos pos, int layer);


        /// <summary>
        /// Same as <see cref="GetBlock(BlockPos, int)"/> with BlockLayersAccess.MostSolid as layer
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        Block GetMostSolidBlock(BlockPos pos);



        #region Obsolete GetBlock methods


        /// <summary>
        /// Retrieve chunk at given block position, returns null if chunk is not loaded
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <returns></returns>
        [Obsolete("Please use BlockPos version instead, for dimension awareness")]
        IWorldChunk GetChunkAtBlockPos(int posX, int posY, int posZ);

        /// <summary>
        /// Get the block id of the block at the given world coordinate
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <returns></returns>
        [Obsolete("Please use BlockPos version instead, for dimension awareness")]
        int GetBlockId(int posX, int posY, int posZ);

        /// <summary>
        /// Get the block type of the block at the given world coordinate. For invalid or unloaded coordinates this method returns null.
        /// </summary>
        /// <param name = "x">x coordinate</param>
        /// <param name = "y">y coordinate</param>
        /// <param name = "z">z coordinate</param>
        /// <param name="layer">Block layer</param>
        /// <returns>ID of the block at the given position</returns>
        [Obsolete("Please use BlockPos version instead, for dimension awareness")]
        Block GetBlockOrNull(int x, int y, int z, int layer = BlockLayersAccess.MostSolid);


        /// <summary>
        /// Get the block type of the block at the given world coordinate. Will never return null. For airblocks or invalid coordinates you'll get a block instance with block code "air" and id 0
        /// <br/>Reads the block from the specified layer(s), see <see cref="BlockLayersAccess"/> documentation for details.
        /// <br/>If this must be used even though it's deprecated, please consider using .GetBlockRaw() instead where calling code is explicitly dimension-aware
        /// </summary>
        /// <param name = "x">x coordinate</param>
        /// <param name = "y">y coordinate</param>
        /// <param name = "z">z coordinate</param>
        /// <param name = "layer">blocks layer e.g. solid, fluid etc.</param>
        /// <returns>ID of the block at the given position</returns>
        [Obsolete("Please use BlockPos version instead, for dimension awareness")]
        Block GetBlock(int x, int y, int z, int layer);

        /// <summary>
        /// Get the block type of the block at the given world coordinate. Will never return null. For air blocks or invalid coordinates you'll get a block instance with block code "air" and id 0
        /// Same as <see cref="GetBlock(BlockPos, int)"/> with BlockLayersAccess.Default as layer
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <returns></returns>
        [Obsolete("Please use BlockPos version instead, for dimension awareness")]
        Block GetBlock(int posX, int posY, int posZ);


        /// <summary>
        /// Same as <see cref="GetBlock(int, int, int, int)"/> with BlockLayersAccess.MostSolid as layer
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        [Obsolete("Please use BlockPos version instead, for dimension awareness")]
        Block GetMostSolidBlock(int x, int y, int z);
        #endregion

        /// <summary>
        /// A method to iterate over blocks in an area. Less overhead than when calling GetBlock(pos) many times. If there is liquids in the liquid layer, the onBlock method will be called twice. Currently used for more efficient collision testing.
        /// <br/>Currently NOT dimensionally aware
        /// </summary>
        /// <param name="minPos"></param>
        /// <param name="maxPos"></param>
        /// <param name="onBlock">The method in which you want to check for the block, whatever it may be.</param>
        /// <param name="centerOrder">If true, the blocks will be ordered by the distance to the center position</param>
        /// <returns></returns>
        void WalkBlocks(BlockPos minPos, BlockPos maxPos, Action<Block, int, int, int> onBlock, bool centerOrder = false);

        /// <summary>
        /// A method to search for a given block in an area
        /// <br/>Currently NOT dimensionally aware
        /// </summary>
        /// <param name="minPos"></param>
        /// <param name="maxPos"></param>
        /// <param name="onBlock">Return false to stop the search</param>
        /// <param name="onChunkMissing">Called when a missing/unloaded chunk was encountered</param>
        void SearchBlocks(BlockPos minPos, BlockPos maxPos, ActionConsumable<Block, BlockPos> onBlock, Action<int, int, int> onChunkMissing = null);

        /// <summary>
        /// A method to search for a given fluid block in an area
        /// <br/>Currently NOT dimensionally aware
        /// </summary>
        /// <param name="minPos"></param>
        /// <param name="maxPos"></param>
        /// <param name="onBlock">Return false to stop the search</param>
        /// <param name="onChunkMissing">Called when a missing/unloaded chunk was encountered</param>
        void SearchFluidBlocks(BlockPos minPos, BlockPos maxPos, ActionConsumable<Block, BlockPos> onBlock, Action<int, int, int> onChunkMissing = null);

        /// <summary>
        /// Calls given handler if it encounters one or more generated structure at given position (read from mapregions, assuming a max structure size of 256x256x256)
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="onStructure"></param>
        void WalkStructures(BlockPos pos, Action<GeneratedStructure> onStructure);

        /// <summary>
        /// Calls given handler if it encounters one or more generated structure that intersect any position inside minpos->maxpos (read from mapregions, assuming a max structure size of 256x256x256)
        /// </summary>
        /// <param name="minpos"></param>
        /// <param name="maxpos"></param>
        /// <param name="onStructure"></param>
        void WalkStructures(BlockPos minpos, BlockPos maxpos, Action<GeneratedStructure> onStructure);

        /// <summary>
        /// Set a block at the given position. Use blockid 0 to clear that position from any blocks. Marks the chunk dirty so that it gets saved to disk during shutdown or next autosave.
        /// If called with a fluid block, the fluid will automatically get set in the fluid layer, and the solid layer will be emptied.
        /// </summary>
        /// <param name="blockId"></param>
        /// <param name="pos"></param>
        void SetBlock(int blockId, BlockPos pos);

        /// <summary>
        /// Sets a block to given layer. Can only use "BlockLayersAccess.Solid" or "BlockLayersAccess.Liquid". Use id 0 to clear a block from given position. Marks the chunk dirty so that it gets saved to disk during shutdown or next autosave.
        /// </summary>
        /// <param name="blockId"></param>
        /// <param name="pos"></param>
        /// <param name="layer"></param>
        void SetBlock(int blockId, BlockPos pos, int layer);

        /// <summary>
        /// Set a block at the given position. Use blockid 0 to clear that position from any blocks. Marks the chunk dirty so that it gets saved to disk during shutdown or next autosave.
        /// If called with a fluid block, the fluid will automatically get set in the fluid layer, and the solid layer will be emptied.
        /// </summary>
        /// <param name="blockId"></param>
        /// <param name="pos"></param>
        /// <param name="byItemstack">If set then it will be passed onto the block.OnBlockPlaced method</param>
        void SetBlock(int blockId, BlockPos pos, ItemStack byItemstack);

        /// <summary>
        /// Set a block at the given position without calling OnBlockRemoved or OnBlockPlaced, which prevents any block entity from being removed or placed. Marks the chunk dirty so that it gets saved to disk during shutdown or next autosave. Should only be used if you want to prevent any block entity deletion at this position.
        /// <br/>This also, for example, does not change a block's reinforcement level, useful for openable blocks such as doors, gates etc
        /// </summary>
        void ExchangeBlock(int blockId, BlockPos pos);


        /// <summary>
        /// Removes the block at given position and calls Block.GetDrops(), Block.OnBreakBlock() and Block.OnNeighbourBlockChange() for all neighbours. Drops the items that are return from Block.GetDrops()
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="byPlayer"></param>
        /// <param name="dropQuantityMultiplier"></param>
        void BreakBlock(BlockPos pos, IPlayer byPlayer, float dropQuantityMultiplier = 1f);


        /// <summary>
        /// Client Side: Will render the block breaking decal on that block. If the remaining block resistance reaches 0, will call break block
        /// Server Side: Broadcasts a package to all nearby clients to update the block damage of this block for rendering the decal (note: there is currently no server side list of current block damages, these are client side only at the moemnt)
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="facing"></param>
        /// <param name="damage"></param>
        void DamageBlock(BlockPos pos, BlockFacing facing, float damage);

        /// <summary>
        /// Get the Block object of a certain block ID. Returns null when not found.
        /// </summary>
        /// <param name = "blockId">The block ID to search for</param>
        /// <returns>BlockType object</returns>
        Block GetBlock(int blockId);


        /// <summary>
        /// Get the Block object of for given block code. Returns null when not found.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Block GetBlock(AssetLocation code);



        /// <summary>
        /// Spawn block entity at this position. Does not place its corresponding block, you have to do this yourself.
        /// </summary>
        /// <param name="classname"></param>
        /// <param name="position"></param>
        /// <param name="byItemStack"></param>
        void SpawnBlockEntity(string classname, BlockPos position, ItemStack byItemStack = null);

        /// <summary>
        /// Adds pre-initialized block entity to the world. Does not call CreateBehaviors/Initialize/OnBlockPlaced on the block entity. This is a very low level method for block entity spawning, normally you'd want to use <see cref="SpawnBlockEntity(string, BlockPos, ItemStack)"/>
        /// </summary>
        /// <param name="be"></param>
        void SpawnBlockEntity(BlockEntity be);


        /// <summary>
        /// Permanently removes any block entity at this postion. Does not remove its corresponding block, you have to do this yourself. Marks the chunk dirty so that it gets saved to disk during shutdown or next autosave.
        /// </summary>
        /// <param name="position"></param>
        void RemoveBlockEntity(BlockPos position);

        /// <summary>
        /// Retrieve the block entity at given position. Returns null if there is no block entity at this position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        BlockEntity GetBlockEntity(BlockPos position);

        /// <summary>
        /// Retrieve the block entity at given position. Returns null if there is no block entity at this position
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="position"></param>
        /// <returns></returns>
        T GetBlockEntity<T>(BlockPos position) where T : BlockEntity;



        /// <summary>
        /// Checks if the position is inside the maps boundaries
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <returns></returns>
        [Obsolete("Please use BlockPos version instead, for dimension awareness")]
        bool IsValidPos(int posX, int posY, int posZ);

        /// <summary>
        /// Checks if the position is inside the maps boundaries
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        bool IsValidPos(BlockPos pos);

        /// <summary>
        /// Checks if this position can be traversed by a normal player (returns false for outside map or not yet loaded chunks)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        [Obsolete("Better to use dimension-aware version")]
        bool IsNotTraversable(double x, double y, double z);

        /// <summary>
        /// Checks if this position can be traversed by a normal player (returns false for outside map or not yet loaded chunks)
        /// Dimension-aware version
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="dim"></param>
        /// <returns></returns>
        bool IsNotTraversable(double x, double y, double z, int dim);

        /// <summary>
        /// Checks if this position can be traversed by a normal player (returns false for outside map or not yet loaded chunks)
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        bool IsNotTraversable(BlockPos pos);


        /// <summary>
        /// Calling this method has no effect in normal block acessors except for:
        /// - Bulk update block accessor: Sets all blocks, relight all affected one chunks in one go and send blockupdates to clients in a packed format.
        /// - World gen block accessor: To Recalculate the heightmap in of all updated blocks in one go
        /// - Revertable block accessor: Same as bulk update block accessor plus stores a new history state.
        /// </summary>
        /// <returns>List of changed blocks</returns>
        List<BlockUpdate> Commit();

        /// <summary>
        /// For the bulk update block accessor reverts all the SetBlocks currently called since the last Commit()
        /// </summary>
        void Rollback();


        /// <summary>
        /// Server side call: Resends the block entity data (if present) to all clients. Triggers a block changed event on the client once received , but will not redraw the chunk. Marks also the chunk dirty so that it gets saved to disk during shutdown or next autosave.<br/>
        /// Client side call: No effect
        /// </summary>
        /// <param name="pos"></param>
        void MarkBlockEntityDirty(BlockPos pos);

        /// <summary>
        /// Triggers the method OnNeighbourBlockChange() to all neighbour blocks at given position
        /// </summary>
        /// <param name="pos"></param>
        void TriggerNeighbourBlockUpdate(BlockPos pos);

        /// <summary>
        /// Server side: Sends that block to the client (via bulk packet).  Through that packet the client will do a SetBlock on that position (which triggers a redraw if oldblockid != newblockid).<br/>
        /// Client side: Triggers a block changed event and redraws the chunk
        /// </summary>
        /// <param name="skipPlayer">Server side: Does not send the update to this player, Client Side: No effect</param>
        /// <param name="pos"></param>
        void MarkBlockDirty(BlockPos pos, IPlayer skipPlayer = null);

        /// <summary>
        /// Server side: Triggers a OnNeighbourBlockChange on that position and sends that block to the client (via bulk packet), through that packet the client will do a SetBlock on that position (which triggers a redraw if oldblockid != newblockid).<br/>
        /// Client side: Triggers a block changed event and redraws the chunk. Deletes and re-create block entities
        /// </summary>
        /// <param name="pos"></param>
        void MarkBlockModified(BlockPos pos);

        /// <summary>
        /// Server Side: Same as MarkBlockDirty()<br/>
        /// Client Side: Same as MarkBlockDirty(), but also calls supplied delegate after the chunk has been re-retesselated. This can be used i.e. for block entities to dynamically switch between static models and dynamic models at exactly the right timing
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="OnRetesselated"></param>
        void MarkBlockDirty(BlockPos pos, Action OnRetesselated);

        /// <summary>
        /// Returns the light level (0..32) at given position. If the chunk at that position is not loaded this method will return the default sunlight value
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        int GetLightLevel(BlockPos pos, EnumLightLevelType type);

        /// <summary>
        /// Returns the light level (0..32) at given position. If the chunk at that position is not loaded this method will return the default sunlight value
        /// <br/>Note this is not currently dimensionally aware
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        int GetLightLevel(int x, int y, int z, EnumLightLevelType type);

        /// <summary>
        /// Returns the light values at given position. XYZ component = block light rgb, W component = sun light brightness
        /// <br/>Note this is not currently dimensionally aware
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <returns></returns>
        Vec4f GetLightRGBs(int posX, int posY, int posZ);

        /// <summary>
        /// Returns the light values at given position. XYZ component = block light rgb, W component = sun light brightness
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        Vec4f GetLightRGBs(BlockPos pos);

        /// <summary>
        /// Returns the light values at given position. bit 0-23: block rgb light, bit 24-31: sun light brightness
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <returns></returns>
        int GetLightRGBsAsInt(int posX, int posY, int posZ);

        /// <summary>
        /// Returns the topmost solid surface position at given x/z coordinate as it was during world generation. This map is not updated after placing/removing blocks
        /// <br/>Note this is meaningless in dimensions other than the normal world
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        int GetTerrainMapheightAt(BlockPos pos);

        /// <summary>
        /// Returns the topmost non-rain-permeable position at given x/z coordinate. This map is always updated after placing/removing blocks
        /// <br/>Note this is meaningless in dimensions other than the normal world
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        int GetRainMapHeightAt(BlockPos pos);

        /// <summary>
        /// Returns a number of how many blocks away there is rain fall. Does a cheap 2D bfs up to x blocks away. Returns 99 if none was found within given blocks
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="horziontalSearchWidth">Horizontal search distance, 4 default</param>
        /// <param name="verticalSearchWidth">Vertical search distance, 1 default</param>
        /// <returns></returns>
        int GetDistanceToRainFall(BlockPos pos, int horziontalSearchWidth = 4, int verticalSearchWidth = 1);

        /// <summary>
        /// Returns the topmost non-rain-permeable position at given x/z coordinate. This map is always updated after placing/removing blocks
        /// <br/>Note this is meaningless in dimensions other than the normal world
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posZ"></param>
        /// <returns></returns>
        int GetRainMapHeightAt(int posX, int posZ);


        /// <summary>
        /// Returns the map chunk at given chunk position
        /// </summary>
        /// <param name="chunkPos"></param>
        /// <returns></returns>
        IMapChunk GetMapChunk(Vec2i chunkPos);

        /// <summary>
        /// Returns the map chunk at given chunk position
        /// </summary>
        /// <param name="chunkX"></param>
        /// <param name="chunkZ"></param>
        /// <returns></returns>
        IMapChunk GetMapChunk(int chunkX, int chunkZ);

        /// <summary>
        /// Returns the map chunk at given block position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        IMapChunk GetMapChunkAtBlockPos(BlockPos pos);

        /// <summary>
        /// Returns the position's current climate conditions
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="mode">WorldGenValues = values as determined by the worldgenerator, NowValues = additionally modified to take season, day/night and hemisphere into account</param>
        /// <param name="totalDays">When mode == ForSuppliedDateValues then supply here the date. Not used param otherwise</param>
        /// <returns></returns>
        ClimateCondition GetClimateAt(BlockPos pos, EnumGetClimateMode mode = EnumGetClimateMode.NowValues, double totalDays = 0);

        /// <summary>
        /// Returns the position's climate conditions at specified date, making use of previously obtained worldgen climate conditions
        /// </summary>
        ClimateCondition GetClimateAt(BlockPos pos, ClimateCondition baseClimate, EnumGetClimateMode mode, double totalDays);

        /// <summary>
        /// Fast shortcut method for the clound renderer
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="climate"></param>
        /// <returns></returns>
        ClimateCondition GetClimateAt(BlockPos pos, int climate);

        /// <summary>
        /// Retrieves the wind speed for given position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        Vec3d GetWindSpeedAt(Vec3d pos);

        /// <summary>
        /// Retrieves the wind speed for given position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        Vec3d GetWindSpeedAt(BlockPos pos);

        /// <summary>
        /// Used by the chisel block when enough chiseled have been removed and the blocks light absorption changes as a result of that
        /// </summary>
        /// <param name="oldAbsorption"></param>
        /// <param name="newAbsorption"></param>
        /// <param name="pos"></param>
        void MarkAbsorptionChanged(int oldAbsorption, int newAbsorption, BlockPos pos);

        /// <summary>
        /// Call this on OnBlockBroken() when your block entity modifies the blocks light range. That way the lighting task can still retrieve the block entity before its gone.
        /// </summary>
        /// <param name="oldLightHsV"></param>
        /// <param name="pos"></param>
        void RemoveBlockLight(byte[] oldLightHsV, BlockPos pos);


        #region Decor system

        /// <summary>
        /// Add a decor block to the side of an existing block. Use air block (id 0) to remove a decor.<br/>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="onFace"></param>
        /// <param name="block"></param>
        /// <returns>True if the decor was sucessfully set</returns>
        bool SetDecor(Block block, BlockPos position, BlockFacing onFace);

        /// <summary>
        /// Add a decor block to a specific sub-position on the side of an existing block. Use air block (id 0) to remove a decor.<br/>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="block"></param>
        /// <param name="decorIndex">You can get this value via <see cref="BlockSelection.ToDecorIndex()"/> or via constructor <see cref="DecorBits(BlockFacing)"/> or <see cref="DecorBits(BlockFacing, int, int, int)"/>. It can include a subPosition for cave art etc.</param>
        /// <returns>True if the decor was sucessfully set</returns>
        bool SetDecor(Block block, BlockPos position, int decorIndex);

        /// <summary>
        /// Get a list of all decors at this position
        /// </summary>
        /// <param name="position"></param>
        /// <returns>null if this block has no decors. Otherwise a 6 element long list of decor blocks, any of which may be null if not set</returns>
        [Obsolete("Use Dictionary<int, Block> GetSubDecors(BlockPos position)")]
        Block[] GetDecors(BlockPos position);

        /// <summary>
        /// Get a list of all decors at this position
        /// </summary>
        /// <param name="position"></param>
        /// <returns>null if this block position has no decors. Otherwise, a Dictionary with the index being the faceAndSubposition (subposition used for cave art etc.), see <see cref="DecorBits"/></returns>
        Dictionary<int, Block> GetSubDecors(BlockPos position);

        /// <summary>
        /// Retrieves a single decor at given position
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="decorIndex">You can get this value via <see cref="BlockSelection.ToDecorIndex()"/> or via constructor <see cref="DecorBits(BlockFacing)"/> or <see cref="DecorBits(BlockFacing, int, int, int)"/></param>
        /// <returns></returns>
        Block GetDecor(BlockPos pos, int decorIndex);

        /// <summary>
        /// Removes all decors at given position, drops items if set
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="side">If not null, breaks all the decor on given block face, otherwise the decor blocks on all sides are removed</param>
        /// <param name="decorIndex">If not null breaks only this part of the decor for give face. You can get this value via <see cref="BlockSelection.ToDecorIndex()"/> or via constructor <see cref="DecorBits(BlockFacing)"/> or <see cref="DecorBits(BlockFacing, int, int, int)"/></param>
        /// <returns>True if a decor was removed</returns>
        bool BreakDecor(BlockPos pos, BlockFacing side = null, int? decorIndex = null);



        /// <summary>
        /// Server: Marks this position as required for resending to the client
        /// Client: No effect
        /// </summary>
        /// <param name="pos"></param>
        void MarkChunkDecorsModified(BlockPos pos);

        #endregion

        /// <summary>
        /// Tests whether a side at the specified position is solid - testing both fluids layer (which could be ice) and solid blocks layer
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="facing"></param>
        /// <returns></returns>
        bool IsSideSolid(int x, int y, int z, BlockFacing facing);

        /// <summary>
        /// Used by World Edit to create previews, ships etc.
        /// </summary>
        IMiniDimension CreateMiniDimension(Vec3d position);
    }

    /// <summary>
    /// The type of climate values you wish to receive
    /// </summary>
    [DocumentAsJson]
    public enum EnumGetClimateMode
    {
        /// <summary>
        /// The values generate during world generation, these are loosely considered as yearly averages
        /// </summary>
        WorldGenValues,
        /// <summary>
        /// The values at the current calendar time
        /// </summary>
        NowValues,
        /// <summary>
        /// The values at the supplied calendar time, supplied as additional arg
        /// </summary>
        ForSuppliedDateValues,
        /// <summary>
        /// The values at the supplied calendar time, ignoring rainfall etc.  Calling IBlockAccessor.GetClimateAt with this mode will never return a null ClimateCondition value, if it would be null it returns a ClimateCondition with a default 4 degrees temperature value
        /// </summary>
        ForSuppliedDate_TemperatureOnly,
        /// <summary>
        /// The values at the supplied calendar time, ignoring forest cover etc.  Calling IBlockAccessor.GetClimateAt with this mode will never return a null ClimateCondition value, if it would be null it returns a ClimateCondition with a default 4 degrees temperature value and no rain
        /// </summary>
        ForSuppliedDate_TemperatureRainfallOnly
    }

}
