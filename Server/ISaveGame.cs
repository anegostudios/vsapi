using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

#nullable disable

namespace Vintagestory.API.Server
{

    public interface ISaveGame
    {
        /// <summary>
        /// True if this is a newly created world
        /// </summary>
        bool IsNew { get; }

        /// <summary>
        /// The game version under which this savegame was created
        /// </summary>
        string CreatedGameVersion { get; }

        /// <summary>
        /// The game version under which this savegame was last saved
        /// </summary>
        string LastSavedGameVersion { get; }

        int Seed { get; set; }

        /// <summary>
        /// A globally unique identifier for this savegame
        /// </summary>
        string SavegameIdentifier { get; }

        long TotalGameSeconds { get; set; }
        string WorldName { get; set; }
        //bool AllowCreativeMode { get; set; }
        string PlayStyle { get; set; }
        string WorldType { get; set; }
        bool EntitySpawning { get; set; }
        [System.Obsolete("Use sapi.WorldManager.LandClaims instead.  ISaveGame.LandClaims will be removed in 1.22")]
        List<LandClaim> LandClaims { get; }

        ITreeAttribute WorldConfiguration { get; }

        PlayerSpawnPos DefaultSpawn { get; set; }

        /// <summary>
        /// Gets a previously saved object from the savegame. Returns null if no such data under this key was previously set.
        /// </summary>
        /// <param name = "key">The key to look for</param>
        /// <returns></returns>
        byte[] GetData(string key);

        /// <summary>
        /// Store the given data persistently to the savegame. Size limit is around 1 gigabyte for *all* data stored along with the savegame datastructure. If you need more space, you have to store it somewhere else.
        /// </summary>
        /// <param name = "key">Key value</param>
        /// <param name = "data">Data to save</param>
        void StoreData(string key, byte[] data);


        /// <summary>
        /// Gets a previously saved object from the savegame. Returns null if no such data under this key was previously set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        T GetData<T>(string key, T defaultValue = default(T));

        /// <summary>
        /// Store the given data persistently to the savegame. Size limit is around 1 gigabyte for *all* data stored along with the savegame datastructure. If you need more space, you have to store it somewhere else.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        void StoreData<T>(string key, T data);

    }
}
