using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Vintagestory.API.Datastructures;

/// <summary>
/// Collection of unique ordered elements sorted in ascending order.<br/>
/// Thread-safe.<br/>
/// <br/>
/// Pros:<br/>
/// - no memory overhead<br/>
/// - fast to compare (O(1))<br/>
/// - fast to compare via set operations<br/>
/// - fast to iterate<br/>
/// - fast to check containment<br/>
/// - cheap to copy and pass around<br/>
/// <br/>
/// Cons:<br/>
/// - slow to construct<br/>
/// - slow to merge via set operations<br/>
/// </summary>
/// <typeparam name="TElement"></typeparam>
public readonly struct CachedSortedSet<TElement> : IEquatable<CachedSortedSet<TElement>>
    where TElement : IComparable<TElement>
{
    public TElement this[int index] => sequence[index];

    public int Count => sequence.Length;

    public static CachedSortedSet<TElement> Empty => new(ImmutableArray<TElement>.Empty);


    private readonly ImmutableArray<TElement> sequence;

    private static readonly SequencesCache<TElement> cache = new();



    /// <summary>
    /// Returns new or existing set. Does sorting and removes duplicate values.<br/>
    /// Slow. Get once, store and then use multiple times.
    /// </summary>
    /// <param name="elements">Any collection of elements</param>
    /// <returns></returns>
    public static CachedSortedSet<TElement> Get(IEnumerable<TElement> elements)
    {
        if (!elements.Any()) return new(ImmutableArray<TElement>.Empty);

        TElement[] _sequence = elements.Distinct().Order().ToArray();
        ImmutableArray<TElement> sequenceList = cache.Get(_sequence);
        return new(sequenceList);
    }

    public CachedSortedSet<TElement> Union(CachedSortedSet<TElement> array)
    {
        TElement[] tagsSequence = sequence.Union(array.sequence).Order().ToArray();
        return Get(tagsSequence);
    }

    public CachedSortedSet<TElement> Intersect(CachedSortedSet<TElement> array)
    {
        TElement[] tagsSequence = sequence.Intersect(array.sequence).Order().ToArray();
        return Get(tagsSequence);
    }

    public CachedSortedSet<TElement> Except(CachedSortedSet<TElement> array)
    {
        TElement[] tagsSequence = sequence.Except(array.sequence).Order().ToArray();
        return Get(tagsSequence);
    }

    public CachedSortedSet<TElement> SymmetricExcept(CachedSortedSet<TElement> array)
    {
        TElement[] tagsSequence = sequence.ToImmutableSortedSet().SymmetricExcept(array.sequence).Order().ToArray();
        return Get(tagsSequence);
    }

    public bool IsSubsetOf(CachedSortedSet<TElement> array) => IsSubset(sequence, array.sequence);

    public bool IsSupersetOf(CachedSortedSet<TElement> array) => IsSubset(array.sequence, sequence);

    public bool Overlaps(CachedSortedSet<TElement> array) => Overlaps(sequence, array.sequence);

    public bool SetEquals(CachedSortedSet<TElement> array) => Equals(array);

    public bool OverlapsWithEach(IEnumerable<CachedSortedSet<TElement>> arrays)
    {
        foreach (CachedSortedSet<TElement> array in arrays)
        {
            if (!Overlaps(array))
            {
                return false;
            }
        }

        return true;
    }

    public bool SupersetOfAtLeastOne(IEnumerable<CachedSortedSet<TElement>> arrays)
    {
        foreach (CachedSortedSet<TElement> array in arrays)
        {
            if (IsSupersetOf(array))
            {
                return true;
            }
        }

        return false;
    }

    public bool Contains(TElement value)
    {
        foreach (TElement element in sequence)
        {
            int comparison = value.CompareTo(element);
            if (comparison == 0)
            {
                return true;
            }
            else if (comparison < 0)
            {
                return false;
            }
        }

        return false;
    }

    public bool Equals(CachedSortedSet<TElement> other) => sequence.Equals(other.sequence);

    public override bool Equals(object? obj)
    {
        return obj is CachedSortedSet<TElement> && Equals((CachedSortedSet<TElement>)obj);
    }

    public override int GetHashCode()
    {
        return sequence.GetHashCode();
    }

    public static bool operator ==(CachedSortedSet<TElement> first, CachedSortedSet<TElement> second) => first.Equals(second);

    public static bool operator !=(CachedSortedSet<TElement> first, CachedSortedSet<TElement> second) => !first.Equals(second);

    public override string ToString() => sequence.Length == 0 ? "[]" : "[" + sequence.Select(element => element.ToString() ?? "_").Aggregate((f, s) => $"{f}, {s}") + "]";

    public ImmutableArray<TElement> ToArray() => sequence;



    private CachedSortedSet(ImmutableArray<TElement> sequence)
    {
        this.sequence = sequence;
    }



    private static bool Overlaps(ImmutableArray<TElement> first, ImmutableArray<TElement> second)
    {
        int firstIndex = 0;
        int secondIndex = 0;

        while (firstIndex < first.Length && secondIndex < second.Length)
        {
            int comparison = first[firstIndex].CompareTo(second[secondIndex]);

            if (comparison < 0)
            {
                firstIndex++;
            }
            else if (comparison > 0)
            {
                secondIndex++;
            }
            else
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsSubset(ImmutableArray<TElement> subset, ImmutableArray<TElement> superset)
    {
        int subsetIndex = 0;
        int subsetLength = subset.Length;
        int supersetIndex = 0;
        int supersetLength = superset.Length;

        while (subsetIndex < subsetLength && supersetIndex < supersetLength)
        {
            int comparison = subset[subsetIndex].CompareTo(superset[supersetIndex]);

            if (comparison < 0)
            {
                return false;
            }

            if (comparison == 0)
            {
                subsetIndex++;
                supersetIndex++;
                continue;
            }

            supersetIndex++;
        }

        return subsetIndex == subsetLength;
    }

#if DEBUG
    /// <summary>
    /// For use in unit tests only.
    /// </summary>
    internal static SequencesCache<TElement> GetCache() => cache;
#endif
}
