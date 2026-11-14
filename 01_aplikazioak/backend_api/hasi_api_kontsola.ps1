# 1T Jatetxea - ASP.NET Core REST API Kontsolan Abiarazlea
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $ScriptDir
$host.UI.RawUI.WindowTitle = "1T Jatetxea - ASP.NET Core REST API (Portua 5050)"
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host "  1T JATETXEA - ASP.NET CORE REST API (PORTUA: 5050)        " -ForegroundColor Yellow
Write-Host "  MySQL Konexioa: localhost:3308 (jatetxea_db)              " -ForegroundColor Cyan
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""
dotnet run --launch-profile http
