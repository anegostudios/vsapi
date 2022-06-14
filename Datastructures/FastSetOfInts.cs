using System;
using System.Collections;
using System.Collections.Generic;

namespace Vintagestory.API.Datastructures
{
    public class FastSetOfInts : IEnumerable<int>
    {
        int arraySize = 27;
        int size = 0;
        int[] set;

        public int Count { get { return size; } }

        public void Clear()
        {
            size = 0;
        }

        public FastSetOfInts()
        {
            set = new int[arraySize];
        }

        /// <summary>
        /// Add four separate components, assumed to be signed int in the range -128 to +127
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public bool Add(int a, int b, int c, int d)
        {
            return Add(a + 128 + (b + 128 << 8) + (c + 128 << 16) + (d << 24));
        }

        /// <summary>
        /// Return false if the set already contained this value; return true if the Add was successful
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Add(int value)
        {
            // fast search, start from the most recently added - this should be faster than any HashSet up to several hundred elements in size
            int i = size;
            while (--i >= 0)
            {
                if (set[i] == value) return false;
            }

            // now actually add the value
            if (size + 1 >= arraySize) expandArray();
            set[size++] = value;
            return true;

            //TODO: we could make it a sorted array and boolean search - maybe worthwhile for larger sets?
        }

        public void RemoveIfMatches(int a, int b, int c, int d)
        {
            int i = size;
            int match = a + 128 + (b + 128 << 8) + (c + 128 << 16) + (d << 24);
            while (--i >= 0)
            {
                if (set[i] == match)
                {
                    RemoveAt(i);
                    return;
                }
            }
        }

        private void RemoveAt(int i)
        {
            for (int j = i + 1; j < size; j++) set[j - 1] = set[j];
            size--;
        }

        private void expandArray()
        {
            int newSize = arraySize * 3 / 2 + 1;
            int[] newArray = new int[newSize];
            for (int i = 0; i < size; i++) newArray[i] = set[i];
            set = newArray;
            arraySize = newSize;
        }


        public IEnumerator<int> GetEnumerator()
        {
            for (int i = 0; i < size; i++)
                yield return set[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
