rem Set the path to the LOCALAPPDATA folder
powershell -Command "&{ Set-Content ((Get-Content -Path 'C:\KOHL\Terminals\Git\Terminals\Terminals\bin\x86\Distribution Release\Out\Terminals.log4net.config').Replace('Logs\\Terminals.txt', '${LOCALAPPDATA}\\Oliver Kohl D.Sc.\\Terminals\\Logs\\Terminals.txt')) -Path 'C:\KOHL\Terminals\Git\Terminals\Terminals\bin\x86\Distribution Release\Out\Terminals.log4net.config' }"

"C:\Program Files (x86)\Inno Setup 5\iscc.exe" Terminals.iss

rem Set the path back to the application directory
powershell -Command "&{ Set-Content ((Get-Content -Path 'C:\KOHL\Terminals\Git\Terminals\Terminals\bin\x86\Distribution Release\Out\Terminals.log4net.config').Replace('${LOCALAPPDATA}\\Oliver Kohl D.Sc.\\Terminals\\Logs\\Terminals.txt', 'Logs\\Terminals.txt')) -Path 'C:\KOHL\Terminals\Git\Terminals\Terminals\bin\x86\Distribution Release\Out\Terminals.log4net.config' }"
powershell ./Zip.ps1

exit /b 0