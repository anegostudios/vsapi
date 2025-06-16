using System;
using System.IO;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    public class StringAttribute : ScalarAttribute<string>, IAttribute
    {
        private const int AttributeID = 5;

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
            return AttributeID;
        }

        public override string ToJsonToken()
        {
            return "\"" + value + "\"";
        }
        public IAttribute Clone()
        {
            return new StringAttribute(value);
        }

        /// <summary>
        /// Equivalent output as if a StringAttribute had been created within a TreeAttribute, and then that TreeAttribute was immediately serialized to the writer
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        internal static void DirectWrite(BinaryWriter writer, string key, string value)
        {
            writer.Write((byte)AttributeID);
            writer.Write(key);
            writer.Write(value);
        }
    }
}
