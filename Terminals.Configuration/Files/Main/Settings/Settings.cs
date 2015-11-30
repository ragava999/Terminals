using System;
using System.IO;
using System.Threading;
using System.Xml;
using Kohl.Framework.Logging;
using Terminals.Configuration.Files.Credentials;
using Terminals.Configuration.Files.Main.Keys;
using Terminals.Configuration.Files.Main.SpecialCommands;
using Terminals.Configuration.Files.Main.ToolTip;
using Terminals.Configuration.Security;

namespace Terminals.Configuration.Files.Main.Settings
{
    public static partial class Settings
    {
        #region Terminals Version
        public static Version ConfigVersion
        {
            get
            {
                // If we use the below uncommented code ->
                // we'd force terminals to replace the config
                // due to parsing errors before being able to kick
                // into the routine (e.g. UpdateConfig.UpdateObsoleteConfigVersions())

                /*
                string configVersion = GetSection().ConfigVersion;

                if (configVersion != String.Empty)
                    return new Version(configVersion);

                return null;
                */

                try
                {
                    XmlAttribute obj = LoadDocument(ConfigurationFileLocation).SelectSingleNode("/configuration/settings").Attributes["ConfigVersion"];

                    if (obj == null || obj.Value == null)
                        return null;

                    if (obj.Value != String.Empty)
                        return new Version(obj.Value);

                    return null;
                }
                catch
                {
                    return null;
                }
                
            }

            set
            {
                GetSection().ConfigVersion = value.ToString();
                SaveImmediatelyIfRequested();
            }
        }
        #endregion

        #region Plugin (2)
        public static void SetPluginOption<T>(string name, T value, T defaultValue = default(T))
        {
            GetSection().SetPluginOption(name, value, defaultValue);
            SaveImmediatelyIfRequested();
        }

        public static T GetPluginOption<T>(string name)
        {
            return GetSection().GetPluginOption<T>(name);
        }
        #endregion

        #region Rdp settings
        public static bool AskToReconnect
        {
            get { return GetSection().AskToReconnect; }

            set
            {
                GetSection().AskToReconnect = value;
                SaveImmediatelyIfRequested();
            }
        }
        #endregion

        public static CredentialStoreType CredentialStore
        {
        	get
        	{
        		TerminalsConfigurationSection config = GetSection();
                if (config != null)
                {
                    string dsp = config.CredentialStore;
                    return (CredentialStoreType) Enum.Parse(typeof (CredentialStoreType), dsp);
                }

        		return CredentialStoreType.Xml;
        	}

            set
            {
                GetSection().CredentialStore = value.ToString();
                SaveImmediatelyIfRequested();
            }
        }
        
