using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Vintagestory.API.MathTools;

#nullable disable

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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] FastCopy<T>(this T[] src, int count)
        {
            T[] dest = new T[count];
            if (count > 127) Array.Copy(src, 0, dest, 0, count);
            else
            {
                for (int i = 0; i < dest.Length; i++)
                {
                    dest[i] = src[i];
                }
            }

            return dest;
        }


        public static T[] Slice<T>(this T[] src, int index, int count)
        {
            T[] dest = new T[count];

            for (int i = 0; i < count; i++)
            {
                dest[i] = src[index + i];
            }

            return dest;
        }
    }

    public static class EnumerableExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> source, Common.ActionBoolReturn<T> onelem)
        {
            int i = 0;
            foreach (var elem in source)
            {
                if (onelem(elem)) return i;
                i++;
            }

            return -1;
        }

        public static void Foreach<T>(this IEnumerable<T> array, Action<T> onelement)
        {
            foreach (var val in array)
            {
                onelement(val);
            }
        }

        public static T Nearest<T>(this IEnumerable<T> array, Func<T, double> getDistance)
        {
            double nearestDist = double.MaxValue;
            T nearest = default(T);
            foreach (var val in array)
            {
                double d = getDistance(val);

                if (d < nearestDist)
                {
                    nearestDist = d;
                    nearest = val;
                }
            }

            return nearest;
        }


        public static double NearestDistance<T>(this IEnumerable<T> array, Func<T, double> getDistance)
        {
            double nearestDist = double.MaxValue;
            foreach (var val in array)
            {
                double d = getDistance(val);

                if (d < nearestDist)
                {
                    nearestDist = d;
                }
            }

            return nearestDist;
        }
    }


    public static class ArrayExtensions
    {
        public static T Nearest<T>(this T[] array, Func<T, double> getDistance)
        {
            double nearestDist = double.MaxValue;
            T nearest = default(T);
            for (int i = 0; i < array.Length; i++)
            {
                double d = getDistance(array[i]);

                if (d < nearestDist)
                {
                    nearestDist = d;
                    nearest = array[i];
                }
            }

            return nearest;
        }

        public static List<T> InRange<T>(this T[] array, Func<T, double> getDistance, double range)
        {
            List<T> found = new List<T>();

            for (int i = 0; i < array.Length; i++)
            {
                double dist = getDistance(array[i]);
                if (dist < range)
                {
                    found.Add(array[i]);
                }
            }

            return found;
        }

        public static bool Contains<T>(this T[] array, Func<T, bool> predicate) => IndexOf(array, predicate) >= 0;

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

        /// <summary>
        /// Creates a new copy of the array with <paramref name="value"/> removed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T[] Remove<T>(this T[] array, T value)
        {
            List<T> elems = new List<T>(array);
            elems.Remove(value);

            return elems.ToArray();
        }

        [Obsolete("Use RemoveAt instead")]
        public static T[] RemoveEntry<T>(this T[] array, int index)
        {
            return RemoveAt(array, index);
        }

        /// <summary>
        /// Creates a new copy of array with element at index removed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T[] RemoveAt<T>(this T[] array, int index)
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

        /// <summary>
        /// Creates a new copy of the array with <paramref name="value"/> appened to the end of the array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T[] Append<T>(this T[] array, T value)
        {
            T[] grown = new T[array.Length + 1];
            Array.Copy(array, grown, array.Length);

            grown[array.Length] = value;

            return grown;
        }

        /// <summary>
        /// Creates a new copy of the array with <paramref name="value"/> inserted at the given index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T[] InsertAt<T>(this T[] array, T value, int index)
        {
            T[] grown = new T[array.Length + 1];
            if (index > 0) Array.Copy(array, grown, index);

            grown[index] = value;
            Array.Copy(array, index, grown, index + 1, array.Length - index);

            return grown;
        }

        /// <summary>
        /// Creates a new copy of the array with <paramref name="value"/> appended to the end of the array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T[] Append<T>(this T[] array, params T[] value)
        {
            if (array == null) return null;
            if (value == null || value.Length == 0) return array;

            T[] grown = new T[array.Length + value.Length];
            Array.Copy(array, grown, array.Length);

            for (int i = 0; i < value.Length; i++)
            {
                grown[array.Length + i] = value[i];
            }

            return grown;
        }

        /// <summary>
        /// Creates a new copy of the array with <paramref name="values"/> appended to the end of the array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static T[] Append<T>(this T[] array, IEnumerable<T> values)
        {
            if (array == null) return null;
            if (values == null) return array;

            T[] grown = new T[array.Length + values.Count()];
            Array.Copy(array, grown, array.Length);

            int i = 0;
            foreach (var val in values) 
            {
                grown[array.Length + i] = val;
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
