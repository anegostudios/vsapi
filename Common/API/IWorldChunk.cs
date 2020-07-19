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
        /// Lowest 5 bits: Sun brightness, Next 5 bits: Block brightness, Highest 6 bits: Block hue 
        /// </summary>
        ushort[] Light { get; set; }

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
        /// Marks this chunk as modified. If called on server side it will be stored to disk on the next autosave or during shutdown, if called on client it will be redrawn
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


        Block GetLocalBlockAtBlockPos(IWorldAccessor world, BlockPos position);
        BlockEntity GetLocalBlockEntityAtBlockPos(BlockPos pos);
    }
}
