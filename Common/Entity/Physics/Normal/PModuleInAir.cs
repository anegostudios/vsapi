using System;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common.Entities;

public class PModuleInAir : PModule
{
    public float AirMovingStrength = 0.05f;
    public double WallDragFactor = 0.3f;

    public override void Initialize(JsonObject config, Entity entity)
    {
        if (config != null)
        {
            WallDragFactor = 0.3 * (float)config["wallDragFactor"].AsDouble(1);
            AirMovingStrength = (float)config["airMovingStrength"].AsDouble(0.05);
        }
    }

    /// <summary>
    /// Applicable if the player is in fly mode or the entity isn't colliding with anything including liquid.
    /// Must be alive.
    /// </summary>
    public override bool Applicable(Entity entity, EntityPos pos, EntityControls controls)
    {
        return (controls.IsFlying || (!entity.Collided && !entity.FeetInLiquid)) && entity.Alive;
    }

    public override void DoApply(float dt, Entity entity, EntityPos pos, EntityControls controls)
    {
        if (controls.IsFlying)
        {
            ApplyFlying(dt, entity, pos, controls);
        }
        else
        {
            ApplyFreeFall(dt, entity, pos, controls);
        }
    }

    public virtual void ApplyFreeFall(float dt, Entity entity, EntityPos pos, EntityControls controls)
    {
        // Ladder motion.
        if (controls.IsClimbing)
        {
            pos.Motion.Add(controls.WalkVector);
            pos.Motion.X *= Math.Pow(1 - WallDragFactor, dt * 60);
            pos.Motion.Y *= Math.Pow(1 - WallDragFactor, dt * 60);
            pos.Motion.Z *= Math.Pow(1 - WallDragFactor, dt * 60);
        }
        else // Try to move around in the air very slowly as if walking.
        {
            float strength = AirMovingStrength * dt * 60f;
            pos.Motion.Add(controls.WalkVector.X * strength, controls.WalkVector.Y * strength, controls.WalkVector.Z * strength);
        }
    }

    /// <summary>
    /// Creative flight movement, possibly glider too?
    /// </summary>
    public virtual void ApplyFlying(float dt, Entity entity, EntityPos pos, EntityControls controls)
    {
        double deltaY = controls.FlyVector.Y;
        if (controls.Up || controls.Down)
        {
            float moveSpeed = Math.Min(0.2f, dt) * GlobalConstants.BaseMoveSpeed * controls.MovespeedMultiplier / 2;
            deltaY = (controls.Up ? moveSpeed : 0) + (controls.Down ? -moveSpeed : 0);
        }

        if (deltaY > 0 && pos.Y % BlockPos.DimensionBoundary > BlockPos.DimensionBoundary * 3 / 4)
        {
            deltaY = 0; // Prevent entities from flying too close to dimension boundaries.
        }

        pos.Motion.Add(controls.FlyVector.X, deltaY, controls.FlyVector.Z);
    }
}
