@echo off
chcp 65001 >nul
title 1T Jatetxea - ASP.NET Core REST API (Portua 5050)
cd /d "%~dp0"
where pwsh.exe >nul 2>nul
if %ERRORLEVEL% EQU 0 (
    start "1T Jatetxea - ASP.NET Core REST API" pwsh.exe -NoExit -ExecutionPolicy Bypass -File "%~dp0hasi_api_kontsola.ps1"
    exit /b 0
)
start "1T Jatetxea - ASP.NET Core REST API" powershell.exe -NoExit -ExecutionPolicy Bypass -File "%~dp0hasi_api_kontsola.ps1"
exit /b 0
