using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Globalization;

namespace Kohl.Framework.Localization
{
	public class LanguageCollector
	{
		private ArrayList m_avalableCutureInfos;

		public LanguageCollector()
		{
			this.m_avalableCutureInfos = this.GetApplicationAvailableCultures();
		}

		public LanguageCollector(CultureInfo defaultCultureInfo) : this()
		{
			if (!this.m_avalableCutureInfos.Contains(defaultCultureInfo))
			{
				this.m_avalableCutureInfos.Add(defaultCultureInfo);
				this.m_avalableCutureInfos.Sort(new LanguageCollector.CultureInfoComparer());
			}
		}

		private Hashtable GetAllCultures()
		{
			CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
			Hashtable hashtables = new Hashtable((int)cultures.Length);
			CultureInfo[] cultureInfoArray = cultures;
			for (int i = 0; i < (int)cultureInfoArray.Length; i++)
			{
				CultureInfo cultureInfo = cultureInfoArray[i];
				hashtables.Add(cultureInfo.Name, cultureInfo);
			}
			return hashtables;
		}

		private ArrayList GetApplicationAvailableCultures()
		{
			ArrayList arrayLists = new ArrayList();
			Hashtable allCultures = this.GetAllCultures();
			string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string[] directories = Directory.GetDirectories(directoryName);
			for (int i = 0; i < (int)directories.Length; i++)
			{
				string str = directories[i];
				CultureInfo item = (CultureInfo)allCultures[Path.GetFileName(str)];
				if (item != null)
				{
					arrayLists.Add(item);
				}
			}
			return arrayLists;
		}

		private string GetDisplayName(CultureInfo cultureInfo, LanguageCollector.LanguageNameDisplay languageNameToDisplay)
		{
			switch (languageNameToDisplay)
			{
				case LanguageCollector.LanguageNameDisplay.DisplayName:
				{
					return cultureInfo.DisplayName;
				}
				case LanguageCollector.LanguageNameDisplay.EnglishName:
				{
					return cultureInfo.EnglishName;
				}
				case LanguageCollector.LanguageNameDisplay.NativeName:
				{
					return cultureInfo.NativeName;
				}
			}
			return "";
		}

		public CultureInfoDisplayItem[] GetLanguages(LanguageCollector.LanguageNameDisplay languageNameToDisplay, out int currentLanguage)
		{
			CultureInfoDisplayItem[] cultureInfoDisplayItem = new CultureInfoDisplayItem[this.m_avalableCutureInfos.Count];
			currentLanguage = -1;
			string name = Thread.CurrentThread.CurrentUICulture.Name;
			string str = Thread.CurrentThread.CurrentUICulture.Parent.Name;
			for (int i = 0; i < this.m_avalableCutureInfos.Count; i++)
			{
				CultureInfo item = (CultureInfo)this.m_avalableCutureInfos[i];
				string displayName = this.GetDisplayName(item, languageNameToDisplay);
				cultureInfoDisplayItem[i] = new CultureInfoDisplayItem(displayName, item);
				if (name == item.Name || currentLanguage == -1 && str == item.Name)
				{
					currentLanguage = i;
				}
			}
			return cultureInfoDisplayItem;
		}

		private class CultureInfoComparer : IComparer
		{
			public CultureInfoComparer()
			{
			}

			public int Compare(CultureInfo cix, CultureInfo ciy)
			{
				return string.Compare(cix.Name, ciy.Name);
			}

			int System.Collections.IComparer.Compare(object x, object y)
			{
				return this.Compare((CultureInfo)x, (CultureInfo)y);
			}
		}

		public enum LanguageNameDisplay
		{
			DisplayName,
			EnglishName,
			NativeName
		}
	}
}