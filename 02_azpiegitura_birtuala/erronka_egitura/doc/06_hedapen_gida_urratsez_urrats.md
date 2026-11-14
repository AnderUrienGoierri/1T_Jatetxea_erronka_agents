# Erronka 1 - Jatetxea Sare Sistema eta Aplikazioen Hedapena
## 06. Hedapen Gida Urratsez Urrats (Runbook)

---

### 1. Sarrera

Gida praktiko honek hiru makina birtualetan (`Erronka-OPNsense`, `Erronka-WinServer2025` eta `Erronka-Ubuntu`) jatetxearen sare-sistema eta aplikazioak zerotik amaierara arte hedatzeko agindu, script eta konfigurazio pauso zehatzak biltzen ditu.

---

### 2. Fasea 1: Sare Birtuala eta OPNsense Interfazeak

#### 2.1. VirtualBox Sare Txartelen Ezarpena
Makinak itzalita daudela edo konfigurazioan, sare-txartelak esleitu:
* **Erronka-OPNsense:**
  * Txartela 1: `NAT` (WAN eta kudeaketa)
  * Txartela 2: `Barne sarea (Internal Network)` -> Izena: `intnet-lan`
  * Txartela 3: `Barne sarea (Internal Network)` -> Izena: `intnet-dmz`
* **Erronka-WinServer2025:**
  * Txartela 1: `Barne sarea (Internal Network)` -> Izena: `intnet-lan`
  * Txartela 2: `NAT` (Host administrazioa: RDP 3391, SSH 2232)
* **Erronka-Ubuntu:**
  * Txartela 1: `Barne sarea (Internal Network)` -> Izena: `intnet-dmz`
  * Txartela 2: `NAT` (Host administrazioa: SSH 2230)

#### 2.2. OPNsense Interfazeak Konfiguratu (WebGUI bidez)
1. Nabigatzailean ireki: `http://127.0.0.1:8443` (Erabiltzailea: `root`, Pasahitza: `Errotagain169232$$`).
2. Joan `Interfaces` -> `Assignments` atalera:
   * **LAN:** Txartel fisikoa esleitu (`em1`). IP estatikoa ezarri: `192.168.10.1 / 24`.
   * **DMZ (OPT1):** Txartel fisikoa esleitu (`em2`). Gaitu interfazea eta IP estatikoa ezarri: `192.168.20.1 / 24`.

---

### 3. Fasea 2: Windows Server 2025 LAN Zerbitzuak

Windows Server-era SSH bidez (`ssh -p 2232 ander@127.0.0.1`) edo RDP bidez (`mstsc /v:127.0.0.1:3391`) sartu eta PowerShell administratzaile gisa exekutatu:

#### 3.1. IP Helbide Estatikoa Ezarri LAN Txartelean:
```powershell
$nic = Get-NetAdapter | Where-Object { $_.Status -eq 'Up' -and $_.Name -notmatch 'Loopback' } | Select-Object -First 1
New-NetIPAddress -InterfaceIndex $nic.ifIndex -IPAddress 192.168.10.10 -PrefixLength 24 -DefaultGateway 192.168.10.1
Set-DnsClientServerAddress -InterfaceIndex $nic.ifIndex -ServerAddresses ("127.0.0.1", "1.1.1.1")
```

#### 3.2. Active Directory eta DNS Rolak Instalatu:
```powershell
Install-WindowsFeature -Name AD-Domain-Services, DNS, DHCP, Web-Server -IncludeManagementTools

# Domeinu baso berria sortu
$secPassword = ConvertTo-SecureString "Errotagain169232$$" -AsPlainText -Force
Install-ADDSForest -DomainName "jatetxea.local" -SafeModeAdministratorPassword $secPassword -InstallDns:$true -Force
```
*(Zerbitzaria automatikoki berrabiaraziko da)*

#### 3.3. AD Antolaketa Unitateak eta Erabiltzaileak Sortu:
```powershell
Import-Module ActiveDirectory
New-ADOrganizationalUnit -Name "Jatetxea" -Path "DC=jatetxea,DC=local"
New-ADOrganizationalUnit -Name "Langileak" -Path "OU=Jatetxea,DC=jatetxea,DC=local"
New-ADOrganizationalUnit -Name "Terminalak" -Path "OU=Jatetxea,DC=jatetxea,DC=local"

# Langileak gehitu
$pw = ConvertTo-SecureString "Errotagain169232$$" -AsPlainText -Force
New-ADUser -Name "zerbitzari1" -AccountPassword $pw -Enabled $true -Path "OU=Langileak,OU=Jatetxea,DC=jatetxea,DC=local"
New-ADUser -Name "sukaldari1" -AccountPassword $pw -Enabled $true -Path "OU=Langileak,OU=Jatetxea,DC=jatetxea,DC=local"
```

#### 3.4. DHCP Esparrua Konfiguratu:
```powershell
Add-DhcpServerV4Scope -Name "Jatetxea-LAN" -StartRange 192.168.10.100 -EndRange 192.168.10.200 -SubnetMask 255.255.255.0 -State Active
Set-DhcpServerV4OptionValue -OptionId 3 -Value 192.168.10.1
Set-DhcpServerV4OptionValue -OptionId 6 -Value 192.168.10.10
Set-DhcpServerV4OptionValue -OptionId 15 -Value "jatetxea.local"
```

#### 3.5. DNS Erregistroak DMZko Zerbitzarirako Sortu:
```powershell
Add-DnsServerResourceRecordA -Name "api" -ZoneName "jatetxea.local" -IPv4Address "192.168.20.10"
Add-DnsServerResourceRecordA -Name "web" -ZoneName "jatetxea.local" -IPv4Address "192.168.20.10"
```

---

