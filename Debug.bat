@Echo off

rem Set the XulRunner runtime version (i.e. the Mozilla engine), ATTENTION: the SDK version won't work
set XULRUNNER=xulrunner-16.0.2.en-US.win32\xulrunner
set XULRUNNER=xulrunner-29.0.en-US.win32\xulrunner

rem Copy SciLexer.dll and SciLexer64.dll from Scintilla to the Plugin directory.
copy "..\..\..\..\DLLs\ScintillaNET v2.5.2\SciLex*.dll" "Plugins\AutoIt\SciLex*.dll" /Y > NUL

rem Copy AutoIt to Plugin directory
copy "..\..\..\..\DLLs\Tools\AutoIt\AutoIt3.exe" "Plugins\AutoIt\AutoIt3.exe" /Y > NUL

rem Copy the explorer browser to Terminals.exe directory
copy "..\..\..\..\Kohl.Explorer\bin\Debug\Kohl.Explorer.exe" "Kohl.Explorer.exe" /Y > NUL

echo Copying configuration files.
copy "Credentials.xml" "..\Distribution Debug\Credentials.xml" /Y > NUL
copy "Terminals.config" "..\Distribution Debug\Terminals.config" /Y > NUL
copy "Terminals.log4net.config" "..\Distribution Debug\Terminals.log4net.config" /Y > NUL

echo.
echo Copying Terminals.Configuration.dll & KeePassLib.dll.
copy "Terminals.Configuration.dll" "..\Distribution Debug\Terminals.Configuration.dll" /Y > NUL
copy "KeePassLib.dll" "..\Distribution Debug\KeePassLib.dll" /Y > NUL

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
echo Copy everything from the debug folder to the distribution folder.
copy "..\..\..\..\DLLs\Tools\Putty\putty.exe" "..\Distribution Debug\putty.exe" /Y > NUL
xcopy "..\..\..\..\DLLs\Tools\Radmin Viewer 3" "..\Distribution Debug\Radmin Viewer 3" /E /C /R /I /K /Y > NUL
copy "*.dll" "..\Distribution Debug\*.dll" /Y > NUL
copy "*.pak" "..\Distribution Debug\*.pak" /Y > NUL
copy "*.bat" "..\Distribution Debug\*.bat" /Y > NUL
copy "Terminals.exe" "..\Distribution Debug\Terminals.exe" /Y > NUL
copy "plugin-container.exe" "..\Distribution Debug\plugin-container.exe" /Y > NUL
copy "Kohl.Explorer.exe" "..\Distribution Debug\Kohl.Explorer.exe" /Y > NUL
copy "omni.ja" "..\Distribution Debug\omni.ja" /Y > NUL

echo. Copying plugins
xcopy "Plugins" "..\Distribution Debug\Plugins" /E /C /R /I /K /Y > NUL
del /Q /F "..\Distribution Debug\Plugins\FlickrNet.dll"
del /Q /F "..\Distribution Debug\Plugins\Kohl.*"
del /Q /F "..\Distribution Debug\Plugins\log4net.dll"
del /Q /F "..\Distribution Debug\Plugins\Terminals.Connection.dll"
del /Q /F "..\Distribution Debug\Plugins\Terminals.Connection.pdb"
del /Q /F "..\Distribution Debug\Plugins\Terminals.Configuration.dll"

echo.
echo Done
echo.
rem pause
exit 0;