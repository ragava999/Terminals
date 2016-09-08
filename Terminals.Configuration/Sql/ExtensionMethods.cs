using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terminals.Configuration.Sql
{
    using Files.Main.Favorites;
	using Terminals.Configuration.Files.Credentials;

    public static class ExtensionMethods
    {
		// TODO: Check if DomainName and CredentialSet get set properly. (not sure for the ToFavorite method -> sure for ToConnection - but is it working!? -> needs to be tested)
		// TODO: WHAT ABOUT THE 'PluginConfigurations'???
        public static FavoriteConfigurationElement ToFavorite(this Connections connection)
        {
			// TODO: WHAT ABOUT THE 'PluginConfigurations'???
			// TODO: Maybe there's a problem with the 'DomainName' and the 'CredentialSet' itself.

			// Everything except 'EncryptedPassword' and 'IsDatabaseFavorite' will be syncronized

            string[] groups = (from g in connection.Groups select g.Name).Distinct().ToArray();

            FavoriteConfigurationElement favorite = new FavoriteConfigurationElement()
            {
                Name = connection.Name,
                Notes = connection.Notes,
                Protocol = connection.Protocol,
                Tags = String.Join(",", groups),

                //ToolBarIcon = connection.ToolBarIcon

                ServerName = connection.ServerName
            };

            #region Credentials
            try
            {
                using (TerminalsObjectContext dc = TerminalsObjectContext.Create())
                {
                    User user = GetMyUser(dc);

                    if (user != null)
                    {
                    	// Get all credentials the user has
                    	int[] credentialIds = dc.Credentials.Where(x=> x.UserId == user.Id).Select(x => x.Id).ToArray();

                        if (credentialIds.Length > 0)
                        {
                        	// Get the credential for the connection                       	
                            /*int credentialId = dc.CredentialLookups.Where(x => credentialIds.Contains(x.CredentialId) && x.ConnectionId == connection.Id).Select(x => x.CredentialId).FirstOrDefault();
                            
                            if (credentialId > 0)
                            {
                                favorite.XmlCredentialSetName = dc.Credentials.Where(x => x.Id == credentialId).Select(x => x.Name).FirstOrDefault();
                            }
                            */
                           
                           var cred = dc.Credentials.Where(x => x.Connections.Contains(connection) && credentialIds.Contains(x.Id)).FirstOrDefault();
                           
                           if (cred != null)
                           	favorite.XmlCredentialSetName = cred.Name;
                        }
                    }
                }
            }
            catch
            {
                Kohl.Framework.Logging.Log.Error("Unable to load the credential set from database for the connection.");
            }
            #endregion

			/*
			if (connection.PuttyDontAddDomainToUserName.HasValue)
				favorite.PuttyDontAddDomainToUserName = connection.PuttyDontAddDomainToUserName.Value;

			if (connection.PuttyEnableX11.HasValue)
				favorite.PuttyEnableX11 = connection.PuttyEnableX11.Value;

			if (connection.PuttyProxyHost.HasValue)
				favorite.PuttyProxyHost = connection.PuttyProxyHost.Value;

			if (connection.PuttyProxyPort.HasValue)
				favorite.PuttyProxyPort = connection.PuttyProxyPort.Value;

			if (connection.PuttyProxyType.HasValue)
				favorite.PuttyProxyType = connection.PuttyProxyType.Value;

			if (connection.PuttyProxyType.HasValue)
				favorite.PuttyProxyType = connection.PuttyProxyType.Value;

			if (connection.TabColor.HasValue)
				favorite.TabColor = connection.TabColor.Value;

			if (connection.TsgwXmlCredentialSetName.HasValue)
				favorite.TsgwXmlCredentialSetName = connection.TsgwXmlCredentialSetName.Value;
			*/

            if (connection.NewWindow.HasValue)
                favorite.NewWindow = connection.NewWindow.Value;

            if (connection.Port.HasValue)
                favorite.Port = connection.Port.Value;

            if (connection.DesktopSizeHeight.HasValue)
                favorite.DesktopSizeHeight = connection.DesktopSizeHeight.Value;

            if (connection.DesktopSizeWidth.HasValue)
                favorite.DesktopSizeWidth = connection.DesktopSizeWidth.Value;

            if (connection.DesktopSize.HasValue)
                favorite.DesktopSize = (DesktopSize)connection.DesktopSize.Value;

            if (connection.ExecuteBeforeConnect.HasValue)
                favorite.ExecuteBeforeConnect = connection.ExecuteBeforeConnect.Value;

            favorite.ExecuteBeforeConnectCommand = connection.ExecuteBeforeConnectCommand;
            favorite.ExecuteBeforeConnectArgs = connection.ExecuteBeforeConnectArgs;
            favorite.ExecuteBeforeConnectInitialDirectory = connection.ExecuteBeforeConnectInitialDirectory;

            if (connection.ExecuteBeforeConnectWaitForExit.HasValue)
                favorite.ExecuteBeforeConnectWaitForExit = connection.ExecuteBeforeConnectWaitForExit.Value;

            if (!string.IsNullOrEmpty(connection.ExplorerStyle))
                favorite.ExplorerStyle = connection.ExplorerStyle;

            if (!string.IsNullOrEmpty(connection.ExplorerDirectory))
                favorite.ExplorerDirectory = connection.ExplorerDirectory;

            if (!string.IsNullOrEmpty(connection.ExplorerDirectoryDual))
                favorite.ExplorerDirectoryDual = connection.ExplorerDirectoryDual;

            if (!string.IsNullOrEmpty(connection.ExplorerDirectoryTripple))
                favorite.ExplorerDirectoryTripple = connection.ExplorerDirectoryTripple;

            if (!string.IsNullOrEmpty(connection.ExplorerDirectoryQuad))
                favorite.ExplorerDirectoryQuad = connection.ExplorerDirectoryQuad;

            favorite.HtmlFormFieldsString = connection.HtmlFormFieldsString;

            if (connection.BrowserAuthentication.HasValue)
                favorite.BrowserAuthentication = (BrowserAuthentication)connection.BrowserAuthentication.Value;

            if (connection.HttpBrowser.HasValue)
                favorite.HttpBrowser = (BrowserType)connection.HttpBrowser.Value;

            favorite.Url = connection.Url;

            if (connection.PuttyConnectionType.HasValue)
                favorite.PuttyConnectionType = (PuttyConnectionType)connection.PuttyConnectionType.Value;

            favorite.PuttySession = connection.PuttySession;

            if (connection.PuttyCompression.HasValue)
                favorite.PuttyCompression = connection.PuttyCompression.Value;

            if (connection.PuttyPasswordTimeout.HasValue)
                favorite.PuttyPasswordTimeout = connection.PuttyPasswordTimeout.Value;

            if (connection.PuttyVerbose.HasValue)
                favorite.PuttyVerbose = connection.PuttyVerbose.Value;

            if (connection.PuttyShowOptions.HasValue)
                favorite.PuttyShowOptions = connection.PuttyShowOptions.Value;

            if (connection.PuttyCloseWindowOnExit.HasValue)
                favorite.PuttyCloseWindowOnExit = (PuttyCloseWindowOnExit)connection.PuttyCloseWindowOnExit.Value;

            favorite.GenericWorkingDirectory = connection.GenericWorkingDirectory;
            favorite.GenericProgramPath = connection.GenericProgramPath;
            favorite.GenericArguments = connection.GenericArguments;
            favorite.RAdminPhonebookPath = connection.RAdminPhonebookPath;

            if (connection.RAdminThrough.HasValue)
                favorite.RAdminThrough = connection.RAdminThrough.Value;

            favorite.RAdminThroughServerName = connection.RAdminThroughServerName;
            favorite.RAdminThroughPort = connection.RAdminThroughPort;

            if (connection.RAdminStandardConnectionMode.HasValue)
                favorite.RAdminStandardConnectionMode = connection.RAdminStandardConnectionMode.Value;

            if (connection.RAdminTelnetMode.HasValue)
                favorite.RAdminTelnetMode = connection.RAdminTelnetMode.Value;

            if (connection.RAdminViewOnlyMode.HasValue)
                favorite.RAdminViewOnlyMode = connection.RAdminViewOnlyMode.Value;

            if (connection.RAdminFileTransferMode.HasValue)
                favorite.RAdminFileTransferMode = connection.RAdminFileTransferMode.Value;

            if (connection.RAdminShutdown.HasValue)
                favorite.RAdminShutdown = connection.RAdminShutdown.Value;

            if (connection.RAdminChatMode.HasValue)
                favorite.RAdminChatMode = connection.RAdminChatMode.Value;

            if (connection.RAdminVoiceChatMode.HasValue)
                favorite.RAdminVoiceChatMode = connection.RAdminVoiceChatMode.Value;

            if (connection.RAdminSendTextMessageMode.HasValue)
                favorite.RAdminSendTextMessageMode = connection.RAdminSendTextMessageMode.Value;

            if (connection.RAdminUseFullScreen.HasValue)
                favorite.RAdminUseFullScreen = connection.RAdminUseFullScreen.Value;

            if (connection.RAdminUpdates.HasValue)
                favorite.RAdminUpdates = connection.RAdminUpdates.Value;

            favorite.RAdminColorMode = connection.RAdminColorMode;

            if (connection.ConsoleRows.HasValue)
                favorite.ConsoleRows = connection.ConsoleRows.Value;

            if (connection.ConsoleCols.HasValue)
                favorite.ConsoleCols = connection.ConsoleCols.Value;

            favorite.ConsoleFont = connection.ConsoleFont;
            favorite.ConsoleBackColor = connection.ConsoleBackColor;
            favorite.ConsoleTextColor = connection.ConsoleTextColor;
            favorite.ConsoleCursorColor = connection.ConsoleCursorColor;

            if (connection.Ssh1.HasValue)
                favorite.Ssh1 = connection.Ssh1.Value;

            if (connection.AuthMethod.HasValue)
                favorite.AuthMethod = (AuthMethod)connection.AuthMethod.Value;

            favorite.KeyTag = connection.KeyTag;

            if (connection.VncAutoScale.HasValue)
                favorite.VncAutoScale = connection.VncAutoScale.Value;

            if (connection.VncViewOnly.HasValue)
                favorite.VncViewOnly = connection.VncViewOnly.Value;

            if (connection.VncDisplayNumber.HasValue)
                favorite.VncDisplayNumber = connection.VncDisplayNumber.Value;

            if (connection.VmrcReducedColorsMode.HasValue)
                favorite.VmrcReducedColorsMode = connection.VmrcReducedColorsMode.Value;

            if (connection.VmrcAdministratorMode.HasValue)
                favorite.VmrcAdministratorMode = connection.VmrcAdministratorMode.Value;

            favorite.IcaApplicationName = connection.IcaApplicationName;
            favorite.IcaServerIni = connection.IcaServerIni;
            favorite.IcaClientIni = connection.IcaClientIni;

            if (connection.IcaEnableEncryption.HasValue)
                favorite.IcaEnableEncryption = connection.IcaEnableEncryption.Value;

            favorite.IcaEncryptionLevel = connection.IcaEncryptionLevel;
            favorite.LoadBalanceInfo = connection.LoadBalanceInfo;

            if (connection.ConnectToConsole.HasValue)
                favorite.ConnectToConsole = connection.ConnectToConsole.Value;

            if (connection.RedirectPrinters.HasValue)
                favorite.RedirectPrinters = connection.RedirectPrinters.Value;

            if (connection.RedirectSmartCards.HasValue)
                favorite.RedirectSmartCards = connection.RedirectSmartCards.Value;

            if (connection.RedirectClipboard.HasValue)
                favorite.RedirectClipboard = connection.RedirectClipboard.Value;

            if (connection.RedirectDevices.HasValue)
                favorite.RedirectDevices = connection.RedirectDevices.Value;

            if (connection.TsgwUsageMethod.HasValue)
                favorite.TsgwUsageMethod = connection.TsgwUsageMethod.Value;

            favorite.TsgwHostname = connection.TsgwHostname;

            if (connection.TsgwCredsSource.HasValue)
                favorite.TsgwCredsSource = connection.TsgwCredsSource.Value;

            if (connection.TsgwSeparateLogin.HasValue)
                favorite.TsgwSeparateLogin = connection.TsgwSeparateLogin.Value;

            favorite.TsgwUsername = connection.TsgwUsername;
            favorite.TsgwDomain = connection.TsgwDomain;
            favorite.TsgwEncryptedPassword = connection.TsgwEncryptedPassword;

            if (connection.Sounds.HasValue)
                favorite.Sounds = (RemoteSounds)connection.Sounds.Value;

            favorite.RedirectedDrives = connection.RedirectedDrives;

            if (connection.RedirectPorts.HasValue)
                favorite.RedirectPorts = connection.RedirectPorts.Value;

            if (connection.ShutdownTimeout.HasValue)
                favorite.ShutdownTimeout = connection.ShutdownTimeout.Value;

            if (connection.OverallTimeout.HasValue)
                favorite.OverallTimeout = connection.OverallTimeout.Value;

            if (connection.ConnectionTimeout.HasValue)
                favorite.ConnectionTimeout = connection.ConnectionTimeout.Value;

            if (connection.IdleTimeout.HasValue)
                favorite.IdleTimeout = connection.IdleTimeout.Value;

            favorite.SecurityWorkingFolder = connection.SecurityWorkingFolder;
            favorite.SecurityStartProgram = connection.SecurityStartProgram;

            if (connection.SecurityFullScreen.HasValue)
                favorite.SecurityFullScreen = connection.SecurityFullScreen.Value;

            if (connection.EnableSecuritySettings.HasValue)
                favorite.EnableSecuritySettings = connection.EnableSecuritySettings.Value;

            if (connection.GrabFocusOnConnect.HasValue)
                favorite.GrabFocusOnConnect = connection.GrabFocusOnConnect.Value;

            if (connection.EnableEncryption.HasValue)
                favorite.EnableEncryption = connection.EnableEncryption.Value;

            if (connection.DisableWindowsKey.HasValue)
                favorite.DisableWindowsKey = connection.DisableWindowsKey.Value;

            if (connection.DoubleClickDetect.HasValue)
                favorite.DoubleClickDetect = connection.DoubleClickDetect.Value;

            if (connection.DisplayConnectionBar.HasValue)
                favorite.DisplayConnectionBar = connection.DisplayConnectionBar.Value;

            if (connection.DisableControlAltDelete.HasValue)
                favorite.DisableControlAltDelete = connection.DisableControlAltDelete.Value;

            if (connection.AcceleratorPassthrough.HasValue)
                favorite.AcceleratorPassthrough = connection.AcceleratorPassthrough.Value;

            if (connection.EnableCompression.HasValue)
                favorite.EnableCompression = connection.EnableCompression.Value;

            if (connection.BitmapPeristence.HasValue)
                favorite.BitmapPeristence = connection.BitmapPeristence.Value;

            if (connection.EnableTlsAuthentication.HasValue)
                favorite.EnableTlsAuthentication = connection.EnableTlsAuthentication.Value;

            if (connection.EnableNlaAuthentication.HasValue)
                favorite.EnableNlaAuthentication = connection.EnableNlaAuthentication.Value;

            if (connection.AllowBackgroundInput.HasValue)
                favorite.AllowBackgroundInput = connection.AllowBackgroundInput.Value;

            if (connection.DisableTheming.HasValue)
                favorite.DisableTheming = connection.DisableTheming.Value;

            if (connection.DisableMenuAnimations.HasValue)
                favorite.DisableMenuAnimations = connection.DisableMenuAnimations.Value;

            if (connection.DisableFullWindowDrag.HasValue)
                favorite.DisableFullWindowDrag = connection.DisableFullWindowDrag.Value;

            if (connection.DisableCursorBlinking.HasValue)
                favorite.DisableCursorBlinking = connection.DisableCursorBlinking.Value;

            if (connection.EnableDesktopComposition.HasValue)
                favorite.EnableDesktopComposition = connection.EnableDesktopComposition.Value;

            if (connection.EnableFontSmoothing.HasValue)
                favorite.EnableFontSmoothing = connection.EnableFontSmoothing.Value;

            if (connection.DisableCursorShadow.HasValue)
                favorite.DisableCursorShadow = connection.DisableCursorShadow.Value;

            if (connection.DisableWallPaper.HasValue)
                favorite.DisableWallPaper = connection.DisableWallPaper.Value;

            if (connection.Colors.HasValue)
                favorite.Colors = (Colors)connection.Colors.Value;

            favorite.DesktopShare = connection.DesktopShare;

            return favorite;
        }

        internal static User GetMyUser (TerminalsObjectContext dc)
        {
            try
            {
                User user = (from u in dc.User
                                where u.DomainName.ToUpper() == Kohl.Framework.Info.UserInfo.UserDomain.ToUpper() && u.UserName.ToUpper() == Kohl.Framework.Info.UserInfo.UserNameAlias.ToUpper()
                                select u).FirstOrDefault();

                if (user == null)
                {
                    user = new Configuration.Sql.User() { UserName = Kohl.Framework.Info.UserInfo.UserNameAlias.ToUpper(), DomainName = Kohl.Framework.Info.UserInfo.UserDomain.ToUpper() };
                    dc.User.AddObject(user);
                    dc.SaveChanges();
                    Kohl.Framework.Logging.Log.Info("Database user has been created.");
                }

                return user;
            }
            catch
            {
                Kohl.Framework.Logging.Log.Fatal("Error retrieving database user.");
                return null;
            }
        }

        public static Connections ToConnection(this FavoriteConfigurationElement favorite, TerminalsObjectContext dc, Connections connection = null)
        {
            // It won't either be changed or set.
            if (connection == null)
                connection = new Connections();
            
			// TODO: WHAT ABOUT THE 'PluginConfigurations'???

			// Everything except 'EncryptedPassword' and 'IsDatabaseFavorite' will be syncronized

            #region Tags
            connection.Groups.Clear();

            foreach (string tag in favorite.TagList)
            {
                Groups group = dc.Groups.Where(g => g.Name.ToLower() == tag.ToLower()).FirstOrDefault();

                if (group == null)
                {
                    group = new Groups() { Name = tag };
                }

                bool contains = false;
                foreach (Groups availableGroup in connection.Groups)
                {
                    if (availableGroup.Name.ToLower() == tag.ToLower())
                    {
                        contains = true;
                        break;
                    }
                }

                if (!contains)
                {
                   connection.Groups.Add(group);
                }
            }
            #endregion

            #region Credentials
            try
            {
                User user = GetMyUser(dc);

                if (user != null)
                {
                    Credentials credential = dc.Credentials.Where(x => x.Name.ToUpper() == favorite.XmlCredentialSetName.ToUpper() && x.UserId == user.Id).FirstOrDefault();

                    if (credential == null)
                    {
                        credential = new Credentials()
                        {
                            Name = favorite.XmlCredentialSetName,
                            DomainName = favorite.DomainName,
                            UserName = favorite.UserName,
                            Password = favorite.Password,
                            UserId = user.Id
                        };

                        dc.Credentials.AddObject(credential);
                        dc.SaveChanges();
                    }

                    // Get all the user's credentials
                    int[] credentialIds = dc.Credentials.Where(x=> x.UserId == user.Id).Select(x => x.Id).ToArray();
                    
                    // If there's more than one ...
                    if (credentialIds.Length > 0)
                    {
                        // check if there's a specific credentialId
                        int credentialId = dc.Credentials.Where(x => credentialIds.Contains(x.Id) && x.Connections.Contains(connection)).Select(x => x.Id).FirstOrDefault();
                        
                        // if yes update it
                        if (credentialId > 0)
                        {
                            // Find the lookup that
                            // contains the credential id for the
                            // specific connection and update it
                            Credentials lookup = dc.Credentials.Where(x => x.Connections.Contains(connection) && x.Id == credentialId).FirstOrDefault();

                            if (lookup == null)
                            {
                                dc.ExecuteStoreCommand("INSERT INTO CredentialLookup (ConnectionId, CredentialId) VALUES (" + connection.Id + ", " + credential.Id + ")", null);
                         
                                /*
                                lookup = new CredentialLookup()
                                {
                                    ConnectionId = connection.Id,
                                    CredentialId = credential.Id
                                };

                                dc.CredentialLookups.InsertOnSubmit(lookup);
                                dc.SubmitChanges();
                                */
                            }
                            else
                            {
                                dc.ExecuteStoreCommand("UPDATE CredentialLookup SET CredentialId = " + credential.Id + " WHERE CredentialId = " + credentialId + " AND ConnectionId = " + connection.Id, null);
                                
                                //lookup.CredentialId = credential.Id;
                            }
                        }
                        else
                        // create a new one
                        {
                            dc.ExecuteStoreCommand("INSERT INTO CredentialLookup (ConnectionId, CredentialId) VALUES (" + connection.Id + ", " + credential.Id + ")", null);
                         
                            /*
                            CredentialLookup lookup = new CredentialLookup()
                            {
                                ConnectionId = connection.Id,
                                CredentialId = credential.Id
                            };

                            dc.CredentialLookups.InsertOnSubmit(lookup);
                            dc.SubmitChanges(System.Data.Linq.ConflictMode.FailOnFirstConflict);
                            */
                        }

                    }
                    else
                    {
                        dc.ExecuteStoreCommand("INSERT INTO CredentialLookup (ConnectionId, CredentialId) VALUES (" + connection.Id + ", " + credential.Id + ")", null);
                         
                        /*
                        CredentialLookup lookup = new CredentialLookup()
                            {
                                ConnectionId = connection.Id,
                                CredentialId = credential.Id
                            };

                        dc.CredentialLookups.InsertOnSubmit(lookup);
                        dc.SubmitChanges();*/
                    }
                }
            }
            catch
            {
                Kohl.Framework.Logging.Log.Error("Error setting credential for connection in database.");
            }
            #endregion

			connection.AcceleratorPassthrough = favorite.AcceleratorPassthrough;
			connection.AllowBackgroundInput = favorite.AllowBackgroundInput;
			connection.AuthMethod = (int)favorite.AuthMethod;
			connection.BitmapPeristence = favorite.BitmapPeristence;
			connection.BrowserAuthentication = (int)favorite.BrowserAuthentication;
			connection.Colors = (int)favorite.Colors;
			connection.ConnectionTimeout = favorite.ConnectionTimeout;
			connection.ConnectToConsole = favorite.ConnectToConsole;
			connection.ConsoleBackColor = favorite.ConsoleBackColor;
			connection.ConsoleCols = favorite.ConsoleCols;
			connection.ConsoleCursorColor = favorite.ConsoleCursorColor;
			connection.ConsoleFont = favorite.ConsoleFont;
			connection.ConsoleRows = favorite.ConsoleRows;
			connection.ConsoleTextColor = favorite.ConsoleTextColor;
			connection.DesktopShare = favorite.DesktopShare;
			connection.DesktopSize = (int)favorite.DesktopSize;
			connection.DesktopSizeHeight = favorite.DesktopSizeHeight;
			connection.DesktopSizeWidth = favorite.DesktopSizeWidth;
			connection.DisableControlAltDelete = favorite.DisableControlAltDelete;
			connection.DisableCursorBlinking = favorite.DisableCursorBlinking;
			connection.DisableCursorShadow = favorite.DisableCursorShadow;
			connection.DisableFullWindowDrag = favorite.DisableFullWindowDrag;
			connection.DisableMenuAnimations = favorite.DisableMenuAnimations;
			connection.DisableTheming = favorite.DisableTheming;
			connection.DisableWallPaper = favorite.DisableWallPaper;
			connection.DisableWindowsKey = favorite.DisableWindowsKey;
			connection.DisplayConnectionBar = favorite.DisplayConnectionBar;
			connection.DoubleClickDetect = favorite.DoubleClickDetect;
			connection.EnableCompression = favorite.EnableCompression;
			connection.EnableDesktopComposition = favorite.EnableDesktopComposition;
			connection.EnableEncryption = favorite.EnableEncryption;
			connection.EnableFontSmoothing = favorite.EnableFontSmoothing;
			connection.EnableNlaAuthentication = favorite.EnableNlaAuthentication;
			connection.EnableSecuritySettings = favorite.EnableSecuritySettings;
			connection.EnableTlsAuthentication = favorite.EnableTlsAuthentication;
			connection.ExecuteBeforeConnect = favorite.ExecuteBeforeConnect;
			connection.ExecuteBeforeConnectArgs = favorite.ExecuteBeforeConnectArgs;
			connection.ExecuteBeforeConnectCommand = favorite.ExecuteBeforeConnectCommand;
			connection.ExecuteBeforeConnectInitialDirectory = favorite.ExecuteBeforeConnectInitialDirectory;
			connection.ExecuteBeforeConnectWaitForExit = favorite.ExecuteBeforeConnectWaitForExit;
			connection.ExplorerDirectory = favorite.ExplorerDirectory;
			connection.ExplorerDirectoryDual = favorite.ExplorerDirectoryDual;
			connection.ExplorerDirectoryQuad = favorite.ExplorerDirectoryQuad;
			connection.ExplorerDirectoryTripple = favorite.ExplorerDirectoryTripple;
			connection.ExplorerStyle = favorite.ExplorerStyle;
			connection.GenericArguments = favorite.GenericArguments;
			connection.GenericProgramPath = favorite.GenericProgramPath;
			connection.GenericWorkingDirectory = favorite.GenericWorkingDirectory;
			connection.GrabFocusOnConnect = favorite.GrabFocusOnConnect;
			connection.HtmlFormFieldsString = favorite.HtmlFormFieldsString;
			connection.HttpBrowser = (int) favorite.HttpBrowser;
			connection.IcaApplicationName = favorite.IcaApplicationName;
			connection.IcaClientIni = favorite.IcaClientIni;
			connection.IcaEnableEncryption = favorite.IcaEnableEncryption;
			connection.IcaEncryptionLevel = favorite.IcaEncryptionLevel;
			connection.IcaServerIni = favorite.IcaServerIni;
			connection.IdleTimeout = favorite.IdleTimeout;
			connection.KeyTag = favorite.KeyTag;
			connection.LoadBalanceInfo = favorite.LoadBalanceInfo;
			connection.Name = favorite.Name;
			connection.NewWindow = favorite.NewWindow;
			connection.Notes = favorite.Notes;
			connection.OverallTimeout = favorite.OverallTimeout;
			connection.Port = favorite.Port;
			connection.Protocol = favorite.Protocol;
			connection.PuttyCloseWindowOnExit = (int)favorite.PuttyCloseWindowOnExit;
			connection.PuttyCompression = favorite.PuttyCompression;
			connection.PuttyConnectionType = (int)favorite.PuttyConnectionType;
			//connection.PuttyDontAddDomainToUserName = favorite.PuttyDontAddDomainToUserName;
			//connection.PuttyEnableX11 = favorite.PuttyEnableX11;
			connection.PuttyPasswordTimeout = favorite.PuttyPasswordTimeout;
			//connection.PuttyProxyHost = favorite.PuttyProxyHost;
			//connection.PuttyProxyPort = favorite.PuttyProxyPort;
			//connection.PuttyProxyType = (int)favorite.PuttyProxyType;
			connection.PuttySession = favorite.PuttySession;
			connection.PuttyShowOptions = favorite.PuttyShowOptions;
			connection.PuttyVerbose = favorite.PuttyVerbose;
			connection.RAdminChatMode = favorite.RAdminChatMode;
			connection.RAdminColorMode = favorite.RAdminColorMode;
			connection.RAdminFileTransferMode = favorite.RAdminFileTransferMode;
			connection.RAdminPhonebookPath = favorite.RAdminPhonebookPath;
			connection.RAdminSendTextMessageMode = favorite.RAdminSendTextMessageMode;
			connection.RAdminShutdown = favorite.RAdminShutdown;
			connection.RAdminStandardConnectionMode = favorite.RAdminStandardConnectionMode;
			connection.RAdminTelnetMode = favorite.RAdminTelnetMode;
			connection.RAdminThrough = favorite.RAdminThrough;
			connection.RAdminThroughPort = favorite.RAdminThroughPort;
			connection.RAdminThroughServerName = favorite.RAdminThroughServerName;
			connection.RAdminUpdates = favorite.RAdminUpdates;
			connection.RAdminUseFullScreen = favorite.RAdminUseFullScreen;
			connection.RAdminViewOnlyMode = favorite.RAdminViewOnlyMode;
			connection.RAdminVoiceChatMode = favorite.RAdminVoiceChatMode;
			connection.RedirectClipboard = favorite.RedirectClipboard;
			connection.RedirectDevices = favorite.RedirectDevices;
			connection.RedirectedDrives = favorite.RedirectedDrives;
			connection.RedirectPorts = favorite.RedirectPorts;
			connection.RedirectPrinters = favorite.RedirectPrinters;
			connection.RedirectSmartCards = favorite.RedirectSmartCards;
			connection.SecurityFullScreen = favorite.SecurityFullScreen;
			connection.SecurityStartProgram = favorite.SecurityStartProgram;
			connection.SecurityWorkingFolder = favorite.SecurityWorkingFolder;
			connection.ServerName = favorite.ServerName;
			connection.ShutdownTimeout = favorite.ShutdownTimeout;
			connection.Sounds = (int)favorite.Sounds;
			connection.Ssh1 = favorite.Ssh1;
			//connection.ToolBarIcon = favorite.ToolBarIcon;
			//connection.TabColor = favorite.TabColor;
			connection.TsgwCredsSource = favorite.TsgwCredsSource;
			connection.TsgwDomain = favorite.TsgwDomain;
			connection.TsgwEncryptedPassword = favorite.TsgwEncryptedPassword;
			connection.TsgwHostname = favorite.TsgwHostname;
			connection.TsgwSeparateLogin = favorite.TsgwSeparateLogin;
			connection.TsgwUsageMethod = favorite.TsgwUsageMethod;
			connection.TsgwUsername = favorite.TsgwUsername;
			//connection.TsgwXmlCredentialSetName = favorite.TsgwXmlCredentialSetName;
			connection.Url = favorite.Url;
			connection.VmrcAdministratorMode = favorite.VmrcAdministratorMode;
			connection.VmrcReducedColorsMode = favorite.VmrcReducedColorsMode;
			connection.VncAutoScale = favorite.VncAutoScale;
			connection.VncDisplayNumber = favorite.VncDisplayNumber;
			connection.VncViewOnly = favorite.VncViewOnly;

            return connection;
        }
    }
}
