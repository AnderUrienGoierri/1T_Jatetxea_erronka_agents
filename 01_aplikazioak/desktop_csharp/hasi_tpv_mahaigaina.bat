@echo off
cd /d "%~dp0"

if exist "bin\Release\net10.0-windows\Jatetxea.Desktop.exe" (
    start "" "bin\Release\net10.0-windows\Jatetxea.Desktop.exe"
    exit /b 0
)

if exist "bin\Debug\net10.0-windows\Jatetxea.Desktop.exe" (
    start "" "bin\Debug\net10.0-windows\Jatetxea.Desktop.exe"
    exit /b 0
)

echo Visual Studio exekutablea konpilatzen...
dotnet build -c Release
if exist "bin\Release\net10.0-windows\Jatetxea.Desktop.exe" (
    start "" "bin\Release\net10.0-windows\Jatetxea.Desktop.exe"
    exit /b 0
)
