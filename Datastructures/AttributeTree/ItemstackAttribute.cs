using System;
using System.IO;
using Vintagestory.API.Common;

namespace Vintagestory.API.Datastructures
{
    public class ItemstackAttribute : IAttribute
    {
        public ItemStack value;

        public ItemstackAttribute()
        {

        }

        public ItemstackAttribute(ItemStack value)
        {
            this.value = value;
        }

        public int GetAttributeId()
        {
            return 7;
        }

        public object GetValue()
        {
            return value;
        }


        public void FromBytes(BinaryReader stream)
        {
            bool isNull = stream.ReadBoolean();
            if (!isNull)
            {
                value = new ItemStack();
                value.FromBytes(stream);
            }
        }


        public void ToBytes(BinaryWriter stream)
        {
            stream.Write(value == null);
            if (value != null) value.ToBytes(stream);
        }

        public bool Equals(IWorldAccessor worldForResolve, IAttribute attr)
        {
            return Equals(worldForResolve, attr, null);
        }

        internal bool Equals(IWorldAccessor worldForResolve, IAttribute attr, string[] ignorePaths)
        {
            if (!(attr is ItemstackAttribute)) return false;

            ItemstackAttribute stackAttr = (ItemstackAttribute)attr;

            return
                (stackAttr.value == null && value == null) ||
                (stackAttr.value != null && stackAttr.value.Equals(worldForResolve, value, ignorePaths))
            ;
        }

        public string ToJsonToken()
        {
            throw new System.NotImplementedException();
        }

        public override int GetHashCode()
        {
            return value?.GetHashCode() ?? 0;
        }


    }
}
