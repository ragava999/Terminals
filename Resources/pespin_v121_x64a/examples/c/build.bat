@echo off
set file=example1
c:\lcc\bin\lcc64.exe -O %file%.c
c:\lcc\bin\lcclnk64.exe -s -subsystem console %file%.obj
del *.obj
pause
cls