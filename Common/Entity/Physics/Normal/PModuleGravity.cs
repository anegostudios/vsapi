using System;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable

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
    //   Flying (but not gliding!)
    //   If you're a butterfly/bird
    //   If you're a fish or frog who is currently swimming (a fish or frog on land is subject to gravity)
    // Or if you're climbing.
    public override bool Applicable(Entity entity, EntityPos pos, EntityControls controls)
    {
        var Habitat = entity.Properties.Habitat;
        if (Habitat == EnumHabitat.Air) return false;     // Butterflies and future flying birds are not subject to gravity (this may need future modification for birds such as ducks which can both swim and fly)
                                                          // For performance we check the habitat things first because they have to be checked every time anyhow: for the relevant creatures it saves having to do the following checks

        if ((Habitat == EnumHabitat.Sea || Habitat == EnumHabitat.Underwater) && entity.Swimming) return false;   // Water creatures have buoyancy

        if (controls.IsFlying && !controls.Gliding) return false;

        // Also, if gravity is off for this entity don't apply.
        return !controls.IsClimbing && entity.ApplyGravity;
    }

    public override void DoApply(float dt, Entity entity, EntityPos pos, EntityControls controls)
    {
        // Don't apply if you're swimming and trying to move.
        if (entity.Swimming && controls.TriesToMove && entity.Alive) return;

        // Drag motion down while above y -100.
        if (pos.Y > -100)
        {
            double gravity = (gravityPerSecond + Math.Max(0, -0.015f * pos.Motion.Y)) * (entity.FeetInLiquid ? 0.33f : 1f) * dt;
            pos.Motion.Y -= gravity * GameMath.Clamp(1 - (50 * controls.GlideSpeed * controls.GlideSpeed), 0, 1);
        }
    }
}
