using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.IO;

namespace System
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

		
		public static bool CanWriteToFolder(this string path)
        {
            try
            {
            	string testFile = Path.Combine(path, "WriteAccessCheck.txt");
            	
                // Test to make sure that the current user has write access to the current directory.
                using (StreamWriter sw = File.AppendText(testFile))
                {
                }
                
                File.Delete(testFile);
                
                return true;
            }
            catch (Exception ex)
            {
                Kohl.Framework.Logging.Log.Warn("Unable to write/delete files in \"" + path + "\".", ex);
                return false;
            }
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
		
		/// <summary>
		/// Creates a relative path from one file or folder to another.
		/// </summary>
		/// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
		/// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
		/// <returns>The relative path from the start directory to the end path or <c>toPath</c> if the paths are not related.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="UriFormatException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public static String GetRelativePath(this string fromPath, string toPath = null)
		{
		    if (string.IsNullOrEmpty(fromPath))
		    	return null;
		    	
		    if (string.IsNullOrEmpty(toPath))
		    	toPath = Kohl.Framework.Info.AssemblyInfo.Directory;
		
		    Uri fromUri = new Uri(fromPath);
		    Uri toUri = new Uri(toPath);
		
		    if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.
		
		    Uri relativeUri = fromUri.MakeRelativeUri(toUri);
		    String relativePath = Uri.UnescapeDataString(relativeUri.ToString());
		
		    if (toUri.Scheme.ToUpperInvariant() == "FILE")
		    {
		        relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
		    }
		
		    return relativePath;
		}
		
		public static string NormalizePath(this string path, string rootPath = null)
		{
			if (string.IsNullOrEmpty(rootPath))
			    rootPath = Kohl.Framework.Info.AssemblyInfo.Directory;
			
			Uri uri;
			if (!Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out uri))
			{
				// Path is not a valid URI
				Kohl.Framework.Logging.Log.Warn("\"" + path + "\" is not a valid URI.");
				return path;
			}
			
			if (!uri.IsAbsoluteUri || Path.IsPathRooted(path))
			{
				// Path is a relative URI
				path = Path.Combine(rootPath, path);
			}
			else if (uri.IsFile)
			{
				if (uri.IsUnc)
				{
					// Path is a UNC path
				}
				else
				{
					// Path is a file URI
				}
			}
			else
			{
				// Path is an absolute URI
			}
			
			return path;
		}
	}
}