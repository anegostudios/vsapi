using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Server
{
    /// <summary>
    /// The world accessor implemented by the server
    /// </summary>
    public interface IServerWorldAccessor : IWorldAccessor
    {
        /// <summary>
        /// The internal cache of all currently loaded entities. Warning: You should not set or remove anything from this dic unless you *really* know what you're doing. Use SpawnEntity/DespawnEntity instead.
        /// </summary>
        ConcurrentDictionary<long, Entity> LoadedEntities { get; }

        /// <summary>
        /// Removes an entity from the game and the chunk it resides in
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="reason"></param>
        void DespawnEntity(Entity entity, EntityDespawnReason reason);

        /// <summary>
        /// Creates an explosion at given position. 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="blastType"></param>
        /// <param name="destructionRadius"></param>
        /// <param name="injureRadius"></param>
        void CreateExplosion(BlockPos pos, EnumBlastType blastType, double destructionRadius, double injureRadius);

        /// <summary>
        /// List of all loaded tree generators
        /// </summary>
        OrderedDictionary<AssetLocation, ITreeGenerator> TreeGenerators { get; }

        /// <summary>
        /// Loads a chunk column at given chunk position in a non-blocking way, once loaded the onloaded method is called. The onloaded Callback is executed on the main thread
        /// If no player is nearby, this chunk will be unloaded after some time. You can prevent that by calling MarkFresh() on the map chunk every few seconds. Alternatively you can use 
        /// the WorldManager api to force load chunks for the duration of the game.
        /// </summary>
        /// <param name="chunkPos"></param>
        /// <param name="onloaded"></param>
        void LoadChunkColumn(int chunkX, int chunkZ, API.Common.Action onloaded);
    }
}
