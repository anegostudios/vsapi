using System.Collections.Generic;
using System.Drawing;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common
{

    public interface IPlayerRole
    {
        bool AutoGrant { get; set; }
        int LandClaimAllowance { get; set; }
        Vec3i LandClaimMinSize { get; set; }
        int LandClaimMaxAreas { get; set; }

        string Code { get; }
        string Name { get; }
        string Description { get; }
        int PrivilegeLevel { get; }
        PlayerSpawnPos DefaultSpawn { get; }
        PlayerSpawnPos ForcedSpawn { get; }

        List<string> Privileges { get; }

        HashSet<string> RuntimePrivileges { get; }

        EnumGameMode DefaultGameMode { get; }

        Color Color { get; }

        bool IsSuperior(IPlayerRole role);

        bool EqualLevel(IPlayerRole role);

        void GrantPrivilege(params string[] privileges);
        void RevokePrivilege(string privilege);
    }
}
