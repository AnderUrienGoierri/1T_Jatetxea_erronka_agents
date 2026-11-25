# Hardware Inbentario Fisikoa - Goierri Eskola Laborategia

Jatetxearen azpiegitura erreala laborategian muntatzeko erabilitako eta egiaztatutako gailu fisikoen fitxa teknikoa.

---

## 1. Sareko Ekipamendua (Networking — Muntatuta eta Aktibo)

### 1.1. Suebaki / Bideratzaile Nagusia (Firewall / Router)
- **Ostalari-izena:** `OPNsense.internal`
- **Sistema Eragilea:** `OPNsense 25.7 (amd64)` / `FreeBSD 14.3-RELEASE-p1`
- **Interfaze Fisikoak (3 NIC independente rack-eko switch-era):**
  - `WAN (re2)`: `192.168.115.159/24` (DHCP Goierri Eskolatik) | MAC: `00-18-e7-16-65-0c` (Cameo Communications) -> **VLAN 30 (WAN)**
  - `LAN (re1)`: `192.168.10.1/24` (Estatikoa, Gateway LAN) | MAC: `bc-ee-7b-77-22-ec` (ASUSTek) -> **VLAN 10 (LAN)**
  - `OPT1 (re0)`: `192.168.20.1/24` (Estatikoa, Gateway DMZ) | MAC: `00-e0-ff-68-0f-ff` (Security Dynamics) -> **VLAN 20 (DMZ)**

### 1.2. Mahai Gaineko Kommutadorea (1. Switch-a)
- **Eredua:** **Cisco SG200-50** (*50-Port Gigabit Smart Switch*)
- **Kudeaketa:** WebGUI bakarrik (ez du SSH/CLIrik). IP lehenetsia: `192.168.1.254` (VLAN 1), kredentzialak `cisco` / `cisco`.
- **Etiketatutako Portu-Taldeak:**
  - `LAN`: Ezkerreko blokea (1–6 / 25–30) -> **VLAN 10** (Admin eta taldekideen portatilak).
  - `DMZ`: Erdiko blokea (7–12 / 31–36) -> **VLAN 20**.
  - `WAN`: Eskuineko blokea (13–20 / 37–43) -> **VLAN 30** (Pareteko Ethernet hartunerako lotura).

### 1.3. Gelako Rack-eko Kommutadorea (2. Switch-a)
- **Kokapena:** Gelako rack-ean (`SW`).
- **Funtzioa:** Goierri Eskolako switch nagusia (`SW Goierri`), pareteko hartunea, OPNsense-ren 3 NICak, Windows Server 2025 (LAN) eta Ubuntu Server (DMZ) elkarlotzen ditu VLAN bidez (`VLAN 10`, `VLAN 20`, `VLAN 30` eta `Trunk`).

### 1.4. Goierri Eskolako Sare Nagusia (Uplink)
- **Gateway / DHCP:** `192.168.115.254` | MAC: `00-09-0f-09-00-0d` (Fortinet).

---

## 2. Zerbitzariak eta Lanpostuak (Egiaztatutakoak)

| Ekipoa / Hostname | Rola Laborategian | Sistema Eragilea | Interfazea / MAC | VLAN / Sarea | IP Fisikoa |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **`WIN-U1CN1H8S33S`** (MiniPC Rack) | Barne Zerbitzaria (AD, DNS, DB, .NET API, RDP) | Windows Server 2025 (ES) | `Ethernet` (`00-e0-4c-43-24-63`, Realtek) | VLAN 10 (`LAN`) | `192.168.10.19/24` |
| **`1taldea-dmz`** (Rack) | DMZ Web Zerbitzaria (SSH aktibo, Apache/PHP) | Ubuntu Server 26.04.1 LTS (Linux 7.0.0) | `enp1s0` (`00-e0-4c-22-a0-9c`)<br>`wlp2s0` (`f4-3c-3b-b4-99-5f`, DOWN) | VLAN 20 (`DMZ`) | `192.168.20.10/24` |
| **`ANDER-SGB5PRO`** (Admin Portatila) | Kudeaketa eta Garapen Lanpostua | Windows 11 | `Ethernet 5` — Realtek USB GbE #3 (`34-99-71-E4-43-ED`, 1 Gbps) | VLAN 10 (`LAN`) | `192.168.10.99/24` |
| **Taldekideen Portatilak** | Garapen eta Proba Lanpostuak | Windows / Linux | Ethernet (Cisco SG200-50 `LAN` atakak) | VLAN 10 (`LAN`) | `192.168.10.101 - .109` |

---

## 3. Hurrengo Fasea: Jatetxeko Hardware Berezia eta Periferikoak (Aurreikusitakoak)

| Gailua | Rola Jatetxean | Konexioa / VLAN | Aurreikusitako IPa |
| :--- | :--- | :--- | :--- |
| **WiFi Access Point (AP)** | `Jatetxea_Langileak` (VLAN 10) / `Jatetxea_Bezeroak` (Guest) | Trunk / PoE | — |
| **TPV Kutxa (PC-03)** | Barrako Kutxa (C# WinForms TPV) | VLAN 10 (`LAN`) | `192.168.10.30` |
| **Sukaldeko KDS (PC-04 / TV)** | Sukaldeko Pantaila (JavaFX / Nginx KDS) | VLAN 10 (`LAN`) | `192.168.10.40` |
| **Ticket Inprimagailu Termikoa** | 80mm ESC/POS tiketak | VLAN 10 (`LAN`) / USB | `192.168.10.50` |
| **PDA / Mugikorra (Samsung)** | `mobile_zerbitzaria.apk` / `mobile_bezeroa.apk` (`RZGL4074D2A`) | WiFi VLAN 10 | DHCP |
