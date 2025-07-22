using System;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Common.Entities;

public class PModulePlayerInLiquid : PModuleInLiquid
{
    private long lastPush = 0;
    private readonly IPlayer player;
    private BlockPos tmpPos = new BlockPos();

    // Stores player attached.
    public PModulePlayerInLiquid(EntityPlayer entityPlayer)
    {
        player = entityPlayer.World.PlayerByUid(entityPlayer.PlayerUID);
    }

    public override void HandleSwimming(float dt, Entity entity, EntityPos pos, EntityControls controls)
    {
        if ((controls.TriesToMove || controls.Jump) && entity.World.ElapsedMilliseconds - lastPush > 2000)
        {
            Push = 6f;
            lastPush = entity.World.ElapsedMilliseconds;
            entity.PlayEntitySound("swim", player);
        }
        else
        {
            Push = Math.Max(1f, Push - 0.1f * dt * 60f);
        }

        tmpPos.dimension = pos.Dimension;
        tmpPos.Set((int)pos.X, (int)pos.Y, (int)pos.Z);
        Block inBlock = entity.World.BlockAccessor.GetBlock(tmpPos, BlockLayersAccess.Fluid);
        Block aboveBlock = entity.World.BlockAccessor.GetBlockAbove(tmpPos, 1, BlockLayersAccess.Fluid);
        Block twoAboveBlock = entity.World.BlockAccessor.GetBlockAbove(tmpPos, 2, BlockLayersAccess.Fluid);

        float waterY = (int)pos.Y + (inBlock.LiquidLevel / 8f) + (aboveBlock.IsLiquid() ? 9 / 8f : 0) + (twoAboveBlock.IsLiquid() ? 9 / 8f : 0);
        float bottomSubmergedness = waterY - (float)pos.Y;

        // 0 => at swim line.
        // 1 => completely submerged.
        float swimLineSubmergedness = GameMath.Clamp(bottomSubmergedness - ((float)entity.SwimmingOffsetY), 0, 1);
        swimLineSubmergedness = Math.Min(1, swimLineSubmergedness + 0.075f);

        double yMotion = 0;
        if (controls.Jump)
        {
            if (swimLineSubmergedness > 0.1f || !controls.TriesToMove)
            {
                yMotion = 0.005f * swimLineSubmergedness * dt * 60;
            }
        }
        else
        {
            yMotion = controls.FlyVector.Y * (1 + Push) * 0.03f * swimLineSubmergedness;
        }

        pos.Motion.Add(controls.FlyVector.X * (1 + Push) * 0.03f,
            yMotion,
            controls.FlyVector.Z * (1 + Push) * 0.03f);
    }
}
