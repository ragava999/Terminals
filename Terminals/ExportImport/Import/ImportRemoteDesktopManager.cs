using System;
using System.Xml;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.Collections.Generic;

using Kohl.Framework.Logging;
using Terminals.Configuration.Files.Main.Favorites;

namespace Terminals.ExportImport.Import
{
	public class ImportRemoteDesktopManager : IImport
	{
		public const string FILE_EXTENSION = ".rdm";

		public static readonly string PROVIDER_NAME = "Remote Desktop Manager";

		List<FavoriteConfigurationElement> IImport.ImportFavorites(string fileName)
		{
			List <FavoriteConfigurationElement> favorites = new List<FavoriteConfigurationElement> ();

			using (FileStream fileStream = new FileStream (fileName, FileMode.Open))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(RemoteDesktopManager));
				RemoteDesktopManager result = (RemoteDesktopManager)serializer.Deserialize (fileStream);

				List<Terminals.Configuration.Files.Credentials.Credential> credentials = new List<Terminals.Configuration.Files.Credentials.Credential> ();

				foreach (RemoteDesktopManager.Connection connection in result.Connections)
				{
					if (connection.ConnectionType == "Credential")
					{
						if (connection.Credentials.CredentialType == "KeePass")
						{
							// Parse KeePass credentials and add the result to the list of credentials
							credentials.Add (new Terminals.Configuration.Files.Credentials.Credential());
						}
						else
						{
							// Parse "normal" credentials and add the result to the list of credentials
							credentials.Add (new Terminals.Configuration.Files.Credentials.Credential());
						}	
					}
				}

				foreach (RemoteDesktopManager.Connection connection in result.Connections)
				{
					FavoriteConfigurationElement favorite = new FavoriteConfigurationElement(connection.Name);
					favorite.Tags = connection.Group;

					// Ignore groups, credentials and data entries
					if (connection.ConnectionType == "Group" || connection.ConnectionType == "DataEntry" || connection.ConnectionType == "Credential")
						continue;

					if (connection.ConnectionType == "Document")
					{
						Log.Warn("Terminals is not capable of embeding Documents. A connection with a link to it has been generated and the document has been extracted from the RDM file");
						continue;
					}

					if (connection.ConnectionType == "Ftp")
					{
						Log.Warn("FTP connection will be skipped.");
						continue;
					}

					if (connection.ConnectionType == "WebBrowser")
					{
						if (connection.ConnectionSubType == "GoogleChrome" && connection.WebBrowserUrl.ToLowerInvariant ().StartsWith ("https://"))
						{
							favorite.Protocol = "HTTPS";
							favorite.HttpBrowser = BrowserType.Firefox;
							Log.Warn ("Converted RDM Google Chrome connection to FireFox");
						}
						else if (connection.ConnectionSubType == "GoogleChrome" && connection.WebBrowserUrl.ToLowerInvariant ().StartsWith ("http://"))
						{
							favorite.Protocol = "HTTP";
							favorite.HttpBrowser = BrowserType.Firefox;
							Log.Warn ("Converted RDM Google Chrome connection to FireFox");
						}
						if (connection.ConnectionSubType == "FireFox" && connection.WebBrowserUrl.ToLowerInvariant ().StartsWith ("https://"))
						{
							favorite.Protocol = "HTTPS";
							favorite.HttpBrowser = BrowserType.Firefox;
						}
						else if (connection.ConnectionSubType == "FireFox" && connection.WebBrowserUrl.ToLowerInvariant ().StartsWith ("http://"))
						{
							favorite.Protocol = "HTTP";
							favorite.HttpBrowser = BrowserType.Firefox;
						}
						else if (connection.ConnectionSubType == "IE" && connection.WebBrowserUrl.ToLowerInvariant ().StartsWith ("https://"))
						{
							favorite.Protocol = "HTTPS";
							favorite.HttpBrowser = BrowserType.InternetExplorer;
						}
						else if (connection.ConnectionSubType == "IE" && connection.WebBrowserUrl.ToLowerInvariant ().StartsWith ("http://"))
						{
							favorite.Protocol = "HTTP";
							favorite.HttpBrowser = BrowserType.InternetExplorer;
						}

						favorite.Url = connection.WebBrowserUrl;
					}
					else if (connection.ConnectionType == "CommandLine" || connection.ConnectionType == "SessionTool")
					{
						favorite.Protocol = "Generic";

						if (!string.IsNullOrEmpty(connection.CommandLine))
						{
							if (connection.CommandLine.Contains (" "))
							{
								favorite.GenericProgramPath = connection.CommandLine.Split (new string[] {" "}, StringSplitOptions.None) [0];
								favorite.GenericArguments = connection.CommandLine.Substring (favorite.GenericProgramPath.Length, connection.CommandLine.Length - favorite.GenericProgramPath.Length - 1);
							}
							else
								favorite.GenericProgramPath = connection.CommandLine;
						}
					}
					else if (connection.ConnectionType == "AddOn" && connection.AddOn.AddOnDescription == "File Explorer AddOn")
					{
						favorite.Protocol = "Explorer";
						string folder = connection.AddOn.Properties;

						if (folder.Contains ("&lt;Folder&gt;"))
						{
							int start = folder.IndexOf ("&lt;Folder&gt;");
							int end = folder.LastIndexOf ("&lt;Folder&gt;");

							folder = folder.Substring (start + 14, end);
							favorite.ExplorerDirectory = folder;
						}
					}
					else if (connection.ConnectionType == "RDPConfigured")
					{
						favorite.Protocol = "RDP";
					}
					else if (connection.ConnectionType == "ICA")
					{
						favorite.Protocol = "Ica";
					}
					else if (connection.ConnectionType == "Putty")
					{
						favorite.Protocol = "Putty";
					}
					else if (connection.ConnectionType == "Radmin")
					{
						favorite.Protocol = "RAdmin";
					}
					else
					{
						Log.Warn(string.Format("Skipping RDM connection '{0}' of type {1}.", connection.Name, connection.ConnectionType));
					}

					favorites.Add (favorite);
				}
			}

