using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace Vintagestory.API.Client
{
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

        bool disposed;

        string trace;
        protected ICoreClientAPI capi;

        public bool Disposed { get { return disposed; } }

        /// <summary>
        /// Set this only you really know what you're doing
        /// </summary>
        public bool IgnoreUndisposed { get; set; }

        static LoadedTexture()
        {
        }

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
                capi?.Logger.Warning("Texture with texture id {0} is leaking memory, missing call to Dispose. Set env var TEXTURE_DEBUG_DISPOSE to get allocation trace.", TextureId);
            }
            else
            {
                capi?.Logger.Warning("Texture with texture id {0} is leaking memory, missing call to Dispose. Allocated at {1}.", TextureId, trace);
            }

        }
    }

}
