﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public class BlockUpdate
    {
        public BlockPos Pos;
        public ushort OldBlockId;
        public ushort NewBlockId;
        public ItemStack ByStack;
    }

    /// A climate condition at a given position
    public class ClimateCondition
    {
        /// <summary>
        /// Between -20 and +40 degrees
        /// </summary>
        public float Temperature;
        /// <summary>
        /// Nomalized value between 0..1
        /// </summary>
        public float Rainfall;
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
    }

    /// <summary>
    /// Used in blockAccessor.GetLightLevel() to determine what kind of light level you want
    /// </summary>
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
        /// Will get you max(sunlight, blocklight)
        /// </summary>
        MaxLight,
        /// <summary>
        /// Will get you max(sunlight * sunbrightness, blocklight)
        /// </summary>
        MaxTimeOfDayLight
    }

    /// <summary>
    /// Provides read/write access to the blocks of a world
    /// </summary>
    public interface IBlockAccessor
    {
        /// <summary>
        /// Width, Length and Height of a chunk
        /// </summary>
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

        /// <summary>
        /// Size of the world in blocks
        /// </summary>
        Vec3i MapSize { get; }


        /// <summary>
        /// Retrieve chunk at given chunk position (= divide block position by chunk size)
        /// </summary>
        /// <param name="chunkX"></param>
        /// <param name="chunkY"></param>
        /// <param name="chunkZ"></param>
        /// <returns></returns>
        IWorldChunk GetChunk(int chunkX, int chunkY, int chunkZ);



        /// <summary>
        /// Retrieves a map region at given region position
        /// </summary>
        /// <param name="regionX"></param>
        /// <param name="regionZ"></param>
        /// <returns></returns>
        IMapRegion GetMapRegion(int regionX, int regionZ);



        /// <summary>
        /// Retrieve chunk at given block position
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <returns></returns>
        IWorldChunk GetChunkAtBlockPos(int posX, int posY, int posZ);


        /// <summary>
        /// Get the block id of the block at the given world coordinate
        /// </summary>
        /// <param name = "x">x coordinate</param>
        /// <param name = "y">y coordinate</param>
        /// <param name = "z">z coordinate</param>
        /// <returns>ID of the block at the given position. Returns 0 for Airblocks or invalid/unloaded coordinates</returns>
        ushort GetBlockId(int x, int y, int z);

        /// <summary>
        /// Get the block id of the block at the given world coordinate
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>ID of the block at the given position. Returns 0 for Airblocks or invalid/unloaded coordinates</returns>
        ushort GetBlockId(BlockPos pos);


        /// <summary>
        /// Get the block type of the block at the given world coordinate. Will never return null. For airblocks or invalid coordinates you'll get a block instance with block code "air" and id 0
        /// </summary>
        /// <param name = "x">x coordinate</param>
        /// <param name = "y">y coordinate</param>
        /// <param name = "z">z coordinate</param>
        /// <returns>ID of the block at the given position</returns>
        Block GetBlock(int x, int y, int z);


        /// <summary>
        /// Get block type at given world coordinate. Will never return null. For airblocks or invalid coordinates you'll get a block instance with block code "air" and id 0
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        Block GetBlock(BlockPos pos);


        /// <summary>
        /// A bulk block checking method. Less overhead than when calling GetBlock(pos) many times. Currently used for more efficient collision testing.
        /// </summary>
        /// <param name="minPos"></param>
        /// <param name="maxPos"></param>
        /// <param name="onBlock">The method in which you want to check for the block, whatever it may be</param>
        /// <param name="centerOrder">If true, the blocks will be ordered by the distance to the center position</param>
        /// <returns></returns>
        void WalkBlocks(BlockPos minPos, BlockPos maxPos, API.Common.Action<Block, BlockPos> onBlock, bool centerOrder = false);


        /// <summary>
        /// Set a block at the given position. Use blockid 0 to clear that position from any blocks. Marks the chunk dirty so that it gets saved to disk during shutdown or next autosave.
        /// </summary>
        /// <param name="blockId"></param>
        /// <param name="pos"></param>
        void SetBlock(ushort blockId, BlockPos pos);

        /// <summary>
        /// Set a block at the given position. Use blockid 0 to clear that position from any blocks. Marks the chunk dirty so that it gets saved to disk during shutdown or next autosave.
        /// </summary>
        /// <param name="blockId"></param>
        /// <param name="pos"></param>
        /// <param name="byItemstack">If set then it will be passed onto the block.OnBlockPlaced method</param>
        void SetBlock(ushort blockId, BlockPos pos, ItemStack byItemstack);

        /// <summary>
        /// Set a block at the given position without calling OnBlockRemoved or OnBlockPlaced, which prevents any block entity from being removed or placed. Marks the chunk dirty so that it gets saved to disk during shutdown or next autosave.
        /// </summary>
        void ExchangeBlock(ushort blockId, BlockPos pos);


        /// <summary>
        /// Removes the block at given position and calls Block.GetDrops(), Block.OnBreakBlock() and Block.OnNeighourBlockChange() for all neighbours. Drops the items that are return from Block.GetDrops()
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="byPlayer"></param>
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
        /// Spawn block entity at this position. Does not place it's corresponding block, you have to this yourself.
        /// </summary>
        /// <param name="classname"></param>
        /// <param name="position"></param>
        void SpawnBlockEntity(string classname, BlockPos position, ItemStack byItemStack = null);


        /// <summary>
        /// Permanently removes any block entity at this postion. Does not remove it's corresponding block, you have to do this yourself. Marks the chunk dirty so that it gets saved to disk during shutdown or next autosave.
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
        /// Checks if the position is inside the maps boundaries
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <returns></returns>
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
        bool IsNotTraversable(int x, int y, int z);

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
        /// Server side call: Resends the block entity data (if present) to all clients. Triggers a block changed event on the client once received , but will not redraw the chunk. Marks also the chunk dirty so that it gets saved to disk during shutdown or next autosave.
        /// Client side call: No effect
        /// </summary>
        /// <param name="pos"></param>
        void MarkBlockEntityDirty(BlockPos pos);

        /// <summary>
        /// Server side call: Triggers the method OnNeighourBlockChange() to all neighbour blocks at given position
        /// Client side call: No effect.
        /// </summary>
        /// <param name="pos"></param>
        void TriggerNeighbourBlockUpdate(BlockPos pos);

        /// <summary>
        /// Server side: Triggers a OnNeighourBlockChange on that position and sends that block to the client (via bulk packet), through that packet the client will do a SetBlock on that position (which triggers a redraw if oldblockid != newblockid).
        /// Client side: Triggers a block changed event and redraws the chunk
        /// </summary>
        /// <param name="pos"></param>
        void MarkBlockDirty(BlockPos pos);

        /// <summary>
        /// Server Side: Same as MarkBlockDirty()
        /// Client Side: Same as MarkBlockDirty(), but also calls supplied delegate after the chunk has been re-retesselated. This can be used i.e. for block entities to dynamically switch between static models and dynamic models at exactly the right timing
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="OnRetesselated"></param>
        void MarkBlockDirty(BlockPos pos, Common.Action OnRetesselated);

        /// <summary>
        /// Returns the light level (0..32) at given position. If the chunk at that position is not loaded this method will return the default sunlight value
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        int GetLightLevel(BlockPos pos, EnumLightLevelType type);

        /// <summary>
        /// Returns the light values at given position. XYZ component = block light rgb, W component = sun light brightness
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
        /// Returns the topmost solid surface position at given x/z coordinate as it was during world generation. This map is not updated after placing/removing blocks
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        int GetTerrainMapheightAt(BlockPos pos);

        /// <summary>
        /// Returns the topmost non-rain-permeable position at given x/z coordinate. This map is always updated after placing/removing blocks
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        int GetRainMapHeightAt(BlockPos pos);

        /// <summary>
        /// Returns the map chunk at given chunk position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        IMapChunk GetMapChunk(Vec2i chunkPos);

        /// <summary>
        /// Returns the map chunk at given block position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        IMapChunk GetMapChunkAtBlockPos(BlockPos pos);

        /// <summary>
        /// Returns the positions current climate conditions
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        ClimateCondition GetClimateAt(BlockPos pos);
    }
}
