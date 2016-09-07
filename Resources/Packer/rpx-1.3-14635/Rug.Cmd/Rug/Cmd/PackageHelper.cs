namespace Rug.Cmd
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Packaging;
    using System.Windows.Forms;

    public static class PackageHelper
    {
        private static object m_Lock = new object();
        private static Dictionary<string, Package> m_Packages = new Dictionary<string, Package>();

        static PackageHelper()
        {
            Application.ApplicationExit += new EventHandler(PackageHelper.Application_ApplicationExit);
        }

        public static void AddFileToPackage(Package package, string uri, string filePath)
        {
            FileInfo info = new FileInfo(filePath);
            string extension = info.Extension;
            string contentType = "text/xml";
            if (extension.Equals(".jpg", StringComparison.InvariantCultureIgnoreCase))
            {
                contentType = "image/jpeg";
            }
            else if (extension.Equals(".bmp", StringComparison.InvariantCultureIgnoreCase))
            {
                contentType = "image/bmp";
            }
            else if (extension.Equals(".png", StringComparison.InvariantCultureIgnoreCase))
            {
                contentType = "image/png";
            }
            else if (extension.Equals(".xml", StringComparison.InvariantCultureIgnoreCase))
            {
                contentType = "text/xml";
            }
            AddFileToPackage(package, uri, filePath, contentType);
        }

        public static void AddFileToPackage(Package package, string uri, string filePath, string contentType)
        {
            FileInfo info = new FileInfo(filePath);
            uri = MakeUriSafe(uri);
            if (!info.Exists)
            {
                throw new FileNotFoundException(Strings.Package_FileCouldNotBeAdded, filePath);
            }
            Uri partUri = new Uri(uri, UriKind.Relative);
            string extension = info.Extension;
            if (package.PartExists(partUri))
            {
                package.DeletePart(partUri);
            }
            PackagePart part = package.CreatePart(partUri, contentType, CompressionOption.Maximum);
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (Stream stream2 = part.GetStream())
                {
                    CopyStream(stream, stream2);
                }
                stream.Close();
            }
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            Dispose();
        }

        public static void CopyStream(Stream source, Stream target)
        {
            byte[] buffer = new byte[0x1000];
            int count = 0;
            while ((count = source.Read(buffer, 0, 0x1000)) > 0)
            {
                target.Write(buffer, 0, count);
            }
        }

        public static void Dispose()
        {
            lock (m_Lock)
            {
                foreach (KeyValuePair<string, Package> pair in m_Packages)
                {
                    pair.Value.Close();
                }
                m_Packages.Clear();
            }
        }

        public static Package GetEmbeddedPackage(System.Type type, string path)
        {
            lock (m_Lock)
            {
                return Package.Open(type.Assembly.GetManifestResourceStream(type, path), FileMode.Open, FileAccess.Read);
            }
        }

        public static Package GetPackage(string path, bool create, FileAccess access)
        {
            string key = ResolvePath(path);
            Package package = null;
            if (m_Packages.TryGetValue(key, out package))
            {
                return package;
            }
            if (key.StartsWith("~/"))
            {
                throw new Exception(string.Format(Strings.Package_ResolveError, key));
            }
            lock (m_Lock)
            {
                FileInfo info = new FileInfo(key);
                if (info.Exists)
                {
                    package = Package.Open(info.FullName, FileMode.Open, access);
                    m_Packages.Add(key, package);
                }
                else if (create)
                {
                    package = Package.Open(info.FullName, FileMode.CreateNew, access);
                    m_Packages.Add(key, package);
                }
                return package;
            }
        }

        public static string MakeUriSafe(string path)
        {
            if (path == null)
            {
                return "";
            }
            path = path.Replace('\\', '/');
            path = path.Replace(' ', '-');
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }
            return path;
        }

        public static void ReleasePackage(Package package)
        {
            lock (m_Lock)
            {
                string key = null;
                foreach (KeyValuePair<string, Package> pair in m_Packages)
                {
                    if (pair.Value == package)
                    {
                        key = pair.Key;
                        break;
                    }
                }
                if (key != null)
                {
                    package.Close();
                    m_Packages.Remove(key);
                }
            }
        }

        public static void ReleasePackage(string path)
        {
            lock (m_Lock)
            {
                string key = ResolvePath(path);
                Package package = null;
                if (m_Packages.TryGetValue(key, out package))
                {
                    package.Close();
                    m_Packages.Remove(key);
                }
            }
        }

        public static void RemoveFileFromPackage(Package package, string uri)
        {
            Uri partUri = new Uri(uri, UriKind.Relative);
            if (package.PartExists(partUri))
            {
                package.DeletePart(partUri);
            }
        }

        public static void RenamePath(Package package, string oldUri, string newUri)
        {
            Uri partUri = new Uri(oldUri, UriKind.Relative);
            Uri uri2 = new Uri(newUri, UriKind.Relative);
            if (package.PartExists(partUri))
            {
                PackagePart part = package.GetPart(partUri);
                PackagePart part2 = package.CreatePart(uri2, part.ContentType, part.CompressionOption);
                using (Stream stream = part.GetStream())
                {
                    using (Stream stream2 = part2.GetStream())
                    {
                        CopyStream(stream, stream2);
                    }
                }
                package.DeletePart(partUri);
            }
        }

        public static string ResolvePath(string path)
        {
            return path;
        }
    }
}

