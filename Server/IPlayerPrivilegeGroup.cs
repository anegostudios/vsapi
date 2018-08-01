using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Server
{
    public interface IPlayerRole
    {
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

    }
}
