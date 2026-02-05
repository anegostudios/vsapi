namespace Vintagestory.API.Common.Entities;

public interface IProjectileJsonConfig
{
    /// <summary>
    /// Projectile damage when hitting an entity.
    /// </summary>
    float Damage { get; }

    /// <summary>
    /// Projectile damage tier when hitting an entity.
    /// </summary>
    int DamageTier { get; }

    /// <summary>
    /// Projectile damage type when hitting an entity.
    /// </summary>
    EnumDamageType DamageType { get; }

    /// <summary>
    /// If damage source done by projectile will ignore invincibility frames.
    /// </summary>
    bool IgnoreInvFrames { get; }

    /// <summary>
    /// Chance for projectile to not be destroyed on impact.
    /// </summary>
    float DropOnImpactChance { get; }

    /// <summary>
    /// How much <see cref="IProjectile.ProjectileStack"/> should be damage on impact.
    /// </summary>
    bool DamageStackOnImpact { get; }

    /// <summary>
    /// If projectile can be collected when stuck.
    /// </summary>
    bool Collectible { get; }

    /// <summary>
    /// Projectile weight used for knockback calculations.
    /// </summary>
    float Weight { get; }
}

/// <summary>
/// Interface for universal approach to spawning projectile entities.<br/>
/// Some of the properties may not be used by all projectiles. But they can still be set or read.
/// </summary>
public interface IProjectile
{
    /// <summary>
    /// Entity that spawned the projectile
    /// </summary>
    Entity? FiredBy { get; set; }

    /// <summary>
    /// Projectile damage when hitting an entity.
    /// </summary>
    float Damage { get; set; }

    /// <summary>
    /// Projectile damage tier when hitting an entity.
    /// </summary>
    int DamageTier { get; set; }

    /// <summary>
    /// Projectile damage type when hitting an entity.
    /// </summary>
    EnumDamageType DamageType { get; set; }

    /// <summary>
    /// If damage source done by projectile will ignore invincibility frames.
    /// </summary>
    bool IgnoreInvFrames { get; set; }

    /// <summary>
    /// Projectile item, can be dropped on impact, or damaged, or destroyed.
    /// </summary>
    ItemStack? ProjectileStack { get; set; }

    /// <summary>
    /// Weapon item stack used to shoot this projectile.
    /// </summary>
    ItemStack? WeaponStack { get; set; }

    /// <summary>
    /// Chance for projectile to not be destroyed on impact.
    /// </summary>
    float DropOnImpactChance { get; set; }

    /// <summary>
    /// How much <see cref="ProjectileStack"/> should be damage on impact.
    /// </summary>
    bool DamageStackOnImpact { get; set; }

    /// <summary>
    /// If projectile can be collected when stuck.
    /// </summary>
    bool Collectible { get; set; }

    /// <summary>
    /// Will return true if projectile hit at least one entity.
    /// </summary>
    bool EntityHit { get; }

    /// <summary>
    /// Projectile weight used for knockback calculations.
    /// </summary>
    float Weight { get; set; }

    /// <summary>
    /// If projectile is stuck in terrain.
    /// </summary>
    bool Stuck { get; set; }

    /// <summary>
    /// Sets initial rotation if needed. Should be called each time entity is spawned before it spawned.
    /// </summary>
    void PreInitialize();

    /// <summary>
    /// Sets projectile parameters from config
    /// </summary>
    /// <param name="config"></param>
    void SetFromConfig(IProjectileJsonConfig config);
}

public class ProjectileJsonConfig : IProjectileJsonConfig
{
    /// <summary>
    /// Projectile damage when hitting an entity.
    /// </summary>
    public float Damage { get; set; } = 1;

    /// <summary>
    /// Projectile damage tier when hitting an entity.
    /// </summary>
    public int DamageTier { get; set; } = 0;

    /// <summary>
    /// Projectile damage type when hitting an entity.
    /// </summary>
    public EnumDamageType DamageType { get; set; } = EnumDamageType.PiercingAttack;

    /// <summary>
    /// If damage source done by projectile will ignore invincibility frames.
    /// </summary>
    public bool IgnoreInvFrames { get; set; } = true;

    /// <summary>
    /// Chance for projectile to not be destroyed on impact.
    /// </summary>
    public float DropOnImpactChance { get; set; } = 0;

    /// <summary>
    /// How much <see cref="IProjectile.ProjectileStack"/> should be damage on impact.
    /// </summary>
    public bool DamageStackOnImpact { get; set; } = false;

    /// <summary>
    /// If projectile can be collected when stuck.
    /// </summary>
    public bool Collectible { get; set; } = true;

    /// <summary>
    /// Projectile weight used for knockback calculations.
    /// </summary>
    public float Weight { get; set; } = -1;
}
