@echo off
echo ========================================================
echo Makina birtualak ixilpean (headless) abiarazten...
echo ========================================================
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0hasi_makinak_headless.ps1"
pause
