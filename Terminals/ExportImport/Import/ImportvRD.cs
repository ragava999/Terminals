using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Kohl.Framework.Localization;
using Kohl.Framework.WinForms;
using Terminals.Configuration.Files.Credentials;
using Terminals.Configuration.Files.Main.Favorites;

namespace Terminals.ExportImport.Import
{
    public class ImportvRD : IImport
    {
        public const string FILE_EXTENSION = ".vrb";

        public List<FavoriteConfigurationElement> ImportFavorites(string Filename)
        {
            List<FavoriteConfigurationElement> fav = new List<FavoriteConfigurationElement>();
            InputBoxResult result =
                InputBox.Show(Localization.Text("ExportImport.Import.ImportvRD.ImportFavorites_Prompt"),
                              Localization.Text("ExportImport.Import.ImportvRD.ImportFavorites_Title"), '*');

            if (result.ReturnCode == DialogResult.OK)
            {
                byte[] file = File.ReadAllBytes(Filename);
                string xml = a(file, result.Text).Replace(" encoding=\"utf-16\"", "");
                byte[] data = Encoding.Default.GetBytes(xml);
                using (MemoryStream sw = new MemoryStream(data))
                {
                    if (sw.Position > 0 & sw.CanSeek) sw.Seek(0, SeekOrigin.Begin);
                    XmlSerializer x = new XmlSerializer(typeof (vRDConfigurationFile));
                    object results = x.Deserialize(sw);

                    List<Connection> connections = new List<Connection>();
                    List<vRDConfigurationFileConnectionsFolder> folders =
                        new List<vRDConfigurationFileConnectionsFolder>();
                    Dictionary<string, vRDConfigurationFileCredentialsFolderCredentials> credentials =
                        new Dictionary<string, vRDConfigurationFileCredentialsFolderCredentials>();

                    if (results == null)
                    {
                        return fav;
                    }
                    vRDConfigurationFile config = (results as vRDConfigurationFile);
                    if (config == null)
                    {
                        return fav;
                    }
                    //scan in config item
                    foreach (object item in config.Items)
                    {
                        if (item == null)
                        {
                            continue;
                        }
                        if (item is vRDConfigurationFileCredentialsFolder)
                        {
                            //scan in all credentials into a dictionary
                            vRDConfigurationFileCredentialsFolder credentialFolder =
                                (item as vRDConfigurationFileCredentialsFolder);
                            if (credentialFolder != null && credentialFolder.Credentials != null)
                            {
                                foreach (
                                    vRDConfigurationFileCredentialsFolderCredentials cred in
                                        credentialFolder.Credentials)
                                {
                                    credentials.Add(cred.Guid, cred);
                                }
                            }
                        }
                        else if (item is vRDConfigurationFileCredentialsFolderCredentials)
                        {
                            vRDConfigurationFileCredentialsFolderCredentials cred =
                                (item as vRDConfigurationFileCredentialsFolderCredentials);
                            credentials.Add(cred.Guid, cred);
                        }
                        else if (item is Connection)
                        {
                            //scan in the connections
                            Connection connection = (item as Connection);
                            if (connection != null)
                            {
                                connections.Add(connection);
                            }
                        }
                        else if (item is vRDConfigurationFileConnectionsFolder)
                        {
                            //scan in recurse folders
                            vRDConfigurationFileConnectionsFolder folder =
                                (item as vRDConfigurationFileConnectionsFolder);
                            if (folder != null)
                            {
                                folders.Add(folder);
                            }
                        }
                    }
                    //save credential item to local
                    this.SaveCredentials(credentials);
                    //save VRD connection to local
                    fav = this.ConvertVRDConnectionCollectionToLocal(connections.ToArray(), folders.ToArray(), null,
                                                                     String.Empty, credentials);
                }
            }
            return fav;
        }

        public string Name
        {
            get { return Localization.Text("ExportImport.Import.ImportvRD.Name"); }
        }

        public string KnownExtension
        {
            get { return FILE_EXTENSION; }
        }

        private void SaveCredentials(Dictionary<string, vRDConfigurationFileCredentialsFolderCredentials> credentials)
        {
            foreach (string guid in credentials.Keys)
            {
                vRDConfigurationFileCredentialsFolderCredentials toImport = credentials[guid];
                //will store the last one if the same credential name 
                CredentialSet destination = StoredCredentials.Instance.GetByName(toImport.Name);
                if (destination == null)
                {
                    destination = new CredentialSet();
                    StoredCredentials.Instance.Add(destination);
                }

                UpdateFromvrDCredentials(toImport, destination);
            }

            StoredCredentials.Instance.Save();
        }

        private static void UpdateFromvrDCredentials(vRDConfigurationFileCredentialsFolderCredentials source,
                                                     CredentialSet target)
        {
            target.Domain = source.Domain;
            target.Name = source.Name;
            target.SecretKey = source.Password;
            target.Username = source.UserName;
        }

