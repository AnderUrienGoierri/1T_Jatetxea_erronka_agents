# Hardware Inbentario Fisikoa - Goierri Eskola Laborategia

Jatetxearen azpiegitura erreala laborategian muntatzeko erabiliko diren gailu fisikoen fitxa teknikoa.

---

## 1. Sareko Ekipamendua (Networking)

### 1.1. Suebaki / Bideratzaile Nagusia (Firewall / Router)
- **Gailua:** OPNsense Hardware Appliance / Mini PC dedikatua (edo PC dorre 3 NIC txartelekin).
- **Interfaze Fisikoak:**
  - `NIC 1 (WAN)`: Goierri Eskolako laborategiko sare nagusira (DHCP bidez Interneteko sarbidea).
  - `NIC 2 (LAN/DMZ Trunk)`: 802.1Q trunk kablea Cisco Switch-eko GigabitEthernet 0/1 portura.
  - `NIC 3 (Kudeaketa Zuzena / Failover)`: Larrialdietarako konexio zuzena (192.168.1.1).
- **Sistema Eragilea:** OPNsense 24.x / FreeBSD 14.

### 1.2. Kommutadore Kudeatua (Managed Switch)
- **Eredua:** Cisco Catalyst 2960-X / 2960-Plus (24 edo 48 portu 10/100/1000 + 2 SFP).
- **Funtzioak:**
  - IEEE 802.1Q VLAN Trunking.
  - Port Security (MAC helbideen iragazketa eraso fisikoak ekiditeko).
  - Spanning Tree Protocol (STP / Rapid-PVST+) bukleak saihesteko.
  - DHCP Snooping eta Dynamic ARP Inspection (DAI) spoofing ekiditeko.

### 1.3. Haririk gabeko Sarbide-Puntua (Wireless Access Point)
- **Eredua:** Cisco Aironet / Ubiquiti UniFi AP / TP-Link Omada EAP (VLAN gaitasuna duena).
- **SSID-ak:**
  - `Jatetxea_Langileak`: WPA2/WPA3 Enterprise (edo PSK sendoa), VLAN 10-era bideratua. Zerbitzarien PDA eta tabletentzat.
  - `Jatetxea_Bezeroak`: WPA2/WPA3 Personal (edo Open Captive Portal), VLAN 30 (Guest). DMZ edo Internetera soilik baimendua, LAN sarbiderik gabe.

---

## 2. Zerbitzariak eta Lanpostu Fisikoak (Compute Endpoints)

| Gailu Zenbakia | Rola Jatetxean | OS / Software Nagusia | Sare Portua (Switch) | IP Fisikoa |
| :--- | :--- | :--- | :--- | :--- |
| **PC-01 (LAN Server)** | Windows Server 2025: AD, DNS, DHCP, MySQL DB, .NET API | Windows Server 2025 Standard | FastEthernet 0/2 (VLAN 10) | `192.168.10.10` |
| **PC-02 (DMZ Server)** | Web Zerbitzaria (Apache, PHP 8.2 web ataria) | Ubuntu Server 24.04 LTS | FastEthernet 0/10 (VLAN 20) | `192.168.20.10` |
| **PC-03 (TPV Kutxa)** | Barrako Kutxa / TPV (C# WinForms TPV aplikazioa) | Windows 11 Pro | FastEthernet 0/3 (VLAN 10) | `192.168.10.30` |
| **PC-04 (Sukaldea KDS)** | Sukaldeko Pantaila (JavaFX KDS aplikazioa) | Ubuntu / Windows 11 | FastEthernet 0/4 (VLAN 10) | `192.168.10.40` |
| **PC-05 (Kudeaketa/Admin)** | Sistemen administratzailearen lanpostua | Windows / Linux | FastEthernet 0/24 (VLAN 99) | `192.168.99.10` |

---

## 3. Jatetxeko Hardware Berezia eta Periferikoak

### 3.1. Ticket Inprimagailu Termikoa (POS Printer)
- **Mota:** 80mm ESC/POS inprimagailu termikoa (Epson TM-T20III edo antzekoa).
- **Konexioa:** Ethernet (IP finkoa LANean) edo USB konexio zuzena PC-03 TPV kutxara.
- **Inprimatze funtzioa:** C# WinForms eta Flutter Bezeroak sortutako ESC/POS formatuko tiketak inprimatzea ordainketa burutzean.

### 3.2. PDA / Tableta Mugikorrak (Zerbitzariak eta Bezeroak)
- **Gailu Erreala:** Samsung Galaxy (ADB bidez konektatutako probako gailua: `RZGL4074D2A`).
- **Aplikazioak instalatuta:**
  - `mobile_zerbitzaria.apk`: Mahaiak ireki, eskaerak hartu, egoerak ikusi.
  - `mobile_bezeroa.apk`: Menua ikusi, mahaiak erreserbatu, norberak ordaindu.
- **Konexioa:** `Jatetxea_Langileak` WiFi sarera (VLAN 10).

### 3.3. Sukaldeko TV Pantaila (KDS Display)
- **Gailua:** HDMI telebista / monitore handia sukaldeko horman.
- **Konexioa:** PC-04-ren HDMI irteeratik, edo Android TV Box bat WiFi bidez Nginx KDS atarira konektatua (`http://192.168.10.10:8086`).
