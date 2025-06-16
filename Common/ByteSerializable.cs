using System.IO;

#nullable disable

namespace Vintagestory.API.Common
{
    public interface IByteSerializable
    {
        void ToBytes(BinaryWriter writer);

        void FromBytes(BinaryReader reader, IWorldAccessor resolver);
    }
}
