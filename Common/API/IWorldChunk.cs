using System.Collections.Generic;
using Vintagestory.API.Common.Entities;

namespace Vintagestory.API.Common
{
    public interface IWorldChunk
    {
        /// <summary>
        /// Holds a reference to the current map data of this chunk column
        /// </summary>
        IMapChunk MapChunk { get; }

        /// <summary>
        /// Holds all the blockids for each coordinate, access via index: (y * chunksize + z) * chunksize + x
        /// </summary>
        ushort[] Blocks { get; set; }

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
        BlockEntity[] BlockEntities { get; set; }

        /// <summary>
        /// Blockdata and Light might be compressed, always call this method if you want to access these
        /// </summary>
        void Unpack();

        /// <summary>
        /// Marks this chunk as modified. If called on server side it will be stored to disk, if called on client it will be redrawn
        /// </summary>
        void MarkModified();

        /// <summary>
        /// Returns a list of a in-chunk indexed positions of all light sources in this chunk
        /// </summary>
        HashSet<int> LightPositions { get; set; }
        

        void AddEntity(Entity entity);

        bool RemoveEntity(long entityId);
    }
}
