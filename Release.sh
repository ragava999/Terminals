#!/bin/bash
ROOT="/home/ubuntu/Terminals"
VERSION=`cat /home/ubuntu/Terminals/Terminals/Properties/AssemblyInfo.cs | grep "AssemblyVersion(" | cut -d \" -f 2`
ARTIFACTS="/home/ubuntu/Terminals/Terminals/bin/x86/Release/"

echo $VERSION

cd $ARTIFACTS

cp "/home/ubuntu/Terminals/TerminalsUpdater/bin/Release/TerminalsUpdater.exe" .
cp "/home/ubuntu/Terminals/Kohl.Explorer/bin/Release/Kohl.Explorer.exe" .

# Copy AutoIt to Plugin directory
cp "/home/ubuntu/Terminals/DLLs/Tools/AutoIt/Au3Info.exe" "Plugins/AutoIt/"
cp "/home/ubuntu/Terminals/DLLs/Tools/AutoIt/Au3Info_x64.exe" "Plugins/AutoIt/"
cp "/home/ubuntu/Terminals/DLLs/Tools/AutoIt/AutoIt3.exe" "Plugins/AutoIt/"
cp "/home/ubuntu/Terminals/DLLs/Tools/AutoIt/AutoIt3_x64.exe" "Plugins/AutoIt/"
cp -r "/home/ubuntu/Terminals/DLLs/Tools/AutoIt/Au3Record" "Plugins/AutoIt/"
cp -r "/home/ubuntu/Terminals/DLLs/Tools/AutoIt/Aut2Exe" "Plugins/AutoIt/"
cp -r "/home/ubuntu/Terminals/DLLs/Tools/AutoIt/Include" "Plugins/AutoIt/"

# Copy XUL runner dependencies
cp "/home/ubuntu/Terminals/DLLs/xulrunner-29.0.en-US.win32/xulrunner/omni.ja" .
cp "/home/ubuntu/Terminals/DLLs/xulrunner-29.0.en-US.win32/xulrunner/plugin-container.exe" .
cp "/home/ubuntu/Terminals/DLLs/xulrunner-29.0.en-US.win32/xulrunner/freebl3.dll" .
cp "/home/ubuntu/Terminals/DLLs/xulrunner-29.0.en-US.win32/xulrunner/gkmedias.dll" .
cp "/home/ubuntu/Terminals/DLLs/xulrunner-29.0.en-US.win32/xulrunner/libEGL.dll" .
cp "/home/ubuntu/Terminals/DLLs/xulrunner-29.0.en-US.win32/xulrunner/libGLESv2.dll" .
cp "/home/ubuntu/Terminals/DLLs/xulrunner-29.0.en-US.win32/xulrunner/mozalloc.dll" .
cp "/home/ubuntu/Terminals/DLLs/xulrunner-29.0.en-US.win32/xulrunner/mozglue.dll" .
cp "/home/ubuntu/Terminals/DLLs/xulrunner-29.0.en-US.win32/xulrunner/mozjs.dll" .
cp "/home/ubuntu/Terminals/DLLs/xulrunner-29.0.en-US.win32/xulrunner/nss3.dll" .
cp "/home/ubuntu/Terminals/DLLs/xulrunner-29.0.en-US.win32/xulrunner/nssckbi.dll" .
cp "/home/ubuntu/Terminals/DLLs/xulrunner-29.0.en-US.win32/xulrunner/nssdbm3.dll" .
cp "/home/ubuntu/Terminals/DLLs/xulrunner-29.0.en-US.win32/xulrunner/softokn3.dll" .
cp "/home/ubuntu/Terminals/DLLs/xulrunner-29.0.en-US.win32/xulrunner/xul.dll" .

# Copying Microsoft Virtual Machine Remote Control (VMRC) dependencies.
cp "/home/ubuntu/Terminals/DLLs/VMRC/VMKeyboardHook.dll" .
cp "/home/ubuntu/Terminals/DLLs/VMRC/VMKeyboardHook64.dll" .
cp "/home/ubuntu/Terminals/DLLs/VMRC/VMRCActiveXClient.dll" .
cp "/home/ubuntu/Terminals/DLLs/VMRC/VMRCActiveXClient64.dll" .
cp "/home/ubuntu/Terminals/DLLs/VMRC/RegisterVMRC.bat" .

# Copying Citrix dependencies.
cp "/home/ubuntu/Terminals/DLLs/Citrix/AxWFICALib.dll" .
cp "/home/ubuntu/Terminals/DLLs/Citrix/WfIcaLib.dll" .

