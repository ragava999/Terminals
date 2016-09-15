using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;
using Kohl.Framework.Info;
using Kohl.Framework.Logging;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Configuration.Properties;
using System.Linq;

namespace Terminals.Configuration.Files.Main.Settings
{
    public static partial class Settings
    {
        /// <summary>
        ///     Gets the name of custom user options configuration file
        /// </summary>
        public static readonly String CONFIG_FILE_NAME = AssemblyInfo.Title + ".config";

        private static string configurationFileLocation;

        private static System.Configuration.Configuration _config;
        private static DataFileWatcher fileWatcher;

        /// <summary>
        ///     Prevent concurent updates on config file by another program
        /// </summary>
        private static readonly Mutex fileLock = new Mutex(false, AssemblyInfo.Title + ".Settings");

        /// <summary>
        ///     Flag informing, that configuration shouldnt be saved imediately, but after explicit call
        ///     This increases performance for
        /// </summary>
        private static bool delayConfigurationSave;

        public static string ConfigurationFileLocation
        {
            get
            {
                if (string.IsNullOrEmpty(configurationFileLocation))
                    SetDefaultConfigurationFileLocation();

                return configurationFileLocation;
            }
            set
            {
                configurationFileLocation = value;
                if (fileWatcher != null)
                    fileWatcher.FullFileName = value;
            }
        }

        private static System.Configuration.Configuration Config
        {
            get
            {
                if (_config == null)
                {
                    InitializeFileWatcher();
                    _config = GetConfiguration();
                }

                return _config;
            }
        }

        /// <summary>
        ///     Informs lisseners, that configuration file was changed by another application
        ///     or another Terminals instance. In this case all cached not saved data are lost.
        /// </summary>
        public static event ConfigurationChangedHandler ConfigurationChanged;

        /// <summary>
        ///     Gets the full file path to the required file or directory in application data directory.
        /// </summary>
        /// <param name="relativePath"> The relative path to the file from data directory. </param>
        public static string GetFullPath(string relativePath)
        {
            return Path.Combine(AssemblyInfo.Directory, relativePath);
        }

        private static void SetDefaultConfigurationFileLocation()
        {
            configurationFileLocation = Path.Combine(AssemblyInfo.DirectoryConfigFiles, CONFIG_FILE_NAME);
        }

        private static void ConfigFileChanged(object sender, EventArgs e)
        {
            TerminalsConfigurationSection old = GetSection();
            ForceReload();
            ConfigurationChangedEventArgs args = ConfigurationChangedEventArgs.CreateFromSettings(old, GetSection());
            FireConfigurationChanged(args);
        }

        private static void FireConfigurationChanged(ConfigurationChangedEventArgs args)
        {
            if (ConfigurationChanged != null)
            {
                ConfigurationChanged(args);
            }
        }

        private static void InitializeFileWatcher()
        {
            if (fileWatcher != null)
                return;

            fileWatcher = new DataFileWatcher(ConfigurationFileLocation);
            fileWatcher.FileChanged += ConfigFileChanged;
        }

        /// <summary>
        ///     Because filewatcher is created before the main form in GUI thread.
        ///     This lets to fire the file system watcher events in GUI thread.
        /// </summary>
        public static void AssignSynchronizationObject(ISynchronizeInvoke synchronizer)
        {
            fileWatcher.AssignSynchronizer(synchronizer);
        }

        public static void ForceReload()
        {
            _config = GetConfiguration();
        }

        /// <summary>
        ///     Prevents save configuration after each change. After this call, no settings are saved
        ///     into config file, until you call SaveAndFinishDelayedUpdate.
        ///     This dramatically increases performance. Use this method for batch updates.
        /// </summary>
        public static void StartDelayedUpdate()
        {
            delayConfigurationSave = true;
        }

        /// <summary>
        ///     Stops prevent write changes into config file and immediately writes last state.
        ///     Usually the changes are saved immediately
        /// </summary>
        public static void SaveAndFinishDelayedUpdate()
        {
            delayConfigurationSave = false;
            SaveImmediatelyIfRequested();
        }

        private static void SaveImmediatelyIfRequested()
        {
            if (!delayConfigurationSave)
            {
                bool locked = false;
                try
                {
                    locked = true;
                    fileLock.WaitOne(); // lock the file for changes by other application instance
                }
                catch (Exception exception)
                {
                    Log.Error("Error locking the configuration file.", exception);
                }

                Save();

                if (locked)
                {
                    fileLock.ReleaseMutex();
                }
            }
        }

