using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    public class CreativeTabAndStackList
    {
        public string[] Tabs;
        public JsonItemStack[] Stacks;

        /// <summary>
        /// Reads the blocks and items from the Json files and converts them to an array of tabs which contain those blocks and items.
        /// </summary>
        /// <param name="reader">The reader to read the json.</param>
        /// <param name="registry">The registry of blocks and items.</param>
        public void FromBytes(BinaryReader reader, IClassRegistryAPI registry)
        {
            Tabs = new string[reader.ReadInt32()];
            for (int i = 0; i < Tabs.Length; i++)
            {
                Tabs[i] = reader.ReadString();
            }

            Stacks = new JsonItemStack[reader.ReadInt32()];
            for (int i = 0; i < Stacks.Length; i++)
            {
                Stacks[i] = new JsonItemStack();
                Stacks[i].FromBytes(reader, registry);
            }
        }

        /// <summary>
        /// Writes all the data to the BinaryWriter.
        /// </summary>
        /// <param name="writer">The writer to write the save data</param>
        /// <param name="registry">The registry of blocks and items.</param>
        public void ToBytes(BinaryWriter writer, IClassRegistryAPI registry)
        {
            writer.Write(Tabs.Length);
            for (int i = 0; i < Tabs.Length; i++)
            {
                writer.Write(Tabs[i]);
            }

            writer.Write(Stacks.Length);
            for (int i = 0; i < Stacks.Length; i++)
            {
                Stacks[i].ToBytes(writer);
            }
        }
    }
}
