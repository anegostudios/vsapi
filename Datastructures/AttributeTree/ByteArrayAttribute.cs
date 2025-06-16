using System;
using System.IO;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    public class ByteArrayAttribute : ArrayAttribute<byte>, IAttribute
    {
        private const int AttributeID = 8;

        public ByteArrayAttribute()
        {

        }

        public ByteArrayAttribute(byte[] value)
        {
            this.value = value;
        }

        public ByteArrayAttribute(FastMemoryStream ms)
        {
            this.value = ms.ToArray();
        }

        public void ToBytes(BinaryWriter stream)
        {
            stream.Write((ushort)value.Length);
            stream.Write(value);
        }

        public void FromBytes(BinaryReader stream)
        {
            int length = stream.ReadInt16();
            value = stream.ReadBytes(length);
        }

        public int GetAttributeId()
        {
            return AttributeID;
        }

        public IAttribute Clone()
        {
            return new ByteArrayAttribute((byte[])value.Clone());
        }
    }
}
