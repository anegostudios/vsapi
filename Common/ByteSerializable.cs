using System.IO;

namespace Vintagestory.API.Common
{
    public interface IByteSerializable
    {
        void ToBytes(BinaryWriter writer);

        void FromBytes(BinaryReader reader, IWorldAccessor resolver);
    }
}
