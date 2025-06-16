using System.IO;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    public class IntAttribute : ScalarAttribute<int>, IAttribute
    {
        public IntAttribute()
        {

        }

        public IntAttribute(int value)
        {
            this.value = value;
        }

        public void FromBytes(BinaryReader stream)
        {
            value = stream.ReadInt32();
        }

        public void ToBytes(BinaryWriter stream)
        {
            stream.Write(value);
        }

        public int GetAttributeId()
        {
            return 1;
        }

        public IAttribute Clone()
        {
            return new IntAttribute(value);
        }
    }
}
