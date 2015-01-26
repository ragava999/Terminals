using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;
using System.IO.Packaging;
using System.IO;

namespace Rug.Packing.Reflection
{   
    /// <summary>
    /// Interrogates an assembly within a package for its assembly info 
    /// </summary>
    public class AssemblyInterrogator : MarshalByRefObject
    {
        #region Private Members
        
        private Assembly m_Assembly;

        #endregion

        #region Get Assembly Info
        
        /// <summary>
        /// Interrogate an assembly to within a package to get its assembly info
        /// </summary>
        /// <param name="archivePath">the path to the package in the file system</param>
        /// <param name="uri">the uri of the assembly within the package</param>
        /// <param name="info">the info extracted from the assembly</param>
		/// <param name="errorMessage">the user friendly error message</param>
		/// <param name="execptionMessage">the raw exception message</param>
        /// <returns>true if the assembly info could be extracted</returns>
		public bool GetAssemblyInfo(string archivePath, string uri, out AssemblyInfo info, out string errorMessage, out string execptionMessage)
        {
            info = null;
			errorMessage = null; 
			execptionMessage = null; 

			try
			{
				// load the package that contains the assembly to interrogate
				using (Package package = PackageHelperLite.GetPackage(archivePath, false, FileAccess.Read))
				{
					// hook on event to resolve the assembly manualy
					AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

					// try to extract assembly from package
					if (LoadAssembly(package, uri, out m_Assembly, out execptionMessage) == false)
					{
						// could not load the assembly
						errorMessage = string.Format(Strings.Error_AssemblyLoad, uri); 
						return false;
					}

					// get the name of the assembly
					AssemblyName asmName = m_Assembly.GetName();

					info = new AssemblyInfo();

					// check the entry point and see if it takes in arguments
					info.PassArgs = m_Assembly.EntryPoint.GetParameters().Length > 0;

					// iterate through all the attributes on the assembly 
					foreach (object obj in m_Assembly.GetCustomAttributes(false))
					{
						#region Handle each attribute

						if (obj is AssemblyTitleAttribute)
						{
							info.Title = (obj as AssemblyTitleAttribute).Title;
						}
						else if (obj is AssemblyDescriptionAttribute)
						{
							info.Description = (obj as AssemblyDescriptionAttribute).Description;
						}
						else if (obj is AssemblyConfigurationAttribute)
						{
							info.Configuration = (obj as AssemblyConfigurationAttribute).Configuration;
						}
						else if (obj is AssemblyCompanyAttribute)
						{
							info.Company = (obj as AssemblyCompanyAttribute).Company;
						}
						else if (obj is AssemblyProductAttribute)
						{
							info.Product = (obj as AssemblyProductAttribute).Product;
						}
						else if (obj is AssemblyCopyrightAttribute)
						{
							info.Copyright = (obj as AssemblyCopyrightAttribute).Copyright;
						}
						else if (obj is AssemblyTrademarkAttribute)
						{
							info.Trademark = (obj as AssemblyTrademarkAttribute).Trademark;
						}
						else if (obj is AssemblyCultureAttribute)
						{
							info.Culture = (obj as AssemblyCultureAttribute).Culture;
						}
						else if (obj is AssemblyVersionAttribute)
						{
							info.Version = (obj as AssemblyVersionAttribute).Version;
						}
						else if (obj is AssemblyFileVersionAttribute)
						{
							info.FileVersion = (obj as AssemblyFileVersionAttribute).Version;
						}

						#endregion
					}

					// if we have a version then copy it accross
					if (IsNullOrEmpty(info.Version))
					{
						info.Version = asmName.Version.ToString();
					}

					// we got the info ok
					return true;
				}
			}
			catch (TypeLoadException ex)
			{
				// the assembly info could not be obtained
				errorMessage = string.Format(Strings.Error_FrameworkVersion, uri);
				execptionMessage = ex.Message; 

				return false;
			}
			catch (Exception ex)
			{
				// the assembly info could not be obtained
				errorMessage = string.Format(Strings.Error_Unknown, uri);
				execptionMessage = ex.Message; 

				return false;
			}            
        }

        #endregion

        #region Load Assembly
        
        /// <summary>
        /// Load an assembly from within a package
        /// </summary>
        /// <param name="package">the package that contains the assembly to load</param>
        /// <param name="uri">the uri of the assembly within the package</param>
        /// <param name="Assembly">the resulting assembly</param>
        /// <returns>true if the assembly could be loaded</returns>
		internal static bool LoadAssembly(Package package, string uri, out Assembly Assembly, out string execptionMessage)
        {
            Assembly = null; 

            try
            {
                byte[] bytes;

                // create the uri path to the file
                Uri pathUri = new Uri(uri, UriKind.Relative);

                // get a stream for the file
                using (Stream stream = package.GetPart(pathUri).GetStream())
                {
                    // create a buffer for the data
                    bytes = new byte[(int)stream.Length];

                    // read the assemblys bytes into the buffer 
                    stream.Read(bytes, 0, bytes.Length);
                }

                // load the assembly into the current app domain from its bytes
                Assembly = AppDomain.CurrentDomain.Load(bytes);

                if (Assembly == null)
                {
                    // could not load the assembly
					execptionMessage = Strings.Error_AssemblyLoad_NoException;
                    return false;
                }
                else
                { 
                    // loaded ok
					execptionMessage = null; 
                    return true;
                }
            }
            catch (Exception ex)
            {
                // faile to load the assembly
				execptionMessage = ex.Message; 
                return false;
            }
        }

        #endregion

        #region Private Methods and Event Handler
        
        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            // Resolve the assembly manualy
            if (m_Assembly.FullName == args.Name)
            { 
                return m_Assembly;
            }

            return null; 
        }

        private bool IsNullOrEmpty(string str)
        {
            if (str == null)
                return true;

            if (str.Trim().Length == 0)
                return true;

            return false;
        }

        #endregion
    }
}