using System.Collections;
using System.Collections.Generic;
using Vintagestory.API.MathTools;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    public class RingArray<T> : IEnumerable<T>
    {
        T[] elements;
        int cursor = 0;

        public RingArray(int capacity)
        {
            elements = new T[capacity];
        }

        public RingArray(int capacity, T[] initialvalues)
        {
            elements = new T[capacity];
            if (initialvalues != null)
            {
                for (int i = 0; i < initialvalues.Length; i++)
                {
                    Add(initialvalues[i]);
                }
            }
        }

        public T this[int index] {
            get { return elements[index]; }
            set { elements[index] = value; }
        }

        public int EndPosition { get { return cursor; } set { cursor = value; } }
        public T[] Values => elements;

        /// <summary>
        /// Total size of the stack
        /// </summary>
        public int Length
        {
            get { return elements.Length; }
        }

        /// <summary>
        /// Pushes an element onto the end of the queue
        /// </summary>
        /// <param name="elem"></param>
        public void Add(T elem)
        {
            elements[cursor] = elem;
            cursor = (cursor + 1) % elements.Length;
            return;
        }


        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < elements.Length; i++)
            {
                yield return elements[(cursor + i) % elements.Length];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        /// <summary>
        /// Wipes the buffer and resets the count
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < elements.Length; i++)
            {
                elements[i] = default(T);
            }

            cursor = 0;
        }

        /// <summary>
        /// If smaller than the old size, will discard oldest elements first
        /// </summary>
        /// <param name="size"></param>
        public void ResizeTo(int size)
        {
            T[] elements = new T[size];
            for (int i = 0; i < this.elements.Length; i++)
            {
                elements[size - 1] = this.elements[GameMath.Mod((EndPosition - i), this.elements.Length)];
                size--;
                if (size <= 0) break;
            }

            this.elements = elements;
        }
    }
}
