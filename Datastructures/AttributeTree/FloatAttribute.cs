using System;
using System.IO;
using Vintagestory.API.Config;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    public class FloatAttribute : ScalarAttribute<float>, IAttribute
    {
        public FloatAttribute()
        {

        }

        public FloatAttribute(float value)
        {
            this.value = value;
        }

        public void FromBytes(BinaryReader stream)
        {
            value = BitConverter.ToSingle(stream.ReadBytes(4), 0);
        }

        public void ToBytes(BinaryWriter stream)
        {
            stream.Write(value);
        }

        public int GetAttributeId()
        {
            return 4;
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
            return new FloatAttribute(value);
        }
    }
}
