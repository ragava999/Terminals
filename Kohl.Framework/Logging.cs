/*
 * Created by SharpDevelop.
 * User: wrzoko
 * Date: 09.12.2014
 * Time: 13:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace Kohl.Framework.Logging
{
	/// <summary>
	/// Description of Logging.
	/// </summary>
	public static class Log
	{
		public static void Error (Exception ex)
		{
			Error (null, ex);
		}
			
		public static void Error (string message = null, Exception ex = null)
		{
		}
		
		public static void Debug (Exception ex)
		{
			Debug (null, ex);
		}
		
		public static void Debug (string message = null, Exception ex = null)
		{
		}
			
		public static void Info (Exception ex)
		{
			Info (null, ex);
		}
		
		public static void Info (string message = null, Exception ex = null)
		{
		}
		
		public static void Fatal (Exception ex)
		{
			Fatal (null, ex);
		}
		
		public static void Fatal (string message = null, Exception ex = null)
		{
		}
	}
}
