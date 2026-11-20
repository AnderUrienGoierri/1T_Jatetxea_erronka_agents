@echo off
chcp 65001 >nul
title 1T Jatetxea - Sukaldeko KDS Android TV Instalatzen
color 0b

echo ================================================================================
echo   1T JATETXEA - SUKALDEKO KDS ANDROID TV PANTILA
echo   APK Aplikazioa Android TV gailuan edo emuladorean instalatzen...
echo ================================================================================
echo.

set ADB_PATH=D:\Android\Sdk\platform-tools\adb.exe
if not exist "%ADB_PATH%" set ADB_PATH=adb

echo [1/3] Konektatutako Android TV / gailuak bilatzen...
"%ADB_PATH%" devices -l
echo.

echo [2/3] Port forwarding ezartzen (localhost:5050 REST API ^<^> TV:5050)...
"%ADB_PATH%" reverse tcp:5050 tcp:5050
echo.

echo [3/3] APK fitxategia instalatzen...
set APK_PATH=%~dp0app\build\outputs\apk\debug\app-debug.apk
if exist "%APK_PATH%" (
    "%ADB_PATH%" install -r -d "%APK_PATH%"
    if %ERRORLEVEL% EQU 0 (
        echo.
        echo [ONDO] Aplikazioa ondo instalatu da Android TV-n!
        echo Aplikazioa telebistan / gailuan abiarazten...
        "%ADB_PATH%" shell am start -n eus.goierri.jatetxea.sukaldea/.ui.MainActivity
    ) else (
        echo.
        echo [KONTUZ] Ezin izan da APK-a instalatu.
        echo Ziurtatu zure Android TV piztuta eta USB/WiFi bidez ADB-ra konektatuta dagoela.
        echo WiFi bidez bada: "%ADB_PATH% connect <TV_IP_HELBIDEA>:5555"
    )
) else (
    echo [ERROREA] APK fitxategia ez da aurkitu: %APK_PATH%
)

echo.
echo Sakatu edozein tekla leiho hau ixteko...
pause >nul
