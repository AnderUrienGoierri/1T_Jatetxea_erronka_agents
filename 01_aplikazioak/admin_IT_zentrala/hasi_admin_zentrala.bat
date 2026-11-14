@echo off
title 1T Jatetxea - IT Kontrol Zentrala
color 0b
echo =====================================================================
echo           1T JATETXEA - IT KONTROL ZENTRALA (ADMIN HUB)
echo           Goierri Eskola - Zibersegurtasuna eta Sareak
echo           Administratzaileak: AnderU, Julen, Iker, Aratz
echo =====================================================================
echo.
echo [1/2] IT Kontrol Zerbitzaria prestatzen (Portua: 5000)...
cd /d "%~dp0server\Jatetxea.AdminZentrala"

start "1T Jatetxea IT Server" dotnet run --urls "http://0.0.0.0:5000"

echo [2/2] Web Panela irekitzen nabigatzailean...
timeout /t 3 /nobreak > nul
start http://localhost:5000

echo.
echo =====================================================================
echo IT KONTROL ZENTRALA MARTXAN DAGO!
echo - Ordenagailu honetan:  http://localhost:5000
echo - Mugikor edo Tabletatik (WiFi): http://[ZURE-IP-A]:5000
echo =====================================================================
echo.
pause
