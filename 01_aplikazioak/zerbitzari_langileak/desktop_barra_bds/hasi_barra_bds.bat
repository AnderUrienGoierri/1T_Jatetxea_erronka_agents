@echo off
chcp 65001 >nul
title 1T Jatetxea - Barrako BDS Pantaila (JavaFX)
cd /d "%~dp0"

echo ===============================================================
echo 1T JATETXEA - BARRAKO BDS TERMINALA ABIARAZTEN
echo ===============================================================
echo Proiektua: %~dp0
echo Modulua: Barra (Bar Display System)
echo.

set "JAVA_BIN=javaw"
if exist "C:\Program Files\Eclipse Adoptium\jre-17.0.21.9-hotspot\bin\javaw.exe" set "JAVA_BIN=C:\Program Files\Eclipse Adoptium\jre-17.0.21.9-hotspot\bin\javaw.exe"
if exist "C:\Program Files\Eclipse Adoptium\jdk-17.0.21.9-hotspot\bin\javaw.exe" set "JAVA_BIN=C:\Program Files\Eclipse Adoptium\jdk-17.0.21.9-hotspot\bin\javaw.exe"
if exist "%USERPROFILE%\.jdks\ms-17.0.21.0.2\bin\javaw.exe" set "JAVA_BIN=%USERPROFILE%\.jdks\ms-17.0.21.0.2\bin\javaw.exe"
if defined JAVA_HOME if exist "%JAVA_HOME%\bin\javaw.exe" set "JAVA_BIN=%JAVA_HOME%\bin\javaw.exe"

if exist "target\classes" if exist "target\lib\javafx-controls-21.0.2-win.jar" if exist "target\lib\desktop-javafx-core-1.0.0.jar" (
    echo Aplikazioaren interfaze grafikoa natiboki abiarazten...
    start "" "%JAVA_BIN%" --module-path "target\classes;target\lib" --add-modules ALL-MODULE-PATH --module eus.goierri.jatetxea.barra/eus.goierri.jatetxea.barra.Launcher
    exit /b 0
)

if not exist "%USERPROFILE%\.m2\repository\eus\goierri\jatetxea\desktop-javafx-core\1.0.0\desktop-javafx-core-1.0.0.jar" (
    echo [INFO] JavaFX Core liburutegi komuna konpilatzen eta instalatzen...
    pushd "..\..\admin\komunak\desktop_javafx_parent"
    call .\mvnw.cmd install -DskipTests -pl :desktop-javafx-core -am
    popd
)

echo Maven Wrapper bidez konpilatzen eta abiarazten...
call .\mvnw.cmd javafx:run
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROREA] JavaFX exekuzioak huts egin du: kodea %ERRORLEVEL%
    pause
)
