@echo off
chcp 65001 >nul
title 1T Jatetxea - C# TPV Areto eta Kutxazaina (.NET 10)
cd /d "%~dp0"

echo ===============================================================
echo 1T JATETXEA - C# TPV ARETO ETA KUTXAZAINA ABIARAZTEN
echo ===============================================================
echo Proiektua: %~dp0
echo.

set "EXE_RELEASE=%~dp0bin\Release\net10.0-windows\Jatetxea.Desktop.exe"
set "EXE_DEBUG=%~dp0bin\Debug\net10.0-windows\Jatetxea.Desktop.exe"

if exist "%EXE_RELEASE%" (
    echo Aplikazioaren interfazea abiarazten: Release
    "%EXE_RELEASE%"
    goto :amaitu
)

if exist "%EXE_DEBUG%" (
    echo Aplikazioaren interfazea abiarazten: Debug
    "%EXE_DEBUG%"
    goto :amaitu
)

echo Dotnet bidez konpilatzen eta abiarazten...
dotnet build "%~dp0Jatetxea.Desktop.csproj" -c Release
if exist "%EXE_RELEASE%" (
    start "" "%EXE_RELEASE%"
    goto :amaitu
)

dotnet run --project "%~dp0Jatetxea.Desktop.csproj" -c Release
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROREA] C# TPV exekuzioak huts egin du: kodea %ERRORLEVEL%
    pause
)

:amaitu
exit /b 0
