# Exekutatu Administratzaile gisa (PowerShell Admin) INN-WPRIVATE sarean IP estatikoa ezartzeko
# OHARRA: Etxera joatean, exekutatu bueltatu_dhcp.ps1 DHCP automatikora itzultzeko!

$alias = "Wi-Fi"
$ip = "10.100.30.112"
$prefix = 24
$gateway = "10.100.30.1"
$dns = @("192.168.5.150", "8.8.8.8")

Write-Host "INN-WPRIVATE Wi-Fi IP estatikoa konfiguratzen: $ip ..." -ForegroundColor Cyan
netsh interface ip set address name="$alias" static $ip 255.255.255.0 $gateway
netsh interface ip set dns name="$alias" static $dns[0]
netsh interface ip add dns name="$alias" $dns[1] index=2

# Ziurtatu 5050 ataka irekita dagoela Windows Firewall-ean
if (-not (Get-NetFirewallRule -DisplayName "T1 Jatetxea API 5050" -ErrorAction SilentlyContinue)) {
    New-NetFirewallRule -DisplayName "T1 Jatetxea API 5050" -Direction Inbound -Protocol TCP -LocalPort 5050 -Action Allow | Out-Null
    Write-Host "Firewall araua sortu da: TCP 5050 irekita!" -ForegroundColor Green
}

Write-Host "Prest! Zure PCak beti $ip izango du INN-WPRIVATE sarean." -ForegroundColor Green
Get-NetIPConfiguration -InterfaceAlias $alias | Select-Object InterfaceAlias, IPv4Address, IPv4DefaultGateway
