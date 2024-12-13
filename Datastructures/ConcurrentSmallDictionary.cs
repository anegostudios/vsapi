using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading;
using Vintagestory.API.Common;

namespace Vintagestory.Common
{
    /// <summary>
    /// Similar to a FastSmallDictionary, but this one is thread-safe for simultaneous reads and writes - will not throw a ConcurrentModificationException
    /// Low-lock: there is no lock or interlocked operation except when adding new keys or when removing entries
    /// Low-memory: and contains only a single null field, if it is empty
    /// Two simultaneous writes at the same time on different threads: small chance of a ConcurrentModificationException if both have the same keys, otherwise it's virtually impossible for us to preserve the rule that the Dictionary holds exactly one entry per key
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class ConcurrentSmallDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        DTable<TKey, TValue> contents;     // Changes to this are atomic, for example if we need to increase capacity

        public ConcurrentSmallDictionary(int capacity)
        {
            if (capacity == 0) contents = null;
            else contents = new DTable<TKey, TValue>(capacity);
        }

        public ConcurrentSmallDictionary() : this(4)
        {
        }

        public ICollection<TKey> Keys { get {
                var contents = this.contents;
                return contents == null ? new TKey[0] : contents.KeysCopy();
        } }

        public ICollection<TValue> Values { get {
                var contents = this.contents;
                return contents == null ? new TValue[0] : contents.ValuesCopy();
        } }

        public bool IsReadOnly => false;

        int ICollection<KeyValuePair<TKey, TValue>>.Count
        {
            get {
                var contents = this.contents;
                return contents == null ? 0 : contents.count;
            }
        }

        /// <summary>
        /// Amount of items currently in the stack
        /// </summary>
        public int Count
        {
            get {
                var contents = this.contents;
                return contents == null ? 0 : contents.count;
            }
        }


        public bool IsEmpty()
        {
            var contents = this.contents;
            return contents == null || contents.count == 0;
        }


        public TValue this[TKey key]
        {
            get
            {
                return contents.GetValue(key);   // Will intentionally throw an NRE if the contents is empty (key not found)
            }
            set
            {
                Add(key, value);
            }
        }

        public void Add(TKey key, TValue value)
        {
            var contents = this.contents;   // Use a consistent snapshot
            if (contents == null)
            {
                if (Interlocked.CompareExchange(ref this.contents, new DTable<TKey, TValue>(key, value), contents) != contents) Add(key, value);
            }
            else
            {
                if (!contents.ReplaceIfKeyExists(key, value))
                {
                    // If we can't simply replace the existing value for this key, we need to add a new key/value pair to the end of the table
                    if (!contents.Add(key, value))
                    {
                        // If adding would exceed the capacity of the table, instead we need to replace contents with a new expanded table
                        if (Interlocked.CompareExchange(ref this.contents, new DTable<TKey, TValue>(contents, key, value), contents) != contents) Add(key, value);
                        // The CompareExchange makes us try again, in the unlikely event that meanwhile another thread also replaced the table
                    }
                    this.contents.DuplicateKeyCheck(key);
                }
            }
        }

