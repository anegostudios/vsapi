using System;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common.Entities;

public class PModuleOnGround : PModule
{
    // Time in milliseconds when jumping is blocked, after last jump
    private const long MinimumJumpInterval = 500;

    // Time the player last jumped.
    private long lastJump;

    // Factor motion will be slowed by the ground.
    private double groundDragFactor = 0.3f;

    private float accum;

    // Time the player can walk off an edge before gravity applies.
    private float coyoteTimer;
    // Getting knocked back disables coyote time for a while
    private float antiCoyoteTimer;

    private double motionDeltaX;
    private double motionDeltaZ;

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

        if (coyoteTimer > 0)
        {
            if (entity.Attributes.GetInt("dmgkb") > 0)
        {
            coyoteTimer = 0;
            antiCoyoteTimer = 0.16f;
                return onGround;    //  Effectively:     return onGround || coyoteTimer > 0;
        }

            return true;
        }

        return onGround;
    }

    public override void DoApply(float dt, Entity entity, EntityPos pos, EntityControls controls)
    {
        // Tick coyote time.
        coyoteTimer -= dt;

        if (antiCoyoteTimer > 0) antiCoyoteTimer = Math.Max(0, antiCoyoteTimer - dt);    // radfast 6.2.25: coded for performance, antiCoyoteTimer is rarely non-zero, only following knockback

        // Get block below's drag - or lack thereof, in the case of ice :)
        float belowBlockDragMultiplier = entity.World.BlockAccessor.GetBlockRaw((int)pos.X, (int)(pos.InternalY - 0.05f), (int)pos.Z).DragMultiplier;

        // Only accumulator in physics modules.
        float accum = Math.Min(1, this.accum + dt);    // Local variable for accum for performance, we set back the field at the end of checks
        float frameTime = 1 / 60f;

        // Move by current walk vector (set in AI and by player).
        double multiplier = (entity as EntityAgent).GetWalkSpeedMultiplier(groundDragFactor);
        double walkX = controls.WalkVector.X * multiplier;
        double walkZ = controls.WalkVector.Z * multiplier;
        var motion = pos.Motion;
        double groundDrag = 1 - groundDragFactor;
        while (accum > frameTime)                   // radfast 6.2.25: This is always going to be true at least once and likely twice on a server, as frameTime is so small. Therefore in perfomance terms it's OK to get the belowBlock even prior to testing this condition
        {
            accum -= frameTime;

            if (entity.Alive)
            {
                motionDeltaX += (walkX - motionDeltaX) * belowBlockDragMultiplier;     // The value will trend towards walkX, at a rate depending on belowBlockDragMultiplier
                motionDeltaZ += (walkZ - motionDeltaZ) * belowBlockDragMultiplier;     // The value will trend towards walkZ, at a rate depending on belowBlockDragMultiplier

                motion.Add(motionDeltaX, 0, motionDeltaZ);
            }

            // Apply ground drag
            motion.X *= groundDrag;
            motion.Z *= groundDrag;
        }

        this.accum = accum;

        // Only able to jump every 500ms. Only works while on the ground or coyoteTimer still active...
        if (controls.Jump && entity.World.ElapsedMilliseconds - lastJump > MinimumJumpInterval && entity.Alive)
        {
            EntityPlayer entityPlayer = entity as EntityPlayer;

            lastJump = entity.World.ElapsedMilliseconds;

            // Set jump motion to something.
            float jumpHeightMultiplier = MathF.Sqrt(MathF.Max(1f, (entityPlayer?.Stats.GetBlended("jumpHeightMul") ?? 1f)));
            pos.Motion.Y = GlobalConstants.BaseJumpForce * 1 / 60f * jumpHeightMultiplier;

            // Play jump sound.
            IPlayer player = entityPlayer?.World.PlayerByUid(entityPlayer.PlayerUID);
            entity.PlayEntitySound("jump", player, false);
        }
    }
}
