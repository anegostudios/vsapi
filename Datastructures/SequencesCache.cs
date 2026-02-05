using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

namespace Vintagestory.API.Datastructures;

/// <summary>
/// Thread-safe cache of sequences (ordered collections).
/// If two elements return 0 from 'CompareTo' call, they are treated as equal.
/// </summary>
/// <typeparam name="TElement">Must be IComparable</typeparam>
public sealed class SequencesCache<TElement>
    where TElement : IComparable<TElement>
{
    public ImmutableArray<TElement> Get(TElement[] sequence)
    {
        ImmutableArray<TElement> result = ImmutableArray<TElement>.Empty;

        if (sequence.Length == 0)
        {
            return result;
        }

        sequencesLock.EnterUpgradeableReadLock();
        try
        {
            int indexOfSequence = 0;
            for (int indexInSequence = 0; indexInSequence < sequence.Length; indexInSequence++)
            {
                TElement currentValue = sequence[indexInSequence];
                bool found = false;

                for (; indexOfSequence < sequences.Count; indexOfSequence++)
                {
                    ImmutableArray<TElement> testSequence = sequences[indexOfSequence];

                    if (testSequence.Length != sequence.Length)
                    {
                        continue;
                    }

                    TElement testValue = testSequence[indexInSequence];

                    int comparisonResult = testValue.CompareTo(currentValue);

                    if (comparisonResult == 0)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    ImmutableArray<TElement> newSequence = sequence.ToImmutableArray();

                    sequencesLock.EnterWriteLock();
                    try
                    {
                        Insert(newSequence);
                    }
                    finally
                    {
                        sequencesLock.ExitWriteLock();
                    }

                    return newSequence;
                }
            }

            result = sequences[indexOfSequence];
        }
        finally
        {
            sequencesLock.ExitUpgradeableReadLock();
        }

        return result;
    }

    public void Clear()
    {
        sequencesLock.EnterWriteLock();
        sequences.Clear();
        sequencesLock.ExitWriteLock();
    }

    private readonly List<ImmutableArray<TElement>> sequences = [];
    private readonly ReaderWriterLockSlim sequencesLock = new();

    private void Insert(ImmutableArray<TElement> sequence)
    {
        int insertIndex = FindInsertIndex(sequence);

        sequences.Insert(insertIndex, sequence);
    }
    private int FindInsertIndex(ImmutableArray<TElement> row)
    {
        int columnIndex = 0;

        for (int rowIndex = 0; rowIndex < row.Length; rowIndex++)
        {
            TElement currentValue = row[rowIndex];

            for (; columnIndex < sequences.Count; columnIndex++)
            {
                ImmutableArray<TElement> currentRow = sequences[columnIndex];

                if (currentRow.Length >= rowIndex)
                {
                    continue;
                }

                TElement currentRowValue = sequences[columnIndex][rowIndex];

                int comparisonResult = currentRowValue.CompareTo(currentValue);

                if (comparisonResult < 0)
                {
                    return columnIndex;
                }
                else if (comparisonResult == 0)
                {
                    break;
                }
            }
        }

        return columnIndex;
    }

#if DEBUG
    internal List<ImmutableArray<TElement>> GetSequences() => sequences;
#endif
}
