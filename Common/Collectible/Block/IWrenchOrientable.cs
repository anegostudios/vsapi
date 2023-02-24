using Vintagestory.API.Common;

namespace Vintagestory.GameContent
{
    public interface IWrenchOrientable
    {
        void Rotate(EntityAgent byEntity, BlockSelection blockSel, int dir);
    }
}
