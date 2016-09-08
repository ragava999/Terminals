using System;
using System.Collections.Generic;
using System.Xml;

using Kohl.Framework.Logging;
using Terminals.Configuration.Files.Main.Favorites;

namespace Terminals.ExportImport.Import
{
    public class ImportCodePlexTerminals : IImport
    {
        public const string FILE_EXTENSION = ".xml";

        public static readonly string PROVIDER_NAME = "CodePlex Terminals favorites";

        List<FavoriteConfigurationElement> IImport.ImportFavorites(string Filename)
        {
            return ImportXML(Filename, true);
        }

        public string Name
        {
            get { return PROVIDER_NAME; }
        }

        public string KnownExtension
        {
            get { return FILE_EXTENSION; }
        }

        /// <summary>
        ///     Loads a new collection of favorites from source file.
        ///     The newly created favorites aren't imported into configuration.
        /// </summary>
        private static List<FavoriteConfigurationElement> ImportXML(string file, bool showOnToolbar)
        {
            List<FavoriteConfigurationElement> favorites = ImportFavorites(file);
            return favorites;
        }

        private static List<FavoriteConfigurationElement> ImportFavorites(string file)
        {
            try
            {
				return TryImport(file);
            }
            catch (Exception ex)
            {
                Log.Fatal("Unable to import the whole CodePlex Terminals XML file.", ex);
                return  new List<FavoriteConfigurationElement>();
            }
        }

        private static List<FavoriteConfigurationElement> TryImport(string filename)
        {
            using (var reader = new XmlTextReader(filename))
            {
                var propertyReader = new PropertyReader(reader);
				var context = new ImportCodePlexTerminalsContext(propertyReader);
                while (propertyReader.Read())
                {
                	try
                	{
                    	ReadProperty(context);
                	}
                	catch (Exception ex)
                	{
                		Log.Error("Unable to import some part of the CodePlex Terminals file.", ex);
                	}
                }

                return context.Favorites;
            }
        }

