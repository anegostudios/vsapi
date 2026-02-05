using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vintagestory.API.Common;

namespace Vintagestory.API.Datastructures;

public readonly struct TagSet : ITagSet, IHasSetOperations<TagSet>
{
    public readonly static TagSet Empty = new(CachedSortedSet<ushort>.Empty);

    public readonly CachedSortedSet<ushort> Value;



    public TagSet(CachedSortedSet<ushort> indexes)
    {
        Value = indexes;
    }
    public TagSet(IEnumerable<ushort> indexes)
    {
        Value = CachedSortedSet<ushort>.Get(indexes);
    }



    public TagSet Union(TagSet array) => new(Value.Union(array.Value));
    public TagSet Intersect(TagSet array) => new(Value.Intersect(array.Value));
    public TagSet Except(TagSet array) => new(Value.Except(array.Value));
    public TagSet SymmetricExcept(TagSet array) => new(Value.SymmetricExcept(array.Value));
    public bool IsSubsetOf(TagSet array) => Value.IsSubsetOf(array.Value);
    public bool IsSupersetOf(TagSet array) => Value.IsSupersetOf(array.Value);
    public bool Overlaps(TagSet array) => Value.Overlaps(array.Value);
    public bool SetEquals(TagSet array) => Value.SetEquals(array.Value);
    public bool OverlapsWithEach(IEnumerable<TagSet> arrays) => Value.OverlapsWithEach(arrays.Select(array => array.Value));
    public bool SupersetOfAtLeastOne(IEnumerable<TagSet> arrays) => Value.SupersetOfAtLeastOne(arrays.Select(array => array.Value));
    public bool IsEmpty() => Value == CachedSortedSet<ushort>.Empty;

    public readonly void ToBytes(BinaryWriter writer)
    {
        writer.Write(Value.Count);
        for (int elementIndex = 0; elementIndex < Value.Count; elementIndex++)
        {
            writer.Write(Value[elementIndex]);
        }
    }
    public static TagSet FromBytes(BinaryReader reader)
    {
        int size = reader.ReadInt32();
        List<ushort> elements = [];
        for (int elementIndex = 0; elementIndex < size; elementIndex++)
        {
            elements.Add(reader.ReadUInt16());
        }

        return new(CachedSortedSet<ushort>.Get(elements));
    }

    public static bool operator ==(TagSet first, TagSet second) => first.Value == second.Value;
    public static bool operator !=(TagSet first, TagSet second) => first.Value != second.Value;
    public readonly override bool Equals(object? obj) => Value.Equals(obj);
    public override int GetHashCode() => Value.GetHashCode();

    public static TagSet GetEmpty() => Empty;



    #region ITagSet
    static ITagSet ITagSet.GetArray(IEnumerable<ushort> indexes) => new TagSet(CachedSortedSet<ushort>.Get(indexes));
    static ITagSet ITagSet.Empty => Empty;
    static string ITagSet.TypeCode => "general tags";
    IEnumerable<ushort> ITagSet.GetIndexes() => Value.ToArray();
    #endregion
}
