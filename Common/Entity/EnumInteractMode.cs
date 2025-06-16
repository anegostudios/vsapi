using Vintagestory.API.Common.Entities;

#nullable disable

namespace Vintagestory.API.Common
{
    public interface IHeldHandAnimOverrider
    {
        bool AllowHeldIdleHandAnim(Entity forEntity, ItemSlot slot, EnumHand hand);
    }
    public enum EnumInteractMode
    {
        Attack,
        Interact
    }
}