		private static void ReadProperty(ImportCodePlexTerminalsContext context)
        {
            switch (context.Reader.NodeType)
            {
                case XmlNodeType.Element:
                    switch (context.Reader.NodeName)
                    {
                        case "favorite":
                            context.SetNewCurrent();
                            break;
                        case "userName":
                            context.Current.UserName = context.Reader.ReadString();
                            break;
                        case "password":
                            context.ReadPassword();
                            break;

                        case "acceleratorPassthrough":
                            context.Current.AcceleratorPassthrough = context.Reader.ReadBool();
                            break;
                        case "allowBackgroundInput":
                            context.Current.AllowBackgroundInput = context.Reader.ReadBool();
                            break;
                        case "bitmapPeristence":
                        	if (context.Reader.ToString().ToUpperInvariant() == "RDP" || context.Reader.ReadBool())
                            	context.Current.BitmapPeristence = true;
                            else
                            	context.Current.BitmapPeristence = false;
                            break;
                        case "connectionTimeout":
                            context.Current.ConnectionTimeout = context.Reader.ReadInt();
                            break;
                        case "consolefont":
                            context.Current.ConsoleFont = context.Reader.ReadString();
                            break;
                        case "consolerows":
                            context.Current.ConsoleRows = context.Reader.ReadInt();
                            break;
                        case "consolecols":
                            context.Current.ConsoleCols = context.Reader.ReadInt();
                            break;
                        case "consolebackcolor":
                            context.Current.ConsoleBackColor = context.Reader.ReadString();
                            break;
                        case "consoletextcolor":
                            context.Current.ConsoleTextColor = context.Reader.ReadString();
                            break;
                        case "consolecursorcolor":
                            context.Current.ConsoleCursorColor = context.Reader.ReadString();
                            break;
                        case "connectToConsole":
                            context.Current.ConnectToConsole = context.Reader.ReadBool();
                            break;
                        case "colors":
                            context.Current.Colors = context.Reader.ReadColors();
                            break;
						case "credential":
                            context.Current.XmlCredentialSetName = context.Reader.ReadString();
                            break;
                        case "disableWindowsKey":
                            context.Current.DisableWindowsKey = context.Reader.ReadBool();
                            break;
                        case "doubleClickDetect":
                            context.Current.DoubleClickDetect = context.Reader.ReadBool();
                            break;
                        case "displayConnectionBar":
                            context.Current.DisplayConnectionBar = context.Reader.ReadBool();
                            break;
                        case "disableControlAltDelete":
                            context.Current.DisableControlAltDelete = context.Reader.ReadBool();
                            break;
                        case "domainName":
                            context.Current.DomainName = context.Reader.ReadString();
                            break;
                        case "desktopSizeHeight":
                            context.Current.DesktopSizeHeight = context.Reader.ReadInt();
                            break;
                        case "desktopSizeWidth":
                            context.Current.DesktopSizeWidth = context.Reader.ReadInt();
                            break;
                        case "desktopSize":
                            context.Current.DesktopSize = context.Reader.ReadDesktopSize();
                            
                            if (context.Current.DesktopSize == DesktopSize.Custom)
                            {
                            	switch (context.Reader.ToString())
                            	{
                            		case "x480":
                            			context.Current.DesktopSizeHeight = 640;
                            			context.Current.DesktopSizeWidth = 480;
                            			break;
                            		case "x600":
                            			context.Current.DesktopSizeHeight = 800;
                            			context.Current.DesktopSizeWidth = 600;
                            			break;
                            		case "x768":
                            			context.Current.DesktopSizeHeight = 1024;
                            			context.Current.DesktopSizeWidth = 768;
                            			break;
                            		case "x864":
                            			context.Current.DesktopSizeHeight = 1152;
                            			context.Current.DesktopSizeWidth = 864;
                            			break;
                            		default:
                        			//case "x1024":
                            			context.Current.DesktopSizeHeight = 1280;
                            			context.Current.DesktopSizeWidth = 1024;
                            			break;
                            	}
                            }
                            break;
                        case "desktopShare":
                            context.Current.DesktopShare = context.Reader.ReadString();
                            break;
                        case "disableTheming":
                            context.Current.DisableTheming = context.Reader.ReadBool();
                            break;
                        case "disableMenuAnimations":
                            context.Current.DisableMenuAnimations = context.Reader.ReadBool();
                            break;
                        case "disableFullWindowDrag":
                            context.Current.DisableFullWindowDrag = context.Reader.ReadBool();
                            break;
                        case "disableCursorBlinking":
                            context.Current.DisableCursorBlinking = context.Reader.ReadBool();
                            break;
                        case "disableCursorShadow":
                            context.Current.DisableCursorShadow = context.Reader.ReadBool();
                            break;
                        case "disableWallPaper":
                            context.Current.DisableWallPaper = context.Reader.ReadBool();
                            break;
                        case "executeBeforeConnect":
                            context.Current.ExecuteBeforeConnect = context.Reader.ReadBool();
                            break;
                        case "executeBeforeConnectCommand":
                            context.Current.ExecuteBeforeConnectCommand = context.Reader.ReadString();
                            break;
                        case "executeBeforeConnectArgs":
                            context.Current.ExecuteBeforeConnectArgs = context.Reader.ReadString();
                            break;
                        case "executeBeforeConnectInitialDirectory":
                            context.Current.ExecuteBeforeConnectInitialDirectory = context.Reader.ReadString();
                            break;
                        case "executeBeforeConnectWaitForExit":
                            context.Current.ExecuteBeforeConnectWaitForExit = context.Reader.ReadBool();
                            break;
                        case "enableDesktopComposition":
                            context.Current.EnableDesktopComposition = context.Reader.ReadBool();
                            break;
                        case "enableFontSmoothing":
                            context.Current.EnableFontSmoothing = context.Reader.ReadBool();
                            break;
                        case "enableSecuritySettings":
                            context.Current.EnableSecuritySettings = context.Reader.ReadBool();
                            break;
                        case "enableEncryption":
                            context.Current.EnableEncryption = context.Reader.ReadBool();
                            break;
                        case "enableCompression":
                            context.Current.EnableCompression = context.Reader.ReadBool();
                            break;
                        case "enableTLSAuthentication":
                            context.Current.EnableTlsAuthentication = context.Reader.ReadBool();
                            break;
                        case "enableNLAAuthentication":
                            context.Current.EnableNlaAuthentication = context.Reader.ReadBool();
                            break;
                        case "grabFocusOnConnect":
                            context.Current.GrabFocusOnConnect = context.Reader.ReadBool();
                            break;
                        case "idleTimeout":
                            context.Current.IdleTimeout = context.Reader.ReadInt();
                            break;
                        case "icaServerINI":
                            context.Current.IcaServerIni = context.Reader.ReadString();
                            break;
                        case "icaClientINI":
                            context.Current.IcaClientIni = context.Reader.ReadString();
                            break;
                        case "icaEncryptionLevel":
                            context.Current.IcaEncryptionLevel = context.Reader.ReadString();
                            break;
                        case "iCAApplicationName":
                            context.Current.IcaApplicationName = context.Reader.ReadString();
                            break;
                        case "iCAApplicationWorkingFolder":
                            //context.Current.ICAApplicationWorkingFolder = context.Reader.ReadString();
                            Log.Warn("The CodePlex Terminals field 'iCAApplicationWorkingFolder' is not used in Terminals, the value '" + context.Reader.ReadString() + "' won't be imported.");
                            break;
                        case "iCAApplicationPath":
                            //context.Current.IcaApplicationPath = context.Reader.ReadString();
                            Log.Warn("The CodePlex Terminals field 'iCAApplicationPath' is not used in Terminals, the value '" + context.Reader.ReadString() + "' won't be imported.");
                            break;
                        case "icaEnableEncryption":
                            context.Current.IcaEnableEncryption = context.Reader.ReadBool();
                            break;
                        case "keyTag":
                            context.Current.KeyTag = context.Reader.ReadString();
                            break;
                        case "newWindow":
                            context.Current.NewWindow = context.Reader.ReadBool();
                            break;
                        case "notes":
                            context.Current.Notes = context.Reader.ReadString();
                            break;
                        case "name":
                            context.Current.Name = context.Reader.ReadString();
                            break;
                        case "overallTimeout":
                            context.Current.OverallTimeout = context.Reader.ReadInt();
                            break;
                        case "protocol":
                            string protocol = context.Reader.ReadString();
                            if (protocol.ToUpperInvariant() == "TELNET")
                            	context.Current.Protocol = "Terminal";
                            else if (protocol.ToUpperInvariant() == "SSH")
                            	context.Current.Protocol = "Ssh";
                            else if (protocol.ToUpperInvariant() == "ICA CITRIX")
                            	context.Current.Protocol = "Ica";
                            else
                            	context.Current.Protocol = protocol;
                            break;
                        case "port":
                            context.Current.Port = context.Reader.ReadInt();
                            break;
                        case "redirectedDrives":
                            context.Current.RedirectedDrives = context.Reader.ReadString();
                            break;
                        case "redirectPorts":
                            context.Current.RedirectPorts = context.Reader.ReadBool();
                            break;
                        case "redirectPrinters":
                            context.Current.RedirectPrinters = context.Reader.ReadBool();
                            break;
                        case "redirectSmartCards":
                            context.Current.RedirectSmartCards = context.Reader.ReadBool();
                            break;
                        case "redirectClipboard":
                            context.Current.RedirectClipboard = context.Reader.ReadBool();
                            break;
                        case "redirectDevices":
                            context.Current.RedirectDevices = context.Reader.ReadBool();
                            break;
                        case "sounds":
                            context.Current.Sounds = context.Reader.ReadRemoteSounds();
                            break;
                        case "serverName":
                            context.Current.ServerName = context.Reader.ReadString();
                            break;
                        case "shutdownTimeout":
                            context.Current.ShutdownTimeout = context.Reader.ReadInt();
                            break;
                        case "ssh1":
							context.Current.Ssh1 = context.Reader.ReadBool();
                            break;
                        case "securityFullScreen":
                            context.Current.SecurityFullScreen = context.Reader.ReadBool();
                            break;
                        case "securityStartProgram":
                            context.Current.SecurityStartProgram = context.Reader.ReadString();
                            break;
                        case "securityWorkingFolder":
                            context.Current.SecurityWorkingFolder = context.Reader.ReadString();
                            break;
                        case "tags":
                            context.Current.Tags = context.Reader.ReadString();
                            break;
                        case "telnetBackColor":
                            context.Current.ConsoleBackColor = context.Reader.ReadString();
                            break;
                        case "telnetCols":
                            context.Current.ConsoleCols = context.Reader.ReadInt();
                            break;
                        case "telnetCursorColor":
                            context.Current.ConsoleCursorColor = context.Reader.ReadString();
                            break;
                        case "telnetFont":
                            context.Current.ConsoleFont = context.Reader.ReadString();
                            break;
                        case "telnetRows":
                            context.Current.ConsoleRows = context.Reader.ReadInt();
                            break;
                        case "telnetTextColor":
                            context.Current.ConsoleTextColor = context.Reader.ReadString();
                            break;
                        case "toolBarIcon":
                            context.Current.ToolBarIcon = context.Reader.ReadString();
                            break;
                        case "tsgwCredsSource":
                            context.Current.TsgwCredsSource = context.Reader.ReadInt();
                            break;
                        case "tsgwDomain":
                            context.Current.TsgwDomain = context.Reader.ReadString();
                            break;
                        case "tsgwHostname":
                            context.Current.TsgwHostname = context.Reader.ReadString();
                            break;
                        case "tsgwPassword":
                            context.ReadTsgwPassword();
                            break;
                        case "tsgwSeparateLogin":
                            context.Current.TsgwSeparateLogin = context.Reader.ReadBool();
                            
                            if (context.Current.TsgwSeparateLogin)
                            	context.Current.TsgwXmlCredentialSetName = Terminals.Forms.Controls.CredentialPanel.Custom;
                            break;
                        case "tsgwUsageMethod":
                            context.Current.TsgwUsageMethod = context.Reader.ReadInt();
                            break;
                        case "tsgwUsername":
                            context.Current.TsgwUsername = context.Reader.ReadString();
                            break;
                        case "url":
                            context.Current.Url = context.Reader.ReadString();
                            break;
                        case "vncAutoScale":
                            context.Current.VncAutoScale = context.Reader.ReadBool();
                            break;
                        case "vncViewOnly":
                            context.Current.VncViewOnly = context.Reader.ReadBool();
                            break;
                        case "vncDisplayNumber":
                            context.Current.VncDisplayNumber = context.Reader.ReadInt();
                            break;
                        case "vmrcadministratormode":
							context.Current.VmrcAdministratorMode = context.Reader.ReadBool();
                            break;
                        case "vmrcreducedcolorsmode":
							context.Current.VmrcReducedColorsMode = context.Reader.ReadBool();
                            break;
                    }
                    break;
            }
        }

