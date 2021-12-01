using System.Collections.Generic;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public interface IChunkBlocks
    {
        int this[int index3d] { get; set; }

        int Length { get; }
    }


    public interface IClientChunk : IWorldChunk
    {
        /// <summary>
        /// True if fully initialized
        /// </summary>
        bool LoadedFromServer { get; }
    }


    public interface IWorldChunk
    {
        bool Empty { get; set; }
        /// <summary>
        /// Holds a reference to the current map data of this chunk column
        /// </summary>
        IMapChunk MapChunk { get; }

        /// <summary>
        /// Holds all the blockids for each coordinate, access via index: (y * chunksize + z) * chunksize + x
        /// </summary>
        IChunkBlocks Blocks { get; }

        /// <summary>
        /// Faster (non-blocking) access to blocks at the cost of sometimes returning 0 instead of the real block. Use <see cref="Blocks"/> if you need reliable block access. Also should only be used for reading. Currently used for the particle system.
        /// </summary>
        IChunkBlocks MaybeBlocks { get; }

        /// <summary>
        /// Lowest 5 bits: Sun brightness, Next 5 bits: Block brightness, Highest 6 bits: Block hue 
        /// </summary>
        ushort[] Light { get; set; }

        /// <summary>
        /// Used to access light which may be buffered, in ChunkIlluminator 
        /// </summary>
        ushort[] Light_Buffered { get; }

        /// <summary>
        /// Second Light buffer used for double-buffering in ChunkIlluminator 
        /// </summary>
        ushort[] Light_SecondBuffer { get; }

        /// <summary>
        /// Holds 3 saturation bits, the other upper 5 bits are unused. 
        /// Useful applications for the unused bits: 
        /// - 3 bits for water level for water permissible blocks, like fences: http://www.minecraftforum.net/forums/minecraft-discussion/suggestions/67465-water-should-flow-through-fences
        /// - 1-2 bits Damage value? 
        /// - 1 bit if the player placed this block or whether it was part of worldgen
        /// </summary>
        byte[] LightSat { get; set; }

        /// <summary>
        /// An array holding all Entities currently residing in this chunk. This array may be larger than the amount of entities in the chunk. 
        /// </summary>
        Entity[] Entities { get; }

        /// <summary>
        /// Actual count of entities in this chunk
        /// </summary>
        int EntitiesCount { get; }

        /// <summary>
        /// An array holding block Entities currently residing in this chunk. This array may be larger than the amount of block entities in the chunk. 
        /// </summary>
        Dictionary<BlockPos, BlockEntity> BlockEntities { get; set; }

        /// <summary>
        /// Blockdata and Light might be compressed, always call this method if you want to access these
        /// </summary>
        void Unpack();

        /// <summary>
        /// Like Unpack(), except it must be used readonly: the calling code promises not to write any changes to this chunk's blocks or lighting
        /// </summary>
        bool Unpack_ReadOnly();

        /// <summary>
        /// Like Unpack_ReadOnly(), except it actually reads and returns the block ID at index<br/>
        /// (Returns 0 if the chunk was disposed)
        /// </summary>
        int Unpack_AndReadBlock(int index);

        /// <summary>
        /// Like Unpack_ReadOnly(), except it actually reads and returns the Light at index<br/>
        /// (Returns 0 if the chunk was disposed)
        /// </summary>
        ushort Unpack_AndReadLight(int index);

        /// <summary>
        /// Marks this chunk as modified. If called on server side it will be stored to disk on the next autosave or during shutdown, if called on client not much happens (but it will be preserved from packing for next ~8 seconds)
        /// </summary>
        void MarkModified();

        /// <summary>
        /// Marks this chunk as recently accessed. This will prevent the chunk from getting compressed by the in-memory chunk compression algorithm
        /// </summary>
        void MarkFresh();

        /// <summary>
        /// Returns a list of a in-chunk indexed positions of all light sources in this chunk
        /// </summary>
        HashSet<int> LightPositions { get; set; }
        
        /// <summary>
        /// Whether this chunk got unloaded
        /// </summary>
        bool Disposed { get; }

        /// <summary>
        /// Adds an entity to the chunk.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        void AddEntity(Entity entity);

        /// <summary>
        /// Removes an entity from the chunk.
        /// </summary>
        /// <param name="entityId">the ID for the entity</param>
        /// <returns>Whether or not the entity was removed.</returns>
        bool RemoveEntity(long entityId);


        /// <summary>
        /// Allows setting of arbitrary, permanantly stored moddata of this chunk. When set on the server before the chunk is sent to the client, the data will also be sent to the client.
        /// When set on the client the data is discarded once the chunk gets unloaded
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        void SetModdata(string key, byte[] data);

        /// <summary>
        /// Removes the permanently stored data. 
        /// </summary>
        /// <param name="key"></param>
        void RemoveModdata(string key);

        /// <summary>
        /// Retrieve arbitrary, permantly stored mod data
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        byte[] GetModdata(string key);

        /// <summary>
        /// Allows setting of arbitrary, permanantly stored moddata of this chunk. When set on the server before the chunk is sent to the client, the data will also be sent to the client.
        /// When set on the client the data is discarded once the chunk gets unloaded
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>

        void SetModdata<T>(string key, T data);
        /// <summary>
        /// Retrieve arbitrary, permantly stored mod data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetModdata<T>(string key);


        /// <summary>
        /// Retrieve a block from this chunk, performs Unpack() and a modulo operation on the position arg to get a local position in the 0..chunksize range (its your job to pick out the right chunk before calling this method)
        /// </summary>
        /// <param name="world"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        Block GetLocalBlockAtBlockPos(IWorldAccessor world, BlockPos position);

        /// <summary>
        /// Retrieve a block entity from this chunk
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        BlockEntity GetLocalBlockEntityAtBlockPos(BlockPos pos);

        /// <summary>
        /// Sets a decor block to the side of an existing block. Use air block (id 0) to remove a decor.<br/>
        /// </summary>
        /// <param name="blockAccessor"></param>
        /// <param name="pos"></param>
        /// <param name="onFace"></param>
        /// <param name="block"></param>
        /// <returns>False if there already exists a block in this position and facing</returns>
        bool SetDecor(IBlockAccessor blockAccessor, Block block, BlockPos pos, BlockFacing onFace);

        /// <summary>
        /// Sets a decor block to a specific sub-position on the side of an existing block. Use air block (id 0) to remove a decor.<br/>
        /// </summary>
        /// <param name="blockAccessor"></param>
        /// <param name="pos"></param>
        /// <param name="onFace"></param>
        /// <param name="block"></param>
        /// <returns>False if there already exists a block in this position and facing</returns>
        bool SetDecor(IBlockAccessor blockAccessor, Block block, BlockPos pos, int faceAndSubposition);


        /// <summary>
        /// If allowed by a player action, removes all decors at given position and calls OnBrokenAsDecor() on all selected decors and drops the items that are returned from Block.GetDrops()
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos"></param>
        /// <param name="side">If null, all the decor blocks on all sides are removed</param>
        /// <param name="faceAndSubposition">If not null breaks only this part of the decor for give face. Requires side to be set.</param>
        bool BreakDecor(IWorldAccessor world, BlockPos pos, BlockFacing side = null, int? faceAndSubposition = null);


        /// <summary>
        /// Removes a decor block from given position, saves a few cpu cycles by not calculating index3d
        /// </summary>
        /// <param name="world"></param>
        /// <param name="pos"></param>
        /// <param name="index3d"></param>
        void BreakAllDecorFast(IWorldAccessor world, BlockPos pos, int index3d);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blockAccessor"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        Block[] GetDecors(IBlockAccessor blockAccessor, BlockPos pos);

        Block GetDecor(IBlockAccessor blockAccessor, BlockPos pos, int faceAndSubposition);

        /// <summary>
        /// Set entire Decors for a chunk - used in Server->Client updates
        /// </summary>
        /// <param name="newDecors"></param>
        void SetDecors(Dictionary<int, Block> newDecors);

        /// <summary>
        /// Adds extra selection boxes in case a decor block is attached at given position
        /// </summary>
        /// <param name="blockAccessor"></param>
        /// <param name="pos"></param>
        /// <param name="orig"></param>
        /// <returns></returns>
        Cuboidf[] AdjustSelectionBoxForDecor(IBlockAccessor blockAccessor, BlockPos pos, Cuboidf[] orig);

        /// <summary>
        /// Only to be implemented client side
        /// </summary>
        void FinishLightDoubleBuffering();
    }
}
