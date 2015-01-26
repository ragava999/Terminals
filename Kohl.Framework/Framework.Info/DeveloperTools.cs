using System;
using System.IO;
using System.Runtime.InteropServices;

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
			IntPtr moduleHandle = DeveloperTools.GetModuleHandle(moduleName);
			if (moduleHandle == IntPtr.Zero)
			{
				return false;
			}
			return DeveloperTools.GetProcAddress(moduleHandle, methodName) != IntPtr.Zero;
		}

		[DllImport("kernel32.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		private static extern IntPtr GetModuleHandle(string moduleName);

		[DllImport("kernel32.dll", CharSet=CharSet.Auto, ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr GetProcAddress(IntPtr module, string procName);

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
			byte[] numArray = new byte[2048];
			Stream fileStream = null;
			try
			{
				fileStream = new FileStream(location, FileMode.Open, FileAccess.Read);
				fileStream.Read(numArray, 0, 2048);
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
			return dateTime;
		}

		public delegate void Code();
	}
}