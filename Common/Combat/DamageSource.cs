using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    public class DamageSource
    {
        /// <summary>
        /// The type of source the damage came from.
        /// </summary>
        public EnumDamageSource Source;

        /// <summary>
        /// the type of damage that was taken.
        /// </summary>
        public EnumDamageType Type;

        /// <summary>
        /// The hit position of the damage.
        /// </summary>
        public Vec3d HitPosition;

        /// <summary>
        /// The source entity the damge came from. (if any)
        /// </summary>
        public Entity SourceEntity;

        /// <summary>
        /// The source block the damage came from. (if any)
        /// </summary>
        public Block sourceBlock;

        /// <summary>
        /// the location of the damage source.
        /// </summary>
        public Vec3d sourcePos;

        /// <summary>
        /// Fetches the location of the damage source (reliable position)
        /// </summary>
        /// <returns></returns>
        public Vec3d GetSourcePosition()
        {
            return SourceEntity == null ? sourcePos : SourceEntity.Pos.XYZ;
        }
    }
}
