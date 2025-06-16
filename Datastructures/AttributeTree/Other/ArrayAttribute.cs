using System.Collections;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    public abstract class ArrayAttribute<T>
    {
        public T[] value;

        public virtual bool Equals(IWorldAccessor worldForResolve, IAttribute attr)
        {
            object othervalue = attr.GetValue();
            if (!othervalue.GetType().IsArray) return false;

            IList a = value as IList;
            IList b = othervalue as IList;

            if (a.Count != b.Count) return false;

            for (int i = 0; i < a.Count; i++)
            {
                if (!a[i].Equals(b[i]))
                {
                    if (!EqualityUtil.NumberEquals(a[i], b[i])) return false;
                }
            }

            return true;
        }

        

        public virtual object GetValue()
        {
            return value;
        }


        public virtual string ToJsonToken()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");

            for (int i = 0; i < value.Length; i++)
            {
                if (i > 0) sb.Append(", ");

                if (value[i] is IAttribute)
                {
                    sb.Append((value[i] as IAttribute).ToJsonToken());
                } else
                {
                    sb.Append(value[i]);
                }                
            }
            sb.Append("]");

            return sb.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");

            for (int i = 0; i < value.Length; i++)
            {
                if (i > 0) sb.Append(", ");

                sb.Append(value[i]);
            }

            sb.Append("]");

            return sb.ToString();
        }


        public override int GetHashCode()
        {
            int hashcode = 0;
            for (int i = 0; i < value.Length; i++)
            {
                if (i == 0) hashcode = value[i].GetHashCode();
                else hashcode ^= value[i].GetHashCode();
            }

            return hashcode;
        }
    }
}
