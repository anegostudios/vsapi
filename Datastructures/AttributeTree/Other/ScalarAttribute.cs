using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Util;

namespace Vintagestory.API.Datastructures
{
    public abstract class ScalarAttribute<T>
    {
        public T value;

        public virtual bool Equals(IAttribute attr)
        {
            return attr.GetValue().Equals(value) || EqualityUtil.NumberEquals(value as object, attr.GetValue());
        }

        public virtual object GetValue()
        {
            return value;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public string ToJsonToken()
        {
            return value.ToString();
        }
    }
}
