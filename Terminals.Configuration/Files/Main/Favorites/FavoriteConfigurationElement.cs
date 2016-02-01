namespace Terminals.Configuration.Files.Main.Favorites
{
    // .NET namespaces
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Drawing;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;

    // Framework and Terminals namespaces
    using Credentials;
    using Kohl.Framework.Logging;
    using Security;

    [Serializable]
    public class FavoriteConfigurationElement : ConfigurationElement, ICloneable
    {
        #region Fields, Constants (3)
        #region Common (2)
        private ConnectionImage connectionImage = null;
        #region Tab Color Preferences (1)
        public const string DefaultColor = "FFFFFFFF (White)";
        #endregion
        #endregion

        #region HTTPConnection, HTTPSConnection (1)
        // Cache the form fields in memory and prevent periodic reloading
        private HtmlFormField[] fields;
        #endregion
        #endregion      

        #region Public Non-Configuration Properties (7)
        #region Common (3)
        #region Credentials (2)
        /// <summary>
        ///     Gets or sets the password String in decrypted form
        /// </summary>
        public String Password
        {
            get
            {
                CredentialSet cred = StoredCredentials.GetByName(this.XmlCredentialSetName);
                if (cred != null)
                    return cred.SecretKey;

                return PasswordFunctions.DecryptPassword(this.EncryptedPassword, this.Name);
            }
            set
            {
                if (value == string.Empty)
                {
                    this.EncryptedPassword = string.Empty;
                    return;
                }

                CredentialSet cred = StoredCredentials.GetByName(this.XmlCredentialSetName);

                if (cred == null)
                {
                    this.EncryptedPassword = PasswordFunctions.EncryptPassword(value);
                }
                else
                {
                    // reset the password to string.empty if we use the credential manager
                    this.EncryptedPassword = string.Empty;
                }
            }
        }

        /// <summary>
        /// Returns user name, password and domain for the current favorite in the form of an object.
        /// </summary>
        /// <remarks>If neither a credential has been specified in the connection manual or
        /// no credential set from the XML file (credentials.xml) has been choosen, the defaults
        /// specified in the settings will be returned i.e. the default password, user name and domain.
        /// If the user has choosen not to use any of the options:
        ///    * credential set
        ///    * manual credential entry per favorite
        ///    * default credentials in settings
        /// <c>NULL</c> values will be returned for each property in the credential object.
        /// </remarks>
        public Credential Credential
        {
            get { return Credential.GetCredentials(this); }
        }
        #endregion
        
        /// <summary>
        /// Returns a custom image that can be used instead of the default connection type image.
        /// </summary>
        public ConnectionImage CustomImage
        {
            get
            {
                if (connectionImage != null)
                    return connectionImage;

                if (string.IsNullOrEmpty(this.ToolBarIcon) || !System.IO.File.Exists(this.ToolBarIcon))
                    return null;

                Image image = Image.FromFile(this.ToolBarIcon);

                connectionImage = new ConnectionImage((Image)image.Clone()) { Name = Guid.NewGuid().ToString() };

                if (image != null)
                {
                    image.Dispose();
                    image = null;
                }

                return connectionImage;
            }
        }

        /// <summary>
        /// Gets not null collection of tags obtained from "Tags" property.
        /// </summary>
        public List<String> TagList
        {
            get
            {
                List<String> tagList = new List<String>();
                String[] splittedTags = this.Tags.Split(',');
                if (!((splittedTags.Length == 1) && (String.IsNullOrEmpty(splittedTags[0]))))
                {
                    foreach (String tag in splittedTags)
                    {
                        tagList.Add(tag);
                    }
                }

                return tagList;
            }
        }
        #endregion

        #region HTTPConnection, HTTPSConnection (1)
        public HtmlFormField[] HtmlFormFields
        {
            get { return this.GetHtmlFormFields(); }
            set { this.SetHtmlFormFields(value); }
        }
        #endregion

        #region RDPConnection (3)
        public List<String> RedirectedDrives
        {
            get
            {
                List<String> outputList = new List<String>();
                if (!String.IsNullOrEmpty(this.redirectedDrives))
                {
                    String[] driveArray = this.redirectedDrives.Split(";".ToCharArray());
                    foreach (String drive in driveArray)
                    {
                        outputList.Add(drive);
                    }
                }

                return outputList;
            }
            set
            {
                String drives = String.Empty;
                for (Int32 i = 0; i < value.Count; i++)
                {
                    drives += value[i];
                    if (i < value.Count - 1)
                        drives += ";";
                }

                this.redirectedDrives = drives;
            }
        }

        public String TsgwPassword
        {
            get { return PasswordFunctions.DecryptPassword(this.TsgwEncryptedPassword, this.Name); }
            set { this.TsgwEncryptedPassword = PasswordFunctions.EncryptPassword(value); }
        }

        public Int32 PerformanceFlags
        {
            get
            {
                Int32 result = 0;

                if (this.DisableCursorShadow) result += (Int32)PerfomanceOptions.TsPerfDisableCursorShadow;
                if (this.DisableCursorBlinking) result += (Int32)PerfomanceOptions.TsPerfDisableCursorsettings;
                if (this.DisableFullWindowDrag) result += (Int32)PerfomanceOptions.TsPerfDisableFullwindowdrag;
                if (this.DisableMenuAnimations) result += (Int32)PerfomanceOptions.TsPerfDisableMenuanimations;
                if (this.DisableTheming) result += (Int32)PerfomanceOptions.TsPerfDisableTheming;
                if (this.DisableWallPaper) result += (Int32)PerfomanceOptions.TsPerfDisableWallpaper;
                if (this.EnableDesktopComposition)
                    result += (Int32)PerfomanceOptions.TsPerfEnableDesktopComposition;
                if (this.EnableFontSmoothing) result += (Int32)PerfomanceOptions.TsPerfEnableFontSmoothing;

                return result;
            }
        }
        #endregion
        #endregion

        #region Exposed Configuration Properties (123)
        #region Common (22)
        #region Toolbar (1)
        [ConfigurationProperty("toolBarIcon", IsRequired = false, DefaultValue = "")]
        public String ToolBarIcon
        {
            get { return (String)this["toolBarIcon"]; }
            set { this["toolBarIcon"] = value; }
        }
        #endregion

        [ConfigurationProperty("isDatabaseFavorite", IsRequired = false, DefaultValue = false)]
        public Boolean IsDatabaseFavorite
        {
            get { return (Boolean)this["isDatabaseFavorite"]; }
            set { this["isDatabaseFavorite"] = value; }
        }

        #region Credentials (4)
        [ConfigurationProperty("tsgwcredential", IsRequired = false, DefaultValue = "")]
        public String TsgwXmlCredentialSetName
        {
            get { return (String)this["tsgwcredential"]; }
            set { this["tsgwcredential"] = value; }
        }
        
        [ConfigurationProperty("credential", IsRequired = false, DefaultValue = "")]
        public String XmlCredentialSetName
        {
            get { return (String)this["credential"]; }
            set { this["credential"] = value; }
        }

        /// <summary>
        /// Used to set the domain name.
        /// </summary>
        /// <remarks>
        /// For being able to retrieve the domain name use <see cref="Credential.DomainName"/>
        /// </remarks>
        [ConfigurationProperty("domainName", IsRequired = false)]
        public String DomainName
        {
            internal get
            {
                CredentialSet cred = StoredCredentials.GetByName(this.XmlCredentialSetName);
                if (cred != null)
                    return cred.Domain;

                return (String)this["domainName"];
            }
            set
            {
                if (value == string.Empty)
                {
                    this["domainName"] = string.Empty;
                    return;
                }

                CredentialSet cred = StoredCredentials.GetByName(this.XmlCredentialSetName);

                if (cred == null)
                {
                    this["domainName"] = value;
                }
                else
                {
                    // reset the domain name to string.empty if we use the credential manager
                    this["domainName"] = string.Empty;
                }
            }
        }

        /// <summary>
        /// Used to set the user name.
        /// </summary>
        /// <remarks>
        /// For being able to retrieve the user name use <see cref="Credential.UserName"/>
        /// </remarks>
        [ConfigurationProperty("userName", IsRequired = false)]
        public String UserName
        {
            internal get
            {
                if (!string.IsNullOrEmpty(this.XmlCredentialSetName))
                {
                    CredentialSet cred = StoredCredentials.GetByName(this.XmlCredentialSetName);

                    if (cred != null)
                        return cred.Username;

                    return (String)this["userName"];
                }

                return (String)this["userName"];
            }
            set
            {
                if (value == string.Empty)
                {
                    this["userName"] = string.Empty;
                    return;
                }

                CredentialSet cred = StoredCredentials.GetByName(this.XmlCredentialSetName);

                if (cred == null)
                {
                    this["userName"] = value;
                }
                else
                {
                    // reset the user name to string.empty if we use the credential manager
                    this["userName"] = string.Empty;
                }
            }
        }

        /// <summary>
        /// Returns the non-human-readable password.
        /// </summary>
        /// <remarks>
        /// Only internal setting - not exposed to the user.
        /// </remarks>
        [ConfigurationProperty("encryptedPassword", IsRequired = false)]
        internal String EncryptedPassword
        {
            get { return (String)this["encryptedPassword"]; }
            set { this["encryptedPassword"] = value; }
        }
        #endregion

        #region Tab Color Preferences (1)
        [ConfigurationProperty("tabColor", IsRequired = false, DefaultValue = DefaultColor)]
        public String TabColor
        {
            get { return (String)this["tabColor"]; }
            set { this["tabColor"] = value; }
        }
        #endregion

        [ConfigurationProperty("tags")]
        public String Tags
        {
            get { return (String)this["tags"]; }
            set
            {
                if (Settings.Settings.AutoCaseTags)
                {
                    this["tags"] = Settings.Settings.ToTitleCase(value);
                }
                else
                {
                    this["tags"] = value;
                }
            }
        }

        [ConfigurationProperty("protocol", IsRequired = true, DefaultValue = "RDP")]
        public String Protocol
        {
            get { return (String)this["protocol"]; }
            set { this["protocol"] = value; }
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public String Name
        {
            get { return (String)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("serverName", IsRequired = true)]
        public String ServerName
        {
            get { return (String)this["serverName"]; }
            set { this["serverName"] = value; }
        }

        [ConfigurationProperty("newWindow")]
        public Boolean NewWindow
        {
            get { return (Boolean)this["newWindow"]; }
            set { this["newWindow"] = value; }
        }

        [ConfigurationProperty("notes")]
        public String Notes
        {
            get { return DecodeFrom64((String)this["notes"]); }
            set { this["notes"] = EncodeTo64(value); }
        }

        [ConfigurationProperty("port", DefaultValue = 3389)]
        public Int32 Port
        {
            get { return (Int32)this["port"]; }
            set { this["port"] = value; }
        }

        [ConfigurationProperty("desktopSizeHeight", IsRequired = false, DefaultValue = 480)]
        public Int32 DesktopSizeHeight
        {
            get { return (Int32)this["desktopSizeHeight"]; }
            set { this["desktopSizeHeight"] = value; }
        }

        [ConfigurationProperty("desktopSizeWidth", IsRequired = false, DefaultValue = 640)]
        public Int32 DesktopSizeWidth
        {
            get { return (Int32)this["desktopSizeWidth"]; }
            set { this["desktopSizeWidth"] = value; }
        }

        [ConfigurationProperty("desktopSize", IsRequired = false, DefaultValue = DesktopSize.AutoScale)]
        public DesktopSize DesktopSize
        {
            get { return (DesktopSize)this["desktopSize"]; }
            set { this["desktopSize"] = value; }
        }

        [ConfigurationProperty("executeBeforeConnect")]
        public Boolean ExecuteBeforeConnect
        {
            get { return (Boolean)this["executeBeforeConnect"]; }
            set { this["executeBeforeConnect"] = value; }
        }

        [ConfigurationProperty("executeBeforeConnectCommand")]
        public String ExecuteBeforeConnectCommand
        {
            get { return (String)this["executeBeforeConnectCommand"]; }
            set { this["executeBeforeConnectCommand"] = value; }
        }

        [ConfigurationProperty("executeBeforeConnectArgs")]
        public String ExecuteBeforeConnectArgs
        {
            get { return (String)this["executeBeforeConnectArgs"]; }
            set { this["executeBeforeConnectArgs"] = value; }
        }

        [ConfigurationProperty("executeBeforeConnectInitialDirectory")]
        public String ExecuteBeforeConnectInitialDirectory
        {
            get { return (String)this["executeBeforeConnectInitialDirectory"]; }
            set { this["executeBeforeConnectInitialDirectory"] = value; }
        }

        [ConfigurationProperty("executeBeforeConnectWaitForExit")]
        public Boolean ExecuteBeforeConnectWaitForExit
        {
            get { return (Boolean)this["executeBeforeConnectWaitForExit"]; }
            set { this["executeBeforeConnectWaitForExit"] = value; }
        }
        #endregion

        #region Plugin (1)
        [ConfigurationCollection(typeof(PluginConfigurationElementCollection))]
        [ConfigurationPropertyAttribute("plugins", IsRequired = false, IsKey = false, IsDefaultCollection = false)]
        public PluginConfigurationElementCollection PluginConfigurations
        {
            get { return (PluginConfigurationElementCollection)this["plugins"]; }
            set { this["plugins"] = value; }
        }
        #endregion

        #region ExplorerConnection (5)
        [ConfigurationProperty("explorerStyle", IsRequired = false, DefaultValue = null)]
        public string ExplorerStyle
        {
            get
            {
                try
                {
                    return (string)this["explorerStyle"];
                }
                catch
                {
                    Log.Warn("Terminals was unable to parse your ExplorerStyle in your '" + this.Name +
                             "' connection. Your ExplorerStyle has been changed to the corresponding integer value '0'. Please correct it.");
                    this["explorerStyle"] = string.Empty;
                    return string.Empty;
                }
            }
            set { this["explorerStyle"] = value; }
        }

        [ConfigurationProperty("explorerDirectory", IsRequired = false, DefaultValue = "")]
        public string ExplorerDirectory
        {
            get { return this["explorerDirectory"].ToString(); }
            set { this["explorerDirectory"] = value; }
        }

        [ConfigurationProperty("explorerDirectoryDual", IsRequired = false, DefaultValue = "")]
        public string ExplorerDirectoryDual
        {
            get { return this["explorerDirectoryDual"].ToString(); }
            set { this["explorerDirectoryDual"] = value; }
        }

        [ConfigurationProperty("explorerDirectoryTripple", IsRequired = false, DefaultValue = "")]
        public string ExplorerDirectoryTripple
        {
            get { return this["explorerDirectoryTripple"].ToString(); }
            set { this["explorerDirectoryTripple"] = value; }
        }

        [ConfigurationProperty("explorerDirectoryQuad", IsRequired = false, DefaultValue = "")]
        public string ExplorerDirectoryQuad
        {
            get { return this["explorerDirectoryQuad"].ToString(); }
            set { this["explorerDirectoryQuad"] = value; }
        }
        #endregion

        #region HTTPConnection, HTTPSConnection (4)
        [ConfigurationProperty("htmlFormFields", IsRequired = false, DefaultValue = null)]
        internal string HtmlFormFieldsString
        {
            get
            {
                return (this["htmlFormFields"] == null) ? string.Empty : this["htmlFormFields"].ToString();
            }
            set
            {
                this["htmlFormFields"] = value;
            }
        }

        [ConfigurationProperty("browserAuthentication", IsRequired = false, DefaultValue = BrowserAuthentication.None)]
        public BrowserAuthentication BrowserAuthentication
        {
            get
            {
                try
                {
                    return (BrowserAuthentication) this["browserAuthentication"];
                }
                catch
                {
                    BrowserAuthentication browserAuthenticationNone = BrowserAuthentication.None;
                    Log.Warn("Terminals was unable to parse your " + browserAuthenticationNone.GetType().Name +
                             " in your '" + this.Name + "' connection. Your " + browserAuthenticationNone.GetType().Name +
                             " has been changed to '" + browserAuthenticationNone.ToString() + "'. Please correct it.");
                    this["browserAuthentication"] = BrowserAuthentication.None;
                    return BrowserAuthentication.None;
                }
            }
            set { this["browserAuthentication"] = value; }
        }

        [ConfigurationProperty("httpBrowser", IsRequired = false, DefaultValue = BrowserType.InternetExplorer)]
        public BrowserType HttpBrowser
        {
            get { return (BrowserType) this["httpBrowser"]; }
            set { this["httpBrowser"] = value; }
        }

        [ConfigurationProperty("url", DefaultValue = "")]
        public String Url
        {
            get { return (String)this["url"]; }
            set { this["url"] = value; }
        }
        #endregion

        #region PuttyConnection (7)
        [ConfigurationProperty("puttyConnectionType", IsRequired = false, DefaultValue = PuttyConnectionType.Ssh)]
        public PuttyConnectionType PuttyConnectionType
        {
            get { return (PuttyConnectionType)this["puttyConnectionType"]; }
            set { this["puttyConnectionType"] = value; }
        }

        [ConfigurationProperty("puttyProxyPort", IsRequired = false, DefaultValue = 80)]
        public int PuttyProxyPort
        {
            get { return (int)this["puttyProxyPort"]; }
            set { this["puttyProxyPort"] = value; }
        }
        
        [ConfigurationProperty("puttyProxyType", IsRequired = false, DefaultValue = PuttyProxyType.None)]
        public PuttyProxyType PuttyProxyType
        {
            get { return (PuttyProxyType)this["puttyProxyType"]; }
            set { this["puttyProxyType"] = value; }
        }
		
        [ConfigurationProperty("puttyEnableX11", IsRequired = false, DefaultValue = true)]
        public bool PuttyEnableX11
        {
            get { return (bool)this["puttyEnableX11"]; }
            set { this["puttyEnableX11"] = value; }
        }
		
        
        [ConfigurationProperty("puttyDontAddDomainToUserName", IsRequired = false, DefaultValue = true)]
        public bool PuttyDontAddDomainToUserName
        {
            get { return (bool)this["puttyDontAddDomainToUserName"]; }
            set { this["puttyDontAddDomainToUserName"] = value; }
        }
        
        [ConfigurationProperty("puttyProxyHost", IsRequired = false, DefaultValue = "")]
        public string PuttyProxyHost
        {
            get { return (string)this["puttyProxyHost"]; }
            set { this["puttyProxyHost"] = value; }
        }
		
        [ConfigurationProperty("puttySession", IsRequired = false, DefaultValue = "")]
        public string PuttySession
        {
            get { return (string)this["puttySession"]; }
            set { this["puttySession"] = value; }
        }

        [ConfigurationProperty("puttyCompression", IsRequired = false, DefaultValue = false)]
        public bool PuttyCompression
        {
            get { return (bool)this["puttyCompression"]; }
            set { this["puttyCompression"] = value; }
        }

        [ConfigurationProperty("puttyPasswordTimeout", IsRequired = false, DefaultValue = 7000)]
        public int PuttyPasswordTimeout
        {
            get { return (int)this["puttyPasswordTimeout"]; }
            set { this["puttyPasswordTimeout"] = value; }
        }

        [ConfigurationProperty("puttyVerbose", IsRequired = false, DefaultValue = false)]
        public bool PuttyVerbose
        {
            get { return (bool)this["puttyVerbose"]; }
            set { this["puttyVerbose"] = value; }
        }

        [ConfigurationProperty("puttyShowOptions", IsRequired = false, DefaultValue = false)]
        public bool PuttyShowOptions
        {
            get { return (bool)this["puttyShowOptions"]; }
            set { this["puttyShowOptions"] = value; }
        }

        [ConfigurationProperty("puttyCloseWindowOnExit", IsRequired = false, DefaultValue = PuttyCloseWindowOnExit.OnlyOnCleanExit)]
        public PuttyCloseWindowOnExit PuttyCloseWindowOnExit
        {
            get { return (PuttyCloseWindowOnExit)this["puttyCloseWindowOnExit"]; }
            set { this["puttyCloseWindowOnExit"] = value; }
        }
        #endregion

        #region GenericConnection (3)
        [ConfigurationProperty("genericWorkingDirectory", IsRequired = false, DefaultValue = "")]
        public String GenericWorkingDirectory
        {
            get { return (String)this["genericWorkingDirectory"]; }
            set { this["genericWorkingDirectory"] = value; }
        }

        [ConfigurationProperty("genericProgramPath", IsRequired = false, DefaultValue = "")]
        public String GenericProgramPath
        {
            get { return (String)this["genericProgramPath"]; }
            set { this["genericProgramPath"] = value; }
        }

        [ConfigurationProperty("genericArguments", IsRequired = false, DefaultValue = "")]
        public String GenericArguments
        {
            get { return (String)this["genericArguments"]; }
            set { this["genericArguments"] = value; }
        }
        #endregion

        #region RAdminConnection (15)
        [ConfigurationProperty("radminPhonebookPath", IsRequired = false)]
        public string RAdminPhonebookPath
        {
            get { return (string)this["radminPhonebookPath"]; }
            set { this["radminPhonebookPath"] = value; }
        }

        [ConfigurationProperty("radminThrough", IsRequired = false, DefaultValue = false)]
        public bool RAdminThrough
        {
            get { return (bool)this["radminThrough"]; }
            set { this["radminThrough"] = value; }
        }

        [ConfigurationProperty("radminThroughServerName", IsRequired = false, DefaultValue = "")]
        public string RAdminThroughServerName
        {
            get { return (string)this["radminThroughServerName"]; }
            set { this["radminThroughServerName"] = value; }
        }

        [ConfigurationProperty("radminThroughPort", IsRequired = false, DefaultValue = "")]
        public string RAdminThroughPort
        {
            get { return (string)this["radminThroughPort"]; }
            set { this["radminThroughPort"] = value; }
        }

        [ConfigurationProperty("radminStandardConnectionMode", IsRequired = false, DefaultValue = true)]
        public bool RAdminStandardConnectionMode
        {
            get { return (bool)this["radminStandardConnectionMode"]; }
            set { this["radminStandardConnectionMode"] = value; }
        }

        [ConfigurationProperty("radminTelnetMode", IsRequired = false, DefaultValue = false)]
        public bool RAdminTelnetMode
        {
            get { return (bool)this["radminTelnetMode"]; }
            set { this["radminTelnetMode"] = value; }
        }

        [ConfigurationProperty("radminViewOnlyMode", IsRequired = false, DefaultValue = false)]
        public bool RAdminViewOnlyMode
        {
            get { return (bool)this["radminViewOnlyMode"]; }
            set { this["radminViewOnlyMode"] = value; }
        }

        [ConfigurationProperty("radminFileTransferMode", IsRequired = false, DefaultValue = false)]
        public bool RAdminFileTransferMode
        {
            get { return (bool)this["radminFileTransferMode"]; }
            set { this["radminFileTransferMode"] = value; }
        }

        [ConfigurationProperty("radminShutdown", IsRequired = false, DefaultValue = false)]
        public bool RAdminShutdown
        {
            get { return (bool)this["radminShutdown"]; }
            set { this["radminShutdown"] = value; }
        }

        [ConfigurationProperty("radminChatMode", IsRequired = false, DefaultValue = false)]
        public bool RAdminChatMode
        {
            get { return (bool)this["radminChatMode"]; }
            set { this["radminChatMode"] = value; }
        }

        [ConfigurationProperty("radminVoiceChatMode", IsRequired = false, DefaultValue = false)]
        public bool RAdminVoiceChatMode
        {
            get { return (bool)this["radminVoiceChatMode"]; }
            set { this["radminVoiceChatMode"] = value; }
        }

        [ConfigurationProperty("radminSendTextMessageMode", IsRequired = false, DefaultValue = false)]
        public bool RAdminSendTextMessageMode
        {
            get { return (bool)this["radminSendTextMessageMode"]; }
            set { this["radminSendTextMessageMode"] = value; }
        }

        [ConfigurationProperty("radminUseFullScreen", IsRequired = false, DefaultValue = false)]
        public bool RAdminUseFullScreen
        {
            get { return (bool)this["radminUseFullScreen"]; }
            set { this["radminUseFullScreen"] = value; }
        }

        [ConfigurationProperty("radminUpdates", IsRequired = false, DefaultValue = 99)]
        public int RAdminUpdates
        {
            get { return (int)this["radminUpdates"]; }
            set { this["radminUpdates"] = value; }
        }

        [ConfigurationProperty("radminColorMode", IsRequired = false, DefaultValue = "Bits24")]
        public string RAdminColorMode
        {
            get { return (string)this["radminColorMode"]; }
            set { this["radminColorMode"] = value; }
        }
        #endregion

        #region TerminalConnection (and Terminals.Forms.Controls.ConsolePreferences) (9)
        [ConfigurationProperty("consolerows", IsRequired = false, DefaultValue = 38)]
        public Int32 ConsoleRows
        {
            get { return (Int32)this["consolerows"]; }
            set { this["consolerows"] = value; }
        }

        [ConfigurationProperty("consolecols", IsRequired = false, DefaultValue = 110)]
        public Int32 ConsoleCols
        {
            get { return (Int32)this["consolecols"]; }
            set { this["consolecols"] = value; }
        }

        [ConfigurationProperty("consolefont", IsRequired = false, DefaultValue = "[Font: Name=Courier New, Size=10, Units=3, GdiCharSet=0, GdiVerticalFont=False, Style=0]")]
        public String ConsoleFont
        {
            get
            {
                String font = (String)this["consolefont"];
                if (String.IsNullOrEmpty(font))
                    font = "[Font: Name=Courier New, Size=10, Units=3, GdiCharSet=0, GdiVerticalFont=False, Style=0]";

                return font;
            }
            set { this["consolefont"] = value; }
        }

        [ConfigurationProperty("consolebackcolor", IsRequired = false, DefaultValue = "FF000000 (Black)")]
        public String ConsoleBackColor
        {
            get { return (String)this["consolebackcolor"]; }
            set { this["consolebackcolor"] = value; }
        }

        [ConfigurationProperty("consoletextcolor", IsRequired = false, DefaultValue = "FFFFFFFF (White)")]
        public String ConsoleTextColor
        {
            get { return (String)this["consoletextcolor"]; }
            set { this["consoletextcolor"] = value; }
        }

        [ConfigurationProperty("consolecursorcolor", IsRequired = false, DefaultValue = "FFFF0000 (Red)")]
        public String ConsoleCursorColor
        {
            get { return (String)this["consolecursorcolor"]; }
            set { this["consolecursorcolor"] = value; }
        }

        [ConfigurationProperty("ssh1", IsRequired = false, DefaultValue = false)]
        public Boolean Ssh1
        {
            get { return (Boolean)this["ssh1"]; }
            set { this["ssh1"] = value; }
        }

        [ConfigurationProperty("authMethod", DefaultValue = AuthMethod.PublicKey)]
        public AuthMethod AuthMethod
        {
            get { return (AuthMethod)this["authMethod"]; }
            set { this["authMethod"] = value; }
        }

        [ConfigurationProperty("keyTag", DefaultValue = "")]
        public String KeyTag
        {
            get { return (String)this["keyTag"]; }
            set { this["keyTag"] = value; }
        }
        #endregion

        #region VNCConnection (3)
        [ConfigurationProperty("vncAutoScale", IsRequired = false, DefaultValue = false)]
        public Boolean VncAutoScale
        {
            get { return (Boolean)this["vncAutoScale"]; }
            set { this["vncAutoScale"] = value; }
        }

        [ConfigurationProperty("vncViewOnly", IsRequired = false, DefaultValue = false)]
        public Boolean VncViewOnly
        {
            get { return (Boolean)this["vncViewOnly"]; }
            set { this["vncViewOnly"] = value; }
        }

        [ConfigurationProperty("vncDisplayNumber", IsRequired = false, DefaultValue = 0)]
        public Int32 VncDisplayNumber
        {
            get { return (Int32)this["vncDisplayNumber"]; }
            set { this["vncDisplayNumber"] = value; }
        }
        #endregion

        #region VMRCConnection (2)
        [ConfigurationProperty("vmrcreducedcolorsmode", IsRequired = false, DefaultValue = false)]
        public Boolean VmrcReducedColorsMode
        {
            get { return (Boolean)this["vmrcreducedcolorsmode"]; }
            set { this["vmrcreducedcolorsmode"] = value; }
        }

        [ConfigurationProperty("vmrcadministratormode", IsRequired = false, DefaultValue = false)]
        public Boolean VmrcAdministratorMode
        {
            get { return (Boolean)this["vmrcadministratormode"]; }
            set { this["vmrcadministratormode"] = value; }
        }
        #endregion

        #region ICAConnection (5)
        [ConfigurationProperty("ICAApplicationName", IsRequired = false, DefaultValue = "")]
        public String IcaApplicationName
        {
            get { return (String)this["ICAApplicationName"]; }
            set { this["ICAApplicationName"] = value; }
        }

        [ConfigurationProperty("icaServerINI")]
        public String IcaServerIni
        {
            get { return (String)this["icaServerINI"]; }
            set { this["icaServerINI"] = value; }
        }

        [ConfigurationProperty("icaClientINI")]
        public String IcaClientIni
        {
            get { return (String)this["icaClientINI"]; }
            set { this["icaClientINI"] = value; }
        }

        [ConfigurationProperty("icaEnableEncryption")]
        public Boolean IcaEnableEncryption
        {
            get { return (Boolean)this["icaEnableEncryption"]; }
            set { this["icaEnableEncryption"] = value; }
        }

        [ConfigurationProperty("icaEncryptionLevel")]
        public String IcaEncryptionLevel
        {
            get { return (String)this["icaEncryptionLevel"]; }
            set { this["icaEncryptionLevel"] = value; }
        }
        #endregion

        #region RDPConnection (44)
        [ConfigurationProperty("loadBalanceInfo", DefaultValue = "")]
        public String LoadBalanceInfo
        {
            get { return (String)this["loadBalanceInfo"]; }
            set { this["loadBalanceInfo"] = value; }
        }

        [ConfigurationProperty("connectToConsole", IsRequired = false)]
        public Boolean ConnectToConsole
        {
            get { return (Boolean)this["connectToConsole"]; }
            set { this["connectToConsole"] = value; }
        }

        [ConfigurationProperty("redirectPrinters")]
        public Boolean RedirectPrinters
        {
            get { return (Boolean)this["redirectPrinters"]; }
            set { this["redirectPrinters"] = value; }
        }

        [ConfigurationProperty("redirectSmartCards")]
        public Boolean RedirectSmartCards
        {
            get { return (Boolean)this["redirectSmartCards"]; }
            set { this["redirectSmartCards"] = value; }
        }

        [ConfigurationProperty("redirectClipboard", DefaultValue = true)]
        public Boolean RedirectClipboard
        {
            get { return (Boolean)this["redirectClipboard"]; }
            set { this["redirectClipboard"] = value; }
        }

        [ConfigurationProperty("redirectDevices")]
        public Boolean RedirectDevices
        {
            get { return (Boolean)this["redirectDevices"]; }
            set { this["redirectDevices"] = value; }
        }

        /// <summary>
        ///     TSC_PROXY_MODE_NONE_DIRECT 0 (0x0)
        ///     Do not use an RD Gateway server. In the Remote Desktop Connection (RDC) client UI, the Bypass RD Gateway server for local addresses check box is cleared.
        /// 
        ///     TSC_PROXY_MODE_DIRECT 1 (0x1)
        ///     Always use an RD Gateway server. In the RDC client UI, the Bypass RD Gateway server for local addresses check box is cleared.
        /// 
        ///     TSC_PROXY_MODE_DETECT 2 (0x2)
        ///     Use an RD Gateway server if a direct connection cannot be made to the RD Session Host server. In the RDC client UI, the Bypass RD Gateway server for local addresses check box is selected.
        /// 
        ///     TSC_PROXY_MODE_DEFAULT 3 (0x3)
        ///     Use the default RD Gateway server settings.
        /// 
        ///     TSC_PROXY_MODE_NONE_DETECT 4 (0x4)
        ///     Do not use an RD Gateway server. In the RDC client UI, the Bypass RD Gateway server for local addresses check box is selected.
        /// </summary>
        [ConfigurationProperty("tsgwUsageMethod", IsRequired = false, DefaultValue = 0)]
        public Int32 TsgwUsageMethod
        {
            get { return (Int32)this["tsgwUsageMethod"]; }
            set { this["tsgwUsageMethod"] = value; }
        }

        [ConfigurationProperty("tsgwHostname", IsRequired = false, DefaultValue = "")]
        public String TsgwHostname
        {
            get { return (String)this["tsgwHostname"]; }
            set { this["tsgwHostname"] = value; }
        }

        [ConfigurationProperty("tsgwCredsSource", IsRequired = false, DefaultValue = 0)]
        public Int32 TsgwCredsSource
        {
            get { return (Int32)this["tsgwCredsSource"]; }
            set { this["tsgwCredsSource"] = value; }
        }

        [ConfigurationProperty("tsgwSeparateLogin", IsRequired = false, DefaultValue = false)]
        public Boolean TsgwSeparateLogin
        {
            get { return (Boolean)this["tsgwSeparateLogin"]; }
            set { this["tsgwSeparateLogin"] = value; }
        }

        [ConfigurationProperty("tsgwUsername", IsRequired = false, DefaultValue = "")]
        public String TsgwUsername
        {
            get { return (String)this["tsgwUsername"]; }
            set { this["tsgwUsername"] = value; }
        }

        [ConfigurationProperty("tsgwDomain", IsRequired = false, DefaultValue = "")]
        public String TsgwDomain
        {
            get { return (String)this["tsgwDomain"]; }
            set { this["tsgwDomain"] = value; }
        }

        /// <remarks>
        /// Needs to be public for reflecation in
        /// <see cref="Terminals.ExportImport.Export.ExportTerminals"/>
        /// and
        /// <see cref="Terminals.ExportImport.Import.ImportTerminals"/>.
        /// </remarks>
        [ConfigurationProperty("tsgwPassword", DefaultValue = "")]
        public String TsgwEncryptedPassword
        {
            get { return (String)this["tsgwPassword"]; }
            set { this["tsgwPassword"] = value; }
        }

        [ConfigurationProperty("sounds", DefaultValue = RemoteSounds.DontPlay)]
        public RemoteSounds Sounds
        {
            get { return (RemoteSounds)this["sounds"]; }
            set { this["sounds"] = value; }
        }

        /// <remarks>
        /// Needs to be public for reflecation in
        /// <see cref="Terminals.ExportImport.Export.ExportTerminals"/>
        /// and
        /// <see cref="Terminals.ExportImport.Import.ImportTerminals"/>.
        /// </remarks>
        [ConfigurationProperty("redirectDrives")]
        public String redirectedDrives
        {
            get { return (String)this["redirectDrives"]; }
            set { this["redirectDrives"] = value; }
        }

        [ConfigurationProperty("redirectPorts")]
        public Boolean RedirectPorts
        {
            get { return (Boolean)this["redirectPorts"]; }
            set { this["redirectPorts"] = value; }
        }

        [ConfigurationProperty("shutdownTimeout", IsRequired = false, DefaultValue = 10)]
        public Int32 ShutdownTimeout
        {
            get
            {
                Int32 val = (Int32)this["shutdownTimeout"];
                if (val > 600)
                    val = 600;

                if (val < 10)
                    val = 10;

                return val;
            }
            set
            {
                if (value > 600)
                    value = 600;

                if (value < 10)
                    value = 10;

                this["shutdownTimeout"] = value;
            }
        }

        [ConfigurationProperty("overallTimeout", IsRequired = false, DefaultValue = 600)]
        public Int32 OverallTimeout
        {
            get
            {
                Int32 val = (Int32)this["overallTimeout"];
                if (val > 600)
                    val = 600;

                if (val < 10)
                    val = 10;

                return val;
            }
            set
            {
                if (value > 600)
                    value = 600;

                if (value < 0)
                    value = 0;

                this["overallTimeout"] = value;
            }
        }

        [ConfigurationProperty("connectionTimeout", IsRequired = false, DefaultValue = 600)]
        public Int32 ConnectionTimeout
        {
            get
            {
                Int32 val = (Int32)this["connectionTimeout"];
                if (val > 600)
                    val = 600;

                if (val < 10)
                    val = 10;

                return val;
            }
            set
            {
                if (value > 600)
                    value = 600;

                if (value < 0)
                    value = 0;

                this["connectionTimeout"] = value;
            }
        }

        [ConfigurationProperty("idleTimeout", IsRequired = false, DefaultValue = 240)]
        public Int32 IdleTimeout
        {
            get
            {
                Int32 val = (Int32)this["idleTimeout"];
                if (val > 600)
                    val = 600;

                if (val < 10)
                    val = 10;

                return val;
            }
            set
            {
                if (value > 240)
                    value = 240;

                if (value < 0)
                    value = 0;

                this["idleTimeout"] = value;
            }
        }

        [ConfigurationProperty("securityWorkingFolder", IsRequired = false, DefaultValue = "")]
        public String SecurityWorkingFolder
        {
            get { return (String)this["securityWorkingFolder"]; }
            set { this["securityWorkingFolder"] = value; }
        }

        [ConfigurationProperty("securityStartProgram", IsRequired = false, DefaultValue = "")]
        public String SecurityStartProgram
        {
            get { return (String)this["securityStartProgram"]; }
            set { this["securityStartProgram"] = value; }
        }

        [ConfigurationProperty("securityFullScreen", IsRequired = false, DefaultValue = false)]
        public Boolean SecurityFullScreen
        {
            get { return (Boolean)this["securityFullScreen"]; }
            set { this["securityFullScreen"] = value; }
        }

        [ConfigurationProperty("enableSecuritySettings", IsRequired = false, DefaultValue = false)]
        public Boolean EnableSecuritySettings
        {
            get { return (Boolean)this["enableSecuritySettings"]; }
            set { this["enableSecuritySettings"] = value; }
        }

        [ConfigurationProperty("grabFocusOnConnect", IsRequired = false, DefaultValue = false)]
        public Boolean GrabFocusOnConnect
        {
            get { return (Boolean)this["grabFocusOnConnect"]; }
            set { this["grabFocusOnConnect"] = value; }
        }

        [ConfigurationProperty("enableEncryption", IsRequired = false, DefaultValue = false)]
        public Boolean EnableEncryption
        {
            get { return (Boolean)this["enableEncryption"]; }
            set { this["enableEncryption"] = value; }
        }

        [ConfigurationProperty("disableWindowsKey", IsRequired = false, DefaultValue = false)]
        public Boolean DisableWindowsKey
        {
            get { return (Boolean)this["disableWindowsKey"]; }
            set { this["disableWindowsKey"] = value; }
        }

        [ConfigurationProperty("doubleClickDetect", IsRequired = false, DefaultValue = false)]
        public Boolean DoubleClickDetect
        {
            get { return (Boolean)this["doubleClickDetect"]; }
            set { this["doubleClickDetect"] = value; }
        }

        [ConfigurationProperty("displayConnectionBar", IsRequired = false, DefaultValue = false)]
        public Boolean DisplayConnectionBar
        {
            get { return (Boolean)this["displayConnectionBar"]; }
            set { this["displayConnectionBar"] = value; }
        }

        [ConfigurationProperty("disableControlAltDelete", IsRequired = false, DefaultValue = false)]
        public Boolean DisableControlAltDelete
        {
            get { return (Boolean)this["disableControlAltDelete"]; }
            set { this["disableControlAltDelete"] = value; }
        }

        [ConfigurationProperty("acceleratorPassthrough", IsRequired = false, DefaultValue = false)]
        public Boolean AcceleratorPassthrough
        {
            get { return (Boolean)this["acceleratorPassthrough"]; }
            set { this["acceleratorPassthrough"] = value; }
        }

        [ConfigurationProperty("enableCompression", IsRequired = false, DefaultValue = false)]
        public Boolean EnableCompression
        {
            get { return (Boolean)this["enableCompression"]; }
            set { this["enableCompression"] = value; }
        }

        [ConfigurationProperty("bitmapPeristence", IsRequired = false, DefaultValue = false)]
        public Boolean BitmapPeristence
        {
            get { return (Boolean)this["bitmapPeristence"]; }
            set { this["bitmapPeristence"] = value; }
        }

        [ConfigurationProperty("enableTLSAuthentication", IsRequired = false, DefaultValue = false)]
        public Boolean EnableTlsAuthentication
        {
            get { return (Boolean)this["enableTLSAuthentication"]; }
            set { this["enableTLSAuthentication"] = value; }
        }

        [ConfigurationProperty("enableNLAAuthentication", IsRequired = false, DefaultValue = false)]
        public Boolean EnableNlaAuthentication
        {
            get { return (Boolean)this["enableNLAAuthentication"]; }
            set { this["enableNLAAuthentication"] = value; }
        }

        [ConfigurationProperty("allowBackgroundInput", IsRequired = false, DefaultValue = false)]
        public Boolean AllowBackgroundInput
        {
            get { return (Boolean)this["allowBackgroundInput"]; }
            set { this["allowBackgroundInput"] = value; }
        }

        [ConfigurationProperty("disableTheming")]
        public Boolean DisableTheming
        {
            get { return (Boolean)this["disableTheming"]; }
            set { this["disableTheming"] = value; }
        }

        [ConfigurationProperty("disableMenuAnimations")]
        public Boolean DisableMenuAnimations
        {
            get { return (Boolean)this["disableMenuAnimations"]; }
            set { this["disableMenuAnimations"] = value; }
        }

        [ConfigurationProperty("disableFullWindowDrag")]
        public Boolean DisableFullWindowDrag
        {
            get { return (Boolean)this["disableFullWindowDrag"]; }
            set { this["disableFullWindowDrag"] = value; }
        }

        [ConfigurationProperty("disableCursorBlinking")]
        public Boolean DisableCursorBlinking
        {
            get { return (Boolean)this["disableCursorBlinking"]; }
            set { this["disableCursorBlinking"] = value; }
        }

        [ConfigurationProperty("enableDesktopComposition")]
        public Boolean EnableDesktopComposition
        {
            get { return (Boolean)this["enableDesktopComposition"]; }
            set { this["enableDesktopComposition"] = value; }
        }

        [ConfigurationProperty("enableFontSmoothing")]
        public Boolean EnableFontSmoothing
        {
            get { return (Boolean)this["enableFontSmoothing"]; }
            set { this["enableFontSmoothing"] = value; }
        }

        [ConfigurationProperty("disableCursorShadow")]
        public Boolean DisableCursorShadow
        {
            get { return (Boolean)this["disableCursorShadow"]; }
            set { this["disableCursorShadow"] = value; }
        }

        [ConfigurationProperty("disableWallPaper")]
        public Boolean DisableWallPaper
        {
            get { return (Boolean)this["disableWallPaper"]; }
            set { this["disableWallPaper"] = value; }
        }
        #endregion

        #region RDPConnection & ICAConnection (2)
        [ConfigurationProperty("colors", IsRequired = false, DefaultValue = Colors.Bit16)]
        public Colors Colors
        {
            get { return (Colors)this["colors"]; }
            set { this["colors"] = value; }
        }

        [ConfigurationProperty("desktopShare")]
        public String DesktopShare
        {
            get { return (String)this["desktopShare"]; }
            set { this["desktopShare"] = value; }
        }
        #endregion
        #endregion

        #region Constructors (2)
        public FavoriteConfigurationElement() : this(null)
        {
        }

        public FavoriteConfigurationElement(String name)
        {
            this.Name = name;
        }
        #endregion

        #region Methods (15)
        #region Plugin(2)
        public void SetPluginValue<T>(string name, T value, T defaultValue = default(T))
        {
            PluginConfiguration pluginConfiguration = this.PluginConfigurations[name];
            if (pluginConfiguration == null)
            {
                pluginConfiguration = new PluginConfiguration() { Name = name };
                this.PluginConfigurations.Add(pluginConfiguration);
            }

            pluginConfiguration.SetValue(value, defaultValue);
        }

        public T GetPluginValue<T>(string name)
        {
            PluginConfiguration pluginConfiguration = this.PluginConfigurations[name];

            T returnValue = default(T);

            if (pluginConfiguration != null)
                returnValue = pluginConfiguration.GetValue<T>();

            return returnValue;
        }
        #endregion

        #region Password (1)
        public void UpdatePasswordsByNewKeyMaterial(string newKeyMaterial)
        {
            this.EncryptedPassword = PasswordFunctions.EncryptPassword(this.Password, newKeyMaterial);
            this.TsgwEncryptedPassword = PasswordFunctions.EncryptPassword(this.TsgwPassword, newKeyMaterial);
        }
        #endregion
        
        #region HTTPConnection, HTTPSConnection (2)
        private HtmlFormField[] GetHtmlFormFields()
        {
            if (this.fields == null)
            {
                List<HtmlFormField> htmlFormFields = new List<HtmlFormField>();

                try
                {
                    string temp = this.HtmlFormFieldsString;

                    // return an empty array
                    if (string.IsNullOrEmpty(temp))
                        return htmlFormFields.ToArray();

                    // return an empty array
                    if (string.IsNullOrEmpty(temp))
                    {
                        this.HtmlFormFieldsString = null;
                        return htmlFormFields.ToArray();
                    }

                    string[] fields = temp.Split(new[] { ":öä%&$$.°°°^..:" }, StringSplitOptions.None);

                    foreach (string field in fields)
                    {
                        htmlFormFields.Add(new HtmlFormField
                        {
                            Key =
                                field.Split(new[] { "=öä%&$$.°°°^..=" },
                                            StringSplitOptions.None)[0],
                            Value =
                                field.Split(new[] { "=öä%&$$.°°°^..=" },
                                            StringSplitOptions.None)[1]
                        });
                    }
                }
                catch
                {
                }

                this.fields = htmlFormFields.ToArray();
            }

            if (this.fields == null || this.fields.Length < 1)
                this.HtmlFormFieldsString = null;

            return this.fields;
        }

        private void SetHtmlFormFields(HtmlFormField[] @value, string keyMaterial = null)
        {
            string result = null;

            this.fields = @value;

            if (this.fields == null || fields.Length < 1)
            {
                this.HtmlFormFieldsString = null;
                return;
            }

            int empty = 0;

            foreach (HtmlFormField field in this.fields)
            {
                if (!string.IsNullOrEmpty(field.Key) || !string.IsNullOrEmpty(field.Value))
                    empty++;

                result += field.Key + "=öä%&$$.°°°^..=" + field.Value + ":öä%&$$.°°°^..:";
            }

            // Ensure that we don't acquire unneed space in the 
            // configuration file.
            if (empty == 0)
                result = null;

            if (!string.IsNullOrEmpty(result))
                result = result.Remove(result.Length - ":öä%&$$.°°°^..:".Length, ":öä%&$$.°°°^..:".Length);

            this.HtmlFormFieldsString = result;
        }
        #endregion

        #region Tab Color Preferences (3)
        public static string GetDisplayColor(Color color)
        {
            string colorValue = color.ToArgb().ToString("X");

            if (color.Name.ToUpper() == color.ToArgb().ToString("X"))
            {
                return colorValue;
            }

            return colorValue + " (" + color.Name + ")";
        }

        public static Color TranslateColor(string value)
        {
            string hexValue = string.Empty;

            // we are dealing with a system or well-known color
            if (value.Contains(" (") && value.Contains(")"))
            {
                hexValue = value.Split(new[] { " (" }, StringSplitOptions.None)[0];
            }
            // we've only the ARGB value
            else
            {
                hexValue = value;
            }

            if (!IsHex(hexValue))
            {
                return Color.White;
            }

            return Color.FromArgb(int.Parse(hexValue, NumberStyles.HexNumber));
        }

        private static bool IsHex(string test)
        {
            // For C-style hex notation (0xFF) you can use @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z"
            return Regex.IsMatch(test, @"\A\b[0-9a-fA-F]+\b\Z");
        }
        #endregion
        
        #region Methods to rename Favorites (5)
        public static string CreateNewFavoriteNameIfAlreadyExists(string oldName, bool isDatabaseFavorite)
        {
            return CreateNewFavoriteNameForCopy(oldName, null, null, null, false, isDatabaseFavorite);
        }

        public static string CreateNewFavoriteNameForCopy(string oldName, bool isDatabaseFavorite)
        {
            return CreateNewFavoriteNameForCopy(oldName, null, null, null, true, isDatabaseFavorite);
        }

        public static string CreateNewFavoriteNameForCopy(string oldName, string newName, string prefix, string suffix, bool appendWithCopy, bool isDatabaseFavorite)
        {
            if (appendWithCopy && string.IsNullOrEmpty(prefix) && string.IsNullOrEmpty(suffix) &&
                string.IsNullOrEmpty(newName))
            {
                newName = oldName + "_Copy";
            }

            if (string.IsNullOrEmpty(newName))
                newName = oldName;

            if (!string.IsNullOrEmpty(prefix))
                newName = prefix + newName;

            if (!string.IsNullOrEmpty(suffix))
                newName += suffix;

            foreach (FavoriteConfigurationElement element in Settings.Settings.GetFavorites(isDatabaseFavorite))
            {
                if (element.Name == newName)
                {
                    newName += "_" + Guid.NewGuid().ToString();
                    break;
                }
            }

            return newName;
        }

        public FavoriteConfigurationElement Copy(bool isDatabaseFavorite)
        {
            return this.Copy(null, null, null, isDatabaseFavorite);
        }

        public FavoriteConfigurationElement Copy(string newName, string prefix, string suffix, bool isDatabaseFavorite)
        {
            FavoriteConfigurationElement fav = (FavoriteConfigurationElement) this.Clone();

            fav.IsDatabaseFavorite = isDatabaseFavorite;
            fav.Name = CreateNewFavoriteNameForCopy(fav.Name, newName, prefix, suffix, true, isDatabaseFavorite);

            return fav;
        }
        #endregion

        #region Methods to encode or decode 'Favorite' notes (2)
        public static String EncodeTo64(String toEncode)
        {
            if (string.IsNullOrEmpty(toEncode))
                return null;

            Byte[] toEncodeAsBytes = Encoding.UTF8.GetBytes(toEncode);
            String returnValue = Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        public static String DecodeFrom64(String encodedData)
        {
            if (string.IsNullOrEmpty(encodedData))
                return null;

            Byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);
            String returnValue = Encoding.UTF8.GetString(encodedDataAsBytes);
            return returnValue;
        }
        #endregion
        #endregion

        #region ToString (1)
        public override String ToString()
        {
            string domain = String.Empty;

            if (this.Credential.IsSetDomainName)
                domain = this.Credential.DomainName;

            if (!domain.EndsWith("\\"))
                domain += "\\";

            return String.Format(@"Favorite:{0}({1})={2}{3}:{4}",this.Name, this.Protocol, domain, this.ServerName, this.Port);
        }
        #endregion

        #region ICloneable Members (1)
        public object Clone()
        {
            FavoriteConfigurationElement fav = new FavoriteConfigurationElement
            {
                #region Common (21)
                #region Toolbar (1)
                ToolBarIcon = this.ToolBarIcon,
                #endregion

                #region Credentials (4)
                XmlCredentialSetName = this.XmlCredentialSetName,
                DomainName = this.DomainName,
                UserName = this.UserName,
                EncryptedPassword = this.EncryptedPassword,
                #endregion

                #region Tab Color Preferences (1)
                TabColor = this.TabColor,
                #endregion

                IsDatabaseFavorite = this.IsDatabaseFavorite,

                Tags = this.Tags,

                ServerName = this.ServerName,
                Name = this.Name,
                NewWindow = this.NewWindow,
                Notes = this.Notes,
                Port = this.Port,
                Protocol = this.Protocol,

                DesktopSize = this.DesktopSize,
                DesktopSizeHeight = this.DesktopSizeHeight,
                DesktopSizeWidth = this.DesktopSizeWidth,

                ExecuteBeforeConnect = this.ExecuteBeforeConnect,
                ExecuteBeforeConnectArgs = this.ExecuteBeforeConnectArgs,
                ExecuteBeforeConnectCommand = this.ExecuteBeforeConnectCommand,
                ExecuteBeforeConnectInitialDirectory = this.ExecuteBeforeConnectInitialDirectory,
                ExecuteBeforeConnectWaitForExit = this.ExecuteBeforeConnectWaitForExit,
                #endregion

                #region Plugin (1)
                PluginConfigurations = this.PluginConfigurations,
                #endregion

                #region ExplorerConnection (5)
                ExplorerDirectory = this.ExplorerDirectory,
                ExplorerDirectoryDual = this.ExplorerDirectoryDual,
                ExplorerDirectoryTripple = this.ExplorerDirectoryTripple,
                ExplorerDirectoryQuad = this.ExplorerDirectoryQuad,
                ExplorerStyle = this.ExplorerStyle,
                #endregion

                #region HTTPConnection, HTTPSConnection (4)
                HtmlFormFields = this.HtmlFormFields,
                BrowserAuthentication = this.BrowserAuthentication,
                HttpBrowser = this.HttpBrowser,
                Url = this.Url,
                #endregion

                #region PuttyConnection (7)
                PuttyConnectionType = this.PuttyConnectionType,
                PuttyCloseWindowOnExit = this.PuttyCloseWindowOnExit,
                PuttyCompression = this.PuttyCompression,
                PuttySession = this.PuttySession,
                PuttyShowOptions = this.PuttyShowOptions,
                PuttyVerbose = this.PuttyShowOptions,
                PuttyPasswordTimeout = this.PuttyPasswordTimeout,
				PuttyProxyHost = this.PuttyProxyHost,
				PuttyProxyPort = PuttyProxyPort,
				PuttyProxyType = PuttyProxyType,
                #endregion

                #region GenericConnection (3)
                GenericArguments = this.GenericArguments,
                GenericProgramPath = this.GenericProgramPath,
                GenericWorkingDirectory = this.GenericWorkingDirectory,
                #endregion

                #region RAdminConnection (15)
                RAdminStandardConnectionMode = this.RAdminStandardConnectionMode,
                RAdminChatMode = this.RAdminChatMode,
                RAdminColorMode = this.RAdminColorMode,
                RAdminFileTransferMode = this.RAdminFileTransferMode,
                RAdminPhonebookPath = this.RAdminPhonebookPath,
                RAdminSendTextMessageMode = this.RAdminSendTextMessageMode,
                RAdminShutdown = this.RAdminShutdown,
                RAdminTelnetMode = this.RAdminTelnetMode,
                RAdminThrough = this.RAdminThrough,
                RAdminThroughPort = this.RAdminThroughPort,
                RAdminThroughServerName = this.RAdminThroughServerName,
                RAdminUpdates = this.RAdminUpdates,
                RAdminUseFullScreen = this.RAdminUseFullScreen,
                RAdminViewOnlyMode = this.RAdminViewOnlyMode,
                RAdminVoiceChatMode = this.RAdminVoiceChatMode,
                #endregion

                #region TerminalConnection (and Terminals.Forms.Controls.ConsolePreferences) (9)
                ConsoleBackColor = this.ConsoleBackColor,
                ConsoleCols = this.ConsoleCols,
                ConsoleCursorColor = this.ConsoleCursorColor,
                ConsoleFont = this.ConsoleFont,
                ConsoleRows = this.ConsoleRows,
                ConsoleTextColor = this.ConsoleTextColor,
                Ssh1 = this.Ssh1,
                AuthMethod = this.AuthMethod,
                KeyTag = this.KeyTag,
                #endregion

                #region VNCConnection (3)
                VncAutoScale = this.VncAutoScale,
                VncDisplayNumber = this.VncDisplayNumber,
                VncViewOnly = this.VncViewOnly,
                #endregion

                #region VMRCConnection (2)
                VmrcAdministratorMode = this.VmrcAdministratorMode,
                VmrcReducedColorsMode = this.VmrcReducedColorsMode,
                #endregion

                #region ICAConnection (5)
                IcaApplicationName = this.IcaApplicationName,
                IcaClientIni = this.IcaClientIni,
                IcaEnableEncryption = this.IcaEnableEncryption,
                IcaEncryptionLevel = this.IcaEncryptionLevel,
                IcaServerIni = this.IcaServerIni,
                #endregion

                #region RDPConnection (44)
                LoadBalanceInfo = this.LoadBalanceInfo,
                DisableControlAltDelete = this.DisableControlAltDelete,
                DisableCursorBlinking = this.DisableCursorBlinking,
                DisableCursorShadow = this.DisableCursorShadow,
                DisableFullWindowDrag = this.DisableFullWindowDrag,
                DisableMenuAnimations = this.DisableMenuAnimations,
                DisableTheming = this.DisableTheming,
                DisableWallPaper = this.DisableWallPaper,
                DisableWindowsKey = this.DisableWindowsKey,
                DisplayConnectionBar = this.DisplayConnectionBar,

                EnableCompression = this.EnableCompression,
                EnableDesktopComposition = this.EnableDesktopComposition,
                EnableEncryption = this.EnableCompression,
                EnableFontSmoothing = this.EnableFontSmoothing,
                EnableSecuritySettings = this.EnableSecuritySettings,
                EnableTlsAuthentication = this.EnableTlsAuthentication,
                EnableNlaAuthentication = this.EnableNlaAuthentication,

                AcceleratorPassthrough = this.AcceleratorPassthrough,
                AllowBackgroundInput = this.AllowBackgroundInput,
                BitmapPeristence = this.BitmapPeristence,
                ConnectToConsole = this.ConnectToConsole,
                DoubleClickDetect = this.DoubleClickDetect,
                GrabFocusOnConnect = this.GrabFocusOnConnect,
                Sounds = this.Sounds,

                ConnectionTimeout = this.ConnectionTimeout,
                IdleTimeout = this.IdleTimeout,
                OverallTimeout = this.OverallTimeout,
                ShutdownTimeout = this.ShutdownTimeout,

                RedirectClipboard = this.RedirectClipboard,
                RedirectDevices = this.RedirectDevices,
                RedirectedDrives = this.RedirectedDrives,
                RedirectPorts = this.RedirectPorts,
                RedirectPrinters = this.RedirectPrinters,
                RedirectSmartCards = this.RedirectSmartCards,

                SecurityFullScreen = this.SecurityFullScreen,
                SecurityStartProgram = this.SecurityStartProgram,
                SecurityWorkingFolder = this.SecurityWorkingFolder,

                TsgwCredsSource = this.TsgwCredsSource,
                TsgwDomain = this.TsgwDomain,
                TsgwEncryptedPassword = this.TsgwEncryptedPassword,
                TsgwHostname = this.TsgwHostname,
                TsgwSeparateLogin = this.TsgwSeparateLogin,
                TsgwUsageMethod = this.TsgwUsageMethod,
                TsgwUsername = this.TsgwUsername,
                #endregion

                #region RDPConnection & ICAConnection (2)
                Colors = this.Colors,
                DesktopShare = this.DesktopShare,
                #endregion
            };

            return fav;
        }
        #endregion
    }
}