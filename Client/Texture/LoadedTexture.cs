using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace Vintagestory.API.Client
{
    public class LoadedTexture
    {
        public int TextureId;
        public int Width;
        public int Height;

        bool disposed;

        string trace;
        protected ICoreClientAPI capi;

        static LoadedTexture()
        {
        }

        public LoadedTexture(ICoreClientAPI capi)
        {
            this.capi = capi;

            if (RuntimeEnv.DebugTextureDispose)
            {
                trace = Environment.StackTrace;
            }
        }

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

        public virtual void Dispose()
        {
            this.disposed = true;
            capi?.Gui.DeleteTexture(TextureId);
        }

        ~LoadedTexture()
        {
            if (disposed || capi?.IsShuttingDown == true) return;

            if (trace == null)
            {
                capi?.Logger.Warning("Texture with texture id {0} is leaking memory, missing call to Dispose. Set env RuntimeEnv.DebugTextureDispose to get allocation trace.", TextureId);
            }
            else
            {
                capi?.Logger.Warning("Texture with texture id {0} is leaking memory, missing call to Dispose. Allocated at {1}.", TextureId, trace);
            }

        }
    }

}
