using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace Vintagestory.API.Datastructures;

/// <summary>
/// A set of tags. When using JSON, this is stored as a string array.
/// </summary>
/// <example> <code lang="json">
/// "tags": ["humanoid", "player", "seraph", "huntable", "habitat-land"],
/// </code>
/// </example>
[DocumentAsJson()]
[JsonConverter(typeof(EntityTagSetConverter))]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public struct TagSetFast(Vector256<UInt64> storage) : IEquatable<TagSetFast>
{
#pragma warning disable IDE0044 // Add readonly modifier. Disabled to avoid wired compiler optimizations, the value is manipulated via ref.
    internal Vector256<UInt64> storage = storage;
#pragma warning restore IDE0044

    //NOTE(Rennorb): Not obsoleting the default ctor is fine here, because the default state is not malformed.
    // public TagSetFast() { }

    public static readonly TagSetFast Empty = default;
    public static readonly TagSetFast All  = new(Vector256<UInt64>.AllBitsSet);

    public readonly bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => this.storage == Vector256<UInt64>.Zero;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TagSetFast operator |(TagSetFast l, TagSetFast r) => new(l.storage | r.storage);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TagSetFast operator &(TagSetFast l, TagSetFast r) => new(l.storage & r.storage);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TagSetFast operator ^(TagSetFast l, TagSetFast r) => new(l.storage ^ r.storage);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TagSetFast operator ~(TagSetFast l) => new(~l.storage);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(TagSetFast l, TagSetFast r) => l.storage == r.storage;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(TagSetFast l, TagSetFast r) => l.storage != r.storage;


    /// <inheritdoc cref="TagSet.Overlaps(in TagSet)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Overlaps(in TagSetFast r) => (this.storage & r.storage) != Vector256<UInt64>.Zero;
    /// <inheritdoc cref="TagSet.IsFullyContainedIn(in TagSet)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool IsFullyContainedIn(in TagSetFast r) => (this.storage & r.storage) == this.storage;

    public override readonly int GetHashCode() => storage.GetHashCode();
    public override readonly bool Equals([NotNullWhen(true)] object? obj) => storage.Equals(obj);
    public readonly bool Equals(TagSetFast other) => storage.Equals(other);

    public readonly void ToBytes(BinaryWriter writer)
    {
        Span<byte> buffer = stackalloc byte[32];
        this.storage.AsByte().StoreUnsafe(ref MemoryMarshal.AsRef<byte>(buffer));
        writer.Write(buffer);
    }

    public static TagSetFast FromBytes(BinaryReader reader)
    {
        Span<byte> buffer = stackalloc byte[32];
        reader.ReadExactly(buffer);
        return new(Vector256.LoadUnsafe(ref MemoryMarshal.AsRef<byte>(buffer)).AsUInt64());
    }
}

public sealed class EntityTagSetConverter : JsonConverter<TagSetFast>
{
    public static TagSetConverter<TagSetFast> ProxyInstance = null!;

    /// <summary> Called from Main init as soon as the registries are available. </summary>
    public static void StaticInit(ITagRegistry<TagSetFast> registry) => ProxyInstance = new(registry);

    public override TagSetFast ReadJson(JsonReader reader, Type objectType, TagSetFast existingValue, bool hasExistingValue, JsonSerializer serializer) => ProxyInstance.ReadJson(reader, objectType, existingValue, hasExistingValue, serializer);
    public override void WriteJson(JsonWriter writer, TagSetFast value, JsonSerializer serializer) => throw new NotImplementedException();
}