			return favorites;
		}

		public string Name
		{
			get { return PROVIDER_NAME; }
		}

		public string KnownExtension
		{
			get { return FILE_EXTENSION; }
		}
	}

	[XmlRoot(ElementName="ArrayOfConnection")]
	public class RemoteDesktopManager
	{
		[XmlElement("Connection")]
		public List<Connection> Connections { get; set; }

		[XmlRoot(ElementName="Connection")]
		public class Connection {
			[XmlElement(ElementName="GroupDetails")]
			public string GroupDetails { get; set; }
			[XmlElement(ElementName="ConnectionType")]
			public string ConnectionType { get; set; }
			[XmlElement(ElementName="Group")]
			public string Group { get; set; }
			[XmlElement(ElementName="ID")]
			public string ID { get; set; }
			[XmlElement(ElementName="Name")]
			public string Name { get; set; }
			[XmlElement(ElementName="OpenEmbedded")]
			public string OpenEmbedded { get; set; }
			[XmlElement(ElementName="Stamp")]
			public string Stamp { get; set; }
			[XmlElement(ElementName="DataEntry")]
			public DataEntry DataEntry { get; set; }
			[XmlElement(ElementName="Web")]
			public Web Web { get; set; }
			[XmlElement(ElementName="Children")]
			public Children Children { get; set; }
			[XmlElement(ElementName="ConnectionSubType")]
			public string ConnectionSubType { get; set; }
			[XmlElement(ElementName="PinEmbeddedMode")]
			public string PinEmbeddedMode { get; set; }
			[XmlElement(ElementName="WebBrowserApplication")]
			public string WebBrowserApplication { get; set; }
			[XmlElement(ElementName="WebBrowserUrl")]
			public string WebBrowserUrl { get; set; }
			[XmlElement(ElementName="CredentialConnectionID")]
			public string CredentialConnectionID { get; set; }
			[XmlElement(ElementName="Image")]
			public string Image { get; set; }
			[XmlElement(ElementName="ImageMD5")]
			public string ImageMD5 { get; set; }
			[XmlElement(ElementName="Cmd")]
			public Cmd Cmd { get; set; }
			[XmlElement(ElementName="CommandLine")]
			public string CommandLine { get; set; }
			[XmlElement(ElementName="CommandLineWorkingDirectory")]
			public string CommandLineWorkingDirectory { get; set; }
			[XmlElement(ElementName="AddOn")]
			public AddOn AddOn { get; set; }
			[XmlElement(ElementName="Events")]
			public Events Events { get; set; }
			[XmlElement(ElementName="MetaInformation")]
			public MetaInformation MetaInformation { get; set; }
			[XmlElement(ElementName="AllowPasswordVariable")]
			public string AllowPasswordVariable { get; set; }
			[XmlElement(ElementName="RDP")]
			public RDP RDP { get; set; }
			[XmlElement(ElementName="Url")]
			public string Url { get; set; }
			[XmlElement(ElementName="UsesSerialPorts")]
			public string UsesSerialPorts { get; set; }
			[XmlElement(ElementName="RunAsConnection")]
			public RunAsConnection RunAsConnection { get; set; }
			[XmlElement(ElementName="TemplateName")]
			public string TemplateName { get; set; }
			[XmlElement(ElementName="TemplateSourceID")]
			public string TemplateSourceID { get; set; }
			[XmlElement(ElementName="DesktopComposition")]
			public string DesktopComposition { get; set; }
			[XmlElement(ElementName="KeyboardHook")]
			public string KeyboardHook { get; set; }
			[XmlElement(ElementName="ScreenColor")]
			public string ScreenColor { get; set; }
			[XmlElement(ElementName="SoundHook")]
			public string SoundHook { get; set; }
			[XmlElement(ElementName="Citrix")]
			public Citrix Citrix { get; set; }
			[XmlElement(ElementName="Description")]
			public string Description { get; set; }
			[XmlElement(ElementName="Document")]
			public Document Document { get; set; }
			[XmlElement(ElementName="DescriptionMode")]
			public string DescriptionMode { get; set; }
			[XmlElement(ElementName="DescriptionRtf")]
			public string DescriptionRtf { get; set; }
			[XmlElement(ElementName="ImageName")]
			public string ImageName { get; set; }
			[XmlElement(ElementName="SmartSizing")]
			public string SmartSizing { get; set; }
			[XmlElement(ElementName="Console")]
			public string Console { get; set; }
			[XmlElement(ElementName="CommandLineWaitForApplicationToExit")]
			public string CommandLineWaitForApplicationToExit { get; set; }
			[XmlElement(ElementName="Ftp")]
			public Ftp Ftp { get; set; }
			[XmlElement(ElementName="SubMode")]
			public string SubMode { get; set; }
			[XmlElement(ElementName="Domain")]
			public string Domain { get; set; }
			[XmlElement(ElementName="UserName")]
			public string UserName { get; set; }
			[XmlElement(ElementName="DisableBitmapCache")]
			public string DisableBitmapCache { get; set; }
			[XmlElement(ElementName="Putty")]
			public Putty Putty { get; set; }
			[XmlElement(ElementName="AllowClipboard")]
			public string AllowClipboard { get; set; }
			[XmlElement(ElementName="Radmin")]
			public Radmin Radmin { get; set; }
			[XmlElement(ElementName="Encrypt")]
			public string Encrypt { get; set; }
			[XmlElement(ElementName="Tools")]
			public Tools Tools { get; set; }
			[XmlElement(ElementName="ShowInTrayIcon")]
			public string ShowInTrayIcon { get; set; }
			[XmlElement(ElementName="SortPriority")]
			public string SortPriority { get; set; }
			[XmlElement(ElementName="Credentials")]
			public Credentials Credentials { get; set; }
			[XmlElement(ElementName="Script")]
			public Script Script { get; set; }
			[XmlElement(ElementName="PowerShell")]
			public PowerShell PowerShell { get; set; }
			[XmlElement(ElementName="AllowViewPasswordAction")]
			public string AllowViewPasswordAction { get; set; }
		}

		[XmlRoot(ElementName="DataEntry")]
		public class DataEntry {
			[XmlElement(ElementName="AutomaticRefresh")]
			public string AutomaticRefresh { get; set; }
			[XmlElement(ElementName="ConnectionTypeInfos")]
			public ConnectionTypeInfos ConnectionTypeInfos { get; set; }
			[XmlElement(ElementName="EncryptedSecureNote")]
			public string EncryptedSecureNote { get; set; }
			[XmlElement(ElementName="SecurityQuestion1")]
			public string SecurityQuestion1 { get; set; }
		}

		[XmlRoot(ElementName="Web")]
		public class Web {
			[XmlElement(ElementName="IgnoreCertificateErrors")]
			public string IgnoreCertificateErrors { get; set; }
			[XmlElement(ElementName="ScriptErrorsSuppressed")]
			public string ScriptErrorsSuppressed { get; set; }
			[XmlElement(ElementName="ShowUrl")]
			public string ShowUrl { get; set; }
			[XmlElement(ElementName="ShowFavicon")]
			public string ShowFavicon { get; set; }
			[XmlElement(ElementName="EnableWebBrowserExtension")]
			public string EnableWebBrowserExtension { get; set; }
			[XmlElement(ElementName="PasswordControlId")]
			public string PasswordControlId { get; set; }
			[XmlElement(ElementName="UserNameControlId")]
			public string UserNameControlId { get; set; }
			[XmlElement(ElementName="TabPageMode")]
			public string TabPageMode { get; set; }
			[XmlElement(ElementName="SubmitControlId")]
			public string SubmitControlId { get; set; }
			[XmlElement(ElementName="AutoSubmit")]
			public string AutoSubmit { get; set; }
			[XmlElement(ElementName="FormId")]
			public string FormId { get; set; }
			[XmlElement(ElementName="AuthenticationMode")]
			public string AuthenticationMode { get; set; }
			[XmlElement(ElementName="UseBasicAuthentication")]
			public string UseBasicAuthentication { get; set; }
			[XmlElement(ElementName="AlwaysLaunchOnProtocolRequest")]
			public string AlwaysLaunchOnProtocolRequest { get; set; }
			[XmlElement(ElementName="EnableKeyboardShortcuts")]
			public string EnableKeyboardShortcuts { get; set; }
			[XmlElement(ElementName="Force32Bit")]
			public string Force32Bit { get; set; }
			[XmlElement(ElementName="DomainControlId")]
			public string DomainControlId { get; set; }
			[XmlElement(ElementName="SafePassword")]
			public string SafePassword { get; set; }
			[XmlElement(ElementName="UrlEncodeBasicAuthentication")]
			public string UrlEncodeBasicAuthentication { get; set; }
			[XmlElement(ElementName="UserName")]
			public string UserName { get; set; }
			[XmlElement(ElementName="WebBrowserSubApplication")]
			public string WebBrowserSubApplication { get; set; }
			[XmlElement(ElementName="Domain")]
			public string Domain { get; set; }
			[XmlElement(ElementName="AutoFillLogin")]
			public string AutoFillLogin { get; set; }
			[XmlElement(ElementName="LanguageCode")]
			public string LanguageCode { get; set; }
		}

		[XmlRoot(ElementName="Children")]
		public class Children {
			[XmlElement(ElementName="Connection")]
			public Connection Connection { get; set; }
		}

		[XmlRoot(ElementName="Cmd")]
		public class Cmd {
			[XmlElement(ElementName="UseDefaultWorkingDirectory")]
			public string UseDefaultWorkingDirectory { get; set; }
			[XmlElement(ElementName="Parameter1DataType")]
			public string Parameter1DataType { get; set; }
			[XmlElement(ElementName="Parameter1Label")]
			public string Parameter1Label { get; set; }
			[XmlElement(ElementName="Parameter2DataType")]
			public string Parameter2DataType { get; set; }
			[XmlElement(ElementName="Parameter2Label")]
			public string Parameter2Label { get; set; }
			[XmlElement(ElementName="SafeParam1Default")]
			public string SafeParam1Default { get; set; }
			[XmlElement(ElementName="SafeParam2Default")]
			public string SafeParam2Default { get; set; }
			[XmlElement(ElementName="EmbeddedWaitTime")]
			public string EmbeddedWaitTime { get; set; }
			[XmlElement(ElementName="Run64BitsMode")]
			public string Run64BitsMode { get; set; }
			[XmlElement(ElementName="RunAsAdministrator")]
			public string RunAsAdministrator { get; set; }
			[XmlElement(ElementName="ExecutionMode")]
			public string ExecutionMode { get; set; }
			[XmlElement(ElementName="UseShellExecute")]
			public string UseShellExecute { get; set; }
		}

		[XmlRoot(ElementName="AddOn")]
		public class AddOn {
			[XmlElement(ElementName="AddOnDescription")]
			public string AddOnDescription { get; set; }
			[XmlElement(ElementName="AddOnVersion")]
			public string AddOnVersion { get; set; }
			[XmlElement(ElementName="Properties")]
			public string Properties { get; set; }
		}

		[XmlRoot(ElementName="Events")]
		public class Events {
			[XmlElement(ElementName="BeforeConnectionEvent")]
			public string BeforeConnectionEvent { get; set; }
			[XmlElement(ElementName="BeforeConnectionPromptMessage")]
			public string BeforeConnectionPromptMessage { get; set; }
			[XmlElement(ElementName="AfterConnectionEvent")]
			public string AfterConnectionEvent { get; set; }
			[XmlElement(ElementName="AfterConnectionMacroScriptID")]
			public string AfterConnectionMacroScriptID { get; set; }
			[XmlElement(ElementName="AfterConnectionTypingMacro")]
			public string AfterConnectionTypingMacro { get; set; }
			[XmlElement(ElementName="AfterConnectionTypingMacroEnabled")]
			public string AfterConnectionTypingMacroEnabled { get; set; }
			[XmlElement(ElementName="AfterConnectionMode")]
			public string AfterConnectionMode { get; set; }
			[XmlElement(ElementName="AfterConnectionTypingMacroID")]
			public string AfterConnectionTypingMacroID { get; set; }
		}

		[XmlRoot(ElementName="PasswordHistory")]
		public class PasswordHistory {
			[XmlElement(ElementName="ModifiedBy")]
			public string ModifiedBy { get; set; }
			[XmlElement(ElementName="ModifiedDateTime")]
			public string ModifiedDateTime { get; set; }
			[XmlElement(ElementName="SafePassword")]
			public string SafePassword { get; set; }
			[XmlElement(ElementName="PasswordHistory")]
			public List<PasswordHistory> Passwordhistory { get; set; }
		}

		[XmlRoot(ElementName="MetaInformation")]
		public class MetaInformation {
			[XmlElement(ElementName="PasswordHistory")]
			public PasswordHistory PasswordHistory { get; set; }
			[XmlElement(ElementName="Keywords")]
			public string Keywords { get; set; }
			[XmlElement(ElementName="Notes")]
			public string Notes { get; set; }
			[XmlElement(ElementName="ServerRemoteManagementUrl")]
			public string ServerRemoteManagementUrl { get; set; }
		}

		[XmlRoot(ElementName="RDP")]
		public class RDP {
			[XmlElement(ElementName="NetworkLevelAuthentication")]
			public string NetworkLevelAuthentication { get; set; }
			[XmlElement(ElementName="RedirectDirectX")]
			public string RedirectDirectX { get; set; }
			[XmlElement(ElementName="SmartSizingStreched")]
			public string SmartSizingStreched { get; set; }
			[XmlElement(ElementName="VideoPlaybackMode")]
			public string VideoPlaybackMode { get; set; }
			[XmlElement(ElementName="GatewayProfileUsageMethod")]
			public string GatewayProfileUsageMethod { get; set; }
			[XmlElement(ElementName="GatewayUsageMethod")]
			public string GatewayUsageMethod { get; set; }
			[XmlElement(ElementName="RedirectedDrives")]
			public RedirectedDrives RedirectedDrives { get; set; }
			[XmlElement(ElementName="ScreenSizingMode")]
			public string ScreenSizingMode { get; set; }
			[XmlElement(ElementName="RDPLogOffMethod")]
			public string RDPLogOffMethod { get; set; }
			[XmlElement(ElementName="GatewayHostname")]
			public string GatewayHostname { get; set; }
			[XmlElement(ElementName="PromptCredentialOnce")]
			public string PromptCredentialOnce { get; set; }
			[XmlElement(ElementName="Domain")]
			public string Domain { get; set; }
			[XmlElement(ElementName="UserName")]
			public string UserName { get; set; }
			[XmlElement(ElementName="Compression")]
			public string Compression { get; set; }
			[XmlElement(ElementName="GatewayCredentialsSource")]
			public string GatewayCredentialsSource { get; set; }
			[XmlElement(ElementName="EnableCredSSPSupport")]
			public string EnableCredSSPSupport { get; set; }
			[XmlElement(ElementName="GatewayCredentialConnectionID")]
			public string GatewayCredentialConnectionID { get; set; }
			[XmlElement(ElementName="SafePassword")]
			public string SafePassword { get; set; }
		}

		[XmlRoot(ElementName="RunAsConnection")]
		public class RunAsConnection {
			[XmlElement(ElementName="CredentialSource")]
			public string CredentialSource { get; set; }
			[XmlElement(ElementName="CredentialConnectionID")]
			public string CredentialConnectionID { get; set; }
		}

		[XmlRoot(ElementName="RedirectedDrives")]
		public class RedirectedDrives {
			[XmlElement(ElementName="string")]
			public string String { get; set; }
		}

		[XmlRoot(ElementName="Citrix")]
		public class Citrix {
			[XmlElement(ElementName="PasswordControlId")]
			public string PasswordControlId { get; set; }
			[XmlElement(ElementName="SubmitControlId")]
			public string SubmitControlId { get; set; }
			[XmlElement(ElementName="UserNameControlId")]
			public string UserNameControlId { get; set; }
			[XmlElement(ElementName="ApplicationName")]
			public string ApplicationName { get; set; }
			[XmlElement(ElementName="CitrixType")]
			public string CitrixType { get; set; }
			[XmlElement(ElementName="DisableCtrlAltDel")]
			public string DisableCtrlAltDel { get; set; }
			[XmlElement(ElementName="Host")]
			public string Host { get; set; }
			[XmlElement(ElementName="UsePasscode")]
			public string UsePasscode { get; set; }
			[XmlElement(ElementName="EncryptionStrength")]
			public string EncryptionStrength { get; set; }
			[XmlElement(ElementName="Port")]
			public string Port { get; set; }
		}

		[XmlRoot(ElementName="Document")]
		public class Document {
			[XmlElement(ElementName="DocumentDataMode")]
			public string DocumentDataMode { get; set; }
			[XmlElement(ElementName="DocumentType")]
			public string DocumentType { get; set; }
			[XmlElement(ElementName="Filename")]
			public string Filename { get; set; }
			[XmlElement(ElementName="PDFViewer")]
			public string PDFViewer { get; set; }
			[XmlElement(ElementName="Size")]
			public string Size { get; set; }
			[XmlElement(ElementName="EmbeddedDataID")]
			public string EmbeddedDataID { get; set; }
		}

		[XmlRoot(ElementName="Ftp")]
		public class Ftp {
			[XmlElement(ElementName="Host")]
			public string Host { get; set; }
			[XmlElement(ElementName="MaxDownloadSpeed")]
			public string MaxDownloadSpeed { get; set; }
			[XmlElement(ElementName="MaxUploadSpeed")]
			public string MaxUploadSpeed { get; set; }
			[XmlElement(ElementName="SSL")]
			public string SSL { get; set; }
			[XmlElement(ElementName="SafePassword")]
			public string SafePassword { get; set; }
			[XmlElement(ElementName="TLS")]
			public string TLS { get; set; }
			[XmlElement(ElementName="UserName")]
			public string UserName { get; set; }
		}

		[XmlRoot(ElementName="Putty")]
		public class Putty {
			[XmlElement(ElementName="Host")]
			public string Host { get; set; }
			[XmlElement(ElementName="Port")]
			public string Port { get; set; }
			[XmlElement(ElementName="PortFowardingArray")]
			public string PortFowardingArray { get; set; }
			[XmlElement(ElementName="ProxyCredentialConnectionID")]
			public string ProxyCredentialConnectionID { get; set; }
			[XmlElement(ElementName="ProxyPort")]
			public string ProxyPort { get; set; }
			[XmlElement(ElementName="ProxyType")]
			public string ProxyType { get; set; }
			[XmlElement(ElementName="ProxyUrl")]
			public string ProxyUrl { get; set; }
			[XmlElement(ElementName="RecordingFileName")]
			public string RecordingFileName { get; set; }
			[XmlElement(ElementName="RecordingMode")]
			public string RecordingMode { get; set; }
			[XmlElement(ElementName="UseProxy")]
			public string UseProxy { get; set; }
			[XmlElement(ElementName="SSHGatewayCredentialConnectionID")]
			public string SSHGatewayCredentialConnectionID { get; set; }
			[XmlElement(ElementName="SSHGatewayCredentialSource")]
			public string SSHGatewayCredentialSource { get; set; }
			[XmlElement(ElementName="FullScreen")]
			public string FullScreen { get; set; }
			[XmlElement(ElementName="Other")]
			public string Other { get; set; }
			[XmlElement(ElementName="Scripting")]
			public string Scripting { get; set; }
			[XmlElement(ElementName="SessionData")]
			public string SessionData { get; set; }
			[XmlElement(ElementName="SessionHost")]
			public string SessionHost { get; set; }
			[XmlElement(ElementName="SessionName")]
			public string SessionName { get; set; }
			[XmlElement(ElementName="UseEmbeddedSession")]
			public string UseEmbeddedSession { get; set; }
			[XmlElement(ElementName="UseSession")]
			public string UseSession { get; set; }
		}

		[XmlRoot(ElementName="Radmin")]
		public class Radmin {
			[XmlElement(ElementName="Color")]
			public string Color { get; set; }
			[XmlElement(ElementName="Fullscreen")]
			public string Fullscreen { get; set; }
			[XmlElement(ElementName="Host")]
			public string Host { get; set; }
			[XmlElement(ElementName="Mode")]
			public string Mode { get; set; }
			[XmlElement(ElementName="PhonebookFileName")]
			public string PhonebookFileName { get; set; }
			[XmlElement(ElementName="Through")]
			public string Through { get; set; }
			[XmlElement(ElementName="UsePhoneBookFile")]
			public string UsePhoneBookFile { get; set; }
		}

		[XmlRoot(ElementName="DataEntryConnectionTypeInfo")]
		public class DataEntryConnectionTypeInfo {
			[XmlElement(ElementName="DataEntryConnectionType")]
			public string DataEntryConnectionType { get; set; }
		}

		[XmlRoot(ElementName="ConnectionTypeInfos")]
		public class ConnectionTypeInfos {
			[XmlElement(ElementName="DataEntryConnectionTypeInfo")]
			public DataEntryConnectionTypeInfo DataEntryConnectionTypeInfo { get; set; }
		}

		[XmlRoot(ElementName="Tools")]
		public class Tools {
			[XmlElement(ElementName="AllowBatchExecute")]
			public string AllowBatchExecute { get; set; }
			[XmlElement(ElementName="ForceRunAsAdministratorIfSupported")]
			public string ForceRunAsAdministratorIfSupported { get; set; }
			[XmlElement(ElementName="ConnectionType")]
			public string ConnectionType { get; set; }
		}

		[XmlRoot(ElementName="Credentials")]
		public class Credentials {
			[XmlElement(ElementName="CredentialType")]
			public string CredentialType { get; set; }
			[XmlElement(ElementName="KeepassName")]
			public string KeepassName { get; set; }
			[XmlElement(ElementName="KeepassUuid")]
			public string KeepassUuid { get; set; }
			[XmlElement(ElementName="PassPortalUserID")]
			public string PassPortalUserID { get; set; }
		}

		[XmlRoot(ElementName="Script")]
		public class Script {
			[XmlElement(ElementName="Arguments")]
			public string Arguments { get; set; }
			[XmlElement(ElementName="EmbeddedScriptCompressed")]
			public string EmbeddedScriptCompressed { get; set; }
			[XmlElement(ElementName="IsEmbeddedScript")]
			public string IsEmbeddedScript { get; set; }
		}

		[XmlRoot(ElementName="PowerShell")]
		public class PowerShell {
			[XmlElement(ElementName="Command")]
			public string Command { get; set; }
			[XmlElement(ElementName="RunAsAdministrator")]
			public string RunAsAdministrator { get; set; }
		}

		[XmlRoot(ElementName="ArrayOfConnection")]
		public class ArrayOfConnection {
			[XmlElement(ElementName="Connection")]
			public List<Connection> Connection { get; set; }
		}
	}
}