using System;
using System.IO;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    public class LongArrayAttribute : ArrayAttribute<long>, IAttribute
    {
        public LongArrayAttribute()
        {

        }

        public LongArrayAttribute(long[] value)
        {
            this.value = value;
        }

        public void ToBytes(BinaryWriter stream)
        {
            var values = this.value;
            stream.Write(values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                stream.Write(values[i]);
            }

        }

        public void FromBytes(BinaryReader stream)
        {
            int quantity = stream.ReadInt32();
            long[] values = GC.AllocateUninitializedArray<long>(quantity);
            for (int i = 0; i < quantity; i++)
            {
                values[i] = stream.ReadInt64();
            }
            this.value = values;
        }

        public uint[] AsUint
        {
            get
            {
                uint[] vals = GC.AllocateUninitializedArray<uint>(value.Length);
                for (int i = 0; i < vals.Length; i++)
                {
                    vals[i] = (uint)value[i];
                }
                return vals;

            }
        }


        public int GetAttributeId()
        {
            return 15;
        }

        public IAttribute Clone()
        {
            return new LongArrayAttribute((long[])value.Clone());
        }

    }
}