        public static void PauseConfigObserving()
        {
            try
            {
                fileWatcher.StopObservation();
            }
            catch (Exception exception)
            {
                Log.Error("Unable to pause configuration file observation.", exception);
            }
        }


        public static void ContinueConfigObserving()
        {
            try
            {
                fileWatcher.StartObservation();
            }
            catch (Exception exception)
            {
                Log.Error("Unable to start configuration file observation.", exception);
            }
        }

        private static void Save()
        {
        	if (remappingInProgress)
        		return;
        	
            PauseConfigObserving();
            
            try
            {
                Config.Save(ConfigurationSaveMode.Minimal, true);
                Log.Debug("The configuration file has been saved successfully.");
            }
            catch (Exception exception)
            {
                Log.Error("Unable to save the configuration file.", exception);
            }

            ContinueConfigObserving();
        }

        private static System.Configuration.Configuration GetConfiguration()
        {
            try
            {
                CreateConfigFileIfNotExist();
                return OpenConfiguration();
            }
            catch (Exception exc) // revert to the default configuration file.
            {
                Log.Error("Get Configuration", exc);
                SaveDefaultConfigFile();
                return OpenConfiguration();
            }
        }

        private static void CreateConfigFileIfNotExist()
        {
            if (!File.Exists(ConfigurationFileLocation))
                SaveDefaultConfigFile();
        }

        private static ExeConfigurationFileMap CreateConfigFileMap()
        {
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = ConfigurationFileLocation;
            return configFileMap;
        }

        private static System.Configuration.Configuration OpenConfiguration()
        {
            ExeConfigurationFileMap configFileMap = CreateConfigFileMap();
            fileLock.WaitOne();
            
            System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
            fileLock.ReleaseMutex();
            return config;
        }

        private static void SaveDefaultConfigFile()
        {
            string templateConfigFile = Resources.Terminals;
            Log.Warn("Using a new plain Terminals configuration file.");
            using (StreamWriter sr = new StreamWriter(ConfigurationFileLocation,false))
            {
                sr.Write(templateConfigFile);
            }
        }

        private static void CopyFile(string fileName, string tempFileName, bool move = false)
        {
        	if (move)
        	{
	            // delete the zerobyte file which is created by default
	            if (File.Exists(tempFileName))
	                File.Delete(tempFileName);
	
	            // move the error file to the temp file
	            File.Move(fileName, tempFileName);
	
	            // if its still hanging around, kill it
	            if (File.Exists(fileName))
	                File.Delete(fileName);
        	}
        	else
        		File.Copy(fileName, tempFileName, true);
        }

        private static XmlDocument LoadDocument(string file)
        {
            // read all the xml from the erroring file
            XmlDocument doc = new XmlDocument();
            string xmlFileContent = null;

            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite & FileShare.Delete))
            {
                using (StreamReader stream = new StreamReader(fs))
                {
                    xmlFileContent = stream.ReadToEnd();
                }
            }

            doc.LoadXml(xmlFileContent);

