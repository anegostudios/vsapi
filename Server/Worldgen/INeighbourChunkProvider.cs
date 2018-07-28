
using Vintagestory.API.Common;

namespace Vintagestory.API.Server
{
    public interface IChunkProvider
    {
        IWorldChunk GetChunk(int chunkX, int chunkY, int chunkZ);

        /// <summary>
        /// Index for a chunk coordinate
        /// </summary>
        /// <param name="chunkX"></param>
        /// <param name="chunkY"></param>
        /// <param name="chunkZ"></param>
        /// <returns></returns>
        long ChunkIndex3D(int chunkX, int chunkY, int chunkZ);
    }
}