        #region KeePass settings (2)
        public static string KeePassPath
        {
            get { return GetSection().KeePassPath; }

            set
            {
                GetSection().KeePassPath = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static string KeePassPassword
        {
            get { return GetSection().KeePassPassword; }

            set
            {
                GetSection().KeePassPassword = value;
                SaveImmediatelyIfRequested();
            }
        }

        #endregion

        
        #region RAdmin settings

        public static string RAdminProgramPath
        {
            get { return GetSection().RAdminProgramPath; }

            set
            {
                GetSection().RAdminProgramPath = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static ushort RAdminDefaultPort
        {
            get { return GetSection().RAdminDefaultPort; }

            set
            {
                GetSection().RAdminDefaultPort = value;
                SaveImmediatelyIfRequested();
            }
        }

        #endregion

        #region Putty settings

        public static string PuttyProgramPath
        {
            get { return GetSection().PuttyProgramPath; }

            set
            {
                GetSection().PuttyProgramPath = value;
                SaveImmediatelyIfRequested();
            }
        }

        #endregion

        #region General tab settings

        public static bool SortTabPagesByCaption
        {
            get { return GetSection().SortTabPagesByCaption; }

            set
            {
                GetSection().SortTabPagesByCaption = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static bool NeverShowTerminalsWindow
        {
            get { return GetSection().NeverShowTerminalsWindow; }

            set
            {
                GetSection().NeverShowTerminalsWindow = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static bool ShowUserNameInTitle
        {
            get { return GetSection().ShowUserNameInTitle; }

            set
            {
                GetSection().ShowUserNameInTitle = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static bool ShowInformationToolTips
        {
            get { return GetSection().ShowInformationToolTips; }

            set
            {
                GetSection().ShowInformationToolTips = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static bool ShowFullInformationToolTips
        {
            get { return GetSection().ShowFullInformationToolTips; }

            set
            {
                GetSection().ShowFullInformationToolTips = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static bool SingleInstance
        {
            get { return GetSection().SingleInstance; }

            set
            {
                GetSection().SingleInstance = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static bool ShowConfirmDialog
        {
            get { return GetSection().ShowConfirmDialog; }

            set
            {
                GetSection().ShowConfirmDialog = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static bool SaveConnectionsOnClose
        {
            get { return GetSection().SaveConnectionsOnClose; }

            set
            {
                GetSection().SaveConnectionsOnClose = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static bool MinimizeToTray
        {
            get { return GetSection().MinimizeToTray; }

            set
            {
                GetSection().MinimizeToTray = value;
                SaveImmediatelyIfRequested();
            }
        }

        // Validate server names
        public static bool ForceComputerNamesAsURI
        {
            get { return GetSection().ForceComputerNamesAsURI; }

            set
            {
                GetSection().ForceComputerNamesAsURI = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static bool WarnOnConnectionClose
        {
            get { return GetSection().WarnOnConnectionClose; }

            set
            {
                GetSection().WarnOnConnectionClose = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static bool AutoSetTag
        {
            get { return GetSection().AutoSetTag; }

            set
            {
                GetSection().AutoSetTag = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static bool AutoCaseTags
        {
            get { return GetSection().AutoCaseTags; }

            set
            {
                GetSection().AutoCaseTags = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static string DefaultDesktopShare
        {
            get { return GetSection().DefaultDesktopShare; }

            set
            {
                GetSection().DefaultDesktopShare = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static int PortScanTimeoutSeconds
        {
            get { return GetSection().PortScanTimeoutSeconds; }

            set
            {
                GetSection().PortScanTimeoutSeconds = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static bool InvertTabPageOrder
        {
            get { return GetSection().InvertTabPageOrder; }

            set
            {
                GetSection().InvertTabPageOrder = value;
                SaveImmediatelyIfRequested();
            }
        }
        #endregion

        #region Execute Before Connect tab settings

        public static bool ExecuteBeforeConnect
        {
            get { return GetSection().ExecuteBeforeConnect; }

            set
            {
                GetSection().ExecuteBeforeConnect = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static string ExecuteBeforeConnectCommand
        {
            get { return GetSection().ExecuteBeforeConnectCommand; }

            set
            {
                GetSection().ExecuteBeforeConnectCommand = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static string ExecuteBeforeConnectArgs
        {
            get { return GetSection().ExecuteBeforeConnectArgs; }

            set
            {
                GetSection().ExecuteBeforeConnectArgs = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static string ExecuteBeforeConnectInitialDirectory
        {
            get { return GetSection().ExecuteBeforeConnectInitialDirectory; }

            set
            {
                GetSection().ExecuteBeforeConnectInitialDirectory = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static bool ExecuteBeforeConnectWaitForExit
        {
            get { return GetSection().ExecuteBeforeConnectWaitForExit; }

            set
            {
                GetSection().ExecuteBeforeConnectWaitForExit = value;
                SaveImmediatelyIfRequested();
            }
        }

        #endregion

        #region Security tab settings

        private static string keyMaterial = string.Empty;

        public static bool IsMasterPasswordDefined
        {
            get { return !String.IsNullOrEmpty(GetMasterPasswordHash()); }
        }

        public static string KeyMaterial
        {
            get { return keyMaterial; }

            private set { keyMaterial = value; }
        }

        public static string DefaultDomain
        {
            get { return GetSection().DefaultDomain; }

            set
            {
                GetSection().DefaultDomain = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static string DefaultUsername
        {
            get { return GetSection().DefaultUsername; }

            set
            {
                GetSection().DefaultUsername = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static string DefaultPassword
        {
            get { return GetSection().DefaultPassword; }

            set
            {
                GetSection().DefaultPassword = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static bool UseAmazon
        {
            get { return GetSection().UseAmazon; }

            set
            {
                GetSection().UseAmazon = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static string AmazonAccessKey
        {
            get { return GetSection().AmazonAccessKey; }

            set
            {
                GetSection().AmazonAccessKey = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static string AmazonSecretKey
        {
            get { return GetSection().AmazonSecretKey; }

            set
            {
                GetSection().AmazonSecretKey = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static string AmazonBucketName
        {
            get { return GetSection().AmazonBucketName; }

            set
            {
                GetSection().AmazonBucketName = value;
                SaveImmediatelyIfRequested();
            }
        }

        private static string GetMasterPasswordHash()
        {
            // If we use the below uncommented code ->
            // we'd force terminals to replace the config
            // due to parsing errors before being able to kick
            // into the routine (e.g. UpdateConfig.UpdateObsoleteConfigVersions())

            /*
            return GetSection().TerminalsPassword;
            */

            try
            {
                XmlAttribute obj = Terminals.Configuration.Files.Main.Settings.Settings.LoadDocument(Terminals.Configuration.Files.Main.Settings.Settings.ConfigurationFileLocation).SelectSingleNode("/configuration/settings").Attributes["terminalsPassword"];

                if (obj == null || obj.Value == null)
                    return null;

                if (obj.Value != String.Empty)
                    return obj.Value.ToString();

                return null;
            }
            catch
            {
                return null;
            }
        }

        public static void UpdateMasterPassword(string newPassword)
        {
            TerminalsConfigurationSection configSection = GetSection();

            UpdateAllFavoritesPasswords(configSection, newPassword);
            // start of not secured transaction. Old key is still present,
            // but passwords are already encrypted by newKey
            configSection.TerminalsPassword = newPassword;
            SaveImmediatelyIfRequested();

            // finish transaction, the passwords now reflect the new key
            UpdateKeyMaterial(newPassword);
        }

        /// <summary>
        ///     During this procedure, the old master key material should be still present.
        ///     This finds all stored passwords and updates them to reflect new key material.
        /// </summary>
        private static void UpdateAllFavoritesPasswords(TerminalsConfigurationSection configSection, string newMasterPassword)
        {
            string newKeyMaterial = GetKeyMaterial(newMasterPassword);
            configSection.UpdatePasswordsByNewKeyMaterial(newKeyMaterial);
            UpdateFavoritePasswordsByNewKeyMaterial(newKeyMaterial);
            StoredCredentials.UpdatePasswordsByNewKeyMaterial(newKeyMaterial);
        }

        public static Boolean IsMasterPasswordValid(string passwordToCheck)
        {
            String hashToCheck = PasswordFunctions.ComputeMasterPasswordHash(passwordToCheck);
            if (GetMasterPasswordHash() == hashToCheck)
            {
                UpdateKeyMaterial(passwordToCheck);
                return true;
            }

            return false;
        }

        private static void UpdateKeyMaterial(String password)
        {
            KeyMaterial = GetKeyMaterial(password);
        }

        private static string GetKeyMaterial(string password)
        {
            if (string.IsNullOrEmpty(password))
                return string.Empty;
            String hashToCheck = PasswordFunctions.ComputeMasterPasswordHash(password);
            return PasswordFunctions.ComputeMasterPasswordHash(password + hashToCheck);
        }

        #endregion

        #region Flickr tab settings

        public static string FlickrToken
        {
            get { return GetSection().FlickrToken; }

            set
            {
                GetSection().FlickrToken = value;
                SaveImmediatelyIfRequested();
            }
        }

        #endregion

        #region Proxy tab settings

        public static bool UseProxy
        {
            get { return GetSection().UseProxy; }

            set
            {
                GetSection().UseProxy = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static string ProxyAddress
        {
            get { return GetSection().ProxyAddress; }

            set
            {
                GetSection().ProxyAddress = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static int ProxyPort
        {
            get { return GetSection().ProxyPort; }

            set
            {
                GetSection().ProxyPort = value;
                SaveImmediatelyIfRequested();
            }
        }

        #endregion

        #region Screen capture tab settings

        public static bool EnableCaptureToClipboard
        {
            get { return GetSection().EnableCaptureToClipboard; }

            set
            {
                GetSection().EnableCaptureToClipboard = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static bool EnableCaptureToFolder
        {
            get { return GetSection().EnableCaptureToFolder; }

            set
            {
                GetSection().EnableCaptureToFolder = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static bool EnabledCaptureToFolderAndClipBoard
        {
            get { return EnableCaptureToClipboard || EnableCaptureToFolder; }
        }

        public static bool AutoSwitchOnCapture
        {
            get { return GetSection().AutoSwitchOnCapture; }

            set
            {
                GetSection().AutoSwitchOnCapture = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static string CaptureRoot
        {
            get
            {
                string root = GetSection().CaptureRoot;
                if (string.IsNullOrEmpty(root))
                    root = GetDefaultCaptureRootDirectory();

                return EnsureCaptureDirectory(root);
            }

            set
            {
                GetSection().CaptureRoot = value;
                SaveImmediatelyIfRequested();
            }
        }

        private static string EnsureCaptureDirectory(string root)
        {
            try
            {
                if (!Directory.Exists(root))
                {
                    Log.Info(string.Format("Capture root folder does not exist:{0}. Lets try to create it now.", root));
                    Directory.CreateDirectory(root);
                }
            }
            catch (Exception exception)
            {
                root = GetDefaultCaptureRootDirectory();
                string logMessage = string.Format(
                    "Capture root could not be created, set it to the default value : {0}", root);
                Log.Error(logMessage, exception);
                SwitchToDefaultDirectory(root);
            }

            return root;
        }

        private static void SwitchToDefaultDirectory(string defaultRoot)
        {
            try
            {
                Directory.CreateDirectory(defaultRoot);
                CaptureRoot = defaultRoot;
            }
            catch (Exception exception)
            {
                Log.Error(@"Capture root could not be created again. Abort!", exception);
            }
        }

        private static string GetDefaultCaptureRootDirectory()
        {
            return Path.Combine(Kohl.Framework.Info.AssemblyInfo.DirectoryConfigFiles, "Terminals Captures");
        }

        #endregion

        #region More tab settings

        public static bool EnableFavoritesPanel
        {
            get { return GetSection().EnableFavoritesPanel; }

            set
            {
                GetSection().EnableFavoritesPanel = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static bool EnableGroupsMenu
        {
            get { return GetSection().EnableGroupsMenu; }

            set
            {
                GetSection().EnableGroupsMenu = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static bool AutoExapandTagsPanel
        {
            get { return GetSection().AutoExapandTagsPanel; }

            set
            {
                GetSection().AutoExapandTagsPanel = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static SortProperties DefaultSortProperty
        {
            get
            {
                TerminalsConfigurationSection config = GetSection();
                if (config != null)
                {
                    string dsp = config.DefaultSortProperty;
                    SortProperties prop = (SortProperties) Enum.Parse(typeof (SortProperties), dsp);
                    return prop;
                }

                return SortProperties.ConnectionName;
            }

            set
            {
                GetSection().DefaultSortProperty = value.ToString();
                SaveImmediatelyIfRequested();
            }
        }

        public static bool Office2007BlackFeel
        {
            get { return GetSection().Office2007BlackFeel; }

            set
            {
                GetSection().Office2007BlackFeel = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static bool Office2007BlueFeel
        {
            get { return GetSection().Office2007BlueFeel; }

            set
            {
                GetSection().Office2007BlueFeel = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static string FavoritesFont
        {
            get { return GetSection().FavoritesFont; }

            set
            {
                GetSection().FavoritesFont = value;
                SaveImmediatelyIfRequested();
            }
        }
        
        public static string FavoritesFontBackColor
        {
            get { return GetSection().FavoritesFontBackColor; }

            set
            {
                GetSection().FavoritesFontBackColor = value;
                SaveImmediatelyIfRequested();
            }
        }
        
        public static string FavoritesFontForeColor
        {
            get { return GetSection().FavoritesFontForeColor; }

            set
            {
                GetSection().FavoritesFontForeColor = value;
                SaveImmediatelyIfRequested();
            }
        }
        
        public static int FavoritesImageHeight
        {
            get { return GetSection().FavoritesImageHeight; }

            set
            {
                GetSection().FavoritesImageHeight = value;
                SaveImmediatelyIfRequested();
            }
        }
        
        public static int FavoritesImageWidth
        {
            get { return GetSection().FavoritesImageWidth; }

            set
            {
                GetSection().FavoritesImageWidth = value;
                SaveImmediatelyIfRequested();
            }
        }
        #endregion
        
        #region Mainform control settings

        public static bool HideFavoritesFromQuickMenu
        {
            get { return GetSection().HideFavoritesFromQuickMenu; }

            set
            {
                GetSection().HideFavoritesFromQuickMenu = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static int FavoritePanelWidth
        {
            get { return GetSection().FavoritePanelWidth; }

            set
            {
                GetSection().FavoritePanelWidth = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static bool ShowFavoritePanel
        {
            get { return GetSection().ShowFavoritePanel; }

            set
            {
                GetSection().ShowFavoritePanel = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static ToolStripSettingElementCollection ToolbarSettings
        {
            get { return GetSection().ToolStripSettings; }
            set
            {
                GetSection().ToolStripSettings = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static bool ToolbarsLocked
        {
            get { return GetSection().ToolbarsLocked; }

            set
            {
                GetSection().ToolbarsLocked = value;
                SaveImmediatelyIfRequested();
            }
        }

        #endregion

        #region Startup settings

        public static int ImageStyle
        {
            get { return GetSection().ImageStyle; }

            set
            {
                GetSection().ImageStyle = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static string DashBoardBackgroundColor
        {
            get { return GetSection().DashBoardBackgroundColor; }

            set
            {
                GetSection().DashBoardBackgroundColor = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static string ImagePath
        {
            get { return GetSection().ImagePath; }

            set
            {
                GetSection().ImagePath = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static string UpdateSource
        {
            get { return GetSection().UpdateSource; }

            set
            {
                GetSection().UpdateSource = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static bool ShowWizard
        {
            get { return GetSection().ShowWizard; }

            set
            {
                GetSection().ShowWizard = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static string LicenseKey
        {
            get { return GetSection().LicenseKey; }

            set
            {
                GetSection().LicenseKey = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static string PsexecLocation
        {
            get { return GetSection().PsexecLocation; }

            set
            {
                GetSection().PsexecLocation = value;
                SaveImmediatelyIfRequested();
            }
        }
        #endregion

        #region MRU lists

        public static string[] MRUServerNames
        {
            get { return GetSection().ServersMRU.ToSortedArray(); }
        }

        public static string[] MRUDomainNames
        {
            get { return GetSection().DomainsMRU.ToSortedArray(); }
        }

        public static string[] MRUUserNames
        {
            get { return GetSection().UsersMRU.ToSortedArray(); }
        }

        #endregion

        #region Tags/Favorite lists Settings
        public static string ExpandedFavoriteNodes
        {
            get { return GetSection().ExpandedFavoriteNodes; }

            set
            {
                GetSection().ExpandedFavoriteNodes = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static string ExpandedHistoryNodes
        {
            get { return GetSection().ExpandedHistoryNodes; }

            set
            {
                GetSection().ExpandedHistoryNodes = value;
                SaveImmediatelyIfRequested();
            }
        }
        #endregion

        #region Public
        public static SpecialCommandConfigurationElementCollection SpecialCommands
        {
            get { return GetSection().SpecialCommands; }

            set
            {
                GetSection().SpecialCommands = value;
                SaveImmediatelyIfRequested();
            }
        }

        public static string[] SavedConnections
        {
            get { return GetSection().SavedConnections.ToList().ToArray(); }
        }

        public static KeysSection SSHKeys
        {
            get
            {
                KeysSection keys = Config.Sections["SSH"] as KeysSection;
                if (keys == null)
                {
                    // The section wasn't found, so add it.
                    keys = new KeysSection();
                    Config.Sections.Add("SSH", keys);
                }

                return keys;
            }
        }

        public static string ToTitleCase(string name)
        {
            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(name.ToLower());
        }

        public static void AddServerMRUItem(string name)
        {
            GetSection().ServersMRU.AddByName(name);
            SaveImmediatelyIfRequested();
        }

        public static void AddDomainMRUItem(string name)
        {
            GetSection().DomainsMRU.AddByName(name);
            SaveImmediatelyIfRequested();
        }

        public static void AddUserMRUItem(string name)
        {
            GetSection().UsersMRU.AddByName(name);
            SaveImmediatelyIfRequested();
        }

        public static void CreateSavedConnectionsList(string[] names)
        {
            var section = GetSection();
            section.SavedConnections.Clear();
            SaveImmediatelyIfRequested();
            foreach (string name in names)
            {
                section.SavedConnections.AddByName(name);
                SaveImmediatelyIfRequested();
            }
        }

        public static void ClearSavedConnectionsList()
        {
            GetSection().SavedConnections.Clear();
            SaveImmediatelyIfRequested();
        }
        #endregion
    }
}