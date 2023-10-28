using System.IO;
using Vintagestory.API.Datastructures;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// 
    /// </summary>
    public interface IResolvableCollectible
    {
        void Resolve(ItemSlot intoslot, IWorldAccessor worldForResolve);
    }
}
