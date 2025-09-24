using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Server
{
    /// <summary>
    /// The world accessor implemented by the server
    /// </summary>
    public interface IServerWorldAccessor : IWorldAccessor
    {
        /// <summary>
        /// The internal cache of all currently loaded entities. Warning: You should not set or remove anything from this dic unless you *really* know what you're doing. Use SpawnEntity/DespawnEntity instead.
        /// <br/><br/>Intended to be used, for example, to search for an entity's presence, or to iterate through all loaded entities.
        /// <br/><br/>Please leave it to the game engine to add and remove entities from this dictionary.  From 1.18.2 onwards, if you really need to add or remove anything directly to/from this dictionary in your own code - which is *strongly* not recommended - then you should cast this to (CachingConcurrentDictionary) first.
        /// </summary>
        ConcurrentDictionary<long, Entity> LoadedEntities { get; }

        /// <summary>
        /// Removes an entity from the game and the chunk it resides in
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="reason"></param>
        void DespawnEntity(Entity entity, EntityDespawnData reason);

        /// <summary>
        /// Creates an explosion at given position.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="blastType"></param>
        /// <param name="destructionRadius"></param>
        /// <param name="injureRadius"></param>
        /// <param name="blockDropChanceMultiplier"></param>
        /// <param name="ignitedByPlayerUid"></param>
        void CreateExplosion(BlockPos pos, EnumBlastType blastType, double destructionRadius, double injureRadius, float blockDropChanceMultiplier = 1, string ignitedByPlayerUid = null);

        /// <summary>
        /// List of all loaded tree generators
        /// </summary>
        Datastructures.OrderedDictionary<AssetLocation, ITreeGenerator> TreeGenerators { get; }

        /// <summary>
        /// Dictionary of entity codes to remap, old codes (which may be present in save files or schematics) as keys
        /// </summary>
        Dictionary<string, string> RemappedEntities { get; }

        /// <summary>
        /// Gets the WorldName from the save game
        /// </summary>
        string WorldName { get; }

        bool IsFullyLoadedChunk(BlockPos pos);
    }
}
