/*
 * Created by SharpDevelop.
 * User: wzhmtl
 * Date: 16.07.2012
 * Time: 16:11
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Reflection;
using System.IO;
using System.IO.Packaging;

namespace test
{
	class Program
	{
		public static void Main(string[] args)
		{
		    try
		    {
		    	Assembly a = Assembly.LoadFile(@"C:\Users\wzhmtl\Desktop\Rpx 1.2\3.5\rpx.exe");
		        Stream manifestResourceStream = a.GetManifestResourceStream("a");
		        byte[] buffer = new byte[manifestResourceStream.Length];
		        manifestResourceStream.Read(buffer, 0, buffer.Length);
		        Array.Reverse(buffer);
		        using (Stream stream2 = new MemoryStream(buffer))
		        {
		            using (Package package = Package.Open(stream2, FileMode.Open, FileAccess.Read))
		            {
		                L(package, "/Ionic.Zip.Reduced.dll");
		                L(package, "/Rug.Cmd.dll");
		                L(package, "/Rpx.exe").EntryPoint.Invoke(null, new object[] { args });
		            }
		        }
		    }
		    catch (Exception)
		    {
		    }

		}
		
		public static bool ByteArrayToFile(string _FileName, byte[] _ByteArray)
	    {
	        try
	        {
	            // Open file for reading
	            System.IO.FileStream _FileStream = new System.IO.FileStream(_FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
	
	            // Writes a block of bytes to this stream using data from a byte array.
	            _FileStream.Write(_ByteArray, 0, _ByteArray.Length);
	
	            // close file stream
	            _FileStream.Close();
	
	            return true;
	        }
	        catch (Exception _Exception)
	        {
	            // Error
	            Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
	        }
	
	    // error occured, return false
	        return false;
	    }

		
		private static Assembly L(Package p, string u)
		{
		    byte[] buffer;
		    Uri partUri = new Uri(u, UriKind.Relative);
		    using (Stream stream = p.GetPart(partUri).GetStream())
		    {
		        buffer = new byte[(int) stream.Length];
		        stream.Read(buffer, 0, buffer.Length);
		    }
		    
		    ByteArrayToFile(@"C:\Kohl\"+u, buffer);
		    
		    Assembly assembly = Assembly.Load(buffer);
		    if (assembly == null)
		    {
		        throw new ArgumentException("Unable to load assembly: " + u);
		    }
		    return assembly;
		}
	}
}