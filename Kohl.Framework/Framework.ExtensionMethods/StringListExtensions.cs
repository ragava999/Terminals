using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Kohl.Framework.ExtensionMethods
{
	public static class StringListExtensions
	{
		public static List<string> GetMissingSourcesInTarget(this List<string> sourceItems, List<string> targetItems)
		{
			return sourceItems.Except<string>(targetItems, StringComparer.CurrentCultureIgnoreCase).ToList<string>();
		}
	}
}