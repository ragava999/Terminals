@Echo off

rem Set the XulRunner runtime version (i.e. the Mozilla engine), ATTENTION: the SDK version won't work
set XULRUNNER=xulrunner-16.0.2.en-US.win32\xulrunner
set XULRUNNER=xulrunner-29.0.en-US.win32\xulrunner

rem Copy SciLexer.dll and SciLexer64.dll from Scintilla to the Plugin directory.
copy "..\..\..\..\DLLs\ScintillaNET v2.5.2\SciLex*.dll" "Plugins\AutoIt\SciLex*.dll" /Y > NUL

rem Copy AutoIt to Plugin directory
xcopy "..\..\..\..\DLLs\Tools\AutoIt" "Plugins\AutoIt" /E /C /R /I /K /Y > NUL
 
rem Copy the explorer browser to Terminals.exe directory
copy "..\..\..\..\Kohl.Explorer\bin\Release\Kohl.Explorer.exe" "Kohl.Explorer.exe" /Y > NUL

del /Q /F /S "..\Distribution Release\*.*" > NUL
rmdir /S /Q "..\Distribution Release\" > NUL
mkdir "..\Distribution Release\" > NUL
cls

echo.
echo Copying configuration files.
copy "Credentials.xml" "..\Distribution Release\Credentials.xml" /Y > NUL
copy "Terminals.config" "..\Distribution Release\Terminals.config" /Y > NUL
copy "Terminals.log4net.config" "..\Distribution Release\Terminals.log4net.config" /Y > NUL

echo.
echo Copying Terminals.Configuration.dll.
copy "Terminals.Configuration.dll" "..\Distribution Release\Terminals.Configuration.dll" /Y > NUL

echo Copying FireFox dependencies.
copy "..\..\..\..\DLLs\%XULRUNNER%\omni.ja" "omni.ja" /Y > NUL
copy "..\..\..\..\DLLs\%XULRUNNER%\plugin-container.exe" "plugin-container.exe" /Y > NUL
copy "..\..\..\..\DLLs\%XULRUNNER%\freebl3.dll" "freebl3.dll" /Y > NUL
copy "..\..\..\..\DLLs\%XULRUNNER%\gkmedias.dll" "gkmedias.dll" /Y > NUL
copy "..\..\..\..\DLLs\%XULRUNNER%\libEGL.dll" "libEGL.dll" /Y > NUL
copy "..\..\..\..\DLLs\%XULRUNNER%\libGLESv2.dll" "libGLESv2.dll" /Y > NUL
copy "..\..\..\..\DLLs\%XULRUNNER%\mozalloc.dll" "mozalloc.dll" /Y > NUL
copy "..\..\..\..\DLLs\%XULRUNNER%\mozglue.dll" "mozglue.dll" /Y > NUL
copy "..\..\..\..\DLLs\%XULRUNNER%\mozjs.dll" "mozjs.dll" /Y > NUL
copy "..\..\..\..\DLLs\%XULRUNNER%\nss3.dll" "nss3.dll" /Y > NUL
copy "..\..\..\..\DLLs\%XULRUNNER%\nssckbi.dll" "nssckbi.dll" /Y > NUL
copy "..\..\..\..\DLLs\%XULRUNNER%\nssdbm3.dll" "nssdbm3.dll" /Y > NUL
copy "..\..\..\..\DLLs\%XULRUNNER%\softokn3.dll" "softokn3.dll" /Y > NUL
copy "..\..\..\..\DLLs\%XULRUNNER%\xul.dll" "xul.dll" /Y > NUL

echo.
echo Copying Microsoft Virtual Machine Remote Control (VMRC) dependencies.
copy "..\..\..\..\DLLs\VMRC\*.dll" "*.dll" /Y > NUL
copy "..\..\..\..\DLLs\VMRC\RegisterVMRC.bat" "RegisterVMRC.bat" /Y > NUL

echo.
echo Copying Citrix dependencies.
copy "..\..\..\..\DLLs\Citrix\*.dll" "*.dll" /Y > NUL

echo.
echo Copy everything from the release folder to the distribution folder.
copy "*.dll" "..\Distribution Release\*.dll" /Y > NUL
copy "*.pak" "..\Distribution Release\*.pak" /Y > NUL
copy "*.bat" "..\Distribution Release\*.bat" /Y > NUL
copy "Kohl.Explorer.exe" "..\Distribution Release\Kohl.Explorer.exe" /Y > NUL
copy "Terminals.exe" "..\Distribution Release\Terminals.exe" /Y > NUL
copy "plugin-container.exe" "..\Distribution Release\plugin-container.exe" /Y > NUL
copy "Kohl.Explorer.exe" "..\Distribution Release\Kohl.Explorer.exe" /Y > NUL
copy "omni.ja" "..\Distribution Release\omni.ja" /Y > NUL

