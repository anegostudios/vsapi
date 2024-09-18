using System;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common.Entities;

public class PModuleGravity : PModule
{
    private double gravityPerSecond = GlobalConstants.GravityPerSecond;

    public override void Initialize(JsonObject config, Entity entity)
    {
        // Get config from behavior.
        if (config != null) gravityPerSecond = GlobalConstants.GravityPerSecond * (float)config["gravityFactor"].AsDouble(1);
    }

    // No gravity applied if:
    // Flying or gliding
    // If you're a bird
    // If you're a fish
    // If you're swimming
    // Or if you're climbing.
    public override bool Applicable(Entity entity, EntityPos pos, EntityControls controls)
    {
        return (!controls.IsFlying || controls.Gliding)
               && entity.Properties.Habitat != EnumHabitat.Air
               && ((entity.Properties.Habitat != EnumHabitat.Sea && entity.Properties.Habitat != EnumHabitat.Underwater) || !entity.Swimming)
               && !controls.IsClimbing;
    }

    public override void DoApply(float dt, Entity entity, EntityPos pos, EntityControls controls)
    {
        // Don't apply if you're swimming and trying to move.
        if (entity.Swimming && controls.TriesToMove && entity.Alive) return;

        // If gravity is off for this entity don't apply.
        if (!entity.ApplyGravity) return;

        // Drag motion down while above y -100.
        if (pos.Y > -100)
        {
            double gravity = (gravityPerSecond + Math.Max(0, -0.015f * pos.Motion.Y)) * (entity.FeetInLiquid ? 0.33f : 1f) * dt;
            pos.Motion.Y -= gravity * GameMath.Clamp(1 - (50 * controls.GlideSpeed * controls.GlideSpeed), 0, 1);
        }
    }
}