        public TValue TryGetValue(TKey key)
        {
            var contents = this.contents;   // Use a consistent snapshot
            return contents == null ? default(TValue) : contents.TryGetValue(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var contents = this.contents;
            if (contents == null)
            {
                value = default(TValue);
                return false;
            }
            return contents.TryGetValue(key, out value);
        }

        public bool ContainsKey(TKey key)
        {
            var contents = this.contents;
            return contents == null ? false : contents.ContainsKey(key);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (!contents.TryGetValue(item.Key, out TValue valueFound)) return false;
            return item.Value.Equals(valueFound);
        }

        public bool Remove(TKey key)
        {
            var contents = this.contents;
            if (contents == null) return false;
            int i = contents.IndexOf(key);
            if (i < 0) return false;
            if (Interlocked.CompareExchange(ref this.contents, new DTable<TKey, TValue>(contents, i), contents) != contents)
            {
                // The CompareExchange makes us try again, in the unlikely event that meanwhile another thread also replaced the table
                return Remove(key);
            }
            return true;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            var contents = this.contents;
            if (contents == null) return false;
            int i = contents.IndexOf(item.Key, item.Value);
            if (i < 0) return false;
            if (Interlocked.CompareExchange(ref this.contents, new DTable<TKey, TValue>(contents, i), contents) != contents)
            {
                // The CompareExchange makes us try again, in the unlikely event that meanwhile another thread also replaced the table
                return Remove(item);
            }
            return true;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            contents.CopyTo(array, arrayIndex);
        }



        /// <summary>
        /// Threadsafe: could occasionally a value which has meanwhile been removed from the Dictionary by a different thread
        /// Iterate over .Keys or .Values instead if an instantaneous snapshot is required (which will also therefore be a historical snapshot if another thread meanwhile makes changes)
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var contents = this.contents;
            if (contents != null)
            {
                int end_snapshot = contents.count;   // We take a snapshot of the end value, this ensures consistent iteration even if there are changes meanwhile
                for (int pos = 0; pos < end_snapshot; pos++)
                {
                    yield return new KeyValuePair<TKey, TValue>(contents.keys[pos], contents.values[pos]);   // The (ushort) operation ensures a positive index
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Wipes the contents and resets the count.
        /// </summary>
        public void Clear()
        {
            contents = null;
        }
    }

    /// <summary>
    /// A single object to allow the arrays in ConcurrentSmallDictionary to be replaced atomically.
    /// Keys, once entered in the keys array, are invariable: if we need to remove a key we create a new DTable
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class DTable<TKey, TValue>
    {
        public readonly TKey[] keys;
        public readonly TValue[] values;
        public volatile int count = 0;
        volatile int countBleedingEdge = 0;

        public DTable(int capacity)
        {
            this.keys = new TKey[capacity];
            this.values = new TValue[capacity];
            count = 0;
        }

        /// <summary>
        /// Special constructor for a new array with a single, first, entry
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public DTable(TKey key, TValue value)
        {
            int capacity = 4;
            this.keys = new TKey[capacity];
            this.values = new TValue[capacity];
            int count = 0;
            keys[count] = key;
            values[count] = value;
            this.count = count + 1;
            this.countBleedingEdge = count + 1;
        }

        /// <summary>
        /// Special constructor which copies the old arrays and adds one new element at the end - all will be atomic when the Dictionary replaces contents with the results of this
        /// </summary>
        /// <param name="old"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public DTable(DTable<TKey, TValue> old, TKey key, TValue value)
        {
            int capacity = old.values.Length;
            capacity = capacity <= 5 ? 8 : (capacity * 3 / 2 + 1) / 2 * 2;   // Ensures it will always be even in size
            this.keys = new TKey[capacity];
            this.values = new TValue[capacity];
            int count = (int)old.count;
            CopyArray(old.keys, this.keys, 0, 0, count);
            CopyArray(old.values, this.values, 0, 0, count);
            keys[count] = key;
            values[count] = value;
            this.count = count + 1;
            this.countBleedingEdge = count + 1;
        }

        /// <summary>
        /// Special constructor which copies the old arrays and removes one element
        /// </summary>
        /// <param name="old"></param>
        /// <param name="toRemove">the index of the item to remove</param>
        public DTable(DTable<TKey, TValue> old, int toRemove)
        {
            int capacity = old.values.Length;
            this.keys = new TKey[capacity];
            this.values = new TValue[capacity];
            int count = (int)old.count;
            if (toRemove >= count)
            {
                toRemove = count++;    // the ++ post-increment is intended, see the later "this.count = count - 1" statement
                CopyArray(old.keys, this.keys, 0, 0, toRemove);
                CopyArray(old.values, this.values, 0, 0, toRemove);
            }
            else
            {
                CopyArray(old.keys, this.keys, 0, 0, toRemove);
                CopyArray(old.keys, this.keys, toRemove + 1, toRemove, count);
                CopyArray(old.values, this.values, 0, 0, toRemove);
                CopyArray(old.values, this.values, toRemove + 1, toRemove, count);
            }

            this.count = count - 1;
            this.countBleedingEdge = count - 1;
        }

        private void CopyArray(TKey[] source, TKey[] dest, int sourcestart, int deststart, int end)
        {
            // We can add performance optimisations here for short arrays, but let's get this working first
            Array.Copy(source, sourcestart, dest, deststart, end - sourcestart);
        }

        private void CopyArray(TValue[] source, TValue[] dest, int sourcestart, int deststart, int end)
        {
            // We can add performance optimisations here for short arrays, but let's get this working first
            Array.Copy(source, sourcestart, dest, deststart, end - sourcestart);
        }

        internal TValue GetValue(TKey key)
        {
            var keys = this.keys;
            for (int i = 0; i < keys.Length; i++)
            {
                if (i >= count) break;
                if (key.Equals(keys[i])) return values[i];
            }
            throw new KeyNotFoundException("The key " + key.ToString() + " was not found");
        }

        internal TValue TryGetValue(TKey key)
        {
            var keys = this.keys;
            for (int i = 0; i < keys.Length; i++)
            {
                if (i >= count) break;
                if (key.Equals(keys[i])) return values[i];
            }
            return default(TValue);
        }

        internal bool TryGetValue(TKey key, out TValue value)
        {
            var keys = this.keys;
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

        internal bool ContainsKey(TKey key)
        {
            var keys = this.keys;
            for (int i = 0; i < keys.Length; i++)
            {
                if (i >= count) break;
                if (key.Equals(keys[i])) return true;
            }
            return false;
        }

        internal int IndexOf(TKey key)
        {
            var keys = this.keys;
            for (int i = 0; i < keys.Length; i++)
            {
                if (i >= count) break;
                if (key.Equals(keys[i])) return i;
            }
            return -1;
        }

        internal int IndexOf(TKey key, TValue value)
        {
            var keys = this.keys;
            for (int i = 0; i < keys.Length; i++)
            {
                if (i >= count) break;
                if (key.Equals(keys[i]))
                {
                    if (value.Equals(values[i])) return i;
                    return -1;
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns true if key exists (in which case the value at that key is replaced), otherwise returns false
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newValue"></param>
        /// <returns>true if successful replacement, otherwise false</returns>
        internal bool ReplaceIfKeyExists(TKey key, TValue newValue)
        {
            var keys = this.keys;
            for (int i = 0; i < keys.Length; i++)
            {
                if (i >= count) break;
                if (key.Equals(keys[i]))
                {
                    values[i] = newValue;
                    return true;
                }
            }
            return false;
        }

        internal ICollection<TKey> KeysCopy()
        {
            TKey[] result = new TKey[count];
            Array.Copy(keys, result, result.Length);
            return result;
        }

        internal ICollection<TValue> ValuesCopy()
        {
            TValue[] result = new TValue[count];
            Array.Copy(keys, result, result.Length);
            return result;
        }

        /// <summary>
        /// Add the element to the table.  Fails (adds nothing and returns false) if the table is full
        /// </summary>
        internal bool Add(TKey key, TValue elem)
        {
            // A threadsafe way to add to the end of the arrays
            int pos = countBleedingEdge;
            while (Interlocked.CompareExchange(ref countBleedingEdge, pos + 1, pos) != pos)
            {
                pos++;
            }
            if (pos >= values.Length) return false;   // Signal we exceeded capacity, so a new table will be required
            keys[pos] = key;   // We know this space is available for us because we have uniquely reserved it just above
            values[pos] = elem;
            Interlocked.Increment(ref count);   // We update count last so that it can't accidentally iterate the new values before they have been added to the arrays.  And we use increment instead of setting count equal to countBleedingEdge, so that in case of a race condition it doesn't matter which thread reaches this line first
            return true;
        }

        internal void DuplicateKeyCheck(TKey key)
        {
            var keys = this.keys;
            bool keyPresentAlready = false;
            for (int i = 0; i < keys.Length; i++)
            {
                if (i >= count) return;
                if (key.Equals(keys[i]))
                {
                    if (keyPresentAlready)
                    {
                        throw new InvalidOperationException("ConcurrentSmallDictionary was written to with the same key '" + key + "' in two different threads, we can't handle that!");
                    }
                    keyPresentAlready = true;
                }
            }
        }

        internal void CopyTo(KeyValuePair<TKey, TValue>[] dest, int destIndex)
        {
            int limit = count;
            if (limit > dest.Length - destIndex) limit = dest.Length - destIndex;
            for (int i = 0; i < keys.Length; i++)
            {
                if (i >= limit) return;
                dest[i + destIndex] = new KeyValuePair<TKey, TValue>(keys[i], values[i]);
            }
        }
    }
}
