namespace Terminals.Plugins.Text
{
	using System;
	using System.IO;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.Text.RegularExpressions;
	using System.Collections.Generic;

	/// <summary>
	/// Description of ImageToBase64.
	/// </summary>
	public static class ImageTools
	{
		static IEnumerable<string> GetImageLinks(string htmlSource)
		{
			const string pattern = @"<img\b[^\<\>]+?\bsrc\s*=\s*[""'](?<L>.+?)[""'][^\<\>]*?\>";

			foreach (Match match in Regex.Matches(htmlSource, pattern, RegexOptions.IgnoreCase))
		    {
		        var imageLink = match.Groups["L"].Value;
		        yield return imageLink;
		    }
		}
		
		public static string ParseHtmlLinks(string htmlSource)
		{
			IEnumerable<string> links = GetImageLinks(htmlSource);
			
			foreach (string link in links)
			{
				string imageFileName = link;
				
				if (imageFileName.StartsWith("file:///", StringComparison.InvariantCultureIgnoreCase))
					imageFileName = imageFileName.ToLower().Replace("file:///", "").Replace("/", @"\");

				if (File.Exists(imageFileName))
				{
					Image image = Image.FromFile(imageFileName);
					
					htmlSource = htmlSource.Replace(link, "data:image/png;base64," + ImageToBase64(image, image.RawFormat));
				}
			}
			
			return htmlSource;
		}
		
		
		public static string ImageToBase64(Image image, ImageFormat format)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				// Convert Image to byte[]
				image.Save(ms, format);
				byte[] imageBytes = ms.ToArray();
				
				// Convert byte[] to Base64 String
				string base64String = Convert.ToBase64String(imageBytes);
				return base64String;
			}
		}

		public static Image Base64ToImage(string base64String)
		{
			// Convert Base64 String to byte[]
			byte[] imageBytes = Convert.FromBase64String(base64String);
			using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
			{
				// Convert byte[] to Image
				ms.Write(imageBytes, 0, imageBytes.Length);
				Image image = Image.FromStream(ms, true);
				return image;
			}
    	} 
	}
}
