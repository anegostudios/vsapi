using Newtonsoft.Json;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;

namespace Vintagestory.API.Common
{
    /// <summary>
    /// General interface to provide callback methods with different overloads, one or more of which may be implemented in any specific use case
    /// (VS team should feel free to add more overloads as needed!)
    /// </summary>
    public interface ICallback
    {
        void Callback();
        void Callback(BlockPos pos);
        void Callback(int a, int b, int c);
    }

}
