using System;
using System.Collections;
using System.Collections.Generic;

namespace Vintagestory.API.Util
{
    public class QueueOfInt
    {
        public int Count = 0;
        public int maxSize = 27;
        protected int tail = 0;
        protected int head = 0;
        protected int[] array;

        public QueueOfInt()
        {
            array = new int[maxSize];
        }

        public void Clear()
        {
            Count = 0;
            head = 0;
            tail = 0;
        }

        public void Enqueue(int v)
        {
            if (Count == maxSize)
            {
                expandArray();
            }

            array[tail++ % maxSize] = v;
            Count++;
        }

        /// <summary>
        /// Will return invalid data if called when Count <= 0: it is the responsibility of the calling code to check Count > 0
        /// </summary>
        /// <returns></returns>
        public int Dequeue()
        {
            Count--;
            return array[head++ % maxSize];
        }

        private void expandArray()
        {
            head %= maxSize;          //normalise this
            int lengthToCopy = maxSize;
            maxSize = maxSize * 2 + 1;
            int[] newArray = new int[maxSize];
            if (head == 0)
            {
                // easy case, it's a straight copy
                ArrayCopy(array, 0, newArray, 0, lengthToCopy);
            }
            else
            {
                // all other cases, we need to unwrap it into the new array
                lengthToCopy -= head;
                ArrayCopy(array, head, newArray, 0, lengthToCopy);
                ArrayCopy(array, 0, newArray, lengthToCopy, head);
            }
            array = newArray;
            head = 0;
            tail = Count;
        }

        private void ArrayCopy(int[] src, int srcOffset, int[] dest, int destOffset, int len)
        {
            if (len > 128)
            {
                Array.Copy(src, srcOffset, dest, destOffset, len);
            }
            else
            {
                int i = srcOffset;
                int j = destOffset;
                int lim = len / 4 * 4 + srcOffset;
                while (i < lim)
                {
                    dest[j] = src[i];   //According to MS docs, in native code the CPU can parallelise four simple instructions like this
                    dest[j + 1] = src[i + 1];
                    dest[j + 2] = src[i + 2];
                    dest[j + 3] = src[i + 3];
                    i += 4;
                    j += 4;
                }
                len += srcOffset;
                while (i < len)
                {
                    dest[j++] = src[i++];
                }
            }
        }
    }
}
