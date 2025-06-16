using System;

#nullable disable

namespace Vintagestory.API.Client
{
    public class MultiTextureMeshRef : IDisposable
    {
        public MeshRef[] meshrefs;
        public int[] textureids;

        bool disposed;

        public bool Disposed => disposed;

        public MultiTextureMeshRef(MeshRef[] meshrefs, int[] textureids)
        {
            this.meshrefs = meshrefs;
            this.textureids = textureids;
        }

        public bool Initialized => meshrefs.Length > 0 && meshrefs[0].Initialized;

        public void Dispose()
        {
            foreach (var meshref in meshrefs) meshref.Dispose();
            disposed = true;
        }


    }

    /// <summary>
    /// A reference to a mesh that's been uploaded onto the graphics card (i.e. that has been placed in an OpenGL <see href="https://www.khronos.org/opengl/wiki/Vertex_Specification#Vertex_Array_Object">VAO</see>). This reference can be used for rendering it.
    /// </summary>
    public abstract class MeshRef : IDisposable
    {
        public bool MultidrawByTextureId { get; protected set; }

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
