using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Server
{
    /// <summary>
    /// Some extra methods available for server side chunks
    /// </summary>
    public interface IServerChunk : IWorldChunk
    {
        /// <summary>
        /// Allows setting of server side only moddata of this chunk
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        void SetServerModdata(string key, byte[] data);

        /// <summary>
        /// Retrieve server side only mod data
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        byte[] GetServerModdata(string key);

        /// <summary>
        /// The game version where this chunk was created. Please note that this is not the version at which this chunk was complete. Chunks can linger around in a half complete state for a long time. 
        /// </summary>
        string GameVersionCreated { get; }

        /// <summary>
        /// If true, this chunk is not at the edge of the loaded or generating map: all eight neighbouring chunks are fully loaded
        /// </summary>
        bool NotAtEdge { get; }

        /// <summary>
        /// Remove a block entity
        /// </summary>
        bool RemoveBlockEntity(BlockPos pos);

        /// <summary>
        /// Amount of (survival) player placed blocks 
        /// </summary>
        int BlocksPlaced { get; }

        /// <summary>
        /// Amount of (survival) player removed blocks 
        /// </summary>
        int BlocksRemoved { get; }
    }
}
