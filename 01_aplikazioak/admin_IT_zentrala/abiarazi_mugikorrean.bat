@echo off
chcp 65001 >nul
title 1T Jatetxea - IT Kontrol Mugikorrean Irekitzen
color 0a

set ADB_PATH=D:\Android\Sdk\platform-tools\adb.exe
if not exist "%ADB_PATH%" set ADB_PATH=adb

echo [1/2] Port forwarding reverse ezartzen (5000 eta 5050 atakak)...
"%ADB_PATH%" reverse tcp:5000 tcp:5000
"%ADB_PATH%" reverse tcp:5050 tcp:5050

echo [2/2] 1T IT Kontrol aplikazioa mugikorrean irekitzen...
"%ADB_PATH%" shell am start -n eus.goierri.jatetxea.adminzentrala/.MainActivity
