using System.Collections.Generic;
using Vintagestory.API.Common;

#nullable disable

namespace Vintagestory.API.Server
{
    public interface IWorldGenHandler
    {
        /// <summary>
        /// List of registered map region generation handlers
        /// </summary>
        List<MapRegionGeneratorDelegate> OnMapRegionGen { get; }
        /// <summary>
        /// List of registered map chunk generation handlers
        /// </summary>
        List<MapChunkGeneratorDelegate> OnMapChunkGen { get; }
        /// <summary>
        /// List of registered map chunk generation handlers per pass (see EnumWorldGenPass)
        /// </summary>
        List<ChunkColumnGenerationDelegate>[] OnChunkColumnGen { get; }
        /// <summary>
        /// Empties all three lists
        /// </summary>
        void WipeAllHandlers();
    }
}
