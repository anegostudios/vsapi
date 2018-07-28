using System.Collections.Generic;
using Vintagestory.API.Common;

namespace Vintagestory.API.Server
{
    public interface IWorldGenHandler
    {
        /// <summary>
        /// List of registered map region generation handlers
        /// </summary>
        List<MapRegionGenerator> OnMapRegionGen { get; }
        /// <summary>
        /// List of registered map chunk generation handlers
        /// </summary>
        List<MapChunkGenerator> OnMapChunkGen { get; }
        /// <summary>
        /// List of registered map chunk generation handlers per pass (see EnumWorldGenPass)
        /// </summary>
        List<ChunkColumnGeneration>[] OnChunkColumnGen { get; }
        /// <summary>
        /// Empties all three lists
        /// </summary>
        void WipeAllHandlers();
    }
}
