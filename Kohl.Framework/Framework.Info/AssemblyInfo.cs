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

		public static string RegistryPath
		{
			get
			{
				return "HKEY_CURRENT_USER\\Software\\" + AssemblyInfo.Author + "\\" + AssemblyInfo.Title;
			}
		}
		
		private static string ProgramFilesx86()
		{
		    if( 8 == IntPtr.Size 
		        || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
		    {
		        return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
		    }
		
		    return Environment.GetEnvironmentVariable("ProgramFiles");
		}
		
		public static string DirectoryConfigFiles
		{
			get
			{
				if (Directory.ToLower().Contains(ProgramFilesx86().ToLower()) || Directory.ToLower().Contains(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles).ToLower()) || !Directory.CanWriteToFolder())
					return Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Author), Title);
			
				return Directory;
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

		/// <summary>
		/// Get the fully qualified original local directory path.
		/// </summary>
		/// <example>C:\Kohl\Terminal\</example>
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

		/// <summary>
		/// Get the fully qualified original local location of the currently executing assembly.
		/// </summary>
		/// <example>C:\Kohl\Terminal\Terminals.exe</example>
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

		/// <summary>
		/// Get the fully qualified original local location of the currently executing debug assembly.
		/// </summary>
		/// <example>C:\Kohl\Terminal\Terminals.vshost.exe</example>
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

		/*
		// Covered by Directory-Property
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
		*/
		
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