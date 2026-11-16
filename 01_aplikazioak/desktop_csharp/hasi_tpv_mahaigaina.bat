@echo off
chcp 65001 >nul
title 1T Jatetxea - C# TPV Areto eta Kutxazaina (.NET 10)
cd /d "%~dp0"

echo ===============================================================
echo 1T JATETXEA - C# TPV ARETO ETA KUTXAZAINA ABIARAZTEN
echo ===============================================================
echo Proiektua: %~dp0
echo.

if exist "bin\Release\net10.0-windows\Jatetxea.Desktop.exe" (
    echo Aplikazioaren interfazea (Release) abiarazten...
    start "" "bin\Release\net10.0-windows\Jatetxea.Desktop.exe"
    exit /b 0
)

if exist "bin\Debug\net10.0-windows\Jatetxea.Desktop.exe" (
    echo Aplikazioaren interfazea (Debug) abiarazten...
    start "" "bin\Debug\net10.0-windows\Jatetxea.Desktop.exe"
    exit /b 0
)

echo Dotnet bidez konpilatzen eta abiarazten...
dotnet build -c Release
if exist "bin\Release\net10.0-windows\Jatetxea.Desktop.exe" (
    start "" "bin\Release\net10.0-windows\Jatetxea.Desktop.exe"
    exit /b 0
)

dotnet run -c Release
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROREA] C# TPV exekuzioak huts egin du: kodea %ERRORLEVEL%
    pause
)
