using Vintagestory.API.Client;
using Vintagestory.API.MathTools;
using Vintagestory.API.Common;

namespace Vintagestory.API.Client
{
    public class MeshRef
    {
        public bool Disposed;

        /// <summary>
        /// Equivalent to calling api.Render.DeleteMesh()
        /// </summary>
        public virtual void Dispose() {
            Disposed = true;
        }
        
    }
}