            return doc;
        }

        private static bool remappingInProgress = false;
        private static TerminalsConfigurationSection remappingSection = null;
        static string tempFile = "";
        
        /// <summary>
        /// Reconstructs a new Terminals configuration file from a broken one.
        /// </summary>
        /// <remarks>Please read all the comments in this methods before chaning something.</remarks>
        /// <returns>Returns a complete new Terminals configuration in the form of a <see cref="System.Configuration.Configuration">.NET configuration file</see>.</returns>
        private static System.Configuration.Configuration ImportConfiguration()
        {
        	// If we are already in the remapping process/progress
        	// i.e. if we are in this method -> we don't want to go
        	// through it a second or third time for just calling
			// a property in the Settings class
			// like			
        	//        public static bool ShowFullInformationToolTips
			//        {
			//            get { return GetSection().ShowFullInformationToolTips; }
			//
			//            set
			//            {
			//                GetSection().ShowFullInformationToolTips = value;
			//                SaveImmediatelyIfRequested();
			//            }
			//        }
			// Which internally calls GetSection() -> and that forces to call
			// this method if the mapping of the file fails.
			// -> So quickly jump out of this procedure
			//
			// BTW: Every Save() method call will be ignored as long as the
			// remappingInProgress := true
			// That's why we save the file that has been constructed in memory
			// to the disk until we are finished with the whole parsing process.
			// (-> see 'Save()' at the end of this method)
        	if (remappingInProgress)
        		return OpenConfiguration();
        	
        	// Stop the file watcher while we create a plain new configuration file
            if (fileWatcher != null)
                fileWatcher.StopObservation();
        	
       		remappingInProgress = true;
       		
       		// Create a temporary copy of this file and overwrite the original
       		// one with a plain new configuration file (as can be seen in the next
       		// few lines (i.e. SaveDefaultConfigFile())
       		// We'll parse the temporary copy of the file that has been created and
			// update the plain file       		
       		if (string.IsNullOrEmpty(tempFile))
       		{
       			tempFile = Path.GetTempFileName();
       			CopyFile(ConfigurationFileLocation, tempFile);
       		}

       		// Restart the file watcher again
       		SaveDefaultConfigFile(); 
       		
            if (fileWatcher != null)
                fileWatcher.StartObservation();

            // get a list of the properties on the Settings object (static props)
            PropertyInfo[] propList = typeof(Settings).GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.SetProperty);

            // Load the whole xml file
            // No problem if this line fails -> the plain new copy is
            // totally enough
            XmlDocument doc = LoadDocument(tempFile);

            // get the settings (options)
            XmlNode root = doc.SelectSingleNode("/configuration/settings");
            
            bool terminalsPasswordDetected = false;
            
			if (root.Attributes.Count == 0)
				Log.Warn("The old configuration file doesn't contain any Terminals settings.");

            try
            {
                // for each setting's attribute
                foreach (XmlAttribute att in root.Attributes)
                {
                	// Set the Terminals password used to encrypt and decrypt the passwords if enabled
                	try
                	{
	                   	if (att.Name.Equals("TerminalsPassword", StringComparison.CurrentCultureIgnoreCase))
	                    {
	                   		terminalsPasswordDetected = true;
	                   		if (!string.IsNullOrWhiteSpace(att.Value))
	                   		{
		                   		remappingSection.TerminalsPassword = att.Value;
		                    	Log.Debug("The Terminals password has been set in the new configuration file.");
		                    	continue;
	                   		}
	                   		
	                   		Log.Debug("A Terminals password attribute exists in the broken original configuration file but it hasn't been set.");
		                    continue;
	                    }
                	}
                	catch (Exception ex)
                	{
                		terminalsPasswordDetected = true;
                		Log.Error("Error occured while trying to set the Terminals password in the new configuration file. This error will prevent terminals from decrypting your passwords.", ex);
                		continue;
                	}
                	
                    // scan for the related property if any
                    foreach (PropertyInfo info in propList)
                    {
                        try
                        {                        	
                            if (info.Name.ToLower() == att.Name.ToLower())
                            {
                                // found a matching property, try to set it
                                string val = att.Value;
                                
                                if (info.PropertyType.Name == "Version")
                                	info.SetValue(null, new Version(val), null);
                                else
                                	info.SetValue(null, Convert.ChangeType(val, info.PropertyType), null);
                                
                                break;
                            }
                        }
                        catch (Exception exc)
                        {
                            Log.Error("Error occured while trying parse the configuration file and map " + att.Name + ": " + exc.Message);
                            break;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Log.Error("Error occured while trying parse the configuration file and remapping the root settings.", exc);
            }

            if (!terminalsPasswordDetected)
            {
            	Log.Debug("No Terminals password has been found in the broken configuration file.");
            }
            
			// TODO: Noch zu überprüfen
            // pluginOptions
            // favoritesButtonsList
            // toolStripSettings
            // savedConnectionsList
            // usersMRUList
            // serversMRUList
            // domainsMRUList
            
            
            // tags -> werden implizit durch die Favorites gesetzt - Überprüfen ob korrekt
            
            // Nicht alle Felder bei den Favorites werden gesetzt
            
            // Work through every favorite configuration element
            XmlNodeList favs = doc.SelectNodes("/configuration/settings/favorites/add");

			if (favs.Count == 0)
				Log.Warn("Your old configuration file doesn't contain any Terminals favorites.");

			try
            {
                foreach (XmlNode fav in favs)
            	{
                    FavoriteConfigurationElement newFav = new FavoriteConfigurationElement();
                    
                    // Add the plugin configuration for each favorite element
                    if (fav.ChildNodes != null && fav.ChildNodes.Count == 1)
                    {
                    	XmlNode pluginRootElement = fav.ChildNodes[0];
                    	
                    	if (pluginRootElement.Name.Equals("PLUGINS", StringComparison.InvariantCultureIgnoreCase))
                    	{
                    		foreach (XmlNode plugin in pluginRootElement.ChildNodes)
                    		{
                    			if (plugin.Name.Equals("PLUGIN", StringComparison.InvariantCultureIgnoreCase) && plugin.Attributes != null && plugin.Attributes.Count > 0)
                    			{
                    				PluginConfiguration pluginConfig = new PluginConfiguration();
                    				
                    				if (plugin.Attributes["name"] != null && !string.IsNullOrEmpty(plugin.Attributes["name"].Value))
                    					pluginConfig.Name = plugin.Attributes["name"].Value;
                    				
                    				if (plugin.Attributes["value"] != null && !string.IsNullOrEmpty(plugin.Attributes["value"].Value))
                    				{
                    					pluginConfig.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty).First(property => property.Name.Equals("value", StringComparison.InvariantCultureIgnoreCase)).SetValue(pluginConfig, plugin.Attributes["value"].Value, null);
                    				}
                    				
                    				if (plugin.Attributes["defaultValue"] != null && !string.IsNullOrEmpty(plugin.Attributes["defaultValue"].Value))
                    				{
                    					pluginConfig.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty).First(property => property.Name.Equals("defaultValue", StringComparison.InvariantCultureIgnoreCase)).SetValue(pluginConfig, plugin.Attributes["defaultValue"].Value, null);
                    				}
                    				
                    				newFav.PluginConfigurations.Add(pluginConfig);
                    			}
                    		}
                    	}
                    }
                    
                    // Add the attributes of each favorite
                    foreach (XmlAttribute att in fav.Attributes)
                    {                    	
                        foreach (PropertyInfo info in newFav.GetType().GetProperties())
                        {
                            try
                            {
                                if (info.Name.ToLower() == att.Name.ToLower())
                                {
                                    // found a matching property, try to set it
                                    string val = att.Value;
                                    if (info.PropertyType.IsEnum)
                                    {
                                        info.SetValue(newFav, Enum.Parse(info.PropertyType, val), null);
                                    }
                                    else
                                    {
                                        info.SetValue(newFav, Convert.ChangeType(val, info.PropertyType), null);
                                    }

                                    break;
                                }
                            }
                            catch (Exception exc)
                            {
                            	string favoriteName = "favorite";
                            	
                            	if (fav == null || fav.Attributes.Count < 1 || fav.Attributes["name"] == null || string.IsNullOrEmpty(fav.Attributes["name"].Value))
                            		favoriteName += "s";
                            	else
                            		favoriteName += " " + fav.Attributes["name"].Value;
                            	
                            	favoriteName += ": ";
                            	
                                Log.Error("Error occured while trying parse the configuration file and remapping the " + favoriteName + exc.Message);
                                break;
                            }
                        }
                    }

                    AddFavorite(newFav);
                }
            }
            catch (Exception exc)
            {
                Log.Error("Error remapping favorites: " + exc.Message);
                remappingInProgress = false;
	            return Config;
            }
            
            File.Delete(tempFile);
			remappingInProgress = false;
            Save();
            return _config;
        }
        
        private static TerminalsConfigurationSection GetSection()
        {
        	if (remappingInProgress && remappingSection != null)
        		return remappingSection;
        	
            try
            {
            	remappingSection = null;
                return Config.GetSection("settings") as TerminalsConfigurationSection;
            }
            catch (Exception ex)
            {
            	if (!(remappingInProgress && remappingSection == null))
            		Log.Warn("Warning remapping configuration file: " + ex.Message, ex);
            	
                try
                {
                    // kick into the import routine
                    _config = ImportConfiguration();
                    _config = GetConfiguration();
                    
                    remappingSection = _config.GetSection("settings") as TerminalsConfigurationSection;
                    
                    return remappingSection;
                }
                catch (Exception importException)
                {
                    Log.Error("Terminals was unable to automatically upgrade your existing connections.", importException);
                    return new TerminalsConfigurationSection();
                }
            }
        }
    }
}