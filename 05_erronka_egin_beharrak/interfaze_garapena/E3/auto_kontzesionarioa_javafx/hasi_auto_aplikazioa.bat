@echo off
chcp 65001 >nul
title Goierri Motors - Auto Kontzesionarioa (E3 JavaFX)
cd /d "%~dp0"

set "JAVA21_HOME="
for /d %%D in ("%USERPROFILE%\.jdks\jbr-21*" "%USERPROFILE%\.jdks\jdk-21*" "%USERPROFILE%\.jdks\ms-21*" "%USERPROFILE%\.jdks\temurin-21*") do (
    if exist "%%~fD\bin\java.exe" if exist "%%~fD\bin\javac.exe" set "JAVA21_HOME=%%~fD"
)
if not defined JAVA21_HOME (
    for /d %%D in ("C:\Program Files\Java\jdk-21*" "C:\Program Files\Eclipse Adoptium\jdk-21*" "C:\Program Files\Microsoft\jdk-21*") do (
        if exist "%%~fD\bin\java.exe" if exist "%%~fD\bin\javac.exe" set "JAVA21_HOME=%%~fD"
    )
)

if not defined JAVA21_HOME (
    echo [ERROREA] Java 21 JDK ez da aurkitu.
    pause
    exit /b 1
)

set "JAVA_HOME=%JAVA21_HOME%"
set "PATH=%JAVA_HOME%\bin;%PATH%"

if not exist "target\classes" (
    call "%~dp0mvnw.cmd" package -DskipTests
)

set "JAVA_BIN=%JAVA_HOME%\bin\javaw.exe"
if not exist "%JAVA_BIN%" set "JAVA_BIN=%JAVA_HOME%\bin\java.exe"

start "" "%JAVA_BIN%" --module-path "target\classes;target\lib" --add-modules ALL-MODULE-PATH --module eus.goierri.autoak/eus.goierri.autoak.Abiarazlea
