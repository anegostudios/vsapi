
using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

#nullable disable

namespace Vintagestory.API.Server
{
    public interface IChunkProvider
    {
        ILogger Logger { get; }

        IWorldChunk GetChunk(int chunkX, int chunkY, int chunkZ);

        /// <summary>
        /// Like GetChunk() but includes a cache of the last chunk fetched - use this in a loop where getting the chunk for neighbouring blockPos, so most will be in the same chunk
        /// This always unpacks the chunk, unless it is null.
        /// Implementing code must ensure the chunk is unpacked before returning it, unless returning the cached chunk and notRecentlyAccessed is false (i.e. if the cache chunk was recently accessed then it's safe to assume it was already unpacked)
        /// </summary>
        /// <param name="chunkX"></param>
        /// <param name="chunkY"></param>
        /// <param name="chunkZ"></param>
        /// <param name="notRecentlyAccessed"></param>
        /// <returns></returns>
        IWorldChunk GetUnpackedChunkFast(int chunkX, int chunkY, int chunkZ, bool notRecentlyAccessed = false);

        /// <summary>
        /// Index for a chunk coordinate - NOT DIMENSION AWARE
        /// </summary>
        /// <param name="chunkX"></param>
        /// <param name="chunkY"></param>
        /// <param name="chunkZ"></param>
        /// <returns></returns>
        [Obsolete("Use dimension aware overloads instead")]
        long ChunkIndex3D(int chunkX, int chunkY, int chunkZ);

        long ChunkIndex3D(EntityPos pos);
    }
}
