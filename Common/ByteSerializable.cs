using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Common
{
    public interface ByteSerializable
    {
        void ToBytes(BinaryWriter writer);

        void FromBytes(BinaryReader reader, IWorldAccessor resolver);
    }
}
