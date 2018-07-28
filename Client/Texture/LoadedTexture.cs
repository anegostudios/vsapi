using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;

namespace Vintagestory.API.Client
{
    public class LoadedTexture
    {
        public int textureId;
        public int width;
        public int height;

        bool disposed;

        string trace;
        ICoreClientAPI capi;

        static bool DEBUG_DISPOSE;

        static LoadedTexture()
        {
            //Environment.SetEnvironmentVariable("LOADEDTEXTURE_DEBUG_DISPOSE", "1");
            DEBUG_DISPOSE = Environment.GetEnvironmentVariable("LOADEDTEXTURE_DEBUG_DISPOSE") == "1";
        }

        public LoadedTexture(ICoreClientAPI capi)
        {
            this.capi = capi;

            if (DEBUG_DISPOSE)
            {
                trace = Environment.StackTrace;
            }
        }

        public LoadedTexture(int textureId, int width, int height)
        {
            this.textureId = textureId;
            this.width = width;
            this.height = height;
        }

        public void Dispose()
        {
            this.disposed = true;
            capi?.Gui.DeleteTexture(textureId);
        }

        ~LoadedTexture()
        {
            if (disposed || capi?.IsShuttingDown == true) return;

            if (!DEBUG_DISPOSE)
            {
                capi?.World.Logger.Warning("Texture with texture id {0} is leaking memory, missing call to Dispose. Set env var LOADEDTEXTURE_DEBUG_DISPOSE to get allocation trace.", textureId);
            }
            else
            {
                capi?.World.Logger.Warning("Texture with texture id {0} is leaking memory, missing call to Dispose. Allocated at {1}.", textureId, trace);
            }

        }
    }

}
