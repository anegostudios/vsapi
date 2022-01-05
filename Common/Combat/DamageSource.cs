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
        /// The type of damage that was taken.
        /// </summary>
        public EnumDamageType Type;

        /// <summary>
        /// The relative hit position of where the damage occured.
        /// </summary>
        public Vec3d HitPosition;

        /// <summary>
        /// The source entity the damge came from. (if any)
        /// </summary>
        public Entity SourceEntity;

        /// <summary>
        /// The source block the damage came from. (if any)
        /// </summary>
        public Block SourceBlock;

        /// <summary>
        /// the location of the damage source.
        /// </summary>
        public Vec3d SourcePos;

        /// <summary>
        /// Tier of the weapon used to damage the entity (if any)
        /// </summary>
        public int DamageTier = 0;

        /// <summary>
        /// The amount of knockback this damage will incur
        /// </summary>
        public float KnockbackStrength = 1f;

        /// <summary>
        /// Fetches the location of the damage source from either SourcePos or SourceEntity
        /// </summary>
        /// <returns></returns>
        public Vec3d GetSourcePosition()
        {
            return SourceEntity == null ? SourcePos : SourceEntity.SidedPos.XYZ;
        }
    }
}
