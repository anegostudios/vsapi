using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

namespace Vintagestory.API.Datastructures;

using HandleType = ushort;

[JsonConverter(typeof(CollectibleTagSetConverter))]
public readonly struct TagSet : IEquatable<TagSet>
{
    // Invariant: storage is always sorted ascending.
    // Invariant: storage never contains duplicate handles.

    /// <remarks> This is internal storage and should not be manipulated in any way unless you know exactly what you are doing. </remarks>
    internal readonly ReadOnlyMemory<HandleType> storage;

    public static readonly TagSet Empty = new(ReadOnlyMemory<HandleType>.Empty);

    public TagSet() => this.storage = TagSet.Empty.storage;

    /// <param name="storage">Must be <b>backed by an array</b> that <b>conforms to the invariants</b>.</param>
    /// <remarks>If this is used incorrectly none of the operations on the resulting <see cref="TagSet"/> will work.</remarks>
    internal TagSet(ReadOnlyMemory<HandleType> storage)
    {
        this.storage = storage;
    }

    public bool IsEmpty => this.storage.IsEmpty;

    /// <returns> True if all any of the tags that are active in this instance are also active in the <paramref name="other"/> instance, false otherwise. </returns>
    /// <remarks> This is symmetrical, meaning a.Overlaps(b) == b.Overlaps(a) .</remarks>
    public readonly bool Overlaps(in TagSet other)
    {
        if (this.storage.IsEmpty || other.storage.IsEmpty) return false;

        var thisSpan = this.storage.Span;
        var otherSpan = other.storage.Span;

        // Fast rejection path: If this only starts after the end of other these cannot overlap, same for the other way round:
        // if (thisSpan[0] > otherSpan[^1]) return false; //NOTE(Rennorb): this check already happens in the first iteration of the loop.
        if (otherSpan[0] > thisSpan[^1]) return false;

        //NOTE(Rennorb) @perf: The check above could be further "improved" by first trimming the section to be looped over to only the
        // [max(minThis, minOther), min(maxThis, maxOther)], e.g. cutting of the parts that can never overlap.
        // We early out if it cannot overlap, and we stop once this reaches the end of other, but we still loop over sections that can never match.
        // In reality however, this compares between 0 and 5 tags to other 0 to 5 tags and it simply is not worth the overhead of even figuring out how long the tails are,
        // as that also requires looping over the tags, meaning loading the memory which is the slow part.
        // Doing 5x5 = 25 integer comparisons is simply not the bottleneck at the moment.

        var lastOtherHandle = otherSpan[^1];

        foreach (var thisHandle in thisSpan)
        {
            if (thisHandle > lastOtherHandle) break;

            foreach (var otherHandle in otherSpan)
            {
                if (thisHandle == otherHandle) return true;
                if (otherHandle > thisHandle) break;
            }
        }

        return false;
    }

    /// <returns> True if all tags that are active in this instance are also active in the <paramref name="other"/> instance, false otherwise. </returns>
    /// <remarks> This is NOT symmetrical, meaning a.IsFullyContainedIn(b) != b.IsFullyContainedIn(a) !</remarks>
    public readonly bool IsFullyContainedIn(in TagSet other)
    {
        // The empty set is always fully contained in any other set:
        if (this.storage.IsEmpty) return true;
        // Any non empty set cannot be contained in an empty one:
        if (other.storage.IsEmpty) return false;

        var thisSpan = this.storage.Span;
        var otherSpan = other.storage.Span;

        var firstThis = thisSpan[0];

        // Fast rejection path: If this only starts after the end of other these cannot overlap:
        if (firstThis > otherSpan[^1]) return false;

        var indexOther = 0;
        // Since the storage has the sorted invariant handles that are smaller than our first one can never match.
        // Fast forward past those handles:
        while (indexOther < otherSpan.Length && otherSpan[indexOther] < firstThis) indexOther++;


        foreach (var thisHandle in thisSpan)
        {
            for(; indexOther < otherSpan.Length; indexOther++)
            {
                var otherHandle = otherSpan[indexOther];
                if (otherHandle == thisHandle)
                {
                    // Found our current handle in other.

                    // Don't forget to skip the matched other handle.
                    // handles are unique, so we cannot match the same one again.
                    indexOther++;

                    // Now find the remaining ones.
                    goto next_this_handle;
                }
                if (otherHandle > thisHandle)
                {
                    // Since the storage has the sorted invariant it is not possible to find our current handle
                    // once other has moved past that value -> we are not fully contained.
                    return false;
                }
            }

            // We ran out of other, but were have not found all of our handles -> we are not fully contained.
            return false;

            next_this_handle:;
        }

        // We did not exit out means all our handles were found in other -> we are fully contained.
        return true;
    }

    public readonly void ToBytes(BinaryWriter writer)
    {
        var span = this.storage.Span;
        // since this can never be more elements than HandleType.MaxValue we can just send that instead of a full int.
        writer.Write((HandleType)span.Length);

        foreach (var handle in span)
        {
            writer.Write(handle);
        }
    }

    public static TagSet FromBytes(BinaryReader reader)
    {
        var length = reader.ReadUInt16();
        var storage = new HandleType[length];

        for (int i = 0; i < storage.Length; i++)
        {
            storage[i] = reader.ReadUInt16();
        }

        return new(storage);
    }

    // Debugging aid.
    public override string ToString()
    {
        var sb = new StringBuilder(256);
        var span = storage.Span;
        for (int i = 0; i < span.Length; i++)
        {
            if (i > 0)  sb.Append(", ");
            sb.Append(span[i]);
        }
        return sb.ToString();
    }

    /// <remark>
    /// Do not try to compare these against each other.<br/>
    /// This is implemented for the rare case where we want to generate distinct keys based on hashes.
    /// </remark>
    public override int GetHashCode()
    {
        int acc = 0;
        foreach (var handle in this.storage.Span)
        {
            // Random prime, no real reason to pick this specific one.
            acc = acc * 17 + handle;
        }
        return acc;
    }

    public static bool operator ==(in TagSet self, in TagSet other) => self.Equals(other);
    public static bool operator !=(in TagSet self, in TagSet other) => !self.Equals(other);
    /// <summary>
    /// A true equality test: are these exactly the same TagSet (e.g. when searching for duplicate grid recipe ingredients using the same tags)
    /// </summary>
    public readonly override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is not TagSet other) return false;
        return this.Equals(other);        
    }
    /// <summary>
    /// A true equality test: are these exactly the same TagSet (e.g. when searching for duplicate grid recipe ingredients using the same tags)
    /// </summary>
    public readonly bool Equals(TagSet other)
    {
        if (this.storage.IsEmpty && other.storage.IsEmpty) return true;
        if (this.storage.IsEmpty || other.storage.IsEmpty) return false;

        var thisSpan = this.storage.Span;
        var otherSpan = other.storage.Span;

        if (thisSpan.Length != otherSpan.Length) return false;

        for (int i = 0; i < thisSpan.Length; i++)
        {
            if (thisSpan[i] != otherSpan[i]) return false;
        }

        return true;
    }
}

