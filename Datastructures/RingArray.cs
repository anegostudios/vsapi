using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.MathTools;

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

        public int Start => cursor;
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

    }
}
