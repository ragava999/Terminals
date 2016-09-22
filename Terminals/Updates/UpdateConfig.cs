using Kohl.Framework.Logging;
using System.IO;

namespace Terminals.Updates
{
    using Configuration.Files.Main.Settings;
    using Kohl.Framework.Info;
    using System;

    /// <summary>
    ///     Class containing methods to update config files.
    /// </summary>
    public static class UpdateConfig
    {
        /// <summary>
        ///     Updates config file to current version, if it isnt up to date
        /// </summary>
        public static void CheckConfigVersionUpdate()
        {
            Log.Info("Checking your configuration file for the need of an upgrade.");

            // If the Terminals version is not in the config or the version number in the config
            // is lower then the current assembly version, check for config updates
            if (Settings.ConfigVersion == null || Settings.ConfigVersion < AssemblyInfo.Version)
            {
                Log.Info(string.Format("Updating your {0} configuration file from version {1} to version {2}", AssemblyInfo.Title, Settings.ConfigVersion, AssemblyInfo.Version));

                // keep update sequence ordered!
                UpdateObsoleteConfigVersions();

                // After all updates change the config version to the current assembly version
                Settings.ConfigVersion = AssemblyInfo.Version;
            }
        }

        private static string ReplaceAttribute(this string config, string name, string newName = null, string newValue = null)
        {
            string startPattern = name + "=\"";
            int startIndex = config.IndexOf(startPattern);

            // no attribute existing -> OK
            if (startIndex == -1)
                return config;

            int endIndex = 0;

            // delete
            if (string.IsNullOrEmpty(newName) && string.IsNullOrEmpty(newValue))
            {
                endIndex = config.IndexOf("\" ", startIndex + startPattern.Length);
                if (endIndex == -1)
                {
                    endIndex = config.IndexOf("\"", startIndex + startPattern.Length);

                    // error -> attribute existing, but parsing error
                    if (endIndex == -1)
                        return config;

                    endIndex++;
                }
                else
                    endIndex += 2;

                return config.Remove(startIndex, endIndex - startIndex);
            }

            if (!string.IsNullOrEmpty(newName) && !string.IsNullOrEmpty(newValue))
            {
                // new name and new value
                endIndex = config.IndexOf("\"", startIndex + startPattern.Length);

                // error -> attribute existing, but parsing error
                if (endIndex == -1)
                    return config;

                config = config.Remove(startIndex, endIndex - startIndex);
                return config.Insert(startIndex, newName + "=\"" + newValue);
            }

            // new name
            if (!string.IsNullOrEmpty(newName))
            {
                endIndex = config.IndexOf("=", startIndex);

                // error -> attribute existing, but parsing error
                if (endIndex == -1)
                    return config;

                config = config.Remove(startIndex, endIndex - startIndex);
                return config.Insert(startIndex, newName);
            }

            // new value
            endIndex = config.IndexOf("\"", startIndex + startPattern.Length);

            // error -> attribute existing, but parsing error
            if (endIndex == -1)
                return config;

            config = config.Remove(startIndex, endIndex - startIndex);
            return config.Insert(startIndex, name + "=\"" + newValue);
        }

        private static void UpdateObsoleteConfigVersions()
        {
            /*
            if (Settings.ConfigVersion != null && Settings.ConfigVersion <= new Version(4, 0, 0, 1))
            {
                Log.Info("Upgrading configuration to version level 4.0.0.2.");

                FavoriteConfigurationElementCollection favorites = Settings.GetFavorites(false);

                foreach (FavoriteConfigurationElement favorite in favorites)
                {
                    switch (favorite.Url)
                    {
                        case "http://":
                        case "https://":
                            favorite.Url = null;
                            break;
                    }

                    if (favorite.ToolBarIcon.StartsWith(@"C:\Kohl\Terminals-bin\"))
                        favorite.ToolBarIcon = favorite.ToolBarIcon.Replace(@"C:\Kohl\Terminals-bin\", string.Empty);
                }

                // The changes need to be saved.
                Settings.SaveAndFinishDelayedUpdate();
            }

            if (Settings.ConfigVersion != null && Settings.ConfigVersion <= new Version(4, 0, 0, 3))
            {
                Log.Info("Fixing configuration to match new version " + AssemblyInfo.Version.ToString() + ".");

                string newConfig = null;
                using (FileStream fs = new FileStream(Settings.ConfigurationFileLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite & FileShare.Delete))
                {
                    using (StreamReader stream = new StreamReader(fs))
                    {
                        newConfig = stream.ReadToEnd();
                    }
                }

                newConfig = newConfig
                          .Replace(
                          @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>", @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<!-- Written by Oliver Kohl D.Sc. -->
<configuration>");
                
                // Save the new configuration file.
                SaveConfigurationFile(newConfig);
            }
            */
        }

        public static void SaveConfigurationFile(string fullyCompleteConfigurationContent)
        {
            try
            {
                // Pause the file observing
                Settings.PauseConfigObserving();

                using (FileStream fs = new FileStream(Settings.ConfigurationFileLocation, FileMode.Open, FileAccess.Write, FileShare.ReadWrite & FileShare.Delete))
                {
                    using (StreamWriter stream = new StreamWriter(fs))
                    {
                        stream.Write(fullyCompleteConfigurationContent);
                        stream.Flush();
                    }
                }

                // Configuration file must be reloaded in order to prevent some internal .NET exceptions.
                Settings.ForceReload();

                // Continue the file observing
                Settings.ContinueConfigObserving();

                Log.Info("The configruation file has been saved successfully.");
            }
            catch (Exception ex)
            {
                Log.Error("Unable to upgrade the configruation file.", ex);
            }
        }
    }
}