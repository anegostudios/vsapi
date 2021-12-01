using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

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


    }
}
