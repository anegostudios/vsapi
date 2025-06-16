using System.Collections;
using System.Collections.Generic;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    /// <summary>
    /// Holds a limited amount of items, discards the lowest index item when an overflow happens
    /// </summary>
    public class LimitedList<TElem> : IEnumerable, IEnumerable<TElem>
    {
        private List<TElem> elems;
        private int capacity;

        /// <summary>
        /// Create a new list with a given maximum capacity
        /// </summary>
        /// <param name="maxCapacity"></param>
        public LimitedList(int maxCapacity)
        {
            elems = new List<TElem>(maxCapacity);
            capacity = maxCapacity;
        }

        /// <summary>
        /// Create a new list with a given maximum capacity
        /// </summary>
        /// <param name="maxCapacity"></param>
        /// <param name="initialElements"></param>
        public LimitedList(int maxCapacity, IEnumerable<TElem> initialElements)
        {
            if (initialElements == null)
            {
                elems = new List<TElem>(maxCapacity);
            } else
            {
                elems = new List<TElem>(initialElements);
            }
            
            capacity = maxCapacity;
        }

        public void Add(TElem key)
        {
            if (elems.Count >= capacity)
            {
                elems.RemoveAt(0);
            }
            elems.Add(key);
        }

        public TElem this[int index]
        {
            get
            {
                return elems[index];
            }
        }

        public void SetCapacity(int maxCapacity)
        {
            this.capacity = maxCapacity;
        }

        public void Clear()
        {
            elems.Clear();
        }

        public int Count
        {
            get { return elems.Count; }
        }

        public void RemoveAt(int i)
        {
            elems.RemoveAt(i);
        }

        public TElem LastElement()
        {
            if (Count == 0) return default(TElem);
            return elems[Count - 1];
        }

        public bool IsFull()
        {
            return elems.Count == capacity;
        }

        public IEnumerator GetEnumerator()
        {
            return elems.GetEnumerator();
        }

        public TElem[] ToArray()
        {
            return elems.ToArray();
        }

        IEnumerator<TElem> IEnumerable<TElem>.GetEnumerator()
        {
            return elems.GetEnumerator();
        }
    }
}
