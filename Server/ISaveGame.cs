using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;

namespace Vintagestory.API.Server
{

    public interface ISaveGame
    {
        /// <summary>
        /// True if this is a newly created world
        /// </summary>
        bool IsNew { get; }

        string CreatedGameVersion { get; }
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
        List<LandClaim> LandClaims { get; set; }

        ITreeAttribute WorldConfiguration { get; }

        /// <summary>
        /// Gets a previously saved object from the savegame. Returns null if no such data under this key was previously set.
        /// </summary>
        /// <param name = "name">The key to look for</param>
        /// <returns></returns>
        byte[] GetData(string name);

        /// <summary>
        /// Store the given data persistently to the savegame.
        /// </summary>
        /// <param name = "name">Key value</param>
        /// <param name = "value">Data to save</param>
        void StoreData(string name, byte[] data);

    }
}
