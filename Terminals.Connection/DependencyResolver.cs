namespace Terminals.Connection
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Description of DependencyResolver.
	/// </summary>
	public static class DependencyResolver
	{
		private static readonly object locker = new object();
		
		public static Assembly ResolveAssembly(object sender, System.ResolveEventArgs args)
		{
			if (args == null || string.IsNullOrEmpty(args.Name))
			{
				Kohl.Framework.Logging.Log.Warn("Assembly to be loaded can't be reflected.");
				return null;
			}
			
			lock (locker)
			{
				AssemblyName askedAssembly = new AssemblyName(args.Name);
			
				string[] fields = args.Name.Split(',');
				string name = fields[0];
				string culture = fields[2];
				// failing to ignore queries for satellite resource assemblies or using [assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.MainAssembly)] 
				// in AssemblyInfo.cs will crash the program on non en-US based system cultures.
				if (name.EndsWith(".resources") && !culture.EndsWith("neutral")) return null;
			
				// Serialization assemblies - can be ignored - not yet loaded
				if (name == "mscorlib.XmlSerializers" || name == "Terminals.Configuration.XmlSerializers")
					return null;
				
				/* find all dlls from all directories */
				string[] dlls = System.IO.Directory.GetFiles(Kohl.Framework.Info.AssemblyInfo.Directory, "*.dll", System.IO.SearchOption.AllDirectories);
			
				for (int i = 0; i < dlls.Length; i++)
				{
					try
					{
						string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(dlls[i]);
			
						// Load only the file specified in the function argument - if the filename received from Directory::GetFiles() doesn't match the
						// argument check the next dll until we find the one needed to be loaded dynamically
						if (fileNameWithoutExtension != args.Name.Split(',')[0])
							continue;
			
						// Load the file into the application's working set
						//Assembly asm = Assembly.LoadFile(dlls[i]);
						Assembly asm = Assembly.LoadFrom(dlls[i]);

						// The dll has been found - return the assembly
						if (asm.FullName == args.Name)
						{
							Kohl.Framework.Logging.Log.Info("Dynamically resolved assembly '" + args.Name + "'.");
							return asm;
						}
					}
					catch (Exception ex)
					{
						Kohl.Framework.Logging.Log.Error("Error loading " + args.Name, ex);
					}
				}
			
				string requestingAssembly = args.RequestingAssembly == null ? "-" : args.RequestingAssembly.FullName;
				
				Kohl.Framework.Logging.Log.Debug("Error resolving " + args.Name + ", requesting assembly: " + requestingAssembly);
				
				return null;
			}
		}
	}
}
