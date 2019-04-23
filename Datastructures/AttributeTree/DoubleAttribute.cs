using System.Globalization;
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

        public override string ToJsonToken()
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
