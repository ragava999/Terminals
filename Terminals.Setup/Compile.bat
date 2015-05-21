rem Set the path to the APPDATA folder
powershell -Command "&{ Set-Content ((Get-Content -Path 'D:\Kohl\Projects\Code\Terminals\Terminals\bin\x86\Distribution Release\Out\Terminals.log4net.config').Replace('Logs\\Terminals.txt', '${APPDATA}\\Oliver Kohl D.Sc.\\Terminals\\Logs\\Terminals.txt')) -Path 'D:\Kohl\Projects\Code\Terminals\Terminals\bin\x86\Distribution Release\Out\Terminals.log4net.config' }"
"C:\Program Files (x86)\Inno Setup 5\iscc.exe" Terminals.iss

rem Set the path back to the application directory
powershell -Command "&{ Set-Content ((Get-Content -Path 'D:\Kohl\Projects\Code\Terminals\Terminals\bin\x86\Distribution Release\Out\Terminals.log4net.config').Replace('${APPDATA}\\Oliver Kohl D.Sc.\\Terminals\\Logs\\Terminals.txt', 'Logs\\Terminals.txt')) -Path 'D:\Kohl\Projects\Code\Terminals\Terminals\bin\x86\Distribution Release\Out\Terminals.log4net.config' }"
powershell ./Zip.ps1

copy *.exe /Y F:\
copy *.zip /Y F:\