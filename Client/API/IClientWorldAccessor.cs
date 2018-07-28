using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// The world accessor implemented by the client, offers some extra features only available on the client
    /// </summary>
    public interface IClientWorldAccessor : IWorldAccessor
    {
        /// <summary>
        /// Whether the player can select liquids
        /// </summary>
        bool ForceLiquidSelectable { get; set;}

        /// <summary>
        /// Whether to spawn ambient particles
        /// </summary>
        bool AmbientParticles { get; set; }

        /// <summary>
        /// Returns the player running this client instance
        /// </summary>
        IClientPlayer Player { get; }

        /// <summary>
        /// Loads a sounds without playing it. Use to individually control when to play/stop. Might want to set DisposeOnFinish to false but then you have to dispose it yourself. 
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        ILoadedSound LoadSound(SoundParams param);

        /// <summary>
        /// Shakes the camera view by given strength
        /// </summary>
        /// <param name="strengh"></param>
        void ShakeCamera(float strengh);

        /// <summary>
        /// The internal cache of all currently loaded entities. Warning: You should not set or remove anything from this dic unless you *really* know what you're doing. Use SpawnEntity/DespawnEntity instead.
        /// </summary>
        Dictionary<long, Entity> LoadedEntities { get; }
    }
}
