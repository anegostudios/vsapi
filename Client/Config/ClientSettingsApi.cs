using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Client
{
    /// <summary>
    /// Sort of like an api accessor to the actual client settings, these values are set by the Client.
    /// This is a little bit of a hack, yes
    /// </summary>
    public class ClientSettingsApi
    {
        public static float GUIScale;

        public static int MainThreadId;

        public static bool ShowEntityDebugInfo;

        public static bool ViewBobbing;

        public static bool HighQualityAnimations;

        public static bool FloatyGuis;
    }
}
