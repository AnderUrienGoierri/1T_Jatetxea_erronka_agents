# scripts/start-all.ps1
# Docker edukiontzi guztiak altxatzeko scripta

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RootDir = Split-Path -Parent $ScriptDir

Set-Location $RootDir

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "         1T JATETXEA - DOCKER ENTORNOA ABIARAZTEN         " -ForegroundColor Yellow
Write-Host "==========================================================" -ForegroundColor Cyan

# Egiaztatu Docker daemon martxan dagoen
docker info > $null 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Warning "Docker Desktop ez dago martxan! Mesedez ireki Docker Desktop aplikazioa lehenik."
    exit 1
}

Write-Host "Docker Compose martxan jartzen..." -ForegroundColor Green
docker compose up -d --build

Write-Host ""
Write-Host "Edukiontzien egoera:" -ForegroundColor Cyan
docker compose ps

Write-Host ""
Write-Host "Sarbide puntuak:" -ForegroundColor Yellow
Write-Host "  - MySQL Datu-Basea:     localhost:3308 (root / root)"
Write-Host "  - ASP.NET Core API:     http://localhost:5000/api/v1 (eta /health)"
Write-Host "  - Web PHP Frontend:     http://localhost:8085"
Write-Host "  - Barne Txat Zerbitzua: http://localhost:3001"
Write-Host "  - Sukaldea KDS TV:      http://localhost:8086"
Write-Host "  - Odoo 17 ERP:          http://localhost:8069"
Write-Host "==========================================================" -ForegroundColor Cyan
