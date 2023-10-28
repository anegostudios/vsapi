using System;

namespace Vintagestory.API.Datastructures
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

        /// <summary>
        /// Enqueue a single value with four separate components, assumed to be signed int in the range -128 to +127
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public void Enqueue(int a, int b, int c, int d)
        {
            Enqueue(a + 128 + (b + 128 << 8) + (c + 128 << 16) + (d << 24));
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
        /// Special method for ChunkIlluminator, if already present does not enqueue, but replaces only if value d (lowest 5 bits representing the spreadlight value) is larger
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public void EnqueueIfLarger(int a, int b, int c, int d)
        {
            EnqueueIfLarger(a + 128 + (b + 128 << 8) + (c + 128 << 16) + (d << 24));
        }

        public void EnqueueIfLarger(int v)
        {
            int h = head % maxSize;
            int t = tail % maxSize;
            int match = v & 0xffffff;
            if (t >= h)
            {
                for (int i = h; i < t; i++)
                {
                    if ((array[i] & 0xffffff) == match)
                    {
                        ReplaceIfLarger(i, v);
                        return;
                    }
                }
            }
            else
            {
                for (int i = 0; i < t; i++)
                {
                    if ((array[i] & 0xffffff) == match)
                    {
                        ReplaceIfLarger(i, v);
                        return;
                    }
                }
                for (int i = h; i < array.Length; i++)
                {
                    if ((array[i] & 0xffffff) == match)
                    {
                        ReplaceIfLarger(i, v);
                        return;
                    }
                }
            }
            Enqueue(v);
        }

        private void ReplaceIfLarger(int i, int v)
        {
            if ((array[i] & 0x1f000000) < (v & 0x1f000000))
            {
                array[i] = v;
            }
        }

        /// <summary>
        /// Will return invalid data if called when Count &lt;= 0: it is the responsibility of the calling code to check Count > 0
        /// </summary>
        /// <returns></returns>
        public int Dequeue()
        {
            Count--;
            return array[head++ % maxSize];
        }

        /// <summary>
        /// Will return invalid data if called when Count &lt;= 0: it is the responsibility of the calling code to check Count > 0
        /// </summary>
        /// <returns></returns>
        public int DequeueLIFO()
        {
            Count--;
            int result = array[tail-- % maxSize];
            if (tail < 0) tail += maxSize;
            return result;
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
                ArrayCopy(array, head, newArray, 0, lengthToCopy);  // Begin at head and copy to the end of the array
                ArrayCopy(array, 0, newArray, lengthToCopy, head);  // Begin at the start of the array and copy to head
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
