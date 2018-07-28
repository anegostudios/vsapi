using System.IO;

namespace Vintagestory.API.Datastructures
{
    public class DoubleAttribute : ScalarAttribute<double>, IAttribute
    {
        public DoubleAttribute()
        {

        }

        public DoubleAttribute(double value)
        {
            this.value = value;
        }

        public void FromBytes(BinaryReader stream)
        {
            value = stream.ReadDouble();
        }

        public void ToBytes(BinaryWriter stream)
        {
            stream.Write(value);
        }

        public int GetAttributeId()
        {
            return 3;
        }
    }
}
