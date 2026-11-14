# scripts/stop-all.ps1
# Docker edukiontzi guztiak gelditzeko scripta

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RootDir = Split-Path -Parent $ScriptDir

Set-Location $RootDir

Write-Host "Jatetxeko Docker edukiontziak gelditzen..." -ForegroundColor Yellow
docker compose down

Write-Host "Edukiontzi guztiak ondo gelditu dira." -ForegroundColor Green
