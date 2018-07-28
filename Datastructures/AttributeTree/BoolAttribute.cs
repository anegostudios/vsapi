using System.IO;

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
    }
}
