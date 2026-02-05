using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Vintagestory.API.Datastructures;

[InlineArray(PartsNumber)]
public struct FixedSizeBitSet
{
    public const int PartsNumber = 4;
    public const int BitsPerByte = 8;
    public const int PartSize = sizeof(ulong) * BitsPerByte;
    public const int Size = PartsNumber * PartSize;

#pragma warning disable S1144 // It is used by [InlineArray(PartsNumber)]
    private ulong part;
#pragma warning restore S1144

    public static readonly FixedSizeBitSet Empty = new();



    public FixedSizeBitSet(IEnumerable<ushort> elements)
    {
        foreach (ushort element in elements)
        {
            Write(element);
        }
    }

    public FixedSizeBitSet(ushort element)
    {
        Write(element);
    }

    public FixedSizeBitSet()
    {

    }



    public readonly FixedSizeBitSet Union(FixedSizeBitSet array) => Or(this, array);

    public readonly FixedSizeBitSet Intersect(FixedSizeBitSet array) => And(this, array);

    public readonly FixedSizeBitSet Except(FixedSizeBitSet array) => Difference(this, array);

    public readonly FixedSizeBitSet SymmetricExcept(FixedSizeBitSet array) => Xor(this, array);

    public readonly bool IsSubsetOf(FixedSizeBitSet array)
    {
        ulong result = 0;
        for (int partIndex = 0; partIndex < PartsNumber; partIndex++)
        {
            result |= this[partIndex] & ~array[partIndex];
        }
        return result == 0;
    }

    public readonly bool IsSupersetOf(FixedSizeBitSet array)
    {
        ulong result = 0;
        for (int partIndex = 0; partIndex < PartsNumber; partIndex++)
        {
            result |= ~this[partIndex] & array[partIndex];
        }
        return result == 0;
    }

    public readonly bool Overlaps(FixedSizeBitSet array)
    {
        ulong result = 0;
        for (int partIndex = 0; partIndex < PartsNumber; partIndex++)
        {
            result |= this[partIndex] & array[partIndex];
        }
        return result != 0;
    }

    public readonly bool SetEquals(FixedSizeBitSet array) => this == array;

    public readonly bool OverlapsWithEach(IEnumerable<FixedSizeBitSet> arrays)
    {
        foreach (FixedSizeBitSet array in arrays)
        {
            if (!Overlaps(array))
            {
                return false;
            }
        }

        return true;
    }

    public readonly bool SupersetOfAtLeastOne(IEnumerable<FixedSizeBitSet> arrays)
    {
        foreach (FixedSizeBitSet array in arrays)
        {
            if (IsSupersetOf(array))
            {
                return true;
            }
        }

        return false;
    }

    public readonly bool Contains(ushort element)
    {
        return Read(element);
    }

    public static FixedSizeBitSet And(FixedSizeBitSet first, FixedSizeBitSet second)
    {
        FixedSizeBitSet result = new();
        for (int partIndex = 0; partIndex < PartsNumber; partIndex++)
        {
            result[partIndex] = first[partIndex] & second[partIndex];
        }
        return result;
    }

    public static FixedSizeBitSet Or(FixedSizeBitSet first, FixedSizeBitSet second)
    {
        FixedSizeBitSet result = new();
        for (int partIndex = 0; partIndex < PartsNumber; partIndex++)
        {
            result[partIndex] = first[partIndex] | second[partIndex];
        }
        return result;
    }

    public static FixedSizeBitSet Xor(FixedSizeBitSet first, FixedSizeBitSet second)
    {
        FixedSizeBitSet result = new();
        for (int partIndex = 0; partIndex < PartsNumber; partIndex++)
        {
            result[partIndex] = first[partIndex] ^ second[partIndex];
        }
        return result;
    }

    public static FixedSizeBitSet Difference(FixedSizeBitSet first, FixedSizeBitSet second)
    {
        FixedSizeBitSet result = new();
        for (int partIndex = 0; partIndex < PartsNumber; partIndex++)
        {
            result[partIndex] = first[partIndex] & ~second[partIndex];
        }
        return result;
    }

    public static FixedSizeBitSet Not(FixedSizeBitSet value)
    {
        FixedSizeBitSet result = new();
        for (int partIndex = 0; partIndex < PartsNumber; partIndex++)
        {
            result[partIndex] = ~value[partIndex];
        }
        return result;
    }

