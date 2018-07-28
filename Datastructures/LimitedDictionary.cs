using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Datastructures
{
    /// <summary>
    /// Can only hold a limited amount of elements, discards oldest elements once the capacity is reached.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class LimitedDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> dictionary;
        private Queue<TKey> keys;
        private int capacity;

        /// <summary>
        /// Create a new limited dictionary with given maximum capacity
        /// </summary>
        /// <param name="maxCapacity"></param>
        public LimitedDictionary(int maxCapacity)
        {
            keys = new Queue<TKey>(maxCapacity);
            capacity = maxCapacity;
            dictionary = new Dictionary<TKey, TValue>(maxCapacity);
        }

        public void Add(TKey key, TValue value)
        {
            if (dictionary.Count == capacity)
            {
                var oldestKey = keys.Dequeue();
                dictionary.Remove(oldestKey);
            }

            dictionary.Add(key, value);
            keys.Enqueue(key);
        }

        public TValue this[TKey key]
        {
            get {
                TValue val = default(TValue);
                dictionary.TryGetValue(key, out val);
               return val;
            }
        }
        

        public int Count
        {
            get { return keys.Count; }
        }

    }
}
