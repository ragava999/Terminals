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
    public sealed class StoredCredentials
    {
        /// <summary>
        ///     Gets default name of the credentials file.
        /// </summary>
        private const string CONFIG_FILE = "Credentials.xml";

        private readonly List<CredentialSet> cache;

        private readonly Mutex fileLock = new Mutex(false, AssemblyInfo.Title() + "." + CONFIG_FILE);
        private DataFileWatcher fileWatcher;

        /// <summary>
        ///     Prevents creating from other class
        /// </summary>
        private StoredCredentials()
        {
            // This prevents SharpDevelop and Visual Studio from both an exception in design mode for controls using this HistoryTreeView and from crashing when opening the
            // designer for this class.
            if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)
            {
                this.cache = new List<CredentialSet>();

                this.InitializeFileWatch();

                if (File.Exists(configFileLocation))
                    this.LoadStoredCredentials(configFileLocation);
                else
                    this.Save();
            }
        }

        /// <summary>
        ///     Gets the not null collection containing stored credentials
        /// </summary>
        public List<CredentialSet> Items
        {
            get
            {
                // prevent manipulation directly with this list
                return (from s in this.cache orderby s.Name select s).ToList();
            }
        }

        public event EventHandler CredentialsChanged;

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

        private void InitializeFileWatch()
        {
            this.fileWatcher = new DataFileWatcher(configFileLocation);
            this.fileWatcher.FileChanged += this.CredentialsFileChanged;
            this.fileWatcher.StartObservation();
        }

        private void CredentialsFileChanged(object sender, EventArgs e)
        {
            this.LoadStoredCredentials(configFileLocation);
            if (this.CredentialsChanged != null)
                this.CredentialsChanged(this, new EventArgs());
        }

        public void AssignSynchronizationObject(ISynchronizeInvoke synchronizer)
        {
            this.fileWatcher.AssignSynchronizer(synchronizer);
        }

        private void LoadStoredCredentials(string configFileName)
        {
            List<CredentialSet> loaded = this.LoadFile(configFileName);
            if (loaded != null)
            {
                this.cache.Clear();
                this.cache.AddRange(loaded);
            }
        }

        private List<CredentialSet> LoadFile(string configFileName)
        {
            try
            {
                this.fileLock.WaitOne();
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
                this.fileLock.ReleaseMutex();
            }
        }

        public void Save()
        {
            try
            {
                this.fileLock.WaitOne();
                this.fileWatcher.StopObservation();
                Serialize.SerializeXmlToDisk(this.cache, configFileLocation);
            }
            catch (Exception exception)
            {
                string errorMessage = string.Format("Save credentials to {0} failed.", configFileLocation);
                Log.Error(errorMessage, exception);
            }
            finally
            {
                this.fileWatcher.StartObservation();
                this.fileLock.ReleaseMutex();
            }
        }

        /// <summary>
        ///     Gets a credential by its name from cached credentials.
        ///     This method isnt case sensitive. If no item matches, returns null.
        /// </summary>
        /// <param name="name"> name of an item to search </param>
        public CredentialSet GetByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            name = name.ToLower();
            return this.Items.FirstOrDefault(candidate => candidate.Name.ToLower() == name);
        }

        public void Remove(CredentialSet toRemove)
        {
            this.cache.Remove(toRemove);
        }

        public void Add(CredentialSet toAdd)
        {
            if (String.IsNullOrEmpty(toAdd.Name))
                return;

            this.cache.Add(toAdd);
        }

        public void UpdatePasswordsByNewKeyMaterial(string newKeyMaterial)
        {
            foreach (CredentialSet credentials in this.cache)
            {
                credentials.UpdatePasswordByNewKeyMaterial(newKeyMaterial);
            }

            this.Save();
        }

        #region Thread safe singleton
        /// <summary>
        ///     Gets the singleton instance with cached credentials
        /// </summary>
        public static StoredCredentials Instance
        {
            get { return Nested.Instance; }
        }

        private static class Nested
        {
            private static StoredCredentials instance;

            public static StoredCredentials Instance
            {
                get
                {
                    // This prevents SharpDevelop and Visual Studio from both an exception in design mode for controls using this HistoryTreeView and from crashing when opening the
                    // designer for this class.
                    if (LicenseManager.UsageMode == LicenseUsageMode.Runtime)
                        if (instance == null)
                            instance = new StoredCredentials();

                    return instance;
                }
            }
        }
        #endregion
    }
}