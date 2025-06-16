using System.IO;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    public class BoolAttribute : ScalarAttribute<bool>, IAttribute
    {
        public BoolAttribute()
        {

        }

        public BoolAttribute(bool value)
        {
            this.value = value;
        }

        public void FromBytes(BinaryReader stream)
        {
            value = stream.ReadBoolean();
        }

        public void ToBytes(BinaryWriter stream)
        {
            stream.Write(value);
        }

        public int GetAttributeId()
        {
            return 9;
        }

        public override string ToJsonToken()
        {
            // Newtonsoft.Json does not like uppercase True/False o.O
            return value ? "true" : "false";
        }

        public IAttribute Clone()
        {
            return new BoolAttribute(value);
        }
    }
}
