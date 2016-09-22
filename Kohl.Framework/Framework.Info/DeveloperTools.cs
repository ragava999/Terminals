using Kohl.PInvoke;
using System;
using System.IO;

namespace Kohl.Framework.Info
{
    public static class DeveloperTools
    {
        public static TimeSpan CalculateTime(DeveloperTools.Code code, int times = 1)
        {
            if (times < 1)
            {
                times = 1;
            }
            long ticks = (long)0;
            long num = (long)0;
            ticks = DateTime.Now.Ticks;
            for (int i = 0; i < times; i++)
            {
                code();
            }
            num = DateTime.Now.Ticks;
            return TimeSpan.FromTicks(num - ticks);
        }

        public static bool DoesWin32MethodExist(string moduleName, string methodName)
        {
            IntPtr moduleHandle = NativeMethods.GetModuleHandle(moduleName);
            if (moduleHandle == IntPtr.Zero)
            {
                return false;
            }
            return NativeMethods.GetProcAddress(moduleHandle, methodName) != IntPtr.Zero;
        }

        public static TimeSpan GetTimeDifference(DeveloperTools.Code code1, DeveloperTools.Code code2, int times = 1)
        {
            TimeSpan timeSpan = DeveloperTools.CalculateTime(code1, times);
            return timeSpan - DeveloperTools.CalculateTime(code2, times);
        }

        public static bool RequiresMoreTime(DeveloperTools.Code code1, DeveloperTools.Code code2, int times = 1)
        {
            if (DeveloperTools.GetTimeDifference(code1, code2, times).TotalMilliseconds > 0)
            {
                return true;
            }
            return false;
        }

        public static DateTime RetrieveLinkerTimestamp(string location)
        {
            try
            {
                Logging.Log.Debug("Trying to get linker timestamp");

                byte[] numArray = new byte[2048];
                Stream fileStream = null;
                try
                {
                    fileStream = new FileStream(location, FileMode.Open, FileAccess.Read);
                    fileStream.Read(numArray, 0, 2048);
                }
                catch
                {
                    Logging.Log.Debug("Unable to get linker timestamp");
                }
                finally
                {
                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }
                }
                int num = BitConverter.ToInt32(numArray, 60);
                int num1 = BitConverter.ToInt32(numArray, num + 8);
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0);
                dateTime = dateTime.AddSeconds((double)num1);
                TimeSpan utcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(dateTime);
                dateTime = dateTime.AddHours((double)utcOffset.Hours);

                if (dateTime.Equals(new DateTime(1970, 1, 1, 0, 0, 0)) || dateTime.Equals(DateTime.MinValue))
                {
                    return GetAssmeblyDateFallback(location);
                }

                return dateTime;
            }
            catch
            {
                return GetAssmeblyDateFallback(location);
            }
        }

        private static DateTime GetAssmeblyDateFallback(string location)
        {
            Logging.Log.Debug("Unable to retrieve linker timestamp. Falling back to last assembly write time.");

            try
            {
                return new System.IO.FileInfo(location).LastWriteTime;
            }
            catch
            {
                Logging.Log.Error("Unable to retrieve any timestamp from assembly.");
                return DateTime.MinValue;
            }
        }

        public delegate void Code();
    }
}