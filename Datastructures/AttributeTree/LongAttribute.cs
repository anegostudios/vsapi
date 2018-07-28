using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API;

namespace Vintagestory.API.Datastructures
{
    public class LongAttribute : ScalarAttribute<long>, IAttribute
    {
        public LongAttribute()
        {

        }

        public LongAttribute(long value)
        {
            this.value = value;
        }

        public void FromBytes(BinaryReader stream)
        {
            value = stream.ReadInt64();
        }

        public void ToBytes(BinaryWriter stream)
        {
            stream.Write(value);
        }

        public int GetAttributeId()
        {
            return 2;
        }
    }
}
