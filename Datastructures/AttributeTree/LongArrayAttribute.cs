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
            stream.Write(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                stream.Write(value[i]);
            }

        }

        public void FromBytes(BinaryReader stream)
        {
            int quantity = stream.ReadInt32();
            value = new long[quantity];
            for (int i = 0; i < quantity; i++)
            {
                value[i] = stream.ReadInt64();
            }

        }

        public uint[] AsUint
        {
            get
            {
                uint[] vals = new uint[value.Length];
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
