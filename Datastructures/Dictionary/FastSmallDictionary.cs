using System;
using System.Collections;
using System.Collections.Generic;

namespace Vintagestory.API.Datastructures
{
    /// <summary>
    /// A fast implementation of IDictionary using arrays.  Only suitable for small dictionaries, typically 1-20 entries.
    /// <br/>Note that Add is implemented differently from a standard Dictionary, it does not check that the key is not already present (and does not throw an ArgumentException)
    /// <br/>Additional methods AddIfNotPresent() and Clone() are provided for convenience.  There are also additional convenient constructors
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class FastSmallDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        TKey[] keys;
        TValue[] values;
        private int count;

        public ICollection<TKey> Keys { get { TKey[] result = new TKey[count]; Array.Copy(keys, result, count);  return result; } }

        public ICollection<TValue> Values { get { TValue[] result = new TValue[count]; Array.Copy(values, result, count); return result; } }

        int ICollection<KeyValuePair<TKey, TValue>>.Count => count;

        public bool IsReadOnly => false;

        public FastSmallDictionary(int size)
        {
            keys = new TKey[size];
            values = new TValue[size];
        }

        public FastSmallDictionary(TKey key, TValue value) : this(1)
        {
            keys[0] = key;
            values[0] = value;
            count = 1;
        }

        public FastSmallDictionary(Dictionary<TKey, TValue> dict) : this(dict.Count)
        {
            foreach (var val in dict) Add(val);
        }

        public FastSmallDictionary<TKey, TValue> Clone()
        {
            var result = new FastSmallDictionary<TKey, TValue>(count);
            result.keys = new TKey[count];
            result.values = new TValue[count];
            result.count = count;
            Array.Copy(keys, result.keys, count);
            Array.Copy(values, result.values, count);
            return result;
        }

        public TKey GetFirstKey()
        {
            return keys[0];
        }

        /// <summary>
        /// It is calling code's responsibility to ensure the key being searched for is not null
        /// </summary>
        public TValue this[TKey key]
        {
            get
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    if (i >= count) break;
                    if (key.Equals(keys[i])) return values[i];
                }
                throw new KeyNotFoundException("The key " + key.ToString() + " was not found");
            }
            set
            {
                for (int i = 0; i < count; i++)
                {
                    if (key.Equals(keys[i]))
                    {
                        values[i] = value;
                        return;
                    }
                }

                if (count == keys.Length) ExpandArrays();

                keys[count] = key;
                values[count++] = value;
            }
        }

        private void ExpandArrays()
        {
            int newSize = keys.Length + 3;
            TKey[] newKeys = new TKey[newSize];
            TValue[] newValues = new TValue[newSize];
            for (int i = 0; i < keys.Length; i++)
            {
                newKeys[i] = keys[i];
                newValues[i] = values[i];
            }
            values = newValues;
            keys = newKeys;
        }

        public bool ContainsKey(TKey key)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (i >= count) break;
                if (key.Equals(keys[i])) return true;
            }
            return false;
        }

        public void Add(TKey key, TValue value)
        {
            if (count == keys.Length) ExpandArrays();

            keys[count] = key;
            values[count++] = value;
        }

        /// <summary>
        /// It is the calling code's responsibility to make sure that key is not null
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public bool TryGetValue(TKey key, out TValue value)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (i >= count) break;
                if (key.Equals(keys[i]))
                {
                    value = values[i];
                    return true;
                }
            }
            value = default(TValue);
            return false;
        }

        public void Clear()
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (i >= count) break;
                keys[i] = default(TKey);
                values[i] = default(TValue);
            }
            count = 0;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (int i = 0; i < count; i++)
                yield return new KeyValuePair<TKey, TValue>(keys[i], values[i]);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        internal void AddIfNotPresent(TKey key, TValue value)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (i >= count) break;
                if (key.Equals(keys[i])) return;
            }
            Add(key, value);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (i >= count) break;
                if (item.Key.Equals(keys[i]))
                {
                    TValue value = values[i];
                    if (item.Value == null) return value == null;
                    return item.Value.Equals(value);
                }
            }
            return false;
        }

        public bool Remove(TKey key)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (i >= count) break;
                if (key.Equals(keys[i]))
                {
                    removeEntry(i);
                    return true;
                }
            }
            return false;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (i >= count) break;
                if (item.Key.Equals(keys[i]))
                {
                    TValue value = values[i];
                    if (item.Value == null)
                    {
                        if (value == null) { removeEntry(i); return true; }
                        return false;
                    }
                    if (item.Value.Equals(value)) { removeEntry(i); return true; }
                    return false;
                }
            }
            return false;
        }

        private void removeEntry(int index)
        {
            for (int i = index + 1; i < keys.Length; i++)
            {
                if (i >= count) break;
                keys[i - 1] = keys[i];
                values[i - 1] = values[i];
            }
            count--;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            KeyValuePair<TKey, TValue>[] ourArray = new KeyValuePair<TKey, TValue>[count];
            for (int i = 0; i < count; i++)
                ourArray[i] = new KeyValuePair<TKey, TValue>(keys[i], values[i]);

            Array.Copy(ourArray, 0, array, arrayIndex, count);
        }

    }
}
