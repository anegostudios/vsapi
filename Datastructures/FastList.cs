using System.Collections;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    /// <summary>
    /// Does not clear elements in Clear(), but only sets the Count to 0
    /// </summary>
    /// <typeparam name="TElem"></typeparam>
    public class FastList<TElem> : IEnumerable
    {
        TElem[] elements = System.Array.Empty<TElem>();
        int count=0;

        public FastList()
        {
        }

        public int Count
        {
            get { return count; }
        }

        public void Add(TElem elem)
        {
            if (count >= elements.Length)
            {
                TElem[] newelements = new TElem[count + 50];
                for (int i = 0; i < elements.Length; i++)
                {
                    newelements[i] = elements[i];
                }
                this.elements = newelements;
            }

            elements[count] = elem;
            count++;
        }

        public void Clear()
        {
            count = 0;
        }

        public void RemoveAt(int index)
        {
            count--;

            if (index == count - 1)
            {
                return;
            }

            for (int i = index; i < count; i++)
            {
                elements[i] = elements[i + 1];
            }
        }

        public TElem this[int index]
        {
            get
            {
                return elements[index];
            }
            set
            {
                elements[index] = value;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return new FastListEnum<TElem>(this);
        }

        public bool Contains(TElem needle)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                if (i >= Count) return false;
                if (needle.Equals(elements[i])) return true;
            }
            return false;
        }
    }

    public class FastListEnum<TElem> : IEnumerator
    {
        int pos=-1;
        FastList<TElem> list;

        public FastListEnum(FastList<TElem> list) {
            this.list = list;
        }

        public object Current => list[pos];

        public bool MoveNext()
        {
            pos++;
            if (pos >= list.Count) return false;
            return true;
        }

        public void Reset()
        {
            pos = -1;
        }
    }


}
