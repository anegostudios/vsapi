using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;

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

        static RuntimeEnv()
        {
            if (Path.DirectorySeparatorChar == '\\')
            {
                OS = OS.Windows;
                EnvSearchPathName = "PATH";
                return;
            }
			if (IsMac())
            {
				OS = OS.Mac;
				EnvSearchPathName = "DYLD_FRAMEWORK_PATH";
                return;
            }

			OS = OS.Linux;
			EnvSearchPathName = "LD_LIBRARY_PATH";
        }

		[DllImport("libc")]
		static extern int uname(IntPtr buf);

		static bool IsMac()
		{
			IntPtr buf = IntPtr.Zero;
			try
			{
				buf = Marshal.AllocHGlobal(8192);
				if (uname(buf) == 0)
				{
					string os = Marshal.PtrToStringAnsi(buf);
					if (os == "Darwin") return true;
				}
			}
			catch
			{
			}
			finally
			{
				if (buf != IntPtr.Zero) Marshal.FreeHGlobal(buf);
			}
			return false;
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
    }
}
