using System.IO;
using System.Text;

namespace Vintagestory.API.Datastructures
{
    public class StringArrayAttribute : ArrayAttribute<string>, IAttribute
    {
        public StringArrayAttribute()
        {

        }

        public StringArrayAttribute(string[] value)
        {
            this.value = value;
        }

        public void ToBytes(BinaryWriter stream)
        {
            stream.Write(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                stream.Write(value[i]);
            }
            
        }

        public void FromBytes(BinaryReader stream)
        {
            int quantity = stream.ReadInt32();
            value = new string[quantity];
            for (int i = 0; i < quantity; i++)
            {
                value[i] = stream.ReadString();
            }
            
        }

        public int GetAttributeId()
        {
            return 10;
        }
        

    }
}
