USE [Terminals]
GO
/****** Object:  ForeignKey [FK_ConnectionParams_Connections]    Script Date: 10/17/2013 16:51:46 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ConnectionParams_Connections]') AND parent_object_id = OBJECT_ID(N'[dbo].[ConnectionParams]'))
ALTER TABLE [dbo].[ConnectionParams] DROP CONSTRAINT [FK_ConnectionParams_Connections]
GO
/****** Object:  ForeignKey [FK_FavoritesInGroup_Favorites]    Script Date: 10/17/2013 16:51:47 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FavoritesInGroup_Favorites]') AND parent_object_id = OBJECT_ID(N'[dbo].[ConnectionsInGroup]'))
ALTER TABLE [dbo].[ConnectionsInGroup] DROP CONSTRAINT [FK_FavoritesInGroup_Favorites]
GO
/****** Object:  ForeignKey [FK_FavoritesInGroup_Groups]    Script Date: 10/17/2013 16:51:47 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FavoritesInGroup_Groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[ConnectionsInGroup]'))
ALTER TABLE [dbo].[ConnectionsInGroup] DROP CONSTRAINT [FK_FavoritesInGroup_Groups]
GO
/****** Object:  ForeignKey [FK_Lookup_Credentials]    Script Date: 10/17/2013 16:51:47 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Lookup_Credentials]') AND parent_object_id = OBJECT_ID(N'[dbo].[CredentialLookup]'))
ALTER TABLE [dbo].[CredentialLookup] DROP CONSTRAINT [FK_Lookup_Credentials]
GO
/****** Object:  ForeignKey [FK_Lookup_Favorites]    Script Date: 10/17/2013 16:51:47 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Lookup_Favorites]') AND parent_object_id = OBJECT_ID(N'[dbo].[CredentialLookup]'))
ALTER TABLE [dbo].[CredentialLookup] DROP CONSTRAINT [FK_Lookup_Favorites]
GO
/****** Object:  ForeignKey [FK_Lookup_User]    Script Date: 10/17/2013 16:51:47 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Lookup_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[CredentialLookup]'))
ALTER TABLE [dbo].[CredentialLookup] DROP CONSTRAINT [FK_Lookup_User]
GO
/****** Object:  ForeignKey [FK_History_Favorites]    Script Date: 10/17/2013 16:51:47 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_History_Favorites]') AND parent_object_id = OBJECT_ID(N'[dbo].[History]'))
ALTER TABLE [dbo].[History] DROP CONSTRAINT [FK_History_Favorites]
GO
/****** Object:  ForeignKey [FK_History_User]    Script Date: 10/17/2013 16:51:47 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_History_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[History]'))
ALTER TABLE [dbo].[History] DROP CONSTRAINT [FK_History_User]
GO
/****** Object:  Table [dbo].[ConnectionParams]    Script Date: 10/17/2013 16:51:46 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ConnectionParams_Connections]') AND parent_object_id = OBJECT_ID(N'[dbo].[ConnectionParams]'))
ALTER TABLE [dbo].[ConnectionParams] DROP CONSTRAINT [FK_ConnectionParams_Connections]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConnectionParams]') AND type in (N'U'))
DROP TABLE [dbo].[ConnectionParams]
GO
/****** Object:  Table [dbo].[ConnectionsInGroup]    Script Date: 10/17/2013 16:51:47 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FavoritesInGroup_Favorites]') AND parent_object_id = OBJECT_ID(N'[dbo].[ConnectionsInGroup]'))
ALTER TABLE [dbo].[ConnectionsInGroup] DROP CONSTRAINT [FK_FavoritesInGroup_Favorites]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FavoritesInGroup_Groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[ConnectionsInGroup]'))
ALTER TABLE [dbo].[ConnectionsInGroup] DROP CONSTRAINT [FK_FavoritesInGroup_Groups]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConnectionsInGroup]') AND type in (N'U'))
DROP TABLE [dbo].[ConnectionsInGroup]
GO
/****** Object:  Table [dbo].[CredentialLookup]    Script Date: 10/17/2013 16:51:47 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Lookup_Credentials]') AND parent_object_id = OBJECT_ID(N'[dbo].[CredentialLookup]'))
ALTER TABLE [dbo].[CredentialLookup] DROP CONSTRAINT [FK_Lookup_Credentials]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Lookup_Favorites]') AND parent_object_id = OBJECT_ID(N'[dbo].[CredentialLookup]'))
ALTER TABLE [dbo].[CredentialLookup] DROP CONSTRAINT [FK_Lookup_Favorites]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Lookup_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[CredentialLookup]'))
ALTER TABLE [dbo].[CredentialLookup] DROP CONSTRAINT [FK_Lookup_User]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CredentialLookup]') AND type in (N'U'))
DROP TABLE [dbo].[CredentialLookup]
GO
/****** Object:  Table [dbo].[History]    Script Date: 10/17/2013 16:51:47 ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_History_Favorites]') AND parent_object_id = OBJECT_ID(N'[dbo].[History]'))
ALTER TABLE [dbo].[History] DROP CONSTRAINT [FK_History_Favorites]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_History_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[History]'))
ALTER TABLE [dbo].[History] DROP CONSTRAINT [FK_History_User]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[History]') AND type in (N'U'))
DROP TABLE [dbo].[History]
GO
/****** Object:  Table [dbo].[Options]    Script Date: 10/17/2013 16:51:48 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Options]') AND type in (N'U'))
DROP TABLE [dbo].[Options]
GO
/****** Object:  Table [dbo].[User]    Script Date: 10/17/2013 16:51:48 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User]') AND type in (N'U'))
DROP TABLE [dbo].[User]
GO
/****** Object:  Table [dbo].[Credentials]    Script Date: 10/17/2013 16:51:47 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Credentials]') AND type in (N'U'))
DROP TABLE [dbo].[Credentials]
GO
/****** Object:  Table [dbo].[Groups]    Script Date: 10/17/2013 16:51:47 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Groups]') AND type in (N'U'))
DROP TABLE [dbo].[Groups]
GO
/****** Object:  Table [dbo].[Connections]    Script Date: 10/17/2013 16:51:46 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Connections]') AND type in (N'U'))
DROP TABLE [dbo].[Connections]
GO
/****** Object:  Table [dbo].[Connections]    Script Date: 10/17/2013 16:51:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Connections]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Connections](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Protocol] [nvarchar](10) NOT NULL,
	[Notes] [nvarchar](500) NULL,
	[ToolBarIcon] [varbinary](max) NULL,
	[ServerName] [nvarchar](max) NULL,
	[NewWindow] [bit] NULL,
	[Port] [int] NULL,
	[DesktopSizeHeight] [int] NULL,
	[DesktopSizeWidth] [int] NULL,
	[DesktopSize] [int] NULL,
	[ExecuteBeforeConnect] [bit] NULL,
	[ExecuteBeforeConnectCommand] [nvarchar](max) NULL,
	[ExecuteBeforeConnectArgs] [nvarchar](max) NULL,
	[ExecuteBeforeConnectInitialDirectory] [nvarchar](max) NULL,
	[ExecuteBeforeConnectWaitForExit] [bit] NULL,
	[ExplorerStyle] [nvarchar](max) NULL,
	[ExplorerDirectory] [nvarchar](max) NULL,
	[ExplorerDirectoryDual] [nvarchar](max) NULL,
	[ExplorerDirectoryTripple] [nvarchar](max) NULL,
	[ExplorerDirectoryQuad] [nvarchar](max) NULL,
	[HtmlFormFieldsString] [nvarchar](max) NULL,
	[BrowserAuthentication] [int] NULL,
	[HttpBrowser] [int] NULL,
	[Url] [nvarchar](max) NULL,
	[PuttyConnectionType] [int] NULL,
	[PuttySession] [nvarchar](max) NULL,
	[PuttyCompression] [bit] NULL,
	[PuttyPasswordTimeout] [int] NULL,
	[PuttyVerbose] [bit] NULL,
	[PuttyShowOptions] [bit] NULL,
	[PuttyCloseWindowOnExit] [int] NULL,
	[GenericWorkingDirectory] [nvarchar](max) NULL,
	[GenericProgramPath] [nvarchar](max) NULL,
	[GenericArguments] [nvarchar](max) NULL,
	[RAdminPhonebookPath] [nvarchar](max) NULL,
	[RAdminThrough] [bit] NULL,
	[RAdminThroughServerName] [nvarchar](max) NULL,
	[RAdminThroughPort] [nvarchar](max) NULL,
	[RAdminStandardConnectionMode] [bit] NULL,
	[RAdminTelnetMode] [bit] NULL,
	[RAdminViewOnlyMode] [bit] NULL,
	[RAdminFileTransferMode] [bit] NULL,
	[RAdminShutdown] [bit] NULL,
	[RAdminChatMode] [bit] NULL,
	[RAdminVoiceChatMode] [bit] NULL,
	[RAdminSendTextMessageMode] [bit] NULL,
	[RAdminUseFullScreen] [bit] NULL,
	[RAdminUpdates] [int] NULL,
	[RAdminColorMode] [nvarchar](max) NULL,
	[ConsoleRows] [int] NULL,
	[ConsoleCols] [int] NULL,
	[ConsoleFont] [nvarchar](max) NULL,
	[ConsoleBackColor] [nvarchar](max) NULL,
	[ConsoleTextColor] [nvarchar](max) NULL,
	[ConsoleCursorColor] [nvarchar](max) NULL,
	[Ssh1] [bit] NULL,
	[AuthMethod] [int] NULL,
	[KeyTag] [nvarchar](max) NULL,
	[VncAutoScale] [bit] NULL,
	[VncViewOnly] [bit] NULL,
	[VncDisplayNumber] [int] NULL,
	[VmrcReducedColorsMode] [bit] NULL,
	[VmrcAdministratorMode] [bit] NULL,
	[IcaApplicationName] [nvarchar](max) NULL,
	[IcaServerIni] [nvarchar](max) NULL,
	[IcaClientIni] [nvarchar](max) NULL,
	[IcaEnableEncryption] [bit] NULL,
	[IcaEncryptionLevel] [nvarchar](max) NULL,
	[LoadBalanceInfo] [nvarchar](max) NULL,
	[ConnectToConsole] [bit] NULL,
	[RedirectPrinters] [bit] NULL,
	[RedirectSmartCards] [bit] NULL,
	[RedirectClipboard] [bit] NULL,
	[RedirectDevices] [bit] NULL,
	[TsgwUsageMethod] [int] NULL,
	[TsgwHostname] [nvarchar](max) NULL,
	[TsgwCredsSource] [int] NULL,
	[TsgwSeparateLogin] [bit] NULL,
	[TsgwUsername] [nvarchar](max) NULL,
	[TsgwDomain] [nvarchar](max) NULL,
	[TsgwEncryptedPassword] [nvarchar](max) NULL,
	[Sounds] [int] NULL,
	[RedirectedDrives] [nvarchar](max) NULL,
	[RedirectPorts] [bit] NULL,
	[ShutdownTimeout] [int] NULL,
	[OverallTimeout] [int] NULL,
	[ConnectionTimeout] [int] NULL,
	[IdleTimeout] [int] NULL,
	[SecurityWorkingFolder] [nvarchar](max) NULL,
	[SecurityStartProgram] [nvarchar](max) NULL,
	[SecurityFullScreen] [bit] NULL,
	[EnableSecuritySettings] [bit] NULL,
	[GrabFocusOnConnect] [bit] NULL,
	[EnableEncryption] [bit] NULL,
	[DisableWindowsKey] [bit] NULL,
	[DoubleClickDetect] [bit] NULL,
	[DisplayConnectionBar] [bit] NULL,
	[DisableControlAltDelete] [bit] NULL,
	[AcceleratorPassthrough] [bit] NULL,
	[EnableCompression] [bit] NULL,
	[BitmapPeristence] [bit] NULL,
	[EnableTlsAuthentication] [bit] NULL,
	[EnableNlaAuthentication] [bit] NULL,
	[AllowBackgroundInput] [bit] NULL,
	[DisableTheming] [bit] NULL,
	[DisableMenuAnimations] [bit] NULL,
	[DisableFullWindowDrag] [bit] NULL,
	[DisableCursorBlinking] [bit] NULL,
	[EnableDesktopComposition] [bit] NULL,
	[EnableFontSmoothing] [bit] NULL,
	[DisableCursorShadow] [bit] NULL,
	[DisableWallPaper] [bit] NULL,
	[Colors] [int] NULL,
	[DesktopShare] [nvarchar](max) NULL,
 CONSTRAINT [PK_Favorites] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_PADDING OFF
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'Connections', N'COLUMN',N'Id'))
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Only to simplyfy relations, otherwise redundant because of Guid' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Connections', @level2type=N'COLUMN',@level2name=N'Id'
GO
/****** Object:  Table [dbo].[Groups]    Script Date: 10/17/2013 16:51:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Groups]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Groups](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_Groups] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Credentials]    Script Date: 10/17/2013 16:51:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Credentials]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Credentials](
	[Id] [int] NOT NULL,
	[UserName] [nchar](10) NULL,
	[DomainName] [nchar](10) NULL,
	[Password] [nchar](10) NULL,
	[Name] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_Credentials_1] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[User]    Script Date: 10/17/2013 16:51:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[User](
	[Id] [int] NOT NULL,
	[User] [nvarchar](50) NOT NULL,
	[Domain] [nvarchar](50) NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[Options]    Script Date: 10/17/2013 16:51:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Options]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Options](
	[PropertyName] [nvarchar](20) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_Options] PRIMARY KEY CLUSTERED 
(
	[PropertyName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[History]    Script Date: 10/17/2013 16:51:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[History]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[History](
	[FavoriteId] [int] NOT NULL,
	[LastUsedDate] [datetime] NOT NULL,
	[UsageCount] [nchar](10) NULL,
	[UserId] [int] NOT NULL,
 CONSTRAINT [PK_History_1] PRIMARY KEY CLUSTERED 
(
	[FavoriteId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[CredentialLookup]    Script Date: 10/17/2013 16:51:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CredentialLookup]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CredentialLookup](
	[ConnectionId] [int] NOT NULL,
	[UserId] [int] NULL,
	[CredentialId] [int] NOT NULL,
 CONSTRAINT [PK_Lookup] PRIMARY KEY CLUSTERED 
(
	[ConnectionId] ASC,
	[CredentialId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ConnectionsInGroup]    Script Date: 10/17/2013 16:51:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConnectionsInGroup]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ConnectionsInGroup](
	[ConnectionId] [int] NOT NULL,
	[GroupId] [int] NOT NULL,
 CONSTRAINT [PK_FavoritesInGroup] PRIMARY KEY CLUSTERED 
(
	[ConnectionId] ASC,
	[GroupId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  Table [dbo].[ConnectionParams]    Script Date: 10/17/2013 16:51:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ConnectionParams]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ConnectionParams](
	[Id] [int] NOT NULL,
	[ParamName] [nvarchar](50) NOT NULL,
	[ParamValue] [nvarchar](max) NULL,
	[ConnectionId] [int] NOT NULL,
 CONSTRAINT [PK_ConnectionParams] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
/****** Object:  ForeignKey [FK_ConnectionParams_Connections]    Script Date: 10/17/2013 16:51:46 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ConnectionParams_Connections]') AND parent_object_id = OBJECT_ID(N'[dbo].[ConnectionParams]'))
ALTER TABLE [dbo].[ConnectionParams]  WITH CHECK ADD  CONSTRAINT [FK_ConnectionParams_Connections] FOREIGN KEY([ConnectionId])
REFERENCES [dbo].[Connections] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ConnectionParams_Connections]') AND parent_object_id = OBJECT_ID(N'[dbo].[ConnectionParams]'))
ALTER TABLE [dbo].[ConnectionParams] CHECK CONSTRAINT [FK_ConnectionParams_Connections]
GO
/****** Object:  ForeignKey [FK_FavoritesInGroup_Favorites]    Script Date: 10/17/2013 16:51:47 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FavoritesInGroup_Favorites]') AND parent_object_id = OBJECT_ID(N'[dbo].[ConnectionsInGroup]'))
ALTER TABLE [dbo].[ConnectionsInGroup]  WITH CHECK ADD  CONSTRAINT [FK_FavoritesInGroup_Favorites] FOREIGN KEY([ConnectionId])
REFERENCES [dbo].[Connections] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FavoritesInGroup_Favorites]') AND parent_object_id = OBJECT_ID(N'[dbo].[ConnectionsInGroup]'))
ALTER TABLE [dbo].[ConnectionsInGroup] CHECK CONSTRAINT [FK_FavoritesInGroup_Favorites]
GO
/****** Object:  ForeignKey [FK_FavoritesInGroup_Groups]    Script Date: 10/17/2013 16:51:47 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FavoritesInGroup_Groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[ConnectionsInGroup]'))
ALTER TABLE [dbo].[ConnectionsInGroup]  WITH CHECK ADD  CONSTRAINT [FK_FavoritesInGroup_Groups] FOREIGN KEY([GroupId])
REFERENCES [dbo].[Groups] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_FavoritesInGroup_Groups]') AND parent_object_id = OBJECT_ID(N'[dbo].[ConnectionsInGroup]'))
ALTER TABLE [dbo].[ConnectionsInGroup] CHECK CONSTRAINT [FK_FavoritesInGroup_Groups]
GO
/****** Object:  ForeignKey [FK_Lookup_Credentials]    Script Date: 10/17/2013 16:51:47 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Lookup_Credentials]') AND parent_object_id = OBJECT_ID(N'[dbo].[CredentialLookup]'))
ALTER TABLE [dbo].[CredentialLookup]  WITH CHECK ADD  CONSTRAINT [FK_Lookup_Credentials] FOREIGN KEY([CredentialId])
REFERENCES [dbo].[Credentials] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Lookup_Credentials]') AND parent_object_id = OBJECT_ID(N'[dbo].[CredentialLookup]'))
ALTER TABLE [dbo].[CredentialLookup] CHECK CONSTRAINT [FK_Lookup_Credentials]
GO
/****** Object:  ForeignKey [FK_Lookup_Favorites]    Script Date: 10/17/2013 16:51:47 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Lookup_Favorites]') AND parent_object_id = OBJECT_ID(N'[dbo].[CredentialLookup]'))
ALTER TABLE [dbo].[CredentialLookup]  WITH CHECK ADD  CONSTRAINT [FK_Lookup_Favorites] FOREIGN KEY([ConnectionId])
REFERENCES [dbo].[Connections] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Lookup_Favorites]') AND parent_object_id = OBJECT_ID(N'[dbo].[CredentialLookup]'))
ALTER TABLE [dbo].[CredentialLookup] CHECK CONSTRAINT [FK_Lookup_Favorites]
GO
/****** Object:  ForeignKey [FK_Lookup_User]    Script Date: 10/17/2013 16:51:47 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Lookup_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[CredentialLookup]'))
ALTER TABLE [dbo].[CredentialLookup]  WITH CHECK ADD  CONSTRAINT [FK_Lookup_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Lookup_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[CredentialLookup]'))
ALTER TABLE [dbo].[CredentialLookup] CHECK CONSTRAINT [FK_Lookup_User]
GO
/****** Object:  ForeignKey [FK_History_Favorites]    Script Date: 10/17/2013 16:51:47 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_History_Favorites]') AND parent_object_id = OBJECT_ID(N'[dbo].[History]'))
ALTER TABLE [dbo].[History]  WITH CHECK ADD  CONSTRAINT [FK_History_Favorites] FOREIGN KEY([FavoriteId])
REFERENCES [dbo].[Connections] ([Id])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_History_Favorites]') AND parent_object_id = OBJECT_ID(N'[dbo].[History]'))
ALTER TABLE [dbo].[History] CHECK CONSTRAINT [FK_History_Favorites]
GO
/****** Object:  ForeignKey [FK_History_User]    Script Date: 10/17/2013 16:51:47 ******/
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_History_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[History]'))
ALTER TABLE [dbo].[History]  WITH CHECK ADD  CONSTRAINT [FK_History_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_History_User]') AND parent_object_id = OBJECT_ID(N'[dbo].[History]'))
ALTER TABLE [dbo].[History] CHECK CONSTRAINT [FK_History_User]
GO
