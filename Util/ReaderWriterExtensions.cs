using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
