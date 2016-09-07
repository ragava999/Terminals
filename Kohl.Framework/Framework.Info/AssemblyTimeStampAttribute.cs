using System;
using System.Reflection;
using System.Globalization;

namespace Kohl.Framework.Info
{
	[AttributeUsage(AttributeTargets.Assembly)]
	public class AssemblyTimeStampAttribute : Attribute
	{
		DateTime dateTime;

		public AssemblyTimeStampAttribute(string dateTime)
		{ 
			if (string.IsNullOrEmpty(dateTime))
			{
				dateTime = DateTime.MinValue.ToString ();
				return;
			}

			DateTime.TryParseExact (dateTime, new CultureInfo ("de-AT").DateTimeFormat.ShortDatePattern + " " + new CultureInfo ("de-AT").DateTimeFormat.LongTimePattern, new CultureInfo ("de-AT"), DateTimeStyles.NoCurrentDateDefault, out this.dateTime);
		}

		public DateTime DateTime
		{
			get
			{
				return dateTime;
			}
		}
	}
}

