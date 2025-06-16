
#nullable disable
namespace Vintagestory.API.Common
{
    /// <summary>
    /// 
    /// </summary>
    public interface IResolvableCollectible
    {
        void Resolve(ItemSlot intoslot, IWorldAccessor worldForResolve, bool resolveImports = true);
        BlockDropItemStack[] GetDropsForHandbook(ItemStack handbookStack, IPlayer forPlayer);
    }
}
