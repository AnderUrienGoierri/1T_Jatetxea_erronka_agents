# =====================================================================
# 1T Jatetxea - Odoo 17 ERP Abiarazte Scripta (Portu 8070)
# =====================================================================
$ErrorActionPreference = "Stop"

Write-Host "1T Jatetxea Odoo 17 eta PostgreSQL edukiontziak abiarazten..." -ForegroundColor Cyan
Set-Location -Path (Split-Path -Parent $PSScriptRoot)

docker compose up -d

Write-Host ""
Write-Host "Egoera egiaztatzen..." -ForegroundColor Yellow
docker compose ps

Write-Host ""
Write-Host "Odoo 17 ERP eskuragarri hemen: http://localhost:8070" -ForegroundColor Green
Write-Host "Datu-basearen kudeatzailea:     http://localhost:8070/web/database/manager" -ForegroundColor Green
Write-Host "Admin Pasahitza Nagusia:        Errotagain169232$$" -ForegroundColor Yellow
