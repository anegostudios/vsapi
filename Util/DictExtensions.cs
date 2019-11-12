using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Util
{
    public class RelaxedReadOnlyDictionary<T, K> : IDictionary<T, K>
    {
        IDictionary<T, K> dict;

        public RelaxedReadOnlyDictionary(IDictionary<T, K> values)
        {
            this.dict = values;
        }

        public K this[T key]
        {
            get
            {
                K value;
                if (!dict.TryGetValue(key, out value))
                {
                    value = default(K);
                }
                return value;
            }
            set
            {
                throw new InvalidOperationException("May not be modified");
            }
        }

        public ICollection<T> Keys => dict.Keys;
        public ICollection<K> Values => dict.Values;
        public int Count => dict.Count;

        public bool IsReadOnly => true;

        public void Add(T key, K value)
        {
            throw new InvalidOperationException("May not be modified");
        }

        public void Add(KeyValuePair<T, K> item)
        {
            throw new InvalidOperationException("May not be modified");
        }

        public void Clear()
        {
            throw new InvalidOperationException("May not be modified");
        }

        public bool Contains(KeyValuePair<T, K> item)
        {
            return dict.Contains(item);
        }

        public bool ContainsKey(T key)
        {
            return dict.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<T, K>[] array, int arrayIndex)
        {
            dict.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<T, K>> GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        public bool Remove(T key)
        {
            throw new InvalidOperationException("May not be modified");
        }

        public bool Remove(KeyValuePair<T, K> item)
        {
            throw new InvalidOperationException("May not be modified");
        }

        public bool TryGetValue(T key, out K value)
        {
            return dict.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dict.GetEnumerator();
        }
    }

    public static class DictExtensions
    {
        public static void Remove<K, V>(this ConcurrentDictionary<K,V> dict, K key)
        {
            V val; // So useless.
            dict.TryRemove(key, out val);
        }

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
