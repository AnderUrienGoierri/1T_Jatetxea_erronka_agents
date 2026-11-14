# =====================================================================
# 1T JATETXEA - IT KONTROL ZENTRALA (POWERSHELL ABIARAZLEA)
# Administratzaileak: AnderU, Julen, Iker, Aratz
# =====================================================================

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ServerDir = Join-Path $ScriptDir "server\Jatetxea.AdminZentrala"

Write-Host "=====================================================================" -ForegroundColor Cyan
Write-Host "          1T JATETXEA - IT KONTROL ZENTRALA (ADMIN HUB)             " -ForegroundColor Yellow
Write-Host "          Goierri Eskola - Zibersegurtasuna eta Sareak              " -ForegroundColor Cyan
Write-Host "=====================================================================" -ForegroundColor Cyan

Set-Location $ServerDir

# IP Lokala lortu mugikorretik sartzeko
$LocalIP = (Get-NetIPAddress -AddressFamily IPv4 | Where-Object { $_.InterfaceAlias -notmatch "vEthernet|Loopback|VirtualBox" -and $_.IPAddress -notlike "169.254*" } | Select-Object -First 1).IPAddress

Write-Host "Zerbitzaria abiarazten..." -ForegroundColor Green
Write-Host "  - Lokalean (Desktop): http://localhost:5000" -ForegroundColor White
if ($LocalIP) {
    Write-Host "  - Mugikor edo Tabletatik (WiFi): http://${LocalIP}:5000" -ForegroundColor White
}

Start-Process "dotnet" -ArgumentList "run --urls http://0.0.0.0:5000" -WorkingDirectory $ServerDir
Start-Sleep -Seconds 3

# Nabigatzailea ireki
Start-Process "http://localhost:5000"
