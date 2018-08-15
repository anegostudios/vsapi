using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace Vintagestory.API.Server
{
    /// <summary>
    /// The player configuration that is world independent
    /// </summary>
    public interface IServerPlayerData
    {
        /// <summary>
        /// Returns all player groups which this player is part of
        /// </summary>
        /// <returns></returns>
        Dictionary<int, PlayerGroupMembership> PlayerGroupMemberships { get; }

        /// <summary>
        /// Store your own custom data in here if you need. Might want to serialize your data to json code first.
        /// </summary>
        Dictionary<string, string> CustomPlayerData { get; }

        /// <summary>
        /// Extra land claim allowance (beyond whats granted by the role)
        /// </summary>
        int ExtraLandClaimAllowance { get; set; }

        /// <summary>
        /// Extra land claim areas (beyond whats granted by the role)
        /// </summary>
        int ExtraLandClaimAreas { get; set; }

    }
}
