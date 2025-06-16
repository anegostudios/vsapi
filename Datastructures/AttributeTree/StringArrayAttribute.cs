using System.IO;
using System.Text;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    public class StringArrayAttribute : ArrayAttribute<string>, IAttribute
    {
        public StringArrayAttribute()
        {

        }

        public StringArrayAttribute(string[] value)
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
            value = new string[quantity];
            for (int i = 0; i < quantity; i++)
            {
                value[i] = stream.ReadString();
            }
            
        }

        public int GetAttributeId()
        {
            return 10;
        }

        public override string ToJsonToken()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");

            for (int i = 0; i < value.Length; i++)
            {
                if (i > 0) sb.Append(", ");

                sb.Append("\"" + value[i] + "\"");
            }
            sb.Append("]");

            return sb.ToString();
        }

        public IAttribute Clone()
        {
            return new StringArrayAttribute((string[])value.Clone());
        }
    }
}
