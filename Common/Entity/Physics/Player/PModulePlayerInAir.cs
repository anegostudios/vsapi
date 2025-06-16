using System;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common.Entities;

public class PModulePlayerInAir : PModuleInAir
{
    private float airMovingStrengthFalling;

    public override void Initialize(JsonObject config, Entity entity)
    {
        base.Initialize(config, entity);

        // Player has a much harder time moving while falling.
        airMovingStrengthFalling = AirMovingStrength / 4;
    }

    public override void ApplyFreeFall(float dt, Entity entity, EntityPos pos, EntityControls controls)
    {
        // Apply normally if climbing.
        if (controls.IsClimbing)
        {
            base.ApplyFreeFall(dt, entity, pos, controls);
        }
        else // Different values when freefalling.
        {
            float strength = AirMovingStrength * Math.Min(1, ((EntityPlayer)entity).walkSpeed) * dt * 60;

            if (!controls.Jump)
            {
                strength = airMovingStrengthFalling;
                pos.Motion.X *= (float)Math.Pow(0.98f, dt * 33);
                pos.Motion.Z *= (float)Math.Pow(0.98f, dt * 33);
            }

            pos.Motion.Add(controls.WalkVector.X * strength, controls.WalkVector.Y * strength, controls.WalkVector.Z * strength);
        }
    }

    public override void ApplyFlying(float dt, Entity entity, EntityPos pos, EntityControls controls)
    {
        if (controls.Gliding)
        {
            double cosPitch = Math.Cos(pos.Pitch);
            double sinPitch = Math.Sin(pos.Pitch);

            double cosYaw = Math.Cos(pos.Yaw);
            double sinYaw = Math.Sin(pos.Yaw);

            double glideFactor = sinPitch + 0.15;

            controls.GlideSpeed = GameMath.Clamp(controls.GlideSpeed - (glideFactor * dt * 0.25f), 0.005f, 0.75f);

            var gliderMaxSpeed = entity.Stats.GetBlended("gliderSpeedMax") - 0.8; // 0.2f, 0.015
            var glideSpeed = GameMath.Clamp(controls.GlideSpeed, 0.005f, gliderMaxSpeed);

            var gliderLiftMax = entity.Stats.GetBlended("gliderLiftMax"); // 1 , -0.01
            var pitch = Math.Min(sinPitch * glideSpeed, gliderLiftMax);

            // Calculate glide vector and add it to the motion.
            pos.Motion.Add(
                -cosPitch * sinYaw * glideSpeed,
                pitch,
                -cosPitch * cosYaw * glideSpeed
            );

            pos.Motion.Mul(GameMath.Clamp(1 - (pos.Motion.Length() * 0.13f), 0, 1));
        }
        else
        {
            // Apply creative flight while not flying.
            base.ApplyFlying(dt, entity, pos, controls);
        }
    }
}
