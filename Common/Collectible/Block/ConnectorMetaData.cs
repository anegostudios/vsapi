using System;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.Common.Collectible.Block;

public struct ConnectorMetaData : IEquatable<ConnectorMetaData>
{
    /// <summary>
    /// This is an offset when loaded from schematic, but an absolute position if its part of the OpenSet during dungeon generation
    /// </summary>
    public BlockPos Position;
    public BlockFacing Facing;
    public int Rotation;
    public string Name = string.Empty;
    public string[] Targets = Array.Empty<string>();
    public string[] TargetsForParent = Array.Empty<string>();

    public ConnectorMetaData(BlockPos position, BlockFacing facing, int rot, string name, string targets, string[] targetsforparent)
    {
        Position = position;
        Facing = facing;
        Rotation = rot;
        Name = name ?? string.Empty;
        Targets = string.IsNullOrEmpty(targets) ? Array.Empty<string>() : targets.Split(",");
        TargetsForParent = targetsforparent;
    }

    public ConnectorMetaData(BlockPos position, BlockFacing facing, int rot, string name, string[] targets, string[] targetsforparent)
    {
        Position = position;
        Facing = facing;
        Rotation = rot;
        Name = name ?? string.Empty;
        Targets = targets;
        TargetsForParent = targetsforparent;
    }

    public bool ConnectsTo(ConnectorMetaData p)
    {
        return Valid && p.Valid && p.Facing.Opposite == Facing && ((Name.Length > 0 && p.Targets.Contains(Name)) || (p.Name.Length > 0 && Targets.Contains(p.Name)));
    }

    public bool ConnectsTo(ConnectorMetaData p, BlockPos pos)
    {
        return ConnectsTo(p) && (Position + pos).Add(Facing) == p.Position;
    }

    /// <summary>
    /// True if this a properly set up BlockPosFacing
    /// </summary>
    public bool Valid => Facing != null;

    public bool Equals(ConnectorMetaData other)
    {
        return Equals(Position, other.Position) && Equals(Facing, other.Facing) && Name == other.Name && Targets == other.Targets;
    }

    public override bool Equals(object obj)
    {
        return obj is ConnectorMetaData other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Position, Facing, Name, Targets);
    }

    public ConnectorMetaData Clone()
    {
        return new ConnectorMetaData(Position, Facing, Rotation, Name, Targets, TargetsForParent);
    }

    public ConnectorMetaData Offset(BlockPos startPos)
    {
        Position += startPos;
        return this;
    }
}
