using System;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Vintagestory.GameContent
{
    public interface ICoolingMedium
    {
        bool CanCool(ItemSlot slot, Vec3d pos);
        void CoolNow(ItemSlot slot, Vec3d pos, float dt, bool playSizzle = true);
    }
}
