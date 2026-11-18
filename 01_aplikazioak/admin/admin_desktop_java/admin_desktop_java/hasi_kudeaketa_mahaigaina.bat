@echo off
setlocal

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
    echo Java 21 JDK ez da aurkitu. Instalatu JDK 21 edo jarri .jdks karpetan.
    pause
    exit /b 1
)

set "JAVA_HOME=%JAVA21_HOME%"
set "PATH=%JAVA_HOME%\bin;%PATH%"
echo Java 21 hautatua: %JAVA_HOME%
call "%~dp0mvnw.cmd" javafx:run
if errorlevel 1 pause
