using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Util
{
    public static class ListExtensions
    {
        /// <summary>
        /// Performs a Fisher-Yates shuffle in linear time or O(n)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rand"></param>
        /// <param name="array"></param>
        public static void Shuffle<T>(this List<T> array, Random rand)
        {
            int n = array.Count;        // The number of items left to shuffle (loop invariant).
            while (n > 1)
            {
                int k = rand.Next(n);  // 0 <= k < n.
                n--;                   // n is now the last pertinent index;
                T temp = array[n];     // swap array[n] with array[k] (does nothing if k == n).
                array[n] = array[k];
                array[k] = temp;
            }
        }

        /// <summary>
        /// Performs a Fisher-Yates shuffle in linear time or O(n)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rand"></param>
        /// <param name="array"></param>
        public static void Shuffle<T>(this List<T> array, IRandom rand)
        {
            int n = array.Count;        // The number of items left to shuffle (loop invariant).
            while (n > 1)
            {
                int k = rand.NextInt(n);  // 0 <= k < n.
                n--;                   // n is now the last pertinent index;
                T temp = array[n];     // swap array[n] with array[k] (does nothing if k == n).
                array[n] = array[k];
                array[k] = temp;
            }
        }

        public static T PopLast<T>(this List<T> items)
        {
            var item = items[items.Count - 1];
            items.RemoveAt(items.Count - 1);
            return item;
        }

        public static bool Contains<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) {
            foreach (var item in source)
            {
                if (predicate(item)) return true;
            }
            return false;
        }
    }

}