echo Copying plugins
copy "..\..\..\..\DLLs\Tools\Putty\putty.exe" "..\Distribution Release\putty.exe" /Y > NUL
xcopy "..\..\..\..\DLLs\Tools\Radmin Viewer 3" "..\Distribution Release\Radmin Viewer 3" /E /C /R /I /K /Y > NUL
xcopy "Plugins" "..\Distribution Release\Plugins" /E /C /R /I /K /Y > NUL
del /Q /F "..\Distribution Release\Plugins\Kohl.*"
del /Q /F "..\Distribution Release\Plugins\AutoIt\ScintillaNET.pdb"
del /Q /F "..\Distribution Release\Plugins\log4net.dll"
del /Q /F "..\Distribution Release\Plugins\Terminals.Connection.dll"
del /Q /F "..\Distribution Release\Plugins\Terminals.Connection.pdb"
del /Q /F "..\Distribution Release\Plugins\Terminals.Configuration.dll"

echo Compilation and clean up done - optional last step will be started now ...

cd ..\Distribution Release\

echo Preparing netz compiler
copy /Y ..\..\..\..\netz-src\netz\compress\defcomp.dll ..\..\..\..\netz-src\netz\bin\Release\defcomp.dll
copy /Y ..\..\..\..\netz-src\netz\compress\zip.dll ..\..\..\..\netz-src\netz\bin\Release\zip.dll

echo Packing and compressing the distributable version.

"..\..\..\..\netz-src\netz\bin\Release\netz.exe" -z -w -o Out -s -so -x86 Kohl.Explorer.exe Microsoft.WindowsAPICodePack.dll ICSharpCode.SharpZipLib.dll Microsoft.WindowsAPICodePack.Shell.dll ExplorerBrowser.dll
rem It is not a good idea to embed the AxInterop.MSTSCLib.dll
"..\..\..\..\netz-src\netz\bin\Release\netz.exe" -z -w -o Out -s -so -x86 Terminals.exe AxInterop.Microsoft.VMRCClientControl.Interop.dll AxWFICALib.dll Be.Windows.Forms.HexBox.dll DotRas.dll ICSharpCode.SharpZipLib.dll log4net.dll Metro.dll Microsoft.VMRCClientControl.Interop.dll PacketDotNet.dll SharpPcap.dll TerminalEmulator.dll VncSharp.dll WFICALib.dll ZedGraph.dll Microsoft.WindowsAPICodePack.dll Microsoft.WindowsAPICodePack.Shell.dll ExplorerBrowser.dll

echo Copy the Terminals DLLs and config to the output folder
copy /Y "Terminals.log4net.config" "Out\Terminals.log4net.config"
copy /Y "Kohl.Framework.dll" "Out\Kohl.Framework.dll"
copy /Y "Terminals.Configuration.dll" "Out\Terminals.Configuration.dll"
copy /Y "Terminals.Connection.dll" "Out\Terminals.Connection.dll"

echo Copy the XUL DLLs to the output folder
copy /Y "omni.ja" "Out\omni.ja"
copy /Y "plugin-container.exe" "Out\plugin-container.exe"
copy /Y "freebl3.dll" "Out\freebl3.dll"
copy /Y "gkmedias.dll" "Out\gkmedias.dll"
copy /Y "libEGL.dll" "Out\libEGL.dll"
copy /Y "libGLESv2.dll" "Out\libGLESv2.dll"
copy /Y "mozalloc.dll" "Out\mozalloc.dll"
copy /Y "mozglue.dll" "Out\mozglue.dll"
copy /Y "mozjs.dll" "Out\mozjs.dll"
copy /Y "nss3.dll" "Out\nss3.dll"
copy /Y "nssckbi.dll" "Out\nssckbi.dll"
copy /Y "nssdbm3.dll" "Out\nssdbm3.dll"
copy /Y "softokn3.dll" "Out\softokn3.dll"
copy /Y "xul.dll" "Out\xul.dll"

