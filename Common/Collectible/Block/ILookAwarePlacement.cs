using Vintagestory.API.Common;

#nullable disable

namespace Vintagestory.GameContent
{
    public interface ILookAwarePlacement
    {
        Block GetLookAwareBlockVariant(IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel);
    }
}