public sealed class CollectibleTagSetConverter : JsonConverter<TagSet>
{
    public static TagSetConverter<TagSet> ProxyInstance = null!;

    /// <summary> Called from Main init as soon as the registries are available. </summary>
    public static void StaticInit(ITagRegistry<TagSet> registry) => ProxyInstance = new(registry);

    public override TagSet ReadJson(JsonReader reader, Type objectType, TagSet existingValue, bool hasExistingValue, JsonSerializer serializer) => ProxyInstance.ReadJson(reader, objectType, existingValue, hasExistingValue, serializer);
    public override void WriteJson(JsonWriter writer, TagSet value, JsonSerializer serializer) => throw new NotImplementedException();
}

public sealed class TagSetConverter<TTagSet>(ITagRegistry<TTagSet> registry) : JsonConverter<TTagSet> where TTagSet : struct
{
    /// <summary> Initialized from Main init. </summary>
    readonly ITagRegistry<TTagSet> registry = registry;

    public override TTagSet ReadJson(JsonReader reader, Type objectType, TTagSet existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return ReadJson(JToken.ReadFrom(reader));
    }

    public TTagSet ReadJson(JToken rootToken)
    {
        if (rootToken is JArray rootArray)
        {
            var firstType = rootArray.First?.Type;
            if (firstType == JTokenType.String)
            {
                //  ["1", "2", "3"]
                var tags = rootArray.Where(t =>
                {
                    if (t.Type != JTokenType.String) return false;
                    var str = (string?)t;

                    var error = TagRegistry.ValidateTag(str);
                    if (error == TagValidationError.None) return true;

                    registry.logger.Debug($"[{registry.debugName}] [{t.Path}] "+TagRegistry.FormatIssueMessage(error, str));
                    return false;
                })
                .Select(e => (string)e!);
                registry.TryRegisterAndCreateTagSetAndLogIssues(out var set, tags!);
                return set;
            }
            else if (!firstType.HasValue)
            {
                //  []
                return default;
            }
        }

        throw new InvalidOperationException($"Error while parsing tag set, must be an array of strings. json:\n{rootToken?.ToString()}");
    }


    public override void WriteJson(JsonWriter writer, TTagSet value, JsonSerializer serializer) => throw new NotImplementedException();
}
