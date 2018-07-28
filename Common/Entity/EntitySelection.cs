using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public class EntitySelection
    {
        public IEntity entity;
        public Vec3d Position;
        public BlockFacing Face;
        public Vec3d HitPosition;
    }

}
