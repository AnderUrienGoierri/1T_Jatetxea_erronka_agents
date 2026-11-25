# Exekutatu Administratzaile gisa (PowerShell Admin) etxeko edo beste edozein Wi-Fi-rako DHCP automatikora itzultzeko

$alias = "Wi-Fi"
Write-Host "Wi-Fi egokitzailea DHCP automatikora itzultzen..." -ForegroundColor Cyan
netsh interface ip set address name="$alias" dhcp
netsh interface ip set dns name="$alias" dhcp
Write-Host "Prest! Wi-Fi DHCP automatikoan dago berriro." -ForegroundColor Green
