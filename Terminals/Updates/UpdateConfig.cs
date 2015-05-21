using Kohl.Framework.Logging;
using Terminals.Configuration.Files.Main.Favorites;
using System.IO;

namespace Terminals.Updates
{
    using System;
    using Kohl.Framework.Info;
    using Configuration.Files.Main.Settings;

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
                Log.Info(string.Format("Updating your {0} configuration file from version {1} to version {2}", AssemblyInfo.Title(), Settings.ConfigVersion, AssemblyInfo.Version));

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
            return config.Insert(startIndex, name + "=\""+ newValue);
        }

        private static void UpdateObsoleteConfigVersions()
        {
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
                        case "http://www.kohl.bz/":
                        case "http://www.kohl.bz":
                        case "http://www.omantl.at/":
                        case "http://www.codeplex.com/terminals":
                        case "http://www.omantl.at/terminals":
                        case "http://www.omantl.at":
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
        
        /*
        private static void UpdateObsoleteConfigVersions()
        {
            // The config version might be null, if Terminals has been started the first time
            // without a Terminals.config or if the config is wrong that it will be replaced by
            // an empty one -> and therefore the ConfigVersion will be empty again.

            // We've changed the sections in the Terminals.config 3.8
            // So update any version below 3.7 to match the new 3.8 structure.
            if (Settings.ConfigVersion != null && Settings.ConfigVersion <= new Version(3, 7, 0, 0))
            {
                //Terminals 3.7.0.0
                //Terminals.Configuration.KeysSection, Terminals.Configuration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
                //Terminals.Configuration.TerminalsConfigurationSection, Terminals.Configuration
                //
                //Terminals 3.8.0.0
                //Terminals.Configuration.Files.Main.Keys.KeysSection, Terminals.Configuration, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
                //Terminals.Configuration.Files.Main.TerminalsConfigurationSection, Terminals.Configuration
                string newConfig =
                    System.IO.File.ReadAllText(Settings.ConfigurationFileLocation)
                          .Replace("type=\"Terminals.Configuration.KeysSection, Terminals.Configuration",
                                   "type=\"Terminals.Configuration.Files.Main.Keys.KeysSection, Terminals.Configuration")
                          .Replace(
                              "type=\"Terminals.Configuration.TerminalsConfigurationSection, Terminals.Configuration",
                              "type=\"Terminals.Configuration.Files.Main.TerminalsConfigurationSection, Terminals.Configuration");

                // Remove the savedCredentials attribute
                newConfig = newConfig.ReplaceAttribute("savedCredentials");

                // Save the new configuration file.
                System.IO.File.WriteAllText(Settings.ConfigurationFileLocation, newConfig);
            }
            
            if (Settings.ConfigVersion != null && Settings.ConfigVersion <= new Version(3, 8, 0, 0))
            {
                // Remove the savedCredentials attribute
                string newConfig = System.IO.File.ReadAllText(Settings.ConfigurationFileLocation).ReplaceAttribute("telnet");
                newConfig = newConfig.ReplaceAttribute("telnetrows");
                newConfig = newConfig.ReplaceAttribute("telnetcols");

                //
                //[ConfigurationProperty("telnet", IsRequired = false, DefaultValue = true)]
                //public Boolean Telnet
                //{
                //    get { return (Boolean)this["telnet"]; }
                //    set { this["telnet"] = value; }
                //}
                //
                //[ConfigurationProperty("telnetrows", IsRequired = false, DefaultValue = 33)]
                //public Int32 TelnetRows
                //{
                //    get { return (Int32)this["telnetrows"]; }
                //    set { this["telnetrows"] = value; }
                //}
                //
                //[ConfigurationProperty("telnetcols", IsRequired = false, DefaultValue = 110)]
                //public Int32 TelnetCols
                //{
                //    get { return (Int32)this["telnetcols"]; }
                //    set { this["telnetcols"] = value; }
                //}
                //

                newConfig = newConfig.ReplaceAttribute("telnetfont");
                newConfig = newConfig.ReplaceAttribute("telnetbackcolor");
                newConfig = newConfig.ReplaceAttribute("telnettextcolor");
                newConfig = newConfig.ReplaceAttribute("telnetcursorcolor");


                //[ConfigurationProperty("telnetfont", IsRequired = false, DefaultValue = "[Font: Name=Courier New, Size=10, Units=3, GdiCharSet=0, GdiVerticalFont=False, Style=0]")]
                //public String TelnetFont
                //{
                //    get
                //    {
                //        String font = (String)this["telnetfont"];
                //        if (String.IsNullOrEmpty(font))
                //            font = "[Font: Name=Courier New, Size=10, Units=3, GdiCharSet=0, GdiVerticalFont=False, Style=0]";
                //
                //        return font;
                //    }
                //    set { this["telnetfont"] = value; }
                //}
                //
                //[ConfigurationProperty("telnetbackcolor", IsRequired = false, DefaultValue = "Black")]
                //public String TelnetBackColor
                //{
                //    get { return (String)this["telnetbackcolor"]; }
                //    set { this["telnetbackcolor"] = value; }
                //}
                //
                //[ConfigurationProperty("telnettextcolor", IsRequired = false, DefaultValue = "White")]
                //public String TelnetTextColor
                //{
                //    get { return (String)this["telnettextcolor"]; }
                //    set { this["telnettextcolor"] = value; }
                //} 
                //
                //[ConfigurationProperty("telnetcursorcolor", IsRequired = false, DefaultValue = "Green")]
                //public String TelnetCursorColor
                //{
                //    get { return (String)this["telnetcursorcolor"]; }
                //    set { this["telnetcursorcolor"] = value; }
                //}


                // Save the new configuration file.
                System.IO.File.WriteAllText(Settings.ConfigurationFileLocation, newConfig);

                FavoriteConfigurationElementCollection favorites = Settings.GetFavorites();


                // Sample for updating the news favorites
                //FavoriteConfigurationElement newsFavorite = favorites[FavoritesFactory.TerminalsReleasesFavoriteName];

                //if (newsFavorite != null)
                //{
                //    newsFavorite.Url = AssemblyInfo.Url;
                //    Settings.SaveDefaultFavorite(newsFavorite);
                //}


                foreach (FavoriteConfigurationElement favorite in favorites)
                {
                    favorite.ClearHtmlFormFields();
                }
            }
        }
        */
    }
}