echo Copy the .NET XUL wrapper DLLs to the output folder
copy /Y "Geckofx-Core.dll" "Out\Geckofx-Core.dll"
copy /Y "Geckofx-Winforms.dll" "Out\Geckofx-Winforms.dll"

echo Copy third party DLLs to the output folder
copy /Y "VMKeyboardHook.dll" "Out\VMKeyboardHook.dll"
copy /Y "VMKeyboardHook64.dll" "Out\VMKeyboardHook64.dll"
copy /Y "VMRCActiveXClient.dll" "Out\VMRCActiveXClient.dll"
copy /Y "VMRCActiveXClient64.dll" "Out\VMRCActiveXClient64.dll"
copy /Y "AxInterop.MSTSCLib.dll" "Out\AxInterop.MSTSCLib.dll"
copy /Y "Interop.MSTSCLib.dll" "Out\Interop.MSTSCLib.dll"
copy /Y ICSharpCode.SharpZipLib.dll Out\ICSharpCode.SharpZipLib.dll
rem copy /Y "..\..\..\..\netz-src\netz\compress\zip.dll" Out\zip.dll
rem copy /Y "..\..\..\..\netz-src\netz\compress\defcomp.dll" Out\defcomp.dll

copy /Y "putty.exe" "Out\putty.exe"
xcopy "Radmin Viewer 3" "Out\Radmin Viewer 3" /E /C /R /I /K /Y > NUL
copy /Y "*.bat" "Out\*.bat"

echo Copying plugins
XCOPY "Plugins" "Out\Plugins" /E /C /R /I /K /Y > NUL

cd Out

rem Merge the Assembly info file into the resulting exe file and pack the original assemblies as dll
rem The Rpx\Packing\CompileProcess.cs file has been patched at line 318 to support the x86 platform i.e.
rem -> from
rem           cp.CompilerOptions = " /filealign:512 /optimize+";
rem -> to
rem           cp.CompilerOptions = " /filealign:512 /optimize+ /platform:x86";
rem
"..\..\..\..\..\Resources\Packer\rpx-1.3-14635\Rpx\bin\Debug\rpx.exe" Terminals.exe /A "..\..\..\..\..\Terminals\Properties\AssemblyInfo.cs" /output Terminals.exe +W 
"..\..\..\..\..\Resources\Packer\rpx-1.3-14635\Rpx\bin\Debug\rpx.exe" Kohl.Explorer.exe /A "..\..\..\..\..\Kohl.Explorer\Properties\AssemblyInfo.cs" /output Kohl.Explorer.exe +W 

echo Pack our DLLs
"..\..\..\..\..\Resources\mpress\mpress.exe" -s plugin-container.exe
rem Packing putty with upx results in an McAfee virus warning!
"..\..\..\..\..\Resources\mpress\mpress.exe" -s putty.exe
..\..\..\..\..\Resources\Packer\upx308w\upx.exe -9 freebl3.dll
..\..\..\..\..\Resources\Packer\upx308w\upx.exe -9 gkmedias.dll
..\..\..\..\..\Resources\Packer\upx308w\upx.exe -9 libEGL.dll
..\..\..\..\..\Resources\Packer\upx308w\upx.exe -9 libGLESv2.dll
..\..\..\..\..\Resources\Packer\upx308w\upx.exe -9 mozalloc.dll
..\..\..\..\..\Resources\Packer\upx308w\upx.exe -9 mozglue.dll
..\..\..\..\..\Resources\Packer\upx308w\upx.exe -9 mozjs.dll
..\..\..\..\..\Resources\Packer\upx308w\upx.exe -9 nss3.dll
..\..\..\..\..\Resources\Packer\upx308w\upx.exe -9 nssckbi.dll
..\..\..\..\..\Resources\Packer\upx308w\upx.exe -9 nssdbm3.dll
..\..\..\..\..\Resources\Packer\upx308w\upx.exe -9 softokn3.dll
..\..\..\..\..\Resources\Packer\upx308w\upx.exe -9 VMKeyboardHook.dll
"..\..\..\..\..\Resources\mpress\mpress.exe" -s VMKeyboardHook64.dll
..\..\..\..\..\Resources\Packer\upx308w\upx.exe -9 VMRCActiveXClient.dll
"..\..\..\..\..\Resources\mpress\mpress.exe" -s VMRCActiveXClient64.dll
..\..\..\..\..\Resources\Packer\upx308w\upx.exe -9 xul.dll

cd ..\..

echo.
echo Done
echo.
rem pause
exit 0;