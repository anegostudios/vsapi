using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Vintagestory.API.Common;

namespace Vintagestory.API.Datastructures;

/// <summary>
/// List of entity tags meant to be used for fast comparisons. Restricts number of registered entity tags to 128
/// </summary>
public readonly struct EntityTagArray
{
    public readonly ulong BitMask1;
    public readonly ulong BitMask2;

    public const byte MasksNumber = 2;

    /// <summary>
    /// Maximum amount of different tags supported by tag array. Limited by total amount of bits in bit masks.
    /// </summary>
    public const int Size = MasksNumber * 64;

    public EntityTagArray(IEnumerable<ushort> tags)
    {
        foreach (ushort tag in tags)
        {
            WriteTagToBitMasks(tag, ref BitMask1, ref BitMask2);
        }
    }

    public EntityTagArray(ushort tag)
    {
        WriteTagToBitMasks(tag, ref BitMask1, ref BitMask2);
    }

    public EntityTagArray()
    {

    }

    public EntityTagArray(ulong bitMask1, ulong bitMask2)
    {
        BitMask1 = bitMask1;
        BitMask2 = bitMask2;
    }

    public static readonly EntityTagArray Empty = new();

    /// <summary>
    /// Converts tag array into list of tag ids sorted in ascending order
    /// </summary>
    /// <returns>List of tag ids in ascending order</returns>
    public IEnumerable<ushort> ToArray()
    {
        for (ushort index = 0; index < 64; index++)
        {
            if ((BitMask1 & (1UL << index)) != 0)
            {
                yield return (ushort)(index + 1);
            }
        }
        for (ushort index = 0; index < 64; index++)
        {
            if ((BitMask2 & (1UL << index)) != 0)
            {
                yield return (ushort)(index + 64 + 1);
            }
        }
    }

    /// <summary>
    /// Converts tag array into list of tags.
    /// </summary>
    /// <param name="api"></param>
    /// <returns></returns>
    public IEnumerable<string> ToArray(ICoreAPI api)
    {
        return ToArray().Select(api.TagRegistry.EntityTagIdToTag);
    }

    /// <summary>
    /// Checks if this tag array contains all tags from <paramref name="other"/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool ContainsAll(EntityTagArray other)
    {
        return (BitMask1 & other.BitMask1) == other.BitMask1 &&
               (BitMask2 & other.BitMask2) == other.BitMask2;
    }

    /// <summary>
    /// Checks if this tag array contains at least on tag from each element of <paramref name="tags"/>
    /// </summary>
    /// <param name="tags"></param>
    /// <returns></returns>
    public bool IntersectsWithEach(EntityTagArray[] tags)
    {
        foreach (EntityTagArray tagArray in tags)
        {
            if (!Intersect(this, tagArray)) return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if this tag array contains all tags from at least one element of <paramref name="tags"/>
    /// </summary>
    /// <param name="tags"></param>
    /// <returns></returns>
    public bool ContainsAllFromAtLeastOne(EntityTagArray[] tags)
    {
        foreach (EntityTagArray tagArray in tags)
        {
            if (ContainsAll(tagArray)) return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if two tag arrays have at least one common tag
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    public static bool Intersect(EntityTagArray first, EntityTagArray second)
    {
        ulong intersect1 = first.BitMask1 & second.BitMask1;
        ulong intersect2 = first.BitMask2 & second.BitMask2;

        return (intersect1 | intersect2) != 0;
    }

    /// <summary>
    /// Checks if this tag array has at least one common tag with <paramref name="other"/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Intersect(EntityTagArray other)
    {
        return Intersect(this, other);
    }

    public EntityTagArray Remove(EntityTagArray other)
    {
        return new(BitMask1 & ~other.BitMask1, BitMask2 & ~other.BitMask2);
    }

    public static EntityTagArray And(EntityTagArray first, EntityTagArray second)
    {
        return new EntityTagArray
        (
            first.BitMask1 & second.BitMask1,
            first.BitMask2 & second.BitMask2
        );
    }

    public static EntityTagArray Or(EntityTagArray first, EntityTagArray second)
    {
        return new EntityTagArray
        (
            first.BitMask1 | second.BitMask1,
            first.BitMask2 | second.BitMask2
        );
    }

    public static EntityTagArray Not(EntityTagArray value)
    {
        return new EntityTagArray
        (
            ~value.BitMask1,
            ~value.BitMask2
        );
    }

    public static EntityTagArray operator &(EntityTagArray first, EntityTagArray second) => And(first, second);

    public static EntityTagArray operator |(EntityTagArray first, EntityTagArray second) => Or(first, second);

    public static EntityTagArray operator ~(EntityTagArray value) => Not(value);

    public static bool operator ==(EntityTagArray first, EntityTagArray second)
    {
        return first.BitMask1 == second.BitMask1 &&
               first.BitMask2 == second.BitMask2;
    }

    public static bool operator !=(EntityTagArray first, EntityTagArray second) => !(first == second);

    public override bool Equals(object? obj)
    {
        if (obj is EntityTagArray other)
        {
            return this == other;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return (int)(BitMask1 ^ BitMask2);
    }

    public override string ToString() => PrintBitMask(BitMask2) + ":" + PrintBitMask(BitMask1);

    public void ToBytes(BinaryWriter writer)
    {
        writer.Write(BitMask1);
        writer.Write(BitMask2);
    }

    public static EntityTagArray FromBytes(BinaryReader reader)
    {
        ulong bitMask1 = reader.ReadUInt64();
        ulong bitMask2 = reader.ReadUInt64();
        return new(bitMask1, bitMask2);
    }

    private static void WriteTagToBitMasks(ushort tag, ref ulong bitMask1, ref ulong bitMask2)
    {
        if (tag == 0) return;

        int tagIndex = (tag - 1) % 64;
        int bitMaskIndex = (tag - 1) / 64;

        switch (bitMaskIndex)
        {
            case 0:
                bitMask1 |= 1UL << tagIndex;
                break;
            case 1:
                bitMask2 |= 1UL << tagIndex;
                break;
            default:
                break;
        }
    }

    private static string PrintBitMask(ulong bitMask) => $"{bitMask:X16}".Chunk(4).Select(chunk => new string(chunk)).Aggregate((first, second) => $"{first}.{second}");
}

/// <summary>
/// Pair of tag arrays that is used for implementation of tag inversion for entity ai tasks
/// </summary>
public readonly struct EntityTagRule
{
    public readonly EntityTagArray TagsThatShouldBePresent;
    public readonly EntityTagArray TagsThatShouldBeAbsent;

    public const string NotPrefix = "not-";
    public readonly static EntityTagRule Empty = new(EntityTagArray.Empty, EntityTagArray.Empty);

    public EntityTagRule(EntityTagArray tagsThatShouldBePresent, EntityTagArray tagsThatShouldBeAbsent)
    {
        TagsThatShouldBePresent = tagsThatShouldBePresent;
        TagsThatShouldBeAbsent = tagsThatShouldBeAbsent;
    }

    public EntityTagRule(ICoreAPI api, IEnumerable<string> tags)
    {
        List<string> straightTags = [];
        List<string> inverseTags = [];
        foreach (string tag in tags)
        {
            if (tag.StartsWith(NotPrefix))
            {
                inverseTags.Add(tag[NotPrefix.Length..]);
            }
            else
            {
                straightTags.Add(tag);
            }
        }

        TagsThatShouldBePresent = api.TagRegistry.EntityTagsToTagArray([.. straightTags]);
        TagsThatShouldBeAbsent = api.TagRegistry.EntityTagsToTagArray([.. inverseTags]);
    }

    public bool Intersects(EntityTagArray tags)
    {
        if (TagsThatShouldBePresent != EntityTagArray.Empty && !tags.Intersect(TagsThatShouldBePresent)) return false;
        if (TagsThatShouldBeAbsent != EntityTagArray.Empty && tags.ContainsAll(TagsThatShouldBeAbsent)) return false;

        return true;
    }

    /// <summary>
    /// Checks if <paramref name="entityTag"/> contains at least on tag from each rule from <paramref name="rules"/>.
    /// </summary>
    /// <param name="entityTag"></param>
    /// /// <param name="rules"></param>
    /// <returns></returns>
    public static bool IntersectsWithEach(EntityTagArray entityTag, EntityTagRule[] rules)
    {
        foreach (EntityTagRule rule in rules)
        {
            if (rule.TagsThatShouldBePresent != EntityTagArray.Empty && !entityTag.Intersect(rule.TagsThatShouldBePresent) ||
                rule.TagsThatShouldBeAbsent != EntityTagArray.Empty && entityTag.ContainsAll(rule.TagsThatShouldBeAbsent)) return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if <paramref name="entityTag"/> contains all tags from at least one rule from <paramref name="rules"/>.
    /// </summary>
    /// <param name="entityTag"></param>
    /// /// <param name="rules"></param>
    /// <returns></returns>
    public static bool ContainsAllFromAtLeastOne(EntityTagArray entityTag, EntityTagRule[] rules)
    {
        foreach (EntityTagRule rule in rules)
        {
            if (entityTag.ContainsAll(rule.TagsThatShouldBePresent) && !(rule.TagsThatShouldBeAbsent != EntityTagArray.Empty && entityTag.Intersect(rule.TagsThatShouldBeAbsent))) return true;
        }
        return false;
    }

    public static bool operator ==(EntityTagRule first, EntityTagRule second)
    {
        return first.TagsThatShouldBePresent == second.TagsThatShouldBePresent &&
               first.TagsThatShouldBeAbsent == second.TagsThatShouldBeAbsent;
    }
    public static bool operator !=(EntityTagRule first, EntityTagRule second)
    {
        return !(first == second);
    }
    public override bool Equals(object? obj)
    {
        if (obj is EntityTagRule other)
        {
            return this == other;
        }
        return false;
    }
    public override int GetHashCode()
    {
        return TagsThatShouldBePresent.GetHashCode() ^ TagsThatShouldBeAbsent.GetHashCode();
    }
    public override string ToString() => $"{TagsThatShouldBePresent}-{TagsThatShouldBeAbsent}";
}



/// <summary>
/// List of block tags meant to be used for fast comparisons. Restricts number of registered block tags to 256.
/// </summary>
public readonly struct BlockTagArray
{
    public readonly ulong BitMask1;
    public readonly ulong BitMask2;
    public readonly ulong BitMask3;
    public readonly ulong BitMask4;

    public const byte MasksNumber = 4;

    /// <summary>
    /// Maximum amount of different tags supported by tag array. Limited by total amount of bits in bit masks.
    /// </summary>
    public const int Size = MasksNumber * 64;

    public BlockTagArray(IEnumerable<ushort> tags)
    {
        foreach (ushort tag in tags)
        {
            WriteTagToBitMasks(tag, ref BitMask1, ref BitMask2, ref BitMask3, ref BitMask4);
        }
    }

    public BlockTagArray(ushort tag)
    {
        WriteTagToBitMasks(tag, ref BitMask1, ref BitMask2, ref BitMask3, ref BitMask4);
    }

    public BlockTagArray()
    {

    }

    public BlockTagArray(ulong bitMask1, ulong bitMask2, ulong bitMask3, ulong bitMask4)
    {
        BitMask1 = bitMask1;
        BitMask2 = bitMask2;
        BitMask3 = bitMask3;
        BitMask4 = bitMask4;
    }

    public static readonly BlockTagArray Empty = new();

    /// <summary>
    /// Converts tag array into list of tag ids sorted in ascending order
    /// </summary>
    /// <returns>List of tag ids in ascending order</returns>
    public IEnumerable<ushort> ToArray()
    {
        for (ushort index = 0; index < 64; index++)
        {
            if ((BitMask1 & (1UL << index)) != 0)
            {
                yield return (ushort)(index + 1);
            }
        }
        for (ushort index = 0; index < 64; index++)
        {
            if ((BitMask2 & (1UL << index)) != 0)
            {
                yield return (ushort)(index + 64 + 1);
            }
        }
        for (ushort index = 0; index < 64; index++)
        {
            if ((BitMask3 & (1UL << index)) != 0)
            {
                yield return (ushort)(index + 128 + 1);
            }
        }
        for (ushort index = 0; index < 64; index++)
        {
            if ((BitMask4 & (1UL << index)) != 0)
            {
                yield return (ushort)(index + 192 + 1);
            }
        }
    }

    /// <summary>
    /// Converts tag array into list of tags.
    /// </summary>
    /// <param name="api"></param>
    /// <returns></returns>
    public IEnumerable<string> ToArray(ICoreAPI api)
    {
        return ToArray().Select(api.TagRegistry.BlockTagIdToTag);
    }

    /// <summary>
    /// Checks if this tag array contains all tags from <paramref name="other"/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool ContainsAll(BlockTagArray other)
    {
        return (BitMask1 & other.BitMask1) == other.BitMask1 &&
               (BitMask2 & other.BitMask2) == other.BitMask2 &&
               (BitMask3 & other.BitMask3) == other.BitMask3 &&
               (BitMask4 & other.BitMask4) == other.BitMask4;
    }

    /// <summary>
    /// Checks if this tag array contains at least on tag from each element of <paramref name="tags"/>
    /// </summary>
    /// <param name="tags"></param>
    /// <returns></returns>
    public bool IntersectsWithEach(BlockTagArray[] tags)
    {
        foreach (BlockTagArray tagArray in tags)
        {
            if (!Intersect(this, tagArray)) return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if this tag array contains all tags from at least one element of <paramref name="tags"/>
    /// </summary>
    /// <param name="tags"></param>
    /// <returns></returns>
    public bool ContainsAllFromAtLeastOne(BlockTagArray[] tags)
    {
        foreach (BlockTagArray tagArray in tags)
        {
            if (ContainsAll(tagArray)) return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if two tag arrays have at least one common tag
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    public static bool Intersect(BlockTagArray first, BlockTagArray second)
    {
        ulong intersect1 = first.BitMask1 & second.BitMask1;
        ulong intersect2 = first.BitMask2 & second.BitMask2;
        ulong intersect3 = first.BitMask3 & second.BitMask3;
        ulong intersect4 = first.BitMask4 & second.BitMask4;

        return (intersect1 | intersect2 | intersect3 | intersect4) != 0;
    }

    /// <summary>
    /// Checks if this tag array has at least one common tag with <paramref name="other"/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Intersect(BlockTagArray other)
    {
        return Intersect(this, other);
    }

    public static BlockTagArray And(BlockTagArray first, BlockTagArray second)
    {
        return new BlockTagArray
        (
            first.BitMask1 & second.BitMask1,
            first.BitMask2 & second.BitMask2,
            first.BitMask3 & second.BitMask3,
            first.BitMask4 & second.BitMask4
        );
    }

    public static BlockTagArray Or(BlockTagArray first, BlockTagArray second)
    {
        return new BlockTagArray
        (
            first.BitMask1 | second.BitMask1,
            first.BitMask2 | second.BitMask2,
            first.BitMask3 | second.BitMask3,
            first.BitMask4 | second.BitMask4
        );
    }

    public static BlockTagArray Not(BlockTagArray value)
    {
        return new BlockTagArray
        (
            ~value.BitMask1,
            ~value.BitMask2,
            ~value.BitMask3,
            ~value.BitMask4
        );
    }

    public static BlockTagArray operator &(BlockTagArray first, BlockTagArray second)
    {
        return And(first, second);
    }

    public static BlockTagArray operator |(BlockTagArray first, BlockTagArray second)
    {
        return Or(first, second);
    }

    public static BlockTagArray operator ~(BlockTagArray value)
    {
        return Not(value);
    }

    public static bool operator ==(BlockTagArray first, BlockTagArray second)
    {
        return first.BitMask1 == second.BitMask1 &&
               first.BitMask2 == second.BitMask2 &&
               first.BitMask3 == second.BitMask3 &&
               first.BitMask4 == second.BitMask4;
    }
    public static bool operator !=(BlockTagArray first, BlockTagArray second)
    {
        return !(first == second);
    }

    public override bool Equals(object? obj)
    {
        if (obj is BlockTagArray other)
        {
            return this == other;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return (int)(BitMask1 ^ BitMask2 ^ BitMask3 ^ BitMask4);
    }

    public override string ToString() => PrintBitMask(BitMask4) + ":" + PrintBitMask(BitMask3) + ":" + PrintBitMask(BitMask2) + ":" + PrintBitMask(BitMask1);

    public void ToBytes(BinaryWriter writer)
    {
        writer.Write(MasksNumber);
        writer.Write(BitMask1);
        writer.Write(BitMask2);
        writer.Write(BitMask3);
        writer.Write(BitMask4);
    }

    public static BlockTagArray FromBytes(BinaryReader reader)
    {
        int size = reader.ReadInt32();
        if (size != MasksNumber)
        {
            throw new ArgumentException($"Trying to read 'BlockTagArray' from BinaryReader, but size of the array in reader ({size * 64}) is not equal current size of all block tag arrays ({MasksNumber * 64}).");
        }

        ulong bitMask1 = reader.ReadUInt64();
        ulong bitMask2 = reader.ReadUInt64();
        ulong bitMask3 = reader.ReadUInt64();
        ulong bitMask4 = reader.ReadUInt64();
        return new(bitMask1, bitMask2, bitMask3, bitMask4);
    }

    private static void WriteTagToBitMasks(ushort tag, ref ulong bitMask1, ref ulong bitMask2, ref ulong bitMask3, ref ulong bitMask4)
    {
        if (tag == 0) return;

        int tagIndex = (tag - 1) % 64;
        int bitMaskIndex = (tag - 1) / 64;

        switch (bitMaskIndex)
        {
            case 0:
                bitMask1 |= 1UL << tagIndex;
                break;
            case 1:
                bitMask2 |= 1UL << tagIndex;
                break;
            case 2:
                bitMask3 |= 1UL << tagIndex;
                break;
            case 3:
                bitMask4 |= 1UL << tagIndex;
                break;
            default:
                break;
        }
    }

    private static string PrintBitMask(ulong bitMask) => $"{bitMask:X16}".Chunk(4).Select(chunk => new string(chunk)).Aggregate((first, second) => $"{first}.{second}");

    public bool isPresentIn(ref BlockTagArray other)
    {
        if ((BitMask1 & other.BitMask1) != BitMask1) return false;
        if ((BitMask2 & other.BitMask2) != BitMask2) return false;
        if ((BitMask3 & other.BitMask3) != BitMask3) return false;
        if ((BitMask4 & other.BitMask4) != BitMask4) return false;
        return true;
    }
}

/// <summary>
/// Pair of tag arrays that is used for implementation of tag inversion
/// </summary>
public readonly struct BlockTagRule
{
    public readonly BlockTagArray TagsThatShouldBePresent;
    public readonly BlockTagArray TagsThatShouldBeAbsent;

    public const string NotPrefix = "not-";
    public readonly static BlockTagRule Empty = new(BlockTagArray.Empty, BlockTagArray.Empty);

    public BlockTagRule(BlockTagArray tagsThatShouldBePresent, BlockTagArray tagsThatShouldBeAbsent)
    {
        TagsThatShouldBePresent = tagsThatShouldBePresent;
        TagsThatShouldBeAbsent = tagsThatShouldBeAbsent;
    }

    public BlockTagRule(ICoreAPI api, IEnumerable<string> tags)
    {
        List<string> straightTags = [];
        List<string> inverseTags = [];
        foreach (string tag in tags)
        {
            if (tag.StartsWith(NotPrefix))
            {
                inverseTags.Add(tag[NotPrefix.Length..]);
            }
            else
            {
                straightTags.Add(tag);
            }
        }

        TagsThatShouldBePresent = api.TagRegistry.BlockTagsToTagArray([.. straightTags]);
        TagsThatShouldBeAbsent = api.TagRegistry.BlockTagsToTagArray([.. inverseTags]);
    }

    public bool Intersects(BlockTagArray tags)
    {
        if (TagsThatShouldBePresent != BlockTagArray.Empty && !tags.Intersect(TagsThatShouldBePresent)) return false;
        if (TagsThatShouldBeAbsent != BlockTagArray.Empty && tags.ContainsAll(TagsThatShouldBeAbsent)) return false;

        return true;
    }

    /// <summary>
    /// Checks if <paramref name="blockTag"/> contains at least on tag from each rule from <paramref name="rules"/>.
    /// </summary>
    /// <param name="blockTag"></param>
    /// /// <param name="rules"></param>
    /// <returns></returns>
    public static bool IntersectsWithEach(BlockTagArray blockTag, BlockTagRule[] rules)
    {
        foreach (BlockTagRule rule in rules)
        {
            if (rule.TagsThatShouldBePresent != BlockTagArray.Empty && !blockTag.Intersect(rule.TagsThatShouldBePresent) ||
                rule.TagsThatShouldBeAbsent != BlockTagArray.Empty && blockTag.ContainsAll(rule.TagsThatShouldBeAbsent)) return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if <paramref name="blockTag"/> contains all tags from at least one rule from <paramref name="rules"/>.
    /// </summary>
    /// <param name="blockTag"></param>
    /// /// <param name="rules"></param>
    /// <returns></returns>
    public static bool ContainsAllFromAtLeastOne(BlockTagArray blockTag, BlockTagRule[] rules)
    {
        foreach (BlockTagRule rule in rules)
        {
            if (blockTag.ContainsAll(rule.TagsThatShouldBePresent) && !(rule.TagsThatShouldBeAbsent != BlockTagArray.Empty && blockTag.Intersect(rule.TagsThatShouldBeAbsent))) return true;
        }
        return false;
    }

    public static bool operator ==(BlockTagRule first, BlockTagRule second)
    {
        return first.TagsThatShouldBePresent == second.TagsThatShouldBePresent &&
               first.TagsThatShouldBeAbsent == second.TagsThatShouldBeAbsent;
    }
    public static bool operator !=(BlockTagRule first, BlockTagRule second)
    {
        return !(first == second);
    }
    public override bool Equals(object? obj)
    {
        if (obj is BlockTagRule other)
        {
            return this == other;
        }
        return false;
    }
    public override int GetHashCode()
    {
        return TagsThatShouldBePresent.GetHashCode() ^ TagsThatShouldBeAbsent.GetHashCode();
    }
    public override string ToString() => $"+{TagsThatShouldBePresent}\n-{TagsThatShouldBeAbsent}";
}



/// <summary>
/// List of item tags meant to be used for fast comparisons. Restricts number of registered item tags to 256.
/// </summary>
public readonly struct ItemTagArray
{
    public readonly ulong BitMask1;
    public readonly ulong BitMask2;
    public readonly ulong BitMask3;
    public readonly ulong BitMask4;

    public const byte MasksNumber = 4;

    /// <summary>
    /// Maximum amount of different tags supported by tag array. Limited by total amount of bits in bit masks.
    /// </summary>
    public const int Size = MasksNumber * 64;

    public ItemTagArray(IEnumerable<ushort> tags)
    {
        foreach (ushort tag in tags)
        {
            WriteTagToBitMasks(tag, ref BitMask1, ref BitMask2, ref BitMask3, ref BitMask4);
        }
    }

    public ItemTagArray(ushort tag)
    {
        WriteTagToBitMasks(tag, ref BitMask1, ref BitMask2, ref BitMask3, ref BitMask4);
    }

    public ItemTagArray()
    {

    }

    public ItemTagArray(ulong bitMask1, ulong bitMask2, ulong bitMask3, ulong bitMask4)
    {
        BitMask1 = bitMask1;
        BitMask2 = bitMask2;
        BitMask3 = bitMask3;
        BitMask4 = bitMask4;
    }

    public static readonly ItemTagArray Empty = new();

    /// <summary>
    /// Converts tag array into list of tag ids sorted in ascending order
    /// </summary>
    /// <returns>List of tag ids in ascending order</returns>
    public IEnumerable<ushort> ToArray()
    {
        for (ushort index = 0; index < 64; index++)
        {
            if ((BitMask1 & (1UL << index)) != 0)
            {
                yield return (ushort)(index + 1);
            }
        }
        for (ushort index = 0; index < 64; index++)
        {
            if ((BitMask2 & (1UL << index)) != 0)
            {
                yield return (ushort)(index + 64 + 1);
            }
        }
        for (ushort index = 0; index < 64; index++)
        {
            if ((BitMask3 & (1UL << index)) != 0)
            {
                yield return (ushort)(index + 128 + 1);
            }
        }
        for (ushort index = 0; index < 64; index++)
        {
            if ((BitMask4 & (1UL << index)) != 0)
            {
                yield return (ushort)(index + 192 + 1);
            }
        }
    }

    /// <summary>
    /// Converts tag array into list of tags.
    /// </summary>
    /// <param name="api"></param>
    /// <returns></returns>
    public IEnumerable<string> ToArray(ICoreAPI api)
    {
        return ToArray().Select(api.TagRegistry.ItemTagIdToTag);
    }

    /// <summary>
    /// Checks if this tag array contains all tags from <paramref name="other"/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool ContainsAll(ItemTagArray other)
    {
        return (BitMask1 & other.BitMask1) == other.BitMask1 &&
               (BitMask2 & other.BitMask2) == other.BitMask2 &&
               (BitMask3 & other.BitMask3) == other.BitMask3 &&
               (BitMask4 & other.BitMask4) == other.BitMask4;
    }

    /// <summary>
    /// Checks if this tag array contains at least on tag from each element of <paramref name="tags"/>
    /// </summary>
    /// <param name="tags"></param>
    /// <returns></returns>
    public bool IntersectsWithEach(ItemTagArray[] tags)
    {
        foreach (ItemTagArray tagArray in tags)
        {
            if (!Intersect(this, tagArray)) return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if this tag array contains all tags from at least one element of <paramref name="tags"/>
    /// </summary>
    /// <param name="tags"></param>
    /// <returns></returns>
    public bool ContainsAllFromAtLeastOne(ItemTagArray[] tags)
    {
        foreach (ItemTagArray tagArray in tags)
        {
            if (ContainsAll(tagArray)) return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if two tag arrays have at least one common tag
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    public static bool Intersect(ItemTagArray first, ItemTagArray second)
    {
        ulong intersect1 = first.BitMask1 & second.BitMask1;
        ulong intersect2 = first.BitMask2 & second.BitMask2;
        ulong intersect3 = first.BitMask3 & second.BitMask3;
        ulong intersect4 = first.BitMask4 & second.BitMask4;

        return (intersect1 | intersect2 | intersect3 | intersect4) != 0;
    }

    /// <summary>
    /// Checks if this tag array has at least one common tag with <paramref name="other"/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Intersect(ItemTagArray other)
    {
        return Intersect(this, other);
    }

    public static ItemTagArray And(ItemTagArray first, ItemTagArray second)
    {
        return new ItemTagArray
        (
            first.BitMask1 & second.BitMask1,
            first.BitMask2 & second.BitMask2,
            first.BitMask3 & second.BitMask3,
            first.BitMask4 & second.BitMask4
        );
    }

    public static ItemTagArray Or(ItemTagArray first, ItemTagArray second)
    {
        return new ItemTagArray
        (
            first.BitMask1 | second.BitMask1,
            first.BitMask2 | second.BitMask2,
            first.BitMask3 | second.BitMask3,
            first.BitMask4 | second.BitMask4
        );
    }

    public static ItemTagArray Not(ItemTagArray value)
    {
        return new ItemTagArray
        (
            ~value.BitMask1,
            ~value.BitMask2,
            ~value.BitMask3,
            ~value.BitMask4
        );
    }

    public static ItemTagArray operator &(ItemTagArray first, ItemTagArray second) => And(first, second);

    public static ItemTagArray operator |(ItemTagArray first, ItemTagArray second) => Or(first, second);

    public static ItemTagArray operator ~(ItemTagArray value) => Not(value);

    public static bool operator ==(ItemTagArray first, ItemTagArray second)
    {
        return first.BitMask1 == second.BitMask1 &&
               first.BitMask2 == second.BitMask2 &&
               first.BitMask3 == second.BitMask3 &&
               first.BitMask4 == second.BitMask4;
    }

    public static bool operator !=(ItemTagArray first, ItemTagArray second) => !(first == second);

    public override bool Equals(object? obj)
    {
        if (obj is ItemTagArray other)
        {
            return this == other;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return (int)(BitMask1 ^ BitMask2 ^ BitMask3 ^ BitMask4);
    }

    public override string ToString() => PrintBitMask(BitMask4) + ":" + PrintBitMask(BitMask3) + ":" + PrintBitMask(BitMask2) + ":" + PrintBitMask(BitMask1);

    public void ToBytes(BinaryWriter writer)
    {
        writer.Write(MasksNumber);
        writer.Write(BitMask1);
        writer.Write(BitMask2);
        writer.Write(BitMask3);
        writer.Write(BitMask4);
    }

    public static ItemTagArray FromBytes(BinaryReader reader)
    {
        int size = reader.ReadInt32();
        if (size != MasksNumber)
        {
            throw new ArgumentException($"Trying to read 'ItemTagArray' from BinaryReader, but size of the array in reader ({size * 64}) is not equal current size of all item tag arrays ({MasksNumber * 64}).");
        }

        ulong bitMask1 = reader.ReadUInt64();
        ulong bitMask2 = reader.ReadUInt64();
        ulong bitMask3 = reader.ReadUInt64();
        ulong bitMask4 = reader.ReadUInt64();
        return new(bitMask1, bitMask2, bitMask3, bitMask4);
    }

    private static void WriteTagToBitMasks(ushort tag, ref ulong bitMask1, ref ulong bitMask2, ref ulong bitMask3, ref ulong bitMask4)
    {
        if (tag == 0) return;

        int tagIndex = (tag - 1) % 64;
        int bitMaskIndex = (tag - 1) / 64;

        switch (bitMaskIndex)
        {
            case 0:
                bitMask1 |= 1UL << tagIndex;
                break;
            case 1:
                bitMask2 |= 1UL << tagIndex;
                break;
            case 2:
                bitMask3 |= 1UL << tagIndex;
                break;
            case 3:
                bitMask4 |= 1UL << tagIndex;
                break;
            default:
                break;
        }
    }

    private static string PrintBitMask(ulong bitMask) => $"{bitMask:X16}".Chunk(4).Select(chunk => new string(chunk)).Aggregate((first, second) => $"{first}.{second}");

    public bool isPresentIn(ref ItemTagArray other)
    {
        if ((BitMask1 & other.BitMask1) != BitMask1) return false;
        if ((BitMask2 & other.BitMask2) != BitMask2) return false;
        if ((BitMask3 & other.BitMask3) != BitMask3) return false;
        if ((BitMask4 & other.BitMask4) != BitMask4) return false;
        return true;
    }
}

/// <summary>
/// Pair of tag arrays that is used for implementation of tag inversion
/// </summary>
public readonly struct ItemTagRule
{
    public readonly ItemTagArray TagsThatShouldBePresent;
    public readonly ItemTagArray TagsThatShouldBeAbsent;

    public const string NotPrefix = "not-";
    public readonly static ItemTagRule Empty = new(ItemTagArray.Empty, ItemTagArray.Empty);

    public ItemTagRule(ItemTagArray tagsThatShouldBePresent, ItemTagArray tagsThatShouldBeAbsent)
    {
        TagsThatShouldBePresent = tagsThatShouldBePresent;
        TagsThatShouldBeAbsent = tagsThatShouldBeAbsent;
    }

    public ItemTagRule(ICoreAPI api, IEnumerable<string> tags)
    {
        List<string> straightTags = [];
        List<string> inverseTags = [];
        foreach (string tag in tags)
        {
            if (tag.StartsWith(NotPrefix))
            {
                inverseTags.Add(tag[NotPrefix.Length..]);
            }
            else
            {
                straightTags.Add(tag);
            }
        }

        TagsThatShouldBePresent = api.TagRegistry.ItemTagsToTagArray([.. straightTags]);
        TagsThatShouldBeAbsent = api.TagRegistry.ItemTagsToTagArray([.. inverseTags]);
    }

    public bool Intersects(ItemTagArray tags)
    {
        if (TagsThatShouldBePresent != ItemTagArray.Empty && !tags.Intersect(TagsThatShouldBePresent)) return false;
        if (TagsThatShouldBeAbsent != ItemTagArray.Empty && tags.ContainsAll(TagsThatShouldBeAbsent)) return false;

        return true;
    }

    /// <summary>
    /// Checks if <paramref name="itemTag"/> contains at least on tag from each rule from <paramref name="rules"/>.
    /// </summary>
    /// <param name="itemTag"></param>
    /// /// <param name="rules"></param>
    /// <returns></returns>
    public static bool IntersectsWithEach(ItemTagArray itemTag, ItemTagRule[] rules)
    {
        foreach (ItemTagRule rule in rules)
        {
            if (rule.TagsThatShouldBePresent != ItemTagArray.Empty && !itemTag.Intersect(rule.TagsThatShouldBePresent) ||
                rule.TagsThatShouldBeAbsent != ItemTagArray.Empty && itemTag.ContainsAll(rule.TagsThatShouldBeAbsent)) return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if <paramref name="itemTag"/> contains all tags from at least one rule from <paramref name="rules"/>.
    /// </summary>
    /// <param name="itemTag"></param>
    /// /// <param name="rules"></param>
    /// <returns></returns>
    public static bool ContainsAllFromAtLeastOne(ItemTagArray itemTag, ItemTagRule[] rules)
    {
        foreach (ItemTagRule rule in rules)
        {
            if (itemTag.ContainsAll(rule.TagsThatShouldBePresent) && !(rule.TagsThatShouldBeAbsent != ItemTagArray.Empty && itemTag.Intersect(rule.TagsThatShouldBeAbsent))) return true;
        }
        return false;
    }

    public static bool operator ==(ItemTagRule first, ItemTagRule second)
    {
        return first.TagsThatShouldBePresent == second.TagsThatShouldBePresent &&
               first.TagsThatShouldBeAbsent == second.TagsThatShouldBeAbsent;
    }
    public static bool operator !=(ItemTagRule first, ItemTagRule second)
    {
        return !(first == second);
    }
    public override bool Equals(object? obj)
    {
        if (obj is ItemTagRule other)
        {
            return this == other;
        }
        return false;
    }
    public override int GetHashCode()
    {
        return TagsThatShouldBePresent.GetHashCode() ^ TagsThatShouldBeAbsent.GetHashCode();
    }
    public override string ToString() => $"+{TagsThatShouldBePresent}\n-{TagsThatShouldBeAbsent}";
}
