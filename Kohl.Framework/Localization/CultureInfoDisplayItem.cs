using System;

namespace Kohl.Framework.Localization
{
	public class CultureInfoDisplayItem
	{
		public string DisplayName;

		public readonly System.Globalization.CultureInfo CultureInfo;

		public CultureInfoDisplayItem(string displayName, System.Globalization.CultureInfo cultureInfo)
		{
			this.DisplayName = displayName;
			this.CultureInfo = cultureInfo;
		}

		public override string ToString()
		{
			return this.DisplayName;
		}
	}
}