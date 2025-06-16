using System;
using System.IO;
using Vintagestory.API.Common;

#nullable disable

namespace Vintagestory.API.Datastructures
{
    /// <summary>
    /// An attribute from an attribute tree
    /// </summary>
    public interface IAttribute
    {
        void ToBytes(BinaryWriter stream);
        void FromBytes(BinaryReader stream);

        int GetAttributeId();

        Type GetType();
        object GetValue();
        string ToJsonToken();

        bool Equals(IWorldAccessor worldForResolve, IAttribute attr);
        IAttribute Clone();
    }
}
