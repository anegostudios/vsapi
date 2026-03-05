using System;
using ProtoBuf;
using Vintagestory.API.MathTools;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.Common.Collectible.Block;

[ProtoContract]
public struct ConnectorMetaData : IEquatable<ConnectorMetaData>
{
    /// <summary>
    /// This is an offset when loaded from schematic, but an absolute position if its part of the OpenSet during dungeon generation
    /// </summary>
    [ProtoMember(1)]
    public FastVec3i Position;

    [ProtoMember(2)]
    public int FacingInt;

    public BlockFacing Facing;

    [ProtoMember(3)]
    public int Rotation;

    [ProtoMember(4)]
    public string Name = string.Empty;

    [ProtoMember(5)]
    public string[] Targets = Array.Empty<string>();

    [ProtoMember(6)]
    public string[] TargetsForParent = Array.Empty<string>();

    public string FromSchematicForDebug = null;

    public ConnectorMetaData(FastVec3i position, BlockFacing facing, int rot, string name, string targets, string[] targetsforparent)
    {
        Position = position;
        Facing = facing;
        Rotation = rot;
        Name = name ?? string.Empty;
        Targets = string.IsNullOrEmpty(targets) ? Array.Empty<string>() : targets.Split(",");
        TargetsForParent = targetsforparent;
    }

    public ConnectorMetaData(FastVec3i position, BlockFacing facing, int rot, string name, string[] targets, string[] targetsforparent)
    {
        Position = position;
        Facing = facing;
        Rotation = rot;
        Name = name ?? string.Empty;
        Targets = targets;
        TargetsForParent = targetsforparent;
    }

    public bool ConnectsTo(string name)
    {
        foreach (var target in Targets)
        {
            if (WildcardUtil.Match(target, name)) return true;
        }

        return false;
    }

    public bool ConnectsTo(ConnectorMetaData p)
    {
        return Valid && p.Valid && p.Facing.Opposite == Facing && ((Name.Length > 0 && p.ConnectsTo(Name)) || (p.Name.Length > 0 && ConnectsTo(p.Name)));
    }

    public bool ConnectsTo(ConnectorMetaData p, FastVec3i pos)
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

    public ConnectorMetaData Offset(FastVec3i startPos)
    {
        Position += startPos;
        return this;
    }

    public override string ToString()
    {
        return string.Format("n-{0} t-{1} ({2})", Name, string.Join(",", Targets), FromSchematicForDebug);
    }
}
