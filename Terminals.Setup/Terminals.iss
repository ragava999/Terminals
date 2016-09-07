[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{D76D63D5-A024-4B5D-8E69-2A65EB1F4C6A}
AppName=Terminals
AppVersion=9.9.9.9
AppVerName=Terminals 9.9.9.9
VersionInfoVersion=9.9.9.9
AppPublisher=Oliver Kohl D.Sc.
AppPublisherURL=https://github.com/OliverKohlDSc/Terminals
AppSupportURL=https://github.com/OliverKohlDSc/Terminals
AppUpdatesURL=https://github.com/OliverKohlDSc/Terminals
DefaultDirName={pf}\Terminals
DefaultGroupName=Terminals
;PrivilegesRequired=lowest
AllowNoIcons=yes
InfoBeforeFile=Z:\home\ubuntu\Terminals\ReadMe_Description.txt
InfoAfterFile=Z:\home\ubuntu\Terminals\ReadMe.md
OutputBaseFilename=Setup_Terminals
SetupIconFile=Z:\home\ubuntu\Terminals\Terminals\terminalsicon.ico
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"
Name: "catalan"; MessagesFile: "compiler:Languages\Catalan.isl"
Name: "corsican"; MessagesFile: "compiler:Languages\Corsican.isl"
Name: "czech"; MessagesFile: "compiler:Languages\Czech.isl"
Name: "danish"; MessagesFile: "compiler:Languages\Danish.isl"
Name: "dutch"; MessagesFile: "compiler:Languages\Dutch.isl"
Name: "finnish"; MessagesFile: "compiler:Languages\Finnish.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"
Name: "greek"; MessagesFile: "compiler:Languages\Greek.isl"
Name: "hebrew"; MessagesFile: "compiler:Languages\Hebrew.isl"
Name: "hungarian"; MessagesFile: "compiler:Languages\Hungarian.isl"
Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"
Name: "norwegian"; MessagesFile: "compiler:Languages\Norwegian.isl"
Name: "polish"; MessagesFile: "compiler:Languages\Polish.isl"
Name: "portuguese"; MessagesFile: "compiler:Languages\Portuguese.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "scottishgaelic"; MessagesFile: "compiler:Languages\ScottishGaelic.isl"
Name: "serbiancyrillic"; MessagesFile: "compiler:Languages\SerbianCyrillic.isl"
Name: "serbianlatin"; MessagesFile: "compiler:Languages\SerbianLatin.isl"
Name: "slovenian"; MessagesFile: "compiler:Languages\Slovenian.isl"
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"
Name: "turkish"; MessagesFile: "compiler:Languages\Turkish.isl"
Name: "ukrainian"; MessagesFile: "compiler:Languages\Ukrainian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "Z:\home\ubuntu\Terminals\Terminals\bin\x86\Release\*"; Excludes: "Z:\home\ubuntu\Terminals\Terminals\bin\x86\Release\Terminals.zip"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\Terminals"; Filename: "{app}\Terminals.exe"
Name: "{group}\{cm:ProgramOnTheWeb,Terminals}"; Filename: "https://github.com/OliverKohlDSc/Terminals"
Name: "{group}\{cm:UninstallProgram,Terminals}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\Terminals"; Filename: "{app}\Terminals.exe"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\Terminals"; Filename: "{app}\Terminals.exe"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\Terminals.exe"; Description: "{cm:LaunchProgram,Terminals}"; Flags: nowait postinstall skipifsilent

[Registry]
Root: HKCU; Subkey: "Software\Oliver Kohl D.Sc.\Terminals"; Flags: uninsdeletekey