		private class ImportCodePlexTerminalsContext
		{
			public PropertyReader Reader { get; private set; }

			public List<FavoriteConfigurationElement> Favorites { get; private set; }

			/// <summary>
			/// because reading more than one property into the same favorite,
			/// keep the favorite out of the read property method.
			/// </summary>
			public FavoriteConfigurationElement Current { get; private set; }

			public ImportCodePlexTerminalsContext(PropertyReader reader)
			{
				this.Reader = reader;
				this.Favorites = new List<FavoriteConfigurationElement>();
			}

			internal void SetNewCurrent()
			{
				this.Current = new FavoriteConfigurationElement();
				this.Favorites.Add(this.Current);
			}

			public void ReadTsgwPassword()
			{
				this.Current.TsgwPassword = this.Reader.ReadString();
			}

			public void ReadPassword()
			{
				this.Current.Password = this.Reader.ReadString();
			}
		}

		private class PropertyReader
		{
			private readonly XmlTextReader innerReader;

			internal XmlNodeType NodeType { get { return this.innerReader.NodeType; } }

			internal string NodeName { get { return this.innerReader.Name; } }

			public PropertyReader(XmlTextReader innerReader)
			{
				this.innerReader = innerReader;
			}

			public bool Read()
			{
				return this.innerReader.Read();
			}

