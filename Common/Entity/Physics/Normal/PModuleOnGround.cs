using System;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common.Entities;

public class PModuleOnGround : PModule
{
    // Time the player last jumped.
    private long lastJump;

    // Factor motion will be slowed by the ground.
    private double groundDragFactor = 0.3f;

    private float accum;

    // Time the player can walk off an edge before gravity applies.
    private float coyoteTimer;
    // Getting knocked back disables coyote time for a while
    private float antiCoyoteTimer;

    private readonly Vec3d motionDelta = new();

    public override void Initialize(JsonObject config, Entity entity)
    {
        if (config != null) groundDragFactor = 0.3 * (float)config["groundDragFactor"].AsDouble(1);
    }

    public override bool Applicable(Entity entity, EntityPos pos, EntityControls controls)
    {
        bool onGround = entity.OnGround && !entity.Swimming;

        if (onGround && antiCoyoteTimer <= 0)
        {
            coyoteTimer = 0.15f;
        }

        if (coyoteTimer > 0 && entity.Attributes.GetInt("dmgkb") > 0)
        {
            coyoteTimer = 0;
            antiCoyoteTimer = 0.16f;
        }

        return onGround || coyoteTimer > 0;
    }

    public override void DoApply(float dt, Entity entity, EntityPos pos, EntityControls controls)
    {
        // Tick coyote time.
        coyoteTimer -= dt;

        antiCoyoteTimer = Math.Max(0, antiCoyoteTimer - dt);

        // Get block below.
        Block belowBlock = entity.World.BlockAccessor.GetBlock((int)pos.X, (int)(pos.InternalY - 0.05f), (int)pos.Z);

        // Only accumulator in physics modules.
        accum = Math.Min(1, accum + dt);
        float frameTime = 1 / 60f;

        while (accum > frameTime)
        {
            accum -= frameTime;

            if (entity.Alive)
            {
                // Move by current walk vector (set in AI and by player).
                double multiplier = (entity as EntityAgent).GetWalkSpeedMultiplier(groundDragFactor);

                motionDelta.Set(
                    motionDelta.X + (((controls.WalkVector.X * multiplier) - motionDelta.X) * belowBlock.DragMultiplier),
                    0,
                    motionDelta.Z + (((controls.WalkVector.Z * multiplier) - motionDelta.Z) * belowBlock.DragMultiplier)
                );

                pos.Motion.Add(motionDelta.X, 0, motionDelta.Z);
            }

            // Apply ground drag
            double dragStrength = 1 - groundDragFactor;

            pos.Motion.X *= dragStrength;
            pos.Motion.Z *= dragStrength;
        }

        // Only able to jump every 500ms. Only works while on the ground.
        if (controls.Jump && entity.World.ElapsedMilliseconds - lastJump > 500 && entity.Alive)
        {
            lastJump = entity.World.ElapsedMilliseconds;

            // Set jump motion to something.
            pos.Motion.Y = GlobalConstants.BaseJumpForce * 1 / 60f;

            // Play jump sound.
            EntityPlayer entityPlayer = entity as EntityPlayer;
            IPlayer player = entityPlayer?.World.PlayerByUid(entityPlayer.PlayerUID);
            entity.PlayEntitySound("jump", player, false);
        }
    }
}
