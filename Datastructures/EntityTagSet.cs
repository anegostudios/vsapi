using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vintagestory.API.Common;

namespace Vintagestory.API.Datastructures;

public readonly struct EntityTagSet : ITagSet, IHasSetOperations<EntityTagSet>
{
    public readonly static EntityTagSet Empty = new(FixedSizeBitSet.Empty);

    public static EntityTagSet GetEmpty() => Empty;

    public readonly FixedSizeBitSet Value;



    public EntityTagSet(FixedSizeBitSet indexes)
    {
        Value = indexes;
    }

    public EntityTagSet(IEnumerable<ushort> indexes)
    {
        Value = new FixedSizeBitSet(indexes);
    }



    public EntityTagSet Union(EntityTagSet array) => new(Value.Union(array.Value));
    public EntityTagSet Intersect(EntityTagSet array) => new(Value.Intersect(array.Value));
    public EntityTagSet Except(EntityTagSet array) => new(Value.Except(array.Value));
    public EntityTagSet SymmetricExcept(EntityTagSet array) => new(Value.SymmetricExcept(array.Value));

    public bool IsSubsetOf(EntityTagSet array) => Value.IsSubsetOf(array.Value);
    public bool IsSupersetOf(EntityTagSet array) => Value.IsSupersetOf(array.Value);
    public bool Overlaps(EntityTagSet array) => Value.Overlaps(array.Value);
    public bool SetEquals(EntityTagSet array) => Value.SetEquals(array.Value);
    public bool OverlapsWithEach(IEnumerable<EntityTagSet> arrays) => Value.OverlapsWithEach(arrays.Select(array => array.Value));
    public bool SupersetOfAtLeastOne(IEnumerable<EntityTagSet> arrays) => Value.SupersetOfAtLeastOne(arrays.Select(array => array.Value));

    public bool IsEmpty() => Value == FixedSizeBitSet.Empty;

    public readonly void ToBytes(BinaryWriter writer) => Value.ToBytes(writer);
    public static EntityTagSet FromBytes(BinaryReader reader) => new(FixedSizeBitSet.FromBytes(reader));

    public static bool operator ==(EntityTagSet first, EntityTagSet second) => first.Value == second.Value;
    public static bool operator !=(EntityTagSet first, EntityTagSet second) => first.Value != second.Value;
    public readonly override bool Equals(object? obj) => Value.Equals(obj);
    public override int GetHashCode() => Value.GetHashCode();

    #region ITagSet
    static ITagSet ITagSet.GetArray(IEnumerable<ushort> indexes) => new EntityTagSet(indexes);
    static ITagSet ITagSet.Empty => Empty;
    static string ITagSet.TypeCode => "entity tags";
    IEnumerable<ushort> ITagSet.GetIndexes() => Value.ToArray();
    #endregion
}
