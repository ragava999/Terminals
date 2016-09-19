using System;
using System.Configuration;
using Terminals.Configuration.Files.Main.Favorites;
using Terminals.Configuration.Files.Main.Groups;
using Terminals.Configuration.Files.Main.MRU;
using Terminals.Configuration.Files.Main.SpecialCommands;
using Terminals.Configuration.Files.Main.ToolTip;
using Terminals.Configuration.Security;

namespace Terminals.Configuration.Files.Main
{
    public class TerminalsConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("specialCommands")]
        [ConfigurationCollection(typeof (SpecialCommandConfigurationElement))]
        public SpecialCommandConfigurationElementCollection SpecialCommands
        {
            get { return (SpecialCommandConfigurationElementCollection) this["specialCommands"]; }
            set { this["specialCommands"] = value; }
        }

        [ConfigurationProperty("savedConnectionsList")]
        [ConfigurationCollection(typeof (MRUItemConfigurationElementCollection))]
        public MRUItemConfigurationElementCollection SavedConnections
        {
            get { return (MRUItemConfigurationElementCollection) this["savedConnectionsList"]; }
            set { this["savedConnectionsList"] = value; }
        }

        #region Plugin (3)
        [ConfigurationCollection(typeof(PluginConfigurationElementCollection))]
        [ConfigurationPropertyAttribute("pluginOptions", IsRequired = false, IsKey = false, IsDefaultCollection = false)]
        public PluginConfigurationElementCollection PluginOptions
        {
            get { return (PluginConfigurationElementCollection)this["pluginOptions"]; }
            set { this["pluginOptions"] = value; }
        }

        public void SetPluginOption<T>(string name, T value, T defaultValue = default(T))
        {
            PluginConfiguration pluginConfiguration = this.PluginOptions[name];
            if (pluginConfiguration == null)
            {
                pluginConfiguration = new PluginConfiguration() { Name = name };
                this.PluginOptions.Add(pluginConfiguration);
            }

            pluginConfiguration.SetValue(value, defaultValue);
        }

        public T GetPluginOption<T>(string name)
        {
            PluginConfiguration pluginConfiguration = this.PluginOptions[name];

            T returnValue = default(T);

            if (pluginConfiguration != null)
                returnValue = pluginConfiguration.GetValue<T>();

            return returnValue;
        }
        #endregion

        public void UpdatePasswordsByNewKeyMaterial(string newKeyMaterial)
        {
        	this.EncryptedKeePassPassword = PasswordFunctions.EncryptPassword(this.KeePassPassword, newKeyMaterial);
            this.EncryptedDefaultPassword = PasswordFunctions.EncryptPassword(this.DefaultPassword, newKeyMaterial);
        }

        #region Terminals Version

        [ConfigurationProperty("ConfigVersion")]
        public String ConfigVersion
        {
            get
            {
                return (String)this["ConfigVersion"];
            }

            set { this["ConfigVersion"] = value; }
        }

        #endregion

        #region RAdmin section
        [ConfigurationProperty("RAdminProgramPath")]
        public string RAdminProgramPath
        {
            get { return (String) this["RAdminProgramPath"]; }

            set { this["RAdminProgramPath"] = value; }
        }

        [ConfigurationProperty("RAdminDefaultPort")]
        public ushort RAdminDefaultPort
        {
            get
            {
                ushort port = 4899;

                if (this["RAdminDefaultPort"] != null && this["RAdminDefaultPort"].ToString() != string.Empty)
                {
                    ushort.TryParse(this["RAdminDefaultPort"].ToString(), out port);
                }

                if (port <= 0)
                    port = 4899;

                return port;
            }

            set { this["RAdminDefaultPort"] = value; }
        }

        #endregion

        #region Putty section

        [ConfigurationProperty("PuttyProgramPath")]
        public string PuttyProgramPath
        {
            get { return (String) this["PuttyProgramPath"]; }

            set { this["PuttyProgramPath"] = value; }
        }

        #endregion

        #region General section
        [ConfigurationProperty("SortTabPagesByCaption", DefaultValue = true)]
        public bool SortTabPagesByCaption
        {
            get { return (bool)this["SortTabPagesByCaption"]; }
            set { this["SortTabPagesByCaption"] = value; }
        }

        [ConfigurationProperty("checkForNewRelease", DefaultValue = true, IsRequired = false)]
        public bool CheckForNewRelease
        {
            get { return (bool)this["checkForNewRelease"]; }
            set { this["checkForNewRelease"] = value; }
        }

        [ConfigurationProperty("NeverShowTerminalsWindow")]
        public bool NeverShowTerminalsWindow
        {
            get { return (bool) this["NeverShowTerminalsWindow"]; }
            set { this["NeverShowTerminalsWindow"] = value; }
        }

        [ConfigurationProperty("showUserNameInTitle")]
        public bool ShowUserNameInTitle
        {
            get { return (bool) this["showUserNameInTitle"]; }
            set { this["showUserNameInTitle"] = value; }
        }

        [ConfigurationProperty("showInformationToolTips")]
        public bool ShowInformationToolTips
        {
            get { return (bool) this["showInformationToolTips"]; }
            set { this["showInformationToolTips"] = value; }
        }

        [ConfigurationProperty("showFullInformationToolTips")]
        public bool ShowFullInformationToolTips
        {
            get { return (bool) this["showFullInformationToolTips"]; }
            set { this["showFullInformationToolTips"] = value; }
        }

        [ConfigurationProperty("singleInstance")]
        public bool SingleInstance
        {
            get { return (bool) this["singleInstance"]; }
            set { this["singleInstance"] = value; }
        }

        [ConfigurationProperty("showConfirmDialog")]
        public bool ShowConfirmDialog
        {
            get { return (bool) this["showConfirmDialog"]; }
            set { this["showConfirmDialog"] = value; }
        }

        [ConfigurationProperty("saveConnectionsOnClose")]
        public bool SaveConnectionsOnClose
        {
            get { return (bool) this["saveConnectionsOnClose"]; }
            set { this["saveConnectionsOnClose"] = value; }
        }

        [ConfigurationProperty("minimizeToTray", DefaultValue = true)]
        public bool MinimizeToTray
        {
            get
            {
                if (this["minimizeToTray"] == null || this["minimizeToTray"].ToString() == string.Empty) return true;
                bool min = true;
                bool.TryParse(this["minimizeToTray"].ToString(), out min);
                return min;
            }
            set { this["minimizeToTray"] = value; }
        }

        [ConfigurationProperty("forceComputerNamesAsURI", DefaultValue = true)]
        public bool ForceComputerNamesAsURI
        {
            get
            {
                if (this["forceComputerNamesAsURI"] == null ||
                    this["forceComputerNamesAsURI"].ToString() == string.Empty) return true;
                bool min = true;
                bool.TryParse(this["forceComputerNamesAsURI"].ToString(), out min);
                return min;
            }
            set { this["forceComputerNamesAsURI"] = value; }
        }

        [ConfigurationProperty("warnOnConnectionClose", DefaultValue = true)]
        public bool WarnOnConnectionClose
        {
            get
            {
                if (this["warnOnConnectionClose"] == null || this["warnOnConnectionClose"].ToString() == string.Empty)
                    return true;
                bool min = true;
                bool.TryParse(this["warnOnConnectionClose"].ToString(), out min);
                return min;
            }
            set { this["warnOnConnectionClose"] = value; }
        }

        [ConfigurationProperty("autoSetTag", DefaultValue = true)]
        public bool AutoSetTag
        {
            get { return (bool)this["autoSetTag"]; }
            set { this["autoSetTag"] = value; }
        }

        [ConfigurationProperty("autoCaseTags")]
        public bool AutoCaseTags
        {
            get { return (bool) this["autoCaseTags"]; }
            set { this["autoCaseTags"] = value; }
        }

        [ConfigurationProperty("defaultDesktopShare")]
        public string DefaultDesktopShare
        {
            get { return (string) this["defaultDesktopShare"]; }
            set { this["defaultDesktopShare"] = value; }
        }

        [ConfigurationProperty("portScanTimeoutSeconds", DefaultValue = 5)]
        public int PortScanTimeoutSeconds
        {
            get
            {
                int timeout = 5;
                if (this["portScanTimeoutSeconds"] != null && this["portScanTimeoutSeconds"].ToString() != string.Empty)
                {
                    int.TryParse(this["portScanTimeoutSeconds"].ToString(), out timeout);
                }
                return timeout;
            }
            set { this["portScanTimeoutSeconds"] = value; }
        }

        [ConfigurationProperty("invertTabPageOrder", DefaultValue=false)]
        public bool InvertTabPageOrder
        {
            get { return (bool)this["invertTabPageOrder"]; }
            set { this["invertTabPageOrder"] = value; }
        }
        #endregion

        #region Rdp settings
        [ConfigurationProperty("askToReconnect")]
        public bool AskToReconnect
        {
            get { return (bool)this["askToReconnect"]; }
            set { this["askToReconnect"] = value; }
        }
        #endregion
        
        [ConfigurationProperty("credentialStore", DefaultValue = "Xml")]
        public string CredentialStore
        {
            get
            {
                if (this["credentialStore"] == null || this["credentialStore"].ToString() == string.Empty)
                    return "Xml";
                return this["credentialStore"].ToString();
            }
            set { this["credentialStore"] = value; }
        }
        
        #region KeePass section (3)
        [ConfigurationProperty("KeePassPath")]
        public string KeePassPath
        {
            get { return (String) this["KeePassPath"]; }

            set { this["KeePassPath"] = value; }
        }
        
        [ConfigurationProperty("encryptedKeePassPassword", IsRequired = false)]
        public string EncryptedKeePassPassword
        {
            get { return (string) this["encryptedKeePassPassword"]; }
            set { this["encryptedKeePassPassword"] = value; }
        }
        
        public string KeePassPassword
        {
        	get { return PasswordFunctions.DecryptPassword(this.EncryptedKeePassPassword, "KeePassPassword"); }
			set { this.EncryptedKeePassPassword = PasswordFunctions.EncryptPassword(value); }
        }
        #endregion
        
        #region Execute Before Connect section

        [ConfigurationProperty("executeBeforeConnect")]
        public bool ExecuteBeforeConnect
        {
            get { return (bool) this["executeBeforeConnect"]; }
            set { this["executeBeforeConnect"] = value; }
        }

        [ConfigurationProperty("executeBeforeConnectCommand")]
        public string ExecuteBeforeConnectCommand
        {
            get { return (string) this["executeBeforeConnectCommand"]; }
            set { this["executeBeforeConnectCommand"] = value; }
        }

        [ConfigurationProperty("executeBeforeConnectArgs")]
        public string ExecuteBeforeConnectArgs
        {
            get { return (string) this["executeBeforeConnectArgs"]; }
            set { this["executeBeforeConnectArgs"] = value; }
        }

        [ConfigurationProperty("executeBeforeConnectInitialDirectory")]
        public string ExecuteBeforeConnectInitialDirectory
        {
            get { return (string) this["executeBeforeConnectInitialDirectory"]; }
            set { this["executeBeforeConnectInitialDirectory"] = value; }
        }

        [ConfigurationProperty("executeBeforeConnectWaitForExit")]
        public bool ExecuteBeforeConnectWaitForExit
        {
            get { return (bool) this["executeBeforeConnectWaitForExit"]; }
            set { this["executeBeforeConnectWaitForExit"] = value; }
        }

        #endregion

        #region Security section

        [ConfigurationProperty("terminalsPassword", DefaultValue = "")]
        public string TerminalsPassword
        {
            get { return Convert.ToString(this["terminalsPassword"]); }
            set
            {
                if (string.IsNullOrEmpty(value))
                    this["terminalsPassword"] = string.Empty;
                else //hash the password
                    this["terminalsPassword"] = PasswordFunctions.ComputeMasterPasswordHash(value);
            }
        }

        [ConfigurationProperty("defaultDomain", IsRequired = false)]
        public string DefaultDomain
        {
            get { return (string) this["defaultDomain"]; }
            set { this["defaultDomain"] = value; }
        }

        [ConfigurationProperty("defaultUsername", IsRequired = false)]
        public string DefaultUsername
        {
            get { return (string) this["defaultUsername"]; }
            set { this["defaultUsername"] = value; }
        }

        [ConfigurationProperty("encryptedDefaultPassword", IsRequired = false)]
        public string EncryptedDefaultPassword
        {
            get { return (string) this["encryptedDefaultPassword"]; }
            set { this["encryptedDefaultPassword"] = value; }
        }

        public string DefaultPassword
        {
            get { return PasswordFunctions.DecryptPassword(this.EncryptedDefaultPassword, "DefaultPassword"); }
            set { this.EncryptedDefaultPassword = PasswordFunctions.EncryptPassword(value); }
        }
        #endregion

        #region Proxy section

		[ConfigurationProperty("proxyUseAuth", DefaultValue = true)]
		public bool ProxyUseAuth
        {
			get { return (bool) this["proxyUseAuth"]; }
			set { this["proxyUseAuth"] = value; }
        }
        
		[ConfigurationProperty("proxyUseAuthCustom", DefaultValue = false)]
		public bool ProxyUseAuthCustom
        {
			get { return (bool) this["proxyUseAuthCustom"]; }
			set { this["proxyUseAuthCustom"] = value; }
        }
	
		[ConfigurationProperty("proxyUse")]
		public bool ProxyUse
        {
			get { return (bool) this["proxyUse"]; }
			set { this["proxyUse"] = value; }
        }
        
		[ConfigurationProperty("proxyAutoDetect", DefaultValue = true)]
		public bool ProxyAutoDetect
        {
			get { return (bool) this["proxyAutoDetect"]; }
			set { this["proxyAutoDetect"] = value; }
        }

		[ConfigurationProperty("proxyXmlCredentialSetName", IsRequired = false, DefaultValue = "")]
		public string ProxyXmlCredentialSetName
		{
			get { return (string) this["proxyXmlCredentialSetName"]; }
			set { this["proxyXmlCredentialSetName"] = value; }
		}

		[ConfigurationProperty("proxyPassword", IsRequired = false, DefaultValue = "")]
		public string ProxyPassword
		{
			get { return (string) this["proxyPassword"]; }
			set { this["proxyPassword"] = value; }
		}

		[ConfigurationProperty("proxyUserName", IsRequired = false, DefaultValue = "")]
		public string ProxyUserName
		{
			get { return (string) this["proxyUserName"]; }
			set { this["proxyUserName"] = value; }
		}

		[ConfigurationProperty("proxyDomainName", IsRequired = false, DefaultValue = "")]
		public string ProxyDomainName
		{
			get { return (string) this["proxyDomainName"]; }
			set { this["proxyDomainName"] = value; }
		}

        [ConfigurationProperty("proxyAddress")]
        public string ProxyAddress
        {
            get { return (string) this["proxyAddress"]; }
            set { this["proxyAddress"] = value; }
        }

        [ConfigurationProperty("proxyPort")]
        public int ProxyPort
        {
            get { return (int) this["proxyPort"]; }
            set { this["proxyPort"] = value; }
        }

        #endregion

        #region Screen capture section

        [ConfigurationProperty("enableCaptureToClipboard")]
        public bool EnableCaptureToClipboard
        {
            get { return (bool) this["enableCaptureToClipboard"]; }
            set { this["enableCaptureToClipboard"] = value; }
        }

        [ConfigurationProperty("enableCaptureToFolder")]
        public bool EnableCaptureToFolder
        {
            get { return (bool) this["enableCaptureToFolder"]; }
            set { this["enableCaptureToFolder"] = value; }
        }

        [ConfigurationProperty("autoSwitchOnCapture", DefaultValue = true)]
        public bool AutoSwitchOnCapture
        {
            get { return (bool) this["autoSwitchOnCapture"]; }
            set { this["autoSwitchOnCapture"] = value; }
        }

        [ConfigurationProperty("captureRoot")]
        public string CaptureRoot
        {
            get { return (string) this["captureRoot"]; }
            set { this["captureRoot"] = value; }
        }

        #endregion

        #region More section

        [ConfigurationProperty("enableFavoritesPanel", DefaultValue = true)]
        public bool EnableFavoritesPanel
        {
            get
            {
                if (this["enableFavoritesPanel"] == null || this["enableFavoritesPanel"].ToString() == string.Empty)
                    return true;
                bool min = true;
                bool.TryParse(this["enableFavoritesPanel"].ToString(), out min);
                return min;
            }
            set { this["enableFavoritesPanel"] = value; }
        }

        [ConfigurationProperty("enableGroupsMenu", DefaultValue = true)]
        public bool EnableGroupsMenu
        {
            get
            {
                if (this["enableGroupsMenu"] == null || this["enableGroupsMenu"].ToString() == string.Empty)
                    return true;
                bool min = true;
                bool.TryParse(this["enableGroupsMenu"].ToString(), out min);
                return min;
            }
            set { this["enableGroupsMenu"] = value; }
        }

        [ConfigurationProperty("autoExapandTagsPanel", DefaultValue = false)]
        public bool AutoExapandTagsPanel
        {
            get { return (bool) this["autoExapandTagsPanel"]; }
            set { this["autoExapandTagsPanel"] = value; }
        }

        [ConfigurationProperty("defaultSortProperty", DefaultValue = "ConnectionName")]
        public string DefaultSortProperty
        {
            get
            {
                if (this["defaultSortProperty"] == null || this["defaultSortProperty"].ToString() == string.Empty)
                    return "ConnectionName";
                return this["defaultSortProperty"].ToString();
            }
            set { this["defaultSortProperty"] = value; }
        }

        [ConfigurationProperty("office2007BlueFeel", DefaultValue = false)]
        public bool Office2007BlueFeel
        {
            get { return (bool) this["office2007BlueFeel"]; }
            set { this["office2007BlueFeel"] = value; }
        }

        [ConfigurationProperty("office2007BlackFeel", DefaultValue = false)]
        public bool Office2007BlackFeel
        {
            get { return (bool) this["office2007BlackFeel"]; }
            set { this["office2007BlackFeel"] = value; }
        }

        [ConfigurationProperty("favoritesFont", DefaultValue = "[Font: Name=Microsoft Sans Serif, Size=8.25, Units=3, GdiCharSet=0, GdiVerticalFont=False, Style=0]")]
        public string FavoritesFont
        {
            get { return (string) this["favoritesFont"]; }
            set { this["favoritesFont"] = value; }
        }
        
        [ConfigurationProperty("favoritesFontBackColor", DefaultValue = "FFFFFFFF (White)")]
        public string FavoritesFontBackColor
        {
            get { return (string) this["favoritesFontBackColor"]; }
            set { this["favoritesFontBackColor"] = value; }
        }
        
        [ConfigurationProperty("favoritesFontForeColor", DefaultValue = "FF000000 (Black)")]
        public string FavoritesFontForeColor
        {
            get { return (string) this["favoritesFontForeColor"]; }
            set { this["favoritesFontForeColor"] = value; }
        }
        
        [ConfigurationProperty("favoritesImageHeight", DefaultValue = 21)]
        public int FavoritesImageHeight
        {
        	get { return (int)this["favoritesImageHeight"]; }
            set { this["favoritesImageHeight"] = value; }
        }
        
        [ConfigurationProperty("favoritesImageWidth", DefaultValue = 21)]
        public int FavoritesImageWidth
        {
            get { return (int) this["favoritesImageWidth"]; }
            set { this["favoritesImageWidth"] = value; }
        }
        #endregion

        #region Vnc section

        [ConfigurationProperty("vncAutoScale", IsRequired = false)]
        public bool VncAutoScale
        {
            get { return (bool) this["vncAutoScale"]; }
            set { this["vncAutoScale"] = value; }
        }

        [ConfigurationProperty("vncDisplayNumber", IsRequired = false)]
        public int VncDisplayNumber
        {
            get { return (int) this["vncDisplayNumber"]; }
            set { this["vncDisplayNumber"] = value; }
        }

        [ConfigurationProperty("vncViewOnly", IsRequired = false)]
        public bool VncViewOnly
        {
            get { return (bool) this["vncViewOnly"]; }
            set { this["vncViewOnly"] = value; }
        }

        #endregion

        #region Mainform controls section

        [ConfigurationProperty("favoritePanelWidth", DefaultValue = 300)]
        public int FavoritePanelWidth
        {
            get
            {
                if (this["favoritePanelWidth"] != null)
                    return (int) this["favoritePanelWidth"];
                else
                    return 300;
            }
            set { this["favoritePanelWidth"] = value; }
        }

        [ConfigurationProperty("hideFavoritesFromQuickMenu", DefaultValue = true)]
        public bool HideFavoritesFromQuickMenu
        {
            get
            {
                return (bool)this["hideFavoritesFromQuickMenu"];
            }
            set { this["hideFavoritesFromQuickMenu"] = value; }
        }

        [ConfigurationProperty("showFavoritePanel", DefaultValue = true)]
        public bool ShowFavoritePanel
        {
            get
            {
                if (this["showFavoritePanel"] != null)
                    return (bool) this["showFavoritePanel"];
                else
                    return true;
            }
            set { this["showFavoritePanel"] = value; }
        }

        [ConfigurationProperty("toolbarsLocked", DefaultValue = true)]
        public bool ToolbarsLocked
        {
            get { return (bool) this["toolbarsLocked"]; }
            set { this["toolbarsLocked"] = value; }
        }

        /// <summary>
        ///     Gets or set ordered collection of favorite names to show in GUI as toolstrip buttons
        /// </summary>
        [ConfigurationProperty("favoritesButtonsList")]
        [ConfigurationCollection(typeof (MRUItemConfigurationElementCollection))]
        public MRUItemConfigurationElementCollection FavoritesButtons
        {
            get { return (MRUItemConfigurationElementCollection) this["favoritesButtonsList"]; }
            set { this["favoritesButtonsList"] = value; }
        }

        #endregion

        #region Startup section
        [ConfigurationProperty("imageStyle", IsRequired = false)]
        public int ImageStyle
        {
            get
            {
                return (int)this["imageStyle"];
            }
            set { this["imageStyle"] = value; }
        }

        [ConfigurationProperty("dashBoardBackgroundColor", IsRequired = false)]
        public string DashBoardBackgroundColor
        {
            get
            {
                return (string)this["dashBoardBackgroundColor"];
            }
            set { this["dashBoardBackgroundColor"] = value; }
        }

        [ConfigurationProperty("imagePath", IsRequired = false)]
        public string ImagePath
        {
            get
            {
                return (string)this["imagePath"];
            }
            set { this["imagePath"] = value; }
        }

        [ConfigurationProperty("updateSource", IsRequired = false)]
        public string UpdateSource
        {
            get
            {
                if (this["updateSource"] == null || (this["updateSource"] as string) == "")
                {
                	this["updateSource"] = string.Empty;
                }
                return (string) this["updateSource"];
            }
            set { this["updateSource"] = value; }
        }

        [ConfigurationProperty("showWizard", DefaultValue = true)]
        public bool ShowWizard
        {
            get { return (bool) this["showWizard"]; }
            set { this["showWizard"] = value; }
        }

        [ConfigurationProperty("psexecLocation")]
        public string PsexecLocation
        {
            get { return (string) this["psexecLocation"]; }
            set { this["psexecLocation"] = value; }
        }
        #endregion

        #region Favorites & groups section

        [ConfigurationProperty("expandedFavoriteNodes", IsRequired = false)]
        public string ExpandedFavoriteNodes
        {
            get
            {
                if (this["expandedFavoriteNodes"] == null || (this["expandedFavoriteNodes"] as string) == "")
                {
                    this["expandedFavoriteNodes"] = @"Untagged";
                }
                return (string) this["expandedFavoriteNodes"];
            }
            set { this["expandedFavoriteNodes"] = value; }
        }


        [ConfigurationProperty("expandedHistoryNodes", IsRequired = false)]
        public string ExpandedHistoryNodes
        {
            get
            {
                if (this["expandedHistoryNodes"] == null || (this["expandedHistoryNodes"] as string) == "")
                {
                    this["expandedHistoryNodes"] = @"Today";
                }
                return (string) this["expandedHistoryNodes"];
            }
            set { this["expandedHistoryNodes"] = value; }
        }

        [ConfigurationProperty("favorites")]
        [ConfigurationCollection(typeof (FavoriteConfigurationElementCollection), AddItemName = "add")]
        public FavoriteConfigurationElementCollection Favorites
        {
            get { return (FavoriteConfigurationElementCollection) this["favorites"]; }
            set { this["favorites"] = value; }
        }

        [ConfigurationProperty("defaultFavorite")]
        [ConfigurationCollection(typeof (FavoriteConfigurationElementCollection))]
        public FavoriteConfigurationElementCollection DefaultFavorite
        {
            get { return (FavoriteConfigurationElementCollection) this["defaultFavorite"]; }
            set { this["defaultFavorite"] = value; }
        }

        [ConfigurationProperty("groups")]
        [ConfigurationCollection(typeof (GroupConfigurationElementCollection))]
        public GroupConfigurationElementCollection Groups
        {
            get { return (GroupConfigurationElementCollection) this["groups"]; }
            set { this["groups"] = value; }
        }

        [ConfigurationProperty("tags")]
        [ConfigurationCollection(typeof (MRUItemConfigurationElementCollection))]
        public MRUItemConfigurationElementCollection Tags
        {
            get { return (MRUItemConfigurationElementCollection) this["tags"]; }
            set { this["tags"] = value; }
        }
        #endregion

        /// <summary>
        /// Most-recently-used (MRU) lists.
        /// </summary>
        #region MRU section
        [ConfigurationProperty("serversMRUList")]
        [ConfigurationCollection(typeof (MRUItemConfigurationElementCollection))]
        public MRUItemConfigurationElementCollection ServersMRU
        {
            get { return (MRUItemConfigurationElementCollection) this["serversMRUList"]; }
            set { this["serversMRUList"] = value; }
        }

        [ConfigurationProperty("domainsMRUList")]
        [ConfigurationCollection(typeof (MRUItemConfigurationElementCollection))]
        public MRUItemConfigurationElementCollection DomainsMRU
        {
            get { return (MRUItemConfigurationElementCollection) this["domainsMRUList"]; }
            set { this["domainsMRUList"] = value; }
        }

        [ConfigurationProperty("usersMRUList")]
        [ConfigurationCollection(typeof (MRUItemConfigurationElementCollection))]
        public MRUItemConfigurationElementCollection UsersMRU
        {
            get { return (MRUItemConfigurationElementCollection) this["usersMRUList"]; }
            set { this["usersMRUList"] = value; }
        }
        #endregion

        #region ToolStrip section

        [ConfigurationProperty("toolStripSettings")]
        [ConfigurationCollection(typeof (ToolStripSettingElementCollection))]
        public ToolStripSettingElementCollection ToolStripSettings
        {
            get { return (ToolStripSettingElementCollection) this["toolStripSettings"]; }
            set { this["toolStripSettings"] = value; }
        }

        #endregion
    }
}