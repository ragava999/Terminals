using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Kohl.Framework.ExtensionMethods
{
	public static class StringExtensions
	{
		private static string ConvertUnicodeToPunycodeDomain(Match match)
		{
			string str;
			IdnMapping idnMapping = new IdnMapping();
			string value = match.Groups[2].Value;
			try
			{
				value = idnMapping.GetAscii(value);
				return string.Concat(match.Groups[1].Value, value);
			}
			catch
			{
				str = null;
			}
			return str;
		}

		public static bool IsEmail(this string email)
		{
			bool flag;
			if (string.IsNullOrEmpty(email))
			{
				return false;
			}
			try
			{
				email = Regex.Replace(email, "(@)(.+)$", new MatchEvaluator(StringExtensions.ConvertUnicodeToPunycodeDomain), RegexOptions.None);
			}
			catch
			{
				flag = false;
				return flag;
			}
			try
			{
				flag = Regex.IsMatch(email, "^(?(\")(\"[^\"]+?\"@)|(([0-9a-z]((\\.(?!\\.))|[-!#\\$%&'\\*\\+/=\\?\\^`\\{\\}\\|~\\w])*)(?<=[0-9a-z])@))(?(\\[)(\\[(\\d{1,3}\\.){3}\\d{1,3}\\])|(([0-9a-z][-\\w]*[0-9a-z]*\\.)+[a-z0-9]{2,17}))$", RegexOptions.IgnoreCase);
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		public static bool IsPhoneNumber(this string phone)
		{
			bool flag;
			if (phone.Contains("(") && !phone.Contains(")"))
			{
				return false;
			}
			if (phone.Contains(")") && !phone.Contains("("))
			{
				return false;
			}
			try
			{
				flag = Regex.IsMatch(phone, "^(\\(?\\+?[0-9]*\\)?)?[0-9\\- \\(\\)]{10,}?([ext]+?[0-9]{3,})?$");
			}
			catch
			{
				flag = false;
			}
			return flag;
		}
	}
}