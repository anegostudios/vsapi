using Newtonsoft.Json;
using System.Collections.Generic;
using Vintagestory.API.Common;

#nullable disable

namespace Vintagestory.API.Server
{
    /// <summary>
    /// The player configuration that is world independent
    /// </summary>
    public interface IServerPlayerData
    {
        /// <summary>
        /// The players unique identifier
        /// </summary>
        string PlayerUID { get; }
        /// <summary>
        /// The players role code
        /// </summary>
        string RoleCode { get; }
        /// <summary>
        /// Privilige explicitly granted to this player
        /// </summary>
        HashSet<string> PermaPrivileges { get; }
        /// <summary>
        /// Privilige explicitly revoked from this player
        /// </summary>
        HashSet<string> DeniedPrivileges { get; }
        /// <summary>
        /// List of groups the player is a member off
        /// </summary>
        Dictionary<int, PlayerGroupMembership> PlayerGroupMemberships { get; }
        /// <summary>
        /// Whether or not this player wants to receive group invites
        /// </summary>
        bool AllowInvite { get; }
        /// <summary>
        /// The players last known player name. This may have changed since the last log in.
        /// </summary>
        string LastKnownPlayername { get; }

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

        public string FirstJoinDate { get; set; }
        public string LastJoinDate { get; set; }
        public string LastCharacterSelectionDate { get; set; }

    }
}
