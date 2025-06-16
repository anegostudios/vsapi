using Vintagestory.API.Datastructures;

#nullable disable

namespace Vintagestory.API.Common.Entities;

public class PModuleKnockback : PModule
{
    public override void Initialize(JsonObject config, Entity entity)
    {
    }

    public override bool Applicable(Entity entity, EntityPos pos, EntityControls controls)
    {
        return true;
    }

    public override void DoApply(float dt, Entity entity, EntityPos pos, EntityControls controls)
    {
        // If a knockback is queued to happen apply the motion.
        int knockbackState = entity.Attributes.GetInt("dmgkb");
        if (knockbackState == 1)
        {
            double kbX = entity.WatchedAttributes.GetDouble("kbdirX");
            double kbY = entity.WatchedAttributes.GetDouble("kbdirY");
            double kbZ = entity.WatchedAttributes.GetDouble("kbdirZ");

            pos.Motion.X += kbX;
            pos.Motion.Y += kbY;
            pos.Motion.Z += kbZ;

            entity.Attributes.SetInt("dmgkb", 0);
        }
    }
}
