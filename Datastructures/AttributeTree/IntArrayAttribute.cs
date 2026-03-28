using System;
using System.IO;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    public class IntArrayAttribute : ArrayAttribute<int>, IAttribute
    {
        public IntArrayAttribute()
        {

        }

        public IntArrayAttribute(int[] value)
        {
            this.value = value;
        }

        public IntArrayAttribute(uint[] value)
        {
            int[] values = GC.AllocateUninitializedArray<int>(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                values[i] = (int)value[i];
            }
            this.value = values;
        }

        public IntArrayAttribute(ushort[] value)
        {
            int[] values = GC.AllocateUninitializedArray<int>(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                values[i] = value[i];
            }
            this.value = values;
        }

        public ushort[] AsUShort
        {
            get
            {
                ushort[] vals = GC.AllocateUninitializedArray<ushort>(value.Length);
                for (int i = 0; i < vals.Length; i++)
                {
                    vals[i] = (ushort)value[i];
                }
                return vals;
            }
        }

        public uint[] AsUint
        {
            get
            {
                uint[] vals = GC.AllocateUninitializedArray<uint>(value.Length);
                for (int i = 0; i < vals.Length; i++)
                {
                    vals[i] = (uint)value[i];
                }
                return vals;

            }
        }

        public void ToBytes(BinaryWriter stream)
        {
            var values = this.value;
            stream.Write(values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                stream.Write(values[i]);
            }
        }

        public void FromBytes(BinaryReader stream)
        {
            int quantity = stream.ReadInt32();
            int[] values = GC.AllocateUninitializedArray<int>(quantity);
            for (int i = 0; i < quantity; i++)
            {
                values[i] = stream.ReadInt32();
            }
            this.value = values;
        }

        public int GetAttributeId()
        {
            return 11;
        }

        public void AddInt(params int[] val)
        {
            if (value == null || value.Length == 0)
            {
                value = val;
            } else
            {
                value = value.Append(val);
            }
        }

        public void RemoveInt(int val)
        {
            value = value.Remove(val);
        }

        public IAttribute Clone()
        {
            return new IntArrayAttribute((int[])value.Clone());
        }

    }
}
