using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using Kohl.Framework.Info;
using Kohl.Framework.Logging;
using Terminals.Configuration.Serialization;

namespace Terminals.Configuration.Files.Credentials
{
    public static class StoredCredentials
    {
        /// <summary>
        ///     Gets default name of the credentials file.
        /// </summary>
        private const string CONFIG_FILE = "Credentials.xml";

        private static readonly List<CredentialSet> cache;

        private static readonly Mutex fileLock = new Mutex(false, AssemblyInfo.Title + "." + CONFIG_FILE);
        private static DataFileWatcher fileWatcher;

        /// <summary>
        ///     Prevents creating from other class
        /// </summary>
        static StoredCredentials()
        {
            cache = new List<CredentialSet>();

            if (Main.Settings.Settings.CredentialStore != Terminals.Configuration.Files.Main.CredentialStoreType.Xml)
            	LoadStoredCredentials(configFileLocation);
            else
            {
            	InitializeFileWatch();
            
                if (File.Exists(configFileLocation))
                    LoadStoredCredentials(configFileLocation);
                else
                    Save();
           	}
        }

        /// <summary>
        ///     Gets the not null collection containing stored credentials
        /// </summary>
        public static List<CredentialSet> Items
        {
            get
            {
                // prevent manipulation directly with this list
                return (from s in cache orderby s.Name select s).ToList();
            }
        }

        public static event EventHandler CredentialsChanged;

        private static string configFileLocation = Path.Combine(AssemblyInfo.DirectoryConfigFiles, CONFIG_FILE);

        public static string ConfigurationFileLocation
        {
            get
            {
                return configFileLocation;
            }
            set
            {
                configFileLocation = value;
            }
        }

        private static void InitializeFileWatch()
        {
            fileWatcher = new DataFileWatcher(configFileLocation);
            fileWatcher.FileChanged += CredentialsFileChanged;
            fileWatcher.StartObservation();
        }

        private static void CredentialsFileChanged(object sender, EventArgs e)
        {
        	if (Main.Settings.Settings.CredentialStore != Terminals.Configuration.Files.Main.CredentialStoreType.Xml)
        		return;
        	
            LoadStoredCredentials(configFileLocation);
            if (CredentialsChanged != null)
                CredentialsChanged("CredentialsFileChanged", new EventArgs());
        }

        public static void AssignSynchronizationObject(ISynchronizeInvoke synchronizer)
        {
        	if (Main.Settings.Settings.CredentialStore != Terminals.Configuration.Files.Main.CredentialStoreType.Xml)
        		return;
            
        	fileWatcher.AssignSynchronizer(synchronizer);
        }

        private static void LoadStoredCredentials(string configFileName)
        {
        	List<CredentialSet> loaded = null;
        	
        	if (Main.Settings.Settings.CredentialStore == Terminals.Configuration.Files.Main.CredentialStoreType.KeePass)
        	{
        		loaded = LoadKeePass();
        	}
        	else if (Main.Settings.Settings.CredentialStore == Terminals.Configuration.Files.Main.CredentialStoreType.DB)
        	{
        		
        	}
            else
				loaded = LoadFile(configFileName);
            
            if (loaded != null)
            {
                cache.Clear();
                cache.AddRange(loaded);
            }
        }
        
        private static List<CredentialSet> LoadKeePass()
        {        	
        	try
        	{
				var ioConnInfo = new KeePassLib.Serialization.IOConnectionInfo { Path = Main.Settings.Settings.KeePassPath };
				var compKey = new KeePassLib.Keys.CompositeKey();
				compKey.AddUserKey(new KeePassLib.Keys.KcpPassword(Main.Settings.Settings.KeePassPassword));
				
				var db = new KeePassLib.PwDatabase();
				db.Open(ioConnInfo, compKey, null);
	
				var entries = db.RootGroup.GetEntries(true);
				
				List<CredentialSet> list = new List<CredentialSet>();
								
				foreach (var entry in entries)
				{
					string title = entry.Strings.ReadSafe("Title");
					string userName = entry.Strings.ReadSafe("UserName");
					string domain = entry.Strings.ReadSafe("Domain");
					
					if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(userName))
					{
						list.Add(new CredentialSet
		                {
		                    Name = title,
		                    Username = string.IsNullOrEmpty(domain) && userName.Contains("\\")  ? userName.Split(new string[] {"\\"}, StringSplitOptions.None)[1] : userName,
		                    Domain = string.IsNullOrEmpty(domain) && userName.Contains("\\")  ? userName.Split(new string[] {"\\"}, StringSplitOptions.None)[0] : domain,
		                    Password = entry.Strings.ReadSafe("Password")
				         });
					}
				}

				db.Close();
				
				return list;
        	} catch (Exception ex)
        	{
                Log.Error("Error loading KeePass-File due to the following reason: " + ex.Message, ex);
                return  new List<CredentialSet>();
        	}
        }

        private static List<CredentialSet> LoadFile(string configFileName)
        {
        	if (Main.Settings.Settings.CredentialStore != Terminals.Configuration.Files.Main.CredentialStoreType.Xml)
        		return new List<CredentialSet>();
        	
            try
            {
                fileLock.WaitOne();
                object loadedObj = Serialize.DeserializeXmlFromDisk(configFileName, typeof (List<CredentialSet>));
                return loadedObj as List<CredentialSet>;
            }
            catch (Exception exception)
            {
                string errorMessage = String.Format("Load credentials from {0} failed.", configFileName);
                Log.Error(errorMessage, exception);
                return new List<CredentialSet>();
            }
            finally
            {
                fileLock.ReleaseMutex();
            }
        }
        
        public static void Save()
        {
        	if (Main.Settings.Settings.CredentialStore != Terminals.Configuration.Files.Main.CredentialStoreType.Xml)
        		return;
        	
            try
            {
                fileLock.WaitOne();
                fileWatcher.StopObservation();
                Serialize.SerializeXmlToDisk(cache, configFileLocation);
            }
            catch (Exception exception)
            {
                string errorMessage = string.Format("Save credentials to {0} failed.", configFileLocation);
                Log.Error(errorMessage, exception);
            }
            finally
            {
                fileWatcher.StartObservation();
                fileLock.ReleaseMutex();
            }
        }

        /// <summary>
        ///     Gets a credential by its name from cached credentials.
        ///     This method isnt case sensitive. If no item matches, returns null.
        /// </summary>
        /// <param name="name"> name of an item to search </param>
        public static CredentialSet GetByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            name = name.ToLower();
            return Items.FirstOrDefault(candidate => candidate.Name.ToLower() == name);
        }

        public static void Remove(CredentialSet toRemove)
        {
            cache.Remove(toRemove);
        }

        public static void Add(CredentialSet toAdd)
        {
            if (String.IsNullOrEmpty(toAdd.Name))
                return;

            cache.Add(toAdd);
        }

        public static void UpdatePasswordsByNewKeyMaterial(string newKeyMaterial)
        {
			if (Main.Settings.Settings.CredentialStore != Terminals.Configuration.Files.Main.CredentialStoreType.Xml)
        		return;
        	        	
            foreach (CredentialSet credentials in cache)
            {
                credentials.UpdatePasswordByNewKeyMaterial(newKeyMaterial);
            }

            Save();
        }
    }
}