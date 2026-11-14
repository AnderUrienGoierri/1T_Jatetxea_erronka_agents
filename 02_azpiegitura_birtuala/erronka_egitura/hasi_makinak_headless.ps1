$vbox = "C:\Program Files\Oracle\VirtualBox\VBoxManage.exe"

Write-Host ">>> Makina birtualak ixilpean (headless) abiarazten..." -ForegroundColor Cyan

$running = [string](& $vbox list runningvms)

if (-not ($running -match "Erronka-Ubuntu")) {
    Write-Host "Abiarazten: Erronka-Ubuntu..."
    & $vbox startvm "Erronka-Ubuntu" --type headless
} else {
    Write-Host "Erronka-Ubuntu martxan zegoen dagoeneko." -ForegroundColor Green
}

if (-not ($running -match "Erronka-OPNsense")) {
    Write-Host "Abiarazten: Erronka-OPNsense..."
    & $vbox startvm "Erronka-OPNsense" --type headless
} else {
    Write-Host "Erronka-OPNsense martxan zegoen dagoeneko." -ForegroundColor Green
}

if (-not ($running -match "Erronka-WinServer2025")) {
    Write-Host "Abiarazten: Erronka-WinServer2025..."
    & $vbox startvm "Erronka-WinServer2025" --type headless
} else {
    Write-Host "Erronka-WinServer2025 martxan zegoen dagoeneko." -ForegroundColor Green
}

Write-Host "`n>>> Makinak abiarazita. Sareko portuak egiaztatzen..." -ForegroundColor Yellow

$ports = @(
    @{ Name = "Ubuntu SSH (2230)"; Port = 2230 },
    @{ Name = "OPNsense SSH (2231)"; Port = 2231 },
    @{ Name = "OPNsense WebGUI (8443)"; Port = 8443 },
    @{ Name = "WinServer SSH (2232)"; Port = 2232 },
    @{ Name = "WinServer RDP (3391)"; Port = 3391 }
)

foreach ($p in $ports) {
    $ok = $false
    for ($i = 0; $i -lt 15; $i++) {
        $t = Test-NetConnection -ComputerName 127.0.0.1 -Port $p.Port -WarningAction SilentlyContinue
        if ($t.TcpTestSucceeded) {
            $ok = $true
            break
        }
        Start-Sleep -Seconds 2
    }
    if ($ok) {
        Write-Host "  [OK] $($p.Name) prest dago!" -ForegroundColor Green
    } else {
        Write-Host "  [..] $($p.Name) oraindik abiarazten ari da..." -ForegroundColor Yellow
    }
}

Write-Host "`n>>> Dena prest! Orain terminaleko profilak ireki ditzakezu." -ForegroundColor Green
