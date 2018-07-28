using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

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
        /// The current operating system
        /// </summary>
        public static readonly OS OS;

        /// <summary>
        /// The Env variable which contains the OS specific search paths for libarires
        /// </summary>
        public static readonly string EnvSearchPathName;

        /// <summary>
        /// .dll for windows, .so for linux, .dylib for mac
        /// </summary>
        public static readonly string LibExtension;

        static RuntimeEnv()
        {
            if (Path.DirectorySeparatorChar == '\\')
            {
                OS = OS.Windows;
                EnvSearchPathName = "PATH";
                LibExtension = ".dll";
                return;
            }
			if (IsMac())
            {
				OS = OS.Mac;
				EnvSearchPathName = "DYLD_FRAMEWORK_PATH";
				LibExtension = ".dylib";
                return;
            }

			OS = OS.Linux;
			EnvSearchPathName = "LD_LIBRARY_PATH";
			LibExtension = ".so";
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
        /// Whether the game is run using the Mono framework
        /// </summary>
        public static readonly bool IsMono = Type.GetType("Mono.Runtime") != null;

        /// <summary>
        /// Whether we are in a dev environment or not
        /// </summary>
        public static readonly bool IsDevEnvironment = !Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets"));
    }
}
