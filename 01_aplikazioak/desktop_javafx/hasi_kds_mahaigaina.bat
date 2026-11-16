@echo off
chcp 65001 >nul
title 1T Jatetxea - JavaFX Sukaldea KDS
cd /d "%~dp0"
echo ===============================================================
echo 1T JATETXEA - JAVAFX SUKALDEA KDS TERMINALA ABIARAZTEN
echo ===============================================================
echo Proiektua: %~dp0
echo JavaFX Maven Plugin bidez aplikazio natiboa abiarazten...
echo.
call .\mvnw.cmd javafx:run
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROREA] JavaFX exekuzioak huts egin du: kodea %ERRORLEVEL%
    pause
)
