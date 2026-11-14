@echo off
title 1T Jatetxea - ASP.NET Core REST API (Portua 5050)
color 0a
cd /d "%~dp0"
echo ============================================================
echo   1T JATETXEA - ASP.NET CORE REST API (PORTUA: 5050)
echo   MySQL Konexioa: localhost:3308 (jatetxea_db)
echo ============================================================
echo.
dotnet run --launch-profile http
pause