# Copy putty.exe and RAdmin
cp "/home/ubuntu/Terminals/DLLs/Tools/Putty/putty.exe" .
cp -r "/home/ubuntu/Terminals/DLLs/Tools/Radmin Viewer 3" .

# Delete files from the root of Terminals which are already part of our plugins (the files are intended for AutoIt plugin not for Terminals itself)
rm -f "Terminals.exe.config"

# Remove things not needed in the root folder of the plugins
rm -rf "Plugins/Kohl.*"
rm -f "Plugins/AutoIt/log4net.dll"
rm -f "Plugins/AutoIt/ICSharpCode.AvalonEdit.xml"
rm -f "Plugins/AutoIt/Terminals.Plugins.AutoIt.pdb"
rm -f "Plugins/AutoIt/Terminals.Plugins.AutoIt.mdb"
rm -f "Plugins/log4net.dll"
rm -f "Plugins/Terminals.Connection.dll"
rm -f "Plugins/Terminals.Connection.pdb"
rm -f "Plugins/Terminals.Connection.mdb"
rm -f "Plugins/Terminals.Configuration.dll"

# Remove debugging symbols
rm -f "ExplorerBrowser.dll.mdb"
rm -f "Geckofx-Core.dll.mdb"
rm -f "Geckofx-Winforms.dll.mdb"
rm -f "VncSharp.dll.mdb"
rm -f "Kohl.Framework.dll.mdb"

# Get a list of all files
# find .

######## CREATE ZIP FILE

echo Packing and compressing the distributable version.
zip -r -9 -o -q Terminals.zip .

######## OPTIONAL STEPS - CREATE SETUP >>>
echo Starting SETUP

# Replace the contents of the log4net file
# Reason: If the user installs Terminals via Setup he won't have write permissions in his ProgramFiles directory
sed -i -- 's/Logs\\\\Terminals\.txt/${LOCALAPPDATA}\\\\Oliver Kohl D\.Sc\.\\\\Terminals\\\\Logs\\Terminals\.txt/g' Terminals.log4net.config

echo Starting InnoSetup compilation

SCRIPTNAME="/home/ubuntu/Terminals/Terminals.Setup/Terminals.iss"
INNO_PATH="/home/ubuntu/Terminals/Terminals.Setup/Inno Setup 5/ISCC.exe"

# Merging version into iss script
sed -i -- "s/9.9.9.9/$VERSION/g" $SCRIPTNAME

# Check if wine is present
command -v wine >/dev/null 2>&1 || { echo >&2 "I require wine but it's not installed. Aborting."; echo; exit 1; }

# Check if Inno Setup is installed into wine
[ ! -f "$INNO_PATH" ] && { echo "Install Inno Setup 5 Quickstart before running this script."; echo; exit 1; }

# Get Program Files path via wine command prompt
#PROGRAMFILES=$(wine cmd /c 'echo %programfiles(x86)%' 2>/dev/null)
# Translate unix script path to windows path 
SCRIPTNAME=$(winepath -w "$SCRIPTNAME" 2> /dev/null)
INNO_PATH=$(winepath -w "$INNO_PATH" 2> /dev/null)

# Compile!
WINEDLLOVERRIDES="mscoree,mshtml=" wine "$INNO_PATH" "$SCRIPTNAME" 

cp /home/ubuntu/Terminals/Terminals.Setup/Output/Setup_Terminals.exe $ARTIFACTS

echo Build optimazation done
######## OPTIONAL STEPS - END <<<

######## REMOVE EVERYTHING EXCEPT THE ZIP FILE AND THE SETUP FILE

rm -f *.xml
rm -f *.bat
rm -f *.ja
rm -f *.dll
rm -f *.mdb
rm -f *.config
rm -f *.exe
rm -rf Plugins
rm -rf "Radmin Viewer 3"

######## DOWNLOAD AND INVOKE "GHR" - a tool capable of uploading a release to GitHub

echo Grabbing GHR

go get github.com/tcnksm/ghr

echo Finished downloading GHR. Progressing with the upload.

ghr -t $GITHUB_TOKEN -u $CIRCLE_PROJECT_USERNAME -r $CIRCLE_PROJECT_REPONAME --replace $VERSION $ARTIFACTS
ghr -t $GITHUB_TOKEN -u $CIRCLE_PROJECT_USERNAME -r $CIRCLE_PROJECT_REPONAME --replace $VERSION "/home/ubuntu/Terminals/Terminals.Setup/Output/"

echo Uploads have been completed successfully.

echo Done. Exiting script ...
