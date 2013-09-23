@echo off
set DEST=deploy
mkdir %DEST%

:hooklib
set SRC=Release
copy %SRC%\WinHookLib.dll  %DEST%\WinHookLib32.dll

:app
set SRC=WindowLock\bin\Release
copy %SRC%\WindowLock.exe  %DEST%\WindowLock32.exe

:end
pause