# scripts/switch-env.ps1
# Jatetxearen Sare Zentralita: WiFi Sareak (Eskola, Etxean, Tolosa, Localhost, VBox) aldatzeko scripta

param(
    [Alias("Environment", "Env", "Profile")]
    [string]$TargetProfile = ""
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$RootDir = Split-Path -Parent $ScriptDir
$AgentsDir = "C:\Dev\GoierriEskola\1T_Jatetxea_erronka_agents"
if (-not (Test-Path $AgentsDir)) {
    $AgentsDir = (Get-Item $RootDir).Parent.Parent.FullName
}
$AppsDir = Join-Path $AgentsDir "01_aplikazioak"
if (-not (Test-Path $AppsDir)) {
    $AppsDir = $AgentsDir
}

$ConfigFile = Join-Path $RootDir "config\environments.json"
if (-not (Test-Path $ConfigFile)) {
    $ConfigFile = "C:\Dev\GoierriEskola\1T_Jatetxea_erronka_agents\02_azpiegitura_birtuala\1T_Jatetxea_Entorno_Birtuala\config\environments.json"
}

if (-not (Test-Path $ConfigFile)) {
    Write-Error "Ez da aurkitu konfigurazio fitxategia: $ConfigFile"
    exit 1
}

$Config = Get-Content $ConfigFile -Raw -Encoding utf8 | ConvertFrom-Json

# Detektatu uneko makinan dauden sare egokitzaileen IP-ak
$DetectedIPs = Get-NetIPAddress -AddressFamily IPv4 -ErrorAction SilentlyContinue | 
    Where-Object { $_.IPAddress -notmatch "^127\." -and $_.IPAddress -notmatch "^169\.254\." } | 
    Select-Object -Property IPAddress, InterfaceAlias

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "       1T JATETXEA - SARE ZENTRALITA ETA PROFILAK         " -ForegroundColor Yellow
Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Uneko ordenagailuan detektatutako sare helbideak:" -ForegroundColor Gray
foreach ($ip in $DetectedIPs) {
    Write-Host "  - $($ip.InterfaceAlias): $($ip.IPAddress)" -ForegroundColor White
}
Write-Host ""

if (-not $TargetProfile) {
    Write-Host "Aukeratu sare profila:" -ForegroundColor Green
    Write-Host "  1) localhost        -> 127.0.0.1 (Tokiko Docker bakarra)"
    Write-Host "  2) etxean           -> Etxeko WiFi Lokala (euskaltel_urite)"
    Write-Host "  3) innovatek        -> Innovatek Sarea (Kablez / INN-WPRIVATE)"
    Write-Host "  4) tolosako_etxea   -> Tolosako etxeko WiFi-a"
    Write-Host "  5) eskola           -> Goierri Eskolako laborategia"
    Write-Host "  6) vbox_erronka     -> VirtualBox 3 Makina (192.168.10.10)"
    Write-Host "  7) Pertsonalizatua  -> Idatzi IP helbide bat eskuz"
    Write-Host ""
    $opt = Read-Host "Aukeratu zenbaki bat (1-7) [Lehenetsia: 1]"
    switch ($opt) {
        "2" { $TargetProfile = "etxean" }
        "3" { $TargetProfile = "innovatek" }
        "4" { $TargetProfile = "tolosako_etxea" }
        "5" { $TargetProfile = "eskola" }
        "6" { $TargetProfile = "vbox_erronka" }
        "7" { $TargetProfile = "custom" }
        default { $TargetProfile = "localhost" }
    }
}

$SelectedIp = ""
$ApiBaseUrl = ""

if ($TargetProfile -eq "custom") {
    $SelectedIp = Read-Host "Idatzi erabili nahi duzun zerbitzariaren IP helbidea (adibidez 192.168.1.129)"
    $ApiBaseUrl = "http://${SelectedIp}:5050/api/v1"
} else {
    $p = $Config.profiles.$TargetProfile
    if (-not $p) {
        Write-Error "Profil ezezaguna: $TargetProfile"
        exit 1
    }
    $SelectedIp = $p.api_host
    $ApiBaseUrl = $p.api_base_url
}

Write-Host ""
Write-Host "Aplikazio guztiak helbide honetara sinkronizatzen: $ApiBaseUrl ..." -ForegroundColor Cyan

# 1. Konfigurazio orokorra gorde
$Config.active_profile = $TargetProfile
$Config | ConvertTo-Json -Depth 5 | Set-Content $ConfigFile -Encoding utf8

# 2. Flutter: mobile_zerbitzaria_flutter eguneratu
$FlutterZerbFile = Join-Path $AppsDir "zerbitzari_langileak\mobile_zerbitzaria_flutter\lib\services\api_service.dart"
if (-not (Test-Path $FlutterZerbFile)) {
    $FlutterZerbFile = Join-Path $AppsDir "mobile_zerbitzaria_flutter\lib\services\api_service.dart"
}
if (Test-Path $FlutterZerbFile) {
    $content = Get-Content $FlutterZerbFile -Raw -Encoding utf8
    $content = $content -replace "static const String _defaultUrl = '.*?';", "static const String _defaultUrl = '$ApiBaseUrl';"
    $content | Set-Content $FlutterZerbFile -Encoding utf8
    Write-Host "  [OK] Flutter Zerbitzaria eguneratua: $FlutterZerbFile" -ForegroundColor Green
}

# 3. Flutter: mobile_bezeroa_flutter eguneratu
$FlutterBezeroFile = Join-Path $AppsDir "bezeroak\mobile_bezeroa_flutter\lib\services\api_service.dart"
if (-not (Test-Path $FlutterBezeroFile)) {
    $FlutterBezeroFile = Join-Path $AppsDir "mobile_bezeroa_flutter\lib\services\api_service.dart"
}
if (Test-Path $FlutterBezeroFile) {
    $content = Get-Content $FlutterBezeroFile -Raw -Encoding utf8
    $content = $content -replace "static const String _defaultUrl = '.*?';", "static const String _defaultUrl = '$ApiBaseUrl';"
    $content | Set-Content $FlutterBezeroFile -Encoding utf8
    Write-Host "  [OK] Flutter Bezeroa eguneratua: $FlutterBezeroFile" -ForegroundColor Green
}

# 4. JavaFX Desktop moduluak (Sukaldea, Barra, Biltegia eta Core)
$JavaFxDirs = @(
    (Join-Path $AppsDir "komunak\desktop_javafx_core\src\main\resources"),
    (Join-Path $AppsDir "sukalde_langileak\desktop_sukaldea_kds\src\main\resources"),
    (Join-Path $AppsDir "zerbitzari_langileak\desktop_barra_bds\src\main\resources"),
    (Join-Path $AppsDir "biltegi_langileak\desktop_biltegia_stock\src\main\resources")
)
foreach ($dir in $JavaFxDirs) {
    if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
    $propFile = Join-Path $dir "api.properties"
    "api.base.url=$ApiBaseUrl" | Set-Content $propFile -Encoding utf8
}
Write-Host "  [OK] JavaFX Desktop Moduluak eguneratuta (Core, Sukaldea, Barra, Biltegia)" -ForegroundColor Green

# 5. C# TPV: desktop_tpv_csharp konfigurazioa
$CsharpAppConfig = Join-Path $AppsDir "zerbitzari_langileak\desktop_tpv_csharp\appsettings.json"
if (-not (Test-Path $CsharpAppConfig)) {
    $legacyConfig = Join-Path $AppsDir "desktop_csharp\appsettings.json"
    if (Test-Path $legacyConfig) { $CsharpAppConfig = $legacyConfig }
}
if (Test-Path $CsharpAppConfig) {
    try {
        $csObj = Get-Content $CsharpAppConfig -Raw -Encoding utf8 | ConvertFrom-Json
        $csObj.ApiBaseUrl = $ApiBaseUrl
        $csObj | ConvertTo-Json -Depth 5 | Set-Content $CsharpAppConfig -Encoding utf8
    } catch {
        @{ "ApiBaseUrl" = $ApiBaseUrl } | ConvertTo-Json | Set-Content $CsharpAppConfig -Encoding utf8
    }
} else {
    $parentDir = Split-Path $CsharpAppConfig
    if (-not (Test-Path $parentDir)) { New-Item -ItemType Directory -Path $parentDir -Force | Out-Null }
    @{ "ApiBaseUrl" = $ApiBaseUrl } | ConvertTo-Json | Set-Content $CsharpAppConfig -Encoding utf8
}
Write-Host "  [OK] C# Desktop TPV eguneratua: $CsharpAppConfig" -ForegroundColor Green

# 6. Web PHP: web_php .env fitxategia
$WebEnvFile = Join-Path $AppsDir "bezeroak\web_php_bezeroak\.env"
if (-not (Test-Path $WebEnvFile)) {
    $WebEnvFile = Join-Path $AppsDir "web_php\.env"
}
"API_BASE_URL=$ApiBaseUrl" | Set-Content $WebEnvFile -Encoding utf8
Write-Host "  [OK] Web PHP Frontend eguneratua: $WebEnvFile" -ForegroundColor Green

Write-Host ""
Write-Host "Sinkronizazioa osatuta!" -ForegroundColor Green
Write-Host "Uneko Profil Aktiboa: $TargetProfile" -ForegroundColor Yellow
Write-Host "REST API Helbidea:    $ApiBaseUrl" -ForegroundColor White
Write-Host "Web PHP Helbidea:     http://${SelectedIp}:8085" -ForegroundColor White
Write-Host "Chat Server Helbidea: http://${SelectedIp}:3001" -ForegroundColor White
Write-Host "Odoo ERP Helbidea:    http://${SelectedIp}:8070" -ForegroundColor White
Write-Host "KDS Android TV:       http://${SelectedIp}:8086" -ForegroundColor White
Write-Host "==========================================================" -ForegroundColor Cyan
