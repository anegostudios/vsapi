using Vintagestory.API.Common;

#nullable disable

namespace Vintagestory.GameContent
{
    public interface IWrenchOrientable
    {
        void Rotate(EntityAgent byEntity, BlockSelection blockSel, int dir);
    }
}
