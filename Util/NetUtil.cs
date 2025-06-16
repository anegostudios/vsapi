using System;
using System.Diagnostics;
using System.Net;
using Vintagestory.API.Config;

#nullable disable

namespace Vintagestory.API.Util
{
    public struct UriInfo {
        public string Hostname;
        public int? Port;
        public string Password;

        
    }

    public static class NetUtil
    {
        public static void OpenUrlInBrowser(string url)
        {
            try
            {
                Process.Start("start \"" + url + "\"");
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeEnv.OS == OS.Windows)
                {
                    url = url.Replace("&", "^&");
                    if (Uri.TryCreate(url, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                    {
                        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                    }
                    else
                    {
                        Process.Start("explorer.exe", url);
                    }
                }
                else if (RuntimeEnv.OS == OS.Linux)
                {
                    url = $"\"{url}\"";
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeEnv.OS == OS.Mac)
                {
                    url = $"\"{url}\"";
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        public static bool IsPrivateIp(string ip)
        {
            string[] parts = ip.Split('.');
            if (parts.Length < 2) return false;

            int.TryParse(parts[1], out int secondnum);

            return
                (parts[0] == "10")
                || (parts[0] == "172" && secondnum >= 16 && secondnum <= 31)
                || (parts[0] == "192" && parts[1] == "168")
            ;
        }

        /// <summary>
        /// Extracts hostname, port and password from given uri. Error will be non null if the uri is incorrect in some ways
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static UriInfo getUriInfo(string uri, out string error)
        {
            bool isipv4 = false;
            bool isipv6 = false;

            string password = null;

            if (uri.Contains("@"))
            {
                string[] parts = uri.Split('@');
                password = parts[0];
                uri = parts[1];
            }

            if (IPAddress.TryParse(uri, out IPAddress addr))
            {
                isipv4 = addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork;
                isipv6 = addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6;
            }

            string hostname = uri;
            int port = 0;
            int? outport = null;

            if (!isipv6 && uri.Contains(":"))
            {
                string[] parts = uri.Split(':');
                hostname = parts[0];
                if (int.TryParse(parts[1], out port)) outport = port;
                else error = Lang.Get("Invalid ipv6 address or invalid port number");
            }

            if (isipv6 && uri.Contains("]:"))
            {
                string[] parts = uri.Split(new string[] { "]:" }, StringSplitOptions.None);
                hostname = addr.ToString();
                if (int.TryParse(parts[1], out port)) outport = port;
                else error = Lang.Get("Invalid port number");
            }
            
            error = null;

            return new UriInfo()
            {
                Hostname = hostname,
                Password = password,
                Port = outport
            };
        }
        
    }
}
