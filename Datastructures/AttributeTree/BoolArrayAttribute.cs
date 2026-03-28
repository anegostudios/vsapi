using System;
using System.IO;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    public class BoolArrayAttribute : ArrayAttribute<bool>, IAttribute
    {
        public BoolArrayAttribute()
        {

        }

        public BoolArrayAttribute(bool[] value)
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
            bool[] values = GC.AllocateUninitializedArray<bool>(quantity);
            for (int i = 0; i < quantity; i++)
            {
                values[i] = stream.ReadBoolean();
            }
            this.value = values;
        }

        public int GetAttributeId()
        {
            return 16;
        }

        public IAttribute Clone()
        {
            return new BoolArrayAttribute((bool[])value.Clone());
        }
    }
}
