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

        /// <summary>
        /// Uses ProtoBuf.NET to serialize T into bytes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Serialize<T>(T data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize(ms, data);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Uses ProtoBuf.Net to deserialize bytes into T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(ms);
            }
        }

        /// <summary>
        /// Uses ProtoBuf.Net to deserialize bytes into T. Returns the default value if data is null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] data, T defaultValue)
        {
            if (data == null) return defaultValue;

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
