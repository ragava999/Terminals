using System;
using System.DirectoryServices;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

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
				if (MachineInfo.IsMac)
				{
					return System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("osascript", "-e \"long user name of (system info)\"") { RedirectStandardOutput = true, UseShellExecute = false }).StandardOutput.ReadLine().Trim();
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

		[DllImport ("libc")]
		private static extern uint getuid ();

		public static string UserSid
		{
			get
			{
				if (MachineInfo.IsUnixOrMac)
					return getuid().ToString();

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

		[DllImport("secur32.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		private static extern int GetUserNameEx(UserInfo.EXTENDED_NAME_FORMAT nameFormat, StringBuilder userName, ref uint userNameSize);

		private enum EXTENDED_NAME_FORMAT
		{
			NameUnknown = 0,
			NameFullyQualifiedDN = 1,
			NameSamCompatible = 2,
			NameDisplay = 3,
			NameUniqueId = 6,
			NameCanonical = 7,
			NameUserPrincipal = 8,
			NameCanonicalEx = 9,
			NameServicePrincipal = 10,
			NameDnsDomain = 12
		}
	}
}