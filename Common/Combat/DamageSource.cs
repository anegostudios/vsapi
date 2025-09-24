using System;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common;

public enum EnumDamageOverTimeEffectType
{
    Unknown = 0,
    Poison,
    Bleeding
}

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
    /// The source entity the damage came from, if any
    /// </summary>
    public Entity SourceEntity;

    /// <summary>
    /// The entity that caused this damage, e.g. the entity that threw the SourceEntity projectile, if any
    /// <br/>NOTE: will be null for non-projectile damage e.g. melee attacks: to get the attacking entity properly for both melee and projectile damage, use GetCauseEntity()
    /// </summary>
    public Entity CauseEntity;

    /// <summary>
    /// The source block the damage came from, if any
    /// </summary>
    public Block SourceBlock;

    /// <summary>
    /// the location of the damage source.
    /// </summary>
    public Vec3d SourcePos;

    /// <summary>
    /// Tier of the weapon used to damage the entity, if any
    /// </summary>
    public int DamageTier = 0;

    /// <summary>
    /// The amount of knockback this damage will incur
    /// </summary>
    public float KnockbackStrength = 1f;

    /// <summary>
    /// If is greater than zero, damage will be turned into damage over time and will be fully applied over this time.<br/>
    /// If <see cref="TicksPerDuration"/> is set to 1, it will mean that damage will just be delayed by <see cref="Duration"/>.
    /// </summary>
    public TimeSpan Duration = TimeSpan.Zero;

    /// <summary>
    /// If <see cref="Duration"/> greater than zero, total damage will be split into this many portions, each applied in <see cref="Duration"/>/<see cref="TicksPerDuration"/> time after another.
    /// </summary>
    public int TicksPerDuration = 1;

    /// <summary>
    /// If <see cref="Duration"/> greater than zero, this damage will be turned into damage overt time effect with this type.<br/>
    /// This is used to look for specific types of DoT effects. For example it can be used to stop bleeding with bandages.
    /// </summary>
    public int DamageOverTimeType = 0;

    public EnumDamageOverTimeEffectType DamageOverTimeTypeEnum { get => (EnumDamageOverTimeEffectType)DamageOverTimeType; set => DamageOverTimeType = (int)value; }

    public float YDirKnockbackDiv = 1f;

    public bool IgnoreInvFrames = false;

    /// <summary>
    /// Fetches the location of the damage source from either SourcePos or SourceEntity
    /// </summary>
    /// <returns></returns>
    public Vec3d GetSourcePosition()
    {
        return SourceEntity == null ? SourcePos : SourceEntity.SidedPos.XYZ;
    }

    /// <summary>
    /// Get the entity that caused the damage.
    /// If a projectile like a stone was thrown this will return the entity that threw the stone instead of the stone.
    /// </summary>
    /// <returns>The entity that caused the damage</returns>
    public Entity GetCauseEntity()
    {
        return CauseEntity ?? SourceEntity;
    }

    /// <summary>
    /// If we have a hitposition this returns the pitch between the attacker and the attacked position
    /// </summary>
    /// <param name="attackedPos"></param>
    /// <param name="attackYaw"></param>
    /// <param name="attackPitch"></param>
    /// <returns></returns>
    public bool GetAttackAngle(Vec3d attackedPos, out double attackYaw, out double attackPitch)
    {
        double dx;
        double dy;
        double dz;
        if (HitPosition != null)
        {
            dx = HitPosition.X;
            dy = HitPosition.Y;
            dz = HitPosition.Z;
        }
        else if (SourceEntity != null)
        {
            dx = SourceEntity.Pos.X - attackedPos.X;
            dy = SourceEntity.Pos.Y - attackedPos.Y;
            dz = SourceEntity.Pos.Z - attackedPos.Z;
        }
        else if (SourcePos != null)
        {
            dx = SourcePos.X - attackedPos.X;
            dy = SourcePos.Y - attackedPos.Y;
            dz = SourcePos.Z - attackedPos.Z;
        }
        else
        {
            attackYaw = 0;
            attackPitch = 0;
            return false;
        }

        attackYaw = Math.Atan2((double)dx, (double)dz);
        double a = dy;
        float b = (float)Math.Sqrt(dx * dx + dz * dz);
        attackPitch = (float)Math.Atan2(a, b);

        return true;
    }
}
