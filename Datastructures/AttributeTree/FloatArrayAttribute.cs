using System;
using System.IO;
using System.Text;
using Vintagestory.API.Config;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    public class FloatArrayAttribute : ArrayAttribute<float>, IAttribute
    {
        public FloatArrayAttribute()
        {

        }

        public FloatArrayAttribute(float[] value)
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
            float[] values = GC.AllocateUninitializedArray<float>(quantity);
            for (int i = 0; i < quantity; i++)
            {
                values[i] = stream.ReadSingle();
            }
            this.value = values;
        }

        public int GetAttributeId()
        {
            return 12;
        }

        public override string ToJsonToken()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('[');

            for (int i = 0; i < value.Length; i++)
            {
                if (i > 0) sb.Append(", ");

                sb.Append(value[i].ToString(GlobalConstants.DefaultCultureInfo));
            }
            sb.Append(']');

            return sb.ToString();
        }

        public IAttribute Clone()
        {
            return new FloatArrayAttribute((float[])value.Clone());
        }

    }
}