    public static FixedSizeBitSet operator &(FixedSizeBitSet first, FixedSizeBitSet second)
    {
        return And(first, second);
    }

    public static FixedSizeBitSet operator |(FixedSizeBitSet first, FixedSizeBitSet second)
    {
        return Or(first, second);
    }

    public static FixedSizeBitSet operator ^(FixedSizeBitSet first, FixedSizeBitSet second)
    {
        return Xor(first, second);
    }

    public static FixedSizeBitSet operator /(FixedSizeBitSet first, FixedSizeBitSet second)
    {
        return Difference(first, second);
    }

    public static FixedSizeBitSet operator ~(FixedSizeBitSet value)
    {
        return Not(value);
    }

    public static bool operator ==(FixedSizeBitSet first, FixedSizeBitSet second)
    {
        ulong result = 0;
        for (int partIndex = 0; partIndex < PartsNumber; partIndex++)
        {
            result |= first[partIndex] ^ second[partIndex];
        }
        return result == 0;
    }

    public static bool operator !=(FixedSizeBitSet first, FixedSizeBitSet second)
    {
        return !(first == second);
    }

    public readonly override bool Equals(object? obj)
    {
        if (obj is FixedSizeBitSet other)
        {
            return this == other;
        }
        return false;
    }

    public readonly override int GetHashCode()
    {
        ulong partsCombined = 0;
        for (int partIndex = 0; partIndex < PartsNumber; partIndex++)
        {
            partsCombined ^= this[partIndex];
        }

        int low = (int)(partsCombined & 0xFFFFFFFF);
        int high = (int)(partsCombined >> 32);

        return low ^ high;
    }

    public override readonly string ToString()
    {
        StringBuilder builder = new();
        builder.Append(PrintBitArray(this[PartsNumber - 1]));
        for (int partIndex = PartsNumber - 2; partIndex >= 0; partIndex--)
        {
            builder.Append(':');
            builder.Append(PrintBitArray(this[partIndex]));
        }
        return builder.ToString();
    }

    public readonly void ToBytes(BinaryWriter writer)
    {
        writer.Write(PartsNumber);
        for (int partIndex = 0; partIndex < PartsNumber; partIndex++)
        {
            writer.Write(this[partIndex]);
        }
    }

    public static FixedSizeBitSet FromBytes(BinaryReader reader)
    {
        int size = reader.ReadInt32();
        if (size != PartsNumber)
        {
            throw new ArgumentException($"Trying to read 'FixedBitArray' from BinaryReader, but size of the array in reader ({size * PartSize}) is not equal current size of the fixed array ({Size}).");
        }

        FixedSizeBitSet result = new();

        for (int partIndex = 0; partIndex < PartsNumber; partIndex++)
        {
            result[partIndex] = reader.ReadUInt64();
        }

        return result;
    }

    public static FixedSizeBitSet FromBytesNoException(BinaryReader reader)
    {
        int size = reader.ReadInt32();

        FixedSizeBitSet result = new();

        for (int partIndex = 0; partIndex < Math.Min(PartsNumber, size); partIndex++)
        {
            result[partIndex] = reader.ReadUInt64();
        }

        return result;
    }

    public readonly IEnumerable<ushort> ToArray()
    {
        for (int partIndex = 0; partIndex < PartsNumber; partIndex++)
        {
            for (int localIndex = 0; localIndex < PartSize; localIndex++)
            {
                if ((this[partIndex] & (1UL << localIndex)) != 0)
                {
                    yield return (ushort)localIndex;
                }
            }
        }
    }



    private void Write(ushort element)
    {
        int partIndex = element / PartSize;
        int localIndex = element % PartSize;

        this[partIndex] |= 1UL << localIndex;
    }

    private readonly bool Read(ushort element)
    {
        int partIndex = element / PartSize;
        int localIndex = element % PartSize;

        return (this[partIndex] & 1UL << localIndex) != 0;
    }

    private static string PrintBitArray(ulong bitArray) => $"{bitArray:X16}".Chunk(4).Select(chunk => new string(chunk)).Aggregate((first, second) => $"{first}.{second}");
}
