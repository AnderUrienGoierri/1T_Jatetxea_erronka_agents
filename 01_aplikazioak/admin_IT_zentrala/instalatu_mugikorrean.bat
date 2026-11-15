@echo off
chcp 65001 >nul
title 1T Jatetxea - IT Kontrol Mugikorra Instalatzen
color 0b

echo ================================================================================
echo   1T JATETXEA - IT KONTROL ZENTRALA (ANDROID MUGIKORRA)
echo   APK Aplikazioa gailuan instalatzen eta konfiguratzen...
echo ================================================================================
echo.

set ADB_PATH=D:\Android\Sdk\platform-tools\adb.exe
if not exist "%ADB_PATH%" set ADB_PATH=adb

echo [1/3] Konektatutako Android gailuak bilatzen...
"%ADB_PATH%" devices
echo.

echo [2/3] Port forwarding reverse ezartzen (localhost:5000 ^<^> Android:5000)...
"%ADB_PATH%" reverse tcp:5000 tcp:5000
"%ADB_PATH%" reverse tcp:5050 tcp:5050
echo.

echo [3/3] APK fitxategia mugikorrean instalatzen...
set APK_PATH=%~dp0mobile_android\app\build\outputs\apk\debug\app-debug.apk
if exist "%APK_PATH%" (
    "%ADB_PATH%" install -r -d "%APK_PATH%"
    if %ERRORLEVEL% EQU 0 (
        echo.
        echo [ONDO] Aplikazioa ondo instalatu da mugikorrean!
        echo Aplikazioa gailuan abiarazten...
        "%ADB_PATH%" shell am start -n eus.goierri.jatetxea.adminzentrala/.MainActivity
    ) else (
        echo.
        echo [ERROREA] Ezin izan da APK-a instalatu. Ziurtatu mugikorra desblokeatuta dagoela.
    )
) else (
    echo [ERROREA] APK fitxategia ez da aurkitu: %APK_PATH%
)

echo.
echo Sakatu edozein tekla leiho hau ixteko...
pause >nul
