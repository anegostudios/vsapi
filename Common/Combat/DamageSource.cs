using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public class DamageSource
    {
        public EnumDamageSource Source;
        public EnumDamageType Type;
        public Vec3d HitPosition;

        public Entity SourceEntity;
        public Block sourceBlock;
        public Vec3d sourcePos;


        public Vec3d GetSourcePosition()
        {
            return SourceEntity == null ? sourcePos : SourceEntity.Pos.XYZ;
        }
    }
}
