using System;
using System.Collections.Generic;
using Vintagestory.API.Common;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    /// <summary>
    /// Same as your normal c# queue but ensures that every element is contained only once using a Hashset.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class UniqueQueue<T> : IEnumerable<T>
    {
        private Dictionary<T, LinkedListNode<T>> nodesByValue;
        private LinkedList<T> list;


        public UniqueQueue()
        {
            nodesByValue = new Dictionary<T, LinkedListNode<T>>();
            list = new LinkedList<T>();
        }

        /// <summary>
        /// Amount of elements in the queue
        /// </summary>
        public int Count
        {
            get
            {
                return nodesByValue.Count;
            }
        }

        /// <summary>
        /// Emptys the queue and the hashset
        /// </summary>
        public void Clear()
        {
            nodesByValue.Clear();
            list.Clear();
        }

        /// <summary>
        /// Check if given item is contained. Uses the hashset to find the item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return nodesByValue.ContainsKey(item);
        }

        /// <summary>
        /// Adds an item to the queue
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item)
        {
            if (!nodesByValue.ContainsKey(item))
            {
                nodesByValue[item] = list.AddLast(item);
            }
        }

        /// <summary>
        /// Removes an item from the queue
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            LinkedListNode<T> node = list.First;
            if (node == null)
            {
                throw new InvalidOperationException("Queue empty.");
            }
            T item = node.Value;
            list.RemoveFirst();
            nodesByValue.Remove(item);
            return item;
        }

        /// <summary>
        /// Returns the first item in the queue without removing it.
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            LinkedListNode<T> node = list.First;
            if (node == null)
            {
                throw new InvalidOperationException("Queue empty.");
            }
            return node.Value;
        }


        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        public void Remove(T item)
        {
            if (nodesByValue.Remove(item, out LinkedListNode<T> node))
            {
                list.Remove(node);
            }
        }
    }
}
