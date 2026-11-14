$vbox = "C:\Program Files\Oracle\VirtualBox\VBoxManage.exe"

Write-Host ">>> Makina birtualak modu seguruan itzaltzen..." -ForegroundColor Yellow

$vms = @("Erronka-Ubuntu", "Erronka-OPNsense", "Erronka-WinServer2025")
foreach ($vm in $vms) {
    Write-Host "Itzaltzen: $vm..."
    & $vbox controlvm $vm acpipowerbutton
}

Write-Host "Itzaltzeko seinalea bidali zaie hirurei." -ForegroundColor Green
