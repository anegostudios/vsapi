using System.Collections.Generic;

namespace Vintagestory.API.Datastructures
{
    public class ListDictionary<TKey, TValue> : Dictionary<TKey, List<TValue>>
    {

        public ListDictionary() : base() { }

        public ListDictionary(int capacity) : base(capacity) { }

        public ListDictionary(IEqualityComparer<TKey> comparer) : base(comparer) { }

        public ListDictionary(IDictionary<TKey, List<TValue>> dictionary) : base(dictionary) { }

        public ListDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer) { }

        public ListDictionary(IDictionary<TKey, List<TValue>> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer) { }

        public void Add(TKey key, TValue value)
        {
            List<TValue> list = this[key];
            if (list == null)
            {
                list = new List<TValue>();
                Add(key, list);
            }
            list.Add(value);
        }

        public TValue GetEquivalent(TKey key, TValue value)
        {
            foreach (var entry in this[key])
            {
                if (entry.Equals(value))
                    return entry;
            }
            return default(TValue);
        }

        public bool ContainsValue(TKey key, TValue value)
        {
            return this[key].Contains(value);
        }

        public bool ContainsValue(TValue value)
        {
            foreach (var valueList in Values)
            {
                if (valueList.Contains(value))
                    return true;
            }
            return false;
        }

        public void ClearKey(TKey key)
        {
            this[key].Clear();
        }

        public bool Remove(TKey key, TValue value)
        {
            return this[key].Remove(value);
        }

        public bool Remove(TValue value)
        {
            foreach (var valueList in Values)
            {
                if (valueList.Remove(value))
                    return true;
            }
            return false;
        }

        public TKey GetKeyOfValue(TValue value)
        {
            foreach (var key in Keys)
            {
                if (this[key].Contains(value))
                    return key;
            }
            return default(TKey);
        }
    }
}
