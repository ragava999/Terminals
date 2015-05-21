using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace Kohl.Framework.Info
{
	public static class AssemblyInfo
	{
		private static System.Reflection.Assembly assembly;

		public static string AboutText
		{
			get
			{
				string titleVersion = AssemblyInfo.TitleVersion;
				DateTime buildDate = AssemblyInfo.BuildDate;
				return string.Format("{0} ({1})", titleVersion, buildDate.ToShortDateString());
			}
		}

		public static System.Reflection.Assembly Assembly
		{
			get
			{
				return AssemblyInfo.assembly;
			}
			set
			{
				AssemblyInfo.assembly = value;
			}
		}

		public static string Author
		{
			get
			{
				return ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(AssemblyInfo.assembly, typeof(AssemblyCompanyAttribute))).Company;
			}
		}

		public static DateTime BuildDate
		{
			get
			{
				return DeveloperTools.RetrieveLinkerTimestamp(AssemblyInfo.Location);
			}
		}

		public static string CommandLine
		{
			get
			{
				string commandLine = Environment.CommandLine;
				if (string.IsNullOrEmpty(commandLine))
				{
					return null;
				}
				if (commandLine.StartsWith("\""))
				{
					commandLine = commandLine.Remove(0, 1);
				}
				if (commandLine.EndsWith("\""))
				{
					commandLine = commandLine.Remove(commandLine.Length - 1, 1);
				}
				if (commandLine.EndsWith("\" "))
				{
					commandLine = commandLine.Remove(commandLine.Length - 2, 1);
				}
				commandLine = commandLine.Replace(AssemblyInfo.Location, "").Replace(AssemblyInfo.LocationVsHost, "").TrimEnd(new char[0]);
				if (string.IsNullOrEmpty(commandLine))
				{
					return null;
				}
				return commandLine;
			}
		}

		public static string Description
		{
			get
			{
				return ((AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(AssemblyInfo.assembly, typeof(AssemblyDescriptionAttribute))).Description;
			}
		}

		public static string Directory
		{
			get
			{
				if (string.IsNullOrEmpty(AssemblyInfo.Location))
				{
					return null;
				}
				if (LicenseManager.UsageMode != LicenseUsageMode.Runtime)
				{
					return null;
				}
				return Path.GetDirectoryName(AssemblyInfo.Location);
			}
		}

		public static bool Is64BitProcess
		{
			get
			{
				if (IntPtr.Size == 4)
				{
					return false;
				}
				return true;
			}
		}

		public static string Location
		{
			get
			{
				if (!string.IsNullOrEmpty(AssemblyInfo.assembly.Location))
				{
					return AssemblyInfo.assembly.Location;
				}
				return (new Uri(AssemblyInfo.assembly.CodeBase)).LocalPath;
			}
		}

		public static string LocationVsHost
		{
			get
			{
				if (string.IsNullOrEmpty(AssemblyInfo.Location))
				{
					return null;
				}
				int length = (new FileInfo(AssemblyInfo.Location)).Extension.Length;
				return string.Concat(AssemblyInfo.Location.Remove(AssemblyInfo.Location.Length - length, length), ".vshost.exe");
			}
		}

		public static string StartupPath
		{
			get
			{
				string baseDirectory;
				try
				{
					baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
				}
				catch
				{
					baseDirectory = null;
				}
				return baseDirectory;
			}
		}

		public static string Title
		{
			get
			{
				return ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(AssemblyInfo.assembly, typeof(AssemblyTitleAttribute))).Title;
			}
		}

		public static string TitleVersion
		{
			get
			{
				return string.Format("{0} {1}", AssemblyInfo.Title, AssemblyInfo.Version.ToString());
			}
		}

		public static string Url
		{
			get
			{
				return ((AssemblyTrademarkAttribute)Attribute.GetCustomAttribute(AssemblyInfo.assembly, typeof(AssemblyTrademarkAttribute))).Trademark;
			}
		}

		public static System.Version Version
		{
			get
			{
				return AssemblyInfo.assembly.GetName().Version;
			}
		}

		static AssemblyInfo()
		{
		}

		public static void SetAssembly(Type type)
		{
			AssemblyInfo.assembly = System.Reflection.Assembly.GetAssembly(type);
		}
	}
}