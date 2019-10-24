using System.IO;

namespace Vintagestory.API.Datastructures
{
    public class StringAttribute : ScalarAttribute<string>, IAttribute
    {
        public StringAttribute()
        {
            this.value = "";
        }

        public StringAttribute(string value)
        {
            this.value = value;
        }

        public void ToBytes(BinaryWriter stream)
        {
            if (value == null) value = "";
            stream.Write(value);
        }

        public void FromBytes(BinaryReader stream)
        {
            value = stream.ReadString();
        }

        public int GetAttributeId()
        {
            return 5;
        }

        public override string ToJsonToken()
        {
            return "\"" + value + "\"";
        }
    }
}
