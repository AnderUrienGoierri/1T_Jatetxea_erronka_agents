@echo off
setlocal
title 1T Jatetxea - IT Kontrol Zentrala
color 0b

for %%I in ("%~dp0..\..\..") do set "PROJECT_ROOT=%%~fI"
set "LAUNCHER=%PROJECT_ROOT%\Admin_IT_Zentrala.exe"

if not exist "%LAUNCHER%" (
    echo Errorea: Admin IT Zentrala abiarazlea ez da aurkitu:
    echo "%LAUNCHER%"
    pause
    exit /b 1
)

start "" "%LAUNCHER%"
exit /b
