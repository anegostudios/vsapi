using System;
using Vintagestory.API.Config;

#nullable disable

namespace Vintagestory.API.Client
{
    /// <summary>
    /// A reference to a texture that has been uploaded onto the graphics cards, if TextureId is not zero
    /// </summary>
    public class LoadedTexture : IDisposable
    {
        /// <summary>
        /// The OpenGL Texture Id
        /// </summary>
        public int TextureId;

        /// <summary>
        /// Width of the texture.
        /// </summary>
        public int Width;

        /// <summary>
        /// Height of the texture.
        /// </summary>
        public int Height;

        protected bool disposed;
        protected string trace;
        protected ICoreClientAPI capi;

        public bool Disposed { get { return disposed; } }

        /// <summary>
        /// Set this only you really know what you're doing
        /// </summary>
        public bool IgnoreUndisposed { get; set; }


        /// <summary>
        /// Creates an empty loaded texture context with the Client API.
        /// </summary>
        /// <param name="capi">The Client API</param>
        public LoadedTexture(ICoreClientAPI capi)
        {
            this.capi = capi;

            if (RuntimeEnv.DebugTextureDispose)
            {
                trace = Environment.StackTrace;
            }
        }

        /// <summary>
        /// Creates a loaded texture context with pre-set texture information.
        /// </summary>
        /// <param name="capi">The Client API</param>
        /// <param name="textureId">The ID of the texture.</param>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        public LoadedTexture(ICoreClientAPI capi, int textureId, int width, int height)
        {
            this.capi = capi;
            this.TextureId = textureId;
            this.Width = width;
            this.Height = height;

            if (RuntimeEnv.DebugTextureDispose)
            {
                trace = Environment.StackTrace;
            }
        }

        /// <summary>
        /// Disposes of the loaded texture safely.
        /// </summary>
        public virtual void Dispose()
        {
            this.disposed = true;
            if (TextureId != 0)
            {
                capi.Gui.DeleteTexture(TextureId);
                TextureId = 0;
            }
        }

        ~LoadedTexture()
        {
            if (IgnoreUndisposed || TextureId == 0 || disposed || capi?.IsShuttingDown == true) return;

            if (trace == null)
            {
                capi?.Logger.Debug("Texture with texture id {0} is leaking memory, missing call to Dispose. Set env var TEXTURE_DEBUG_DISPOSE to get allocation trace.", TextureId);
            }
            else
            {
                capi?.Logger.Debug("Texture with texture id {0} is leaking memory, missing call to Dispose. Allocated at {1}.", TextureId, trace);
            }

        }
    }

}
