@echo off

"c:\WinDDK\6000\bin\x86\amd64\ml64.exe" "example1.asm" /link /subsystem:windows /entry:start

if errorlevel 1 goto End
   
   
   del "mllink$.lnk"
   del "example1.obj"

:End
 pause
 cls