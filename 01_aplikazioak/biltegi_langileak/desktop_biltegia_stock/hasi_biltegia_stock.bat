@echo off
chcp 65001 >nul
title 1T Jatetxea - Biltegiko Stock Kudeaketa (JavaFX)
cd /d "%~dp0"

echo ===============================================================
echo 1T JATETXEA - BILTEGIKO STOCK TERMINALA ABIARAZTEN
echo ===============================================================
echo Proiektua: %~dp0
echo Modulua: Biltegia (Stock Management)
echo.

set "JAVA_BIN=javaw"
if exist "C:\Program Files\Eclipse Adoptium\jre-17.0.21.9-hotspot\bin\javaw.exe" set "JAVA_BIN=C:\Program Files\Eclipse Adoptium\jre-17.0.21.9-hotspot\bin\javaw.exe"
if exist "C:\Program Files\Eclipse Adoptium\jdk-17.0.21.9-hotspot\bin\javaw.exe" set "JAVA_BIN=C:\Program Files\Eclipse Adoptium\jdk-17.0.21.9-hotspot\bin\javaw.exe"
if exist "%USERPROFILE%\.jdks\ms-17.0.21.0.2\bin\javaw.exe" set "JAVA_BIN=%USERPROFILE%\.jdks\ms-17.0.21.0.2\bin\javaw.exe"
if defined JAVA_HOME if exist "%JAVA_HOME%\bin\javaw.exe" set "JAVA_BIN=%JAVA_HOME%\bin\javaw.exe"

if exist "target\classes" if exist "target\lib\javafx-controls-21.0.2-win.jar" (
    echo Aplikazioaren interfaze grafikoa natiboki abiarazten...
    "%JAVA_BIN%" --module-path "target\classes;target\lib" --add-modules ALL-MODULE-PATH --module eus.goierri.jatetxea.biltegia/eus.goierri.jatetxea.biltegia.Launcher
    exit /b 0
)

echo Maven Wrapper bidez konpilatzen eta abiarazten...
call .\mvnw.cmd javafx:run
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROREA] JavaFX exekuzioak huts egin du: kodea %ERRORLEVEL%
    pause
)