        private List<FavoriteConfigurationElement> ConvertVRDConnectionCollectionToLocal(Connection[] connections,
                                                                                         vRDConfigurationFileConnectionsFolder
                                                                                             [] folders,
                                                                                         vRDConfigurationFileConnectionsFolderFolder
                                                                                             [] subFolders,
                                                                                         String connectionTag,
                                                                                         Dictionary
                                                                                             <string,
                                                                                             vRDConfigurationFileCredentialsFolderCredentials
                                                                                             > credentials)
        {
            List<FavoriteConfigurationElement> coll = new List<FavoriteConfigurationElement>();
            //covert vrd connection
            if (connections != null && connections.Length > 0)
            {
                foreach (Connection con in connections)
                {
                    FavoriteConfigurationElement fav = ConvertVRDConnectionToLocal(credentials, con);

                    if (!string.IsNullOrEmpty(connectionTag) && !fav.TagList.Contains(connectionTag))
                    {
                        fav.TagList.Add(connectionTag);
                        fav.Tags = connectionTag;
                    }

                    coll.Add(fav);
                }
            }

            //get connection object from root folder
            if (folders != null && folders.Length > 0)
            {
                foreach (vRDConfigurationFileConnectionsFolder folder in folders)
                {
                    coll.AddRange(this.ConvertVRDConnectionCollectionToLocal(folder.Connection, null, folder.Folder,
                                                                             folder.Name, credentials));
                }
            }

            //get connection object from sub folder
            if (subFolders != null && subFolders.Length > 0)
            {
                foreach (vRDConfigurationFileConnectionsFolderFolder folder in subFolders)
                {
                    string tag = connectionTag + folder.Name;
                    coll.AddRange(this.ConvertVRDConnectionCollectionToLocal(folder.Connection, null, null, tag,
                                                                             credentials));
                }
            }

            return coll;
        }

        private static FavoriteConfigurationElement ConvertVRDConnectionToLocal(
            Dictionary<string, vRDConfigurationFileCredentialsFolderCredentials> credentials, Connection con)
        {
            FavoriteConfigurationElement fav = new FavoriteConfigurationElement {ServerName = con.ServerName};

            int p = 3389;
            int.TryParse(con.Port, out p);
            fav.Port = p;

            if (credentials.ContainsKey(con.Credentials))
            {
                fav.XmlCredentialSetName = credentials[con.Credentials].Name;
                fav.UserName = credentials[con.Credentials].UserName;
                fav.DomainName = credentials[con.Credentials].Domain;
                fav.Password = credentials[con.Credentials].Password;
            }

            switch (con.ColorDepth)
            {
                case "8":
                    fav.Colors = Colors.Bits8;
                    break;
                case "16":
                    fav.Colors = Colors.Bit16;
                    break;
                case "24":
                    fav.Colors = Colors.Bits24;
                    break;
                case "32":
                    fav.Colors = Colors.Bits32;
                    break;
                default:
                    fav.Colors = Colors.Bit16;
                    break;
            }
            
            fav.DesktopSize = DesktopSize.AutoScale;
            if (con.SeparateWindow == "true") fav.DesktopSize = DesktopSize.FullScreen;

            fav.ConnectToConsole = false;
            if (con.Console == "true") fav.ConnectToConsole = true;

            fav.DisableWallPaper = false;
            if (con.BitmapCaching == "false") fav.DisableWallPaper = true;

            fav.RedirectSmartCards = false;
            if (con.SmartCard == "true") fav.RedirectSmartCards = true;

            fav.RedirectPorts = false;
            //if (pValue == "1") fav.RedirectPorts = true;

            fav.RedirectPrinters = false;
            if (con.Printer == "true") fav.RedirectPrinters = true;

            fav.Sounds = ImportRDP.ConvertToSounds(con.Audio);
            fav.Name = con.Name;

            return fav;
        }

        private void ImportConnection(Connection Connection, List<string> FolderNames,
                                      Dictionary<string, vRDConfigurationFileCredentialsFolderCredentials> Credentials)
        {
        }

        private void ImportFolder(vRDConfigurationFileConnectionsFolder Folder, List<string> FolderNames,
                                  Dictionary<string, vRDConfigurationFileCredentialsFolderCredentials> Credentials)
        {
            foreach (Connection conn in Folder.Connection)
            {
                this.ImportConnection(conn, FolderNames, Credentials);
            }

            FolderNames.AddRange(Folder.Folder.Select(folder => folder.Name));
        }

        private enum ab
        {
            a = 1,
            b = 2,
            c = 3,
            d = 4,
            e = 5,
            f = 6,
            g = 7,
            h = 8,
            i = 9,
            j = 10,
            k = 11,
            l = 12,
            m = 13,
            n = 14,
            o = 15,
            p = 0x10,
            q = 0x11
        }

