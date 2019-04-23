using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API;

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
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
