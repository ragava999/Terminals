# Terminals
Terminals is a multi-tab terminal services and remote desktop client. Moreover Terminals is capable beside RDP of supporting AutoIt, a shell explorer, http connections, ICA/Citrix connections, telnet, ssh, VNC and many other protocols.

Download the latest release:
* [as a ZIP file](https://github.com/OliverKohlDSc/Terminals/releases/download/4.9.0.0/Terminals_4.9.0.0.zip) or
* in form of an [installer/setup](https://github.com/OliverKohlDSc/Terminals/releases/download/4.9.0.0/Setup_4.9.0.0.exe)

The [official website](http://oliverkohldsc.github.io/Terminals) can be found [here](http://oliverkohldsc.github.io/Terminals).

[My personal website](http://www.kohl.bz)


###Release v 4.9.0.0

Date: 2016-02-08

The Terminal Services API code has been improved again.

Improved the stability of the autoit editor (scintilla)

Fixed a bug where the credential synchronizer tried to synchronize a missing credentials.xml file if KeePass credentials are in use.

Improved logging for the favorites tool tip in the favslist for unresolvable dns names. The server name will now be logged too.

The flag 'DontLoadMe' has been introduced in the FavoritePanel class to prevent loading broken favorite panels in the favorites editor.

The AutoItFavoritePanel class is now capable to react on errors in the underlying code e.g. unmanaged code and reports it now to the logger.

The Framework DLL has been excluded from the plugin directories - one, located under the root of Terminals, is fully enough

A typo has been fixed in the CredentialStoreOptionPanel.Designer.cs file

Changed the default button for a server reboot or shutdown in the favorites list to "No" instead of "Yes".

Added browse button in the RAdmin options

Added browse button in the putty options

Fixed a bug that occured in the screen capture option's folder selection menu

The locking model of log4net has been changed to minimal locking - which enables one to truncate the file or just delete it.

A new menu entry in Terminals main window has been added to truncate the current log file.

Reimplemented the Terminals release update mechanism.


###Release v 4.8.0.0

Date: 2016-02-01

Fixed a TSGW bug.

Improved the handling of RDP session enumeration and added the possibility to impersonate


###Release v 4.7.5.0

Date: 2016-01-04

Fixed a bug that occured after loading the FavoriteEditor

Added some additional debug logging the traditional way (without the use of AOP or any lib like postsharp)

Updated log4net and the log wrapper


###Release v 4.7.4.0

Date: 2015-11-30

Integrated the following NuGet packages:
* KeePassLib
* log4net
* Microsoft.WindowsAPICodePack
* Microsoft.WindowsAPICodePack.Core
* Microsoft.WindowsAPICodePack.Shell

Configuration files are now portable, if Terminals has write access to the local application directory it will put and read the files from there, otherwise if Terminals is for example located in "C:\Program Files (x86)\" or in "C:\Prgram Files\" it wil put the files here: ${LOCALAPPDATA}\Oliver Kohl D.Sc.\Terminals\xxx  

Now the user can actively choose if he or she desires to use either the Credentials.xml file or a KeePass database as the default credential store.

Removed obsolete localization which went back to the time of .NET 1.0.

Reduced the logging complexity

Added output to .gitignore

Removed obsolete files not any longer required

KeePass:
* Your password to your keepass store is now encrypted.
* The domain will be extracted from either the UserName field in KeePass (if it has the form of DOMAIN\USERNAME) or from and advanced filed called Domain.


###Release v 4.7.3.0

Date: 2015-05-21

It's now possible to connect to a KeePass 2.0 DB instead of using the Credentials.xml by filling in both the path & the password in the Terminals Options.

Fixed a memory leak in the StoredCredentials -> thread safe singelton had problems.

Fixed an error that occured after detaching a window from terminals.

Updated ToDO and FixMe comments to match the following style:
// TODO: KOHL> xyz
// FIXME: KOHL> xyz

Removed all obsolete code comments.

TODO: KOHL> Remove localization and replace with state of the art model.

TODO: KOHL> Decrypt/Hide password for keepass.


##Release v 4.7.0.0

Date: 2015-01-26

Removed AWSSDK (Amazon AWS / Amazon S3)

Removed Flickr

Updated XUL from version 16 to version 29

Updated Gecko engine to version v 29

Removed obfuscation

Removed license check and activation framework etc.

Deleted obsolete binaries + removed obsolete installer

Added binaries to _Version directory.

Some minor changes, some code refactoring, updated assembly version numbers and copyright dates

Simplified build process - much fewer additional tasks after the compilation phase

Added compilation switch to remove the XUL code and to remove the dependencies to Gecko.

The next release will be a bug fixing release.


##Release v 4.6.0.1

Date: 2014-10-13

Added new favorite properties to the PuttyConnection. X11 forwarding is now supported. It is now possible to ignore the domain part.

It is now possible to abort the putty dialog before the connection is available.


##Release v 4.6.0.0

Date: 2014-06-02

The "MiniBrowser" control is now capable of showing the actual current URL.

Fixed some additional minor bugs in the MiniBrowser control.

Some RDP connections report a problem (-2). But in fact the error is not an error per se it's just the way the authentication mechanism works for some machines. -> Log level for -2 errors has been changed to "debug" level.

Upgraded to .NET 4.0 -> Huge code changes have been necassary

Reconfigured some projects to operate on "any CPU" - not just x86.

ZedGraph had a problem with the localization when being build with SharpDevelop.

Extended Putty connection to support proxy settings
The new keywords for putty are not supported for Terminals 4.5.0.3 or below:
```
puttyProxyType="SOCKS5"
puttyProxyPort="80"
puttyProxyHost="myproxy"
```

Plugin settings and option will now only be written to configuration file if the value differs from the default value.
Settings or favorites like
```
<plugin name="TEXT_ShowTinyMceInEditMode" value="False" defaultValue="False" />
```
make no sense.
Instead the config will look like:
```
<plugin name="TEXT_ShowTinyMceInEditMode" defaultValue="False" />
```

updated icsharp

updated log4net

updated flickrnet

updated packetdotnet

Improved loading speed of terminals

Terminals will now load the DB connection only if a Terminals.Configuration.dll.config can be found and
a datasource has been entered like:
```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
	<appSettings>
	<add key="TerminalsConnection" value="Data Source=MyMachine;User Id=MyUser;Password=MyPwd;Initial Catalog=Terminals"></add>
	</appSettings>
</configuration>
```

Reduced plugin size due to autoresolving feature

Changed old database layer from Linq-to-Sql to Entity Framework (first time SharpDevelop support)

Increased logging

Upgraded TinyMce to 4.0.28.

Backward compatibility code and workarounds to make Terminals run on .net 2.0 has been removed -> Terminals is now fully .NET 4.0 compatible

The images that have been lost for the activation dialog in Terminals version 4.5.0.3 have been re-added.

Favorite notes will now be encoded and decoded in UFT-8 instead of ASCII (no conversion of the favorites notes needed - but after the first time Terminals 4.6 has been started the config won't be readable by Terminals 4.5.0.3 or lower).

Credentials XML will now support UTF-8 (no conversion needed - but after the first time Terminals 4.6 has been started the config won't be readable by Terminals 4.5.0.3 or lower).

Both the text plugin and the Kohl.TinyMce are now supporting data (base64) images.


##Release v 4.5.0.3

Date: 2014-03-21

Fixed a RDP focus problem that occured on some workstations due to problems with the underlaying COM control.

Removed the "grab focus" functionality from terminals.

Changed shortcut key for "Full Screen" from **{F11}** to **{Alt}** + **{F11}**.

The password manager now automatically sends the password to the keyboard after unleashing the password by right double clicks on the label.


##Release v 4.5.0.2

Date: 2013-12-02

Added script tag to the html form fields -> it is now able to use javascript for html automation.

Protocols are now sorted in the "Favorites Editor" by name.

The default selected protocol in the "Favorites Editor" is now RDP.

Fix DNS resolution error in tooltip

Terminals is now closing completely and won't remain in memory after the program exit.

Fixed a bug in the auto start program menu.

SayT has been implemented

Fixed HTTP connections in Terminals, IE browser control behaves different than Internet Explorer.

Terminals is now skipping certificate warnings in internet explorer

Removed de- and encryption for form fields -> no need and pain when porting the configuration from one machine to another.


##Release v 4.3.0.0

Date: 2013-10-21

Discovered bug in SSH and TELNET sessions -> not working -> Threading exception

Upgraded RDP version 6 to 8. Version 7 is now used.

Removed unneeded MSTSC dll.

Discovered a bug in RDP client control -> uncaught unmanaged exception where mouse stopped working in the RDP control.

Optimized startup of Terminals.

Optimized the process of showing the help screen. (-help and --help are working too, instead of /? -? or /help)

Option to use a different password safe file (Credentials.xml) is now available.
e.g.
> Terminals.exe -Cred:D:\MyPasswords.xml

Option to use a differnt configuration file for Terminals is now available.
e.g.
> Terminals.exe -Config:D:\MyConfigFile.conf

Fixed two bugs in the FireFox HTTP and HTTPS connection.

Sorted credentials in FavoriteEditor.


##Release v 4.2.0.0

Date: 2013-08-14

It is now possible to remove a custom assigned icon by hovering the picture box and pressing the red "x".

Removed chromium support (CefSharp)

Terminals is now able to redirect once in the hmtl form fields

RDP connection has been reprogrammed from scratch

Implemented NLA for RDP

Write access to the application directory will now be checked by comparing the write access of the ACL flags.

Fixed bugs in explorer connection.

The build process is now fully automated.

Removed Thumbs directory.

Removed obsolete resources.

If we can write to the application directory write there; otherwise use the "$AppData$\Oliver Kohl D.Sc.\Terminals" folder.

Now cultures can be specified in the program settings.

Fixed a obfuscation problem that prevented the history from being shown.

Improved history logging.

Fixed bug: RAdmin session not closing after click on the right corner x button (Application.Exit).

Fixed localization of input box and Kohl.Framework.

Improved the way treeviews are localized in the resx files e.g.
> EN_myTreeView1.Nodes[0] = 'My translation'
or
> DE_treeView1.Nodes["Test"] = 'ABC'

Localized TreeView in the options form.

Localized the untagged node in the favorites treeview.

Fixed localization issues.

Terminals is now callable from console with specific arguments (type help for more details)

Fixed a bug that occured in the favorite panel load logic. -> Old protocols haven't worked together with the new plugin architecture.

Added IAfterConnectSupport - connections can be chained by now

The plugin programmer is now able to exclude connections for which the plugin has NOT been written (via the ExcludedProtocols member).

Added the auto it editor.

Added scintilla for auto it connection.

Kohl.Framework will now initialize automatically.


##Release v 4.0.0.5

Date: 2013-06-19

Fixed HTML form field issue -> DateTime parser was unable to find some date formats.

Known Issues: History is not working.

Fixed some minor language display bugs.

Added new feature it is now possible to load the current log4net log file.

Fixed hard coded logging location -> will now be extracted from log4net config file.

Code cleanup in the main form.

Fixed a bug in the configuration file upgrade process.

If we have defined to use a master key in Terminals, we'll now request the password before and perform the configuration file parsing afterwards. This prevents some unbeautiful warnings in the log that the passwords are not vaild though they are.

Fixed languages issues with plugins.

Fixed an impersonation bug that occured in the explorer connection: Don't impersonate again if we are already impersonated.

Fixed a display bug in the tripple vertical view in the ControlStyler control (i.e. ExplorerConnection).

Improved Text-Plugin threading behaviour.

Unified password characters for password fields.

Improved performance of screen capture manager.

Fixed a memory leak in the screen capture manager.

Added some workarounds in the Explorer connection -> The underlaying Microsoft API is buggy.

Fixed a bug in the favorites tree that caused the application to crash.

Terminals will now fix hard coded configuration file values for this release.

Fixed some problems with the connection tool tip.

Saved connections will now load correctly.

A bug that prevented the usage of the feature "connect all" has been fixed. -> Connections will be loaded sequentially for that case instead of parallel.

Fixed bugs that occured on disposure.

Fixed bugs related to the connect to all/startup auto connect.

The URI http://www.kohl.bz/ has prevented the tool tip control from showing the actual server name.

Fixed the totally broken tab order sequence.

Clean up: Removed RDP lib from Main form.

Removed "tsRemoteToolbar" from code and configuration file.

Fixed a minor bug in drive redirection.

Terminals is now able to automatically load when the OS starts up (AutoStart).

Added the possibility to configure a custom wallpaper or background image in Terminals.

"this.MemberwiseClone" seems to be broken -> has been replaced with own method.

Made it possible to configure a relative path for the screen capture folder.

Removed obosolete tool strip icon

Removed max size of a label in an option panel which prevented the label from ever being shown.

Fixed form window state, size and location bugs.

Fixed multi-threading issues in RDP connection etc.!

Corrected null pointer exception when opening the option form.

Added setting "InvertTabPageOrder".

Changed resize behaviour of RDP.


##Release v 3.9.0.0

Date: 2013-02-22

Changed the connection.connected property behaviour!!!! -> not always true - false on disconnect etc.

Unified form icons.

First draft of plugin architecture has been implemented ... Terminals is now capable of dynamically loading "favorite panels", "option panels" and custom "connection types". Have a look a the "TextConnection" type version 1.2 Beta.

Moved RDP favorites to a separate tab page

Generic application will now load the exe application icon if no icon has been set.

Fixed a file handle bug - Terminals has locked a file and never released it until application exit after a custom connection icon has been choosen.

Fixed minor bugs in the Html form fields logic.

Reduced log4net configuration file complexity.

Custom icons can now be added per connection.

Implemented TextConnection (based on the great LGPL TinyMce http://www.tinymce.com/)

Implemented a favorites search functionality in the Terminals main window (search as you type).

Default credentials are now consistent.

Seperated configuration and connections from UI. (Terminals.Configuration, Terminals.Connection)

Removed VNCSharp dependency for zlib.net (Own compression mechanism will be taken!)

Changed Gecko (XulRunner) from Skybound Geckofx 14 (they stopped development) to Bitbucket Geckofx (fork of Skybound) 16.

Removed obsolete library "TabControl"

Downloaded the new Gecko SDK from https://developer.mozilla.org/en/docs/Gecko_SDK (yet unused - version 17.0) and
http://ftp.mozilla.org/pub/mozilla.org/xulrunner/releases/, http://ftp.mozilla.org/pub/mozilla.org/xulrunner/releases/18.0b7/sdk/xulrunner-18.0b7.en-US.win32.sdk.zip (yet unused - version 18.0 Beta 7)

Removed unneeded FormLanguageSwitch framework (-> custom logic is smaller and smarter)

Deleted unused and obsolete (2006) socks library from Mentalis.org:
http://www.mentalis.org/soft/projects/ssocket/

Excerpt from http://www.mentalis.org/soft/projects/secserv/download.qpx:
>   Security Services for .NET 2.0 - Downloads
>   This page lists the download locations of the Mentalis.org Security Services for .NET 2.0 library. The current version is v0.0.2 (beta).
>   Download Full Archive
>   This archive consists of the Security Services source code, a signed pre-compiled version of the library, and the full documentation. Future versions of the library will contain example projects and unit tests. 
>   Mentalis.org (North America, Virginia) [406 Kb] 
>   Version History
>   v0.0.1b  Initial public beta release. 2006/05/14 
>   v0.0.2b  First beta revision. 2006/06/25   

Upgraded ICSharpCode.SharpZipLib.dll from version 0.85.4.369 to version 0.86.0.518.

Updated Amazon S3 (AWS SDK) from Version 1.4.6.2 to Version 1.5.10.0.

Improved logging mechanism in RAS connection and RAS connection properties, user will now see the connection result both on the screen and in the log file.

Upgraded DotRas from change set 93435 (DotRas 1.2) to change set 99043 (DotRas 1.3 RC).

Updated third party library HexBox from 1.4.8 to 1.5.0.

Added quick connection feature (this can be accessed via context menu or by pressing F3).

Implemented auto reload for Favorites only if the favorites settings have been changed.

Setting "HideFavoritesFromQuickMenu": Configurable whether to show the favorites in the context menu (quick menu) or vice versa (default not shown: HideFavoritesFromQuickMenu = true). The menu can be accessed by right clicking a plain terminal without any connection.

Tag will be choosen automatically for new connections, if a tag has been selected in a treeview or a favorite has been selected in the treeview (Setting "AutoSetTag" must be true, default = true).

Tab Pages can now be sorted by caption or by sequence (setting the checkbox (Setting "SortTabPagesByCaption") will sort the pages by caption).

Complete rewrite of tab page code basis.

The active tab page is now always the first one (exception if the user manually selects a tab page header).

Performed a huge number of refactoring operations on tab page classes.

Added the possibility to manually rearrange the pages by using drag and drop:
	-> Hover a tab page header, press the left mouse button and hold it down, now move the cursor
	-> without releasing the mouse button, to place the new tab page put it onto an other one.

TreeView icons and font settings (size, thickness, etc.) are now configurable.

Removed unused vnc settings.

Clean up of class "FavoriteConfigurationElement".

The favorite property "telnet" is now obsolete and has been removed from the config file.

The favorite property "telnetrows" is now obsolete and has been removed from the config file.

The favorite property "telnetcols" is now obsolete and has been removed from the config file.

The favorite property "telnetfont" is now obsolete and has been removed from the config file.

The favorite property "telnetbackcolor" is now obsolete and has been removed from the config file.

The favorite property "telnettextcolor" is now obsolete and has been removed from the config file.

The favorite property "telnetcursorcolor" is now obsolete and has been removed from the config file.

The configuration file size has been shrinked by defaulting a null value for the favorite property "htmlFormFields".


##Release v 3.8.0.0

Date: 2012-12-12

Improved Terminals' import and export mechanism. -> No need for hard-coding anything!!!

Fixed a GUI bug in the Optionpanel (StartShutdownOptionPanel) -> not both CheckBoxes can be selected at the same time, but it is possible to select nothing.

Added text to a CheckBox in the StartShutdownOptionPanel.

Added the possiblity to set the favorites tree view font size, text color, background color and icon size.

FavoritesTreeView has caused the MainForm to break the VS Designer (SharpDevelop was able to display the form) -> resolved

Removed two empty TerminalTabControlItems in the MainForm -> this causes a little performance improvement.

TerminalTabControlItems are now identifyable by name (instead of the caption/title as previously) -> this fixes problems if Terminals auto starts connections.

Removed the unneeded method AddConnection from Terminals.Configuration.Settings.Settings

Prepared FavoritesTreeView for configurable font size and icon size.

Removed about 40 classes -> huge code cleanup and refactoring.

Fixed a bug in the html form fields date time replacement engine.

Cleaned up namespaces.

Fixed a bug in the RAdmin which appeared when clicking "Connect to all"

Fixed a RDP bug that prevented Terminals from taking over the shared RDP clipboard.

Improved configuration update code (i.e. from 3.7.0.0 to 3.8.0.0)

Moved configuration backup method to start and removed other obsolete backup methods.

The settings property "savedCredentials" is now obsolete and has been removed from the config file.


##Release v 3.7.0.0

Date: 2012-12-04

History and favorite icon handling has been cleaned up.

Integrated explorer connection.

Added serial number protection.

Added license model.

Now getting the expanded favorites nodes in a thread safe manner.

Now getting the expanded history nodes in a thread safe way.

A fix for RDP connections has been implemented which prevents the system from showing: "A website is trying to start a remote connection" since the last Windows update.

Improved threading and dispose behaviour for each network tool.

Fixed a memory leak in the network tools, which led to high CPU and to an application freeze.

Reveal credential passwords by double clicking them (with the right mouse button)

Trusted firefox sites and certificates will only be synced if our application is of type HTTP(S) connection is of type FireFox.

Performed havy code clean up

Html form fields will now be saved machine independent or dependent on a master password -> and updated if we change the master password.

Updated SplitButton to version 2.1 http://wyday.com/splitbutton/

Improved the Terminals command line options and arguments.

The screen capture manager will now initialize with the correct ruler setting.

Fixed a lot of designer bugs i.e. design time bugs -> SharpDevelop designer has thrown exceptions.

Added form fields to the generic connection.

Replaced the hard coded "Terminals" string with Info.Title

Added explorer connection (C# File explorer <- allows mapping of fileshares)

Moved the properties to the end of the context menu.

Compressed the binaries

Removed obsolete "Terminals 1.0.x localization"

Implemented modern localization for Terminals 3.7
	German
	English

First part of localization finished second part will follow in Terminals 3.8

User experience has been improved due to the usage of a different icon set.


##Release v 3.6.0.0

Date: 2012-07-26

Terminals is now multi monitor compatible and able to save and reload form settings and window state.

Integrated FireFox browser.

Changed default browser to FireFox

Removed some obsolete code.

Added some images to the context menus.

Removed some obsolete context menus.

Fixed the Terminals.config backup mechanism

Added a backup mechanism for the Credentials.xml file.

Simplified the Terminals VersionInformation handling. -> Removed a lot of obsolete and redundant code.

Fixed a problem in build date detection (took wrong assembly -> Terminals.Configuration.dll instead of main assembly).

Fixed output formatting for About form.

Added a document for problems with XulRunner.

Removed duplicate app.config files in the Terminals proj and Terminals.Configuration proj.

Fixed a problem in the debug.bat and release.bat files.


##Release v 3.5.0.0

Date: 2011-01-03

Fixed exceptions that appear in the log file.

Updated Terminals RAS connection to "DotRas".

Added threading capabilities for Terminals connections.

Added Citrix ICA (Receiver) connection.


##Release v 3.4.0.0

Date: 2009-09-17

Added chromium browser


##Release v 3.0.0.0

Date: 2007-01-26

IE Browser extensions:
	http Form fields
	authentication support
	Problems with active X security have been solved

Terminals.config backup

Support for RAdmin

Support for Putty

Support for any external application

Fixed color bug for terminal connections

Removed weird protocol handling

Reduction of Terminals.config size (ConfigurationSaveMode.Minimal)

Migrated ToolStripSettings (ToolStrip.settings.config) to Terminals.config

Added icons, removed obsolete icons.

Added copy and delete actions to the favorites context menu

Limited the number of allowed connections to be loaded (Favorites and Tags are configurable)

Fixed invalid port or server name bug.

Updated network tools -> Package capture feature is now working, updated Hex-Editor, updated PCap, updated WhoIs.

Added the possibility to configure the Tab Page Header Color.

Added dispatch functionality in "Quick Edit" context menu.

Removed support for .TRM file handle extension -> Throwed exceptions in event log.

The ManageCredentialForm is now able to show the password in plain text by double clicking the password label.

##History

This code is based on the Terminals version 1.0 located on CodePlex.

| URL                                              | Date | Version |
|--------------------------------------------------|------|---------|
|https://terminals.codeplex.com/releases/view/1659 | 2007 |     1.0 |


1) Support for RDP 6:
- 32bit color support
- Supports screen resolutions of up to 4096x2048
- Supports disabling clipboard redirection
- Enable smart card redirection
- Enable plug&play devices redirection

2) Save position and size
3) Nicer about box...
4) Execute before connect (per connection and for all connections).
5) Some bugs were fixed.

The original version had an issue on some machines and crashed.

All rights for the original code belong to [Rob Chartier](https://www.codeplex.com/site/users/view/RobChartier) and his colleagues ([about 8 people in total](https://terminals.codeplex.com/team/view)).

The code itself differs completely from the original version, nearly no parts except 3rd party libraries are in it's orignal condition. I attempted to improve the code, fixed a lot of bugs and added a lot of features.



