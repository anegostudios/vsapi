using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#nullable disable

namespace Vintagestory.API.Client
{
    public abstract class UBORef : IDisposable
    {
        public int Handle;

        /// <summary>
        /// Am I disposed?
        /// </summary>
        public bool Disposed { get; protected set; }

        /// <summary>
        /// Size in Bytes
        /// </summary>
        public int Size { get; set; }

        public abstract void Bind();
        public abstract void Unbind();

        /// <summary>
        /// Frees up any gpu allocated memory. Equivalent to calling api.Render.DeleteMesh()
        /// </summary>
        public virtual void Dispose()
        {
            Disposed = true;
        }

        public abstract void Update<T>(T data) where T : struct;

        public abstract void Update<T>(T data, int offset, int size) where T : struct;

        public abstract void Update(object data, int offset, int size);
    }
}


