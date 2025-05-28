using System;
using System.IO;

namespace Vintagestory.API.Datastructures
{
    public class StreamedTreeAttribute
    {
        private readonly BinaryWriter stream;

        public StreamedTreeAttribute(BinaryWriter writer)
        {
            this.stream = writer;
        }

        public IAttribute this[string key]
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                if (value is StreamedByteArrayAttribute sba)
                {
                    sba.BeginDirectWrite(stream, key);
                    sba.ToBytes(stream);
                }
            }
        }

        internal void WithKey(string key)
        {
            TreeAttribute.BeginDirectWrite(stream, key);
        }

        internal void EndKey()
        {
            TreeAttribute.TerminateWrite(stream);
        }
    }
}
