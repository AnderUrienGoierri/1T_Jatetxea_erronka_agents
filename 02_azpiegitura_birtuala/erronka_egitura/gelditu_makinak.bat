@echo off
echo ========================================================
echo Makina birtualak modu seguruan itzaltzen...
echo ========================================================
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0gelditu_makinak.ps1"
pause
