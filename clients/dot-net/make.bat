@ECHO OFF

REM Set command switch for building debug or retail (default is to build debug)
REM Type "build.bat -r" to build for retail

SET DEBUG=/debug+ /optimize-
IF "%1"=="-r" SET DEBUG=/debug- /optimize+
IF "%1"=="-R" SET DEBUG=/debug- /optimize+
SET RES=/res:train.gif /res:jail.jpg /res:bulb.png /res:car.png /res:water.gif /res:police.jpg /res:monopoly.ico /res:pieces.png /res:pieces-16.png /res:hotel.ico /res:house.ico
SET COMMON=/nologo %DEBUG% /warn:4 /warnaserror+

csc.exe %COMMON% /t:winexe %RES% /win32icon:monopoly.ico message.cs pimp-messages.cs network.cs board.cs monopoly.cs floating-forms.cs state.cs dialogs.cs houses.cs log.cs
IF ERRORLEVEL 1 goto error

:done
start monopoly.exe
goto end

:error
echo aborted
goto end

:end
