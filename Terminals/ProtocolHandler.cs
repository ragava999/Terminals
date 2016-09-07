using System;
using System.Windows.Forms;
using Kohl.Framework.Info;
using Kohl.Framework.Logging;
using Microsoft.Win32;

namespace Terminals
{
    public static class ProtocolHandler
    {
        private const string TRM_REGISTRY = "TRM";

        public static void Register()
        {
            try
            {
                if (IsTrmKeyRegistred())
                    return;

                CreateTrmRegistrySubKey();
            }
            catch (Exception ex)
            {
                //ignore any security errors and such
                Log.Error("Error accessing registry", ex);
            }
        }

        private static void CreateTrmRegistrySubKey()
        {
            using (RegistryKey trmKey = Registry.ClassesRoot.CreateSubKey(TRM_REGISTRY))
            {
                trmKey.SetValue(null, "URL:" + AssemblyInfo.Title + " Protocol");
                trmKey.SetValue("URL Protocol", "");
                using (RegistryKey shellKey = trmKey.CreateSubKey("shell"))
                {
                    shellKey.SetValue(null, "open");
                    using (RegistryKey commandKey = shellKey.CreateSubKey("open\\command"))
                    {
                        commandKey.SetValue(null, Application.ExecutablePath + " /reuse /url:\"%1\"");
                    }
                }
            }
        }

        private static bool IsTrmKeyRegistred()
        {
            RegistryKey trmKey = Registry.ClassesRoot.OpenSubKey(TRM_REGISTRY);
            if (trmKey != null)
            {
                trmKey.Close();
                return true;
            }

            return false;
        }

        public static void Parse(string url, out string server, out int port)
        {
            server = url.Contains("trm://") ? url.Substring(("trm://").Length) : url;

            if (server.EndsWith("/"))
                server = server.TrimEnd('/');
            port = 0;
            string[] serverParams = server.Split(':');
            if (serverParams.Length == 2)
            {
                server = serverParams[0];
                port = Int32.Parse(serverParams[1]);
            }
        }
    }
}