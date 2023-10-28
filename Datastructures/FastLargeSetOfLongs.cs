using System.Collections;
using System.Collections.Generic;

namespace Vintagestory.API.Datastructures
{
    public class FastLargeSetOfLongs : IEnumerable<long>
    {
        int count = 0;
        readonly long[][] buckets;
        readonly int[] sizes;
        readonly int mask;

        public int Count { get { return count; } }

        public void Clear()
        {
            count = 0;
            for (int j = 0; j < sizes.Length; j++) sizes[j] = 0;
        }

        public FastLargeSetOfLongs(int numbuckets)
        {
            int allBits = numbuckets - 1;
            allBits |= allBits >> 1;  // This is a fast way to round up to the next power of 2.
            allBits |= allBits >> 2;  // We subtract 1 at the start to deal with the case where count is already a round number
            allBits |= allBits >> 4;
            allBits |= allBits >> 8;
            allBits |= allBits >> 16;
            numbuckets = allBits + 1;

            sizes = new int[numbuckets];
            buckets = new long[numbuckets][];
            for (int i = 0; i < buckets.Length; i++) buckets[i] = new long[7];
            mask = numbuckets - 1;
        }

        /// <summary>
        /// Return false if the set already contained this value; return true if the Add was successful
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Add(long value)
        {
            int j = (int)value & mask;
            long[] set = buckets[j];

            // fast search, start from the most recently added - this should be faster than any HashSet up to several hundred elements in size
            int siz = sizes[j];
            int i = siz;
            while (--i >= 0)
            {
                if (set[i] == value) return false;
            }

            // now actually add the value
            if (siz >= set.Length) set = expandArray(j);
            set[siz++] = value;
            sizes[j] = siz;
            count++;
            return true;

            //TODO: we could make it a sorted array and boolean search - maybe worthwhile for sets expected to have many duplicates?
        }

        private long[] expandArray(int j)
        {
            long[] set = buckets[j];
            int newSize = set.Length * 3 / 2 + 1;
            long[] newArray = new long[newSize];
            for (int i = 0; i < set.Length; i++) newArray[i] = set[i];
            buckets[j] = newArray;
            return newArray;
        }


        public IEnumerator<long> GetEnumerator()
        {
            for (int j = 0; j < buckets.Length; j++)
            {
                int size = sizes[j];
                long[] set = buckets[j];
                for (int i = 0; i < size; i++)
                    yield return set[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
