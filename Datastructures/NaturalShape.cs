using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.Datastructures;

public class NaturalShape
{
    private readonly Dictionary<Vec2i, ShapeCell> outline;
    private readonly HashSet<Vec2i> inside;

    private readonly IRandom rand;
    private readonly NatFloat natFloat;
    private bool hasSquareStart;

    public NaturalShape(IRandom rand)
    {
        this.rand = rand;
        outline = new Dictionary<Vec2i, ShapeCell>();
        inside = new HashSet<Vec2i>();
        // values will be recalculated per iteration
        natFloat = NatFloat.createGauss(1, 1);
        Init();
    }

    private void Init()
    {
        var tmpVec = new Vec2i();
        outline.Add(tmpVec, new ShapeCell(tmpVec, new[] { true, true, true, true }));
    }

    public void InitSquare(int sizeX, int sizeZ)
    {
        hasSquareStart = true;
        inside.Clear();
        outline.Clear();
        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                var pos = new Vec2i(x, z);
                var openSide = new[] { false, false, false, false };
                if (z == 0) // N
                {
                    openSide[0] = true;
                }
                else if (z == sizeZ-1) // S
                {
                    openSide[2] = true;
                }
                if (x == 0) // W
                {
                    openSide[3] = true;
                }
                else if (x == sizeX-1) // E
                {
                    openSide[1] = true;
                }
                var shapeCell = new ShapeCell(pos,openSide);
                if(!openSide.Any(s => s))
                {
                    inside.Add(pos);
                }
                else
                {
                    outline.Add(pos, shapeCell);
                }
            }
        }
    }

    public bool[] GetOpenSides(Vec2i c)
    {
        var openSides = new bool[4];
        for (var i = 0; i < 4; i++)
        {
            var offsetByIndex = c + GetOffsetByIndex(i);
            var hasONe = outline.ContainsKey(offsetByIndex);
            var hasINe = inside.Contains(offsetByIndex);

            openSides[i] = !hasONe && !hasINe;
        }

        return openSides;
    }

    public ShapeCell GetBySide(ShapeCell cell, int index)
    {
        var offset = GetOffsetByIndex(index);
        var offPos = cell.Position + offset;
        var openSides = GetOpenSides(offPos);
        return new ShapeCell(offPos, openSides);
    }

    private static Vec2i GetOffsetByIndex(int index)
    {
        var offset = new Vec2i();
        switch (index)
        {
            case 0: // N
                offset.Set(0, -1);
                break;
            case 1: // E
                offset.Set(1, 0);
                break;
            case 2: // S
                offset.Set(0, 1);
                break;
            case 3: // W
                offset.Set(-1, 0);
                break;
        }

        return offset;
    }

    public void Grow(int steps)
    {
        for (var i = 0; i < steps; i++)
        {
            if (hasSquareStart)
            {
                natFloat.avg = outline.Count * 0.5f;
                natFloat.var = outline.Count * 0.5f;
            }
            else
            {
                natFloat.avg = outline.Count * 0.85f;
                natFloat.var = outline.Count * 0.15f;
            }
            var next = (int)natFloat.nextFloat(1f, rand);
            var cell = outline.ElementAt(next);

            next = rand.NextInt(4);
            ShapeCell newCell = null;
            for (var j = next; j < next + 4; j++)
            {
                if (!cell.Value.OpenSides[j % 4]) continue;

                newCell = GetBySide(cell.Value, j % 4);

                if (newCell.OpenSides.Any(s => s))
                {
                    outline.TryAdd(newCell.Position, newCell);
                }
                else
                {
                    inside.Add(newCell.Position);
                }

                break;
            }

            if (newCell == null) continue;

            // on new cell creation check its neighbours for open sides update
            for (var j = 0; j < 4; j++)
            {
                var offset = GetOffsetByIndex(j);
                var newCellPosition = newCell.Position + offset;

                if (!outline.TryGetValue(newCellPosition, out var foundCell)) continue;

                foundCell.OpenSides = GetOpenSides(newCellPosition);

                // if all sides are taken then move from outline -> inside
                if (GetOpenSides(newCellPosition).Any(s => s)) continue;

                outline.Remove(newCellPosition);
                inside.Add(new Vec2i(newCellPosition.X, newCellPosition.Y));
            }
        }
    }

    public List<BlockPos> GetPositions(BlockPos start)
    {
        var list = new List<BlockPos>();
        foreach (var (pos, _) in outline)
        {
            list.Add(new BlockPos(start.X + pos.X, start.Y, start.Z + pos.Y, 0));
        }

        foreach (var pos in inside)
        {
            list.Add(new BlockPos(start.X + pos.X, start.Y, start.Z + pos.Y, 0));
        }

        return list;
    }

    public List<Vec2i> GetPositions()
    {
        var list = new List<Vec2i>();
        foreach (var (pos, _) in outline)
        {
            list.Add(pos);
        }

        foreach (var pos in inside)
        {
            list.Add(pos);
        }

        return list;
    }
}
