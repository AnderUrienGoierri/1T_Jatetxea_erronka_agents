# scripts/test-connectivity.ps1
# Entornoko zerbitzu guztien osasuna eta konexioa egiaztatzeko scripta

param(
    [string]$HostTarget = "127.0.0.1"
)

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "      1T JATETXEA - ZERBITZUEN OSASUNA ETA FROGAK         " -ForegroundColor Yellow
Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "Helburu IP: $HostTarget" -ForegroundColor Gray
Write-Host ""

$tests = @(
    @{ Name = "ASP.NET Core API Health"; Url = "http://${HostTarget}:5000/health"; ExpectedCode = 200 },
    @{ Name = "REST API Platerak Endpoint"; Url = "http://${HostTarget}:5000/api/v1/platerak"; ExpectedCode = 200 },
    @{ Name = "Web PHP Frontend"; Url = "http://${HostTarget}:8085/"; ExpectedCode = 200 },
    @{ Name = "Barne Txat Zerbitzaria"; Url = "http://${HostTarget}:3001/health"; ExpectedCode = 200 },
    @{ Name = "Sukaldea KDS Android TV"; Url = "http://${HostTarget}:8086/"; ExpectedCode = 200 },
    @{ Name = "Odoo 17 ERP Hasiera"; Url = "http://${HostTarget}:8069/web/login"; ExpectedCode = 200 }
)

foreach ($t in $tests) {
    try {
        $res = Invoke-WebRequest -Uri $t.Url -TimeoutSec 3 -UseBasicParsing -ErrorAction Stop
        if ($res.StatusCode -eq $t.ExpectedCode) {
            Write-Host "  [PASS] $($t.Name) -> $($res.StatusCode) OK" -ForegroundColor Green
        } else {
            Write-Host "  [WARN] $($t.Name) -> $($res.StatusCode) (Espero zena: $($t.ExpectedCode))" -ForegroundColor Yellow
        }
    } catch {
        Write-Host "  [FAIL] $($t.Name) -> Ezin izan da konektatu ($($_.Exception.Message))" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "Proba guztiak burututa." -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan
