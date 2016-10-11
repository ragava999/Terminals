using Kohl.PInvoke;
using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Security.Principal;

namespace Kohl.Framework.Info
{
    public static class UserInfo
    {
        public static bool IsAdministrator
        {
            get
            {
                return (new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public static string UserDomain
        {
            get
            {
                return Environment.UserDomainName;
            }
        }

        public static string UserName
        {
            get
            {
                // Get the user's display name on a MacOSX
                if (MachineInfo.IsMac)
                {
                    return System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("osascript", "-e \"long user name of (system info)\"") { RedirectStandardOutput = true, UseShellExecute = false }).StandardOutput.ReadLine().Trim();
                }
                // Get the user's AD display name for Windows
                else if (!MachineInfo.IsUnix && !string.IsNullOrWhiteSpace(UserNameAlias))
                {
                    // set up domain context
                    using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain))
                    {
                        // find user by it's SAM account name
                        UserPrincipal user = UserPrincipal.FindByIdentity(ctx, UserNameAlias);

                        if (user != null)
                        {
                            return user.DisplayName;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

                return null;
            }
        }

        public static string UserNameAlias
        {
            get
            {
                return Environment.UserName;
            }
        }

        public static string UserSid
        {
            get
            {
                if (MachineInfo.IsUnixOrMac)
                    return UnixNativeMethods.getuid().ToString();

                return WindowsIdentity.GetCurrent().User.ToString();
            }
        }

        private static string GetUserName()
        {
            string str;
            string str1;
            try
            {
                DirectoryEntry directoryEntry = new DirectoryEntry(string.Concat("WinNT://", Environment.UserDomainName, "/", UserInfo.UserNameAlias));
                if (directoryEntry.Properties["fullName"].Value.ToString() == UserInfo.UserNameAlias)
                {
                    str1 = (string.Concat(directoryEntry.Properties["sn"].Value.ToString(), ", ", directoryEntry.Properties["sn"].Value.ToString()) == UserInfo.UserNameAlias ? directoryEntry.Properties["displayName"].Value.ToString() : directoryEntry.Properties["displayName"].Value.ToString());
                }
                else
                {
                    str1 = directoryEntry.Properties["fullName"].Value.ToString();
                }
                str = str1;
            }
            catch
            {
                str = null;
            }
            return str;
        }
    }
}