using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Util
{
    public static class ArrayExtensions
    {
        public delegate T fillCallback<T>(int index);

        public static T[] Fill<T>(this T[] originalArray, T with)
        {
            for (int i = 0; i < originalArray.Length; i++)
            {
                originalArray[i] = with;
            }

            return originalArray;
        }

        public static T[] Fill<T>(this T[] originalArray, fillCallback<T> fillCallback)
        {
            for (int i = 0; i < originalArray.Length; i++)
            {
                originalArray[i] = fillCallback(i);
            }

            return originalArray;
        }

        /// <summary>
        /// Performs a Fisher-Yates shuffle in linear time or O(n)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rand"></param>
        /// <param name="array"></param>
        public static void Shuffle<T>(this T[] array, Random rand)
        {
            int n = array.Length;        // The number of items left to shuffle (loop invariant).
            while (n > 1)
            {
                int k = rand.Next(n);  // 0 <= k < n.
                n--;                   // n is now the last pertinent index;
                T temp = array[n];     // swap array[n] with array[k] (does nothing if k == n).
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}
