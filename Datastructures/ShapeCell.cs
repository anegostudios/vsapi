using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.Datastructures;

public class ShapeCell
{
    public readonly Vec2i Position;
    public bool[] OpenSides;

    public ShapeCell(Vec2i position, bool[] openSides)
    {
        Position = position;
        OpenSides = openSides;
    }
}
