#nullable disable

using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace Vintagestory.GameContent
{
    public delegate void RenderBoxDelegate(Cuboidf cuboid, float lineWidth, Vec4f color);

    public interface ICustomSelectionBoxRender
    {
        void RenderSelectionBoxes(BlockSelection blockSel, RenderBoxDelegate renderBoxHandler);
    }
}
