using Vintagestory.API.Common.Entities;

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
