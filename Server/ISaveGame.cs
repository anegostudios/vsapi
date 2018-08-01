using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace Vintagestory.API.Server
{

    public interface ISaveGame
    {
        /// <summary>
        /// True if this is a newly created world
        /// </summary>
        bool IsNew { get; }

        int Seed { get; set; }
        long TotalGameSeconds { get; set; }
        string WorldName { get; set; }
        bool AllowCreativeMode { get; set; }
        EnumPlayStyle WorldPlayStyle { get; set; }
        bool EntitySpawning { get; set; }
        List<LandClaim> LandClaims { get; set; }
    }
}
