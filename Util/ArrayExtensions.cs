using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Util
{
    public delegate T fillCallback<T>(int index);

    public static class ArrayUtil
    {

        public static T[] CreateFilled<T>(int quantity, fillCallback<T> fillCallback)
        {
            T[] array = new T[quantity];
            for (int i = 0; i < quantity; i++)
            {
                array[i] = fillCallback(i);
            }

            return array;
        }

        public static T[] CreateFilled<T>(int quantity, T with)
        {
            T[] array = new T[quantity];
            for (int i = 0; i < quantity; i++)
            {
                array[i] = with;
            }

            return array;
        }


    }


    public static class ArrayExtensions
    {
        public static int IndexOf<T>(this T[] array, Func<T, bool> predicate)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (predicate(array[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public static int IndexOf<T>(this T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (object.Equals(value, array[i]))
                {
                    return i;
                }
            }

            return -1;
        }


        public static bool Contains<T>(this T[] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (Object.Equals(array[i], value))
                {
                    return true;
                }
            }

            return false;
        }

        public static T[] Remove<T>(this T[] array, T value)
        {
            List<T> elems = new List<T>(array);
            elems.Remove(value);

            return elems.ToArray();
        }

        public static T[] RemoveEntry<T>(this T[] array, int index)
        {
            T[] cut = new T[array.Length - 1];

            if (index == 0)
            {
                if (cut.Length == 0) return cut; // Below line seems to crash otherwise

                Array.Copy(array, 1, cut, 0, array.Length - 1);
            } else if (index == array.Length - 1)
            {
                Array.Copy(array, cut, array.Length - 1);
            } else
            {
                Array.Copy(array, 0, cut, 0, index);
                Array.Copy(array, index + 1, cut, index, array.Length - index - 1);
            }
            

            return cut;
        }

        public static T[] Append<T>(this T[] array, T value)
        {
            T[] grown = new T[array.Length + 1];
            Array.Copy(array, grown, array.Length);

            grown[array.Length] = value;

            return grown;
        }


        public static T[] Append<T>(this T[] array, T[] value)
        {
            if (array == null) return null;
            if (value == null) return array;

            T[] grown = new T[array.Length + value.Length];
            Array.Copy(array, grown, array.Length);

            for (int i = 0; i < value.Length; i++)
            {
                grown[array.Length + i] = value[i];
            }

            return grown;
        }

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
        public static T[] Shuffle<T>(this T[] array, Random rand)
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

            return array;
        }


        /// <summary>
        /// Performs a Fisher-Yates shuffle in linear time or O(n)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rand"></param>
        /// <param name="array"></param>
        public static T[] Shuffle<T>(this T[] array, LCGRandom rand)
        {
            int n = array.Length;        // The number of items left to shuffle (loop invariant).
            while (n > 1)
            {
                int k = rand.NextInt(n);  // 0 <= k < n.
                n--;                   // n is now the last pertinent index;
                T temp = array[n];     // swap array[n] with array[k] (does nothing if k == n).
                array[n] = array[k];
                array[k] = temp;
            }

            return array;
        }
    }
}
