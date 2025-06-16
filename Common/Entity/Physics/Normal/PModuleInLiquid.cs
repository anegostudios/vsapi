using System;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Common.Entities;

public class PModuleInLiquid : PModule
{
    public float Push;
    public float Swimspeed = 1f;

    public override void Initialize(JsonObject config, Entity entity)
    {
        if (!(entity is EntityPlayer))
        {
            Swimspeed = (float)entity.World.Config.GetDecimal("creatureSwimSpeed", 1);
        }
    }

    // Only applies if entity is in water.
    public override bool Applicable(Entity entity, EntityPos pos, EntityControls controls)
    {
        return entity.FeetInLiquid;
    }

    public override void DoApply(float dt, Entity entity, EntityPos pos, EntityControls controls)
    {
        // If entity is alive and swimming.
        if (entity.Swimming && entity.Alive) HandleSwimming(dt, entity, pos, controls);
        // Move entity by push vector of block.
        Block block = entity.World.BlockAccessor.GetBlockRaw((int)pos.X, (int)pos.InternalY, (int)pos.Z, BlockLayersAccess.Fluid);
        if (block.PushVector != null)
        {
            // Fix for those unfair cases where there is downward flowing water in a 1 deep hole and you can't get out.
            if (block.PushVector.Y >= 0 || !entity.World.BlockAccessor.IsSideSolid((int)pos.X, (int)pos.InternalY - 1, (int)pos.Z, BlockFacing.UP))
            {
                pos.Motion.Add(block.PushVector);
            }
        }
    }

    public virtual void HandleSwimming(float dt, Entity entity, EntityPos pos, EntityControls controls)
    {
        Push = Math.Max(1f, Push - (0.1f * dt * 60f));

        int x = (int)pos.X;
        int y = (int)pos.InternalY;
        int z = (int)pos.Z;
        Block inBlock = entity.World.BlockAccessor.GetBlockRaw(x, y, z, BlockLayersAccess.Fluid);
        Block aboveBlock = entity.World.BlockAccessor.GetBlockRaw(x, y + 1, z, BlockLayersAccess.Fluid);
        Block twoAboveBlock = entity.World.BlockAccessor.GetBlockRaw(x, y + 2, z, BlockLayersAccess.Fluid);
        float waterY = (int)pos.Y + (inBlock.LiquidLevel / 8f) + (aboveBlock.IsLiquid() ? 9 / 8f : 0) + (twoAboveBlock.IsLiquid() ? 9 / 8f : 0);
        float bottomSubmergedness = waterY - (float)pos.Y;

        // 0 => at swim line.
        // 1 => completely submerged.
        float swimLineSubmergedness = GameMath.Clamp(bottomSubmergedness - (float)entity.SwimmingOffsetY, 0, 1);
        swimLineSubmergedness = Math.Min(1, swimLineSubmergedness + 0.075f);

        double yMotion;

        // Move up if jump is pressed.
        if (controls.Jump)
        {
            yMotion = 0.005f * swimLineSubmergedness * dt * 60;
        }
        else
        {
            yMotion = controls.FlyVector.Y * (1 + Push) * 0.03f * swimLineSubmergedness;
        }

        // Fish motion.
        if (entity.Properties.Habitat == EnumHabitat.Underwater && inBlock.IsLiquid() && !aboveBlock.IsLiquid())
        {
            float maxY = (int)pos.Y + (inBlock.LiquidLevel / 8f) - entity.CollisionBox.Y2;
            if (pos.Y > maxY)
            {
                yMotion = -GameMath.Clamp(pos.Y - maxY, 0, 0.05);
            }
        }
        
        // Finally add to motion for this tick.
        pos.Motion.Add(
            Swimspeed * controls.FlyVector.X * (1 + Push) * 0.03f,
            Swimspeed * yMotion,
            Swimspeed * controls.FlyVector.Z * (1 + Push) * 0.03f
        );
    }
}
