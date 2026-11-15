@echo off
chcp 65001 >nul
title 1T Jatetxea - ASP.NET Core REST API (Portua 5050)
color 0f
cd /d "%~dp0"
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0hasi_api_kontsola.ps1"
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo Kontsola ixteko sakatu edozein tekla...
    pause >nul
)
