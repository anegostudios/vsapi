using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable disable

namespace Vintagestory.API.Util
{
    public static class HashsetExtensions
    {
        public static void AddRange<T>(this HashSet<T> hashset, IEnumerable<T> elements)
        {
            foreach (T elem in elements) hashset.Add(elem);
        }

        public static void AddRange<T>(this HashSet<T> hashset, HashSet<T> elements)
        {
            foreach (T elem in elements) hashset.Add(elem);
        }

        public static void AddRange<T>(this HashSet<T> hashset, T[] elements)
        {
            foreach (T elem in elements) hashset.Add(elem);
        }

        public static string Implode<T>(this HashSet<T> hashset, string seperator = ", ")
        {
            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (T elem in hashset)
            {
                if (i > 0) builder.Append(seperator);
                builder.Append(elem.ToString());
                i++;
            }

            return builder.ToString();
        }


        public static T PopOne<T>(this ICollection<T> items)
        {
            T item = items.FirstOrDefault();
            if (item != null)
            {
                items.Remove(item);
            }
            return item;
        }
        
    }
}
