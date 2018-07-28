using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public class DamageSource
    {
        public EnumDamageSource source;
        public EnumDamageType type;

        public Entity sourceEntity;
        public Block sourceBlock;
        public Vec3d sourcePos;


        public Vec3d GetSourcePosition()
        {
            return sourceEntity == null ? sourcePos : sourceEntity.Pos.XYZ;
        }
    }
}
