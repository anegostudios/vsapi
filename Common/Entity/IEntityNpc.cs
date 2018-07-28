using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common.Entities;

namespace Vintagestory.API.Common.Entities
{
    public interface IEntityNpc : IEntity
    {
        EntityControls Controls { get; }
        string Name { get; }
    }
}
