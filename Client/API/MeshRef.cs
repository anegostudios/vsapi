using Vintagestory.API.Client;
using Vintagestory.API.MathTools;
using Vintagestory.API.Common;
using System;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// A reference to a mesh that's been uploaded onto the graphics card (i.e. that has been placed in an OpenGL <see href="https://www.khronos.org/opengl/wiki/Vertex_Specification#Vertex_Array_Object">VAO</see>). This reference can be used for rendering it.
    /// </summary>
    public abstract class MeshRef : IDisposable
    {
        public abstract bool Initialized { get; }

        /// <summary>
        /// Am I disposed?
        /// </summary>
        public bool Disposed { get; protected set; }

        /// <summary>
        /// Frees up any gpu allocated memory. Equivalent to calling api.Render.DeleteMesh()
        /// </summary>
        public virtual void Dispose() {
            Disposed = true;
        }
        
    }
}
