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
