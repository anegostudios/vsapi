using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Server
{
    public class ChunkLoadOptions
    {
        /// <summary>
        /// If true, the chunk will never get unloaded unless UnloadChunkColumn() is called
        /// </summary>
        public bool KeepLoaded = false;
        /// <summary>
        /// Callback for when the chunks are ready and loaded
        /// </summary>
        public API.Common.Action OnLoaded = null;
        /// <summary>
        /// Additional config to pass onto the world generators
        /// </summary>
        public ITreeAttribute ChunkGenParams = new TreeAttribute();
    }

    public delegate void OnChunkPeekedDelegate(Dictionary<Vec2i, IServerChunk[]> columnsByChunkCoordinate);

    public class ChunkPeekOptions
    {
        /// <summary>
        /// Until which world gen pass to generate the chunk (default: Done)
        /// </summary>
        public EnumWorldGenPass UntilPass = EnumWorldGenPass.Done;
        /// <summary>
        /// Callback for when the chunks are ready and loaded
        /// </summary>
        public OnChunkPeekedDelegate OnGenerated = null;
        /// <summary>
        /// Additional config to pass onto the world generators
        /// </summary>
        public ITreeAttribute ChunkGenParams = new TreeAttribute();
    }

    /// <summary>
    /// Methods to modify the game world
    /// </summary>
    public interface IWorldManagerAPI
    {
        /// <summary>
        /// Returns a (cloned) list of all currently loaded map chunks. The key is the 2d index of the map chunk, can be turned into an x/z coord
        /// </summary>
        Dictionary<long, IMapChunk> AllLoadedMapchunks { get; }

        /// <summary>
        /// Returns a (cloned) list of all currently loaded map regions. The key is the 2d index of the map region, can be turned into an x/z coord
        /// </summary>
        Dictionary<long, IMapRegion> AllLoadedMapRegions { get; }

        /// <summary>
        /// Returns a (cloned) list of all currently loaded chunks. The key is the 3d index of the chunk, can be turned into an x/y/z coord. Warning: This locks the loaded chunk dictionary during the clone, preventing other threads from updating it. In other words: Using this method often will have a significant performance impact.
        /// </summary>
        Dictionary<long, IServerChunk> AllLoadedChunks { get; }

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
        /// Gets the Server map region at given coordinate. Returns null if it's not loaded or does not exist yet
        /// </summary>
        /// <param name="index2d"></param>
        /// <returns></returns>
        IMapRegion GetMapRegion(long index2d);

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

        long ChunkIndex3D(int chunkX, int chunkY, int chunkZ);
        long MapRegionIndex2D(int regionX, int regionZ);
        long MapRegionIndex2DByBlockPos(int posX, int posZ);
        Vec3i MapRegionPosFromIndex2D(long index);


        /// <summary>
        /// Gets the Server chunk at given coordinate. Returns null if it's not loaded or does not exist yet
        /// </summary>
        /// <param name="index3d"></param>
        /// <returns></returns>
        IServerChunk GetChunk(long index3d);

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
        IBlockAccessor GetBlockAccessorBulkUpdate(bool synchronize, bool relight, bool debug = false);


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

        #endregion


        #region World Properties, Generation

        /// <summary>
        /// Returns a number that is guaranteed to be unique for the current world every time it is called. Curently use for entity herding behavior.
        /// </summary>
        /// <returns></returns>
        long GetNextUniqueId();

        /// <summary>
        /// Completely disables automatic generation of chunks that normally builds up a radius of chunks around the player. 
        /// </summary>
        bool AutoGenerateChunks { get; set; }

        /// <summary>
        /// Disables sending of normal chunks to all players except for force loaded ones using ForceLoadChunkColumn
        /// </summary>
        bool SendChunks { get; set; }


        /// <summary>
        /// Asynchronly high priority load a chunk column at given coordinate. 
        /// </summary>
        /// <param name="chunkX"></param>
        /// <param name="chunkZ"></param>
        /// <param name="options">Additional loading options</param>
        void LoadChunkColumnFast(int chunkX, int chunkZ, ChunkLoadOptions options = null);

        /// <summary>
        /// Asynchronly high priority load an area of chunk columns at given coordinates. Make sure that X1&lt;=X2 and Z1&lt;=Z2
        /// </summary>
        /// <param name="chunkX1"></param>
        /// <param name="chunkZ1"></param>
        /// <param name="chunkX2"></param>
        /// <param name="chunkZ2"></param>
        /// <param name="options">Additional loading options</param>
        void LoadChunkColumnFast(int chunkX1, int chunkZ1, int chunkX2, int chunkZ2, ChunkLoadOptions options = null);

        /// <summary>
        /// Asynchronly normal priority load a chunk column at given coordinate. No effect when already loaded.
        /// </summary>
        /// <param name="chunkX"></param>
        /// <param name="chunkZ"></param>
        /// <param name="keepLoaded">If true, the chunk will never get unloaded unless UnloadChunkColumn() is called</param>
        void LoadChunkColumn(int chunkX, int chunkZ, bool keepLoaded = false);

        /// <summary>
        /// Generates chunk at given coordinate, completely bypassing any existing world data and caching methods, in other words generates, a chunk from scratch without keeping it in the list of loaded chunks
        /// </summary>
        /// <param name="chunkX"></param>
        /// <param name="chunkZ"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        void PeekChunkColumn(int chunkX, int chunkZ, ChunkPeekOptions options);

        /// <summary>
        /// Asynchrounly checks if this chunk is currently loaded or in the savegame database. Calls the callback method with true or false once done looking up
        /// </summary>
        /// <param name="chunkX"></param>
        /// <param name="chunkZ"></param>
        /// <returns></returns>
        void TestChunkExists(int chunkX, int chunkY, int chunkZ, API.Common.Action<bool> onTested);

        /// <summary>
        /// Send or Resend a loaded chunk to all connected players. Has no effect when the chunk is not loaded
        /// </summary>
        /// <param name="chunkX"></param>
        /// <param name="chunkY"></param>
        /// <param name="chunkZ"></param>
        /// <param name="onlyIfInRange">If true, the chunk will not be sent to connected players that are out of range from that chunk</param>
        void BroadcastChunk(int chunkX, int chunkY, int chunkZ, bool onlyIfInRange = true);

        /// <summary>
        /// Send or Resend a loaded chunk to a connected player. Has no effect when the chunk is not loaded
        /// </summary>
        /// <param name="chunkX"></param>
        /// <param name="chunkY"></param>
        /// <param name="chunkZ"></param>
        /// <param name="onlyIfInRange">If true, the chunk will not be sent to connected players that are out of range from that chunk</param>
        void SendChunk(int chunkX, int chunkY, int chunkZ, IServerPlayer player, bool onlyIfInRange = true);


        /// <summary>
        /// Send or resent a loaded map chunk to all connected players. Has no effect when the map chunk is not loaded
        /// </summary>
        /// <param name="chunkX"></param>
        /// <param name="chunkZ"></param>
        /// <param name="onlyIfInRange"></param>
        void ResendMapChunk(int chunkX, int chunkZ, bool onlyIfInRange);



        /// <summary>
        /// Unloads a column of chunks at given coordinate independent of any nearby players and sends an appropriate unload packet to the player
        /// </summary>
        /// <param name="chunkX"></param>
        /// <param name="chunkZ"></param>
        void UnloadChunkColumn(int chunkX, int chunkZ);

        /// <summary>
        /// Deletes a column of chunks at given coordinate from the save file. Also deletes the map chunk and map region at the same coordinate. Also unloads the chunk in the same process.
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

        

        #endregion



        #region Get/Set Block Infos
        /// <summary>
        /// Get the ID of a certain BlockType
        /// </summary>
        /// <param name = "name">Name of the BlockType</param>
        /// <returns>ID of the BlockType</returns>
        int GetBlockId(AssetLocation name);

        
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
