using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using Vintagestory.API.Util;

#nullable disable

namespace Vintagestory.API.Config
{
    /// <summary>
    /// Operating System Enum
    /// </summary>
    public enum OS
    {
        Windows,
        Mac,
        Linux
    }

    /// <summary>
    /// Information about the runningtime environment
    /// </summary>
    public static class RuntimeEnv
    {
        /// <summary>
        /// If TEXTURE_DEBUG_DISPOSE is set, the initial value set here will be overridden
        /// </summary>
        public static bool DebugTextureDispose = false;
        /// <summary>
        /// If VAO_DEBUG_DISPOSE is set, the initial value set here will be overridden
        /// </summary>
        public static bool DebugVAODispose = false;
        /// <summary>
        /// Debug sound memory leaks. No ENV var
        /// </summary>
        public static bool DebugSoundDispose = false;

        /// <summary>
        /// If true, will print the stack trace on some of the blockaccessor if something attempts to get or set blocks outside of its available chunks
        /// </summary>
        public static bool DebugOutOfRangeBlockAccess = false;

        /// <summary>
        /// If true, will print allocation trace whenever a new task was enqueued to the thread pool
        /// </summary>
        public static bool DebugThreadPool = false;

        public static int MainThreadId;
        public static int ServerMainThreadId;


        public static float GUIScale;

        /// <summary>
        /// The current operating system
        /// </summary>
        public static readonly OS OS;

        /// <summary>
        /// The Env variable which contains the OS specific search paths for libarires
        /// </summary>
        public static readonly string EnvSearchPathName;

        /// <summary>
        /// This will only be set to true on Linux when the session is a wayland session
        /// This is needed so we can skip adding a app Icon or setting the cursor postion since those are not supported on wayland
        /// </summary>
        public static readonly bool IsWaylandSession;

        static RuntimeEnv()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                OS = OS.Windows;
                EnvSearchPathName = "PATH";
                return;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                OS = OS.Linux;
                EnvSearchPathName = "LD_LIBRARY_PATH";
                var sessionType = Environment.GetEnvironmentVariable("XDG_SESSION_TYPE");
                var useWayland = Environment.GetEnvironmentVariable("OPENTK_4_USE_WAYLAND");
                // try to use X11 windows (xwayland) even if we are on wayland since its performance is better and has less issues like scaling
                if (sessionType == "wayland" && useWayland == null)
                {
                    Environment.SetEnvironmentVariable("OPENTK_4_USE_WAYLAND", "0");
                    IsWaylandSession = false;
                    return;
                }
                IsWaylandSession = sessionType == "wayland" && useWayland != "0";
                return;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                OS = OS.Mac;
                EnvSearchPathName = "DYLD_FRAMEWORK_PATH";
            }
        }


        /// <summary>
        /// Whether we are in a dev environment or not
        /// </summary>
        public static readonly bool IsDevEnvironment = !Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets"));


        public static string GetLocalIpAddress()
        {
            try
            {
                // This seems to be the preferred / more reliable way of getting the ip
                // but it seems one of the methods are not implemented in net (crossplatform) so we fallback
                // to a simple method if an exception is thrown
                var allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var networkInterface in allNetworkInterfaces)
                {
                    if (networkInterface.OperationalStatus != OperationalStatus.Up)
                    {
                        continue;
                    }

                    var iPProperties = networkInterface.GetIPProperties();
                    if (iPProperties.GatewayAddresses.Count == 0)
                    {
                        continue;
                    }

                    foreach (var unicastAddress in iPProperties.UnicastAddresses)
                    {
                        if (unicastAddress.Address.AddressFamily != AddressFamily.InterNetwork ||
                            IPAddress.IsLoopback(unicastAddress.Address))
                        {
                            continue;
                        }

                        return unicastAddress.Address.ToString();
                    }
                }

                return "Unknown ip";
            }
            catch (Exception)
            {
                try
                {
                    return Dns.GetHostEntry(Dns.GetHostName()).AddressList
                        .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                        ?.ToString();
                }
                catch (Exception)
                {
                    return "Unknown ip";
                }
            }
        }

        public static string GetOsString()
        {
            switch (OS)
            {
                case OS.Windows:
                    return $"Windows {Environment.OSVersion.Version}";
                case OS.Mac:
                    return $"Mac {Environment.OSVersion.Version}";
                case OS.Linux:
                {
                    try
                    {
                        if (File.Exists("/etc/os-release"))
                        {
                            var lines = File.ReadAllLines("/etc/os-release");
                            var distro = lines.FirstOrDefault(line => line.StartsWithOrdinal("PRETTY_NAME="))
                                ?.Split('=').ElementAt(1)
                                .Trim('"');
                            return $"Linux ({distro}) [Kernel {Environment.OSVersion.Version}]";
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    return $"Linux (Unknown) [Kernel {Environment.OSVersion.Version}]";
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