        [DllImport("advapi32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern bool GetTokenInformation(IntPtr A_0, ab A_1, [Out] byte[] A_2, uint A_3, ref uint A_4);

        private static SymmetricAlgorithm a()
        {
            WindowsIdentity current = WindowsIdentity.GetCurrent();

            uint num = 0;

            if (!GetTokenInformation(current.Token, ab.a, null, 0, ref num))
            {
                int error = Marshal.GetLastWin32Error();

                if (error != 0x7a)
                {
                    throw new Win32Exception(error);
                }
            }

            byte[] buffer = new byte[num];

            if (!GetTokenInformation(current.Token, ab.a, buffer, num, ref num))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            byte[] destinationArray = new byte[num - 8];

            Array.Copy(buffer, 8L, destinationArray, 0L, (num - 8));

            byte[] buffer3 = new byte[0x10];

            Array.Copy(destinationArray, 0, buffer3, 0, 0x10);

            byte[] buffer4 = new byte[0x20];

            Array.Copy(destinationArray, destinationArray.Length - 0x10, buffer4, 0, 0x10);
            Array.Copy(destinationArray, destinationArray.Length - 0x10, buffer4, 0x10, 0x10);

            RijndaelManaged managed = new RijndaelManaged {KeySize = 0x100, IV = buffer3, Key = buffer4};

            return managed;
        }

        private static SymmetricAlgorithm a(bool A_0, string Password)
        {
            byte[] bytes = new UnicodeEncoding().GetBytes(Password.PadRight(0x100, ' '));
            byte[] sourceArray = new SHA1Managed().ComputeHash(bytes);
            byte[] destinationArray = new byte[0x10];

            Array.Copy(sourceArray, 0, destinationArray, 0, 0x10);

            byte[] buffer4 = new byte[0x20];

            Array.Copy(sourceArray, 0, buffer4, 0, 20);
            Array.Copy(sourceArray, 0, buffer4, 20, 12);

            RijndaelManaged managed2 = new RijndaelManaged();

            managed2.KeySize = 0x100;
            managed2.IV = destinationArray;
            managed2.Key = buffer4;

            return managed2;
        }

        public static byte[] a(string A_0, string Password)
        {
            SymmetricAlgorithm algorithm = a(true, Password);

            if (algorithm == null)
            {
                return null;
            }

            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, algorithm.CreateEncryptor(), CryptoStreamMode.Write);

            byte[] bytes = new UnicodeEncoding().GetBytes(A_0);

            stream2.Write(bytes, 0, bytes.Length);
            stream2.FlushFinalBlock();
            stream.Position = 0L;

            byte[] buffer2 = stream.ToArray();

            stream2.Close();
            stream.Close();

            return buffer2;
        }

        private static string a(byte[] A_0, string Password)
        {
            SymmetricAlgorithm algorithm = a(false, Password);

            if (algorithm == null)
            {
                return "";
            }

            MemoryStream stream = new MemoryStream();

            stream.Write(A_0, 0, A_0.Length);
            stream.Position = 0L;

            CryptoStream stream2 = new CryptoStream(stream, algorithm.CreateDecryptor(), CryptoStreamMode.Read);

            byte[] buffer = new byte[A_0.Length];
            int count = stream2.Read(buffer, 0, buffer.Length);

            stream2.Close();
            stream.Close();

            UnicodeEncoding encoding = new UnicodeEncoding();

            return encoding.GetString(buffer, 0, count);
        }

        public static byte[] b(string A_0)
        {
            SymmetricAlgorithm algorithm = a();
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, algorithm.CreateEncryptor(), CryptoStreamMode.Write);

            byte[] bytes = new UnicodeEncoding().GetBytes(A_0.PadRight(0x100, ' '));

            stream2.Write(bytes, 0, bytes.Length);
            stream2.FlushFinalBlock();
            stream.Position = 0L;

            byte[] buffer2 = stream.ToArray();

            stream2.Close();
            stream.Close();

            return buffer2;
        }

        public static string b(byte[] A_0)
        {
            SymmetricAlgorithm algorithm = a();
            MemoryStream stream = new MemoryStream();

            stream.Write(A_0, 0, A_0.Length);
            stream.Position = 0L;

            CryptoStream stream2 = new CryptoStream(stream, algorithm.CreateDecryptor(), CryptoStreamMode.Read);

            byte[] buffer = new byte[A_0.Length];
            int count = stream2.Read(buffer, 0, A_0.Length);

            stream2.Close();
            stream.Close();

            UnicodeEncoding encoding = new UnicodeEncoding();

            return encoding.GetString(buffer, 0, count).TrimEnd(new[] {' '});
        }
    }
}