using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Util
{
    public static class SerializerUtil
    {
        public delegate void ByteWriteDelegatae(BinaryWriter writer);
        public delegate void ByteReadDelegatae(BinaryReader reader);

        public static byte[] Serialize<T>(T data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize(ms, data);
                return ms.ToArray();
            }
        }

        public static T Deserialize<T>(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(ms);
            }
        }

        public static byte[] ToBytes(ByteWriteDelegatae toWrite)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using(BinaryWriter writer = new BinaryWriter(ms))
                {
                    toWrite(writer);
                }

                return ms.ToArray();
            }
        }

        public static void FromBytes(byte[] data, ByteReadDelegatae toRead)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(ms))
                {
                    toRead(reader);
                }
            }
        }
    }
}
