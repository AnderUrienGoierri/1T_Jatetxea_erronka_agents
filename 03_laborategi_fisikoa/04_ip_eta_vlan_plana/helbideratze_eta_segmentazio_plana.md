# Helbideratze eta Segmentazio Plana (Fisikoa eta Birtuala)

Sare fisikoan eta birtualean aplikatuko den IP helbide, azpisare eta suebaki-arauen diseinu integratua.

---

## 1. Azpisareen eta VLANen Taula Orokorra

| VLAN ID | Izena | Sarea / CIDR | Defektuzko Pasabidea (Gateway - OPNsense) | DHCP Barrutia | Xedea / Funtzioa |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **VLAN 10** | `LAN_JATETXEA` | `192.168.10.0/24` | `192.168.10.1` | `192.168.10.100 - .200` | Barne ekipoak: WinServer, DB, TPV, KDS, Inprimagailua |
| **VLAN 20** | `DMZ_WEB` | `192.168.20.0/24` | `192.168.20.1` | Finkoak (Ez dago DHCPrik) | Web zerbitzari publikoa (Ubuntu Apache / PHP ataria) |
| **VLAN 30** | `WIFI_BEZEROAK` | `192.168.30.0/24` | `192.168.30.1` | `192.168.30.50 - .250` | Bezeroen gailuak (Karta ikusi, mahaitik eskatu) |
| **VLAN 99** | `KUDEAKETA` | `192.168.99.0/24` | `192.168.99.1` | Finkoak | Switch SVI, OPNsense WebGUI, SSH administrazioa |

---

## 2. IP Helbide Estatikoen Esleipena (Erreserbak)

### 2.1. VLAN 10 (LAN)
- `192.168.10.1`: OPNsense Gateway (Subinterfazea `vlan0.10`).
- `192.168.10.10`: Windows Server 2025 (Active Directory, DNS, MySQL DB, .NET Core API).
- `192.168.10.20`: Odoo 17 ERP zerbitzaria (Portu 8069).
- `192.168.10.30`: TPV C# Desktop Kutxa (Barra).
- `192.168.10.40`: KDS Sukaldeko PC / Pantaila (JavaFX).
- `192.168.10.50`: Ticket Inprimagailu Termikoa (ESC/POS Ethernet).

### 2.2. VLAN 20 (DMZ)
- `192.168.20.1`: OPNsense Gateway (Subinterfazea `vlan0.20`).
- `192.168.20.10`: Ubuntu Server 24.04 (Apache Web, PHP 8.2 bezero ataria).

### 2.3. VLAN 99 (Kudeaketa)
- `192.168.99.1`: OPNsense Gateway kudeaketa.
- `192.168.99.2`: Cisco Catalyst 2960 Switch-aren SVI (`interface vlan 99`).
- `192.168.99.10`: Administratzailearen Lanpostua (Admin PC).

---

## 3. OPNsense Suebaki Fisikoaren Trafiko Matrizea (Zero-Trust)

| Jatorria | Helburua | Protokoloa / Ataka | Ekintza | Arrazoia |
| :--- | :--- | :--- | :--- | :--- |
| **WAN / Internet** | DMZ (`192.168.20.10`) | TCP 80, 443 | **ONARTU (PASS)** | Bezeroek kanpotik webgunera sartzeko |
| **WAN / Internet** | LAN (`192.168.10.0/24`)| Edozein | **UKATU (BLOCK)** | Kanpotik ezin da barne sarera sartu |
| **DMZ (`192.168.20.10`)** | LAN API (`192.168.10.10`) | TCP 5000 | **ONARTU (PASS)** | Webguneak APIari kontsultak egiteko |
| **DMZ (`192.168.20.10`)** | LAN DB (`192.168.10.10`)  | TCP 3306 | **UKATU (BLOCK)** | Webak EZ du zuzenean DB ikusten (API bidez soilik) |
| **VLAN 30 (WiFi Bezeroak)**| DMZ (`192.168.20.10`) | TCP 80, 443 | **ONARTU (PASS)** | Jatetxean dauden bezeroek atarira jotzeko |
| **VLAN 30 (WiFi Bezeroak)**| LAN (`192.168.10.0/24`)| Edozein | **UKATU (BLOCK)** | Bezeroek ezin dituzte zerbitzari/TPVak ikusi |
| **VLAN 10 (LAN Langileak)**| Edozein | Edozein | **ONARTU (PASS)** | Langileek eta zerbitzariek komunikazio osoa dute |
