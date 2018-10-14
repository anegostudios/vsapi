using Vintagestory.API.Client;
using Vintagestory.API.MathTools;
using Vintagestory.API.Common;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Mesh reference.
    /// </summary>
    public class MeshRef
    {
        /// <summary>
        /// Am I disposed?
        /// </summary>
        public bool Disposed { get; protected set; }

        /// <summary>
        /// Equivalent to calling api.Render.DeleteMesh()
        /// </summary>
        public virtual void Dispose() {
            Disposed = true;
        }
        
    }
}
