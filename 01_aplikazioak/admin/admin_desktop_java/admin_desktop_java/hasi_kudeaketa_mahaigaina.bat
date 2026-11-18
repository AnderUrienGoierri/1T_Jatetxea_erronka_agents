@echo off
chcp 65001 >nul
title 1T Jatetxea - Kudeaketa Mahaigaina (JavaFX)
cd /d "%~dp0"

echo ===============================================================
echo 1T JATETXEA - ADMIN KUDEAKETA MAHAIGAINA (JAVAFX)
echo ===============================================================
echo Proiektua: %~dp0
echo Modulua: Admin Kudeaketa Zentrala (JavaFX 21)
echo.

set "JAVA21_HOME="
for /d %%D in ("%USERPROFILE%\.jdks\jbr-21*" "%USERPROFILE%\.jdks\jdk-21*" "%USERPROFILE%\.jdks\ms-21*" "%USERPROFILE%\.jdks\temurin-21*") do (
    if exist "%%~fD\bin\java.exe" if exist "%%~fD\bin\javac.exe" set "JAVA21_HOME=%%~fD"
)
if not defined JAVA21_HOME (
    for /d %%D in ("C:\Program Files\Java\jdk-21*" "C:\Program Files\Eclipse Adoptium\jdk-21*" "C:\Program Files\Microsoft\jdk-21*" "C:\Program Files\Eclipse Adoptium\jbr-21*") do (
        if exist "%%~fD\bin\java.exe" if exist "%%~fD\bin\javac.exe" set "JAVA21_HOME=%%~fD"
    )
)

if not defined JAVA21_HOME (
    echo [ERROREA] Java 21 JDK ez da aurkitu. Instalatu JDK 21 edo jarri .jdks karpetan.
    pause
    exit /b 1
)

set "JAVA_HOME=%JAVA21_HOME%"
set "PATH=%JAVA_HOME%\bin;%PATH%"
echo Java 21 hautatua: %JAVA_HOME%

set "JAVA_BIN=%JAVA_HOME%\bin\javaw.exe"
if not exist "%JAVA_BIN%" set "JAVA_BIN=%JAVA_HOME%\bin\java.exe"

if exist "target\classes" if exist "target\lib\javafx-controls-21.0.6-win.jar" (
    echo Aplikazioaren interfaze grafikoa natiboki abiarazten...
    start "" "%JAVA_BIN%" --module-path "target\classes;target\lib" --add-modules ALL-MODULE-PATH --module eus.goierri.jatetxea.admin/eus.goierri.jatetxea.admin.Abiarazlea
    exit /b 0
)

echo Maven bidez paketatu eta mendekotasunak prestatzen...
call "%~dp0mvnw.cmd" clean package -DskipTests
if exist "target\classes" if exist "target\lib\javafx-controls-21.0.6-win.jar" (
    start "" "%JAVA_BIN%" --module-path "target\classes;target\lib" --add-modules ALL-MODULE-PATH --module eus.goierri.jatetxea.admin/eus.goierri.jatetxea.admin.Abiarazlea
    exit /b 0
)

echo [OHARRA] Maven javafx:run bidez abiarazten...
call "%~dp0mvnw.cmd" javafx:run
if errorlevel 1 (
    echo.
    echo [ERROREA] JavaFX exekuzioak huts egin du: kodea %ERRORLEVEL%
    pause
)
