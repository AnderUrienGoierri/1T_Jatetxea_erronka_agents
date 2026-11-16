@echo off
chcp 65001 >nul
title 1T Jatetxea - C# TPV Areto eta Kutxazaina
cd /d "%~dp0"
echo ===============================================================
echo 1T JATETXEA - C# TPV ARETOA ETA KUTXAZAINA ABIARAZTEN
echo ===============================================================

set "EXE_PATH=%~dp0bin\Release\net10.0-windows\Jatetxea.Desktop.exe"
if not exist "%EXE_PATH%" (
    set "EXE_PATH=%~dp0bin\Debug\net10.0-windows\Jatetxea.Desktop.exe"
)

if not exist "%EXE_PATH%" (
    echo [INFO] Exekutablea ez da aurkitu. dotnet build exekutatzen...
    dotnet build Jatetxea.Desktop.csproj -c Release
    set "EXE_PATH=%~dp0bin\Release\net10.0-windows\Jatetxea.Desktop.exe"
)

if exist "%EXE_PATH%" (
    echo [INFO] Exekutatzen: %EXE_PATH%
    start "1T Jatetxea - C# TPV" "%EXE_PATH%"
) else (
    echo [ERROREA] Ezin izan da Jatetxea.Desktop.exe sortu edo aurkitu.
    pause
)
