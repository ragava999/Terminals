using System;
using System.Globalization;
using System.Drawing;

namespace Kohl.Framework.Converters
{
    /// <summary>
    /// Converts a color to a string and vice versa
    /// </summary>
    /// <example>
    /// "FFFFFFFF (White)"
    /// </example>
	public static class ColorParser
	{
        public static Color FromString(this string source, Color? fallbackColor = null)
        {
            Int32 sizeIndex = source.IndexOf(" (", 0);
            
            // We are dealing with a known color
            if (sizeIndex > 2)
            {
            	string colorName = source.Substring(sizeIndex, source.Length - sizeIndex-1);
            	
            	try
            	{
            		return Color.FromName(colorName.Substring(2,colorName.Length -2));
            	}
            	catch
            	{
            		Logging.Log.Warn("Color " + source + " is not a known color");
            	}
            }
            
            try
            {
            	return ColorTranslator.FromHtml("#" + source);	
            }
            catch
            {
            	if (!fallbackColor.HasValue)
            	{
            		Logging.Log.Fatal("Color " + source + " can't be parsed. Black will be used as a fallback.");
            		return Color.Black;
            	}
            	
            	return fallbackColor.Value;
            }
        }
        
        
        public static string ToString(this Color source)
        {
        	if (source.IsKnownColor)
        		return source.ToString() + " (" + source.Name + ")";
        	
        	return source.ToString();
        }
	}
}
