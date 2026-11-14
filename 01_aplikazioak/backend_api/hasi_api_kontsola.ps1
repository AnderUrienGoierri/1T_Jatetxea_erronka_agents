# 1T Jatetxea - ASP.NET Core REST API Kontsolan Abiarazlea
$host.UI.RawUI.WindowTitle = "1T Jatetxea - ASP.NET Core REST API (Portua 5050)"
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host "  1T JATETXEA - ASP.NET CORE REST API (PORTUA: 5050)        " -ForegroundColor Yellow
Write-Host "  MySQL Konexioa: localhost:3308 (jatetxea_db)              " -ForegroundColor Cyan
Write-Host "  Logak eta Egoera Zuzenean                                 " -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Cyan
Write-Host ""
docker logs -f --tail 50 jatetxea_api
