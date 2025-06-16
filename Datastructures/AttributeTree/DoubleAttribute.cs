using System.IO;
using Vintagestory.API.Config;

#nullable disable

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
            return value.ToString(GlobalConstants.DefaultCultureInfo);
        }

        public override string ToString()
        {
            return value.ToString(GlobalConstants.DefaultCultureInfo);
        }

        public IAttribute Clone()
        {
            return new DoubleAttribute(value);
        }
    }
}
