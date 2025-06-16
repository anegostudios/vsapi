using System.IO;
using System.Text;
using Vintagestory.API.Config;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    public class DoubleArrayAttribute : ArrayAttribute<double>, IAttribute
    {
        public DoubleArrayAttribute()
        {

        }

        public DoubleArrayAttribute(double[] value)
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
            value = new double[quantity];
            for (int i = 0; i < quantity; i++)
            {
                value[i] = stream.ReadDouble();
            }

        }

        public int GetAttributeId()
        {
            return 13;
        }

        public override string ToJsonToken()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");

            for (int i = 0; i < value.Length; i++)
            {
                if (i > 0) sb.Append(", ");

                sb.Append(value[i].ToString(GlobalConstants.DefaultCultureInfo));
            }
            sb.Append("]");

            return sb.ToString();
        }

        public IAttribute Clone()
        {
            return new DoubleArrayAttribute((double[])value);
        }

    }
}
