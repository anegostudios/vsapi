using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Server
{
    /// <summary>
    /// Methods to modify the game world
    /// </summary>
    public interface IWorldManagerAPI
    {
        /// <summary>
        /// The worlds savegame object. If you change these values they will be permanently stored
        /// </summary>
        ISaveGame SaveGame { get; }

        /// <summary>
        /// Allows setting a 32 float array that defines the brightness of each block light level. Has to be set before any players join or any chunks are generated.
        /// </summary>
        /// <param name="lightLevels"></param>
        void SetBlockLightLevels(float[] lightLevels);

        /// <summary>
        /// Allows setting a 32 float array that defines the brightness of each sun light level. Has to be set before any players join or any chunks are generated.
        /// </summary>
        /// <param name="lightLevels"></param>
        void SetSunLightLevels(float[] lightLevels);

        /// <summary>
        /// Sets the default light range of sunlight. Default is 24. Has to be set before any players join or any chunks are generated.
        /// </summary>
        /// <param name="lightlevel"></param>
        void SetSunBrightness(int lightlevel);

        /// <summary>
        /// Sets the default sea level for the world to be generated. Currently used by the client to calculate the correct temperature/rainfall values for climate tinting.
        /// </summary>
        /// <param name="sealevel"></param>
        void SetSealLevel(int sealevel);


        #region World Access / Control

        /// <summary>
        /// Gets the Server map region at given coordinate. Returns null if it's not loaded or does not exist yet
        /// </summary>
        /// <param name="regionX"></param>
        /// <param name="regionZ"></param>
        /// <returns></returns>
        IMapRegion GetMapRegion(int regionX, int regionZ);

        /// <summary>
        /// Gets the Server map chunk at given coordinate. Returns null if it's not loaded or does not exist yet
        /// </summary>
        /// <param name="chunkX"></param>
        /// <param name="chunkZ"></param>
        /// <returns></returns>
        IMapChunk GetMapChunk(int chunkX, int chunkZ);

        /// <summary>
        /// Gets the Server chunk at given coordinate. Returns null if it's not loaded or does not exist yet
        /// </summary>
        /// <param name="chunkX"></param>
        /// <param name="chunkY"></param>
        /// <param name="chunkZ"></param>
        /// <returns></returns>
        IServerChunk GetChunk(int chunkX, int chunkY, int chunkZ);

        /// <summary>
        /// Gets the Server chunk at given coordinate. Returns null if it's not loaded or does not exist yet
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        IServerChunk GetChunk(BlockPos pos);


        /// <summary>
        /// Retrieve a customized interface to access blocks in the loaded game world.
        /// </summary>
        /// <param name="synchronize">Whether or not a call to Setblock should send the update also to all connected clients</param>
        /// <param name="relightChunk">Whether or not to relight the whole chunk after the a call to SetBlock</param>
        /// <param name="strict">Log an error message if GetBlock/SetBlock was called to an unloaded chunk</param>
        /// <param name="debug">If strict, crashes the server if a unloaded chunk was crashed, prints an exception and exports a png image of the current loaded chunks</param>
        /// <returns></returns>
        IBlockAccessor GetBlockAccessor(bool synchronize, bool relightChunk, bool strict, bool debug = false);


        /// <summary>
        /// Retrieve a customized interface to access blocks in the loaded game world. Does not to relight/sync on a SetBlock until Commit() is called. On commit all touched blocks are relit/synced at once. This method should be used when setting many blocks (e.g. tree generation, explosion, etc.).
        /// </summary>
        /// <param name="synchronize"></param>
        /// <param name="relightChunk"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        IBlockAccessor GetBlockAccessorBulkUpdate(bool synchronize, bool relightChunk, bool debug = false);


        /// <summary>
        /// Same as GetBlockAccessorBulkUpdate, additionally, each Commit() stores the previous state and you can perform undo/redo operations on these. 
        /// </summary>
        /// <param name="synchronize"></param>
        /// <param name="relightChunk"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        IBlockAccessorRevertable GetBlockAccessorRevertable(bool synchronize, bool relightChunk, bool debug = false);

        #endregion


        #region World Properties, Generation

        /// <summary>
        /// Returns a number that is guaranteed to be unique for the current world every time it is called. Curently use for entity herding behavior.
        /// </summary>
        /// <returns></returns>
        long GetNextHerdId();

        /// <summary>
        /// Completely disables automatic generation of chunks that normally builds up a radius of chunks around the player. 
        /// </summary>
        bool AutoGenerateChunks { get; set; }

        /// <summary>
        /// Disables sending of normal chunks to all players except for force loaded ones using ForceLoadChunkColumn
        /// </summary>
        bool SendChunks { get; set; }

        /// <summary>
        /// Loads/Generates a chunk independent of any nearby players and sends it to all clients ignoring whether this chunk has been sent before or not. The loading happens in a seperate thread, so it might take some time.
        /// </summary>
        /// <param name="chunkX"></param>
        /// <param name="chunkZ"></param>
        /// <param name="onlyIfInRange">If True will send the chunks only to clients that are in range with their viewdistance</param>
        void ForceLoadChunkColumn(int chunkX, int chunkZ, bool onlyIfInRange);

        /// <summary>
        /// Forcefully (re-)sends a chunk to all connected clients ignoring whether this chunk has been sent before or not
        /// </summary>
        /// <param name="chunkX"></param>
        /// <param name="chunkY"></param>
        /// <param name="chunkZ"></param>
        /// <param name="onlyIfInRange"></param>
        void ForceSendChunk(int chunkX, int chunkY, int chunkZ, bool onlyIfInRange);

        
        /// <summary>
        /// Unloads a column of chunks at given coordinate independent of any nearby players and sends an appropriate unload packet to the player
        /// </summary>
        /// <param name="chunkX"></param>
        /// <param name="chunkZ"></param>
        void UnloadChunkColumn(int chunkX, int chunkZ);

        /// <summary>
        /// Deletes a column of chunks at given coordinate from the save file. Also unloads the chunk in the same process.
        /// </summary>
        /// <param name="chunkX"></param>
        /// <param name="chunkZ"></param>
        void DeleteChunkColumn(int chunkX, int chunkZ);

        /// <summary>
        /// Width of the current world
        /// </summary>
        /// <value></value>
        int MapSizeX { get; }

        /// <summary>
        /// Height of the current world
        /// </summary>
        /// <value></value>
        int MapSizeY { get; }

        /// <summary>
        /// Length of the current world
        /// </summary>
        /// <value></value>
        int MapSizeZ { get; }

        /// <summary>
        /// Finds the first y position that is solid ground to stand on. Returns null if the chunk is not loaded.
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posZ"></param>
        /// <returns></returns>
        int? GetSurfacePosY(int posX, int posZ);

        /// <summary>
        /// Width/Length/Height in blocks of a region on the server
        /// </summary>
        /// <value></value>
        int RegionSize { get; }

        /// <summary>
        /// Width/Length/Height in blocks of a chunk on the server
        /// </summary>
        /// <value></value>
        int ChunkSize { get; }


        /// <summary>
        /// Get the seed used to generate the current world
        /// </summary>
        /// <value>The map seed</value>
        int Seed { get; }

        

        /// <summary>
        /// Gets a previously saved object from the savegame. Returns null if no such data under this key was previously set.
        /// </summary>
        /// <param name = "name">The key to look for</param>
        /// <returns></returns>
        byte[] GetData(string name);

        /// <summary>
        /// Store the given data persistently to the savegame.
        /// </summary>
        /// <param name = "name">Key value</param>
        /// <param name = "value">Data to save</param>
        void StoreData(string name, byte[] data);

      
        

        /// <summary>
        /// The current world filename
        /// </summary>
        string CurrentWorldName { get; }

        
        /// <summary>
        /// Retrieves the default spawnpoint (x/y/z coordinate)
        /// </summary>
        /// <value>Default spawnpoint</value>
        int[] DefaultSpawnPosition { get; }

        /// <summary>
        /// Permanently sets the default spawnpoint
        /// </summary>
        /// <param name = "x">X coordinate of new spawnpoint</param>
        /// <param name = "y">Y coordinate of new spawnpoint</param>
        /// <param name = "z">Z coordinate of new spawnpoint</param>
        void SetDefaultSpawnPosition(int x, int y, int z);

        /// <summary>
        /// Get the BlockType object of a certain block ID. This method causes an exception when the ID is not found
        /// </summary>
        /// <param name = "blockid">The block ID to search for</param>
        /// <returns>BlockType object</returns>
        Block GetBlockType(int blockid);

        /// <summary>
        /// Returns all loaded block types
        /// </summary>
        /// <value></value>
        Block[] BlockTypes { get; }


        /// <summary>
        /// Returns all blocktypes starting with given string
        /// </summary>
        /// <param name="codeBeginsWith"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        Block[] SearchBlockTypes(string codeBeginsWith, string domain = GlobalConstants.DefaultDomain);


        /// <summary>
        /// Sends given player a list of block positions that should be highlighted
        /// </summary>
        /// <param name="player"></param>
        /// <param name="blocks"></param>
        /// <param name="mode"></param>
        /// <param name="shape">When arbitrary, the blocks list represents the blocks to be highlighted. When Cube the blocks list should contain 2 positions for start and end</param>
        void HighlightBlocks(IPlayer player, List<BlockPos> blocks, EnumHighlightBlocksMode mode = EnumHighlightBlocksMode.Absolute, EnumHighlightShape shape = EnumHighlightShape.Arbitrary);

        #endregion



        #region Get/Set Block Infos
        /// <summary>
        /// Get the ID of a certain BlockType
        /// </summary>
        /// <param name = "name">Name of the BlockType</param>
        /// <returns>ID of the BlockType</returns>
        ushort GetBlockId(AssetLocation name);

        
        #endregion

        /// <summary>
        /// Floods the chunk column with sunlight. Only works on full chunk columns.
        /// </summary>
        /// <param name="chunks"></param>
        /// <param name="chunkX"></param>
        /// <param name="chunkZ"></param>
        void SunFloodChunkColumnForWorldGen(IWorldChunk[] chunks, int chunkX, int chunkZ);

        /// <summary>
        /// Spreads the chunk columns light into neighbour chunks and vice versa. Only works on full chunk columns.
        /// </summary>
        /// <param name="chunks"></param>
        /// <param name="chunkX"></param>
        /// <param name="chunkZ"></param>
        void SunFloodChunkColumnNeighboursForWorldGen(IWorldChunk[] chunks, int chunkX, int chunkZ);


        /// <summary>
        /// Does a complete relighting of the cuboid deliminated by given min/max pos. Completely resends all affected chunk columns to all connected nearby clients.
        /// </summary>
        /// <param name="minPos"></param>
        /// <param name="maxPos"></param>
        void FullRelight(BlockPos minPos, BlockPos maxPos);
    }
}
