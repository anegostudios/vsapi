using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Vintagestory.API.Util
{

    public static class DictExtensions
    {
        /// <summary>
        /// Get value or defaultValue if key does not exists
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static V Get<K, V>(this IDictionary<K, V> dict, K key, V defaultValue = default(V)) {
            if (dict.TryGetValue(key, out var val))
            {
                return val;
            }

            return defaultValue;
        }

        public static void Remove<K, V>(this ConcurrentDictionary<K,V> dict, K key)
        {
            dict.TryRemove(key, out _);
        }

        public static void RemoveAll<K, V>(this IDictionary<K, V> dict, Func<K, V, bool> predicate)
        {
            foreach (var key in dict.Keys.ToArray().Where(key => predicate(key, dict[key])))
            {
                dict.Remove(key);
            }
        }

        public static void RemoveAllByKey<K, V>(this IDictionary<K, V> dict, Func<K, bool> predicate)
        {
            foreach (var key in dict.Keys.ToArray().Where(key => predicate(key)))
            {
                dict.Remove(key);
            }
        }
    }
}
