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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(AssemblyInfo.Assembly, AssemblyInfo.Title + ".log4net.config");

        static Log()
        {
            SetXmlConfig();
        }

        private static string Log4NetConfig
        {
            get
            {
                return Path.Combine(AssemblyInfo.Directory, ConfigName + ".log4net.config");
            }
        }

        public static bool IsDebugLogLevel()
        {
            return log.IsDebugEnabled;
        }

        public static void SetLogLevel(bool useDebug)
        {
            string text = "";

            try
            {
                using (FileStream fileStream = new FileStream(Log4NetConfig, FileMode.Open, FileAccess.Read))
                {
                    StreamReader streamReader = new StreamReader(fileStream, System.Text.Encoding.UTF8, false);
                    text = streamReader.ReadToEnd();
                }
            }
            catch
            {
                Error("Unable to change log level.");
                return;
            }

            if (useDebug)
                text = text.Replace("value=\"INFO\"", "value=\"DEBUG\"");
            else
                text = text.Replace("value=\"DEBUG\"", "value=\"INFO\"");

            try
            {
                using (FileStream fileStream = new FileStream(Log4NetConfig, FileMode.Truncate, FileAccess.Write))
                {
                    StreamWriter streamWriter = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
                    streamWriter.Write(text);
                }
            }
            catch
            {
                Error("Unable to change log level.");
                return;
            }

            try
            {
                SetXmlConfig(ConfigName);

                Info("Changed log level to " + (useDebug ? "debug" : "information") + " level.");
            }
            catch
            {
                Error("Unable to change log level.");
            }
        }

        private static string ConfigName = AssemblyInfo.Title;

        public static void SetXmlConfig(string configName = null)
        {
            if (string.IsNullOrEmpty(ConfigName = configName))
                ConfigName = AssemblyInfo.Title;

            log4net.Config.XmlConfigurator.Configure(new FileInfo(Log4NetConfig));

            // If the path has only been configure for Windows in the Log4Net config ->
            // then fix it end set it for unix systems
            if (MachineInfo.IsUnixOrMac)
            {
                if (GetRootAppenderFileName != CurrentLogFile)
                {
                    GetRootAppender.File = CurrentLogFile;
                    GetRootAppender.ActivateOptions();
                }
            }
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

        private static log4net.Appender.FileAppender GetRootAppender
        {
            get
            {
                return ((log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository())
                                         .Root.Appenders.OfType<log4net.Appender.FileAppender>()
                                         .FirstOrDefault();
            }
        }

        private static string GetRootAppenderFileName
        {
            get
            {
                return (GetRootAppender != null ? GetRootAppender.File : Path.Combine(Path.Combine(CurrentLogFolder, "Logs"), AssemblyInfo.Title + ".log"));
            }
        }

        public static string CurrentLogFile
        {
            get
            {
                string file = GetRootAppenderFileName;

                if (MachineInfo.IsUnixOrMac && file.Contains("\\\\"))
                    file = file.Replace("\\\\", Path.DirectorySeparatorChar.ToString());

                return file;
            }
        }

        public static void Error(Exception ex)
        {
            Error(null, ex);
        }

        public static void Error(string message)
        {
            Error(message, null);
        }

        public static void Error(string message = null, Exception ex = null)
        {
            log.Error(message, ex);
        }

        public static void Debug(Exception ex)
        {
            Debug(null, ex);
        }

        public static void Debug(string message = null, Exception ex = null)
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

        public static void Info(Exception ex)
        {
            Info(null, ex);
        }

        public static void Info(string message = null, Exception ex = null)
        {
            log.Info(message, ex);
        }

        public static void Warn(Exception ex)
        {
            Warn(null, ex);
        }

        public static void Warn(string message = null, Exception ex = null)
        {
            log.Warn(message, ex);
        }

        public static void Fatal(Exception ex)
        {
            Fatal(null, ex);
        }

        public static void Fatal(string message = null, Exception ex = null)
        {
            log.Fatal(message, ex);
        }
    }
}