using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Packaging;
using System.Reflection;
using System.IO;

namespace Rug.Packing.Reflection
{
    /// <summary>
    /// Light weight helper for loading packages
    /// </summary>
    internal static class PackageHelperLite    
    {
        /// <summary>
        /// Loads a package from the file system or creates one if required
        /// </summary>
        /// <param name="path">the path of the package in the file system</param>
        /// <param name="create">should a new package be created if one does not exist at the path provided</param>
        /// <param name="access">file access mode</param>
        /// <returns>the loaded package</returns>
        public static Package GetPackage(string path, bool create, FileAccess access)
        {
            string resolvedPath = path;

            Package package = null;

            FileInfo info = new FileInfo(resolvedPath);

            if (info.Exists)
            {
                package = Package.Open(info.FullName, FileMode.Open, access);
            }
            else if (create)
            {
                package = ZipPackage.Open(info.FullName, FileMode.CreateNew, access); 
            }
        
            return package;
        }
    }
}
