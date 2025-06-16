using ProtoBuf;
using System.IO;

#nullable disable

namespace Vintagestory.API.Util
{
    public static class SerializerUtil
    {
        public delegate void ByteWriteDelegatae(BinaryWriter writer);
        public delegate void ByteReadDelegatae(BinaryReader reader);

        public static readonly byte[] SerializedOne;
        public static readonly byte[] SerializedZero;

        static SerializerUtil()
        {
            SerializedOne = Serialize(1);
            SerializedZero = Serialize(0);
        }

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
        /// For performance, version using a reusable FastMemoryStream provided as a parameter.
        /// <br/>(Caution: do not re-use the same stream in a nested way e.g. serializing individual elements within a larger object, as every call to this resets the stream)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static byte[] Serialize<T>(T data, Datastructures.FastMemoryStream ms)
        {
            ms.Reset();
            Serializer.Serialize(ms, data);
            return ms.ToArray();
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
        /// Uses ProtoBuf.Net to deserialize bytes into existing object T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T DeserializeInto<T>(T instance, byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                return Serializer.Merge(ms, instance);
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
