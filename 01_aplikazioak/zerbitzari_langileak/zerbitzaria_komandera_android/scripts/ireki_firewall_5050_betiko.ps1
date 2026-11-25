# Exekutatu PowerShell Administratzaile gisa (behin bakarrik!)
# Windows Firewall-eko "Jatetxea API - Innovatek Wi-Fi" araua atzoko IP zaharrera (10.100.30.101) mugatuta zegoen.
# Honek LocalAddress = Any eta RemoteAddress = LocalSubnet ezartzen du, edozein Wi-Fi lokaletan eta edozein IP-rekin betiko funtzionatzeko!

$ruleName = "Jatetxea API - Innovatek Wi-Fi"
if (Get-NetFirewallRule -DisplayName $ruleName -ErrorAction SilentlyContinue) {
    Set-NetFirewallRule -DisplayName $ruleName -LocalAddress Any -RemoteAddress LocalSubnet -Profile Private,Domain
    Write-Host "OK! '$ruleName' araua eguneratu da (LocalAddress=Any, RemoteAddress=LocalSubnet)." -ForegroundColor Green
} else {
    New-NetFirewallRule -DisplayName $ruleName -Direction Inbound -Protocol TCP -LocalPort 5050 -LocalAddress Any -RemoteAddress LocalSubnet -Profile Private,Domain -Action Allow | Out-Null
    Write-Host "OK! '$ruleName' firewall araua sortu da (TCP 5050)." -ForegroundColor Green
}

Get-NetFirewallRule -DisplayName $ruleName | Get-NetFirewallAddressFilter | Format-List LocalAddress, RemoteAddress
