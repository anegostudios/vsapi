using System.IO;

#nullable disable

namespace Vintagestory.API.Util
{
    public static class ReaderWriterExtensions
    {

        public static void WriteArray(this BinaryWriter writer, string[] values)
        {
            writer.Write(values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                writer.Write(values[i]);
            }
        }

        public static string[] ReadStringArray(this BinaryReader reader)
        {
            int length = reader.ReadInt32();
            string[] values = new string[length];

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = reader.ReadString();
            }

            return values;
        }


        public static void WriteArray(this BinaryWriter writer, int[] values)
        {
            writer.Write(values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                writer.Write(values[i]);
            }
        }

        public static int[] ReadIntArray(this BinaryReader reader)
        {
            int length = reader.ReadInt32();
            int[] values = new int[length];

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = reader.ReadInt32();
            }

            return values;
        }

        public static void Clear(this MemoryStream ms)
        {
            ms.Seek(0, System.IO.SeekOrigin.Begin);
            ms.SetLength(0);
        }
    }
}
