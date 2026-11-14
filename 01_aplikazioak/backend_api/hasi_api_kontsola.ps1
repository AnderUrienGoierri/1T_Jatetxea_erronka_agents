# 1T Jatetxea - ASP.NET Core REST API Kontsolan Abiarazlea
$host.UI.RawUI.WindowTitle = "1T Jatetxea - ASP.NET Core REST API (Portua 5050)"
$host.UI.RawUI.ForegroundColor = "White"
Write-Host "============================================================" -ForegroundColor White
Write-Host "  1T JATETXEA - ASP.NET CORE REST API (PORTUA: 5050)        " -ForegroundColor White
Write-Host "  MySQL Konexioa: localhost:3308 (jatetxea_db)              " -ForegroundColor White
Write-Host "  Logak eta Egoera Zuzenean                                 " -ForegroundColor White
Write-Host "============================================================" -ForegroundColor White
Write-Host ""
docker logs -f --tail 50 jatetxea_api
