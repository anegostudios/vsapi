using System;
using System.Collections;
using System.Collections.Generic;

#nullable disable

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
                if (!dict.TryGetValue(key, out K value))
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
}
