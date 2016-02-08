using System;
using System.IO;
using Kohl.Framework.Info;
using System.Linq;
using System.Diagnostics;

namespace Kohl.Framework.Logging
{
	/// <summary>
	/// Description of Logging.
	/// </summary>
	public static class Log
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(AssemblyInfo.Assembly, AssemblyInfo.Title+".log4net.config");
		
		static Log()
		{
			SetXmlConfig();
		}
		
		public static void SetXmlConfig(string configName = null)
		{
			if (string.IsNullOrEmpty(configName))
			    configName = AssemblyInfo.Title;
			
			log4net.Config.XmlConfigurator.Configure(new FileInfo(Path.Combine(AssemblyInfo.Directory, configName+".log4net.config")));
		}
		
		public static string CurrentLogFolder 
		{
			get
			{
				string file = CurrentLogFile;
				
				if (string.IsNullOrEmpty(file))
					return AssemblyInfo.DirectoryConfigFiles;
				
				return Path.GetDirectoryName(file);
			}
		}
		
		public static string CurrentLogFile {
			get 
			{
				var rootAppender = ((log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository())
                                         .Root.Appenders.OfType<log4net.Appender.FileAppender>()
                                         .FirstOrDefault();

				return rootAppender != null ? rootAppender.File : Path.Combine(Path.Combine(CurrentLogFolder, "Logs"), Kohl.Framework.Info.AssemblyInfo.Title + ".log");
			}
		}
		
		public static void Error (Exception ex)
		{
			Error (null, ex);
		}
		
		public static void Error (string message)
		{
			Error(message, null);
		}
		
		public static void Error (string message = null, Exception ex = null)
		{
			log.Error(message, ex);
		}
		
		public static void Debug (Exception ex)
		{
			Debug (null, ex);
		}
		
		public static void Debug (string message = null, Exception ex = null)
		{
			log.Debug(message, ex);
		}
		
		public static void InsideMethod()
		{
			// get call stack
			StackTrace stackTrace = new StackTrace();
			
			// get calling method name
			Debug("Inside method '" + stackTrace.GetFrame(1).GetMethod().DeclaringType.FullName + "." + stackTrace.GetFrame(1).GetMethod().Name + "'.");
		}
		
		public static void Info (Exception ex)
		{
			Info (null, ex);
		}
		
		public static void Info (string message = null, Exception ex = null)
		{
			log.Info(message, ex);
		}
		
		public static void Warn (Exception ex)
		{
			Warn (null, ex);
		}
		
		public static void Warn (string message = null, Exception ex = null)
		{
			log.Warn(message, ex);
		}
		
		public static void Fatal (Exception ex)
		{
			Fatal (null, ex);
		}
		
		public static void Fatal (string message = null, Exception ex = null)
		{
			log.Fatal(message, ex);
		}
	}
}