### 4. Fasea 3: Ubuntu Server DMZ Zerbitzuak

Ubuntu zerbitzarian SSH bidez sartu (`ssh -p 2230 ander@127.0.0.1`):

#### 4.1. IP Estatikoa Ezarri Netplan bidez (`/etc/netplan/01-netcfg.yaml`):
```yaml
network:
  version: 2
  renderer: networkd
  ethernets:
    enp0s3:
      dhcp4: no
      addresses:
        - 192.168.20.10/24
      routes:
        - to: default
          via: 192.168.20.1
      nameservers:
        addresses: [192.168.10.10, 1.1.1.1]
```
Aplikatu: `sudo netplan apply`

#### 4.2. MySQL 8.0 Instalatu eta Datu-Basea Kargatu:
```bash
sudo apt update && sudo apt install -y mysql-server
sudo systemctl enable --now mysql

# Datu-basea eta erabiltzailea sortu
sudo mysql -e "CREATE DATABASE IF NOT EXISTS jatetxea_db CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;"
sudo mysql -e "CREATE USER IF NOT EXISTS 'jatetxea_user'@'localhost' IDENTIFIED BY 'Errotagain169232$$';"
sudo mysql -e "GRANT ALL PRIVILEGES ON jatetxea_db.* TO 'jatetxea_user'@'localhost';"
sudo mysql -e "FLUSH PRIVILEGES;"

# Eskema inportatu
sudo mysql jatetxea_db < /home/ander/datu_basea_schema.sql
```

#### 4.3. .NET 10 Runtime eta ASP.NET Core APIa Hedatu:
```bash
# .NET 10 paketeak instalatu
sudo apt install -y dotnet-runtime-10.0 aspnetcore-runtime-10.0

# API aplikazioa kokatu
sudo mkdir -p /var/www/jatetxea_api
sudo cp -r /home/ander/backend_api_publish/* /var/www/jatetxea_api/
sudo chown -R www-data:www-data /var/www/jatetxea_api

# Systemd daemon zerbitzua konfiguratu
sudo tee /etc/systemd/system/jatetxea-api.service > /dev/null << 'EOF'
[Unit]
Description=Jatetxea ASP.NET Core REST API
After=network.target mysql.service

[Service]
WorkingDirectory=/var/www/jatetxea_api
ExecStart=/usr/bin/dotnet /var/www/jatetxea_api/Jatetxea.Api.dll
Restart=always
RestartSec=10
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5000

[Install]
WantedBy=multi-user.target
EOF

sudo systemctl daemon-reload
sudo systemctl enable --now jatetxea-api.service
```

#### 4.4. Apache 2.4 eta Web PHP Hedatu:
```bash
sudo apt install -y apache2 php8.2 php8.2-mysql php8.2-curl
sudo a2enmod proxy proxy_http headers rewrite

# Web fitxategiak kopiatu
sudo cp -r /home/ander/web_php/* /var/www/html/web_php/
sudo chown -R www-data:www-data /var/www/html/web_php

# VirtualHost gaitu (doc/04 dokumentuan ikusitako jatetxea.conf erabiliz)
sudo systemctl restart apache2
```

---

### 5. Fasea 4: OPNsense Suebaki eta NAT Arauak

OPNsense WebGUIn (`http://127.0.0.1:8443`):
1. **Firewall -> Rules -> DMZ:**
   * Araua 1: `Action: Block | Source: DMZ net | Destination: LAN net` (**DMZtik LANera debekatuta**).
   * Araua 2: `Action: Pass | Source: DMZ net | Destination: Any | Port: 53, 80, 443` (Eguneraketak).
2. **Firewall -> Rules -> LAN:**
   * Araua 1: `Action: Pass | Source: LAN net | Destination: 192.168.20.10 | Port: 80, 443, 5000` (Web eta APIra sarbidea).
   * Araua 2: `Action: Pass | Source: LAN net | Destination: Any` (Interneterako irteera orokorra).
3. **Firewall -> NAT -> Port Forward:**
   * Araua 1: `Interface: WAN | Proto: TCP | Port: 80 -> Target: 192.168.20.10:80`
   * Araua 2: `Interface: WAN | Proto: TCP | Port: 443 -> Target: 192.168.20.10:443`
   * Araua 3: `Interface: WAN | Proto: TCP | Port: 5000 -> Target: 192.168.20.10:5000`

---

### 6. Fasea 5: Egiaztapen eta Integrazio Probak

| Proba | Komandoa / Ekintza | Espero den Emaitza | Egoera |
| :--- | :--- | :--- | :---: |
| **1. LAN -> DMZ API Ping/HTTP** | Windows Server-etik: `curl http://api.jatetxea.local/api/v1/platerak` | HTTP 200 OK eta plateren JSON zerrenda | Egiaztatua |
| **2. DMZ -> LAN Blokeoa** | Ubuntu zerbitzaritik: `ping 192.168.10.10` edo `nc -zv 192.168.10.10 445` | Timeout / Drop (OPNsense-k blokeatuta) | Egiaztatua |
| **3. Web Nabigazioa** | Nabigatzailetik: `http://192.168.20.10` | Jatetxeko Karta eta Erreserben orria | Egiaztatua |
| **4. TPV C# Desktop** | Windows bezeroan: `dotnet run --project desktop_csharp` | Mahaien mapa kargatuta eta PIN bidezko sarbidea | Egiaztatua |
| **5. JavaFX Sukaldea** | Java bezeroan: `mvn javafx:run` (desktop_javafx) | KDS eta BDS pantailak eskaera aktiboekin | Egiaztatua |
| **6. Transakzio Pesimista** | Plater bat eskatu stockik gabe | HTTP 409 Conflict eta Rollback atomikoa | Egiaztatua |
