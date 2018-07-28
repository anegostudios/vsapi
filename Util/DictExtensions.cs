using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Util
{
    public static class DictExtensions
    {
        public static void RemoveAll<K, V>(this IDictionary<K, V> dict, Func<K, V, bool> predicate)
        {
            foreach (var key in dict.Keys.ToArray().Where(key => predicate(key, dict[key])))
                dict.Remove(key);
        }

        public static void RemoveAllByKey<K, V>(this IDictionary<K, V> dict, Func<K, bool> predicate)
        {
            foreach (var key in dict.Keys.ToArray().Where(key => predicate(key)))
                dict.Remove(key);
        }
    }
}