			public string ReadString()
			{
				return this.innerReader.ReadString().Trim();
			}

			internal bool ReadBool()
			{
				bool tmp = false;
				bool.TryParse(this.ReadString(), out tmp);
				return tmp;
			}

			internal int ReadInt()
			{
				int tmp = 0;
				int.TryParse(this.ReadString(), out tmp);
				return tmp;
			}

			internal DesktopSize ReadDesktopSize()
			{
				DesktopSize tmp = DesktopSize.AutoScale;
				string str = this.ReadString();
				
				try
				{
					if (!String.IsNullOrEmpty(str))
						tmp = (DesktopSize)Enum.Parse(typeof(DesktopSize), str);
				}
				catch
				{
					// if the value is something like 768x1024
					// The CodePlex Terminals holds the value 'x1024'
					// which is a custom value.
					// possible exception:
					// >> System.ArgumentException: Requested value 'x1024' was not found. <<
					return DesktopSize.Custom;
				}
				
				return tmp;
			}

			internal Colors ReadColors()
			{
				Colors tmp = Colors.Bit16;
				string str = this.ReadString();
				if (!String.IsNullOrEmpty(str))
					tmp = (Colors)Enum.Parse(typeof(Colors), str);
				return tmp;
			}

			internal RemoteSounds ReadRemoteSounds()
			{
				RemoteSounds tmp = RemoteSounds.DontPlay;
				string str = this.ReadString();
				if (!String.IsNullOrEmpty(str))
					tmp = (RemoteSounds)Enum.Parse(typeof(RemoteSounds), str);
				return tmp;
			}

			public override string ToString()
			{
				return this.ReadString();
			}
		}
    }
}